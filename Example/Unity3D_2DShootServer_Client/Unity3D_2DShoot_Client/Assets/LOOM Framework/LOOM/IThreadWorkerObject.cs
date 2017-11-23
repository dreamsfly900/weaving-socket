using System;


namespace Frankfort.Threading
{

    
    
    public interface IThreadWorkerObject
    {
        /// <summary>
        /// This method is called by the Scheduler as the main entryPoint to start working.
        /// - Note: Make sure it doesn't throw any errors, because errors outside of the Mainthread are "eaten" by Unity.
        /// - You can enable "savemode" if required, which executes all the work within a Try-catch statement to keep it going...
        /// Example:
        /// 
        ///     private bool isAborted;
        ///     public void ExecuteThreadedWork()
        ///     {
        ///         long i = 10000000;
        ///         while (--i > -1 && !isAborted)
        ///             result++;
        ///     }
        ///     
        ///     public void AbortThreadedWork()
        ///     {
        ///         isAborted = true;
        ///     }
        /// </summary>
        void ExecuteThreadedWork();


        /// <summary>
        /// When called by the Scheduler, do whatever you need to do to directly interupt/stop the work in progress.
        /// Why no aborting the thread? Its not possible to interupt an loop without calling Thread.Abort() or Thread.Join(). 
        /// This freamwork does NOT use Thread.Abort & Thread.Join because of webplayer-sandbox restrictions etc.
        /// Therefore its up to you to implement a way to always directly abort the process as shown in the example bellow.
        /// - Note: Make sure it doesn't throw any errors, because errors outside of the Mainthread are "eaten" by Unity.
        /// - You can enable "savemode" if required, which executes all the work within a Try-catch statement to keep it going...
        /// 
        /// Example:
        /// 
        ///     private bool isAborted;
        ///     public void ExecuteThreadedWork()
        ///     {
        ///         long i = 10000000;
        ///         while (--i > -1 && !isAborted)
        ///             result++;
        ///     }
        ///     
        ///     public void AbortThreadedWork()
        ///     {
        ///         isAborted = true;
        ///     }
        /// </summary>
        void AbortThreadedWork();
    }
}