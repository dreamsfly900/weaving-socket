using System;
using System.Threading;
using UnityEngine;

namespace Frankfort.Threading.Internal
{
    public class ThreadWaitForSeconds
    {
        public ThreadWaitForSeconds(float seconds)
        {
            if (MainThreadWatchdog.CheckIfMainThread())
            {
                Debug.Log("Its not allowed to put the MainThread to sleep!");
                return;
            }
			
			if(!UnityActivityWatchdog.CheckUnityRunning())
				return;

            Thread.Sleep((int)Mathf.Max( 1, Mathf.Round(seconds * 1000f)));
            while (!UnityActivityWatchdog.CheckUnityActive())
                Thread.Sleep(5);
        }
    }


    public class ThreadWaitForNextFrame
    {
        public ThreadWaitForNextFrame(int waitFrames = 1, int sleepTime = 5)
        {
            if (waitFrames > 0)
            {
                if (MainThreadWatchdog.CheckIfMainThread())
                {
                    Debug.Log("Its not allowed to put the MainThread to sleep!");
                    return;
                }

                int startFrame = MainThreadDispatcher.currentFrame;

				if(!UnityActivityWatchdog.CheckUnityRunning())
					return;

                Thread.Sleep(sleepTime);
				while (!UnityActivityWatchdog.CheckUnityActive() || startFrame + waitFrames >= MainThreadDispatcher.currentFrame)
                    Thread.Sleep(sleepTime);
            }
        }
    }
}
