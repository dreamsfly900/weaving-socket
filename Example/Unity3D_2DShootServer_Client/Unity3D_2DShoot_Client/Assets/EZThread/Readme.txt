EZThread - Easy Threads, Powerful Script
(c) 2017 Digital Ruby, LLC

Version 1.0

Introduction:
--------------------------------------------------
EZThread is a simple yet powerful Unity script for threading.

EZThread runs great on all platforms, including UWP, mobile and VR. A fully functional demo scene with examples is provided.

Please be sure to watch the tutorial I made at https://youtu.be/ypLRonEOr7s for a complete walkthrough.

Setup instructions:
--------------------------------------------------
- Add the EZThread.cs script to your project. No other files are required.
- In your scene you do NOT need to add the script to the inspector, instead simply access the static methods in the EZThread class in your scripts to perform background operations. 
- Add the following statement to the top of your script files that will be using the EZThead script: using DigitalRuby.Threading;

What it can do:
--------------------------------------------------
- Run repeating background thread actions via the BeginThread method, and optionally sync with Unity's update function. Action keeps running until you call EndThread.
- Run one time background actions via the ExecuteInBackground method.
- Run one time main thread actions with optional delay via the ExecuteOnMainThread method.
- Easily stop threads.
- Handle synchronization, locking and main thread synching for you!
- Perform way faster than and use much less memory than Unity Co-Routines.

How it works:
--------------------------------------------------
EZThread is a single Unity script that allows you to handle threading, synchronization, locking and main thread communication easily. All the complexity of threading is hidden away in the EZThread.cs script.

Please see EZThreadDemoScript.cs to see some examples.

API:
--------------------------------------------------
EZThread contains a number of public static functions that allow you to do threading operations. The functions are listed here for your convenience.

/// <summary>
/// Reset all state - stop all threads, remove all queued actions, etc. Called automatically when the app quits or a new scene loads.
/// </summary>
public void Reset();

/// <summary>
/// Run an action on a background thread. The action only runs once.
/// </summary>
/// <param name="action">Action to run</param>
public static void ExecuteInBackground(Action action);

/// <summary>
/// Run an action and return the result on a background thread. The action only runs once.
/// </summary>
/// <param name="action">Action</param>
/// <param name="completion">Action to run on the main thread with the result of the action</param>
public static void ExecuteInBackground(Func<object> action, Action<object> completion);

/// <summary>
/// Queue an action to run as soon as possible on the main thread. The action only runs once.
/// </summary>
/// <param name="action">Action to run</param>
public static void ExecuteOnMainThread(Action action);

/// <summary>
/// Queue an action to run after a delay on the main thread. The action only runs once.
/// </summary>
/// <param name="action">Action to run</param>
/// <param name="delaySeconds">Delay in seconds before running action</param>
public static void ExecuteOnMainThread(Action action, float delaySeconds);

/// <summary>
/// Begin running a thread. For short, one time background operations, Execute is usually a better choice.
/// The specified action will be called over and over until EndThread is called.
/// </summary>
/// <param name="action">Action to run over and over</param>
/// <returns>Running thread instance. Call EndThread to end the thread.</returns>
public static EZThreadRunner BeginThread(Action action);

/// <summary>
/// Begin running a thread. For short, one time background operations, Execute is usually a better choice.
/// The specified action will be called over and over until EndThread is called.
/// </summary>
/// <param name="action">Action to run over and over</param>
/// <param name="synchronizeWithUpdate">Whether to sync with Unitys update function. Generally you want this to be true.</param>
/// <returns>Running thread instance. Call EndThread to end the thread.</returns>
public static EZThreadRunner BeginThread(Action action, bool synchronizeWithUpdate);

/// <summary>
/// Remove a running thread
/// </summary>
/// <param name="thread">Thread to stop and remove</param>
public static void EndThread(EZThreadRunner thread);

Support:
--------------------------------------------------
Please email support@digitalruby.com if you have feedback, feature requests or questions!

- Jeff Johnson