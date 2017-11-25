using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.Threading
{
    public class EZThreadDemoScript : MonoBehaviour
    {
        public GameObject CircleSync;
        public GameObject CircleNonSync;
        public UnityEngine.UI.Text BackgroundResultLabel;

        private Vector3 circleSyncScale;
        private float circleSyncMultiplier = 1.1f;
        private Vector3 circleNonSyncScale;
        private float circleNonSyncMultiplier = 1.1f;

        private void ScaleCircle(ref Vector3 scale, ref float multiplier)
        {
            if (scale.x > 3.0f)
            {
                multiplier = 0.9f;
                scale = new Vector3(3.0f, 3.0f, 3.0f);
            }
            else if (scale.x < 0.25f)
            {
                multiplier = 1.1f;
                scale = new Vector3(0.25f, 0.25f, 0.25f);
            }
            else
            {
                scale *= multiplier;
            }
        }

        private void ScaleSyncThread()
        {
            // note- this function is called over and over, eliminating the need for a while (running) loop here
            ScaleCircle(ref circleSyncScale, ref circleSyncMultiplier);
        }

        private void ScaleNonSyncThread()
        {
            // note- this function is called over and over, eliminating the need for a while (running) loop here
            ScaleCircle(ref circleNonSyncScale, ref circleNonSyncMultiplier);
        }

        private object CalculateRandomNumberInBackgroundThread()
        {
            System.Random r = new System.Random();
            //NETFX_CORE 用于 Windows Store 8.0, Windows Store 8.1, Windows Phone 8.1 和 Universal 8.1 ，用微软的C#编译器编译的脚本
#if NETFX_CORE

            System.Threading.Tasks.Task.Delay(500).Wait();

#else

            System.Threading.Thread.Sleep(500); // simulate a long running background task

#endif

            return r.Next();
        }

        private void CalculateRandomNumberInBackgroundCompletionOnMainThread(object result)
        {
            BackgroundResultLabel.text = "Your random number was " + result.ToString();
        }

        private void Start()
        {
            // start scaling the circle where the background thread runs in sync with the Update method
            //后台线程运行同步更新方法
            // this would be great for something like pathfinding where the path needs to update every frame in the background
            //这将是伟大的东西像寻路的道路需要在后台更新每一帧
            circleSyncScale = CircleSync.transform.localScale;
            EZThread.BeginThread(ScaleSyncThread, true);
             //bool值表示是否与统一同步更新功能。通常你希望这是真的。

            // start scaling the circle where the background thread runs as fast as possible
            //后台线程运行尽可能快
            // this could be useful for something like network calls or other external resource loading
            //这可能是有用的网络调用或其他外部资源加载
            // you will notice this circle appears to randomly change size, that is because
            //你会注意到这个圆似乎随机改变大小,这是因为
            // the background thread is scaling the circle super fast so when the update method
            //后台线程扩展圈非常快所以当更新方法
            // executes to set the actual scale, it will essentially be a random value.
            //实际执行设置规模,它基本上是一个随机值。
            circleNonSyncScale = CircleNonSync.transform.localScale;
            EZThread.BeginThread(ScaleNonSyncThread, false);
        }

        private void Update()
        {
            // set the scales from the background thread calculations
            CircleSync.transform.localScale = circleSyncScale;
            CircleNonSync.transform.localScale = circleNonSyncScale;
        }

        public void ReloadScene()
        {
            // reload scene, causes all threads to be stopped
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void CalculateRandomNumberInBackground()
        {
            // execute a one-time background operation and call a completion on the main thread
            //执行一次后台操作,在主线程调用完成
            // completion is optional and could be null if desired
            //可选，如果需要为空的情况
            EZThread.ExecuteInBackground(CalculateRandomNumberInBackgroundThread, CalculateRandomNumberInBackgroundCompletionOnMainThread);

            // ExecuteInBackground can be called with a single void function if you don't care about completion or the return result, i.e.
            //这可以被称为一个void函数如果你不关心完成或返回的结果
            // EZThread.ExecuteInBackground(() => DoBackgroundStuff());
        }
    }
}
