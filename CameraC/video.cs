using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CameraC
{
    public class VideoWork
    {
        private const int WM_USER = 0x400;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CAP_START = WM_USER;
        private const int WM_CAP_STOP = WM_CAP_START + 68;
        private const int WM_CAP_DRIVER_CONNECT = WM_CAP_START + 10;
        private const int WM_CAP_DRIVER_DISCONNECT = WM_CAP_START + 11;
        private const int WM_CAP_SAVEDIB = WM_CAP_START + 25;
        private const int WM_CAP_GRAB_FRAME = WM_CAP_START + 60;
        private const int WM_CAP_SEQUENCE = WM_CAP_START + 62;
        private const int WM_CAP_FILE_SET_CAPTURE_FILEA = WM_CAP_START + 20;
        private const int WM_CAP_SEQUENCE_NOFILE = WM_CAP_START + 63;
        private const int WM_CAP_SET_OVERLAY = WM_CAP_START + 51;
        private const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;
        private const int WM_CAP_SET_CALLBACK_VIDEOSTREAM = WM_CAP_START + 6;
        private const int WM_CAP_SET_CALLBACK_ERROR = WM_CAP_START + 2;
        private const int WM_CAP_SET_CALLBACK_STATUSA = WM_CAP_START + 3;
        private const int WM_CAP_SET_CALLBACK_FRAME = WM_CAP_START + 5;
        private const int WM_CAP_SET_SCALE = WM_CAP_START + 53;
        private const int WM_CAP_SET_PREVIEWRATE = WM_CAP_START + 52;
        private IntPtr hWndC;
        private bool bWorkStart = false;
        private IntPtr mControlPtr;
        private int mWidth;
        private int mHeight;
        private int mLeft;
        private int mTop;

        /// <summary>
        /// 初始化显示图像
        /// </summary>
        /// <param name= “handle “> 控件的句柄 </param>
        /// <param name= “left “> 开始显示的左边距 </param>
        /// <param name= “top “> 开始显示的上边距 </param>
        /// <param name= “width “> 要显示的宽度 </param>
        /// <param name= “height “> 要显示的长度 </param>
        public VideoWork(IntPtr handle, int left, int top, int width, int height)
        {
            mControlPtr = handle;
            mWidth = width;
            mHeight = height;
            mLeft = left;
            mTop = top;
        }

        [DllImport("avicap32.dll ")]
        private static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);

        [DllImport("avicap32.dll ")]
        private static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);

        //
        //这里特别注意，因为WinAPI中的long为32位，而C#中的long为64wei，所以需要将lParam该为int
        //
        [DllImport("User32.dll ")]
        private static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// 开始显示图像
        /// </summary>
        public void Start()
        {
            if (bWorkStart)
                return;

            bWorkStart = true;
            byte[] lpszName = new byte[100];

            hWndC = capCreateCaptureWindowA(lpszName, WS_CHILD | WS_VISIBLE, mLeft, mTop, mWidth, mHeight, mControlPtr, 0);

            if (hWndC.ToInt32() != 0)
            {
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_VIDEOSTREAM, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_ERROR, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_STATUSA, 0, 0);
                SendMessage(hWndC, WM_CAP_DRIVER_CONNECT, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_SCALE, 1, 0);
                SendMessage(hWndC, WM_CAP_SET_PREVIEWRATE, 66, 0);
                SendMessage(hWndC, WM_CAP_SET_OVERLAY, 1, 0);
                SendMessage(hWndC, WM_CAP_SET_PREVIEW, 1, 0);
                //Global.log.Write( “SendMessage “);
            }
            return;

        }

        /// <summary>
        /// 停止显示
        /// </summary>
        public void Stop()
        {
            SendMessage(hWndC, WM_CAP_DRIVER_DISCONNECT, 0, 0);
            bWorkStart = false;
        }

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name= “path “> 要保存bmp文件的路径 </param>
        public void GrabImage(string path)
        {
           
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(path);
            SendMessage(hWndC, WM_CAP_SAVEDIB, 0, hBmp.ToInt32());
        }
    }
}
