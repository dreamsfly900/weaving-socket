

using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Frankfort.Threading.Internal;









namespace Frankfort.Threading
{
    public delegate void ThreadPoolSchedulerEvent(IThreadWorkerObject[] finishedObjects);
    public delegate void ThreadedWorkCompleteEvent(IThreadWorkerObject finishedObject);


    /// <summary>
    /// This is the most important class of the framework. It starts & stops threads, caps the amount of threads running at the same time, handles Mainthread completion-delegates, etc.
    /// </summary>
    public class ThreadPoolScheduler : MonoBehaviour
    {

        //--------------- public Session Variables --------------------
        public bool DebugMode = false;
        public bool ForceToMainThread = false;
        public float WaitForSecondsTime = 0.001f; //Wait 1 ms per tick!
        
        public bool isBusy
        {
            get
            {
                return _shedularBusy;
            }
        }

        public float Progress
        {
            get
            {
                if (workData == null || workData.workerPackages == null || workData.workerPackages.Length == 0)
                    return 1f;

                //return (float)GetHandledFinishedPackages().Length / (float)workData.workerPackages.Length;
                int finishedPackages = 0;
                int i = workData.workerPackages.Length;
                while(--i > -1)
                {
                    if (workData.workerPackages[i].finishedWorking)
                        finishedPackages++;
                }

                return (float)finishedPackages / (float)workData.workerPackages.Length;
            }
        }
        //--------------- public Session Variables --------------------




        //--------------- PrivateSession Variables --------------------
        private bool _providerThreadBusy;
        private bool _shedularBusy;
        private bool _isAborted;
        private ASyncThreadWorkData workData;
        private Thread providerThread;
        private int workObjectIndex;
        private ThreadPoolSchedulerEvent onCompleteCallBack;
        private ThreadedWorkCompleteEvent onWorkerObjectDoneCallBack;
        private bool safeMode;
        //--------------- Private Session Variables --------------------








        //--------------------------------------- UNITY MONOBEHAVIOUR COMMANDS --------------------------------------
        //--------------------------------------- UNITY MONOBEHAVIOUR COMMANDS --------------------------------------
        #region UNITY MONOBEHAVIOUR COMMANDS

        protected virtual void Awake()
        {
            MainThreadWatchdog.Init();
            MainThreadDispatcher.Init();
            UnityActivityWatchdog.Init();
        }

        protected virtual void OnApplicationQuit()
        {
            Debug.Log("ThreadPoolScheduler.OnApplicationQuit!");
            AbortASyncThreads();    
        }

        protected virtual void OnDestroy()
		{
			Debug.Log("ThreadPoolScheduler.OnDestroy!");
            AbortASyncThreads();
        }
        #endregion
        //--------------------------------------- UNITY MONOBEHAVIOUR COMMANDS --------------------------------------
        //--------------------------------------- UNITY MONOBEHAVIOUR COMMANDS --------------------------------------
			












        //--------------------------------------- UNITY COROUTINE & PROVIDER-THREAD IMPLEMENTATION --------------------------------------
        //--------------------------------------- UNITY COROUTINE & PROVIDER-THREAD IMPLEMENTATION --------------------------------------
        #region UNITY COROUTINE & PROVIDER-THREAD IMPLEMENTATION

        /// <summary>
        /// Unlike "StartMultithreadedWorkloadExecution", you will have to build your own IThreadWorkerObject.
        /// Downside: It requires some extra work. Upside: you got more controll over what goes in and comes out
        /// Infact: You can create you own polymorphed IThreadWorkerObject-array, each ellement being a completely different type. For example: the statemachines of enemies are IThreadWorkerObject's and the array contains completely different classes with enemies/AI-behaviours.
        /// </summary>
        /// <param name="workerObjects">An array of IThreadWorkerObject objects to be handled by the threads. If you want multiple cores/threads to be active, make sure that the number of IThreadWorkerObject's proves matches/exeeds your preferred number maxWorkingThreads. </param>
        /// <param name="onComplete">Fired when all re-packaged workLoad-objects are finished computing</param>
        /// <param name="onPackageExecuted">Fires foreach finished re-packaged set of workLoad-object</param>
        /// <param name="maxThreads"> Lets you choose how many threads will be run simultaneously by the threadpool. Default: -1 == number of cores minus one, to make sure the MainThread has at least one core to run on. (quadcore == 1 core Mainthread, 3 cores used by the ThreadPoolScheduler)</param>
        /// <param name="scheduler">If Null, a new ThreadPoolScheduler will be instantiated.</param>
        /// <param name="safeMode">Executes all the computations within try-catch events, logging it the message + stacktrace</param>
        public void StartASyncThreads(IThreadWorkerObject[] workerObjects, ThreadPoolSchedulerEvent onCompleteCallBack, ThreadedWorkCompleteEvent onPackageExecuted = null, int maxThreads = -1, bool safeMode = true)
        {
            if (_shedularBusy)
            {
                Debug.LogError("You are trying the start a new ASync threading-process, but is still Busy!");
                return;
            }

            if (workerObjects == null || workerObjects.Length == 0)
            {
                Debug.LogError("Please provide an Array with atleast \"IThreadWorkerObject\"-object!");
                return;
            }

            _isAborted = false;
            _shedularBusy = true;
            _providerThreadBusy = true;
            this.onCompleteCallBack = onCompleteCallBack;
            this.onWorkerObjectDoneCallBack = onPackageExecuted;
                
            if (!ForceToMainThread)
            {   
                //--------------- Start Waiting for the Provider-thread to complete --------------------
                StartCoroutine("WaitForCompletion");
                workData = new ASyncThreadWorkData(workerObjects, safeMode, maxThreads);
                providerThread = new Thread(new ThreadStart(InvokeASyncThreadPoolWork));
                providerThread.Start();
                //--------------- Start Waiting for the Provider-thread to complete --------------------
            }
            else
            {
                //--------------- Execute all work in one bunch! --------------------
                StartCoroutine(WaitAndExecuteWorkerObjects(workerObjects));
                //--------------- Execute all work in one bunch! --------------------
            }
        }




        private IEnumerator WaitAndExecuteWorkerObjects(IThreadWorkerObject[] workerObjects)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < workerObjects.Length; i++)
            {
                workerObjects[i].ExecuteThreadedWork();

                if (onWorkerObjectDoneCallBack != null)
                    onWorkerObjectDoneCallBack(workerObjects[i]);
            }

            _shedularBusy = false;
            _providerThreadBusy = false;

            if (onCompleteCallBack != null)
                onCompleteCallBack(workerObjects);
        }




        private IEnumerator WaitForCompletion()
        {
            if (DebugMode)
                Debug.Log(" ----- WaitForCompletion: " + Thread.CurrentThread.ManagedThreadId);
            
			while (!_isAborted)
			{
				//After waiting a while, in the meantime it might have finished itself, or got aborted!
				yield return new WaitForSeconds(WaitForSecondsTime);

				if(_isAborted)
					break;

                //--------------- fire events while still working --------------------
                int finishedObjectsCount = GetFinishedPackagesCount();
                if (finishedObjectsCount == workData.workerPackages.Length)
                    break;
        
                int unhandledPackagesCount = GetUnhandledFinishedPackagesCount();
                if (DebugMode)
                    Debug.Log(" ----- unhandledPackages: " + unhandledPackagesCount + " ( out of: " + finishedObjectsCount + " completed so far...)");

                if (unhandledPackagesCount > 0)
                {
                    foreach (ThreadWorkStatePackage package in workData.workerPackages)
                    {
                        if (package.finishedWorking && !package.eventFired)
                        {
                            if (onWorkerObjectDoneCallBack != null)
                                onWorkerObjectDoneCallBack(package.workerObject);

                            package.eventFired = true;
                        }
                    }
                }
                //--------------- fire events while still working --------------------
			}

			if(!_isAborted)
			{
				if (DebugMode)
	                Debug.Log(" ----- Coroutine knows its done!");

	            IThreadWorkerObject[] workedObjects = GetWorkerObjectsFromPackages();
	            
	            workData.Dispose();
	            workData = null;
	            _shedularBusy = false;
	            
	            if (onCompleteCallBack != null)
	                onCompleteCallBack(workedObjects);
			}
        }
        
        #endregion
        //--------------------------------------- UNITY COROUTINE & PROVIDER-THREAD IMPLEMENTATION --------------------------------------
        //--------------------------------------- UNITY COROUTINE & PROVIDER-THREAD IMPLEMENTATION --------------------------------------











        //--------------------------------------- EXTRA COMMANDS & ACTIONS --------------------------------------
        //--------------------------------------- EXTRA COMMANDS & ACTIONS --------------------------------------
        #region EXTRA COMMANDS & ACTIONS

        /// <summary>
        /// Aborts all worker processes currently queued.
        /// </summary>
        /// <param name="sleepTillAborted">if true: Makes sure that after invoking "AbortASyncThreads" the ThreadPoolSheduler is available again, but halts the MainThread while waiting for the other threads to finish</param>
        public void AbortASyncThreads()
        {
            if (!_providerThreadBusy)
                return;

            _isAborted = true;
            StopCoroutine("WaitForCompletion");

            if(workData != null && workData.workerPackages != null)
            {
                lock (workData.workerPackages)
                {
                    foreach (ThreadWorkStatePackage package in workData.workerPackages)
                    {
                        if (package.running && !package.finishedWorking)
                            package.workerObject.AbortThreadedWork();
                    }
                }
            }
			
			if (providerThread != null && providerThread.IsAlive)
			{
				Debug.Log("ThreadPoolScheduler.AbortASyncThreads - Interrupt!");
				providerThread.Interrupt();
				providerThread.Join();
			}
			else
			{
				Debug.Log("ThreadPoolScheduler.AbortASyncThreads!");
			}

			_providerThreadBusy = false;
		}
		
		#endregion
        //--------------------------------------- EXTRA COMMANDS & ACTIONS --------------------------------------
        //--------------------------------------- EXTRA COMMANDS & ACTIONS --------------------------------------
			



        






        //--------------------------------------- .NET THREADPOOL IMPLEMENTATION --------------------------------------
        //--------------------------------------- .NET THREADPOOL IMPLEMENTATION --------------------------------------
        #region .NET THREADPOOL IMPLEMENTATION

        /// <summary>
        /// This method is the work-provider-method. It makes sure the .NET threadpool has things to do...
        /// This method is NOT invoked by the mainThread, therefor is safe to use WaitHandle.WaitAll / WaitAny without halting Unity's gameThread!
        /// </summary>
        public void InvokeASyncThreadPoolWork()
        {
            UnityActivityWatchdog.SleepOrAbortIfUnityInactive();

            int totalWork = workData.workerPackages.Length;
            int startBurst = Mathf.Clamp(workData.maxWorkingThreads, 1, totalWork);

            if (DebugMode)
                Debug.Log(" ----- InvokeASyncThreadPoolWork. startBurst: " + startBurst + ", totalWork: " + totalWork);
            
            //--------------- Initial Startup burst --------------------
            for (int i = 0; i < startBurst && !_isAborted; i++)
            {
                //Add to .NET ThreadPool
                if (workData.workerPackages[i] != null)
                {
                    workData.workerPackages[i].started = true;
                    ThreadPool.QueueUserWorkItem(workData.workerPackages[i].ExecuteThreadWork, i);
                }
            }
            //--------------- Initial Startup burst ---------------s-----


            if (DebugMode)
                Debug.Log(" ----- Burst with WorkerObjects being processed!");

            //--------------- Create a new Thread to keep the Threadpool running  & cores saturated! --------------------
            workObjectIndex = startBurst; //at this point the amount of running WorkObjects/Threads is equal to the startBurst;
            while (workObjectIndex < totalWork && !_isAborted)
            {
                UnityActivityWatchdog.SleepOrAbortIfUnityInactive();

                AutoResetEvent[] startedEvents = GetStartedPackageEvents();
                if (startedEvents.Length > 0)
                    WaitHandle.WaitAny(startedEvents);
                
                workData.workerPackages[workObjectIndex].started = true;
                ThreadPool.QueueUserWorkItem(workData.workerPackages[workObjectIndex].ExecuteThreadWork, workObjectIndex);
                workObjectIndex++;
            } 
            //--------------- Create a new Thread to keep the Threadpool running & cores saturated! --------------------


            if (DebugMode)
                Debug.Log(" ----- all packages fed to the pool!");
            
            //--------------- Wait till all are finished! --------------------
            //All WorkObjects have been set to work, but the last few Threads might still be pending!
            AutoResetEvent[] pendingEvents = GetStartedPackageEvents();
            if (pendingEvents.Length > 0)
            {
                UnityActivityWatchdog.SleepOrAbortIfUnityInactive();
                WaitHandle.WaitAll(pendingEvents);
            }
            //--------------- Wait till all are finished! --------------------


            if (DebugMode)
                Debug.Log(" ----- PROVIDER THREAD DONE");
    
            //DONE!
            _providerThreadBusy = false;
        }

        #endregion
        //--------------------------------------- .NET THREADPOOL IMPLEMENTATION --------------------------------------
        //--------------------------------------- .NET THREADPOOL IMPLEMENTATION --------------------------------------













        //--------------------------------------- MISC --------------------------------------
        //--------------------------------------- MISC --------------------------------------
        #region MISC

        private AutoResetEvent[] GetStartedPackageEvents()
        {
            List<AutoResetEvent> result = new List<AutoResetEvent>();
            for (int i = 0; i < workData.workerPackages.Length; i++)
            {
                ThreadWorkStatePackage package = workData.workerPackages[i];
                if(package.started && !package.finishedWorking)
                    result.Add(package.waitHandle);
            }
            return result.ToArray();
		}

        
        private IThreadWorkerObject[] GetWorkerObjectsFromPackages()
        {
            if (workData == null || workData.workerPackages == null)
                return new IThreadWorkerObject[0];

            IThreadWorkerObject[] result = new IThreadWorkerObject[workData.workerPackages.Length];
            int i = workData.workerPackages.Length;
            while (--i > -1)
                result[i] = workData.workerPackages[i].workerObject;

            return result;
        }

        public int GetFinishedPackagesCount()
        {
            if (workData == null || workData.workerPackages == null)
                return 0;

            int count = 0;
            int i = workData.workerPackages.Length;
            while (--i > -1)
            {
                if (workData.workerPackages[i].finishedWorking)
                    count++;
            }
            return count;
        }

        public int GetUnhandledFinishedPackagesCount()
        {
            if (workData == null || workData.workerPackages == null)
                return 0;
            
            int count = 0;
            int i = workData.workerPackages.Length;
            while (--i > -1)
            {
                if (workData.workerPackages[i].finishedWorking && !workData.workerPackages[i].eventFired)
                    count++;
            }
            return count;
        }

        #endregion
        //--------------------------------------- MISC --------------------------------------
        //--------------------------------------- MISC --------------------------------------


    }
}