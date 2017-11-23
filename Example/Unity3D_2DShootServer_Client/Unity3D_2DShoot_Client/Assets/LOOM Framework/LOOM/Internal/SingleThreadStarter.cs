using System;
using System.Collections.Generic;
using System.Threading;
using Frankfort.Threading;
using UnityEngine;


namespace Frankfort.Threading.Internal
{

    /// <summary>
    /// The SingleThreadStarter is a safe/managed wrapper around the normal new Thread() & Thread.Start();
    /// It also initiates all the helpers such as the MainThreadDispatcher, WaitForSeconds & WaitForSeconds commands.
    /// </summary>
	public static class SingleThreadStarter
    {
        
        //--------------------------------------- SAFEMODE WRAPPERS --------------------------------------
        //--------------------------------------- SAFEMODE WRAPPERS --------------------------------------
        #region SAFEMODE WRAPPERS

        /// <summary>
        /// Internal temporary session data needed to fire a Singele-thread safely.
        /// </summary>
        private class SafeSingleThreadSession
        {
            private ThreadStart targetMethod;
            private ParameterizedThreadStart paramTargetMethod;
            
            public SafeSingleThreadSession(ThreadStart targetMethod)
            {
                this.targetMethod = targetMethod;
            }
            public SafeSingleThreadSession(ParameterizedThreadStart targetMethod)
            {
                this.paramTargetMethod = targetMethod;
            }



            public void SafeExecte_ThreadStart()
            {
                try
                {
                    targetMethod();
                }
                catch (Exception e)
                {
                    Loom.DispatchToMainThread(() => Debug.LogError(e.Message + e.StackTrace + "\n\n"), true);
                }
            }

            public void SafeExecte_ParamThreadStart(object argument)
            {
                try
                {
                    paramTargetMethod(argument);
                }
                catch (Exception e)
                {
                    Loom.DispatchToMainThread(() => Debug.LogError(e.Message + e.StackTrace + "\n\n"), true);
                }
            }
        }

        #endregion
        //--------------------------------------- SAFEMODE WRAPPERS --------------------------------------
        //--------------------------------------- SAFEMODE WRAPPERS --------------------------------------
			






        






        //--------------------------------------- 2 START SINGLE THREAD OVERLOADS --------------------------------------
        //--------------------------------------- 2 START SINGLE THREAD OVERLOADS --------------------------------------
        #region 2 START SINGLE THREAD OVERLOADS
        
        
        private static bool helperCreated;
        private static List<Thread> startedThreads = new List<Thread>();



        /// <summary>
        /// Starts an method running on a new thread. The Thread dies when the method has stopped running.
        /// You can now make use of the DispatchToMainThread-actions & WaitForNextFrame
        /// </summary>
        /// <param name="targetMethod">The method that will be executed by the thread</param>
        /// <param name="priority">Thread priority</param>
        /// <returns>Newly instantiated Thread</returns>
        public static Thread StartSingleThread(ThreadStart targetMethod, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal, bool safeMode = true)
        {
            Init();
            MainThreadWatchdog.Init();
            MainThreadDispatcher.Init();
            UnityActivityWatchdog.Init();

            Thread result = null;
            if(safeMode)
            {
                SafeSingleThreadSession sessionData = new SafeSingleThreadSession(targetMethod);
                result = new Thread(sessionData.SafeExecte_ThreadStart);
            }
            else
            {
                result = new Thread(targetMethod);
            }

            result.Priority = priority;
            startedThreads.Add(result);
            result.Start();
            return result;
        }



        /// <summary>
        /// Starts an method running on a new thread. The Thread dies when the method has stopped running.
        /// You can now make use of the DispatchToMainThread-actions & WaitForNextFrame
        /// </summary>
        /// <param name="targetMethod">The method that will be executed by the thread</param>
        /// <param name="argument">Object to pass to the targetMethod as soon as the Thread is started</param>
        /// <param name="priority">Thread priority</param>
        /// <returns>Newly instantiated Thread</returns>
        public static Thread StartSingleThread(ParameterizedThreadStart targetMethod, object argument, System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal, bool safeMode = true)
        {
            Init();
            MainThreadWatchdog.Init();
            MainThreadDispatcher.Init();
            UnityActivityWatchdog.Init();
            
            Thread result = null;
            if (safeMode)
            {
                SafeSingleThreadSession sessionData = new SafeSingleThreadSession(targetMethod);
                result = new Thread(sessionData.SafeExecte_ParamThreadStart);
            }
            else
            {
                result = new Thread(targetMethod);
            }

            result.Priority = priority;
            startedThreads.Add(result);

            result.Start(argument);
            return result;
        }

        #endregion
        //--------------------------------------- 2 START SINGLE THREAD OVERLOADS --------------------------------------
        //--------------------------------------- 2 START SINGLE THREAD OVERLOADS --------------------------------------














        //--------------------------------------- INIT --------------------------------------
        //--------------------------------------- INIT --------------------------------------
        #region INIT

        /// <summary>
        /// Starts the Helper & validates the running Threads.
        /// </summary>
        private static void Init()
        {
            ValidateThreadStates();
            if (!helperCreated)
                CreateHelperGameObject();
        }


        private static void CreateHelperGameObject()
        {
            GameObject helperGO = new GameObject("SingleThreadHelper");
            SingleThreadHelper helper = helperGO.AddComponent<SingleThreadHelper>();
            helperGO.hideFlags = helper.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            GameObject.DontDestroyOnLoad(helperGO);
            helperCreated = true;
        }
        #endregion
        //--------------------------------------- INIT --------------------------------------
        //--------------------------------------- INIT --------------------------------------
			











        //--------------------------------------- MANAGE THREADS --------------------------------------
        //--------------------------------------- MANAGE THREADS --------------------------------------
        #region MANAGE THREADS

        /// <summary>
        /// Aborts all running threads.
        /// </summary>
        public static void AbortRunningThreads()
        {
            ValidateThreadStates();
            Debug.Log("Abort running Threads: " + startedThreads.Count);
            foreach (Thread thread in startedThreads)
			{
				thread.Interrupt();
                thread.Join(100);
			}
			startedThreads.Clear();
        }

        /// <summary>
        /// Thread-state validation.
        /// </summary>
        private static void ValidateThreadStates()
        {
            int i = startedThreads.Count;
            while (--i > -1)
            {
                Thread thread = startedThreads[i];
                if( thread.ThreadState == ThreadState.Aborted ||
                    thread.ThreadState == ThreadState.AbortRequested ||
                    thread.ThreadState == ThreadState.Stopped ||
                    thread.ThreadState == ThreadState.StopRequested ||
                    thread.ThreadState == ThreadState.Unstarted )
                {
                    startedThreads.RemoveAt(i);
                }
            }
        }

        #endregion
        //--------------------------------------- MANAGE THREADS --------------------------------------
        //--------------------------------------- MANAGE THREADS --------------------------------------
	}











    //--------------------------------------- SINGLE THREAD HELPER COMPONENT --------------------------------------
    //--------------------------------------- SINGLE THREAD HELPER COMPONENT --------------------------------------
    #region SINGLE THREAD HELPER COMPONENT

    public class SingleThreadHelper : MonoBehaviour
    {	
        private void OnApplicationQuit()
        {
            SingleThreadStarter.AbortRunningThreads();
        }
    }

    #endregion
    //--------------------------------------- SINGLE THREAD HELPER COMPONENT --------------------------------------
    //--------------------------------------- SINGLE THREAD HELPER COMPONENT --------------------------------------
			
}
