using System;
using System.Threading;

namespace Frankfort.Threading.Internal
{
    public class ThreadDispatchAction
    {
        public bool executed = false;
        public object dispatchExecutionResult = null;

        private ThreadDispatchDelegate          dispatchCallClean;
        private ThreadDispatchDelegateArg       dispatchCallArg;
        private ThreadDispatchDelegateArgReturn dispatchCallArgReturn;
        private ThreadDispatchDelegateReturn    dispatchCallReturn;
        
        private object dispatchArgParam;
        private bool safeMode;
        

        public ThreadDispatchAction()
        {
            //Do nothing...
        }



        //--------------------------------------- 4 DIFFERENT OVERLOADS --------------------------------------
        //--------------------------------------- 4 DIFFERENT OVERLOADS --------------------------------------
        #region 4 DIFFERENT OVERLOADS

        public void Init(ThreadDispatchDelegate dispatchCall, bool waitForExecution, bool safeMode)
        {
            this.safeMode = safeMode;
            this.dispatchCallClean = dispatchCall;
            ValidateExecutionOnInit(waitForExecution);
        }

        public void Init(ThreadDispatchDelegateArg dispatchCall, object dispatchArgumentParameter, bool waitForExecution, bool safeMode)
        {
            this.safeMode = safeMode;
            this.dispatchCallArg = dispatchCall;
            this.dispatchArgParam = dispatchArgumentParameter;
            ValidateExecutionOnInit(waitForExecution);
        }

        public void Init(ThreadDispatchDelegateArgReturn dispatchCall, object dispatchArgumentParameter, bool safeMode)
        {
            this.safeMode = safeMode;
            this.dispatchCallArgReturn = dispatchCall;
            this.dispatchArgParam = dispatchArgumentParameter;
            ValidateExecutionOnInit(true); //Forced to wait, the return-result should always be available after execution
        }

        public void Init(ThreadDispatchDelegateReturn dispatchCall, bool safeMode)
        {
            this.safeMode = safeMode;
            this.dispatchCallReturn = dispatchCall;
            ValidateExecutionOnInit(true); //Forced to wait, the return-result should always be available after execution
        } 
        #endregion
        //--------------------------------------- 4 DIFFERENT OVERLOADS --------------------------------------
        //--------------------------------------- 4 DIFFERENT OVERLOADS --------------------------------------
			





        private void ValidateExecutionOnInit(bool waitForExecution)
        {
            if (waitForExecution)
            {
                if (!MainThreadWatchdog.CheckIfMainThread())
                {
                    while (!executed && Loom.CheckUnityActive())
                        Thread.Sleep(5);
                }
                else
                {
                    ExecuteDispatch();
                }
            }
        }

        public void ExecuteDispatch()
        {
            if (safeMode)
            {
                try
                {
                    if (dispatchCallClean != null)
                    {
                        dispatchCallClean();
                    }
                    else if (dispatchCallArg != null)
                    {
                        dispatchCallArg(dispatchArgParam);
                    }
                    else if (dispatchCallArgReturn != null)
                    {
                        dispatchExecutionResult = dispatchCallArgReturn(dispatchArgParam);
                    }
                    else if (dispatchCallReturn != null)
                    {
                        dispatchExecutionResult = dispatchCallReturn();
                    }
                }
                catch (Exception e)
                {
                    //At this point, we should always be in the MainThread anyways...
                    UnityEngine.Debug.Log(e.Message + e.StackTrace);
                }
            }
            else
            {
                if (dispatchCallClean != null)
                {
                    dispatchCallClean();
                }
                else if (dispatchCallArg != null)
                {
                    dispatchCallArg(dispatchArgParam);
                }
                else if (dispatchCallArgReturn != null)
                {
                    dispatchExecutionResult = dispatchCallArgReturn(dispatchArgParam);
                }
                else if (dispatchCallReturn != null)
                {
                    dispatchExecutionResult = dispatchCallReturn();
                }
            }

            executed = true;
        }
    }
}
