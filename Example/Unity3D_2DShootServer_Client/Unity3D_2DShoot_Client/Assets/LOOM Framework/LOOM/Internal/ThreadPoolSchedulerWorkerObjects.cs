using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



namespace Frankfort.Threading.Internal
{
    public class ThreadWorkStatePackage
    {
        public bool safeMode = true;
        //public bool safeModeErrorFound;
        //public List<string> safeModeErrorLog = new List<string>();
        
        public bool started;
        public bool running;
        public bool finishedWorking;
        public bool eventFired;

        public IThreadWorkerObject workerObject;
        public AutoResetEvent waitHandle;


        public void ExecuteThreadWork(object obj)
        {
            running = true;

            if (workerObject == null || waitHandle == null)
                return;

            //Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;

            if (safeMode)
            {
                try
                {
                    workerObject.ExecuteThreadedWork();
                }
                catch (Exception e)
                {
                    //safeModeErrorFound = true;
                    //safeModeErrorLog.Add(e.Message + e.StackTrace);
                    Loom.DispatchToMainThread(() => Debug.LogError(e.Message + e.StackTrace + "\n\n"), true);
                }
            }
            else
            {
                workerObject.ExecuteThreadedWork();
            }

            running = false;
            finishedWorking = true;
            waitHandle.Set(); //Fire back to the MainThread!
        }
    }




    /// <summary>
    /// Used as packaged set of variables to be fed to the WorkProvider-Thread.
    /// </summary>
    public class ASyncThreadWorkData
    {
        public ThreadWorkStatePackage[] workerPackages;
        public int maxWorkingThreads;

        public ASyncThreadWorkData(IThreadWorkerObject[] workerObjects, bool safeMode, int maxWorkingThreads = -1)
        {
            if (workerObjects == null)
                return;

            workerPackages = new ThreadWorkStatePackage[workerObjects.Length];
            
            int i = workerObjects.Length;
            while (--i > -1)
            {
                ThreadWorkStatePackage package = new ThreadWorkStatePackage();
                package.waitHandle = new AutoResetEvent(false);
                package.workerObject = workerObjects[i];
                package.safeMode = safeMode;
                workerPackages[i] = package;
            }

            if (maxWorkingThreads <= 0)
            {
                maxWorkingThreads = Mathf.Max(SystemInfo.processorCount - 1, 1);
            }
            else
            {
                this.maxWorkingThreads = maxWorkingThreads;
            }
        }


        public void Dispose()
        {
            if (workerPackages != null)
            {
                foreach (ThreadWorkStatePackage package in workerPackages)
                {
                    if (package.waitHandle != null)
                        package.waitHandle.Close();

                    package.waitHandle = null;
                    package.workerObject = null;
                }
            }
            workerPackages = null;
        }
    }
}