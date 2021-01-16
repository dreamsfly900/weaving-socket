using System;
using System.Runtime.InteropServices;

namespace Weave.Base
{
    public class WevaeSocketSession
    {
        public string[] Parameter
        {
            get; set;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr handle;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public System.Drawing.Point p;
    }
    public class AccurateTimer
    {
        public static bool IsTimeBeginPeriod = false;

        const int PM_REMOVE = 0x0001;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
           uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DispatchMessage(ref MSG lpMsg);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(ref Int64 count);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(ref Int64 frequency);

        public static int GetTimeTick()
        {
            return Environment.TickCount;
        }

        public static void AccurateSleep(int a_i4MSec)
        {
            Int64 t_i8Frequency = 0;
            Int64 t_i8StartTime = 0;
            Int64 t_i8EndTime = 0;
            double t_r8PassedMSec = 0;
            MSG msg;
            AccurateTimer.QueryPerformanceCounter(ref t_i8StartTime);
            AccurateTimer.QueryPerformanceFrequency(ref t_i8Frequency);
            do
            {
                if (AccurateTimer.PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    AccurateTimer.TranslateMessage(ref msg);
                    AccurateTimer.DispatchMessage(ref msg);
                }
                AccurateTimer.QueryPerformanceCounter(ref t_i8EndTime);
                t_r8PassedMSec = ((double)(t_i8EndTime - t_i8StartTime) / (double)t_i8Frequency) * 1000;
            } while (t_r8PassedMSec <= a_i4MSec);
        }
    }
}
