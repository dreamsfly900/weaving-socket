using DHNetSDK;
using DHPlaySDK;
using DHVDCSDK;
using P2P;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHDVR
{
    public class CameraData
    {
        public string IP { set; get; }
        public string Code { set; get; }
       
        public int Port { set; get; }
        public string UserName { set; get; }
        public string Pwd { set; get; }
        public IntPtr Handle { set; get; }
        public string VideoPath { set; get; }
        public string ImagesPath { set; get; }
        public Control control { set; get; }
    }

    public class ModeA
    {
        Socket soc;
        string tongdao = "";

        public Socket Soc
        {
            get
            {
                return soc;
            }

            set
            {
                soc = value;
            }
        }

        public string Tongdao
        {
            get
            {
                return tongdao;
            }

            set
            {
                tongdao = value;
            }
        }
    }
    public class DHCamera
    {
      static  List<ModeA> listA = new List<ModeA>();
        p2psever server = new p2psever();
        public DHCamera()
        {
        
            server.receiveevent += Server_receiveevent;
            server.EventDeleteConnSoc += Server_EventDeleteConnSoc;
            server.EventUpdataConnSoc += Server_EventUpdataConnSoc;
          
            
        }

        private void Server_EventUpdataConnSoc(Socket soc)
        {
            ModeA m = new ModeA();
            m.Tongdao = cameraData.Code;
            m.Soc = soc;
            listA.Add(m);
        }

        private void Server_EventDeleteConnSoc(Socket soc)
        {
            try
            {
                int count = listA.Count;
                ModeA[] ma = new ModeA[listA.Count];
                listA.CopyTo(0, ma, 0, count);
                foreach (ModeA m in ma)
                {
                    if (m != null)
                        if (m.Soc == soc)
                            listA.Remove(m);
                }
            }
            catch { }
        }

        private void Server_receiveevent(byte command, string data, Socket soc)
        {
            
        }
        #region << 变量定义 >>

        /// <summary>
        /// 设备用户登录ＩＤ
        /// </summary>
        private int pLoginID;

        /// <summary>
        /// 程序消息提示Title
        /// </summary>
        private const string pMsgTitle = "大华网络SDK Demo程序";

        /// <summary>
        /// 最后操作信息显示格式
        /// </summary>
        private const string pErrInfoFormatStyle = "代码:errcode;\n描述:errmSG.";

        /// <summary>
        /// 当前回放的文件信息
        /// </summary>
        NET_RECORDFILE_INFO fileInfo;

        /// <summary>
        /// 播放方式
        /// </summary>
        private int playBy = 0;

        /// <summary>
        /// 实时播放句柄保存
        /// </summary>
        private int[] pRealPlayHandle;

        /// <summary>
        /// 回放句柄保存
        /// </summary>
        private int[] pPlayBackHandle;

        /// <summary>
        /// 回放通道号
        /// </summary>
        private int pPlayBackChannelID;

        /// <summary>
        /// 上次点击的PictureBox控件
        /// </summary>
        //private PictureBox oldPicRealPlay;

        /// <summary>
        /// 当前点击的PictureBox控件
        /// </summary>
        //private PictureBox picRealPlay;

        private fDisConnect disConnect;

        private NET_DEVICEINFO deviceInfo;

        #endregion
        CameraData cameraData;
        IntPtr lLiveHandle = IntPtr.Zero;
        int error;

          
        public bool Init(CameraData _cameraData)
        {
            cameraData = _cameraData;
            server.start(Convert.ToInt32(cameraData.Code));
            disConnect = new fDisConnect(DisConnectEvent);
            DHClient.DHInit(disConnect, IntPtr.Zero);
            DHClient.DHSetEncoding((int)LANGUAGE_ENCODING.gb2312);//字符编码格式设置，默认为gb2312字符编码，如果为其他字符编码请设置
            pLoginID = DHClient.DHLogin(cameraData.IP, (ushort)cameraData.Port, cameraData.UserName, cameraData.Pwd, out deviceInfo, out error);
            // cameraData.control.Text = pLoginID + "";
            if (pLoginID != 0)
            {
                // (cameraData.control as UserControl).Text = pLoginID.ToString();
                pPlayBackHandle = new int[deviceInfo.byChanNum];
                //画面按钮有效性控制
                pRealPlayHandle = null;
                //for (int i = 0; i < deviceInfo.byChanNum; i++)
                //{
                //    cmbChannelSelect.Items.Add(i.ToString());
                //    //cmbChannelSelect.Items.Add((i+1).ToString());
                //}
                NET_SDK_CLIENTINFO lsc = new NET_SDK_CLIENTINFO();

                lsc.lChannel = deviceInfo.byChanNum; //预览的通道
                lsc.streamType = 1;// 码流
                lsc.hPlayWnd = cameraData.Handle;//pictureBox.Handle
                lLiveHandle = (IntPtr)DHClient.DHRealPlay(pLoginID, 0, lsc.hPlayWnd);//ID 通道 ，Picture句柄
                
                IntPtr userdata = (IntPtr)Convert.ToInt32(_cameraData.Code);


                DHClient.DHSetRealDataCallBack((int)lLiveHandle, fd, userdata);
                //  DHPlay.PLAY_OpenStream(tongdao, IntPtr.Zero, 0, (UInt32)900 * 1024);
                //播放的部分
                //bool b2 = DHPlay.DHPlayControl(PLAY_COMMAND.OpenStream, tongdao, IntPtr.Zero, 0, 900 * 1024);
                //DHPlay.DHSetStreamOpenMode(tongdao, PLAY_MODE.STREAME_FILE);
                //DHPlay.DHPlayControl(0, tongdao, _cameraData.control.Handle);
                return true;
            }
            else
            {
                return false;
            }
        }
        int tongdao = 1;
        static fRealDataCallBack fd = new fRealDataCallBack(RealDataCallBack);
        public static void RealDataCallBack(int lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr dwUser)
        {
            if (dwDataType == 1)
            { }
            else
            { return; }
            try
            {
                byte[] pBufferdata = new byte[dwBufSize];
                int i = 0;
                while (i < pBufferdata.Length)
                {
                    pBufferdata[i] = Marshal.ReadByte(pBuffer, i);
                    i++;
                }
                if (dwUser.ToString() == "1314")
                {
                }
                else { }
                try
                {
                    int count = listA.Count;
                    ModeA[] ma = new ModeA[listA.Count];
                    listA.CopyTo(0, ma, 0, count);
                    foreach (ModeA m in ma)
                    {
                        if (m != null)
                        
                        if (m.Tongdao == dwUser.ToString())
                            {
                             
                                try
                                {
                                    m.Soc.Send(pBufferdata);
                                }
                                catch { }
                            }
                    }
                }
                catch { }
            }
            catch { }
            //byte[] pBufferdatas = new byte[dwBufSize];
            //pBufferdata.CopyTo(pBufferdatas, 0);
            //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(aabbcc), pBufferdata);

            System.Threading.Thread.Sleep(10);

        }
        //void aabbcc(object obj)
        //{


        //    byte[] pBufferdata = (byte[])obj;
        //    IntPtr pName = Marshal.AllocHGlobal((IntPtr)pBufferdata.Length);
        //    Marshal.Copy(pBufferdata, 0, pName, (int)pBufferdata.Length);
        //    DHPlay.PLAY_InputData(tongdao, pName, (uint)pBufferdata.Length);
        //}
        public void Dispose()
        {
           
            DHClient.DHStopSaveRealData((int)lLiveHandle);
            bool result = DHClient.DHLogout(pLoginID);
            DHClient.DHCleanup();
        }
        private void DisConnectEvent(int lLoginID, StringBuilder pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            //设备断开连接处理            
            //MessageBox.Show("设备用户断开连接", pMsgTitle);
        }
        /// <summary>
        /// 抓图处理代码
        /// </summary>
        /// <param name="hPlayHandle">播放句柄</param>
        /// <param name="bmpPath">图像促存路径</param>
        private bool CapturePicture(int hPlayHandle, string bmpPath)
        {
            if (DHClient.DHCapturePicture(hPlayHandle, bmpPath))
            {
                //抓图成功处理
                return true;
            }
            else
            {
                //抓图失败处理
                return false;
            }
        }
        /// <summary>
        /// 抓图按钮按下
        /// </summary>
        /// <param name="hPlayHandle"></param>
        private bool CapturePicture(int hPlayHandle)
        {
            string bmpPath = @"D:\起重设备监控\监控\bin\Debug\CAMERA\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";
            //抓图处理
            return CapturePicture(hPlayHandle, bmpPath);
        }
        public struct NET_SDK_CLIENTINFO
        {
            public Int32 lChannel;//通道号
            public Int32 streamType;
            public IntPtr hPlayWnd;//播放窗口的句柄
        }
    }
}
