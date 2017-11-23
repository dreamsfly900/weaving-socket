using System;
using Frankfort.Threading;
using UnityEngine;

namespace Frankfort.Threading
{
    public delegate void MultithreadedWorkloadComplete<T>(T[] workLoad);
    public delegate void MultithreadedWorkloadPackageComplete<T>(T[] workLoad, int firstIndex, int lastIndex);
}




namespace Frankfort.Threading.Internal
{
    public delegate void ThreadWorkloadExecutor<T>(T workload);
    public delegate void ThreadWorkloadExecutorIndexed<T>(T workload, int workloadIndex);
    public delegate void ThreadWorkloadExecutorArg<T>(T workload, object extraArgument);
    public delegate void ThreadWorkloadExecutorArgIndexed<T>(T workload, int workloadIndex, object extraArgument);



    /// <summary>
    /// Helps making use of the IThreadWorkerObject-interface and helps you distribute the workload evenly to multiple-cores the easy way.
    /// This class divides your workload into smaller pieces, feeds them to the ThreadPool.
    /// </summary>
    public static class MultithreadedWorkloadHelper
	{


        /// <summary>
        /// Helps spreading the same repetetive workload over multiple threads/cores in an easy way.
        /// </summary>
        /// <typeparam name="D">Generic-Type of the delegate that will be executed and compute the workload</typeparam>
        /// <typeparam name="T">Generic-Type of the object you want to be computed by the executor</typeparam>
        /// <param name="executor">A (static) method that computes one workLoad-object at a time</param>
        /// <param name="workLoad">An array with objects you want to get computed by the executor</param>
        /// <param name="onComplete">Fired when all re-packaged workLoad-objects are finished computing</param>
        /// <param name="onPackageComplete">Fires foreach finished re-packaged set of workLoad-object</param>
        /// <param name="maxThreads"> Lets you choose how many threads will be run simultaneously by the threadpool. Default: -1 == number of cores minus one, to make sure the MainThread has at least one core to run on. (quadcore == 1 core Mainthread, 3 cores used by the ThreadPoolScheduler)</param>
        /// <param name="scheduler">If Null, a new ThreadPoolScheduler will be instantiated.</param>
        /// <param name="safeMode">Executes all the computations within try-catch events, logging it the message + stacktrace</param>
        /// <returns>A ThreadPoolScheduler that handles all the repackaged workLoad-Objects =</returns>
        public static ThreadPoolScheduler StartMultithreadedWorkloadExecution<D, T>(D executor, T[] workLoad, object extraArgument, MultithreadedWorkloadComplete<T> onComplete, MultithreadedWorkloadPackageComplete<T> onPackageComplete, int maxThreads = -1, ThreadPoolScheduler scheduler = null, bool safeMode = true)
        {
            if (scheduler == null)
            {
                scheduler = Loom.CreateThreadPoolScheduler();
            }
            else if (scheduler.isBusy)
            {
                Debug.LogError("Provided Scheduler stil busy!!!");
            }

            if (maxThreads <= 0)
                maxThreads = Mathf.Max(SystemInfo.processorCount - 1, 1);

            int packagesPerThread = 1;
            if (maxThreads > 1) //If we are running in just one thread at a time, just use one, if more, for sake of better cpu-saturation, subdive into smaller packages per core.
                packagesPerThread = 2;

            int packages = Mathf.Min(maxThreads * packagesPerThread, workLoad.Length);
            int objectsPerPackage = (int)Mathf.Ceil((float)workLoad.Length / (float)packages);

            ThreadWorkDistribution<T>[] workerPackages = new ThreadWorkDistribution<T>[packages];
            Type delegateType = typeof(D);
            //Debug.Log(delegateType.FullName);

            int count = 0;
            for (int i = 0; i < packages; i++)
            {
                int packagedSize = Mathf.Min(workLoad.Length - count, objectsPerPackage);
                if (delegateType == typeof(ThreadWorkloadExecutor<T>))
                {
                    workerPackages[i] = new ThreadWorkDistribution<T>((executor as ThreadWorkloadExecutor<T>), workLoad, count, count + packagedSize);
                }
                else if (delegateType == typeof(ThreadWorkloadExecutorIndexed<T>))
                {
                    workerPackages[i] = new ThreadWorkDistribution<T>((executor as ThreadWorkloadExecutorIndexed<T>), workLoad, count, count + packagedSize);
                }
                else if (delegateType == typeof(ThreadWorkloadExecutorArg<T>))
                {
                    workerPackages[i] = new ThreadWorkDistribution<T>((executor as ThreadWorkloadExecutorArg<T>), workLoad, extraArgument, count, count + packagedSize);
                }
                else if (delegateType == typeof(ThreadWorkloadExecutorArgIndexed<T>))
                {
                    workerPackages[i] = new ThreadWorkDistribution<T>((executor as ThreadWorkloadExecutorArgIndexed<T>), workLoad, extraArgument, count, count + packagedSize);
                }

                workerPackages[i].ID = i;
                count += objectsPerPackage;
            }



            //--------------- Store session data --------------------
            ThreadWorkDistributionSession<T> sessionData = new ThreadWorkDistributionSession<T>();
            sessionData.workLoad = workLoad;
            sessionData.onComplete = onComplete;
            sessionData.onPackageComplete = onPackageComplete;
            sessionData.packages = workerPackages;
            //--------------- Store session data --------------------
			

            scheduler.StartASyncThreads(workerPackages, sessionData.onCompleteBubble, sessionData.onPackageCompleteBubble, maxThreads, safeMode);
            return scheduler;
        }

	}













    /// <summary>
    /// Generic class to store temporary divideed Threadwork data for internal usage
    /// </summary>
    /// <typeparam name="T">The generic Type of the object being processed by the Executor </typeparam>
    public class ThreadWorkDistributionSession<T>
    {
        public MultithreadedWorkloadComplete<T> onComplete;
        public MultithreadedWorkloadPackageComplete<T> onPackageComplete;
        public T[] workLoad;
        public ThreadWorkDistribution<T>[] packages;


        public void onCompleteBubble(IThreadWorkerObject[] finishedObjects)
        {
            if (onComplete != null)
                onComplete(workLoad);
        }
        public void onPackageCompleteBubble(IThreadWorkerObject finishedObject)
        {
            if (onPackageComplete != null)
            {
                ThreadWorkDistribution<T> fCast = (ThreadWorkDistribution<T>)finishedObject;

                if (fCast != null)
                    onPackageComplete(fCast.workLoad, fCast.startIndex, fCast.endIndex - 1);
            }
        }
    }



    /// <summary>
    /// Simple IThreadWorkerObject implementation that spreads the workload-objects for you. This saves you the efford to subdive your workload into multiple packages yourself.
    /// </summary>
    /// <typeparam name="T">Generic-Type of the object you want to be computed by the executor</typeparam>
    public class ThreadWorkDistribution<T> : IThreadWorkerObject
    {
        public int ID;
        public ThreadWorkloadExecutor<T> workloadExecutor;
        public ThreadWorkloadExecutorIndexed<T> workloadExecutorIndexed;
        public ThreadWorkloadExecutorArg<T> workloadExecutorArg;
        public ThreadWorkloadExecutorArgIndexed<T> workloadExecutorArgIndexed;

        public int startIndex;
        public int endIndex;
        public T[] workLoad;
        public object extraArgument;
        private bool _isAborted = false;


        /// <summary>
        /// ThreadWorkDistribion is a workload-packages containing a subdived segment of work based on the workload divided by the amount of packages
        /// </summary>
        /// <param name="staticWorkloadExecutor">A (static) method that computes one workLoad-object at a time</param>
        /// <param name="workLoad">An array with objects you want to get computed by the executor</param>
        public ThreadWorkDistribution(ThreadWorkloadExecutor<T> workloadExecutor, T[] workLoad, int startIndex, int endIndex)
        {
            this.workloadExecutor = workloadExecutor;
            this.workLoad = workLoad;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        /// <summary>
        /// ThreadWorkDistribion is a workload-packages containing a subdived segment of work based on the workload divided by the amount of packages
        /// </summary>
        /// <param name="staticWorkloadExecutorIndexed">A (static) method that computes one workLoad-object at a time and provides the index of the array it origionally came from</param>
        /// <param name="workLoad">An array with objects you want to get computed by the executor</param>
        public ThreadWorkDistribution(ThreadWorkloadExecutorIndexed<T> workloadExecutorIndexed, T[] workLoad, int startIndex, int endIndex)
        {
            this.workloadExecutorIndexed = workloadExecutorIndexed;
            this.workLoad = workLoad;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }


        /// <summary>
        /// ThreadWorkDistribion is a workload-packages containing a subdived segment of work based on the workload divided by the amount of packages
        /// </summary>
        /// <param name="staticWorkloadExecutorIndexed">A (static) method that computes one workLoad-object at a time and provides the index of the array it origionally came from</param>
        /// <param name="workLoad">An array with objects you want to get computed by the executor</param>
        /// <param name="extraArgument">An extra Argument will be passed to the Executor if present</param>
        public ThreadWorkDistribution(ThreadWorkloadExecutorArg<T> workloadExecutorArg, T[] workLoad, object extraArgument, int startIndex, int endIndex)
        {
            this.workloadExecutorArg = workloadExecutorArg;
            this.workLoad = workLoad;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.extraArgument = extraArgument;
        }

        /// <summary>
        /// ThreadWorkDistribion is a workload-packages containing a subdived segment of work based on the workload divided by the amount of packages
        /// </summary>
        /// <param name="staticWorkloadExecutorIndexed">A (static) method that computes one workLoad-object at a time and provides the index of the array it origionally came from</param>
        /// <param name="workLoad">An array with objects you want to get computed by the executor</param>
        /// <param name="extraArgument">An extra Argument will be passed to the Executor if present</param>
        public ThreadWorkDistribution(ThreadWorkloadExecutorArgIndexed<T> workloadExecutorArgIndexed, T[] workLoad, object extraArgument, int startIndex, int endIndex)
        {
            this.workloadExecutorArgIndexed = workloadExecutorArgIndexed;
            this.workLoad = workLoad;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.extraArgument = extraArgument;
        }





        /// <summary>
        /// IThreadWorkerObject implementation: This method computes all the work.
        /// </summary>
        public void ExecuteThreadedWork()
        {
            if (workLoad == null || workLoad.Length == 0)
                return;

            //Debug.Log("Execute ID: " + ID + ", startIndex: " + startIndex  + ", endIndex: " + endIndex);
            if (workloadExecutor != null)
            {
                for (int i = startIndex; i < endIndex && !_isAborted; i++)
                {
                    UnityActivityWatchdog.SleepOrAbortIfUnityInactive();
                    workloadExecutor(workLoad[i]);
                }
            }
            else if (workloadExecutorIndexed != null)
            {
                for (int i = startIndex; i < endIndex && !_isAborted; i++)
                {
                    UnityActivityWatchdog.SleepOrAbortIfUnityInactive();
                    workloadExecutorIndexed(workLoad[i], i);
                }
            }
            else if (workloadExecutorArg != null)
            {
                for (int i = startIndex; i < endIndex && !_isAborted; i++)
                {
                    UnityActivityWatchdog.SleepOrAbortIfUnityInactive();
                    workloadExecutorArg(workLoad[i], extraArgument);
                }
            }
            else if (workloadExecutorArgIndexed != null)
            {
                for (int i = startIndex; i < endIndex && !_isAborted; i++)
                {
                    UnityActivityWatchdog.SleepOrAbortIfUnityInactive();
                    workloadExecutorArgIndexed(workLoad[i], i, extraArgument);
                }
            }
        }

        public void AbortThreadedWork()
        {
            _isAborted = true;
        }
    }

}
