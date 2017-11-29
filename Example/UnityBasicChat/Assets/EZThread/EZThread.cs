#if NETFX_CORE

#define TASK_AVAILABLE

using System.Threading.Tasks;

#endif

using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

namespace DigitalRuby.Threading
{
    public class EZThread : MonoBehaviour
    {
        /// <summary>
        /// Encapsulates a fully running thread
        /// </summary>
        public class EZThreadRunner
        {
            private bool running = true;
            private Action action;

            private void ThreadFunction()
            {
                while (running)
                {
                    action();
                }
            }

            private void ThreadFunctionSync()
            {
                while (running)
                {
                    action();
                    SyncEvent.WaitOne(100);
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="action">Action</param>
            /// <param name="synchronizeWithUpdate">Whether to sync with the main thread update loop</param>
            internal EZThreadRunner(Action action, bool synchronizeWithUpdate)
            {
                this.action = action;
                this.running = true;
                if (synchronizeWithUpdate)
                {
                    SyncEvent = new System.Threading.AutoResetEvent(true);
                }

#if TASK_AVAILABLE

                if (synchronizeWithUpdate)
                {
                    Task.Factory.StartNew(ThreadFunctionSync);
                }
                else
                {
                    Task.Factory.StartNew(ThreadFunction);
                }

#else

                WaitCallback w;
                if (synchronizeWithUpdate)
                {
                    w = new WaitCallback((s) => ThreadFunctionSync());
                }
                else
                {
                    w = new WaitCallback((s) => ThreadFunction());
                }
                ThreadPool.QueueUserWorkItem(w);

#endif

            }

            /// <summary>
            /// Stop and remove the thread
            /// </summary>
            public void Stop()
            {
                running = false;
            }

            public AutoResetEvent SyncEvent { get; private set; }
        }

        private static GameObject singletonObject;
        private static EZThread singleton;
        private readonly List<KeyValuePair<Action, float>> mainThreadActions = new List<KeyValuePair<Action, float>>();
        private readonly Queue<KeyValuePair<object, Action<object>>> mainThreadCompletions = new Queue<KeyValuePair<object, Action<object>>>();
        private readonly List<EZThreadRunner> threads = new List<EZThreadRunner>();

        private static void EnsureCreated()
        {
            if (singleton != null)
            {
                return;
            }
            singletonObject = new GameObject("EZTHREAD");
            singletonObject.hideFlags = HideFlags.HideAndDontSave;
            singleton = singletonObject.AddComponent<EZThread>();
            GameObject.DontDestroyOnLoad(singleton);

#if !TASK_AVAILABLE

            System.Threading.ThreadPool.SetMinThreads(Environment.ProcessorCount, Environment.ProcessorCount);
            System.Threading.ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

#endif

        }

        private static void InternalExecute(Func<object> a, Action<object> completion)
        {

#if TASK_AVAILABLE

            Task.Factory.StartNew(() =>
            {
                object result = a();
                if (completion != null)
                {
                    ExecuteOnMainThread(() => completion(result));
                }
            });

#else

            ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
            {
                object result = a();
                if (completion != null)
                {
                    ExecuteOnMainThread(() => completion(result));
                }
            }));

#endif

        }

        private void RunMainThreadActions()
        {
            KeyValuePair<Action, float> a;
            KeyValuePair<object, Action<object>> c;
            int index = 0;

            while (true)
            {
                lock (mainThreadActions)
                {
                    if (index >= mainThreadActions.Count)
                    {
                        break;
                    }
                    a = mainThreadActions[index];
                    float timeRemaining = a.Value - Time.deltaTime;
                    if (timeRemaining > 0.0f)
                    {
                        // update the entry with the new time remaining value
                        mainThreadActions[index++] = new KeyValuePair<Action, float>(a.Key, timeRemaining);
                        continue;
                    }

                    // action is ready to be run, remove it
                    mainThreadActions.RemoveAt(index);
                }
                a.Key();
            }
            while (true)
            {
                lock (mainThreadCompletions)
                {
                    if (mainThreadCompletions.Count == 0)
                    {
                        break;
                    }
                    c = mainThreadCompletions.Dequeue();
                }
                c.Value(c.Key);
            }
        }

        private void NotifyThreads()
        {
            lock (threads)
            {
                foreach (EZThreadRunner thread in threads)
                {
                    if (thread.SyncEvent != null)
                    {
                        thread.SyncEvent.Set();
                    }
                }
            }
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneManagerSceneUnloaded;
        }

        private void SceneManagerSceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
        {
            Reset();
        }

        private void Update()
        {
            RunMainThreadActions();
            NotifyThreads();
        }

        private void OnDestroy()
        {
            Reset();
        }

        private void OnApplicationQuit()
        {
            Reset();
        }

        /// <summary>
        /// Reset all state - stop all threads, remove all queued actions, etc. Called automatically when the app quits or a new scene loads.
        /// </summary>
        public void Reset()
        {
            lock (threads)
            {
                foreach (EZThreadRunner thread in threads)
                {
                    thread.Stop();
                }
                threads.Clear();
            }
            lock (mainThreadActions)
            {
                mainThreadActions.Clear();
            }
            lock (mainThreadCompletions)
            {
                mainThreadCompletions.Clear();
            }
        }

        /// <summary>
        /// Run an action on a background thread. The action only runs once.
        /// </summary>
        /// <param name="action">Action to run</param>
        public static void ExecuteInBackground(Action action)
        {
            EnsureCreated();
            InternalExecute(() =>
            {
                action();
                return null;
            }, null);
        }

        /// <summary>
        /// Run an action and return the result on a background thread. The action only runs once.
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="completion">Action to run on the main thread with the result of the action</param>
        public static void ExecuteInBackground(Func<object> action, Action<object> completion)
        {
            EnsureCreated();
            InternalExecute(action, completion);
        }

        /// <summary>
        /// Queue an action to run as soon as possible on the main thread. The action only runs once.
        /// </summary>
        /// <param name="action">Action to run</param>
        public static void ExecuteOnMainThread(Action action)
        {
            EnsureCreated();
            lock (singleton.mainThreadActions)
            {
                singleton.mainThreadActions.Add(new KeyValuePair<Action, float>(action, 0.0f));
            }
        }

        /// <summary>
        /// Queue an action to run after a delay on the main thread. The action only runs once.
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="delaySeconds">Delay in seconds before running action</param>
        public static void ExecuteOnMainThread(Action action, float delaySeconds)
        {
            EnsureCreated();
            lock (singleton.mainThreadActions)
            {
                singleton.mainThreadActions.Add(new KeyValuePair<Action, float>(action, delaySeconds));
            }
        }

        /// <summary>
        /// Begin running a thread. For short, one time background operations, Execute is usually a better choice.
        /// The specified action will be called over and over until EndThread is called.
        /// </summary>
        /// <param name="action">Action to run over and over</param>
        /// <returns>Running thread instance. Call EndThread to end the thread.</returns>
        public static EZThreadRunner BeginThread(Action action)
        {
            return BeginThread(action, true);
        }

        /// <summary>
        /// Begin running a thread. For short, one time background operations, Execute is usually a better choice.
        /// The specified action will be called over and over until EndThread is called.
        /// </summary>
        /// <param name="action">Action to run over and over</param>
        /// <param name="synchronizeWithUpdate">Whether to sync with Unitys update function. Generally you want this to be true.</param>
        /// <returns>Running thread instance. Call EndThread to end the thread.</returns>
        public static EZThreadRunner BeginThread(Action action, bool synchronizeWithUpdate)
        {
            EnsureCreated();
            EZThreadRunner thread = new EZThreadRunner(action, synchronizeWithUpdate);
            lock (singleton.threads)
            {
                singleton.threads.Add(thread);
            }
            return thread;
        }

        /// <summary>
        /// Remove a running thread
        /// </summary>
        /// <param name="thread">Thread to stop and remove</param>
        public static void EndThread(EZThreadRunner thread)
        {
            thread.Stop();
            lock (singleton.threads)
            {
                singleton.threads.Remove(thread);
            }
        }

        /// <summary>
        /// Singleton instance of EZThread script
        /// </summary>
        public EZThread Instance { get { return singleton; } }
    }
}
