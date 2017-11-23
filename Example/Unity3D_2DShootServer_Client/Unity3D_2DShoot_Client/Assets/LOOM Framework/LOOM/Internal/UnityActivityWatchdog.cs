using System;
using UnityEngine;
using System.Threading;


namespace Frankfort.Threading.Internal
{
    public class UnityActivityWatchdog : MonoBehaviour
    {
        private static bool helperCreated = false;
        private static bool unityRunning = true;
        private static bool unityPaused = false;
        private static bool unityFocused = true;
        private static bool combinedActive = true;




        //--------------------------------------- CHECK UNTIY ACTIVE --------------------------------------
        //--------------------------------------- CHECK UNTIY ACTIVE --------------------------------------
        #region CHECK UNTIY ACTIVE
		public static bool CheckUnityRunning()
		{
			return unityRunning;
		}


        /// <summary>
        /// A easy way to check if Unity is still running, focused and not pauzed.
        /// This comes in handy if threads keep running heavy workloads on seperate threads while IOS for example tries to puch the Application to the background.
        /// If you are executing a giant routine on a seperate thread that takes several seconds per cycle, you might want to check regularly if unity is still active by using this check.
        /// </summary>
        /// <returns>Returns TRUE if Unity is still running, not pauzed and has focus. </returns>
        public static bool CheckUnityActive()
        {
            Init();
            return combinedActive;
        }



        /// <summary>
        /// !IMPORTANT! This method should be called regularly within routines that take more then half a second to complete, to make sure IOS for example is able to force an application to sleep when it looses focus or gets puched to the background.
        /// This is a very light-weight check, internally it only needs to check two static booleans once everything is Initialized and running.
        /// You can use this without causing any serious overhead.
        /// Motivation: Sins Threads cannot be put asleep from the outside, it needs the be managed from within the thread itself, thats why this method was build.
        /// 
        /// Example:
        /// for(int i = 0; i < 999999999; i++)
        /// {
        ///     Loom.SleepOrAbortIfUnityInactive(); //Prevents IOS for example of killing this app because the threads won't sleep once your unity-app is puched to the background.
        ///     //Do something heavy that will cause this routine to run more then 0.5 seconds.
        /// }
        /// </summary>
        public static void SleepOrAbortIfUnityInactive()
        {
            Init();
            while (!combinedActive && !MainThreadWatchdog.CheckIfMainThread())
            {
                if (unityRunning)
                {
                    //Debug.Log("UNITY INACTIVE: About to sleep for 100 ms: " + Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(100);
                }
                else
                {
                    //Debug.Log("UNITY NOT RUNNING: About to abort: " + Thread.CurrentThread.ManagedThreadId);
                    Thread.CurrentThread.Interrupt();
					Thread.CurrentThread.Join();
                }
            }
        }

        #endregion
        //--------------------------------------- CHECK UNTIY ACTIVE --------------------------------------
        //--------------------------------------- CHECK UNTIY ACTIVE --------------------------------------
		
	









        //--------------------------------------- INIT --------------------------------------
        //--------------------------------------- INIT --------------------------------------
        #region INIT

        /// <summary>
        /// Starts the Helper & validates the running Threads.
        /// </summary>
        public static void Init()
        {
            if (!helperCreated)
                CreateHelperGameObject();
        }


        private static void CreateHelperGameObject()
        {
            GameObject helperGO = new GameObject("UnityActivityHelper");
            UnityActivityWatchdog helper = helperGO.AddComponent<UnityActivityWatchdog>();
            helperGO.hideFlags = helper.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            GameObject.DontDestroyOnLoad(helperGO);
            helperCreated = true;
        }

        #endregion
        //--------------------------------------- INIT --------------------------------------
        //--------------------------------------- INIT --------------------------------------










        //--------------------------------------- LISTEN TO UNITY CALLS --------------------------------------
        //--------------------------------------- LISTEN TO UNITY CALLS --------------------------------------
        #region LISTEN TO UNITY CALLS

        private void OnApplicationQuit()
        {
            unityRunning = false;
            UpdateStatus();
            Debug.Log("UnityActivityWatchdog: OnApplicationQuit");
        }
        private void OnApplicationPause(bool pause)
        {
            unityPaused = pause;
            UpdateStatus();
            Debug.Log("UnityActivityWatchdog: OnApplicationPauze [" + pause + "]");
        }
        private void OnApplicationFocus(bool focus)
        {
            unityFocused = focus;
            UpdateStatus();
            Debug.Log("UnityActivityWatchdog: OnApplicationFocus [" + focus + "]");
        }

        private static void UpdateStatus()
        {
            #if UNITY_IPHONE
                combinedActive = unityRunning && unityFocused && !unityPaused;
            #elif UNITY_ANDROID
                combinedActive = unityRunning && unityFocused && !unityPaused;
            #else
                combinedActive = unityRunning && !unityPaused;
            #endif
        }
        #endregion
        //--------------------------------------- LISTEN TO UNITY CALLS --------------------------------------
        //--------------------------------------- LISTEN TO UNITY CALLS --------------------------------------
			






   }				
}