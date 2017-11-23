using System;
using System.Threading;
using UnityEngine;

namespace Frankfort.Threading.Internal
{
    /// <summary>
    /// Keeps track of the MainThread. This is needed to handle the MainThread-edispatcher, WaitForNextFrame & WaitForSeconds safely.
    /// </summary>
    public class MainThreadWatchdog
	{

        //--------------------------------------- MANAGE MAIN THREAD VALIDATION --------------------------------------
        //--------------------------------------- MANAGE MAIN THREAD VALIDATION --------------------------------------
        #region MANAGE MAIN THREAD VALIDATION
        private static Thread mainThread = null;

        /// <summary>
        /// Set the CurrentThread to be the MainThread, if the CurrentThread happens NOT to be MultiThreadedApartment, NOT a ThreadPoolThread, and with an ID of <= 1; 
        /// </summary>
        public static void Init()
        {
            if (mainThread == null)
            {
                Thread currentThread = Thread.CurrentThread;
                if(currentThread.GetApartmentState() == ApartmentState.MTA || currentThread.ManagedThreadId > 1 || currentThread.IsThreadPoolThread)
                {
                    Debug.Log("Trying to Init a WorkerThread as the MainThread! ");
                }
                else
                {
                    mainThread = currentThread;
                }
            }
        }



        /// <summary>
        /// If you need your current code to be running on the MainThread, you can always call this method to check if its the MainThread or not...
        /// </summary>
        /// <returns>Returns TRUE if its the MainThread</returns>
        public static bool CheckIfMainThread()
        {
            Init(); //Just to be sure....
            return Thread.CurrentThread == mainThread;
        }
        #endregion

        //--------------------------------------- MANAGE MAIN THREAD VALIDATION --------------------------------------
        //--------------------------------------- MANAGE MAIN THREAD VALIDATION --------------------------------------
			
	}
}
