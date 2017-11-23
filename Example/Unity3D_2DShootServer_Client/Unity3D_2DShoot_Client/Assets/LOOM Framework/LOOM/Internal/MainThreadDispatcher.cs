using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;



namespace Frankfort.Threading
{
    /// <summary>
    /// Fire and forget: The MainThread will execute this method witout any arguments to pass, nothing will be returned.
    /// </summary>
    public delegate void ThreadDispatchDelegate();

    /// <summary>
    /// Alows you to pass a argument to the delegate.
    /// </summary>
    /// <param name="arg">Once the MainThread executes this action, the argument will be passed to the delegate</param>
    public delegate void ThreadDispatchDelegateArg(object arg);

    /// <summary>
    /// Alows you to pass a argument to the delegate.
    /// Allows you to dispatch an delegate returning an object (for example: a newly instantiated gameobject) that is directly available in your ThreadPool-Thread.
    /// Because the thread you are dispatching from is not the MainThread, your ThreadPool-thread needs to wait till the MainThread executed the method, and the returned value can be used directly
    /// </summary>
    /// <param name="arg">Once the MainThread executes this action, the argument will be passed to the delegate</param>
    /// <returns></returns>
    public delegate object ThreadDispatchDelegateArgReturn(object arg);


    /// <summary>
    /// Allows you to dispatch an delegate returning an object (for example: a newly instantiated gameobject) that is directly available in your ThreadPool-Thread.
    /// Because the thread you are dispatching from is not the MainThread, your ThreadPool-thread needs to wait till the MainThread executed the method, and the returned value can be used directly
    /// </summary>
    /// <param name="arg">Once the MainThread executes this action, the argument will be passed to the delegate</param>
    public delegate object ThreadDispatchDelegateReturn();
}








namespace Frankfort.Threading.Internal
{
    /// <summary>
    /// This static class helps you use Unity-engine related objects like GameObjects or Components, without violating threadsafty of Unity.
    /// </summary>
    public static class MainThreadDispatcher
    {

        private static List<ThreadDispatchAction> dispatchActions = new List<ThreadDispatchAction>();
        private static bool helperCreated;
        public static int currentFrame = 0;
        



        //--------------------------------------- INIT --------------------------------------
        //--------------------------------------- INIT --------------------------------------
        #region INIT

        //Sins this class is static, it needs to be called from the MainThread, by some monobehaviour-awake (ThreadPoolScheduler that is...)
        public static void Init()
        {
            if (!helperCreated)
                CreateHelperGameObject();
        }

        private static void CreateHelperGameObject()
        {
            GameObject helperGO = new GameObject("MainThreadDispatchHelper");
            MainThreadDispatchHelper helper = helperGO.AddComponent<MainThreadDispatchHelper>();
            helper.hideFlags = helperGO.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            GameObject.DontDestroyOnLoad(helperGO);
            helperCreated = true;
        }

        #endregion
        //--------------------------------------- INIT --------------------------------------
        //--------------------------------------- INIT --------------------------------------









        //--------------------------------------- UPDATE CALLED BY HELPER --------------------------------------
        //--------------------------------------- UPDATE CALLED BY HELPER --------------------------------------

        #region UPDATE CALLED BY HELPER
        public static void Update()
        {
            DispatchActionsIfPresent();
        }


        private static void DispatchActionsIfPresent()
        {
            if (dispatchActions != null && dispatchActions.Count > 0)
            {
                lock (dispatchActions)
                {
                    foreach (ThreadDispatchAction action in dispatchActions)
                    {
                        if (!action.executed) //Might be executed internally. This happens when de event was dispatched from the MainThread in the firstplace....
                            action.ExecuteDispatch();
                    }
                    dispatchActions.Clear();
                    Monitor.PulseAll(dispatchActions);
                }
            }
        }

        #endregion

        //--------------------------------------- UPDATE CALLED BY HELPER --------------------------------------
        //--------------------------------------- UPDATE CALLED BY HELPER --------------------------------------











        //--------------------------------------- 4 DISPATCH OVERLOADS --------------------------------------
        //--------------------------------------- 4 DISPATCH OVERLOADS --------------------------------------
        #region 4 DISPATCH OVERLOADS


        /// <summary>
        /// Fire and forget: The MainThread will execute this method witout any arguments to pass, nothing will be returned.
        /// </summary>
        /// <param name="dispatchCall">Example: "() => Debug.Log("This will be fired from the MainThread: " + System.Threading.Thread.CurrentThread.Name)" </param>
        /// <param name="waitForExecution">Freezes the thread, waiting for the MainThread to execute & finish the "dispatchCall".</param>
        /// <param name="safeMode">Executes all the computations within try-catch events, logging it the message + stacktrace</param>
        public static void DispatchToMainThread(ThreadDispatchDelegate dispatchCall, bool waitForExecution = false, bool safeMode = true)
        {
            if (MainThreadWatchdog.CheckIfMainThread())
            {
                if (dispatchCall != null)
                    dispatchCall();
            }
            else
            {
                ThreadDispatchAction action = new ThreadDispatchAction();
                lock (dispatchActions) { dispatchActions.Add(action); }
                action.Init(dispatchCall, waitForExecution, safeMode);
            }
        }



        /// <summary>
        /// When executed by the MainThread, this argument will be passed to the "dispatchCall";
        /// </summary>
        /// <param name="dispatchCall">Example: "(object obj) => Debug.Log("This will be fired from the MainThread: " + System.Threading.Thread.CurrentThread.Name + "\nObject: " + obj.toString())"</param>
        /// <param name="dispatchArgument">Once the MainThread executes this action, the argument will be passed to the delegate</param>
        /// <param name="waitForExecution">Freezes the thread, waiting for the MainThread to execute & finish the "dispatchCall".</param>
        /// <param name="safeMode">Executes all the computations within try-catch events, logging it the message + stacktrace</param>
        public static void DispatchToMainThread(ThreadDispatchDelegateArg dispatchCall, object dispatchArgument, bool waitForExecution = false, bool safeMode = true)
        {
            if (MainThreadWatchdog.CheckIfMainThread())
            {
                if (dispatchCall != null)
                    dispatchCall(dispatchArgument);
            }
            else
            {
                ThreadDispatchAction action = new ThreadDispatchAction();
                lock (dispatchActions) { dispatchActions.Add(action); }
                action.Init(dispatchCall, dispatchArgument, waitForExecution, safeMode);
            }
        }




        /// <summary>     
        /// When executed by the MainThread, this argument will be passed to the "dispatchCall";
        /// Allows you to dispatch an delegate returning an object (for example: a newly instantiated gameobject) that is directly available in your ThreadPool-Thread.
        /// Because the thread you are dispatching from is not the MainThread, your ThreadPool-thread needs to wait till the MainThread executed the method, and the returned value can be used directly
        /// </summary>
        /// <param name="dispatchCall">Example: "(object obj) => Debug.Log("This will be fired from the MainThread: " + System.Threading.Thread.CurrentThread.Name + "\nObject: " + obj.toString())"</param>
        /// <param name="dispatchArgument">Once the MainThread executes this action, the argument will be passed to the delegate</param>
        /// <param name="safeMode">Executes all the computations within try-catch events, logging it the message + stacktrace</param>
        /// <returns>After the MainThread has executed the "dispatchCall" (and the worker-thread has been waiting), it will return whatever the dispatchCall returns to the worker-thread</returns>
        public static object DispatchToMainThreadReturn(ThreadDispatchDelegateArgReturn dispatchCall, object dispatchArgument, bool safeMode = true)
        {
            if (MainThreadWatchdog.CheckIfMainThread())
            {
                if (dispatchCall != null)
                    return dispatchCall(dispatchArgument);
            }
            else
            {
                ThreadDispatchAction action = new ThreadDispatchAction();
                lock (dispatchActions) { dispatchActions.Add(action); }
                action.Init(dispatchCall, dispatchArgument, safeMode); //Puts the Thread to sleep while waiting for the action to be invoked.
                return action.dispatchExecutionResult;
            }
            return null;
        }





        /// <summary>
        /// Allows you to dispatch an delegate returning an object (for example: a newly instantiated gameobject) that is directly available in your ThreadPool-Thread.
        /// Because the thread you are dispatching from is not the MainThread, your ThreadPool-thread needs to wait till the MainThread executed the method, and the returned value can be used directly
        /// </summary>
        /// <param name="dispatchCall">Example: "(object obj) => Debug.Log("This will be fired from the MainThread: " + System.Threading.Thread.CurrentThread.Name + "\nObject: " + obj.toString())"</param>
        /// <param name="safeMode">Executes all the computations within try-catch events, logging it the message + stacktrace</param>
        /// <returns>After the MainThread has executed the "dispatchCall" (and the worker-thread has been waiting), it will return whatever the dispatchCall returns to the worker-thread</returns>
        public static object DispatchToMainThreadReturn(ThreadDispatchDelegateReturn dispatchCall, bool safeMode = true)
        {
            if (MainThreadWatchdog.CheckIfMainThread())
            {
                if (dispatchCall != null)
                    return dispatchCall();
            }
            else
            {
                ThreadDispatchAction action = new ThreadDispatchAction();
                lock (dispatchActions) { dispatchActions.Add(action); }
                action.Init(dispatchCall, safeMode); //Puts the Thread to sleep while waiting for the action to be invoked.
                return action.dispatchExecutionResult;
            }
            return null;
        }

        #endregion
        //--------------------------------------- 4 DISPATCH OVERLOADS --------------------------------------
        //--------------------------------------- 4 DISPATCH OVERLOADS --------------------------------------
			
    }









    //--------------------------------------- HELPER GAME OBJECT WITH COMPONENT --------------------------------------
    //--------------------------------------- HELPER GAME OBJECT WITH COMPONENT --------------------------------------
    #region HELPER GAME OBJECT WITH COMPONENT


    public class MainThreadDispatchHelper : MonoBehaviour
    {
        private float WaitForSecondsTime = 0.005f;

        private void Awake()
        {
            MainThreadWatchdog.Init();
            UnityActivityWatchdog.Init();
            InvokeRepeating("UpdateMainThreadDispatcher", WaitForSecondsTime, WaitForSecondsTime);
        }


        private void Update()
        {
            MainThreadDispatcher.currentFrame = Time.frameCount;
        }
        private void UpdateMainThreadDispatcher()
        {
            MainThreadDispatcher.Update();
        }
    } 
    #endregion
    //--------------------------------------- HELPER GAME OBJECT WITH COMPONENT --------------------------------------
    //--------------------------------------- HELPER GAME OBJECT WITH COMPONENT --------------------------------------
			
}