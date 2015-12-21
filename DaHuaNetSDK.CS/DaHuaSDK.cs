/*
 * ************************************************************************
 *                            SDK
 *                      大华网络SDK(C#版)
 * 
 * (c) Copyright 2007, ZheJiang Dahua Technology Stock Co.Ltd.
 *                      All Rights Reserved
 * 版 本 号:0.01
 * 文件名称:DaHuaSDK.cs
 * 功能说明:在现有的SDK(C++版)上再一次封装，针对C#应用开发
 * 作    者:李德明
 * 作成日期:2007/11/21
 * 修改日志:    日期        版本号      作者        变更事由
 *              2007/11/21  1.0         李德明      新建作成
 * 
 * ************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Net;

namespace DHNetSDK.CS
{
    /// <summary>
    /// 本类是在大华网络SDK(C++版)的基础上针对C#应用程序开发的开发包，使用此开
    /// 发包时需要将原有的SDK的动态链接库文件和本SDK文件放置在同一个目录下
    /// </summary>
    public class Client
    {
        #region << 类定义代码 >>

        #region << 常量、变量定义 >>

        #region << 变量>>

        #region << 临时使用 >>

        ///// <summary>
        ///// **临时**实时监视句柄,后面将整合到Channel信息中
        ///// </summary>
        //private int pRealHandle;
        ///// <summary>
        ///// **临时**网络回放句柄,后面将整合到Channel信息中
        ///// </summary>
        //private long pPlayBackHandle;

        #endregion

        /// <summary>
        /// 初始化操作完成标志
        /// </summary>
        private static bool initialized;

        /// <summary>
        /// SDK版本号使用的私有变量
        /// </summary>
        private string pSDKVersion;

        /// <summary>
        /// 设备用户基本信息使用的私有变量
        /// </summary>
        private CLIENT_INFO pClientInfo;
        
        /// <summary>
        /// 设备用户登录标志
        /// </summary>
        private bool pLogined=false;

        /// <summary>
        /// SDK使用最后一次错误信息
        /// </summary>
        private ErrInfo pLastError;

        #endregion

        #endregion

        #region << 属性 >>

        /// <summary>
        /// SDK版本号(只读属性)
        /// </summary>
        public string SDKVersion
        {
            get
            {
                return pSDKVersion;
            }
            //set
            //{
            //    pSDKVersion = value;
            //}
        }

        /// <summary>
        /// SDK使用最后一次错误信息
        /// </summary>
        public ErrInfo LastError
        {
            get
            {
                return pLastError;
            }
            //set
            //{
            //    pLastError = value;
            //}
        }

        /// <summary>
        /// 设备用户信息(只写属性)
        /// </summary>
        public CLIENT_INFO ClientInfo
        {
            //get
            //{
            //    return pClientInfo;
            //}
            set
            {
                pClientInfo = value;
            }
        }

        /// <summary>
        /// 设备用户登录标志:true己登录;false未登录
        /// </summary>
        public bool Logined
        {
            get
            {
                return pLogined;
            }
        }

        #endregion

        #region << 构造函数 >>

        /// <summary>
        /// 构造函数:
        /// ·对开发包的版本号的取得;
        /// ·初始化数字录像机;
        /// </summary>
        public Client()
        {
            pSDKVersion = "1.0";//**临时**使用1.0代替，正确使用是由现有的SDK的接口取得
            pClientInfo = new CLIENT_INFO();
            Init();//初始化数字录像机
        }

        #endregion

        #region << 方法 >>

        #region << 错误处理函数 >>

        /// <summary>
        /// 大华SDK调用失败时时抛出异常。
        /// </summary>
        /// <exception cref="Win32Exception">数字录像机原生异常,当SDK调用失败时触发。</exception>
        private  void ThrowLastError()
        {
            Int32 errorCode = CLIENT_GetLastError();
            if (errorCode != 0)
            {
                pLastError.errCode = errorCode.ToString();
                pLastError.errMessage = GetLastErrorName((uint)errorCode);
                throw new Win32Exception(errorCode, pLastError.errMessage);
            }
            else
            {
                pLastError.errCode = "0";
                pLastError.errMessage = "最近操作没有异常发生";
            }
        }

        /// <summary>
        /// 错误代码转换成标准备的文字信息
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <returns></returns>
        private static string GetLastErrorName(uint errorCode)
        {
            switch (errorCode)
            {
                case 0x80000000 | 1:
                    return "Windows系统出错";
                case 0x80000000 | 2:
                    return "网络错误，可能是因为网络超时";
                case 0x80000000 | 3:
                    return "设备协议不匹配";
                case 0x80000000 | 4:
                    return "句柄无效";
                case 0x80000000 | 5:
                    return "打开通道失败";
                case 0x80000000 | 6:
                    return "关闭通道失败";
                case 0x80000000 | 7:
                    return "用户参数不合法";
                case 0x80000000 | 8:
                    return "SDK初始化出错";
                case 0x80000000 | 9:
                    return "SDK清理出错";
                case 0x80000000 | 10:
                    return "申请render资源出错";
                case 0x80000000 | 11:
                    return "打开解码库出错";
                case 0x800000 | 12:
                    return "关闭解码库出错";
                case 0x80000000 | 13:
                    return "多画面预览中检测到通道数为0";
                case 0x80000000 | 14:
                    return "录音库初始化失败";
                case 0x80000000 | 15:
                    return "录音库未经初始化";
                case 0x80000000 | 16:
                    return "发送音频数据出错";
                case 0x80000000 | 17:
                    return "实时数据已经处于保存状态";
                case 0x80000000 | 18:
                    return "未保存实时数据";
                case 0x80000000 | 19:
                    return "打开文件出错";
                case 0x80000000 | 20:
                    return "启动云台控制定时器失败";
                case 0x80000000 | 21:
                    return "对返回数据的校验出错";
                case 0x80000000 | 22:
                    return "没有足够的缓存";
                case 0x80000000 | 23:
                    return "当前SDK未支持该功能";
                case 0x80000000 | 24:
                    return "查询不到录像";
                case 0x80000000 | 25:
                    return "无操作权限";
                case 0x80000000 | 26:
                    return "暂时无法执行";
                case 0x80000000 | 27:
                    return "未发现对讲通道";
                case 0x80000000 | 28:
                    return "未发现音频";
                case 0x80000000 | 29:
                    return "CLientSDK未经初始化";
                case 0x80000000 | 30:
                    return "下载已结束";
                case 0x80000000 | 31:
                    return "查询结果为空";
                case 0x80000000 | 32:
                    return "获取配置失败位置：系统属性";
                case 0x80000000 | 33:
                    return "获取配置失败位置：序列号";
                case 0x80000000 | 34:
                    return "获取配置失败位置：常规属性";
                case 0x80000000 | 35:
                    return "获取配置失败位置：DSP能力描述";
                case 0x80000000 | 36:
                    return "获取配置失败位置：网络属性";
                case 0x80000000 | 37:
                    return "获取配置失败位置：通道名称";
                case 0x80000000 | 38:
                    return "获取配置失败位置：视频属性";
                case 0x80000000 | 39:
                    return "获取配置失败位置：录像定时配置";
                case 0x80000000 | 40:
                    return "获取配置失败位置：解码器协议名称";
                case 0x80000000 | 41:
                    return "获取配置失败位置：232串口功能名称";
                case 0x80000000 | 42:
                    return "获取配置失败位置：解码器属性";
                case 0x80000000 | 43:
                    return "获取配置失败位置：232串口属性";
                case 0x80000000 | 44:
                    return "取配置失败位置：外部报警输入属性";
                case 0x80000000 | 45:
                    return "获取配置失败位置：图像检测报警属性";
                case 0x80000000 | 46:
                    return "获取配置失败位置：设备时间";
                case 0x80000000 | 47:
                    return "获取配置失败位置：预览参数";
                case 0x80000000 | 48:
                    return "获取配置失败位置：自动维护配置";
                case 0x80000000 | 49:
                    return "获取配置失败位置：视频矩阵配置";
                case 0x80000000 | 55:
                    return "设置配置失败位置：常规属性";
                case 0x80000000 | 56:
                    return "设置配置失败位置：网络属性";
                case 0x80000000 | 57:
                    return "设置配置失败位置：通道名称";
                case 0x80000000 | 58:
                    return "设置配置失败位置：视频属性";
                case 0x80000000 | 59:
                    return "设置配置失败位置：录像定时配置";
                case 0x80000000 | 60:
                    return "设置配置失败位置：解码器属性";
                case 0x80000000 | 61:
                    return "设置配置失败位置：232串口属性";
                case 0x80000000 | 62:
                    return "设置配置失败位置：外部报警输入属性";
                case 0x80000000 | 63:
                    return "设置配置失败位置：图像检测报警属性";
                case 0x80000000 | 64:
                    return "设置配置失败位置：设备时间";
                case 0x80000000 | 65:
                    return "设置配置失败位置：预览参数";
                case 0x80000000 | 66:
                    return "设置配置失败位置：自动维护配置";
                case 0x80000000 | 67:
                    return "设置配置失败位置：视频矩阵配置";
                case 0x80000000 | 70:
                    return "音频编码接口没有成功初始化";
                case 0x80000000 | 71:
                    return "数据过长";
                case 0x80000000 | 72:
                    return "设备不支持该操作";
                case 0x80000000 | 73:
                    return "设备资源不足";
                case 0x80000000 | 74:
                    return "服务器已经启动";
                case 0x80000000 | 75:
                    return "服务器尚未成功启动";
                case 0x80000000 | 80:
                    return "输入序列号有误";
                case 0x80000000 | 100:
                    return "密码不正确";
                case 0x80000000 | 101:
                    return "帐户不存在";
                case 0x80000000 | 102:
                    return "等待登录返回超时";
                case 0x80000000 | 103:
                    return "帐号已登录";
                case 0x80000000 | 104:
                    return "帐号已被锁定";
                case 0x80000000 | 105:
                    return "帐号已被列为黑名单";
                case 0x80000000 | 106:
                    return "资源不足，系统忙";
                case 0x80000000 | 107:
                    return "连接主机失败";
                case 0x80000000 | 108:
                    return "网络连接失败";
                case 0x80000000 | 120:
                    return "Render库打开音频出错";
                case 0x80000000 | 121:
                    return "Render库关闭音频出错";
                case 0x80000000 | 122:
                    return "Render库控制音量出错";
                case 0x80000000 | 123:
                    return "Render库设置画面参数出错";
                case 0x80000000 | 124:
                    return "Render库暂停播放出错";
                case 0x80000000 | 125:
                    return "Render库抓图出错";
                case 0x80000000 | 126:
                    return "Render库步进出错";
                case 0x80000000 | 127:
                    return "Render库设置帧率出错";
                case 0x80000000 | 999:
                    return "暂时无法设置";
                case 0x80000000 | 1000:
                    return "配置数据不合法";
                default:
                    return "未知错误";
            }
        }

        #endregion

        /// <summary>
        /// 初始化数字录像机
        /// </summary>
        public  void Init()
        {
            if (initialized==false)
            {
                CLIENT_Init(null, IntPtr.Zero);
                ThrowLastError();
                initialized = true;
            }
        }

        /// <summary>
        /// 设备用户登录
        /// </summary>
        /// <param name="ip">ip地址。</param>
        /// <param name="port">端口号。</param>        
        /// <returns>登录Id,失败返回0,否则返回Id。</returns>
        public bool Login(string ip, int port)
        {
            pClientInfo.DeviceIP = ip;
            pClientInfo.DevicePort = port;
            return Login();
        }

        /// <summary>
        /// 设备用户登录
        /// </summary>
        /// <param name="userName">设备用户名</param>
        /// <param name="userPassword">设备用户密码</param> 
        /// <param name="ip">ip地址。</param>
        /// <param name="port">端口号。</param>        
        /// <returns>登录Id,失败返回0,否则返回Id。</returns>
        public bool Login(string userName,string userPassword, string ip, int port)
        {
            pClientInfo.Name = userName;
            pClientInfo.Password = userPassword;
            pClientInfo.DeviceIP = ip;
            pClientInfo.DevicePort = port;
            return Login();
        }

        /// <summary>
        /// 设备用户登录
        /// </summary>
        public bool Login()
        {
            bool returnValue = false;
            if (initialized == true && pLogined==false)//己初始化但未登录过
            {
                NET_DEVICEINFO sdkInfo;                
                int error;
                int result = CLIENT_Login(pClientInfo.DeviceIP, (ushort)pClientInfo.DevicePort, pClientInfo.Name, pClientInfo.Password, out sdkInfo, out error);
                ThrowLastError();
                if (result != 0)
                {
                    pLogined = true;
                    pClientInfo.ID = result;
                    pClientInfo.DeviceAlarmInPortCount = sdkInfo.byAlarmInPortNum;
                    pClientInfo.DeviceAlarmOutPortCount = sdkInfo.byAlarmOutPortNum;
                    pClientInfo.DeviceChannelCount = sdkInfo.byChanNum;
                    pClientInfo.DeviceDiskCount = sdkInfo.byDiskNum;
                    pClientInfo.DeviceType = sdkInfo.byDVRType;
                    returnValue = true;
                    if (pClientInfo.ChannelInit() == false)
                    {
                        pClientInfo.Channel = null;
                    }
                    //else
                    //{
                    //    for (int i = 0; i < sdkInfo.byChanNum; i++)
                    //    {

                    //    }

                    //}
                }
                else
                {
                    pLogined = false;
                    pClientInfo.ID = 0;
                    pClientInfo.DeviceAlarmInPortCount = 0;
                    pClientInfo.DeviceAlarmOutPortCount = 0;
                    pClientInfo.DeviceChannelCount = 0;
                    pClientInfo.DeviceDiskCount = 0;
                    pClientInfo.DeviceType = 0;
                    returnValue = false;
                }
            }
            return returnValue;
        }
        
        /// <summary>
        /// 注销用户
        /// </summary>        
        public void Logout()
        {
            if (pLogined == true)//己经登录过的设备用户
            {
                CLIENT_Logout(pClientInfo.ID);
                pLogined = false;
                pClientInfo = new CLIENT_INFO();
                ThrowLastError();
            }
        }

        /// <summary>
        /// 清理SDK环境,当程序结束前执行。
        /// </summary>
        public void Cleanup()
        {
            CLIENT_Cleanup();
            ThrowLastError();
        }

        #region << 实时监视 >>

        /// <summary>
        /// 实时监视开始
        /// </summary>
        /// <param name="intChannelID">通道ID</param>
        /// <param name="hWnd">播放容器句柄</param>
        /// <returns></returns>
        public int RealPlay(int intChannelID, IntPtr hWnd)
        {
            int returnValue;
            if (intChannelID >= pClientInfo.DeviceChannelCount)
            {
                returnValue = 0;
            }
            else
            {
                returnValue = CLIENT_RealPlay(pClientInfo.ID, intChannelID, hWnd);
                ThrowLastError();
                pClientInfo.Channel[intChannelID].RealPlayHandle = returnValue;//通道对应的实时监视句柄保存
            }
            if (returnValue != 0)
            {
                pClientInfo.Channel[intChannelID].blnRealPlay = true;
            }
            else
            {
                pClientInfo.Channel[intChannelID].blnRealPlay = false;
            }
            return returnValue;
        }

        /// <summary>
        /// 结束实时监控
        /// </summary>
        /// <param name="intChannelID">通道ID</param>
        /// <returns>True:成功;False:失败</returns>
        public bool StopRealPlay(int intChannelID)
        {
            bool returnValue = true;
            if (pLogined == true)//判断是否己经登录
            {
                //returnValue = CLIENT_StopRealPlay(pRealHandle);//**临时**实时监视句柄,后面将整合到Channel信息中
                if (pClientInfo.DeviceChannelCount >= intChannelID + 1)//判断是否是有效通道
                {
                    if (pClientInfo.Channel[intChannelID].blnRealPlay == true)//判断该通道是否实时监视着
                    {
                        returnValue = CLIENT_StopRealPlay(pClientInfo.Channel[intChannelID].RealPlayHandle);
                        ThrowLastError();
                    }
                }
            }
            return returnValue;
        }

        #endregion

        #region << 回放和下载 >>

        /// <summary>
        /// 查询录像文件
        /// </summary>
        /// <param name="nChannelId">通道ID</param>
        /// <param name="nRecordFileType">录像文像类型:0.所有录像文件;1.外部报警;2.动态检测报警;3.所有报警;4.卡号查询;5.组合条件查询;</param>
        /// <param name="tmStart">录像开始时间</param>
        /// <param name="tmEnd">录像结束时间</param>
        /// <param name="pchCardid">卡号,只针对卡号查询有效，其他情况下可以填NULL</param>
        /// <param name="nriFileinfo">返回的录像文件信息，是一个NET_RECORDFILE_INFO结构数组</param>
        /// <param name="maxlen">nriFileinfo缓冲的最大长度;（单位字节，建议在100-200*sizeof(NET_RECORDFILE_INFO)之间）</param>
        /// <param name="filecount">返回的文件个数,属于输出参数最大只能查到缓冲满为止的录像记录</param>        
        /// <param name="waittime">等待时间</param>
        /// <param name="bTime">是否按时间查(目前无效)</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        public bool QueryRecordFile(int nChannelId, int nRecordFileType, DateTime tmStart, DateTime tmEnd, string pchCardid, out NET_RECORDFILE_INFO nriFileinfo, int maxlen,out int filecount, int waittime, bool bTime)
        {
            bool returnValue = false;
            nriFileinfo = new NET_RECORDFILE_INFO();
            filecount = 0;
            if (tmStart.Date >= tmEnd.Date)
            {
                return returnValue;
            }
            NET_TIME netStartTime = ToNetTime(tmStart);
            NET_TIME netEndTime = ToNetTime(tmEnd);
            returnValue = CLIENT_QueryRecordFile(pClientInfo.ID, nChannelId, nRecordFileType, ref netStartTime, ref netEndTime, pchCardid, ref nriFileinfo, maxlen, ref  filecount, waittime, bTime);
            ThrowLastError();
            return returnValue;
        }

        /// <summary>
        /// 网络回放[注意:用户登录一台设备后，每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录]
        /// </summary>
        /// <param name="lpRecordFile">录像文件信息, 当按时间播放是只需填充起始时间和结束时间, 其他项填0</param>
        /// <param name="hWnd">回放容器名柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        public long PlayBackByRecordFile(NET_RECORDFILE_INFO lpRecordFile, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData)
        {
            long returnValue;
            returnValue = CLIENT_PlayBackByRecordFile(pClientInfo.ID, ref lpRecordFile, hWnd, cbDownLoadPos, dwUserData);
            ThrowLastError();
            return returnValue;
        }

        /// <summary>
        /// 按时间范围网络回放[注意:用户登录一台设备后，每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录]
        /// </summary>
        /// <param name="nChannelID">通道</param>
        /// <param name="tmStart">播放开始时间</param>
        /// <param name="tmEnd">播放结束时间</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        public bool PlayBackByTime(int nChannelID, DateTime tmStart, DateTime tmEnd, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData)
        {
            bool returnValue=false;
            NET_TIME netStartTime = ToNetTime(tmStart);
            NET_TIME netEndTime = ToNetTime(tmEnd);
            int playHandel = CLIENT_PlayBackByTime(pClientInfo.ID,nChannelID,ref netStartTime,ref netEndTime,hWnd,cbDownLoadPos, dwUserData);
            if (playHandel != 0)
            {
                pClientInfo.Channel[nChannelID].blnPlayBack = true;
                pClientInfo.Channel[nChannelID].PlayBackHandle = playHandel;
                returnValue = true;
            }
            ThrowLastError();
            return returnValue;
        }

        ///// <summary>
        ///// 按时间回放。
        ///// </summary>        
        ///// <param name="channelId">通道号。</param>
        ///// <param name="startTime">起始时间。</param>
        ///// <param name="endTime">结束时间。</param>
        ///// <param name="hWnd">窗口句柄。</param>
        ///// <param name="downLoadPositonCallback">播放进度回调。</param>
        ///// <returns>返回playHandle。</returns>
        //public bool PlayBackByTime( int channelId, DateTime startTime,
        //                                 DateTime endTime, IntPtr hWnd, fDownLoadPosCallBack downLoadPositonCallback)
        //{

        //    NET_TIME netStartTime = ToNetTime(startTime);
        //    NET_TIME netEndTime = ToNetTime(endTime);
        //    int result =
        //        CLIENT_PlayBackByTime(pClientInfo.ID, channelId, ref netStartTime, ref netEndTime, hWnd,
        //                              downLoadPositonCallback, IntPtr.Zero);
        //    ThrowLastError();
        //    return result;
        //}

        /// <summary>
        /// 网络回放控制
        /// </summary>
        /// <param name="nChannelID">通道</param>
        /// <param name="playControl">播放控制命令</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        public bool PlayBackControl(int nChannelID, PlayCon playControl)
        {
            bool returnValue = false;
            switch (playControl)
            {
                case PlayCon.Play://播放
                    returnValue=CLIENT_PausePlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle, 0);
                    break;
                case PlayCon.Pause://暂停
                    returnValue=CLIENT_PausePlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle, 1);
                    break;
                case PlayCon.Slow://慢放
                    returnValue = CLIENT_SlowPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle);
                    break;
                case PlayCon.Fast://快进
                    returnValue = CLIENT_FastPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle);
                    break;
                case PlayCon.StepPlay://单步播放开始
                    returnValue = CLIENT_StepPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle, false);
                    break;
                case PlayCon.StepStop://单步播放停止
                    returnValue = CLIENT_StepPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle, true);
                    break;
                case PlayCon.Stop://停止播放
                    returnValue = CLIENT_StopPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle);
                    break;
                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// 拖动播放操作
        /// </summary>
        /// <param name="nChannelID">通道</param>
        /// <param name="playControl">播放控制命令</param>
        /// <param name="seekNum">相对文件开始处的偏移值</param>
        /// <returns></returns>
        public bool PlayBackControl(int nChannelID, PlayCon playControl,uint seekNum)
        { 
            bool returnValue = false;
            uint uTemp = 0xffffffff;
            switch (playControl)
            {  
                case PlayCon.SeekByTime://拖动播放[按时间偏移]
                    returnValue = CLIENT_SeekPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle, seekNum, uTemp);
                    break;
                case PlayCon.SeekByBit://拖动播放[按字节偏移]
                    returnValue = CLIENT_SeekPlayBack(pClientInfo.Channel[nChannelID].PlayBackHandle, uTemp, seekNum);
                    break;
                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        #endregion

        #region << 云台控制 >>

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="dwPTZCommand">云台控制命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>是否停止</returns>
        public bool PTZControl(int nChannelID, PTZControlType dwPTZCommand,char param1, char param2, char param3, bool dwStop)
        { 
            bool returnValue=false;
            switch (dwPTZCommand)
            {
                case PTZControlType.EXTPTZ_FASTGOTO://快速定位
                    CLIENT_DHPTZControl(pClientInfo.ID, nChannelID,(int) dwPTZCommand, param1, param2, param3, dwStop);
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="dwPTZCommand">云台控制命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>是否停止</returns>
        public bool PTZControl(int nChannelID, PTZControlType dwPTZCommand, char param1, char param2, bool dwStop)
        {
            bool returnValue = false;
            switch (dwPTZCommand)
            {
                case PTZControlType.EXTPTZ_LEFTTOP://左上
                case PTZControlType.EXTPTZ_RIGHTTOP://右上
                case PTZControlType.EXTPTZ_LEFTDOWN://左下
                case PTZControlType.EXTPTZ_RIGHTDOWN://右下
                case PTZControlType.EXTPTZ_ADDTOLOOP://加入预置点到巡航
                case PTZControlType.EXTPTZ_DELFROMLOOP://删除巡航中预置点
                case PTZControlType.EXTPTZ_AUXIOPEN://x34辅助开关开
                case PTZControlType.EXTPTZ_AUXICLOSE://0x35辅助开关关
                    CLIENT_DHPTZControl(pClientInfo.ID, nChannelID,(int) dwPTZCommand, param1, param2, ' ', dwStop);
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="dwPTZCommand">云台控制命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>是否停止</returns>
        public bool PTZControl(int nChannelID, PTZControlType dwPTZCommand, char param1, bool dwStop)
        {
            bool returnValue = false;
            return returnValue;
        }

        #endregion

        #endregion

        #endregion

        #region << 原有SDK(C++版)调用代码 >>

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_Init(fDisConnect cbDisConnect, IntPtr dwUser);

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_Login(string pchDVRIP, ushort wDVRPort, string pchUserName,
                                                 string pchPassword, out NET_DEVICEINFO lpDeviceInfo, out int error);

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_StartListen(Int32 lLoginID);

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_StopListen(Int32 lLoginID);

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_Logout(Int32 lLoginID);

        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_DownloadByTime(int lLoginID, int nChannelId, int nRecordFileType,
                                                        ref NET_TIME tmStart,
                                                        ref NET_TIME tmEnd, string sSavedFileName,
                                                        fTimeDownLoadPosCallBack cbTimeDownLoadPos, IntPtr dwUserData);

        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_StopDownload(int lFileHandle);



        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_PlayBackByTimeEx(int lLoginID, int nChannelID, ref NET_TIME lpStartTime,
                                                          ref NET_TIME lpStopTime, IntPtr hWnd,
                                                          fDownLoadPosCallBack cbDownLoadPos, int dwPosUser,
                                                          fDataCallBack fDownLoadDataCallBack, IntPtr dwDataUser);

        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_OpenSound(int hPlayHandle);

        #region << 实时监视接口 >>

        /// <summary>
        /// 启动实时监视
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="hWnd">监视播放容器句柄</param>
        /// <returns>失败返回0，成功返回实时监视ID(实时监视句柄)，将作为相关函数的参数</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_RealPlay(int lLoginID, int nChannelID, IntPtr hWnd);        

        /// <summary>
        /// 停止实时监视
        /// </summary>
        /// <param name="lRealHandle">实时监视句柄</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopRealPlay(int lRealHandle);
        
        #endregion


        #region << 回放和下载接口 >>
        
        /// <summary>
        /// 查询录像文件
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelId">通道ID</param>
        /// <param name="nRecordFileType">录像文件类型 </param>
        /// <param name="tmStart">录像开始时间</param>
        /// <param name="tmEnd">录像结束时间</param>
        /// <param name="pchCardid">卡号,只针对卡号查询有效，其他情况下可以填NULL</param>
        /// <param name="nriFileinfo">返回的录像文件信息，是一个NET_RECORDFILE_INFO结构数组</param>
        /// <param name="maxlen">nriFileinfo缓冲的最大长度;（单位字节，建议在100-200*sizeof(NET_RECORDFILE_INFO)之间）</param>
        /// <param name="filecount">返回的文件个数,属于输出参数最大只能查到缓冲满为止的录像记录</param>
        /// <param name="waittime">等待时间</param>
        /// <param name="bTime">是否按时间查(目前无效)</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryRecordFile(int lLoginID, int nChannelId, int nRecordFileType, ref NET_TIME tmStart,ref NET_TIME tmEnd, string pchCardid, ref NET_RECORDFILE_INFO nriFileinfo, int maxlen, ref  int filecount, int waittime, bool bTime);
                                   
        /// <summary>
        /// 网络回放[注意:用户登录一台设备后，每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录]
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="lpRecordFile">录像文件信息, 当按时间播放是只需填充起始时间和结束时间, 其他项填0</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern long CLIENT_PlayBackByRecordFile(int lLoginID,ref NET_RECORDFILE_INFO lpRecordFile, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData);

        /// <summary>
        /// 网络按时间回放
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="lpStartTime">录像开始时间</param>
        /// <param name="lpStopTime">录像结束时间</param>
        /// <param name="hWnd">录像回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_PlayBackByTime(int lLoginID, int nChannelID, ref NET_TIME lpStartTime,
                                                        ref NET_TIME lpStopTime, IntPtr hWnd,
                                                        fDownLoadPosCallBack cbDownLoadPos,
                                                        IntPtr dwUserData);
        /// <summary>
        /// 网络回放停止
        /// </summary>
        /// <param name="lPlayHandle">回放句柄</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopPlayBack(int lPlayHandle);

        /// <summary>
        /// 网络回放暂停与恢复播放
        /// </summary>
        /// <param name="lPlayHandle">播放句柄</param>
        /// <param name="bPause">1:暂停;0:恢复</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_PausePlayBack(int lPlayHandle, int bPause);

        /// <summary>
        /// 改变位置播放[即拖动播放，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义]
        /// </summary>
        /// <param name="lPlayHandle">播放句柄</param>
        /// <param name="offsettime">相对文件开始处偏移时间，单位为秒 .当其值为0xffffffff时,该参数无效.</param>
        /// <param name="offsetbyte">相对文件开始处偏移字节, 当其值为0xffffffff时, 该参数无效；当offsettime有效的时候,此参数无意义</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SeekPlayBack(int lPlayHandle, uint offsettime, uint offsetbyte);
        
        

        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StepPlayBack(int lPlayHandle, bool bStop);

        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_FastPlayBack(int lPlayHandle);

        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SlowPlayBack(int lPlayHandle);

          
        #endregion

        #region << 云台控制 >>

        /// <summary>
        /// 扩展云台控制
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">球机控制命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_DHPTZControl(int lLoginID, int nChannelID, int dwPTZCommand, char param1, char param2, char param3, bool dwStop);

        #endregion
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_CloseSound();

        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_SetVolume(int lPlayHandle, int nVolume);

        
        [DllImport("dhnetsdk.dll")]
        private static extern void CLIENT_Cleanup();

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_GetLastError();

        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_SetupDeviceTime(int lLoginID, ref NET_TIME pDeviceTime);

        #endregion

        #region << 委托 >>

        public delegate void fDisConnect(
            Int32 lLoginID, StringBuilder pchDVRIP, Int32 nDVRPort, IntPtr dwUser);

        public delegate void fDownLoadPosCallBack(int lPlayHandle, int dwTotalSize, int dwDownLoadSize, IntPtr dwUser);

        public delegate int fDataCallBack(int lRealHandle, int dwDataType, ref byte pBuffer, int dwBufSize, IntPtr dwUser);

        

        #endregion

        #region << 公共函数定义 >>

        /// <summary>
        /// Windows系统标准时间格式转为自定义格式
        /// </summary>
        /// <param name="dateTime">系统时间对象</param>
        /// <returns>自定义时间格式的时间数据</returns>
        private static NET_TIME ToNetTime(DateTime dateTime)
        {
            NET_TIME result = new NET_TIME();
            result.dwYear = dateTime.Year;
            result.dwMonth = dateTime.Month;
            result.dwDay = dateTime.Day;
            result.dwHour = dateTime.Hour;
            result.dwMinute = dateTime.Minute;
            result.dwSecond = dateTime.Second;
            return result;
        }

        #endregion
    }

    #region << 结构 >>
    /// <summary>
    /// 设备用户信息
    /// </summary>
    public struct CLIENT_INFO
    {
        public int ID;
        public string Name;
        public string Password;
        public string DeviceName;
        public string DeviceIP;
        public byte DeviceAlarmInPortCount;
        public int DevicePort;
        public byte DeviceAlarmOutPortCount;
        public byte DeviceChannelCount;
        public byte DeviceType;
        public byte DeviceDiskCount;
        /// <summary>
        /// 通道信息
        /// </summary>
        public DEVCHANNEL_INFO[] Channel;
        /// <summary>
        /// 通道初期化
        /// </summary>
        /// <returns>true:成功;false:失败</returns>
        public bool ChannelInit()
        {
            try
            {
                Channel = new DEVCHANNEL_INFO[DeviceChannelCount];
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 设备通道信息
    /// </summary>
    public struct DEVCHANNEL_INFO
    {
        /// <summary>
        /// 通道名
        /// </summary>
        public string Name;
        /// <summary>
        /// 通道实时监视标志
        /// </summary>
        public bool blnRealPlay;
        /// <summary>
        /// 实时监视句柄
        /// </summary>
        public int RealPlayHandle;
        /// <summary>
        /// 通道回放标志
        /// </summary>
        public bool blnPlayBack;
        /// <summary>
        /// 通道回放句柄
        /// </summary>
        public int PlayBackHandle;
        /// <summary>
        /// 亮度
        /// </summary>
        public int Bright;
        /// <summary>
        /// 对比度
        /// </summary>
        public int Contrast;
        /// <summary>
        /// 湿度
        /// </summary>
        public int Humidity;

    }

    /// <summary>
    /// 网络设备信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NET_DEVICEINFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] sSerialNumber; //序列号
        public byte byAlarmInPortNum; //DVR报警输入个数
        public byte byAlarmOutPortNum; //DVR报警输出个数
        public byte byDiskNum; //DVR硬盘个数
        public byte byDVRType; //DVR类型, 
        public byte byChanNum; //DVR通道个数
    }

    /// <summary>
    /// 网络时间
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NET_TIME
    {
        /// <summary>
        /// 年
        /// </summary>
        public int dwYear;
        /// <summary>
        /// 月
        /// </summary>
        public int dwMonth;
        /// <summary>
        /// 日
        /// </summary>
        public int dwDay;
        /// <summary>
        /// 时
        /// </summary>
        public int dwHour;
        /// <summary>
        /// 分
        /// </summary>
        public int dwMinute;
        /// <summary>
        /// 秒
        /// </summary>
        public int dwSecond;
    }

    ///// <summary>
    ///// 录像文件信息
    ///// </summary>
    //public struct NET_RECORDFILE_INFO
    //{
    //    /// <summary>
    //    /// 通道号
    //    /// </summary>
    //    public uint ch;
    //    /// <summary>
    //    /// 文件名
    //    /// </summary>
    //    [MarshalAs(UnmanagedType.ByValArray,ArraySubType= UnmanagedType.ByValTStr,SizeConst=128)]
    //    public char[] filename;
    //    /// <summary>
    //    /// 文件长度
    //    /// </summary>
    //    public uint size;
    //    /// <summary>
    //    /// 开始时间
    //    /// </summary>
    //    public NET_TIME starttime; 
    //    /// <summary>
    //    /// 结束时间
    //    /// </summary>
    //    public NET_TIME endtime; 
    //    /// <summary>
    //    /// 磁盘号
    //    /// </summary>
    //    public uint driveno; 
    //    /// <summary>
    //    /// 起始簇号
    //    /// </summary>
    //    public uint startcluster;
    //}

    /// <summary>
    /// 错误内容
    /// </summary>
    public struct ErrInfo
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string errCode;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errMessage;
    }
    #endregion

    #region << 枚举定义 >>

    /// <summary>
    /// 播放控制命令
    /// </summary>
    public enum PlayCon
    {
        /// <summary>
        /// 播放
        /// </summary>
        Play,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 拖动播放[按时间偏移]
        /// </summary>
        SeekByTime,
        /// <summary>
        /// 拖动播放[按字节偏移]
        /// </summary>
        SeekByBit,
        /// <summary>
        /// 单步播放开始[调用一次播放一帧]
        /// </summary>
        StepPlay,
        /// <summary>
        /// 单步播放停止
        /// </summary>
        StepStop,
        /// <summary>
        /// 快放
        /// </summary>
        Fast,
        /// <summary>
        /// 慢放
        /// </summary>
        Slow
    }

    /// <summary>
    /// 云台控制命令
    /// </summary>
    public enum PTZControlType
    {        
        /// <summary>
        /// 上
        /// </summary>
        PTZ_UP_CONTROL = 0,
        /// <summary>
        /// 下
        /// </summary>
        PTZ_DOWN_CONTROL,
        /// <summary>
        /// 左
        /// </summary>
        PTZ_LEFT_CONTROL,
        /// <summary>
        /// 右
        /// </summary>
        PTZ_RIGHT_CONTROL,
        /// <summary>
        /// 变倍+
        /// </summary>
        PTZ_ZOOM_ADD_CONTROL,
        /// <summary>
        /// 变倍-
        /// </summary>
        PTZ_ZOOM_DEC_CONTROL,
        /// <summary>
        /// 调焦+
        /// </summary>
        PTZ_FOCUS_ADD_CONTROL,
        /// <summary>
        /// 调焦- 
        /// </summary>
        PTZ_FOCUS_DEC_CONTROL,
        /// <summary>
        /// 光圈+ 
        /// </summary>
        PTZ_APERTURE_ADD_CONTROL,
        /// <summary>
        /// 光圈- 
        /// </summary>
        PTZ_APERTURE_DEC_CONTROL,
        /// <summary>
        /// 转至预置点 
        /// </summary>
        PTZ_POINT_MOVE_CONTROL,
        /// <summary>
        /// 设置 
        /// </summary>
        PTZ_POINT_SET_CONTROL,
        /// <summary>
        /// 删除 
        /// </summary>
        PTZ_POINT_DEL_CONTROL,
        /// <summary>
        /// 点间轮循 
        /// </summary>
        PTZ_POINT_LOOP_CONTROL,
        /// <summary>
        /// 灯光雨刷	
        /// </summary>
        PTZ_LAMP_CONTROL,
        /// <summary>
        /// 左上:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_LEFTTOP = 0X20,
        /// <summary>
        /// 右上:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_RIGHTTOP,
        /// <summary>
        /// 左下:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_LEFTDOWN,
        /// <summary>
        /// 右下:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_RIGHTDOWN,
        /// <summary>
        /// 加入预置点到巡航:p1巡航线路	p2预置点值
        /// </summary>
        EXTPTZ_ADDTOLOOP,
        /// <summary>
        /// 删除巡航中预置点:p1巡航线路	p2预置点值
        /// </summary>
        EXTPTZ_DELFROMLOOP, 
        /// <summary>
        /// 清除巡航:p1巡航线路	
        /// </summary>
        EXTPTZ_CLOSELOOP,
        /// <summary>
        /// 开始水平旋转
        /// </summary>
        EXTPTZ_STARTPANCRUISE,
        /// <summary>
        /// 停止水平旋转	
        /// </summary>
        EXTPTZ_STOPPANCRUISE,
        /// <summary>
        /// 设置左边界
        /// </summary>
        EXTPTZ_SETLEFTBORDER,
        /// <summary>
        /// 设置右边界		
        /// </summary>
        EXTPTZ_RIGHTBORDER,
        /// <summary>
        /// 开始线扫
        /// </summary>
        EXTPTZ_STARTLINESCAN,
        /// <summary>
        /// 停止线扫
        /// </summary>
        EXTPTZ_CLOSELINESCAN,
        /// <summary>
        /// 设置模式开始:模式线路
        /// </summary>
        EXTPTZ_SETMODESTART,
        /// <summary>
        /// 设置模式结束:模式线路
        /// </summary>
        EXTPTZ_SETMODESTOP,
		/// <summary>
        /// 运行模式:p1模式线路
		/// </summary>		
        EXTPTZ_RUNMODE,
        /// <summary>
        /// 停止模式:p1模式线路
        /// </summary>
        EXTPTZ_STOPMODE,
        /// <summary>
        /// 清除模式:p1模式线路
        /// </summary>
        EXTPTZ_DELETEMODE,
        /// <summary>
        /// 翻转命令
        /// </summary>
        EXTPTZ_REVERSECOMM,
        /// <summary>
        /// 快速定位  p1水平坐标 p2垂直坐标	 p3变倍
        /// </summary>
        EXTPTZ_FASTGOTO,
        /// <summary>
        /// x34:辅助开关开	 p1辅助点		
        /// </summary>
        EXTPTZ_AUXIOPEN,
        /// <summary>
        /// 0x35:辅助开关关	p1辅助点
        /// </summary>
        EXTPTZ_AUXICLOSE,
        /// <summary>
        /// 打开球机菜单
        /// </summary>
        EXTPTZ_OPENMENU = 0X36,
        /// <summary>
        /// 关闭菜单
        /// </summary>
        EXTPTZ_CLOSEMENU,
        /// <summary>
        /// 菜单确定
        /// </summary>
        EXTPTZ_MENUOK,
        /// <summary>
        /// 菜单取消
        /// </summary>
        EXTPTZ_MENUCANCEL,
        /// <summary>
        /// 菜单上
        /// </summary>
        EXTPTZ_MENUUP,
        /// <summary>
        /// 菜单下
        /// </summary>
        EXTPTZ_MENUDOWN,
        /// <summary>
        /// 菜单左
        /// </summary>
        EXTPTZ_MENULEFT,
        /// <summary>
        /// 菜单右
        /// </summary>
        EXTPTZ_MENURIGHT,
        /// <summary>
        /// 最大命令值
        /// </summary>
        EXTPTZ_TOTAL

    }
    #endregion
}

/*
 * ************************************************************************
 *                            SDK
 *                      大华网络SDK(C#版)
 * 
 * (c) Copyright 2007, ZheJiang Dahua Technology Stock Co.Ltd.
 *                      All Rights Reserved
 * 版 本 号:0.01
 * 文件名称:DaHuaSDK.cs
 * 功能说明:原始封装[在现有的SDK(C++版)上再一次封装,基本与原C++接口对应]
 * 作    者:李德明
 * 作成日期:2007/11/26
 * 修改日志:    日期        版本号      作者        变更事由
 *              2007/11/26  0.01        李德明      新建作成
 * 
 * ************************************************************************
 */
namespace DHNetSDK
{
    #region << DHClient类定义 >>

    /// <summary>
    /// 本开发包是在大华网络SDK(C++版)的基础上封装是对原SDK中C++接口转成C#风格的接口的开发包，
    /// 以便于应用程序开发使用。使用此开发包发布应用程序时需要将原有的SDK的动态链接库文件和本
    /// SDK文件放置在同一个目录下
    /// </summary>
    public sealed class DHClient
    {
        private static bool initialized = false;

        /// <summary>
        /// 是否将错误抛出[默认不抛出，只将错误信息返回给属性LastOperationInfo]
        /// </summary>
        private static bool pShowException = false;

        #region << 属性 >>

        /// <summary>
        /// 最后操作信息
        /// </summary>
        private static OPERATION_INFO pErrInfo;

        /// <summary>
        /// 最后操作信息[最后操作错误和成功,该属性只读]
        /// </summary>
        public static OPERATION_INFO LastOperationInfo
        {
            get
            {
                return pErrInfo;
            }
        }

        /// <summary>
        /// 字符编码格式[默认:gb2312 → 936]
        /// </summary>
        private static Encoding pEncoding=Encoding.GetEncoding(936);

        /// <summary>
        /// 字符编码格式
        /// </summary>
        public static Encoding EncodingInfo
        {
            get
            {
                return pEncoding;
            }
            set
            {
                pEncoding = value;
            }
        }
        #endregion

        #region << 类方法 >>

        #region << 私有方法 >>

        /// <summary>
        /// SDK调用失败时抛出异常,成功时返回无异常信息,并把操作信息赋给LastOperationInfo
        /// </summary>
        /// <exception cref="Win32Exception">数字录像机原生异常,当SDK调用失败时触发</exception>
        private static void DHThrowLastError()
        {
            Int32 errorCode = CLIENT_GetLastError();
            if (errorCode != 0)
            {
                pErrInfo.errCode = errorCode.ToString();
                pErrInfo.errMessage = DHGetLastErrorName((uint)errorCode);
                if (pShowException == true)
                {
                    throw new Win32Exception(errorCode, pErrInfo.errMessage);
                }
            }
            else
            {
                pErrInfo.errCode = "0";
                pErrInfo.errMessage = "最近操作无异常发生";
            }
        }

        /// <summary>
        /// 错误代码转换为标准备的错误信息描述
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <returns>标准备错误信息描述</returns>
        private static string DHGetLastErrorName(uint errorCode)
        {
            switch (errorCode)
            {
                case 0x80000000 | 1:
                    return "Windows系统出错";
                case 0x80000000 | 2:
                    return "网络错误，可能是因为网络超时";
                case 0x80000000 | 3:
                    return "设备协议不匹配";
                case 0x80000000 | 4:
                    return "句柄无效";
                case 0x80000000 | 5:
                    return "打开通道失败";
                case 0x80000000 | 6:
                    return "关闭通道失败";
                case 0x80000000 | 7:
                    return "用户参数不合法";
                case 0x80000000 | 8:
                    return "SDK初始化出错";
                case 0x80000000 | 9:
                    return "SDK清理出错";
                case 0x80000000 | 10:
                    return "申请render资源出错";
                case 0x80000000 | 11:
                    return "打开解码库出错";
                case 0x800000 | 12:
                    return "关闭解码库出错";
                case 0x80000000 | 13:
                    return "多画面预览中检测到通道数为0";
                case 0x80000000 | 14:
                    return "录音库初始化失败";
                case 0x80000000 | 15:
                    return "录音库未经初始化";
                case 0x80000000 | 16:
                    return "发送音频数据出错";
                case 0x80000000 | 17:
                    return "实时数据已经处于保存状态";
                case 0x80000000 | 18:
                    return "未保存实时数据";
                case 0x80000000 | 19:
                    return "打开文件出错";
                case 0x80000000 | 20:
                    return "启动云台控制定时器失败";
                case 0x80000000 | 21:
                    return "对返回数据的校验出错";
                case 0x80000000 | 22:
                    return "没有足够的缓存";
                case 0x80000000 | 23:
                    return "当前SDK未支持该功能";
                case 0x80000000 | 24:
                    return "查询不到录像";
                case 0x80000000 | 25:
                    return "无操作权限";
                case 0x80000000 | 26:
                    return "暂时无法执行";
                case 0x80000000 | 27:
                    return "未发现对讲通道";
                case 0x80000000 | 28:
                    return "未发现音频";
                case 0x80000000 | 29:
                    return "CLientSDK未经初始化";
                case 0x80000000 | 30:
                    return "下载已结束";
                case 0x80000000 | 31:
                    return "查询结果为空";
                case 0x80000000 | 32:
                    return "获取配置失败位置：系统属性";
                case 0x80000000 | 33:
                    return "获取配置失败位置：序列号";
                case 0x80000000 | 34:
                    return "获取配置失败位置：常规属性";
                case 0x80000000 | 35:
                    return "获取配置失败位置：DSP能力描述";
                case 0x80000000 | 36:
                    return "获取配置失败位置：网络属性";
                case 0x80000000 | 37:
                    return "获取配置失败位置：通道名称";
                case 0x80000000 | 38:
                    return "获取配置失败位置：视频属性";
                case 0x80000000 | 39:
                    return "获取配置失败位置：录像定时配置";
                case 0x80000000 | 40:
                    return "获取配置失败位置：解码器协议名称";
                case 0x80000000 | 41:
                    return "获取配置失败位置：232串口功能名称";
                case 0x80000000 | 42:
                    return "获取配置失败位置：解码器属性";
                case 0x80000000 | 43:
                    return "获取配置失败位置：232串口属性";
                case 0x80000000 | 44:
                    return "取配置失败位置：外部报警输入属性";
                case 0x80000000 | 45:
                    return "获取配置失败位置：图像检测报警属性";
                case 0x80000000 | 46:
                    return "获取配置失败位置：设备时间";
                case 0x80000000 | 47:
                    return "获取配置失败位置：预览参数";
                case 0x80000000 | 48:
                    return "获取配置失败位置：自动维护配置";
                case 0x80000000 | 49:
                    return "获取配置失败位置：视频矩阵配置";
                case 0x80000000 | 55:
                    return "设置配置失败位置：常规属性";
                case 0x80000000 | 56:
                    return "设置配置失败位置：网络属性";
                case 0x80000000 | 57:
                    return "设置配置失败位置：通道名称";
                case 0x80000000 | 58:
                    return "设置配置失败位置：视频属性";
                case 0x80000000 | 59:
                    return "设置配置失败位置：录像定时配置";
                case 0x80000000 | 60:
                    return "设置配置失败位置：解码器属性";
                case 0x80000000 | 61:
                    return "设置配置失败位置：232串口属性";
                case 0x80000000 | 62:
                    return "设置配置失败位置：外部报警输入属性";
                case 0x80000000 | 63:
                    return "设置配置失败位置：图像检测报警属性";
                case 0x80000000 | 64:
                    return "设置配置失败位置：设备时间";
                case 0x80000000 | 65:
                    return "设置配置失败位置：预览参数";
                case 0x80000000 | 66:
                    return "设置配置失败位置：自动维护配置";
                case 0x80000000 | 67:
                    return "设置配置失败位置：视频矩阵配置";
                case 0x80000000 | 70:
                    return "音频编码接口没有成功初始化";
                case 0x80000000 | 71:
                    return "数据过长";
                case 0x80000000 | 72:
                    return "设备不支持该操作";
                case 0x80000000 | 73:
                    return "设备资源不足";
                case 0x80000000 | 74:
                    return "服务器已经启动";
                case 0x80000000 | 75:
                    return "服务器尚未成功启动";
                case 0x80000000 | 80:
                    return "输入序列号有误";
                case 0x80000000 | 100:
                    return "密码不正确";
                case 0x80000000 | 101:
                    return "帐户不存在";
                case 0x80000000 | 102:
                    return "等待登录返回超时";
                case 0x80000000 | 103:
                    return "帐号已登录";
                case 0x80000000 | 104:
                    return "帐号已被锁定";
                case 0x80000000 | 105:
                    return "帐号已被列为黑名单";
                case 0x80000000 | 106:
                    return "资源不足，系统忙";
                case 0x80000000 | 107:
                    return "连接主机失败";
                case 0x80000000 | 108:
                    return "网络连接失败";
                case 0x80000000 | 120:
                    return "Render库打开音频出错";
                case 0x80000000 | 121:
                    return "Render库关闭音频出错";
                case 0x80000000 | 122:
                    return "Render库控制音量出错";
                case 0x80000000 | 123:
                    return "Render库设置画面参数出错";
                case 0x80000000 | 124:
                    return "Render库暂停播放出错";
                case 0x80000000 | 125:
                    return "Render库抓图出错";
                case 0x80000000 | 126:
                    return "Render库步进出错";
                case 0x80000000 | 127:
                    return "Render库设置帧率出错";
                case 0x80000000 | 999:
                    return "暂时无法设置";
                case 0x80000000 | 1000:
                    return "配置数据不合法";
                default:
                    return "未知错误";
            }
        }

        private static void DHThrowLastError(int returnValue)
        {
            if (returnValue == 0)
            {
                DHThrowLastError();
            }
            else
            {
                pErrInfo.errCode = "0";
                pErrInfo.errMessage = "最近操作无异常发生";
            }
        }

        private static void DHThrowLastError(bool returnValue)
        {
            if (returnValue == false)
            {
                DHThrowLastError();
            }
            else
            {
                pErrInfo.errCode = "0";
                pErrInfo.errMessage = "最近操作无异常发生";
            }
        }

        /// <summary>
        /// SDK调用失败时抛出异常
        /// </summary>
        /// <param name="e"></param>
        private static void DHThrowLastError(Exception e)
        {

            pErrInfo.errCode = e.ToString();
            pErrInfo.errMessage = e.Message;
            if (pShowException == true)
            {
                throw e;
            }
        }

        #endregion

        #region << 公用方法 >>

        #region << 设置抛错方式 >>

        /// <summary>
        /// 设置使用本类的程序中的是否将错误信息抛出[默认不抛出，只将错误信息返回给属性LastOperationInfo]
        /// </summary>
        /// <param name="blnShowException">true:抛出错误信息,并将错误信息返回给属性LastOperationInfo,由客户自行处理;不抛出错误信息,将错误信息返回给属性LastOperationInfo,不作任何处理,用户可根据方法的返回值或LastOperationInfo判断是否有错误发生,然后再作相应处理</param>
        public static void DHSetShowException(bool blnShowException)
        {
            pShowException = blnShowException;
        }

        #endregion

        #region << 字符编码 >>

        /// <summary>
        /// 设置字符编码格式[默认为:gb2312,其他字符编码请通过本函数设置]
        /// </summary>
        /// <param name="encodingNum">字符编码整数</param>
        public static void DHSetEncoding(int encodingNum)
        {
            pEncoding = Encoding.GetEncoding(encodingNum);
        }

        /// <summary>
        /// 设备字符编码格式[默认为:gb2312,其他字符编码请通过本函数设置]
        /// </summary>
        /// <param name="encodingString">字符编码字符串，参见MSDN中的Encoding</param>
        public static void DHSetEncoding(string encodingString)
        {
            pEncoding = Encoding.GetEncoding(encodingString);
        }

        /// <summary>
        /// 设备字符编码格式[默认为:gb2312,其他字符编码请通过本函数设置]
        /// </summary>
        /// <param name="encoding">字符编码枚举,参见LANGUAGE_ENCODING枚举类型</param>
        public static void DHSetEncoding(LANGUAGE_ENCODING encoding)
        {
            pEncoding =  Encoding.GetEncoding((int)encoding);
        }

        /// <summary>
        /// 设备字符编码格式[默认为:gb2312,其他字符编码请通过本函数设置]
        /// </summary>
        /// <param name="encoding">字符编码</param>
        public static void DHSetEncoding(Encoding encoding)
        {
            pEncoding = encoding;
        }

        #endregion

        #region << SDK初期化和信息获取 >>

        /// <summary>
        /// 初始化SDK
        /// </summary>
        /// <param name="cbDisConnect">
        /// 断线回调函数,参见委托:<seealso cref="fDisConnect"/>
        /// </param>
        /// <param name="dwUser">用户数据,没有数据请使用IntPtr.Zero</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHInit(fDisConnect cbDisConnect, IntPtr dwUser)
        {
            bool returnValue = false;
            if (initialized == false)
            {
                returnValue = CLIENT_Init(cbDisConnect, dwUser);
                DHThrowLastError(returnValue);
                initialized = true;
            }
            return returnValue;
        }

        /// <summary>
        /// 清空SDK, 释放占用的资源，在所有的SDK函数之后调用
        /// </summary>
        public static void DHCleanup()
        {
            CLIENT_Cleanup();
            //DHThrowLastError();
        }

        /// <summary>
        /// 设置与设备的连接等待时间
        /// </summary>
        /// <param name="nWaitTime">连接等待时间[单位:毫秒]</param>
        /// <param name="nTryTimes">连接次数</param>
        public static void DHSetConnectTime(int nWaitTime, int nTryTimes)
        {
            CLIENT_SetConnectTime(nWaitTime, nTryTimes);
            //DHThrowLastError();
        }

        /// <summary>
        /// 得到SDK的版本号
        /// </summary>
        /// <param name="formatStyle">格式[S:00.00.00.00;D:0.0.0.0;S3:00.00.00;D3:0.0.0;X:十六进制表示版本号信息]</param>
        /// <returns>版本号</returns>
        public static string DHGetSDKVersion(string formatStyle)
        {
            UInt32 returnValue = CLIENT_GetSDKVersion();
            //转换为16进制表示值       
            string returnString = returnValue.ToString("X");
            //格式化版本号
            string strTemp = "00000000";
            strTemp = strTemp.Remove(0, returnString.Length) + returnString;
            string strVerPart1 = strTemp.Substring(0, 2);
            string strVerPart2 = strTemp.Substring(2, 2);
            string strVerPart3 = strTemp.Substring(4, 2);
            string strVerPart4 = strTemp.Substring(6, 2);
            switch (formatStyle.ToUpper())
            {
                case "S":
                    returnString = strVerPart1 + "." + strVerPart2 + "." + strVerPart3 + "." + strVerPart4;
                    break;
                case "D":
                    returnString = int.Parse(strVerPart1).ToString() + "." + int.Parse(strVerPart2).ToString() + "." + int.Parse(strVerPart3).ToString() + "." + int.Parse(strVerPart4).ToString();
                    break;
                case "S3":
                    returnString = strVerPart1 + "." + int.Parse(strVerPart2).ToString() +int.Parse(strVerPart3).ToString() + "." + strVerPart4;
                    break;
                case "D3":
                    returnString = int.Parse(strVerPart1).ToString() + "." + int.Parse(strVerPart2).ToString() + int.Parse(strVerPart3).ToString() + "." + int.Parse(strVerPart4).ToString();
                    break;
                case "X":
                    returnString = returnValue.ToString("X");
                    break;
                default:
                    returnString = strVerPart1 + "." + strVerPart2 + "." + strVerPart3;
                    break;
            }
            DHThrowLastError(int.Parse(returnValue.ToString()));
            return returnString;
        }

        /// <summary>
        /// 得到SDK的版本号(格式为S3)
        /// </summary>
        /// <returns>版本号</returns>
        public static string DHGetSDKVersion()
        {
            return DHGetSDKVersion("S3");
        }

        #endregion

        #region << 字符叠加 >>

        /// <summary>
        /// 用户自定义画图, 在打开图像之前调用此函数,否则无效,必须在所有窗口未显示之前调用, 可以用来对画面进行字符叠加
        /// </summary>
        /// <param name="cbDraw">画图回调，当设置为0时表示禁止回调</param>
        /// <param name="dwUser">用户数据,没有数据请使用IntPtr.Zero</param>
        public static void DHRigisterDrawFun(fDrawCallBack cbDraw, IntPtr dwUser)
        {   
            CLIENT_RigisterDrawFun(cbDraw,dwUser);
            //DHThrowLastError();
        }

        #endregion

        #region << 参数配置 >>

        #region << 获取参数 >>

        /// <summary>
        ///  获取设备配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配置类型</param>
        /// <param name="lChannel">通道号，如果不是通道参数，lChannel不用,置为-1即可</param>
        /// <param name="lpOutBuffer">存放输出参数的缓冲区, 根据不同的类型, 输出不同的配置结构, 具体见数据结构定义</param>
        /// <param name="dwOutBufferSize">输入缓冲区的大小, (单位字节).</param>
        /// <param name="lpBytesReturned">实际返回的缓冲区大小,对应配置结构的大小, (单位字节)</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, CONFIG_COMMAND dwCommand, int lChannel, IntPtr lpOutBuffer, UInt32 dwOutBufferSize, ref UInt32 lpBytesReturned, int waittime)
        {
            bool returnValue = false;
            returnValue = CLIENT_GetDevConfig(lLoginID, (UInt32)dwCommand, lChannel, lpOutBuffer, dwOutBufferSize, ref lpBytesReturned, waittime);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        ///  获取设备配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配置类型</param>
        /// <param name="lChannel">通道号，如果不是通道参数，lChannel不用,置为-1即可</param>
        /// <param name="lpOutBuffer">存放输出参数的缓冲区, 根据不同的类型, 输出不同的配置结构, 具体见数据结构定义</param>
        /// <param name="dwOutBufferSize">输入缓冲区的大小, (单位字节).</param>
        /// <param name="lpBytesReturned">实际返回的缓冲区大小,对应配置结构的大小, (单位字节)</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, CONFIG_COMMAND dwCommand, int lChannel, IntPtr lpOutBuffer, UInt32 dwOutBufferSize, ref UInt32 lpBytesReturned)
        {
            return DHGetDevConfig(lLoginID, dwCommand, lChannel, lpOutBuffer, dwOutBufferSize, ref lpBytesReturned, 3000);            
        }

        /// <summary>
        /// 内部使用的取设备信息命令函数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="dwCommand">配制命令</param>
        /// <param name="obj">object对象</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        private static bool PGetDevConfig(int lLoginID, int lChannel, CONFIG_COMMAND dwCommand, ref object obj, Type typeName, int waittime)
        {
            bool returnValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {                
                UInt32 returnBuffSize = 0;
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeName));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    returnValue = DHGetDevConfig(lLoginID, dwCommand, lChannel, pBoxInfo, (UInt32)Marshal.SizeOf(typeName), ref returnBuffSize, waittime);
                    if (returnValue == true)
                    {
                        obj = Marshal.PtrToStructure((IntPtr)((UInt32)pBoxInfo), typeName);
                    }
                }
                DHThrowLastError(returnValue);
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                returnValue= false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;                
            }
            return returnValue;
        }

        /// <summary>
        /// 内部使用的取设备所有信息命令函数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="dwCommand">配制命令</param>
        /// <param name="obj">object对象</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        private static bool PGetDevConfig(int lLoginID, CONFIG_COMMAND dwCommand, ref object[] obj, Type typeName, int waittime)
        {
            bool returnValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {
                UInt32 returnBuffSize = 0;
                pBoxInfo = Marshal.AllocHGlobal(obj.Length * Marshal.SizeOf(typeName));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    returnValue = DHGetDevConfig(lLoginID, dwCommand, -1, pBoxInfo, (uint)obj.Length * (UInt32)Marshal.SizeOf(typeName), ref returnBuffSize, waittime);
                    if (returnValue == true)
                    {
                        for (int loop = 0; loop < obj.Length; loop++)
                        {
                            obj[loop] = Marshal.PtrToStructure((IntPtr)((UInt32)pBoxInfo + (UInt32)Marshal.SizeOf(typeName) * loop), typeName);
                        }
                    }
                }
                DHThrowLastError(returnValue);
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                returnValue= false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
            }
            return returnValue;            
        }

        /// <summary>
        ///  获取设备配置[设备信息配置]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="systemAttrConfig">设备信息</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_SYSTEM_ATTR_CFG systemAttrConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, 0, CONFIG_COMMAND.DH_DEV_DEVICECFG, ref result, typeof(DHDEV_SYSTEM_ATTR_CFG),waittime);
            systemAttrConfig = (DHDEV_SYSTEM_ATTR_CFG)result;
            return returnValue;
        }
        
        /// <summary>
        ///  获取设备配置[设备信息配置默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="systemAttrConfig">设备信息</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_SYSTEM_ATTR_CFG systemAttrConfig)
        {
            return DHGetDevConfig(lLoginID, ref systemAttrConfig, 3000);
        }

        /// <summary>
        ///  获取网络参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">网络参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_NET_CFG netConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, 0, CONFIG_COMMAND.DH_DEV_NETCFG, ref result, typeof(DHDEV_NET_CFG), waittime);
            netConfig = (DHDEV_NET_CFG)result;
            return returnValue;

        }

        /// <summary>
        ///  获取网络参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">网络参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_NET_CFG netConfig)
        {
            return DHGetDevConfig(lLoginID, ref netConfig, 3000);
        }

        /// <summary>
        ///  获取指定通道配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号[此值不能为-1]</param>
        /// <param name="netConfig">通道配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, int lChannel, ref DHDEV_CHANNEL_CFG channelConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, lChannel, CONFIG_COMMAND.DH_DEV_CHANNELCFG, ref result, typeof(DHDEV_CHANNEL_CFG), waittime);
            channelConfig = (DHDEV_CHANNEL_CFG)result;
            return returnValue;
        }

        /// <summary>
        ///  获取指定通道配置[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号[此值不能为-1]</param>
        /// <param name="netConfig">通道配置</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, int lChannel, ref DHDEV_CHANNEL_CFG channelConfig)
        {
            return DHGetDevConfig(lLoginID, lChannel, ref channelConfig);
        }

        /// <summary>
        ///  获取所有通道配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">通道配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_CHANNEL_CFG[] channelConfig, int waittime)
        {
            object[] result = new object[channelConfig.Length];
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, CONFIG_COMMAND.DH_DEV_CHANNELCFG, ref result, typeof(DHDEV_CHANNEL_CFG), waittime);
            for(int i=0;i<channelConfig.Length;i++)
            {
                channelConfig[i] = (DHDEV_CHANNEL_CFG)result[i];
            }            
            return returnValue;
        }

        /// <summary>
        ///  获取所有通道配置[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">通道配置</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_CHANNEL_CFG[] channelConfig)
        {
            return DHGetDevConfig(lLoginID, ref channelConfig, 3000);
        }

        /// <summary>
        ///  获取指定通道的录像配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">录像配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, int lChannel, ref DHDEV_RECORD_CFG recordConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, lChannel, CONFIG_COMMAND.DH_DEV_RECORDCFG, ref result, typeof(DHDEV_RECORD_CFG), waittime);
            recordConfig = (DHDEV_RECORD_CFG)result;
            return returnValue;
        }

        /// <summary>
        ///  获取指定通道的录像配置[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">录像配置</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, int lChannel, ref DHDEV_RECORD_CFG recordConfig)
        {
            return DHGetDevConfig(lLoginID, lChannel, ref recordConfig, 3000);
        }

        /// <summary>
        ///  获取所有通道的录像配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">录像配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_RECORD_CFG[] recordConfig, int waittime)
        {
            object[] result = new object[recordConfig.Length];
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, CONFIG_COMMAND.DH_DEV_RECORDCFG, ref result, typeof(DHDEV_RECORD_CFG), waittime);
            for (int i = 0; i < recordConfig.Length; i++)
            {
                recordConfig[i] = (DHDEV_RECORD_CFG)result[i];
            }
            return returnValue;
        }

        /// <summary>
        ///  获取所有通道的录像配置[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">录像配置</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_RECORD_CFG[] recordConfig)
        {
            return DHGetDevConfig(lLoginID, ref recordConfig, 3000);
        }

        /// <summary>
        ///  获取预览参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_PREVIEW_CFG previewConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_PREVIEWCFG, ref result, typeof(DHDEV_PREVIEW_CFG), waittime);
            previewConfig = (DHDEV_PREVIEW_CFG)result;
            return returnValue;
        }

        /// <summary>
        ///  获取预览参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_PREVIEW_CFG previewConfig)
        {
            return DHGetDevConfig(lLoginID, ref previewConfig, 3000);
        }

        /// <summary>
        ///  获取串口参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_COMM_CFG commConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_COMMCFG, ref result, typeof(DHDEV_COMM_CFG), waittime);
            commConfig = (DHDEV_COMM_CFG)result;
            return returnValue;
        }

        /// <summary>
        ///  获取串口参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_COMM_CFG commConfig)
        {
            return DHGetDevConfig(lLoginID, ref commConfig, 3000);
        }

        /// <summary>
        ///  获取报警参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="alarmConfig">报警参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_ALARM_SCHEDULE alarmConfig, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, 0, CONFIG_COMMAND.DH_DEV_ALARMCFG, ref result, typeof(DHDEV_ALARM_SCHEDULE), waittime);
            alarmConfig = (DHDEV_ALARM_SCHEDULE)result;
            return returnValue;
        }

        /// <summary>
        ///  获取报警参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="alarmConfig">报警参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref DHDEV_ALARM_SCHEDULE alarmConfig)
        {
            return DHGetDevConfig(lLoginID, ref alarmConfig, 3000);

        }

        /// <summary>
        ///  获取DVR时间
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dvrTime">时间参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref NET_TIME dvrTime, int waittime)
        {
            object result = new object();
            bool returnValue = false;
            returnValue = PGetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_TIMECFG, ref result, typeof(NET_TIME), waittime);
            dvrTime = (NET_TIME)result;
            return returnValue;
        }

        /// <summary>
        ///  获取DVR时间[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dvrTime">时间参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDevConfig(int lLoginID, ref NET_TIME dvrTime)
        {
            return DHGetDevConfig(lLoginID, ref dvrTime, 3000);
        }

        #region << 对讲参数以后的信息因与机型有关故第一版SDK中暂不实现[暂时屏蔽] >>

        ///// <summary>
        /////  获取对讲参数
        ///// </summary>
        ///// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        ///// <param name="dvrTime">对讲参数</param>
        ///// <returns>true:成功;false:失败</returns>
        //public static bool DHGetDevConfig(int lLoginID, ref DHDEV_TALK_CFG talkConfig)
        //{
        //    object result = new object();
        //    bool returnValue = false;
        //    returnValue = PGetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_TALKCFG, ref result, typeof(DHDEV_TALK_CFG));
        //    talkConfig = (DHDEV_TALK_CFG)result;
        //    return returnValue;
        //}

        ///// <summary>
        /////  获取自动维护参数
        ///// </summary>
        ///// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        ///// <param name="dvrTime">自动维护参数</param>
        ///// <returns>true:成功;false:失败</returns>
        //public static bool DHGetDevConfig(int lLoginID, ref DHDEV_TALK_CFG autoConfig)
        //{
        //    object result = new object();
        //    bool returnValue = false;
        //    returnValue = PGetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_AUTOMTCFG, ref result, typeof(DHDEV_TALK_CFG));
        //    autoConfig = (DHDEV_TALK_CFG)result;
        //    return returnValue;
        //}

        #endregion

        #endregion

        #region << 设置参数 >>

        /// <summary>
        /// 设置设备配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配置类型</param>
        /// <param name="lChannel">通道号，如果设置全部通道数据为0xFFFFFFFF(-1)，如果命令不需要通道号，该参数无效</param>
        /// <param name="lpInBuffer">存放输出参数的缓冲区, 根据不同的类型, 输入不同的配置结构, 具体见数据结构定义</param>
        /// <param name="dwInBufferSize">输入缓冲区的大小, (单位字节).</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, CONFIG_COMMAND dwCommand, int lChannel, IntPtr lpInBuffer, UInt32 dwInBufferSize, int waittime)
        {
            bool returnValue = false;
            returnValue = CLIENT_SetDevConfig(lLoginID, (UInt32)dwCommand, lChannel, lpInBuffer, dwInBufferSize, waittime);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 设置设备配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配置类型</param>
        /// <param name="lChannel">通道号，如果设置全部通道数据为0xFFFFFFFF(-1)，如果命令不需要通道号，该参数无效</param>
        /// <param name="lpOutBuffer">存放输出参数的缓冲区, 根据不同的类型, 输入不同的配置结构, 具体见数据结构定义</param>
        /// <param name="dwOutBufferSize">输入缓冲区的大小(单位字节).</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, CONFIG_COMMAND dwCommand, int lChannel, IntPtr lpInBuffer, UInt32 dwInBufferSize)
        {
            return DHSetDevConfig(lLoginID, dwCommand, lChannel, lpInBuffer, dwInBufferSize, 3000);
        }

        /// <summary>
        /// 内部使用的设置设备信息命令函数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号，如果设置全部通道数据为0xFFFFFFFF(-1)，如果命令不需要通道号，该参数无效</param>
        /// <param name="dwCommand">配制命令</param>
        /// <param name="obj">object对象</param>
        /// <param name="typeName">结构类型</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        private static bool PSetDevConfig(int lLoginID, int lChannel, CONFIG_COMMAND dwCommand, object obj, Type typeName, int waittime)
        {
            bool returnValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeName));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    Marshal.StructureToPtr(obj, pBoxInfo, true);
                    returnValue = DHSetDevConfig(lLoginID, dwCommand, lChannel, pBoxInfo, (UInt32)Marshal.SizeOf(typeName), waittime);
                }
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                returnValue= false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
            }
            return returnValue;
            
        }

        /// <summary>
        /// 内部使用的设置设备所有信息命令函数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配制命令</param>
        /// <param name="obj">object对象</param>
        /// <param name="typeName">结构类型</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        private static bool PSetDevConfig(int lLoginID, CONFIG_COMMAND dwCommand,object[] obj, Type typeName, int waittime)
        {
            bool returnValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {
                pBoxInfo = Marshal.AllocHGlobal(obj.Length * Marshal.SizeOf(typeName));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    for (int loop = 0; loop < obj.Length; loop++)
                    {
                        Marshal.StructureToPtr(obj[loop], (IntPtr)((UInt32)pBoxInfo + (UInt32)Marshal.SizeOf(typeName) * loop), true);
                    }
                    returnValue = DHSetDevConfig(lLoginID, dwCommand, -1, pBoxInfo, (uint)obj.Length * (UInt32)Marshal.SizeOf(typeName), waittime);
                }
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                returnValue= false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
                
            }
            return returnValue;
        }

        /// <summary>
        /// 设备属性设定
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="systemAttrConfig">设备属性</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID,DHDEV_SYSTEM_ATTR_CFG systemAttrConfig, int waittime)
        {
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, 0, CONFIG_COMMAND.DH_DEV_DEVICECFG, (object)systemAttrConfig, typeof(DHDEV_SYSTEM_ATTR_CFG), waittime);
            return returnValue;
        }

        /// <summary>
        /// 设备属性设定[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="systemAttrConfig">设备属性</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_SYSTEM_ATTR_CFG systemAttrConfig)
        {
            return DHSetDevConfig(lLoginID, systemAttrConfig, 3000);
        }

        /// <summary>
        /// 设定网络参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">网络参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_NET_CFG netConfig, int waittime)
        {            
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, 0, CONFIG_COMMAND.DH_DEV_NETCFG, (object)netConfig, typeof(DHDEV_NET_CFG), waittime);           
            return returnValue;
        }

        /// <summary>
        /// 设定网络参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">网络参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_NET_CFG netConfig)
        {
            return DHSetDevConfig(lLoginID, netConfig, 3000);
        }

        /// <summary>
        /// 设定指定通道配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号[此值不能为-1]</param>
        /// <param name="netConfig">通道配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, int lChannel, DHDEV_CHANNEL_CFG channelConfig, int waittime)
        {            
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, lChannel, CONFIG_COMMAND.DH_DEV_CHANNELCFG,(object)channelConfig, typeof(DHDEV_CHANNEL_CFG), waittime);           
            return returnValue;
        }

        /// <summary>
        /// 设定指定通道配置[默认时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号[此值不能为-1]</param>
        /// <param name="netConfig">通道配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, int lChannel, DHDEV_CHANNEL_CFG channelConfig)
        {
            return DHSetDevConfig(lLoginID, lChannel, channelConfig, 3000);
        }

        /// <summary>
        ///  设定所有通道配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">通道配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_CHANNEL_CFG[] channelConfig, int waittime)
        {
            object[] result = new object[channelConfig.Length];
            for (int i = 0; i < channelConfig.Length; i++)
            {
                result[i] = (object)channelConfig[i];
            }
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, CONFIG_COMMAND.DH_DEV_CHANNELCFG, result, typeof(DHDEV_CHANNEL_CFG), waittime);
            
            return returnValue;
        }

        /// <summary>
        /// 设定所有通道配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">通道配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_CHANNEL_CFG[] channelConfig)
        {
            return DHSetDevConfig(lLoginID, channelConfig, 3000);
        }

        /// <summary>
        /// 设定指定通道的录像配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">录像配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, int lChannel,DHDEV_RECORD_CFG recordConfig, int waittime)
        {
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, lChannel, CONFIG_COMMAND.DH_DEV_RECORDCFG, (object)recordConfig, typeof(DHDEV_RECORD_CFG), waittime);
            return returnValue;
        }

        /// <summary>
        /// 设定指定通道的录像配置[默认时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="netConfig">录像配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, int lChannel, DHDEV_RECORD_CFG recordConfig)
        {
            return DHSetDevConfig(lLoginID, lChannel, recordConfig, 600);
        }

        /// <summary>
        /// 设定所有通道的录像配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">录像配置</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_RECORD_CFG[] recordConfig, int waittime)
        {
            object[] result = new object[recordConfig.Length];
            for (int i = 0; i < recordConfig.Length; i++)
            {
                result[i]=(object)recordConfig[i];
            }
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, CONFIG_COMMAND.DH_DEV_RECORDCFG, result, typeof(DHDEV_RECORD_CFG), waittime);
            
            return returnValue;
        }

        /// <summary>
        /// 设定所有通道的录像配置[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">录像配置</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_RECORD_CFG[] recordConfig)
        {
            return DHSetDevConfig(lLoginID, recordConfig, 3000);
        }


        /// <summary>
        /// 设定预览参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID,DHDEV_PREVIEW_CFG previewConfig, int waittime)
        {
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_PREVIEWCFG, (object)previewConfig, typeof(DHDEV_PREVIEW_CFG), waittime);
            return returnValue;
        }

        /// <summary>
        /// 设定预览参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID,DHDEV_PREVIEW_CFG previewConfig)
        {
            return DHSetDevConfig(lLoginID,previewConfig, 3000);
        }

        /// <summary>
        /// 设定串口参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_COMM_CFG commConfig, int waittime)
        {
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_COMMCFG, (object)commConfig, typeof(DHDEV_COMM_CFG), waittime);
            return returnValue;
        }

        /// <summary>
        /// 设定串口参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="netConfig">预览参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, DHDEV_COMM_CFG commConfig)
        {
            return DHSetDevConfig(lLoginID, commConfig, 3000);
        }

        /// <summary>
        /// 设定报警参数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="alarmConfig">报警参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID,DHDEV_ALARM_SCHEDULE alarmConfig, int waittime)
        {
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, 0, CONFIG_COMMAND.DH_DEV_ALARMCFG, (object)alarmConfig, typeof(DHDEV_ALARM_SCHEDULE), waittime);
            return returnValue;
        }

        /// <summary>
        /// 设定报警参数[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="alarmConfig">报警参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID,DHDEV_ALARM_SCHEDULE alarmConfig)
        {
            return DHSetDevConfig(lLoginID,alarmConfig, 3000);
        }

        /// <summary>
        /// 设定DVR时间
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dvrTime">时间参数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID,NET_TIME dvrTime, int waittime)
        {
            
            bool returnValue = false;
            returnValue = PSetDevConfig(lLoginID, -1, CONFIG_COMMAND.DH_DEV_TIMECFG, (object)dvrTime, typeof(NET_TIME), waittime);
            return returnValue;
        }

        /// <summary>
        /// 设定DVR时间[默认等待时间3000]
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dvrTime">时间参数</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetDevConfig(int lLoginID, NET_TIME dvrTime)
        {
            return DHSetDevConfig(lLoginID, dvrTime, 3000);
        }

        #endregion

        #endregion

        #region << 用户管理 >>

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="iLogin">DHLogin的返回值</param>
        /// <param name="userInfo">USER_MANAGE_INFO结构</param>
        /// <param name="waitTime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHQueryUserInfo(int iLogin,ref USER_MANAGE_INFO userInfo,int waitTime)
        {
            bool retValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_MANAGE_INFO)));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    retValue = CLIENT_QueryUserInfo(iLogin, pBoxInfo, waitTime);
                    if (retValue == true)
                    {
                        userInfo = (USER_MANAGE_INFO)Marshal.PtrToStructure((IntPtr)((UInt32)pBoxInfo), typeof(USER_MANAGE_INFO));
                    }
                }
                DHThrowLastError(retValue);                
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                retValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
            } 
            return retValue;
                           
        }

        /// <summary>
        /// 增加用户组/删除用户组
        /// </summary>
        /// <param name="iLogin">DHLogin的返回值</param>
        /// <param name="grpInfo">用户组信息</param>
        /// <param name="waitTime">等待时间</param>
        /// <returns></returns>
        public static bool DHOperateUserInfo(int iLogin, USER_OPERATE operate,USER_GROUP_INFO grpInfo,int waitTime)
        {
            bool retValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_GROUP_INFO)));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    Marshal.StructureToPtr(grpInfo, pBoxInfo, true);
                    retValue = CLIENT_OperateUserInfo(iLogin, (int)operate, pBoxInfo, IntPtr.Zero, waitTime);
                }
                DHThrowLastError(retValue);
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                retValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;                
            }
            return retValue;
        }

        /// <summary>
        /// 修改用户组
        /// </summary>
        /// <param name="iLogin"></param>
        /// <param name="grpInfo"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static bool DHOperateUserInfo(int iLogin, USER_OPERATE operate, USER_GROUP_INFO grpNewInfo, USER_GROUP_INFO grpOldInfo, int waitTime)
        {
            bool retValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            IntPtr pBoxInfo1 = IntPtr.Zero;
            try
            {
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_GROUP_INFO)));//分配固定的指定大小的内存空间                
                pBoxInfo1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_GROUP_INFO)));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero && pBoxInfo1!=IntPtr.Zero)
                {
                    Marshal.StructureToPtr(grpNewInfo, pBoxInfo, true);
                    Marshal.StructureToPtr(grpOldInfo, pBoxInfo1, true);
                    retValue = CLIENT_OperateUserInfo(iLogin, (int)operate, pBoxInfo, pBoxInfo1, waitTime);
                }
                DHThrowLastError(retValue);                
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                retValue= false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
                Marshal.FreeHGlobal(pBoxInfo1);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;                
            }
            return retValue;
            
        }

        /// <summary>
        /// 增加用户/删除用户
        /// </summary>
        /// <param name="iLogin">DHLogin返回值</param>
        /// <param name="usrInfo">新增用户信息</param>
        /// <param name="waitTime">等待时间</param>
        /// <returns></returns>
        public static bool DHOperateUserInfo(int iLogin, USER_OPERATE operate, USER_INFO usrInfo, int waitTime)
        {
            bool retValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_INFO)));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero)
                {
                    Marshal.StructureToPtr(usrInfo, pBoxInfo, true);
                    retValue = CLIENT_OperateUserInfo(iLogin, (int)operate, pBoxInfo, IntPtr.Zero, waitTime);
                }
                DHThrowLastError(retValue);
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                retValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
            }
            return retValue;
        }

        /// <summary>
        /// 修改用户基本信息/修改用户密码
        /// </summary>
        /// <param name="iLogin">DHLogin返回值</param>
        /// <param name="userNewInfo">用户变更后信息</param>
        /// <param name="userOldInfo">用户变更前信息</param>
        /// <param name="waitTime">等待时间</param>
        /// <returns></returns>
        public static bool DHOperateUserInfo(int iLogin, USER_OPERATE operate, USER_INFO userNewInfo, USER_INFO userOldInfo, int waitTime)
        {
            bool retValue = false;
            IntPtr pBoxInfo = IntPtr.Zero;
            IntPtr pBoxInfo1 = IntPtr.Zero;
            try
            {                
                pBoxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_INFO)));//分配固定的指定大小的内存空间               
                pBoxInfo1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USER_INFO)));//分配固定的指定大小的内存空间
                if (pBoxInfo != IntPtr.Zero & pBoxInfo1!=IntPtr.Zero)
                {
                    Marshal.StructureToPtr(userNewInfo, pBoxInfo, true);
                    Marshal.StructureToPtr(userOldInfo, pBoxInfo1, true);
                    retValue = CLIENT_OperateUserInfo(iLogin, (int)operate, pBoxInfo, pBoxInfo1, waitTime);
                }
                DHThrowLastError(retValue);                
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                retValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;
                Marshal.FreeHGlobal(pBoxInfo1);//释放固定内存分配
                pBoxInfo1 = IntPtr.Zero;                
            }
            return retValue;
        }
        
        #endregion

        #region << 状态侦听 >>

        /// <summary>
        /// 设置设备消息回调函数, 用来得到设备当前状态信息
        /// </summary>
        /// <param name="cbMessage">消息回调参数</param>
        /// <param name="dwUser">用户数据,没有数据请使用IntPtr.Zero</param>
        public static void DHSetDVRMessCallBack(fMessCallBack cbMessage, IntPtr dwUser)
        {
            CLIENT_SetDVRMessCallBack(cbMessage, dwUser);
            //DHThrowLastError();
        }

        /// <summary>
        /// 开始对某个设备订阅消息,用来设置是否需要对设备消息回调，得到的消息从<seealso cref="DHSetDVRMessCallBack"/>的设置值回调出来。
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="DHLogin"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStartListen(int lLoginID)
        {
            bool returnValue = false;
            returnValue = CLIENT_StartListen(lLoginID);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 停止对某个设备侦听消息
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStopListen(int lLoginID)
        {
            bool returnValue = false;
            returnValue = CLIENT_StopListen(lLoginID);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 开始对某个设备订阅消息,用来设置是否需要对设备消息回调，得到的消息从<seealso cref="DHSetDVRMessCallBack"/>的设置值回调出来。
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="DHLogin"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStartListenEx(int lLoginID)
        {
            bool returnValue = false;
            returnValue = CLIENT_StartListenEx(lLoginID);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 启动监听服务, 目前只实现了报警监听功能
        /// </summary>
        /// <param name="wPort">启动监听的端口</param>
        /// <param name="pIp">绑定的IP，为NULL时绑定本机所有合法IP</param>
        /// <param name="pfscb">服务器的消息回调接口</param>
        /// <param name="dwTimeOut">服务器维护连接的超时时间</param>
        /// <param name="dwUserData">用户回调的自定义数据</param>
        /// <returns>成功返回服务器句柄，失败返回0</returns>
        public static int DHStartService(ushort wPort, string pIp, fServiceCallBack pfscb, IntPtr dwTimeOut, IntPtr dwUserData)
        {
            int returnValue;
            returnValue = CLIENT_StartService(wPort, pIp, pfscb, dwTimeOut, dwUserData);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 停止端口监听服务
        /// </summary>
        /// <param name="lHandle">
        /// 要关闭的服务器的句柄:<seealso cref="CLIENT_StartService"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStopService(int lHandle)
        {
            bool returnValue = false;
            returnValue = CLIENT_StopService(lHandle);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        #endregion

        #region << 设备注册 >>

        /// <summary>
        /// 注册用户到设备，当设备端把用户设置为复用（设备默认的用户如admin,不能设置为复用），则使用该帐号可以多次向设备注册
        /// </summary>
        /// <param name="pchDVRIP">设备IP</param>
        /// <param name="wDVRPort">设备端口</param>
        /// <param name="pchUserName">用户名</param>
        /// <param name="pchPassword">用户密码</param>
        /// <param name="lpDeviceInfo">设备信息,属于输出参数</param>
        /// <param name="error">返回登录错误码</param>
        /// <returns>失败返回0，成功返回设备ID</returns>
        public static int DHLogin(string pchDVRIP, ushort wDVRPort, string pchUserName, string pchPassword, out NET_DEVICEINFO lpDeviceInfo, out int error)
        {
            int result = CLIENT_Login(pchDVRIP, wDVRPort, pchUserName, pchPassword, out lpDeviceInfo, out error);
            DHThrowLastError(result);
            return result;
        }

        /// <summary>
        /// 注册用户到设备的扩展接口，支持一个用户指定设备支持的能力
        /// </summary>
        /// <param name="pchDVRIP">设备IP</param>
        /// <param name="wDVRPort">设备端口</param>
        /// <param name="pchUserName">用户名</param>
        /// <param name="pchPassword">用户密码</param>
        /// <param name="nSpecCap">设备支持的能力，值为2表示主动侦听模式下的用户登录。[车载dvr登录]</param>
        /// <param name="pCapParam">对nSpecCap 的补充参数, nSpecCap = 2时，pCapParam填充设备序列号字串。[车载dvr登录]</param>
        /// <param name="lpDeviceInfo">设备信息,属于输出参数</param>
        /// <param name="error">返回登录错误码</param>
        /// <returns>失败返回0，成功返回设备ID</returns>
        public static int DHLogin(string pchDVRIP, ushort wDVRPort, string pchUserName, string pchPassword, int nSpecCap, string pCapParam, out NET_DEVICEINFO lpDeviceInfo, out int error)
        {
            int result = CLIENT_LoginEx(pchDVRIP, wDVRPort, pchUserName, pchPassword, nSpecCap, pCapParam, out lpDeviceInfo, out error);
            DHThrowLastError(result);
            return result;
        }

        /// <summary>
        /// 注销设备用户
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHLogout(int lLoginID)
        {
            bool returnValue = false;
            returnValue = CLIENT_Logout(lLoginID);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        #endregion

        #region << 设备控制 >>
        //暂时无
        #endregion

        #region << 实时监视 >>

        /// <summary>
        /// 启动实时监视
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="hWnd">显示容器窗口句柄</param>
        /// <returns>失败返回0，成功返回实时监视ID(实时监视句柄)</returns>
        public static int DHRealPlay(int lLoginID, int nChannelID, IntPtr hWnd)
        {
            int returnValue = CLIENT_RealPlay(lLoginID, nChannelID, hWnd);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 启动实时监视(增强版)
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="realPlayType">实时播放类型</param>
        /// <param name="hWnd">显示容器窗口句柄</param>
        /// <returns>失败返回0，成功返回实时监视ID(实时监视句柄)</returns>
        public static int DHRealPlayEx(int lLoginID, int nChannelID, REALPLAY_TYPE realPlayType,IntPtr hWnd)
        {
            int returnValue = CLIENT_RealPlayEx(lLoginID, nChannelID, hWnd,realPlayType);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 停止实时监视
        /// </summary>
        /// <param name="lRealHandle">实时监视句柄:<seealso cref="CLIENT_RealPlay"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStopRealPlay(int lRealHandle)
        {
            bool returnValue = CLIENT_StopRealPlay(lRealHandle);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 停止实时监视(增强版)
        /// </summary>
        /// <param name="lRealHandle">实时监视句柄:<seealso cref="CLIENT_RealPlay"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStopRealPlayEx(int lRealHandle)
        {
            bool returnValue = CLIENT_StopRealPlayEx(lRealHandle);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 开始保存实时监视数据,对前端设备监视的图像进行数据保存,形成录像文件,此数据是设备端传送过来的原始视频数据
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="pchFileName">实时监视保存文件名</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStartSaveRealData(int lRealHandle, string pchFileName)
        {
            bool returnValue = CLIENT_SaveRealData(lRealHandle, pchFileName);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 停止保存实时监视数据,关闭保存的文件
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStopSaveRealData(int lRealHandle)
        {
            bool returnValue = CLIENT_StopSaveRealData(lRealHandle);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 设置实时监视数据回调，给用户提供设备流出的数据，当   cbRealData为NULL时结束回调数据
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="cbRealData">回调函数,用于传出设备流出的实时数据</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetRealDataCallBack(int lRealHandle, fRealDataCallBack cbRealData, IntPtr dwUser)
        {
            bool returnValue = CLIENT_SetRealDataCallBack(lRealHandle, cbRealData, dwUser);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 设置实时监视数据回调扩展接口 ,是对上一接口的补充, 增加一个回调数据类型标志dwFlag 参数, 可以选择性的回调出需要的数据, 对于没设置回调的数据类型就不回调出来了, 当设置为0x1f时与上一接口效果一样, 不过对回调函数也做了扩展
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="cbRealData">回调函数,用于传出多种类型的实时数据</param>
        /// <param name="dwFlag">
        /// 是按位来的, 可以组合, 为0x1f时五种数据类型都回调
        /// 0x00000001  等同原来的原始数据
        /// 0x00000002  是MPEG4/H264标准数据
        /// 0x00000004  YUV数据
        /// 0x00000008  PCM数据
        /// 0x00000010  原始音频数据</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetRealDataCallBack(int lRealHandle, fRealDataCallBackEx cbRealData,UInt32 dwFlag, IntPtr dwUser)
        {
            bool returnValue = CLIENT_SetRealDataCallBackEx(lRealHandle, cbRealData, dwUser,dwFlag);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        #endregion

        #region << 云台控制 >>

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令<seealso cref="PTZControlType"/>[PTZ_****的命令]</param>
        /// <param name="dwStep">步进/速度</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHPTZControl(int lLoginID, int nChannelID, PTZ_CONTROL dwPTZCommand, ushort dwStep, bool dwStop)
        {
            bool returnValue = false;
            switch (dwPTZCommand)
            {
                case PTZ_CONTROL.PTZ_APERTURE_ADD_CONTROL:
                case PTZ_CONTROL.PTZ_APERTURE_DEC_CONTROL:
                case PTZ_CONTROL.PTZ_DOWN_CONTROL:
                case PTZ_CONTROL.PTZ_FOCUS_ADD_CONTROL:
                case PTZ_CONTROL.PTZ_FOCUS_DEC_CONTROL:
                //case PTZ_CONTROL.PTZ_LAMP_CONTROL:
                case PTZ_CONTROL.PTZ_LEFT_CONTROL:
                //case PTZ_CONTROL.PTZ_POINT_DEL_CONTROL:
                case PTZ_CONTROL.PTZ_POINT_LOOP_CONTROL:
                //case PTZ_CONTROL.PTZ_POINT_MOVE_CONTROL:
                //case PTZ_CONTROL.PTZ_POINT_SET_CONTROL:
                case PTZ_CONTROL.PTZ_RIGHT_CONTROL:
                case PTZ_CONTROL.PTZ_UP_CONTROL:
                case PTZ_CONTROL.PTZ_ZOOM_ADD_CONTROL:
                case PTZ_CONTROL.PTZ_ZOOM_DEC_CONTROL:
                    returnValue = CLIENT_PTZControl(lLoginID, nChannelID, (ushort)dwPTZCommand, dwStep, dwStop);
                    break;
                case PTZ_CONTROL.PTZ_LAMP_CONTROL:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, dwStep, 0, 0, dwStop);
                    break;
                case PTZ_CONTROL.PTZ_POINT_DEL_CONTROL:                
                case PTZ_CONTROL.PTZ_POINT_MOVE_CONTROL:
                case PTZ_CONTROL.PTZ_POINT_SET_CONTROL:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, 0, dwStep, 0, dwStop);
                    break;

                //case PTZControlType.PTZ_POINT_LOOP_CONTROL:
                //    returnValue = CLIENT_DHPTZControl(lLoginID, nChannelID, (ushort)dwPTZCommand, "", "", dwStep, dwStop);
                //    break;
                case PTZ_CONTROL.EXTPTZ_SETMODESTART:
                case PTZ_CONTROL.EXTPTZ_SETMODESTOP:
                case PTZ_CONTROL.EXTPTZ_RUNMODE:
                case PTZ_CONTROL.EXTPTZ_STOPMODE:
                case PTZ_CONTROL.EXTPTZ_DELETEMODE:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, dwStep, 0, 0, dwStop);
                    break;
                case PTZ_CONTROL.EXTPTZ_STARTPANCRUISE:
                case PTZ_CONTROL.EXTPTZ_STOPPANCRUISE:
                case PTZ_CONTROL.EXTPTZ_SETLEFTBORDER:
                case PTZ_CONTROL.EXTPTZ_RIGHTBORDER:
                case PTZ_CONTROL.EXTPTZ_STARTLINESCAN:
                case PTZ_CONTROL.EXTPTZ_CLOSELINESCAN:
                case PTZ_CONTROL.EXTPTZ_REVERSECOMM:
                case PTZ_CONTROL.EXTPTZ_OPENMENU:
                case PTZ_CONTROL.EXTPTZ_CLOSEMENU:
                case PTZ_CONTROL.EXTPTZ_MENUOK:
                case PTZ_CONTROL.EXTPTZ_MENUCANCEL:
                case PTZ_CONTROL.EXTPTZ_MENUUP:
                case PTZ_CONTROL.EXTPTZ_MENUDOWN:
                case PTZ_CONTROL.EXTPTZ_MENULEFT:
                case PTZ_CONTROL.EXTPTZ_MENURIGHT:
                case PTZ_CONTROL.EXTPTZ_TOTAL:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, 0, 0, 0, dwStop);
                    break;
            }
            
            return returnValue;
        }

        /// <summary>
        /// 扩展云台控制, 对云台控制函数功能的增强控制
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令<seealso cref="PTZControlType"/>接受参数格式参见文档说明</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHPTZControl(int lLoginID, int nChannelID, PTZ_CONTROL dwPTZCommand, ushort param1, ushort param2, ushort param3, bool dwStop)
        {
            bool returnValue = false;
            switch (dwPTZCommand)
            {
                case PTZ_CONTROL.PTZ_APERTURE_ADD_CONTROL:
                case PTZ_CONTROL.PTZ_APERTURE_DEC_CONTROL:
                case PTZ_CONTROL.PTZ_DOWN_CONTROL:
                case PTZ_CONTROL.PTZ_FOCUS_ADD_CONTROL:
                case PTZ_CONTROL.PTZ_FOCUS_DEC_CONTROL:
                case PTZ_CONTROL.PTZ_LAMP_CONTROL:
                case PTZ_CONTROL.PTZ_LEFT_CONTROL:
                case PTZ_CONTROL.PTZ_POINT_DEL_CONTROL:
                case PTZ_CONTROL.PTZ_POINT_MOVE_CONTROL:
                case PTZ_CONTROL.PTZ_POINT_SET_CONTROL:
                case PTZ_CONTROL.PTZ_RIGHT_CONTROL:
                case PTZ_CONTROL.PTZ_UP_CONTROL:
                case PTZ_CONTROL.PTZ_ZOOM_ADD_CONTROL:
                case PTZ_CONTROL.PTZ_ZOOM_DEC_CONTROL:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, 0, param2, 0, dwStop);
                    break;
                case PTZ_CONTROL.PTZ_POINT_LOOP_CONTROL:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, 0, 0, param3, dwStop);
                    break;
                case PTZ_CONTROL.EXTPTZ_LEFTTOP:
                case PTZ_CONTROL.EXTPTZ_RIGHTTOP:
                case PTZ_CONTROL.EXTPTZ_LEFTDOWN:
                case PTZ_CONTROL.EXTPTZ_RIGHTDOWN:
                case PTZ_CONTROL.EXTPTZ_ADDTOLOOP:
                case PTZ_CONTROL.EXTPTZ_DELFROMLOOP:
                case PTZ_CONTROL.EXTPTZ_AUXIOPEN:
                case PTZ_CONTROL.EXTPTZ_AUXICLOSE:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, param1, param2, 0, dwStop);
                    break;
                case PTZ_CONTROL.EXTPTZ_SETMODESTART:
                case PTZ_CONTROL.EXTPTZ_SETMODESTOP:
                case PTZ_CONTROL.EXTPTZ_RUNMODE:
                case PTZ_CONTROL.EXTPTZ_STOPMODE:
                case PTZ_CONTROL.EXTPTZ_DELETEMODE:
                    returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)dwPTZCommand, param1, 0, 0, dwStop);
                    break;
                //case PTZ_CONTROL.EXTPTZ_FASTGOTO:
                //    returnValue = CLIENT_DHPTZControl(lLoginID, nChannelID, (ushort)dwPTZCommand, param1, param2, param3, dwStop);
                //    break;
            }
            return returnValue;
        }

        /// <summary>
        /// 扩展云台控制, 对云台控制函数功能的增强控制
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令<seealso cref="PTZControlType"/>接受只有两个参数的命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHPTZControl(int lLoginID, int nChannelID, PTZ_CONTROL dwPTZCommand, ushort param1, ushort param2, bool dwStop)
        {
            bool returnValue = false;
            switch (dwPTZCommand)
            {
                case PTZ_CONTROL.EXTPTZ_LEFTTOP:
                case PTZ_CONTROL.EXTPTZ_RIGHTTOP:
                case PTZ_CONTROL.EXTPTZ_LEFTDOWN:
                case PTZ_CONTROL.EXTPTZ_RIGHTDOWN:
                case PTZ_CONTROL.EXTPTZ_ADDTOLOOP:
                case PTZ_CONTROL.EXTPTZ_DELFROMLOOP:
                case PTZ_CONTROL.EXTPTZ_AUXIOPEN:
                case PTZ_CONTROL.EXTPTZ_AUXICLOSE:
                    returnValue = CLIENT_DHPTZControl(lLoginID, nChannelID, (ushort)dwPTZCommand, param1, param2, 0, dwStop, IntPtr.Zero);
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// 扩展云台控制, 支持三维快速定位
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令<seealso cref="PTZControlType"/>接受只有两个参数的命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHPTZControl(int lLoginID, int nChannelID, bool blnFastGoto, int  param1, int param2,int param3, bool dwStop)
        {
            bool returnValue = false;
            if (blnFastGoto)
            {
                returnValue = CLIENT_DHPTZControlEx(lLoginID, nChannelID, (ushort)PTZ_CONTROL.EXTPTZ_FASTGOTO, param1, param2, param3, dwStop);
            }
            return returnValue;
        }

        #endregion

        #region << 回放下载 >>

        /// <summary>
        /// 查询录像文件
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelId">通道ID</param>
        /// <param name="nRecordFileType">录像文件类型 </param>
        /// <param name="tmStart">录像开始时间</param>
        /// <param name="tmEnd">录像结束时间</param>
        /// <param name="pchCardid">卡号,只针对卡号查询有效，其他情况下可以填NULL</param>
        /// <param name="nriFileinfo">返回的录像文件信息，是一个NET_RECORDFILE_INFO结构数组[录像文件信息为指定条]</param>
        /// <param name="maxlen">nriFileinfo缓冲的最大长度;[单位字节，大小为结构数组维数*sizeof(NET_RECORDFILE_INFO),数组维为大小等于1，建议小于２００]</param>
        /// <param name="filecount">返回的文件个数,属于输出参数最大只能查到缓冲满为止的录像记录</param>
        /// <param name="waittime">等待时间</param>
        /// <param name="bTime">是否按时间查(目前无效)</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHQueryRecordFile(int lLoginID, int nChannelId, RECORD_FILE_TYPE nRecordFileType, DateTime tmStart, DateTime tmEnd, string pchCardid, ref NET_RECORDFILE_INFO[] nriFileinfo, int maxlen, out  int filecount, int waittime, bool bTime)
        {
            bool returnValue = false;
            filecount = 0;
            IntPtr pBoxInfo = IntPtr.Zero;
            try
            {                
                NET_TIME timeStart = ToNetTime(tmStart);
                NET_TIME timeEnd = ToNetTime(tmEnd);
                pBoxInfo = Marshal.AllocHGlobal(maxlen);//分配固定的指定大小的内存空间
                int fileCountMin = 0;
                if (pBoxInfo != IntPtr.Zero)
                {
                    returnValue = CLIENT_QueryRecordFile(lLoginID, nChannelId, (int)nRecordFileType, ref timeStart, ref timeEnd, pchCardid, pBoxInfo, maxlen, out filecount, waittime, bTime);
                    fileCountMin = (filecount <= nriFileinfo.Length ? filecount : nriFileinfo.Length);
                    for (int dwLoop = 0; dwLoop < fileCountMin; dwLoop++)
                    {
                        //将指定内存空间的数据按指定格式复制到目的数组中
                        nriFileinfo[dwLoop] = (NET_RECORDFILE_INFO)Marshal.PtrToStructure((IntPtr)((UInt32)pBoxInfo + Marshal.SizeOf(typeof(NET_RECORDFILE_INFO)) * dwLoop), typeof(NET_RECORDFILE_INFO));
                    }
                }
                DHThrowLastError(returnValue);
               
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                returnValue= false;
            }
            finally
            {
                Marshal.FreeHGlobal(pBoxInfo);//释放固定内存分配
                pBoxInfo = IntPtr.Zero;                
            }
            return returnValue;           
        }

        /// <summary>
        /// 查询录像文件
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelId">通道ID</param>
        /// <param name="nRecordFileType">录像文件类型 </param>
        /// <param name="tmStart">录像开始时间</param>
        /// <param name="tmEnd">录像结束时间</param>
        /// <param name="pchCardid">卡号,只针对卡号查询有效，其他情况下可以填NULL</param>
        /// <param name="nriFileinfo">返回的录像文件信息，是一个NET_RECORDFILE_INFO结构[录像文件信息只有一条]</param>
        /// <param name="maxlen">nriFileinfo缓冲的最大长度;[单位字节大小为sizeof(NET_RECORDFILE_INFO)]</param>
        /// <param name="filecount">返回的文件个数,属于输出参数最大只能查到缓冲满为止的录像记录</param>
        /// <param name="waittime">等待时间</param>
        /// <param name="bTime">是否按时间查(目前无效)</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHQueryRecordFile(int lLoginID, int nChannelId, RECORD_FILE_TYPE nRecordFileType, DateTime tmStart, DateTime tmEnd, string pchCardid, ref NET_RECORDFILE_INFO nriFileinfo, int maxlen, out  int filecount, int waittime, bool bTime)
        {
            bool returnValue = false;
            filecount = 0;
            NET_RECORDFILE_INFO[] ntFileInfo = new NET_RECORDFILE_INFO[1];
            returnValue = DHQueryRecordFile(lLoginID, nChannelId, nRecordFileType, tmStart, tmEnd, pchCardid, ref ntFileInfo, maxlen, out filecount, waittime, bTime);
            nriFileinfo = ntFileInfo[0];
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 网络回放[注意:用户登录一台设备后，每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录]
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="lpRecordFile">录像文件信息, 当按时间播放是只需填充起始时间和结束时间, 其他项填0</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        public static int DHPlayBackByRecordFile(int lLoginID, ref NET_RECORDFILE_INFO lpRecordFile, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData)
        {
            int returnValue;
            returnValue = CLIENT_PlayBackByRecordFile(lLoginID, ref lpRecordFile, hWnd, cbDownLoadPos, dwUserData);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 带数据回调的按录像文件回放扩展接口,每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录。窗口参数（hWnd）有效时回调数据的返回值将被忽略，窗口参数 (hWnd)为0时，需要注意回调函数的返回值，具体见回调函数说明。
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="lpRecordFile">录像文件信息</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwPosUser">进度回调函数用户数据</param>
        /// <param name="fDownLoadDataCallBack">数据回调函数</param>
        /// <param name="dwDataUser">数据回调函数用户数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        public static int DHPlayBackByRecordFile(int lLoginID, ref NET_RECORDFILE_INFO lpRecordFile, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwPosUser, fDataCallBack fDownLoadDataCallBack, IntPtr dwDataUser)
        {
            int returnValue;
            returnValue = CLIENT_PlayBackByRecordFileEx(lLoginID, ref lpRecordFile, hWnd, cbDownLoadPos, dwPosUser,fDownLoadDataCallBack,dwDataUser);
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 网络按时间回放
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="lpStartTime">录像开始时间</param>
        /// <param name="lpStopTime">录像结束时间</param>
        /// <param name="hWnd">录像回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        public static int DHPlayBackByTime(int lLoginID, int nChannelID, DateTime tmStart, DateTime tmEnd, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData)
        {
            
            try
            {
                int returnValue = 0;
                NET_TIME timeStart = ToNetTime(tmStart);
                NET_TIME timeEnd = ToNetTime(tmEnd);
                returnValue = CLIENT_PlayBackByTime(lLoginID, nChannelID, ref timeStart, ref  timeEnd, hWnd, cbDownLoadPos, dwUserData);
                DHThrowLastError(returnValue);
                return returnValue;
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                return 0;
            }
            
        }

        /// <summary>
        /// 网络按时间回放
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="tmStart">录像开始时间</param>
        /// <param name="tmEnd">录像结束时间</param>
        /// <param name="hWnd">录像回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwPosUser">用户自定义数据[进度]</param>
        /// <param name="cbData">数据回调函数</param>
        /// <param name="dwDataUser">用户自定义数据[进度]</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        public static int DHPlayBackByTime(int lLoginID, int nChannelID, DateTime tmStart, DateTime tmEnd, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwPosUser,fDataCallBack cbData,IntPtr dwDataUser)
        {
            try
            {
                int returnValue = 0;
                NET_TIME timeStart = ToNetTime(tmStart);
                NET_TIME timeEnd = ToNetTime(tmEnd);
                returnValue = CLIENT_PlayBackByTimeEx(lLoginID, nChannelID, ref timeStart, ref  timeEnd, hWnd, cbDownLoadPos, dwPosUser,cbData,dwDataUser);
                DHThrowLastError(returnValue);
                return returnValue;
            }
            catch (Exception e)
            {
                DHThrowLastError(e);
                return 0;
            }

        }

        /// <summary>
        /// 回放播放控制:播放，暂停，停止，单步播放，单步停止，慢放，快放
        /// </summary>
        /// <param name="lPlayHandle">播放句柄</param>
        /// <param name="pPlayCommand">控制命令:参见<seealso cref="PlayControlType"/></param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHPlayBackControl(int lPlayHandle, PLAY_CONTROL pPlayCommand)
        {
            bool returnValue = false;
            switch (pPlayCommand)
            {
                case PLAY_CONTROL.Play:
                    returnValue = CLIENT_PausePlayBack(lPlayHandle, 0);
                    break;
                case PLAY_CONTROL.Pause:
                    returnValue = CLIENT_PausePlayBack(lPlayHandle, 1);
                    break;
                case PLAY_CONTROL.Stop:
                    returnValue = CLIENT_StopPlayBack(lPlayHandle);
                    break;
                case PLAY_CONTROL.StepPlay:
                    returnValue = CLIENT_StepPlayBack(lPlayHandle, false);
                    break;
                case PLAY_CONTROL.StepStop:
                    returnValue = CLIENT_StepPlayBack(lPlayHandle, true);
                    break;
                case PLAY_CONTROL.Fast:
                    returnValue = CLIENT_FastPlayBack(lPlayHandle);
                    break;
                case PLAY_CONTROL.Slow:
                    returnValue = CLIENT_SlowPlayBack(lPlayHandle);
                    break;
            }
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 回放播放控制:改变位置播放[即拖动播放，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义]
        /// </summary>
        /// <param name="lPlayHandle">播放句柄</param>
        /// <param name="pPlayCommand">播放命令:SeekByBit,SeekByTime</param>
        /// <param name="offset">SeekByTime:相对文件开始处偏移时间,单位为秒;SeekByBit:相对文件开始处偏移字节;</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHPlayBackControl(int lPlayHandle, PLAY_CONTROL pPlayCommand, uint offset)
        {
            bool returnValue = false;
            uint FalseValue = 0xFFFFFFFF;
            switch (pPlayCommand)
            {
                case PLAY_CONTROL.SeekByBit:
                    returnValue = CLIENT_SeekPlayBack(lPlayHandle, FalseValue, offset);
                    break;
                case PLAY_CONTROL.SeekByTime:
                    returnValue = CLIENT_SeekPlayBack(lPlayHandle, offset, FalseValue);
                    break;
            }
            DHThrowLastError(returnValue);
            return returnValue;
        }

        /// <summary>
        /// 按文件下载录像文件, 通过查询到的文件信息下载
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="lpRecordFile">录像文件信息</param>
        /// <param name="sSavedFileName">要保存的录像文件名，全路径</param>
        /// <param name="cbDownLoadPos">下载进度回调函数</param>
        /// <param name="dwUserData">下载进度回调用户自定义数据</param>
        /// <returns>成功返回下载ID，失败返回0</returns>
        public static int DHDownloadByRecordFile(int lLoginID, NET_RECORDFILE_INFO lpRecordFile,string sSavedFileName,  fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData)
        {
            int intReturnValue = 0;
            intReturnValue=CLIENT_DownloadByRecordFile(lLoginID, ref  lpRecordFile,sSavedFileName, cbDownLoadPos, dwUserData);            
            DHThrowLastError();            
            return intReturnValue;
        }

        /// <summary>
        /// 按时间下载，直接输入指定通道起始时间和结束时间下载放录像
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelId">图像通道号，从0开始</param>
        /// <param name="nRecordFileType">保留</param>
        /// <param name="tmStart">开始时间</param>
        /// <param name="tmEnd">结束时间</param>
        /// <param name="sSavedFileName">要保存的录像文件名，全路径</param>
        /// <param name="cbTimeDownLoadPos">下载进度回调函数</param>
        /// <param name="dwUserData">下载进度回调用户自定义数据</param>
        /// <returns>成功返回下载ID，失败返回0</returns>
        public static int DHDownloadByTime(int lLoginID, int nChannelId, int nRecordFileType, DateTime tmStart, DateTime tmEnd, string sSavedFileName, fTimeDownLoadPosCallBack cbTimeDownLoadPos, IntPtr dwUserData)
        {
            int intReturnValue = 0;
            NET_TIME ntStart = ToNetTime(tmStart);
            NET_TIME ntEnd = ToNetTime(tmEnd);
            intReturnValue = CLIENT_DownloadByTime(lLoginID, nChannelId, nRecordFileType, ref ntStart, ref ntEnd, sSavedFileName, cbTimeDownLoadPos, dwUserData);
            DHThrowLastError();
            return intReturnValue;
        }

        /// <summary>
        /// 停止下载录像文件
        /// </summary>
        /// <param name="lFileHandle">CLIENT_DownloadByRecordFile的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHStopDownload(int lFileHandle)
        {
            bool blnReturnValue = false;
            blnReturnValue = CLIENT_StopDownload(lFileHandle);
            DHThrowLastError();
            return blnReturnValue;
        }

        /// <summary>
        /// 获得下载录像的当前位置,可以用于不需要实时显示下载进度的接口,与下载回调函数的功能类似
        /// </summary>
        /// <param name="lFileHandle">CLIENT_DownloadByRecordFile的返回值</param>
        /// <param name="nTotalSize">下载的总长度,单位KB</param>
        /// <param name="nDownLoadSize">已下载的长度,单位KB</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetDownloadPos(int lFileHandle, out int nTotalSize, out int nDownLoadSize)
        {
            bool blnReturnValue = false;
            blnReturnValue = CLIENT_GetDownloadPos(lFileHandle, out nTotalSize, out  nDownLoadSize);
            DHThrowLastError();
            return blnReturnValue;
        }

        #endregion

        #region << 视频抓图 >>

        /// <summary>
        /// 保存图片,对显示图像进行瞬间抓图，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义
        /// </summary>
        /// <param name="hPlayHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="pchPicFileName">位图文件名，当前只支持BMP位图</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHCapturePicture(int lPlayHandle, string phPicFileName)
        { 
            return CLIENT_CapturePicture(lPlayHandle,phPicFileName);
        }

        #endregion

        #region <<智能交通中增加的接口>>

        /// <summary>
        /// 新版本--内部使用的取设备信息命令函数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="dwCommand">配制命令</param>
        /// <param name="obj">object对象</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="nRuleSize">
        ///     CFG_CMD_ANALYSERULE此类型才会用到，其它默认设为0，表示CFG_ANALYSERULES_INFO
        ///     此结构体中指针数据要分配的大小内存,
        /// </param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHGetNewDevConfig(int lLoginID, int lChannel, string strCommand, ref object obj, Type typeName, int nRuleSize, int waittime)
        {
            bool returnValue = false;
            IntPtr pInBuf = IntPtr.Zero;
            IntPtr pOutBuf = IntPtr.Zero;
            IntPtr pRetLen = IntPtr.Zero;

            UInt32 nRetLen = 0;
            UInt32 nBufSize = 32 * 1024;
            UInt32 nError = 0;

            try
            {
                // 分配固定的指定大小的内存空间
                pInBuf = Marshal.AllocHGlobal((int)nBufSize);
                pOutBuf = Marshal.AllocHGlobal(Marshal.SizeOf(typeName));
                pRetLen = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)));
                
                // CFG_ANALYSERULES_INFO结构体中有指针数据，要为其单独分配空间
                if (strCommand.CompareTo(DHClient.CFG_CMD_ANALYSERULE) == 0)
                {
                    CFG_ANALYSERULES_INFO resultInfo = new CFG_ANALYSERULES_INFO();
                    resultInfo.pRuleBuf = Marshal.AllocHGlobal((int)nRuleSize); 
                    resultInfo.nRuleLen = nRuleSize;
                    Marshal.StructureToPtr(resultInfo, pOutBuf, true);
                }

                if (pInBuf != IntPtr.Zero && pOutBuf != IntPtr.Zero)
                {
                    returnValue = CLIENT_GetNewDevConfig(lLoginID, strCommand, lChannel, pInBuf,
                                                         nBufSize, ref nError, waittime);
                    VIDEOThrowLastError(returnValue);
                    if (returnValue == true)
                    {
                        returnValue = CLIENT_ParseData(strCommand, pInBuf, pOutBuf, (UInt32)Marshal.SizeOf(typeName), pRetLen);
                        nRetLen = (UInt32)Marshal.PtrToStructure((IntPtr)((UInt32)pRetLen), typeof(UInt32));
                        obj = Marshal.PtrToStructure((IntPtr)((UInt32)pOutBuf), typeName);
                    }
                }
                VIDEOThrowLastError(returnValue);
            }
            catch (Exception e)
            {
                VIDEOThrowLastError(e);
                returnValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pInBuf);//释放固定内存分配
                Marshal.FreeHGlobal(pOutBuf);//释放固定内存分配
                Marshal.FreeHGlobal(pRetLen);//释放固定内存分配
                
                pInBuf = IntPtr.Zero;
                pOutBuf = IntPtr.Zero;
                pRetLen = IntPtr.Zero;
            }
            return returnValue;
        }

        /// <summary>
        /// 新版本--内部使用的设置设备信息命令函数
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="lChannel">通道号</param>
        /// <param name="dwCommand">配制命令</param>
        /// <param name="obj">object对象</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHSetNewDevConfig(int lLoginID, int lChannel, string strCommand, object obj, Type typeName, int waittime)
        {
            bool returnValue = false;
            IntPtr pInBuf = IntPtr.Zero;
            IntPtr pOutBuf = IntPtr.Zero;

            UInt32 nBufSize = 32 * 1024;
            UInt32 nRestart = 0;
            UInt32 nError = 0;

            try
            {
                pInBuf = Marshal.AllocHGlobal(Marshal.SizeOf(typeName));
                pOutBuf = Marshal.AllocHGlobal((int)nBufSize);

                Marshal.StructureToPtr(obj, pInBuf, true);
                returnValue = CLIENT_PacketData(strCommand, pInBuf, (UInt32)Marshal.SizeOf(typeName), pOutBuf, nBufSize);
                VIDEOThrowLastError(returnValue);

                if (returnValue)
                {
                    returnValue = CLIENT_SetNewDevConfig(lLoginID, strCommand, lChannel, pOutBuf,
                                     nBufSize, ref nError, ref nRestart, waittime);
                    VIDEOThrowLastError(returnValue);
                }
            }
            catch (Exception e)
            {
                VIDEOThrowLastError(e);
                returnValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pInBuf);//释放固定内存分配
                Marshal.FreeHGlobal(pOutBuf);//释放固定内存分配
                pInBuf = IntPtr.Zero;
                pOutBuf = IntPtr.Zero;
            }
            return returnValue;
        }

        /// <summary>
        /// 新系统能力查询接口，查询系统能力信息(以Json格式，具体见配置SDK)
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="szCommand"></param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="szOutBuffer">输出缓冲</param>
        /// <param name="dwOutBufferSize">输出缓冲大小</param>
        /// <param name="error">错误码</param>
        /// <param name="waittime">等待超时时间,默认设定为1000</param>
        /// <returns></returns>
        public static bool DHQueryNewSystemInfo(Int32 lLoginID, Int32 lChannel, string strCommand, ref object obj, Type typeName, int waittime)
        {
            bool returnValue = false;
            IntPtr pInBuf = IntPtr.Zero;
            IntPtr pOutBuf = IntPtr.Zero;
            IntPtr pRetLen = IntPtr.Zero;

            UInt32 nRetLen = 0;
            UInt32 nBufSize = 1024 * 1024;
            UInt32 nError = 0;

            try
            {
                pInBuf = Marshal.AllocHGlobal((int)nBufSize);//分配固定的指定大小的内存空间
                pOutBuf = Marshal.AllocHGlobal(Marshal.SizeOf(typeName));

                pRetLen = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)));

                if (pInBuf != IntPtr.Zero && pOutBuf != IntPtr.Zero)
                {
                    returnValue = CLIENT_QueryNewSystemInfo(lLoginID, strCommand, lChannel, pInBuf,
                                                         nBufSize, ref nError, waittime);
                    VIDEOThrowLastError(returnValue);
                    if (returnValue == true)
                    {
                        returnValue = CLIENT_ParseData(strCommand, pInBuf, pOutBuf, (UInt32)Marshal.SizeOf(typeName), pRetLen);
                        nRetLen = (UInt32)Marshal.PtrToStructure((IntPtr)((UInt32)pRetLen), typeof(UInt32));
                        obj = Marshal.PtrToStructure((IntPtr)((UInt32)pOutBuf), typeName);
                    }
                }
                VIDEOThrowLastError(returnValue);
            }
            catch (Exception e)
            {
                VIDEOThrowLastError(e);
                returnValue = false;
            }
            finally
            {
                Marshal.FreeHGlobal(pInBuf);//释放固定内存分配
                Marshal.FreeHGlobal(pOutBuf);//释放固定内存分配
                Marshal.FreeHGlobal(pRetLen);//释放固定内存分配
                pInBuf = IntPtr.Zero;
                pOutBuf = IntPtr.Zero;
                pRetLen = IntPtr.Zero;
            }
            return returnValue;
        }

        /// <summary>
        /// 实时上传智能分析数据－图片
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwAlarmType">类型</param>
        /// <param name="cbAnalyzerData">分析数据回调</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns></returns>
        public static Int32 DHRealLoadPicture(Int32 lLoginID, Int32 nChannelID, UInt32 dwAlarmType, fAnalyzerDataCallBack cbAnalyzerData, UInt32 dwUser)
        {
            Int32 nReturn = 0;
            try
            {
                nReturn = CLIENT_RealLoadPicture(lLoginID, nChannelID, dwAlarmType, cbAnalyzerData, dwUser);
                VIDEOThrowLastError(nReturn);
            }
            catch (System.Exception ex)
            {
                VIDEOThrowLastError(ex);
                nReturn = 0;
            }

            return nReturn;
        }

        /// <summary>
        /// 实时上传智能分析数据－图片(扩展接口，bNeedPicFile表示是否订阅图片文件,Reserved类型为RESERVED_PARA) 
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwAlarmType">类型</param>
        /// <param name="cbAnalyzerData">分析数据回调</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns></returns>
        public static Int32 DHRealLoadPictureEx(Int32 lLoginID, Int32 nChannelID, UInt32 dwAlarmType, bool bNeedPicFile, fAnalyzerDataCallBack cbAnalyzerData, UInt32 dwUser, IntPtr Reserved)
        {
            Int32 nReturn = 0;
            try
            {
                nReturn = CLIENT_RealLoadPictureEx(lLoginID, nChannelID, dwAlarmType, bNeedPicFile, cbAnalyzerData, dwUser, Reserved);
                VIDEOThrowLastError(nReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                nReturn = 0;
            }
            return nReturn;
        }

        /// <summary>
        /// 停止上传智能分析数据－图片
        /// </summary>
        /// <param name="lAnalyzerHandle">CLIENT_RealLoadPictureEx返回的值</param>
        /// <returns></returns>
        public static bool DHStopLoadPic(Int32 lAnalyzerHandle)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_StopLoadPic(lAnalyzerHandle);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                bReturn = false;
            }
            return bReturn;
        }

        /// <summary>
        /// 下载指定的智能分析数据 - 图片
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="emType">EM_FILE_QUERY_TYPE枚举值</param>
        /// <param name="lpMediaFileInfo"></param>
        /// <param name="sSavedFileName"></param>
        /// <param name="cbDownLoadPos"></param>
        /// <param name="dwUserData"></param>
        /// <param name="reserved"></param>
        /// <returns></returns>
        public static Int32 DHDownloadMediaFile(Int32 lLoginID, EM_FILE_QUERY_TYPE emType, IntPtr lpMediaFileInfo, string sSavedFileName, fDownLoadPosCallBack cbDownLoadPos, UInt32 dwUserData, IntPtr reserved)
        {
            Int32 nReturn = 0;
            try
            {
                nReturn = CLIENT_DownloadMediaFile(lLoginID, emType, lpMediaFileInfo, sSavedFileName, cbDownLoadPos, dwUserData, reserved);
                VIDEOThrowLastError(nReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                nReturn = 0;
            }
            return nReturn;
        }

        /// <summary>
        /// 停止下载数据
        /// </summary>
        /// <param name="lFileHandle"></param>
        /// <returns></returns>
        public static bool DHStopDownloadMediaFile(Int32 lFileHandle)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_StopDownloadMediaFile(lFileHandle);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                bReturn = false;
            }
            return bReturn;
        }

        /// <summary>
        /// 设备控制
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="type">CtrlType的枚举</param>
        /// <param name="param"></param>
        /// <param name="waittime">超时等待，默认设定1000</param>
        /// <returns></returns>
        public static bool DHControlDevice(Int32 lLoginID, CtrlType type, IntPtr param, Int32 waittime)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_ControlDevice(lLoginID, type, param, waittime);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                bReturn = false;
            }

            return bReturn;
        }

        /// <summary>
        /// 解析查询到的配置信息
        /// </summary>
        /// <param name="szCommand">命令参数,查看开发文档</param>
        /// <param name="szInBuffer">输入缓冲，字符配置缓冲</param>
        /// <param name="lpOutBuffer">输出缓冲，结构体类型可以参见上表</param>
        /// <param name="dwOutBufferSize">输出缓冲的大小</param>
        /// <param name="pReserved">保留参数</param>
        /// <returns></returns>
        public static bool DHParseData(string szCommand, IntPtr szInBuffer, IntPtr lpOutBuffer, UInt32 dwOutBufferSize, IntPtr pReserved)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_ParseData(szCommand, szInBuffer, lpOutBuffer, dwOutBufferSize, pReserved);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                bReturn = false;
            }
            return bReturn;
        }


        /// <summary>
        /// 将需要设置的配置信息，打包成字符串格式
        /// </summary>
        /// <param name="szCommand">命令参数，参见CLIENT_ParseData中表格</param>
        /// <param name="lpInBuffer">输入缓冲，结构体类型参见CLIENT_ParseData中表格</param>
        /// <param name="dwInBufferSize">输入缓冲大小</param>
        /// <param name="szOutBuffer">输出缓冲</param>
        /// <param name="dwOutBufferSize">输出缓冲大小</param>
        /// <returns></returns>
        public static bool DHPacketData(string szCommand, IntPtr lpInBuffer, UInt32 dwInBufferSize, IntPtr szOutBuffer, UInt32 dwOutBufferSize)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_PacketData(szCommand, lpInBuffer, dwInBufferSize, szOutBuffer, dwOutBufferSize);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                bReturn = false;
            }
            return bReturn;
        }

        /// <summary>
        /// 按查询条件查询文件
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="emType">media文件查询条件 </param>
        /// <param name="pQueryCondition">查询条件 </param>
        /// <param name="reserved">保留字节</param>
        /// <param name="waittime">等待时间</param>
        /// <returns></returns>
        public static Int32 DHFindFileEx(Int32 lLoginID, EM_FILE_QUERY_TYPE emType, IntPtr pQueryCondition, IntPtr reserved, Int32 waittime)
        {
            Int32 nReturn = 0;
            try
            {
                nReturn = CLIENT_FindFileEx(lLoginID, emType, pQueryCondition, reserved, waittime);
                VIDEOThrowLastError(nReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                nReturn = 0;
            }
            return nReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lFindHandle">CLIENT_FindFileEx的返回值</param>
        /// <param name="nFilecount">需要查询的条数</param>
        /// <param name="pMediaFileInfo">文件记录缓冲，用于输出查询到的媒体文件记录</param>
        /// <param name="maxlen">缓冲区的最大长度</param>
        /// <param name="reserved">保留字节</param>
        /// <param name="waittime">等待时间</param>
        /// <returns></returns>
        public static Int32 DHFindNextFileEx(Int32 lFindHandle, Int32 nFilecount, IntPtr pMediaFileInfo, Int32 maxlen, IntPtr reserved, Int32 waittime)
        {
            Int32 nReturn = 0;
            try
            {
                nReturn = CLIENT_FindNextFileEx(lFindHandle, nFilecount, pMediaFileInfo, maxlen, reserved, waittime);
                VIDEOThrowLastError(nReturn);
            }
            catch (System.Exception ex)
            {
                VIDEOThrowLastError(ex);
                nReturn = 0;
            }
            return nReturn;
        }

        /// <summary>
        /// 关闭查询句柄
        /// </summary>
        /// <param name="lFindHandle">CLIENT_FindFileEx的返回值</param>
        /// <returns></returns>
        public static bool DHFindCloseEx(Int32 lFindHandle)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_FindCloseEx(lFindHandle);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
            	VIDEOThrowLastError(ex);
                bReturn = false;
            }
            return bReturn;
        }

        /// <summary>
        /// 查询系统信息,不同的信息有不同的数据结构
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="nSystemType">要查询的系统类型
        ///  0		一般信息
        ///  1		查询设备属性信息
        ///  2		查询硬盘信息
        ///  3		查询文件系统信息
        ///  4		查询视频属性信息
        ///  5		查询系统字符集属性信息
        ///  6		查询光存储设备信息
        ///  7		获取设备序列号
        /// </param>
        /// <param name="pSysInfoBuffer">接收的协议缓冲区</param>
        /// <param name="maxlen">接收的协议缓冲区长度,(单位字节)</param>
        /// <param name="nSysInfolen">接收的总字节数,(单位字节).</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        public static bool DHQuerySystemInfo(Int32 lLoginID, DH_SYS_ABILITY nSystemType, IntPtr pSysInfoBuffer, Int32 maxlen, ref  Int32 nSysInfolen, Int32 waittime)
        {
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_QuerySystemInfo(lLoginID, (int)nSystemType, pSysInfoBuffer, maxlen, ref nSysInfolen, waittime);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
                VIDEOThrowLastError(bReturn);
                bReturn = false;
            }
            return bReturn;
        }


        public static bool DHQueryDevState(int lLoginID, int nType, IntPtr pBuf, int nBufLen, ref int pRetLen, int waittime)
        {
             bool bReturn = false;
            try
            {
                bReturn = CLIENT_QueryDevState(lLoginID, nType, pBuf, nBufLen, ref pRetLen, waittime);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
                VIDEOThrowLastError(bReturn);
                bReturn = false;
            }
            return bReturn;
        }


        public static bool DHSnapPicture(Int32 lLoginID, SNAP_PARAMS par)
        {
            
            bool bReturn = false;
            try
            {
                bReturn = CLIENT_SnapPicture(lLoginID, par);
                VIDEOThrowLastError(bReturn);
            }
            catch (System.Exception ex)
            {
                VIDEOThrowLastError(bReturn);
                bReturn = false;
            }
            return bReturn;
        }

        public static void DHSetSnapRevCallBack(fSnapRev OnSnapRevMessage, Int32 dwUser)
        {
            CLIENT_SetSnapRevCallBack(OnSnapRevMessage, dwUser);
        }
        //CLIENT_API void CALL_METHOD CLIENT_SetSnapRevCallBack(fSnapRev OnSnapRevMessage, DWORD dwUser);


        public static Int32 DHStartSearchDevices(fSearchDevicesCB cbSearchDevices, IntPtr pUserData, IntPtr szLocalIp)
        {
            return CLIENT_StartSearchDevices(cbSearchDevices, pUserData, szLocalIp);
        }

        public static bool DHStopSearchDevices(Int32 lSearchHandle)
        {
            return CLIENT_StopSearchDevices(lSearchHandle);
        }


        #endregion


#region << 设备升级 >>

        /// <summary>
        /// 开始升级
        /// </summary>
        public static Int32 DHCLIENT_StartUpgradeEx(Int32 lLoginID, EM_UPGRADE_TYPE emType, string pchFileName, fUpgradeCallBack cbUpgrade, UInt32 dwUser)
        {
            Int32 nErr = CLIENT_StartUpgradeEx(lLoginID, emType, pchFileName, cbUpgrade, dwUser);
            Int32 errorCode = CLIENT_GetLastError();
            return nErr;
        }

        // <summary>
        /// 发送数据
        /// </summary>
        public static bool DHCLIENT_SendUpgrade(Int32 lUpgradeID)
        {
            return CLIENT_SendUpgrade(lUpgradeID);
        }


        // <summary>
        /// 结束升级设备程序
        /// </summary>
        public static bool DHCLIENT_StopUpgrade(Int32 lUpgradeID)
        {
            return CLIENT_StopUpgrade(lUpgradeID);
        }

#endregion

        #endregion

        #endregion

        #region << 公共函数 >>

        /// <summary>
        /// Windows系统标准时间格式转为自定义格式
        /// </summary>
        /// <param name="dateTime">系统时间对象</param>
        /// <returns>自定义时间格式的时间数据</returns>
        private static NET_TIME ToNetTime(DateTime dateTime)
        {
            NET_TIME result = new NET_TIME();
            result.dwYear = dateTime.Year;
            result.dwMonth = dateTime.Month;
            result.dwDay = dateTime.Day;
            result.dwHour = dateTime.Hour;
            result.dwMinute = dateTime.Minute;
            result.dwSecond = dateTime.Second;
            return result;
        }

        /// <summary>
        /// 特列UInt32类型值转换成指定的格式
        /// </summary>
        /// <param name="formatStyle">
        /// 格式参数:
        /// yyyy表示日期的年(4位);yy表示日期的年(2位);
        /// m表示日期的月(自动);mm表示日期的月(2位);
        /// d表示日期的天(自动);dd表示日期的天(2位);
        /// p1表示IP地址的第一部分;p2表示IP地址的第二部分;p3表示IP地址的第三部分;p4表示IP地址的第四部分;
        /// v1表示版本号的第一部分;v2表示版本号的第2部分;[版本号格式:主版本号.次版本号]
        /// vs1表示版本号的第一部分;vs2表示版本号的第2部分;vs3表示版本号的第3部分;vs4表示版本号的第4部分;[版本号格式:x.x.x.x]
        /// </param>
        /// <returns></returns>
        public static string DHUInt32ToString(UInt32 intValue,string formatStyle)
        {
            //转换为16进制表示值       
            string valueString = intValue.ToString("X");
            string strReturn = formatStyle.ToUpper();
            //格式化
            string strTemp = "00000000";
            strTemp = strTemp.Remove(0, valueString.Length) + valueString;
            string strPart1 = strTemp.Substring(0, 2);
            string strPart2 = strTemp.Substring(2, 2);
            string strPart3 = strTemp.Substring(4, 2);
            string strPart4 = strTemp.Substring(6, 2);
            //日期年的处理
            if (strReturn.IndexOf("YYYY") != -1)
            {
                strReturn = strReturn.Replace("YYYY", int.Parse(strPart1 + strPart2, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("YY") != -1)
            {
                strReturn = strReturn.Replace("YY", (int.Parse(strPart1 + strPart2, System.Globalization.NumberStyles.AllowHexSpecifier) % 100).ToString());
            }
            //日期月的处理
            if (strReturn.IndexOf("MM") != -1)
            {
                int mm = int.Parse(strPart3, System.Globalization.NumberStyles.AllowHexSpecifier);
                strReturn = strReturn.Replace("MM", ((mm>10?mm.ToString():"0"+mm.ToString())));
            }
            if (strReturn.IndexOf("M") != -1)
            {
                strReturn = strReturn.Replace("M", int.Parse(strPart3, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            //日期日的处理
            if (strReturn.IndexOf("DD") != -1)
            {
                int dd = int.Parse(strPart4, System.Globalization.NumberStyles.AllowHexSpecifier);
                strReturn = strReturn.Replace("DD", ((dd > 10 ? dd.ToString() : "0" + dd.ToString())));
            }
            if (strReturn.IndexOf("D") != -1)
            {
                strReturn = strReturn.Replace("D", int.Parse(strPart4, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            //IP地址的处理
            if (strReturn.IndexOf("P1") != -1)
            {
                strReturn = strReturn.Replace("P1", int.Parse(strPart1, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("P2") != -1)
            {
                strReturn = strReturn.Replace("P2", int.Parse(strPart2, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("P3") != -1)
            {
                strReturn = strReturn.Replace("P3", int.Parse(strPart1, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("P4") != -1)
            {
                strReturn = strReturn.Replace("P4", int.Parse(strPart1, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            //版本号的处理[主版本号.次版本号]
            if (strReturn.IndexOf("V1") != -1)
            {
                strReturn = strReturn.Replace("V1", int.Parse(strPart1 + strPart2, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("V2") != -1)
            {
                strReturn = strReturn.Replace("V2", int.Parse(strPart3 + strPart4, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            //版本号的处理[X.X.X.X]
            if (strReturn.IndexOf("VS1") != -1)
            {
                strReturn = strReturn.Replace("VS1", int.Parse(strPart1, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("VS2") != -1)
            {
                strReturn = strReturn.Replace("VS2", int.Parse(strPart2, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("VS3") != -1)
            {
                strReturn = strReturn.Replace("VS3", int.Parse(strPart1, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            if (strReturn.IndexOf("VS4") != -1)
            {
                strReturn = strReturn.Replace("VS4", int.Parse(strPart1, System.Globalization.NumberStyles.AllowHexSpecifier).ToString());
            }
            return strReturn;
        }

        /// <summary>
        /// 字符数组转换为字符串
        /// </summary>
        /// <param name="charValue">字符数组</param>
        /// <returns>字符串</returns>
        public static string DHByteArrayToString(byte[] byteArray)
        {
            //string result = Encoding.GetEncoding("gb2312").GetString(byteArray);
            //string result = Encoding.GetEncoding(936).GetString(byteArray);
            string result = pEncoding.GetString(byteArray);
            return result;
        }

        /// <summary>
        /// 字符数组转换为字符串[仅适用于字符数据长度为16的字符数组对象]
        /// </summary>
        /// <param name="charValue">字符数组</param>
        /// <param name="formatStyle">字符串格式[不区分大小写] IP1:IP地址的第一部分;IP2:IP地址第二部分;IP3:IP地址的第三部分;IP4:IP地址的第四部分</param>
        /// <returns>字符串</returns>
        public static string DHByteArrayToString(byte[] byteArray,string formatStyle)
        {
            //string result = Encoding.GetEncoding("gb2312").GetString(byteArray);
            //string result = Encoding.GetEncoding(936).GetString(byteArray);
            string result = pEncoding.GetString(byteArray);
            if (result.Length == 16)
            {
                string sPart1 = result.Substring(0, 4);
                string sPart2 = result.Substring(4, 4);
                string sPart3 = result.Substring(8, 4);
                string sPart4 = result.Substring(12, 4);
                string strTemp = formatStyle.ToUpper();
                //IP地址格式处理
                if (strTemp.IndexOf("IP1")!=-1)
                {
                    strTemp = strTemp.Replace("IP1", int.Parse(sPart1).ToString());
                }
                if (strTemp.IndexOf("IP2") != -1)
                {
                    strTemp = strTemp.Replace("IP2", int.Parse(sPart2).ToString());
                }
                if (strTemp.IndexOf("IP3") != -1)
                {
                    strTemp = strTemp.Replace("IP3", int.Parse(sPart3).ToString());
                }
                if (strTemp.IndexOf("IP4") != -1)
                {
                    strTemp = strTemp.Replace("IP4", int.Parse(sPart4).ToString());
                }
                result = strTemp;
            }
            return result;
        }


        /// <summary>
        /// 字符串转换为字符数组
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool DHStringToByteArry(string strValue,ref byte[] byteArry)
        {
            try
            {
                //byte[] byteTemp = Encoding.GetEncoding("gb2312").GetBytes(strValue);
                byte[] byteTemp = pEncoding.GetBytes(strValue);
                int maxLen=(byteTemp.Length>byteArry.Length?byteArry.Length:byteTemp.Length);
                for (int i = 0; i < byteArry.Length; i++)
                {
                    if (i < maxLen)
                    {
                        byteArry[i] = byteTemp[i];
                    }
                    else
                    {
                        byteArry[i] = new byte();
                    }
                }
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// SDK调用失败时抛出异常,成功时返回无异常信息,并把操作信息赋给LastOperationInfo
        /// </summary>
        /// <exception cref="Win32Exception">数字录像机原生异常,当SDK调用失败时触发</exception>
        private static void VIDEOThrowLastError()
        {
            Int32 errorCode = CLIENT_GetLastError();
            if (errorCode != 0)
            {
                pErrInfo.errCode = errorCode.ToString();
                pErrInfo.errMessage = VIDEOGetLastErrorName((uint)errorCode);
                if (pShowException == true)
                {
                    throw new Win32Exception(errorCode, pErrInfo.errMessage);
                }
            }
            else
            {
                pErrInfo.errCode = "0";
                pErrInfo.errMessage = "最近操作无异常发生";
            }
        }

        /// <summary>
        /// 错误代码转换为标准备的错误信息描述
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <returns>标准备错误信息描述</returns>
        private static string VIDEOGetLastErrorName(uint errorCode)
        {
            switch (errorCode)
            {
                case 0x80000000 | 1:
                    return "Windows系统出错";
                case 0x80000000 | 2:
                    return "网络错误，可能是因为网络超时";
                case 0x80000000 | 3:
                    return "设备协议不匹配";
                case 0x80000000 | 4:
                    return "句柄无效";
                case 0x80000000 | 5:
                    return "打开通道失败";
                case 0x80000000 | 6:
                    return "关闭通道失败";
                case 0x80000000 | 7:
                    return "用户参数不合法";
                case 0x80000000 | 8:
                    return "SDK初始化出错";
                case 0x80000000 | 9:
                    return "SDK清理出错";
                case 0x80000000 | 10:
                    return "申请render资源出错";
                case 0x80000000 | 11:
                    return "打开解码库出错";
                case 0x800000 | 12:
                    return "关闭解码库出错";
                case 0x80000000 | 13:
                    return "多画面预览中检测到通道数为0";
                case 0x80000000 | 14:
                    return "录音库初始化失败";
                case 0x80000000 | 15:
                    return "录音库未经初始化";
                case 0x80000000 | 16:
                    return "发送音频数据出错";
                case 0x80000000 | 17:
                    return "实时数据已经处于保存状态";
                case 0x80000000 | 18:
                    return "未保存实时数据";
                case 0x80000000 | 19:
                    return "打开文件出错";
                case 0x80000000 | 20:
                    return "启动云台控制定时器失败";
                case 0x80000000 | 21:
                    return "对返回数据的校验出错";
                case 0x80000000 | 22:
                    return "没有足够的缓存";
                case 0x80000000 | 23:
                    return "当前SDK未支持该功能";
                case 0x80000000 | 24:
                    return "查询不到录像";
                case 0x80000000 | 25:
                    return "无操作权限";
                case 0x80000000 | 26:
                    return "暂时无法执行";
                case 0x80000000 | 27:
                    return "未发现对讲通道";
                case 0x80000000 | 28:
                    return "未发现音频";
                case 0x80000000 | 29:
                    return "CLientSDK未经初始化";
                case 0x80000000 | 30:
                    return "下载已结束";
                case 0x80000000 | 31:
                    return "查询结果为空";
                case 0x80000000 | 32:
                    return "获取配置失败位置：系统属性";
                case 0x80000000 | 33:
                    return "获取配置失败位置：序列号";
                case 0x80000000 | 34:
                    return "获取配置失败位置：常规属性";
                case 0x80000000 | 35:
                    return "获取配置失败位置：DSP能力描述";
                case 0x80000000 | 36:
                    return "获取配置失败位置：网络属性";
                case 0x80000000 | 37:
                    return "获取配置失败位置：通道名称";
                case 0x80000000 | 38:
                    return "获取配置失败位置：视频属性";
                case 0x80000000 | 39:
                    return "获取配置失败位置：录像定时配置";
                case 0x80000000 | 40:
                    return "获取配置失败位置：解码器协议名称";
                case 0x80000000 | 41:
                    return "获取配置失败位置：232串口功能名称";
                case 0x80000000 | 42:
                    return "获取配置失败位置：解码器属性";
                case 0x80000000 | 43:
                    return "获取配置失败位置：232串口属性";
                case 0x80000000 | 44:
                    return "取配置失败位置：外部报警输入属性";
                case 0x80000000 | 45:
                    return "获取配置失败位置：图像检测报警属性";
                case 0x80000000 | 46:
                    return "获取配置失败位置：设备时间";
                case 0x80000000 | 47:
                    return "获取配置失败位置：预览参数";
                case 0x80000000 | 48:
                    return "获取配置失败位置：自动维护配置";
                case 0x80000000 | 49:
                    return "获取配置失败位置：视频矩阵配置";
                case 0x80000000 | 55:
                    return "设置配置失败位置：常规属性";
                case 0x80000000 | 56:
                    return "设置配置失败位置：网络属性";
                case 0x80000000 | 57:
                    return "设置配置失败位置：通道名称";
                case 0x80000000 | 58:
                    return "设置配置失败位置：视频属性";
                case 0x80000000 | 59:
                    return "设置配置失败位置：录像定时配置";
                case 0x80000000 | 60:
                    return "设置配置失败位置：解码器属性";
                case 0x80000000 | 61:
                    return "设置配置失败位置：232串口属性";
                case 0x80000000 | 62:
                    return "设置配置失败位置：外部报警输入属性";
                case 0x80000000 | 63:
                    return "设置配置失败位置：图像检测报警属性";
                case 0x80000000 | 64:
                    return "设置配置失败位置：设备时间";
                case 0x80000000 | 65:
                    return "设置配置失败位置：预览参数";
                case 0x80000000 | 66:
                    return "设置配置失败位置：自动维护配置";
                case 0x80000000 | 67:
                    return "设置配置失败位置：视频矩阵配置";
                case 0x80000000 | 70:
                    return "音频编码接口没有成功初始化";
                case 0x80000000 | 71:
                    return "数据过长";
                case 0x80000000 | 72:
                    return "设备不支持该操作";
                case 0x80000000 | 73:
                    return "设备资源不足";
                case 0x80000000 | 74:
                    return "服务器已经启动";
                case 0x80000000 | 75:
                    return "服务器尚未成功启动";
                case 0x80000000 | 80:
                    return "输入序列号有误";
                case 0x80000000 | 100:
                    return "密码不正确";
                case 0x80000000 | 101:
                    return "帐户不存在";
                case 0x80000000 | 102:
                    return "等待登录返回超时";
                case 0x80000000 | 103:
                    return "帐号已登录";
                case 0x80000000 | 104:
                    return "帐号已被锁定";
                case 0x80000000 | 105:
                    return "帐号已被列为黑名单";
                case 0x80000000 | 106:
                    return "资源不足，系统忙";
                case 0x80000000 | 107:
                    return "连接主机失败";
                case 0x80000000 | 108:
                    return "网络连接失败";
                case 0x80000000 | 120:
                    return "Render库打开音频出错";
                case 0x80000000 | 121:
                    return "Render库关闭音频出错";
                case 0x80000000 | 122:
                    return "Render库控制音量出错";
                case 0x80000000 | 123:
                    return "Render库设置画面参数出错";
                case 0x80000000 | 124:
                    return "Render库暂停播放出错";
                case 0x80000000 | 125:
                    return "Render库抓图出错";
                case 0x80000000 | 126:
                    return "Render库步进出错";
                case 0x80000000 | 127:
                    return "Render库设置帧率出错";
                case 0x80000000 | 999:
                    return "暂时无法设置";
                case 0x80000000 | 1000:
                    return "配置数据不合法";
                default:
                    return "未知错误";
            }
        }

        private static void VIDEOThrowLastError(int returnValue)
        {
            if (returnValue == 0)
            {
                VIDEOThrowLastError();
            }
            else
            {
                pErrInfo.errCode = "0";
                pErrInfo.errMessage = "最近操作无异常发生";
            }
        }

        private static void VIDEOThrowLastError(bool returnValue)
        {
            if (returnValue == false)
            {
                VIDEOThrowLastError();
            }
            else
            {
                pErrInfo.errCode = "0";
                pErrInfo.errMessage = "最近操作无异常发生";
            }
        }

        /// <summary>
        /// SDK调用失败时抛出异常
        /// </summary>
        /// <param name="e"></param>
        private static void VIDEOThrowLastError(Exception e)
        {

            pErrInfo.errCode = e.ToString();
            pErrInfo.errMessage = e.Message;
            if (pShowException == true)
            {
                throw e;
            }
        }
        
        #region **IP地址转换为字符数组功能代码[暂不用]**

        ///// <summary>
        ///// 将IP地址转为字符数组
        ///// </summary>
        ///// <param name="P1">IP地址的第一部分</param>
        ///// <param name="P2">IP地址的第二部分</param>
        ///// <param name="P3">IP地址的第三部分</param>
        ///// <param name="P4">IP地址的第四部分</param>
        ///// <returns></returns>
        //public static char[] DHIPAddToCharArry(string P1, string P2, string P3, string P4)
        //{
        //    char[] result = new char[16];
        //    string strResult = "";
        //    string strTemp = "0000";
        //    strTemp = strTemp.Remove(0, P1.ToString().Length) + P1.ToString();
        //    strResult+=strTemp;
        //    strTemp = "0000";
        //    strTemp = strTemp.Remove(0, P2.ToString().Length) + P2.ToString();
        //    strResult += strTemp;
        //    strTemp = "0000";
        //    strTemp = strTemp.Remove(0, P3.ToString().Length) + P3.ToString();
        //    strResult += strTemp;
        //    strTemp = "0000";
        //    strTemp = strTemp.Remove(0, P4.ToString().Length) + P4.ToString();
        //    strResult += strTemp;
        //    strResult.CopyTo(0, result, 0, 16);
        //    return result;
        //}

        ///// <summary>
        ///// 将IP地址的字符串转为字符数组
        ///// </summary>
        ///// <param name="IPAdd">IP地址的字符串</param>
        ///// <param name="strSprtr">分隔符</param>
        ///// <returns></returns>
        //public static char[] DHIPAddToCharArry(string IPAdd,char chrSprtr)
        //{
        //    string[] strPart = new string[4];
        //    strPart = IPAdd.Split(chrSprtr);
        //    if (strPart.Length ==4)
        //    {
        //        return DHIPAddToCharArry(strPart[0], strPart[1], strPart[2], strPart[3]);
        //    }
        //    else
        //    {
        //        return new char[16];
        //    }
        //}
        #endregion

        #endregion

        #region << 原SDK调用 >>

        /// <summary>
        /// 返回函数执行失败代码
        /// </summary>
        /// <returns>执行失败代码</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_GetLastError();

        /// <summary>
        /// 初始化SDK
        /// </summary>
        /// <param name="cbDisConnect">
        /// 断线回调函数,参见委托<seealso cref="fDisConnect"/>
        /// </param>
        /// <param name="dwUser">用户信息</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_Init(fDisConnect cbDisConnect, IntPtr dwUser);

        /// <summary>
        /// 清空SDK, 释放占用的资源，在所有的SDK函数之后调用
        /// </summary>
        [DllImport("dhnetsdk.dll")]
        private static extern void CLIENT_Cleanup();

        /// <summary>
        /// 设置与设备的连接等待时间
        /// </summary>
        /// <param name="nWaitTime">连接等待时间[单位:毫秒]</param>
        /// <param name="nTryTimes">连接次数</param>
        [DllImport("dhnetsdk.dll")]
        private static extern void CLIENT_SetConnectTime(int nWaitTime, int nTryTimes);

        /// <summary>
        /// 设置设备消息回调函数, 用来得到设备当前状态信息
        /// </summary>
        /// <param name="cbMessage">消息回调参数,参见委托<seealso cref="fMessCallBack"/></param>
        /// <param name="dwUser">用户数据</param>
        [DllImport("dhnetsdk.dll")]
        private static extern void CLIENT_SetDVRMessCallBack(fMessCallBack cbMessage, IntPtr dwUser);        

        /// <summary>
        /// 开始对某个设备订阅消息,用来设置是否需要对设备消息回调，得到的消息从<seealso cref="CLIENT_SetDVRMessCallBack"/>的设置值回调出来。
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StartListenEx(int lLoginID);

        /// <summary>
        /// 开始对某个设备订阅消息,用来设置是否需要对设备消息回调，得到的消息从<seealso cref="CLIENT_SetDVRMessCallBack"/>的设置值回调出来。
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StartListen(int lLoginID);

        /// <summary>
        /// 停止对某个设备侦听消息
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopListen(int lLoginID);

        /// <summary>
        /// 启动监听服务, 目前只实现了报警监听功能
        /// </summary>
        /// <param name="wPort">启动监听的端口</param>
        /// <param name="pIp">绑定的IP，为NULL时绑定本机所有合法IP</param>
        /// <param name="pfscb">服务器的消息回调接口</param>
        /// <param name="dwTimeOut">服务器维护连接的超时时间</param>
        /// <param name="dwUserData">用户回调的自定义数据</param>
        /// <returns>成功返回服务器句柄，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_StartService(ushort wPort, string pIp, fServiceCallBack pfscb, IntPtr dwTimeOut, IntPtr dwUserData);

        /// <summary>
        /// 停止端口监听服务
        /// </summary>
        /// <param name="lHandle">
        /// 要关闭的服务器的句柄:<seealso cref="CLIENT_StartService"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopService(int lHandle);

        /// <summary>
        /// 注册用户到设备，当设备端把用户设置为复用（设备默认的用户如admin,不能设置为复用），则使用该帐号可以多次向设备注册
        /// </summary>
        /// <param name="pchDVRIP">设备IP</param>
        /// <param name="wDVRPort">设备端口</param>
        /// <param name="pchUserName">用户名</param>
        /// <param name="pchPassword">用户密码</param>
        /// <param name="lpDeviceInfo">设备信息,属于输出参数</param>
        /// <param name="error">返回登录错误码</param>
        /// <returns>失败返回0，成功返回设备ID</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_Login(string pchDVRIP, ushort wDVRPort, string pchUserName, string pchPassword, out NET_DEVICEINFO lpDeviceInfo, out int error);

        /// <summary>
        /// 注册用户到设备的扩展接口，支持一个用户指定设备支持的能力
        /// </summary>
        /// <param name="pchDVRIP">设备IP</param>
        /// <param name="wDVRPort">设备端口</param>
        /// <param name="pchUserName">用户名</param>
        /// <param name="pchPassword">用户密码</param>
        /// <param name="nSpecCap">设备支持的能力，值为2表示主动侦听模式下的用户登录。[车载dvr登录]</param>
        /// <param name="pCapParam">对nSpecCap 的补充参数, nSpecCap = 2时，pCapParam填充设备序列号字串。[车载dvr登录]</param>
        /// <param name="lpDeviceInfo">设备信息,属于输出参数</param>
        /// <param name="error">返回登录错误码</param>
        /// <returns>失败返回0，成功返回设备ID</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_LoginEx(string pchDVRIP, ushort wDVRPort, string pchUserName, string pchPassword, int nSpecCap, string pCapParam, out NET_DEVICEINFO lpDeviceInfo, out int error);

        /// <summary>
        /// 注销设备用户
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_Logout(int lLoginID);

        /// <summary>
        /// 启动实时监视
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="hWnd">显示容器窗口句柄</param>
        /// <returns>失败返回0，成功返回实时监视ID(实时监视句柄)</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_RealPlay(int lLoginID, int nChannelID, IntPtr hWnd);

        /// <summary>
        /// 停止实时监视
        /// </summary>
        /// <param name="lRealHandle">实时监视句柄:<seealso cref="CLIENT_RealPlay"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopRealPlay(int lRealHandle);

        /// <summary>
        /// 停止实时监视
        /// </summary>
        /// <param name="lRealHandle">实时监视句柄:<seealso cref="CLIENT_RealPlay"/>的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopRealPlayEx(int lRealHandle);

        /// <summary>
        /// 设置实时监视数据回调，给用户提供设备流出的数据，当   cbRealData为NULL时结束回调数据
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="cbRealData">回调函数,用于传出设备流出的实时数据</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SetRealDataCallBack(int lRealHandle, fRealDataCallBack cbRealData, IntPtr dwUser);

        /// <summary>
        /// 设置实时监视数据回调扩展接口,是对上一接口的补充, 增加一个回调数据类型标志dwFlag 参数, 
        /// 可以选择性的回调出需要的数据, 对于没设置回调的数据类型就不回调出来了, 当设置为0x1f时与
        /// 上一接口效果一样, 不过对回调函数也做了扩展。
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="cbRealData">回调函数,用于传出设备流出的实时数据</param>
        /// <param name="dwUser">用户数据</param>
        /// <param name="dwFlag">数据类型标志</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SetRealDataCallBackEx(int lRealHandle, fRealDataCallBackEx cbRealData, IntPtr dwUser, UInt32 dwFlag);

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令<seealso cref="PTZControlType"/></param>
        /// <param name="dwStep">步进/速度</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_PTZControl(int lLoginID, int nChannelID, ushort dwPTZCommand, ushort dwStep, bool dwStop);

        /// <summary>
        /// 扩展云台控制, 对云台控制函数功能的增强控制
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_DHPTZControl(int lLoginID, int nChannelID, ushort dwPTZCommand, ushort param1, ushort param2, ushort param3, bool dwStop, IntPtr intPtr);

        /// <summary>
        /// 扩展云台控制, 支持三维快速定位
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwPTZCommand">云台控制命令</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="dwStop">是否停止</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_DHPTZControlEx(int lLoginID, int nChannelID, ushort dwPTZCommand, int lParam1, int lParam2, int lParam3, bool dwStop);

        /// <summary>
        /// 查询录像文件
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelId">通道ID</param>
        /// <param name="nRecordFileType">录像文件类型 </param>
        /// <param name="tmStart">录像开始时间</param>
        /// <param name="tmEnd">录像结束时间</param>
        /// <param name="pchCardid">卡号,只针对卡号查询有效，其他情况下可以填NULL</param>
        /// <param name="nriFileinfo">返回的录像文件信息，是一个NET_RECORDFILE_INFO结构数组</param>
        /// <param name="maxlen">nriFileinfo缓冲的最大长度;（单位字节，建议在100-200*sizeof(NET_RECORDFILE_INFO)之间）</param>
        /// <param name="filecount">返回的文件个数,属于输出参数最大只能查到缓冲满为止的录像记录</param>
        /// <param name="waittime">等待时间</param>
        /// <param name="bTime">是否按时间查(目前无效)</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryRecordFile(int lLoginID, int nChannelId, int nRecordFileType, ref NET_TIME tmStart, ref NET_TIME tmEnd, string pchCardid, IntPtr nriFileinfo, int maxlen, out  int filecount, int waittime, bool bTime);

        /// <summary>
        /// 网络回放[注意:用户登录一台设备后，每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录]
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="lpRecordFile">录像文件信息, 当按时间播放是只需填充起始时间和结束时间, 其他项填0</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_PlayBackByRecordFile(int lLoginID, ref NET_RECORDFILE_INFO lpRecordFile, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData);

        /// <summary>
        /// 带数据回调的按时间回放扩展接口。窗口参数（hWnd）有效时回调数据的返回值将被忽略，窗口参数 (hWnd)为0时，需要注意回调函数的返回值。
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelID">图像通道号，必须指定</param>
        /// <param name="lpStartTime">开始时间</param>
        /// <param name="lpStopTime">结束时间</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwPosUser">进度回调用户数据</param>
        /// <param name="fDownLoadDataCallBack">数据回调函数</param>
        /// <param name="dwDataUser">数据回调用户数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_PlayBackByTimeEx(int lLoginID,int nChannelID, ref NET_TIME lpStartTime,
                                                          ref NET_TIME lpStopTime, IntPtr hWnd,
                                                          fDownLoadPosCallBack cbDownLoadPos, IntPtr dwPosUser,
                                                          fDataCallBack fDownLoadDataCallBack, IntPtr dwDataUser);

        /// <summary>
        /// 带数据回调的按录像文件回放扩展接口,每通道同一时间只能播放一则录像,不能同时播放同一通道的多条记录。窗口参数（hWnd）有效时回调数据的返回值将被忽略，窗口参数 (hWnd)为0时，需要注意回调函数的返回值，具体见回调函数说明
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="lpRecordFile">录像文件信息</param>
        /// <param name="hWnd">回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwPosUser">进度回调函数用户数据</param>
        /// <param name="fDownLoadDataCallBack">数据回调函数</param>
        /// <param name="dwDataUser">数据回调函数用户数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_PlayBackByRecordFileEx(int lLoginID, ref NET_RECORDFILE_INFO lpRecordFile, IntPtr hWnd, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwPosUser,fDataCallBack fDownLoadDataCallBack, IntPtr dwDataUser);

        /// <summary>
        /// 网络按时间回放
        /// </summary>
        /// <param name="lLoginID">设备用户登录句柄</param>
        /// <param name="nChannelID">通道ID</param>
        /// <param name="lpStartTime">录像开始时间</param>
        /// <param name="lpStopTime">录像结束时间</param>
        /// <param name="hWnd">录像回放容器句柄</param>
        /// <param name="cbDownLoadPos">进度回调函数</param>
        /// <param name="dwUserData">用户自定义数据</param>
        /// <returns>成功返回网络回放ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_PlayBackByTime(int lLoginID, int nChannelID, ref NET_TIME lpStartTime,
                                                        ref NET_TIME lpStopTime, IntPtr hWnd,
                                                        fDownLoadPosCallBack cbDownLoadPos,
                                                        IntPtr dwUserData);
        /// <summary>
        /// 网络回放停止
        /// </summary>
        /// <param name="lPlayHandle">回放句柄</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopPlayBack(int lPlayHandle);

        /// <summary>
        /// 网络回放暂停与恢复播放
        /// </summary>
        /// <param name="lPlayHandle">播放句柄</param>
        /// <param name="bPause">1:暂停;0:恢复</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_PausePlayBack(int lPlayHandle, int bPause);

        /// <summary>
        /// 改变位置播放[即拖动播放，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义]
        /// </summary>
        /// <param name="lPlayHandle">播放句柄</param>
        /// <param name="offsettime">相对文件开始处偏移时间，单位为秒 .当其值为0xffffffff时,该参数无效.</param>
        /// <param name="offsetbyte">相对文件开始处偏移字节, 当其值为0xffffffff时, 该参数无效；当offsettime有效的时候,此参数无意义</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SeekPlayBack(int lPlayHandle, uint offsettime, uint offsetbyte);

        /// <summary>
        /// 单步播放[调用一次播放一帧图像，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义]
        /// </summary>
        /// <param name="lPlayHandle">回放句柄</param>
        /// <param name="bStop">是否停止单步播放, 在结束单步时调用</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StepPlayBack(int lPlayHandle, bool bStop);

        /// <summary>
        /// 快放[将当前帧率提高一倍,但是不能无限制的快放,目前最大200,大于时返回FALSE, 有音频的话不可以快放，慢放没有问题，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义]
        /// </summary>
        /// <param name="lPlayHandle">回放句柄</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_FastPlayBack(int lPlayHandle);

        /// <summary>
        /// 慢放[将当前帧率降低一倍,最慢为每秒一帧,小于1则返回FALSE，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义]
        /// </summary>
        /// <param name="lPlayHandle">回放句柄</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SlowPlayBack(int lPlayHandle);

        /// <summary>
        /// 得到SDK的版本号
        /// </summary>
        /// <returns>版本号</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern UInt32 CLIENT_GetSDKVersion();

        /// <summary>
        /// 用户自定义画图, 在打开图像之前调用此函数,否则无效,必须在所有窗口未显示之前调用, 可以用来对画面进行字符叠加
        /// </summary>
        /// <param name="cbDraw">画图回调，当设置为0时表示禁止回调</param>
        /// <param name="dwUser">用户数据</param>
        [DllImport("dhnetsdk.dll")]
        private static extern void CLIENT_RigisterDrawFun(fDrawCallBack cbDraw, IntPtr dwUser);

        /// <summary>
        ///  获取设备配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配置类型</param>
        /// <param name="lChannel">通道号，如果不是通道参数，lChannel不用,置为-1即可</param>
        /// <param name="lpOutBuffer">存放输出参数的缓冲区, 根据不同的类型, 输出不同的配置结构, 具体见数据结构定义</param>
        /// <param name="dwOutBufferSize">输入缓冲区的大小, (单位字节).</param>
        /// <param name="lpBytesReturned">实际返回的缓冲区大小,对应配置结构的大小, (单位字节)</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_GetDevConfig(int lLoginID, UInt32 dwCommand, int lChannel, IntPtr lpOutBuffer, UInt32 dwOutBufferSize, ref UInt32 lpBytesReturned, int waittime);

        /// <summary>
        /// 设置设备配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="dwCommand">配置类型</param>
        /// <param name="lChannel">通道号，如果设置全部通道数据为0xFFFFFFFF，如果命令不需要通道号，该参数无效</param>
        /// <param name="lpInBuffer">存放输入参数的缓冲区, 根据不同的类型, 输入不同的配置结构, 具体见数据结构定义</param>
        /// <param name="dwInBufferSize">输入缓冲区的大小, (单位字节)</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SetDevConfig(int lLoginID, UInt32 dwCommand, int lChannel, IntPtr lpInBuffer, UInt32 dwInBufferSize, int waittime);

        /// <summary>
        /// 查询设备的通道名称
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="pChannelName">通道名缓冲区（一般每个通道名是32字节长，这里需分配16*32字节长度的缓冲）</param>
        /// <param name="maxlen">缓冲区长度, (单位字节)</param>
        /// <param name="nChannelCount">总共通道数</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryChannelName(int lLoginID, ref char pChannelName, int maxlen, ref int nChannelCount, int waittime);

        /// <summary>
        /// 查询串口协议与解码器协议, 属于配置信息的一部分,查询前端设备目前支持的可选控制协议
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="nProtocolType">0是串口协议1是解码器协议(云台控制协议)</param>
        /// <param name="pProtocolBuffer">接收的协议缓冲区</param>
        /// <param name="maxlen">接收的协议缓冲区长度</param>
        /// <param name="nProtocollen">接收的总字节数,(单位字节).</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryComProtocol(int lLoginID, int nProtocolType, ref char pProtocolBuffer, int maxlen, ref int nProtocollen, int waittime);

        /// <summary>
        /// 查询系统信息,不同的信息有不同的数据结构
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="nSystemType">要查询的系统类型
        ///  0		一般信息
        ///  1		查询设备属性信息
        ///  2		查询硬盘信息
        ///  3		查询文件系统信息
        ///  4		查询视频属性信息
        ///  5		查询系统字符集属性信息
        ///  6		查询光存储设备信息
        ///  7		获取设备序列号
        /// </param>
        /// <param name="pSysInfoBuffer">接收的协议缓冲区</param>
        /// <param name="maxlen">接收的协议缓冲区长度,(单位字节)</param>
        /// <param name="nSysInfolen">接收的总字节数,(单位字节).</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QuerySystemInfo(int lLoginID, int nSystemType, IntPtr pSysInfoBuffer, int maxlen, ref  int nSysInfolen, int waittime);

        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryDevState(int lLoginID, int nType, IntPtr pBuf, int nBufLen,ref int pRetLen, int waittime);

        /// <summary>
        /// 获取设备配置信息
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="nConfigType">配置类型</param>
        /// <param name="pConfigbuf">配置的接收缓冲区, 根据不同的配置信息,构和数据长度也不一致</param>
        /// <param name="maxlen">配置的接收缓冲区最大长度,(单位字节)</param>
        /// <param name="nConfigbuflen">收到的配置包长度,(单位字节)</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryConfig(int lLoginID, int nConfigType, ref  char pConfigbuf, int maxlen, ref int nConfigbuflen, int waittime);

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="lLoginID">设备用户登录ID:CLIENT_Login的返回值</param>
        /// <param name="nConfigType">配置类型</param>
        /// <param name="pConfigbuf">设置配置缓冲区</param>
        /// <param name="nConfigbuflen">设置配置包长度,(单位字节)</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SetupConfig(int lLoginID, int nConfigType, ref char pConfigbuf, int nConfigbuflen, int waittime);

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="info">用户信息缓存，应传进一个已分配好内存的USER_MANAGE_INFO结构指针; 返回各用户信息USER_INFO中的password字段是加过密的。</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryUserInfo(int lLoginID, IntPtr info, int waittime);

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nOperateType">
        /// 设置类型
        /// 0		增加用户组－opParam为欲设置的USER_GROUP_INF结构指针
        /// 1		删除用户组－opParam为欲设置的USER_GROUP_INF结构指针其name成员是实际起作用的变量
        /// 2		修改用户组－opParam为新的USER_GROUP_INF结构指针，subParam为原始的USER_GROUP_INF结构指针，
        /// 3		增加用户 －opParam为欲增加的USER_INF结构指针其password成员传普通字符串即可，不用加密
        /// 4		删除用户 －opParam为欲删除的USER_INF结构指针其name成员是实际起作用的变量
        /// 5		修改用户 －opParam为新的USER_INF结构指针subParam为原始的USER_INF结构指针,其password成员必须是查询时返回的密码字段（加密过的）
        /// 6		修改用户密码－opParam为新的USER_INF结构指针subParam为原始的USER_INF结构指针,其password成员必须是普通字符串，不用加密
        /// </param>
        /// <param name="opParam">设置用户信息的输入缓冲，具体见nOperateType说明。</param>
        /// <param name="subParam">设置用户信息的输入缓冲，具体见nOperateType说明。</param>
        /// <param name="waittime">等待时间</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_OperateUserInfo(int lLoginID, int nOperateType, IntPtr opParam, IntPtr subParam, int waittime);

        /// <summary>
        /// 保存图片,对显示图像进行瞬间抓图，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义
        /// </summary>
        /// <param name="hPlayHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="pchPicFileName">位图文件名，当前只支持BMP位图</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_CapturePicture(int hPlayHandle, string pchPicFileName);

        
        /// <summary>
        /// 按文件下载录像文件, 通过查询到的文件信息下载
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="lpRecordFile">录像文件信息</param>
        /// <param name="sSavedFileName">要保存的录像文件名，全路径</param>
        /// <param name="cbDownLoadPos">下载进度回调函数</param>
        /// <param name="dwUserData">下载进度回调用户自定义数据</param>
        /// <returns>成功返回下载ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_DownloadByRecordFile(int lLoginID,ref NET_RECORDFILE_INFO lpRecordFile,string  sSavedFileName, fDownLoadPosCallBack cbDownLoadPos, IntPtr dwUserData);

        /// <summary>
        /// 按时间下载，直接输入指定通道起始时间和结束时间下载放录像
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelId">图像通道号，从0开始</param>
        /// <param name="nRecordFileType">保留</param>
        /// <param name="tmStart">开始时间</param>
        /// <param name="tmEnd">结束时间</param>
        /// <param name="sSavedFileName">要保存的录像文件名，全路径</param>
        /// <param name="cbTimeDownLoadPos">下载进度回调函数</param>
        /// <param name="dwUserData">下载进度回调用户自定义数据</param>
        /// <returns>成功返回下载ID，失败返回0</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_DownloadByTime(int lLoginID, int nChannelId, int nRecordFileType, ref  NET_TIME tmStart, ref  NET_TIME tmEnd, string sSavedFileName, fTimeDownLoadPosCallBack cbTimeDownLoadPos, IntPtr dwUserData);

        /// <summary>
        /// 停止下载录像文件
        /// </summary>
        /// <param name="lFileHandle">CLIENT_DownloadByRecordFile的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopDownload(int lFileHandle);

        /// <summary>
        /// 获得下载录像的当前位置,可以用于不需要实时显示下载进度的接口,与下载回调函数的功能类似
        /// </summary>
        /// <param name="lFileHandle">CLIENT_DownloadByRecordFile的返回值</param>
        /// <param name="nTotalSize">下载的总长度,单位KB</param>
        /// <param name="nDownLoadSize">已下载的长度,单位KB</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_GetDownloadPos(int lFileHandle, out int nTotalSize, out int nDownLoadSize);

        /// <summary>
        /// 启动实时监视或多画面预览
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelID">实时监视通道号，如果rType为RType_Multiplay该参数保留。当rType为RType_Multiplay_1~RType_Multiplay_16时，nChannelID决定了预览的画面，如当RType_Multiplay_4时，通道为4或5或6或7表示预览第五到第7通道的四画面预览。</param>
        /// <param name="hWnd">窗口句柄，值为0对数据不解码、不显示图像。</param>
        /// <param name="rType">数据类型(参考REALPLAY_TYPE)</param>
        /// <returns>失败返回0，成功返回实时监视ID(实时监视句柄)，将作为相关函数的参数</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern int CLIENT_RealPlayEx(int lLoginID, int nChannelID, IntPtr hWnd, REALPLAY_TYPE rType);

        /// <summary>
        /// 开始保存实时监视数据,对前端设备监视的图像进行数据保存,形成录像文件,此数据是设备端传送过来的原始视频数据
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="pchFileName">实时监视保存文件名</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SaveRealData(int lRealHandle, string pchFileName);

        /// <summary>
        /// 停止保存实时监视数据,关闭保存的文件
        /// </summary>
        /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopSaveRealData(int lRealHandle);
        
        /// <summary>
        /// 设置解码库视频参数，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义
        /// </summary>
        /// <param name="lPlayHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="nBrightness">亮度(0-255)</param>
        /// <param name="nContrast">对比度(0-255)</param>
        /// <param name="nHue">色度(0-255)</param>
        /// <param name="nSaturation">饱和度(0-255)</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_ClientSetVideoEffect(int lPlayHandle, byte nBrightness, byte nContrast, byte nHue, byte nSaturation);

        /// <summary>
        /// 获取解码库视频参数，只有打开图像的函数参数hWnd有效时该函数获取的参数才有效，否则无意义。
        /// </summary>
        /// <param name="lPlayHandle">CLIENT_RealPlay的返回值</param>
        /// <param name="nBrightness">返回亮度指针(0-255)</param>
        /// <param name="nContrast">返回对比度指针(0-255)</param>
        /// <param name="nHue">返回色度指针(0-255)</param>
        /// <param name="nSaturation">返回饱和度指针(0-255)</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_ClientGetVideoEffect(int lPlayHandle, ref byte nBrightness, ref byte nContrast, ref byte nHue, ref byte nSaturation);

        /// <summary>
        /// 获取配置，按照字符串格式
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="szCommand">命令参数，参见CLIENT_ParseData</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="szOutBuffer">输出缓冲 </param>
        /// <param name="dwOutBufferSize">输出缓冲大小</param>
        /// <param name="error">错误码</param>
        /// <param name="waittime">等待超时时间,默认设定为500</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_GetNewDevConfig(Int32 lLoginID, string szCommand, Int32 nChannelID, IntPtr szOutBuffer, UInt32 dwOutBufferSize, ref UInt32 error, Int32 waittime);

        /// <summary>
        /// 设置配置，按照字符串格式
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="szCommand">请参考CLIENT_GetNewDevConfig中的说明</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="szInBuffer">输入缓冲 </param>
        /// <param name="dwInBufferSize">输入缓冲大小</param>
        /// <param name="error">错误码</param>
        /// <param name="restart">配置设置后是否需要重启设备，1表示需要重启，0表示不需要重启</param>
        /// <param name="waittime">等待超时时间,默认设定为500</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SetNewDevConfig(Int32 lLoginID, string strCommand, Int32 lChannel, IntPtr lpInBuffer, UInt32 dwInBufferSize, ref UInt32 error, ref UInt32 restart, Int32 waittime);

        /// <summary>
        /// 新系统能力查询接口，查询系统能力信息(以Json格式，具体见配置SDK)
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="szCommand"></param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="szOutBuffer">输出缓冲</param>
        /// <param name="dwOutBufferSize">输出缓冲大小</param>
        /// <param name="error">错误码</param>
        /// <param name="waittime">等待超时时间,默认设定为1000</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_QueryNewSystemInfo(Int32 lLoginID, string szCommand, Int32 nChannelID, IntPtr szOutBuffer, UInt32 dwOutBufferSize, ref UInt32 error, Int32 waittime);

        /// <summary>
        /// 实时上传智能分析数据－图片
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwAlarmType">类型</param>
        /// <param name="cbAnalyzerData">分析数据回调</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_RealLoadPicture(Int32 lLoginID, Int32 nChannelID, UInt32 dwAlarmType, fAnalyzerDataCallBack cbAnalyzerData, UInt32 dwUser);

        /// <summary>
        /// 实时上传智能分析数据－图片(扩展接口，bNeedPicFile表示是否订阅图片文件,Reserved类型为RESERVED_PARA) 
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="nChannelID">通道号</param>
        /// <param name="dwAlarmType">类型</param>
        /// <param name="cbAnalyzerData">分析数据回调</param>
        /// <param name="dwUser">用户数据</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_RealLoadPictureEx(Int32 lLoginID, Int32 nChannelID, UInt32 dwAlarmType, bool bNeedPicFile, fAnalyzerDataCallBack cbAnalyzerData, UInt32 dwUser, IntPtr Reserved);

        /// <summary>
        /// 停止上传智能分析数据－图片
        /// </summary>
        /// <param name="lAnalyzerHandle">CLIENT_RealLoadPictureEx返回的值</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopLoadPic(Int32 lAnalyzerHandle);

        /// <summary>
        /// 设备控制
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="type">CtrlType的枚举</param>
        /// <param name="param"></param>
        /// <param name="waittime">超时等待，默认设定1000</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_ControlDevice(Int32 lLoginID, CtrlType type, IntPtr param, Int32 waittime);

        /// <summary>
        /// 解析查询到的配置信息
        /// </summary>
        /// <param name="szCommand">命令参数,查看开发文档</param>
        /// <param name="szInBuffer">输入缓冲，字符配置缓冲</param>
        /// <param name="lpOutBuffer">输出缓冲，结构体类型可以参见上表</param>
        /// <param name="dwOutBufferSize">输出缓冲的大小</param>
        /// <param name="pReserved">保留参数</param>
        /// <returns></returns>
        [DllImport("dhconfigsdk.dll")]
        private static extern bool CLIENT_ParseData(string szCommand, IntPtr szInBuffer, IntPtr lpOutBuffer, UInt32 dwOutBufferSize, IntPtr pReserved);


        /// <summary>
        /// 将需要设置的配置信息，打包成字符串格式
        /// </summary>
        /// <param name="szCommand">命令参数，参见CLIENT_ParseData中表格</param>
        /// <param name="lpInBuffer">输入缓冲，结构体类型参见CLIENT_ParseData中表格</param>
        /// <param name="dwInBufferSize">输入缓冲大小</param>
        /// <param name="szOutBuffer">输出缓冲</param>
        /// <param name="dwOutBufferSize">输出缓冲大小</param>
        /// <returns></returns>
        [DllImport("dhconfigsdk.dll")]
        private static extern bool CLIENT_PacketData(string strCommand, IntPtr lpInBuffer, UInt32 dwInBufferSize, IntPtr szOutBuffer, UInt32 dwOutBufferSize);

        /// <summary>
        /// 下载指定的智能分析数据 - 图片
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="emType">EM_FILE_QUERY_TYPE枚举</param>
        /// <param name="lpMediaFileInfo">图片文件数据</param>
        /// <param name="sSavedFileName">保存文件名</param>
        /// <param name="cbDownLoadPos">下载回调</param>
        /// <param name="dwUserData">用户数据</param>
        /// <param name="reserved">保留数据</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_DownloadMediaFile(Int32 lLoginID,EM_FILE_QUERY_TYPE emType, IntPtr lpMediaFileInfo, string sSavedFileName, fDownLoadPosCallBack cbDownLoadPos, UInt32 dwUserData,  IntPtr reserved);

        /// <summary>
        /// 停止下载数据
        /// </summary>
        /// <param name="lFileHandle">CLIENT_DownloadMediaFile返回值</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopDownloadMediaFile(Int32 lFileHandle);

        /// <summary>
        /// 按查询条件查询文件
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login的返回值</param>
        /// <param name="emType">media文件查询条件 </param>
        /// <param name="pQueryCondition">查询条件 </param>
        /// <param name="reserved">保留字节</param>
        /// <param name="waittime">等待时间</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32	CLIENT_FindFileEx(Int32 lLoginID, EM_FILE_QUERY_TYPE emType, IntPtr pQueryCondition, IntPtr reserved, Int32 waittime);	

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lFindHandle">CLIENT_FindFileEx的返回值</param>
        /// <param name="nFilecount">需要查询的条数</param>
        /// <param name="pMediaFileInfo">文件记录缓冲，用于输出查询到的媒体文件记录</param>
        /// <param name="maxlen">缓冲区的最大长度</param>
        /// <param name="reserved">保留字节</param>
        /// <param name="waittime">等待时间</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_FindNextFileEx(Int32 lFindHandle, Int32 nFilecount, IntPtr pMediaFileInfo, Int32 maxlen, IntPtr reserved, Int32 waittime);

        /// <summary>
        /// 关闭查询句柄
        /// </summary>
        /// <param name="lFindHandle">CLIENT_FindFileEx的返回值</param>
        /// <returns></returns>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_FindCloseEx(Int32 lFindHandle);

        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SnapPicture(Int32 lLoginID, SNAP_PARAMS par);

        [DllImport("dhnetsdk.dll")]
        private static extern void CLIENT_SetSnapRevCallBack(fSnapRev OnSnapRevMessage, Int32 dwUser);


        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_StartSearchDevices(fSearchDevicesCB cbSearchDevices, IntPtr pUserData, IntPtr szLocalIp);


        /// <summary>
        /// 停止异步搜索局域网内IPC、NVS等设备
        /// </summary>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopSearchDevices(Int32 lSearchHandle);

        /// <summary>
        /// 停止异步搜索局域网内IPC、NVS等设备
        /// </summary>
        [DllImport("dhnetsdk.dll")]
        private static extern Int32 CLIENT_StartUpgradeEx(Int32 lLoginID, EM_UPGRADE_TYPE emType, string pchFileName, fUpgradeCallBack cbUpgrade, UInt32 dwUser);

        // <summary>
        /// 发送数据
        /// </summary>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_SendUpgrade(Int32 lUpgradeID);


        // <summary>
        /// 结束升级设备程序
        /// </summary>
        [DllImport("dhnetsdk.dll")]
        private static extern bool CLIENT_StopUpgrade(Int32 lUpgradeID);

        #endregion


        #region <<常量字符串--配置命令 对应CLIENT_GetNewDevConfig和CLIENT_SetNewDevConfig接口>>
        /// <summary>
        /// 图像通道属性配置(对应CFG_ENCODE_INFO或AV_CFG_Encode)
        /// </summary>
        public static string CFG_CMD_ENCODE = "Encode";
        /// <summary>
        /// 定时录像配置(对应CFG_RECORD_INFO或AV_CFG_Record)
        /// </summary>
        public static string CFG_CMD_RECORD = "Record";
        /// <summary>
        /// 外部输入报警配置(对应CFG_ALARMIN_INFO)
        /// </summary>
        public static string CFG_CMD_ALARMINPUT = "Alarm";
        /// <summary>
        /// 网络报警配置(对应CFG_NETALARMIN_INFO)
        /// </summary>
        public static string CFG_CMD_NETALARMINPUT = "NetAlarm";
        /// <summary>
        /// 动态检测报警配置(对应CFG_MOTION_INFO)
        /// </summary>
        public static string CFG_CMD_MOTIONDETECT = "MotionDetect";
        /// <summary>
        /// 视频丢失报警配置(对应CFG_VIDEOLOST_INFO)
        /// </summary>
        public static string CFG_CMD_VIDEOLOST = "LossDetect";
        /// <summary>
        /// 视频遮挡报警配置(对应CFG_SHELTER_INFO)
        /// </summary>
        public static string CFG_CMD_VIDEOBLIND = "BlindDetect";
        /// <summary>
        /// 无存储设备报警配置(对应CFG_STORAGENOEXIST_INFO)
        /// </summary>
        public static string CFG_CMD_STORAGENOEXIST = "StorageNotExist";
        /// <summary>
        /// 存储设备访问出错报警配置(对应CFG_STORAGEFAILURE_INFO)
        /// </summary>
        public static string CFG_CMD_STORAGEFAILURE = "StorageFailure";
        /// <summary>
        /// 存储设备空间不足报警配置(对应CFG_STORAGELOWSAPCE_INFO)
        /// </summary>
        public static string CFG_CMD_STORAGELOWSAPCE = "StorageLowSpace";
        /// <summary>
        /// 网络断开报警配置(对应CFG_NETABORT_INFO)	
        /// </summary>
        public static string CFG_CMD_NETABORT = "NetAbort";
        /// <summary>
        /// IP冲突报警配置(对应CFG_IPCONFLICT_INFO)
        /// </summary>
        public static string CFG_CMD_IPCONFLICT = "IPConflict";
        /// <summary>
        /// 抓图能力查询(对应CFG_SNAPCAPINFO_INFO)
        /// </summary>
        public static string CFG_CMD_SNAPCAPINFO = "SnapInfo";
        /// <summary>
        /// 网络存储服务器配置(对应CFG_NAS_INFO)
        /// </summary>
        public static string CFG_CMD_NAS = "NAS";
        /// <summary>
        /// 云台配置(对应CFG_PTZ_INFO)
        /// </summary>
        public static string CFG_CMD_PTZ = "Ptz";
        /// <summary>
        /// 视频水印配置(对应CFG_WATERMARK_INFO)
        /// </summary>
        public static string CFG_CMD_WATERMARK = "WaterMark";
        /// <summary>
        /// 视频分析全局配置(对应CFG_ANALYSEGLOBAL_INFO)
        /// </summary>
        public static string CFG_CMD_ANALYSEGLOBAL = "VideoAnalyseGlobal";
        /// <summary>
        /// 物体的检测模块配置(对应CFG_ANALYSEMODULES_INFO)
        /// </summary>
        public static string CFG_CMD_ANALYSEMODULE = "VideoAnalyseModule";
        /// <summary>
        /// 视频分析规则配置(对应CFG_ANALYSERULES_INFO)
        /// </summary>
        public static string CFG_CMD_ANALYSERULE = "VideoAnalyseRule";
        /// <summary>
        /// 视频分析资源配置(对应CFG_ANALYSESOURCE_INFO)
        /// </summary>
        public static string CFG_CMD_ANALYSESOURCE = "VideoAnalyseSource";
        /// <summary>
        /// 雨刷配置(对应CFG_RAINBRUSH_INFO)
        /// </summary>
        public static string CFG_CMD_RAINBRUSH = "RainBrush";
        /// <summary>
        /// 智能交通抓拍(CFG_TRAFFICSNAPSHOT_INFO)
        /// </summary>
        public static string CFG_CMD_INTELLECTIVETRAFFIC = "TrafficSnapshot";
        /// <summary>
        /// 智能交通全局配置(CFG_TRAFFICGLOBAL_INFO)
        /// </summary>
        public static string CFG_CMD_TRAFFICGLOBAL = "TrafficGlobal";
        /// <summary>
        /// 普通配置 (CFG_DEV_DISPOSITION_INFO)
        /// </summary>
        public static string CFG_CMD_DEV_GENERRAL = "General";
        /// <summary>
        /// ATM前的移动报警(对应CFG_ATMMOTION_INFO)
        /// </summary>
        public static string CFG_CMD_ATMMOTION = "FetchMoneyOverTime";
        /// <summary>
        /// 设备状态信息(对应CFG_DEVICESTATUS_INFO)
        /// </summary>
        public static string CFG_CMD_DEVICESTATUS = "DeviceStatus";
        /// <summary>
        /// 扩展柜信息(对应CFG_HARDISKTANKGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_HARDDISKTANK = "HardDiskTank";
        /// <summary>
        /// Raid组信息(对应CFG_RAIDGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_RAIDGROUP = "RaidGroup";
        /// <summary>
        /// 存储池组信息(对应CFG_STORAGEPOOLGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_STORAGEPOOLGROUP = "StoragePoolGroup";
        /// <summary>
        /// 文件系统组信息(对应CFG_STORAGEPOSITIONGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_STORAGEPOSITIONGROUP = "StoragePositionGroup";
        /// <summary>
        /// 前端设备组信息(对应CFG_VIDEOINDEVGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_VIDEOINDEVGROUP = "VideoInDevGroup";
        /// <summary>
        /// 通道录像组状态(对应CFG_DEVRECORDGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_DEVRECORDGROUP = "DevRecordGroup";
        /// <summary>
        /// 服务状态(对应CFG_IPSERVER_STATUS)
        /// </summary>
        public static string CFG_CMD_IPSSERVER = "IpsServer";
        /// <summary>
        /// 抓图源配置(对应CFG_SNAPSOURCE_INFO)
        /// </summary>
        public static string CFG_CMD_SNAPSOURCE = "SnapSource";
        /// <summary>
        /// 大华雷达配置
        /// </summary>
        public static string CFG_CMD_DHRADER = "DahuaRadar";
        /// <summary>
        /// 川苏雷达配置
        /// </summary>
        public static string CFG_CMD_TRANSRADER = "TransRadar";
        /// <summary>
        /// 蓝盾雷达配置
        /// </summary>
        public static string CFG_CMD_LANDUNRADER = "LanDunRadar";
        /// <summary>
        /// 蓝盾线圈配置
        /// </summary>
        public static string CFG_CMD_LANDUNCOILS = "LanDunCoils";
        /// <summary>
        /// Spot视屏矩阵(对应CFG_VIDEO_MATRIX)
        /// </summary>
        public static string CFG_CMD_MATRIX_SPOT = "SpotMatrix";
        /// <summary>
        /// HDVR传每个数字通道的dsp信息 数字通道可以连IPC或DVR 也就是IPC或DVR的能力(对应CFG_DSPENCODECAP_INFO)
        /// </summary>
        public static string CFG_CMD_HDVR_DSP = "DspEncodeCap";
        /// <summary>
        /// HDVR传每个数字通道的所连设备的信息
        /// </summary>
        public static string CFG_CMD_HDVR_ATTR_CFG = "SystemAttr";
        public static string CFG_CMD_CHANNEL_HOLIDAY = "HolidaySchedule";
        /// <summary>
        /// 健康邮件
        /// </summary>
        public static string CFG_CMD_HEALTH_MAIL = "HealthMail";
        /// <summary>
        /// 视频分割轮巡配置(对应CFG_VIDEO_MATRIX)
        /// </summary>
        public static string CFG_CMD_SPLITTOUR = "SplitTour";
        /// <summary>
        /// 视频编码ROI(Region of Intrest)配置
        /// </summary>
        public static string CFG_CMD_VIDEOENCODEROI = "VideoEncodeROI";
        /// <summary>
        /// 测光配置(对应CFG_VIDEO_INMETERING_INFO)
        /// </summary>
        public static string CFG_CMD_VIDEO_INMETERING = "VideoInMetering";
        /// <summary>
        /// 交通流量统计配置(对应CFG_TRAFFIC_FLOWSTAT_INFO)
        /// </summary>
        public static string CFG_CMD_TRAFFIC_FLOWSTAT = "TrafficFlowStat";
        /// <summary>
        /// HDMI视频矩阵配置
        /// </summary>
        public static string CFG_CMD_HDMIMATRIX = "HDMIMatrix";
        /// <summary>
        /// 视频输入前端选项(对应CFG_VIDEO_IN_OPTIONS)
        /// </summary>
        public static string CFG_CMD_VIDEOINOPTIONS = "VideoInOptions";
        /// <summary>
        /// RTSP的配置( 对应CFG_RTSP_INFO_IN和CFG_RTSP_INFO_OUT )
        /// </summary>
        public static string CFG_CMD_RTSP = "RTSP";
        /// <summary>
        /// 智能交通抓拍(CFG_TRAFFICSNAPSHOT_INFO)
        /// </summary>
        public static string CFG_CMD_TRAFFICSNAPSHOT = "TrafficSnapshotNew";
        /// <summary>
        /// 智能交通抓拍(CFG_TRAFFICSNAPSHOT_NEW_INFO)
        /// </summary>
        public static string CFG_CMD_TRAFFICSNAPSHOT_MULTI = "TrafficSnapshotNew";
        /// <summary>
        /// 组播的相关配置(对应CFG_MULTICASTS_INFO_IN和CFG_MULTICASTS_INFO_OUT)
        /// </summary>
        public static string CFG_CMD_MULTICAST = "Multicast";
        /// <summary>
        /// 视频诊断参数表(CFG_VIDEODIAGNOSIS_PROFILE)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_PROFILE = "VideoDiagnosisProfile";
        /// <summary>
        /// 视频诊断任务表(CFG_VIDEODIAGNOSIS_TASK)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_TASK = "VideoDiagnosisTask";
        /// <summary>
        /// 视频诊断计划表(CFG_VIDEODIAGNOSIS_PROJECT)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_PROJECT = "VideoDiagnosisProject";
        /// <summary>
        /// 视频诊断全局表(CFG_VIDEODIAGNOSIS_GLOBAL)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_GLOBAL = "VideoDiagnosisGlobal";
        /// <summary>
        /// 视频诊断任务表(CFG_VIDEODIAGNOSIS_TASK)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_TASK_ONE = "VideoDiagnosisTask.x";
        /// <summary>
        /// 设备工作状态相关配置(对应CFG_TRAFFIC_WORKSTATE_INFO)
        /// </summary>
        public static string CFG_CMD_TRAFFIC_WORKSTATE = "WorkState";
        /// <summary>
        /// 磁盘存储组配置(对应CFG_STORAGEGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_STORAGEDEVGROUP = "StorageDevGroup";
        /// <summary>
        /// 录像存放的存储组配置(对应CFG_RECORDTOGROUP_INFO)
        /// </summary>
        public static string CFG_CMD_RECORDTOGROUP = "RecordToGroup";
        /// <summary>
        /// 智能跟踪场景配置(CFG_INTELLITRACKSCENE_INFO) 
        /// </summary>
        public static string CFG_CMD_INTELLITRACKSCENE = "IntelliTrackScene";
        /// <summary>
        /// 智能帧规则配置(对应CFG_ANALYSERULES_INFO)
        /// </summary>
        public static string CFG_CMD_IVSFRAM_RULE = "IVSFramRule";

        // 以下配置如果没有明确说明与通道无关, 那么分别具有单通道模式和所有通道模式
        // 通道无关时, 只能对应单个结构体和结构体数组的其中一种, 通道号必须为-1
        // 通道相关时, 单通道模式(0~n)对应单个配置结构体, 所有通道模式(-1)对应配置结构体数组
        /// <summary>
        /// 通道名称(对应AV_CFG_ChannelName)
        /// </summary>
        public static string CFG_CMD_CHANNELTITLE = "ChannelTitle";
        /// <summary>
        /// 录像模式(对应AV_CFG_RecordMode)
        /// </summary>
        public static string CFG_CMD_RECORDMODE = "RecordMode";
        /// <summary>
        /// 视频输出属性(对应AV_CFG_VideoOutAttr)
        /// </summary>
        public static string CFG_CMD_VIDEOOUT = "VideoOut";
        /// <summary>
        /// 远程设备信息(对应AV_CFG_RemoteDevice数组, 通道无关)
        /// </summary>
        public static string CFG_CMD_REMOTEDEVICE = "RemoteDevice";
        /// <summary>
        /// 远程通道(对应AV_CFG_RemoteChannel)
        /// </summary>
        public static string CFG_CMD_REMOTECHANNEL = "RemoteChannel";
        /// <summary>
        /// 画面轮训配置(对应AV_CFG_MonitorTour), 不支持
        /// </summary>
        public static string CFG_CMD_MONITORTOUR = "MonitorTour";
        /// <summary>
        /// 画面收藏配置, 不支持
        /// </summary>
        public static string CFG_CMD_MONITORCOLLECTION = "MonitorCollection";
        /// <summary>
        /// 画面分割显示源配置(对应AV_CFG_ChannelDisplaySource)
        /// </summary>
        public static string CFG_CMD_DISPLAYSOURCE = "DisplaySource";
        /// <summary>
        /// 存储卷组配置(对应AV_CFG_Raid数组, 通道无关)
        /// </summary>
        public static string CFG_CMD_RAID = "Raid";
        /// <summary>
        /// 录像源配置(对应AV_CFG_RecordSource)
        /// </summary>
        public static string CFG_CMD_RECORDSOURCE = "RecordSource";
        /// <summary>
        /// 视频输入颜色配置(对应AV_CFG_ChannelVideoColor)
        /// </summary>
        public static string CFG_CMD_VIDEOCOLOR = "VideoColor";
        /// <summary>
        /// 视频编码物件配置(对应AV_CFG_VideoWidget)
        /// </summary>
        public static string CFG_CMD_VIDEOWIDGET = "VideoWidget";
        /// <summary>
        /// 存储组信息(对应AV_CFG_StorageGroup数组, 通道无关), 不支持
        /// </summary>
        public static string CFG_CMD_STORAGEGROUP = "StorageGroup";
        /// <summary>
        /// 区域配置(对应AV_CFG_Locales)
        /// </summary>
        public static string CFG_CMD_LOCALS = "Locales";
        /// <summary>
        /// 语言选择(对应AV_CFG_Language)
        /// </summary>
        public static string CFG_CMD_LANGUAGE = "Language";
        /// <summary>
        /// 访问地址过滤(对应AV_CFG_AccessFilter)
        /// </summary>
        public static string CFG_CMD_ACCESSFILTER = "AccessFilter";
        /// <summary>
        /// 自动维护(对应AV_CFG_AutoMaintain)
        /// </summary>
        public static string CFG_CMD_AUTOMAINTAIN = "AutoMaintain";
        /// <summary>
        /// 远程设备事件处理(对应AV_CFG_RemoteEvent数组), 不支持
        /// </summary>
        public static string CFG_CMD_REMOTEEVENT = "RemoteEvent";
        /// <summary>
        /// 电视墙配置(对应AV_CFG_MonitorWall数组, 通道无关)
        /// </summary>
        public static string CFG_CMD_MONITORWALL = "MonitorWall";
        /// <summary>
        /// 融合屏配置(对应AV_CFG_SpliceScreen数组, 通道无关)
        /// </summary>
        public static string CFG_CMD_SPLICESCREEN = "VideoOutputComposite";
        /// <summary>
        /// 温度报警配置(对应AV_CFG_TemperatureAlarm, 通道无关)
        /// </summary>
        public static string CFG_CMD_TEMPERATUREALARM = "TemperatureAlarm";
        /// <summary>
        /// 风扇转速报警配置(对应AV_CFG_FanSpeedAlarm, 通道无关)
        /// </summary>
        public static string CFG_CMD_FANSPEEDALARM = "FanSpeedAlarm";
        /// <summary>
        /// 录像回传配置(对应AV_CFG_RecordBackup, 通道无关)
        /// </summary>
        public static string CFG_CMD_RECORDBACKUP = "RecordBackupRestore.BitrateLimit";
        /// <summary>
        /// 录像回传备用设备配置(AV_CFG_RemoteDevice[], 通道无关)
        /// </summary>
        public static string CFG_CMD_RECORDBACKUPDEVICE = "RecordBackupRestore.BackupDevices";
          #endregion

        #region <<常量字符串--能力集命令  对应CLIENT_QueryNewSystemInfo>>
        /// <summary>
        /// 视频分析能力集(对应CFG_CAP_ANALYSE_INFO)
        /// </summary>
        public static string CFG_CAP_CMD_VIDEOANALYSE = "devVideoAnalyse.getCaps";
        /// <summary>
        /// 获取后端设备的的在线状态(对应CFG_REMOTE_DEVICE_STATUS)
        /// </summary>
        public static string CFG_NETAPP_REMOTEDEVICE = "netApp.getRemoteDeviceStatus";
        /// <summary>
        /// 接入设备信息
        /// </summary>
        public static string CFG_CAP_CMD_PRODUCTDEFINITION = "magicBox.getProductDefinition";
        /// <summary>
        /// 设备智能分析能力(对应CFG_DEVICE_ANALYSE_INFO)兼容老设备
        /// </summary>
        public static string CFG_DEVICE_CAP_CMD_VIDEOANALYSE = "intelli.getVideoAnalyseDeviceChannels";
        /// <summary>
        /// 设备智能分析能力(对应CFG_CAP_DEVICE_ANALYSE_INFO)
        /// </summary>
        public static string CFG_DEVICE_CAP_NEW_CMD_VIDEOANALYSE = "devVideoAnalyse.factory.getCollect";
        /// <summary>
        /// 获得CPU个数
        /// </summary>
        public static string CFG_CAP_CMD_CPU_COUNT = "magicBox.getCPUCount";
        /// <summary>
        /// 获取CPU占用率
        /// </summary>
        public static string CFG_CAP_CMD_CPU_USAGE = "magicBox.getCPUUsage";
        /// <summary>
        /// 获得内存容量
        /// </summary>
        public static string CFG_CAP_CMD_MEMORY_INFO = "magicBox.getMemoryInfo";
        /// <summary>
        /// 获取设备状态信息 
        /// </summary>
        public static string CFG_CAP_CMD_DEVICE_STATE = "trafficSnap.getDeviceStatus";
        /// <summary>
        /// 视频输入能力集(对应CFG_CAP_VIDEOINPUT_INFO)
        /// </summary>
        public static string CFG_CAP_CMD_VIDEOINPUT = "devVideoInput.getCaps";
        /// <summary>
        /// 得到所有活动的用户信息(对应CFG_ACTIVEUSER_INFO)
        /// </summary>
        public static string CFG_USERMANAGER_ACTIVEUSER = "userManager.getActiveUserInfoAll";
        /// <summary>
        /// 获取视频统计摘要信息(对应CFG_VIDEOSATA_SUMMARY_INFO)
        /// </summary>
        public static string CFG_CAP_VIDEOSTAT_SUMMARY = "videoStatServer.getSummary";
        /// <summary>
        /// 获取视频诊断服务能力(CFG_VIDEODIAGNOSIS_CAPS_INFO)
        /// </summary>
        public static string CFG_CAP_CMD_VIDEODIAGNOSIS_SERVER = "videoDiagnosisServer.getCaps";
        /// <summary>
        /// 获取视频诊断通道数目(CFG_VIDEODIAGNOSIS_GETCOLLECT_INFO)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_GETCOLLECT = "videoDiagnosisServer.factory.getCollect";
        /// <summary>
        /// 获取视频诊断进行状态(CFG_VIDEODIAGNOSIS_GETSTATE_INFO)
        /// </summary>
        public static string CFG_CMD_VIDEODIAGNOSIS_GETSTATE = "videoDiagnosisServer.getState";

        #endregion

        #region <<字符串常量--单独解析命令  对应CLIENT_ParseData>>
        /// <summary>
        /// 视频分析ATM场景能力集(对应CFG_CAP_ATM_SCENE)
        /// </summary>
        public static string CFG_CAP_CMD_ATM_SCENE = "devVideoAnalyse.getCaps.ATM";
        /// <summary>
        /// 视频分析普通场景能力集(对应CFG_CAP_NORMAL_SCENE)
        /// </summary>
        public static string CFG_CAP_CMD_NORMAL_SCENE = "devVideoAnalyse.getCaps.Normal";
        #endregion

        #region <<常量UInt32>>
        public static UInt32 RESERVED_TYPE_FOR_COMMON = 0x00000010;

        // 查询类型，对应CLIENT_QueryDevState接口
        public static UInt32 DH_DEVSTATE_COMM_ALARM	=	0x0001;		// 查询普通报警状态(包括外部报警，视频丢失，动态检测)
        public static UInt32 DH_DEVSTATE_SHELTER_ALARM=	0x0002;		// 查询遮挡报警状态
        public static UInt32 DH_DEVSTATE_RECORDING	=	0x0003;		// 查询录象状态
        public static UInt32 DH_DEVSTATE_DISK		=	0x0004;		// 查询硬盘信息
        public static UInt32 DH_DEVSTATE_RESOURCE	=	0x0005;		// 查询系统资源状态
        public static UInt32 DH_DEVSTATE_BITRATE	=		0x0006;		// 查询通道码流
        public static UInt32 DH_DEVSTATE_CONN		=	0x0007	;	// 查询设备连接状态
        public static UInt32 DH_DEVSTATE_PROTOCAL_VER=	0x0008	;	// 查询网络协议版本号，pBuf = int*
        public static UInt32 DH_DEVSTATE_TALK_ECTYPE=		0x0009;		// 查询设备支持的语音对讲格式列表，见结构体DHDEV_TALKFORMAT_LIST
        public static UInt32 DH_DEVSTATE_SD_CARD	=		0x000A;		// 查询SD卡信息(IPC类产品)
        public static UInt32 DH_DEVSTATE_BURNING_DEV=		0x000B;		// 查询刻录机信息
        public static UInt32 DH_DEVSTATE_BURNING_PROGRESS= 0x000C;		// 查询刻录进度
        public static UInt32 DH_DEVSTATE_PLATFORM	=	0x000D	;	// 查询设备支持的接入平台
        public static UInt32 DH_DEVSTATE_CAMERA		=	0x000E	;	// 查询摄像头属性信息(IPC类产品)，pBuf = DHDEV_CAMERA_INFO *，可以有多个结构体
        public static UInt32 DH_DEVSTATE_SOFTWARE	=	0x000F	;	// 查询设备软件版本信息
        public static UInt32 DH_DEVSTATE_LANGUAGE    =    0x0010;		// 查询设备支持的语音种类
        public static UInt32 DH_DEVSTATE_DSP		=		0x0011;		// 查询DSP能力描述(对应结构体DHDEV_DSP_ENCODECAP)
        public static UInt32	DH_DEVSTATE_OEM		=		0x0012;		// 查询OEM信息
        public static UInt32	DH_DEVSTATE_NET		=		0x0013;		// 查询网络运行状态信息
        public static UInt32 DH_DEVSTATE_TYPE		=	0x0014	;	// 查询设备类型
        public static UInt32 DH_DEVSTATE_SNAP		=	0x0015	;	// 查询功能属性(IPC类产品)
        public static UInt32 DH_DEVSTATE_RECORD_TIME=		0x0016;		// 查询最早录像时间和最近录像时间
        public static UInt32 DH_DEVSTATE_NET_RSSI    =    0x0017   ;   // 查询无线网络信号强度，见结构体DHDEV_WIRELESS_RSS_INFO
        public static UInt32 DH_DEVSTATE_BURNING_ATTACH	=0x0018	;	// 查询附件刻录选项
        public static UInt32 DH_DEVSTATE_BACKUP_DEV	=	0x0019	;	// 查询备份设备列表
        public static UInt32 DH_DEVSTATE_BACKUP_DEV_INFO=	0x001a;		// 查询备份设备详细信息
        public static UInt32 DH_DEVSTATE_BACKUP_FEEDBACK=	0x001b;		// 备份进度反馈
        public static UInt32 DH_DEVSTATE_ATM_QUERY_TRADE = 0x001c;		// 查询ATM交易类型
        public static UInt32 DH_DEVSTATE_SIP			=	0x001d;		// 查询sip状态
        public static UInt32 DH_DEVSTATE_VICHILE_STATE=	0x001e	;	// 查询车载wifi状态
        public static UInt32 DH_DEVSTATE_TEST_EMAIL    =  0x001f ;     // 查询邮件配置是否成功
        public static UInt32 DH_DEVSTATE_SMART_HARD_DISK= 0x0020  ;    // 查询硬盘smart信息
        public static UInt32 DH_DEVSTATE_TEST_SNAPPICTURE= 0x0021  ;   // 查询抓图设置是否成功
        public static UInt32 DH_DEVSTATE_STATIC_ALARM   = 0x0022    ;  // 查询静态报警状态
        public static UInt32 DH_DEVSTATE_SUBMODULE_INFO = 0x0023     ; // 查询设备子模块信息
        public static UInt32 DH_DEVSTATE_DISKDAMAGE     = 0x0024   ;   // 查询硬盘坏道能力 
        public static UInt32 DH_DEVSTATE_IPC            = 0x0025    ;  // 查询设备支持的IPC能力, 见结构体DH_DEV_IPC_INFO
        public static UInt32 DH_DEVSTATE_ALARM_ARM_DISARM= 0x0026   ;  // 查询报警布撤防状态
        public static UInt32 DH_DEVSTATE_ACC_POWEROFF_ALARM= 0x0027 ;  // 查询ACC断电报警状态(返回一个DWORD, 1表示断电，0表示通电)
        public static UInt32 DH_DEVSTATE_TEST_FTP_SERVER= 0x0028    ;  // 测试FTP服务器连接
        public static UInt32 DH_DEVSTATE_3GFLOW_EXCEED  = 0x0029    ;  // 查询3G流量超出阈值状态,(见结构体DHDEV_3GFLOW_EXCEED_STATE_INFO)
        public static UInt32 DH_DEVSTATE_3GFLOW_INFO     =0x002a    ;  // 查询3G网络流量信息,见结构体DH_DEV_3GFLOW_INFO
        public static UInt32 DH_DEVSTATE_VIHICLE_INFO_UPLOAD=  0x002b; // 车载自定义信息上传(见结构体ALARM_VEHICLE_INFO_UPLOAD)
        public static UInt32 DH_DEVSTATE_SPEED_LIMIT     =0x002c     ; // 查询限速报警状态(见结构体ALARM_SPEED_LIMIT)
        public static UInt32 DH_DEVSTATE_DSP_EX         = 0x002d     ; // 查询DSP扩展能力描述(对应结构体DHDEV_DSP_ENCODECAP_EX)
        public static UInt32 DH_DEVSTATE_3GMODULE_INFO   = 0x002e    ; // 查询3G模块信息(对应结构体DH_DEV_3GMODULE_INFO)
        public static UInt32 DH_DEVSTATE_MULTI_DDNS      =0x002f     ; // 查询多DDNS状态信息(对应结构体DH_DEV_MULTI_DDNS_INFO)
        public static UInt32 DH_DEVSTATE_CONFIG_URL      =0x0030     ; // 查询设备配置URL信息(对应结构体DH_DEV_URL_INFO)
        public static UInt32 DH_DEVSTATE_HARDKEY = 0x0031;// 查询HardKey状态（对应结构体DHDEV_HARDKEY_STATE)

        #endregion

    }
    #region <<智能分析事件类型>>
    
    public sealed class EventIvs
    {
        // 
        /// <summary>
        /// 订阅所有事件
        /// </summary>
        public static UInt32 EVENT_IVS_ALL = 0x00000001;
        /// <summary>
        /// 警戒线事件
        /// </summary>
        public static UInt32 EVENT_IVS_CROSSLINEDETECTION = 0x00000002;
        /// <summary>
        /// 警戒区事件
        /// </summary>
        public static UInt32 EVENT_IVS_CROSSREGIONDETECTION = 0x00000003;
        /// <summary>
        /// 贴条事件
        /// </summary>
        public static UInt32 EVENT_IVS_PASTEDETECTION = 0x00000004;
        /// <summary>
        /// 物品遗留事件
        /// </summary>
        public static UInt32 EVENT_IVS_LEFTDETECTION = 0x00000005;
        /// <summary>
        /// 停留事件
        /// </summary>
        public static UInt32 EVENT_IVS_STAYDETECTION = 0x00000006;
        /// <summary>
        /// 徘徊事件
        /// </summary>
        public static UInt32 EVENT_IVS_WANDERDETECTION = 0x00000007;
        /// <summary>
        /// 物品保全事件
        /// </summary>
        public static UInt32 EVENT_IVS_PRESERVATION = 0x00000008;
        /// <summary>
        /// 移动事件
        /// </summary>
        public static UInt32 EVENT_IVS_MOVEDETECTION = 0x00000009;
        /// <summary>
        /// 尾随事件
        /// </summary>
        public static UInt32 EVENT_IVS_TAILDETECTION = 0x0000000A;
        /// <summary>
        /// 聚众事件
        /// </summary>
        public static UInt32 EVENT_IVS_RIOTERDETECTION = 0x0000000B;
        /// <summary>
        /// 火警事件
        /// </summary>
        public static UInt32 EVENT_IVS_FIREDETECTION = 0x0000000C;
        /// <summary>
        /// 烟雾报警事件
        /// </summary>
        public static UInt32 EVENT_IVS_SMOKEDETECTION = 0x0000000D;
        /// <summary>
        /// 斗殴事件
        /// </summary>
        public static UInt32 EVENT_IVS_FIGHTDETECTION = 0x0000000E;
        /// <summary>
        /// 流量统计事件
        /// </summary>
        public static UInt32 EVENT_IVS_FLOWSTAT = 0x0000000F;
        /// <summary>
        /// 数量统计事件
        /// </summary>
        public static UInt32 EVENT_IVS_NUMBERSTAT = 0x00000010;
        /// <summary>
        /// 摄像头覆盖事件
        /// </summary>
        public static UInt32 EVENT_IVS_CAMERACOVERDDETECTION = 0x00000011;
        /// <summary>
        /// 摄像头移动事件
        /// </summary>
        public static UInt32 EVENT_IVS_CAMERAMOVEDDETECTION = 0x00000012;
        /// <summary>
        /// 视频异常事件
        /// </summary>
        public static UInt32 EVENT_IVS_VIDEOABNORMALDETECTION = 0x00000013;
        /// <summary>
        /// 视频损坏事件
        /// </summary>
        public static UInt32 EVENT_IVS_VIDEOBADDETECTION = 0x00000014;
        /// <summary>
        /// 交通管制事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFICCONTROL = 0x00000015;
        /// <summary>
        /// 交通事故事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFICACCIDENT = 0x00000016;
        /// <summary>
        /// 交通路口事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFICJUNCTION = 0x00000017;
        /// <summary>
        /// 交通卡口事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFICGATE = 0x00000018;
        /// <summary>
        /// 交通抓拍事件
        /// </summary>
        public static UInt32 EVENT_TRAFFICSNAPSHOT = 0x00000019;
        /// <summary>
        /// 人脸检测事件
        /// </summary>
        public static UInt32 EVENT_IVS_FACEDETECT = 0x0000001A;
        /// <summary>
        /// 交通拥堵事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFICJAM = 0x0000001B;
        /// <summary>
        /// 交通违章-闯红灯事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_RUNREDLIGHT = 0x00000100;
        /// <summary>
        /// 交通违章-压车道线事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_OVERLINE = 0x00000101;
        /// <summary>
        /// 交通违章-逆行事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_RETROGRADE = 0x00000102;
        /// <summary>
        /// 交通违章-违章左转
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_TURNLEFT = 0x00000103;
        /// <summary>
        /// 交通违章-违章右转
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_TURNRIGHT = 0x00000104;
        /// <summary>
        /// 交通违章-违章掉头
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_UTURN = 0x00000105;
        /// <summary>
        /// 交通违章-超速
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_OVERSPEED = 0x00000106;
        /// <summary>
        /// 交通违章-低速
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_UNDERSPEED = 0x00000107;
        /// <summary>
        /// 交通违章-违章停车
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_PARKING = 0x00000108;
        /// <summary>
        /// 交通违章-不按车道行驶
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_WRONGROUTE = 0x00000109;
        /// <summary>
        /// 交通违章-违章变道
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_CROSSLANE = 0x0000010A;
        /// <summary>
        /// 交通违章-压黄线 
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_OVERYELLOWLINE = 0x0000010B;
        /// <summary>
        /// 交通违章-路肩行驶事件  
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_DRIVINGONSHOULDER = 0x0000010C;
        /// <summary>
        /// 交通违章-黄牌车占道事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_YELLOWPLATEINLANE = 0x0000010E;
        /// <summary>
        /// 交通违章-斑马线行人优先事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_PEDESTRAINPRIORITY = 0X0000010F;
        /// <summary>
        /// 翻越围栏事件
        /// </summary>
        public static UInt32 EVENT_IVS_CROSSFENCEDETECTION = 0x0000011F;
        /// <summary>
        /// 电火花事件
        /// </summary>
        public static UInt32 EVENT_IVS_ELECTROSPARKDETECTION = 0X00000110;
        /// <summary>
        /// 交通违章-禁止通行事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_NOPASSING = 0x00000111;
        /// <summary>
        /// 异常奔跑事件
        /// </summary>
        public static UInt32 EVENT_IVS_ABNORMALRUNDETECTION = 0x00000112;
        /// <summary>
        /// 人员逆行事件
        /// </summary>
        public static UInt32 EVENT_IVS_RETROGRADEDETECTION = 0x00000113;
        /// <summary>
        /// 区域内检测事件
        /// </summary>
        public static UInt32 EVENT_IVS_INREGIONDETECTION = 0x00000114;
        /// <summary>
        /// 物品搬移事件
        /// </summary>
        public static UInt32 EVENT_IVS_TAKENAWAYDETECTION = 0x00000115;
        /// <summary>
        /// 非法停车事件
        /// </summary>
        public static UInt32 EVENT_IVS_PARKINGDETECTION = 0x00000116;
        /// <summary>
        /// 人脸识别事件
        /// </summary>
        public static UInt32 EVENT_IVS_FACERECOGNITION = 0x00000117;
        /// <summary>
        /// 交通手动抓拍事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_MANUALSNAP = 0x00000118;
        /// <summary>
        /// 交通流量统计事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_FLOWSTATE = 0x00000119;
        /// <summary>
        /// 交通滞留事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_STAY = 0x0000011A;
        /// <summary>
        /// 有车占道事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_VEHICLEINROUTE = 0x0000011B;
        /// <summary>
        /// 视频移动侦测事件
        /// </summary>
        public static UInt32 EVENT_ALARM_MOTIONDETECT = 0x0000011C;
        /// <summary>
        /// 外部报警事件
        /// </summary>
        public static UInt32 EVENT_ALARM_LOCALALARM = 0x0000011D;
        /// <summary>
        /// 看守所囚犯起身事件
        /// </summary>
        public static UInt32 EVENT_IVS_PRISONERRISEDETECTION = 0X0000011E;
        /// <summary>
        /// 交通卡口事件
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_TOLLGATE = 0X00000120;
        /// <summary>
        /// 人员密集度检测
        /// </summary>
        public static UInt32 EVENT_IVS_DENSITYDETECTION = 0x00000121;
        /// <summary>
        /// 视频诊断结果事件
        /// </summary>
        public static UInt32 EVENT_IVS_VIDEODIAGNOSIS = 0X00000122;
        /// <summary>
        /// 所有以traffic开头的事件，目前指的是
        /// EVENT_IVS_TRAFFICCONTROL -> EVENT_TRAFFICSNAPSHOT
        /// EVENT_IVS_TRAFFIC_RUNREDLIGHT -> EVENT_IVS_TRAFFIC_UNDERSPEED
        /// </summary>
        public static UInt32 EVENT_IVS_TRAFFIC_ALL = 0x000001FF;
    }
    #endregion

    #endregion

    #region << 委托 >>

    /// <summary>
    /// 断线回调函数，回调出当前网络已经断开的设备, 对调用SDK的ClIENT_LogOut()函数主动断开的设备不回调
    /// </summary>
    /// <param name="lLoginID">设备用户登录句柄</param>
    /// <param name="pchDVRIP">DVR设备IP</param>
    /// <param name="nDVRPort">DVR设备连接端口</param>
    /// <param name="dwUser">用户数据</param>
    public delegate void fDisConnect(
        int lLoginID, StringBuilder pchDVRIP, int nDVRPort, IntPtr dwUser);

    /// <summary>
    /// 设备消息回调函数
    /// </summary>
    /// <param name="lCommand">回调类型</param>
    /// <param name="lLoginID">设备用户登录ID:<seealso cref="CLIENT_Login"/>返回值</param>
    /// <param name="pBuf">接收报警数据的缓存，根据调用的侦听接口和lCommand值不同，填充的数据不同</param>
    /// <param name="dwBufLen">pBuf的长度[单位字节]</param>
    /// <param name="pchDVRIP">设备IP</param>
    /// <param name="nDVRPort">设备端口</param>
    /// <param name="dwUser">回调的用户数据</param>
    /// <returns>true:成功;false:失败</returns>
    public delegate bool fMessCallBack(
        int lCommand, int lLoginID, IntPtr pBuf, UInt32 dwBufLen, IntPtr pchDVRIP, int nDVRPort, UInt32 dwUser);

    /// <summary>
    /// 监听回调函数
    /// </summary>
    /// <param name="lHandle">回调消息的服务器句柄</param>
    /// <param name="pIp">上传消息的设备Ip</param>
    /// <param name="wPort">上传消息的设备Port</param>
    /// <param name="lCommand">回调类型</param>
    /// <param name="pParam">接收报警数据的缓存，根据调用的侦听接口和lCommand值不同，填充的数据不同</param>
    /// <param name="dwParamLen">pParam的长度[单位字节]</param>
    /// <param name="dwUserData">用户回调的自定义数据</param>
    /// <returns></returns>
    public delegate int fServiceCallBack(
        int lHandle, string pIp, ushort wPort, int lCommand, string pParam, ushort dwParamLen, IntPtr dwUserData);

    /// <summary>
    /// 进度回调函数
    /// </summary>
    /// <param name="lPlayHandle">播放句柄:<seealso cref="CLIENT_PlayBackByRecordFile"/>返回值</param>
    /// <param name="dwTotalSize">指本次播放总大小，单位为KB</param>
    /// <param name="dwDownLoadSize">指已经播放的大小，单位为KB，当其值为-1时表示本次回防结束</param>
    /// <param name="dwUser">用户数据</param>
    public delegate void fDownLoadPosCallBack(int lPlayHandle, UInt32 dwTotalSize, UInt32 dwDownLoadSize, IntPtr dwUser);


    public delegate void fTimeDownLoadPosCallBack(
            int lPlayHandle, int dwTotalSize, int dwDownLoadSize, int index, NET_RECORDFILE_INFO recordfileinfo, IntPtr dwUser);

    /// <summary>
    /// 画图回调
    /// </summary>
    /// <param name="lLoginID">设备用户登录句柄:<seealso cref="CLIENT_Login"/>的返回值，标识设备</param>
    /// <param name="lPlayHandle">实时监视播放句柄:<seealso cref="CLIENT_RealPlay"/>的返回值，标识通道。</param>
    /// <param name="hDC">是对整个显示画面区域的画板指针,根据用户的需要可以对任何位置叠加显示</param>
    /// <param name="dwUser">用户数据，就是上面输入的用户数据</param>
    public delegate void fDrawCallBack(int lLoginID, int lPlayHandle, IntPtr hDC, IntPtr dwUser);

    /// <summary>
    /// 实时监视数据回调
    /// </summary>
    /// <param name="lRealHandle">CLIENT_RealPlay的返回值</param>
    /// <param name="dwDataType">
    /// 标识回调出来的数据类型, 一旦设置回调,是以下4种数据都会同时回调出来,用户可以根据需要有选择的处理部分数据; 
    /// 0:原始数据(与SaveRealData保存的数据一致);
    /// 1:标准视频数据;
    /// 2:yuv数据;
    /// 3:pcm音频数据;
    /// </param>
    /// <param name="pBuffer">回调数据,根据数据类型的不同每次回调不同的长度的数据,除类型0, 其他数据类型都是按帧,每次回调一帧数据</param>
    /// <param name="dwBufSize">回调数据的长度, (单位字节)</param>
    /// <param name="dwUser">用户数据</param>
    public delegate void fRealDataCallBack(int lRealHandle, UInt32 dwDataType,IntPtr pBuffer, UInt32 dwBufSize, IntPtr dwUser);

    /// <summary>
    /// 实时监视数据回调(实时监视数据回调)
    /// </summary>
    /// <param name="lRealHandle">实时监视ID</param>
    /// <param name="dwDataType">
    /// 标识回调出来的数据类型, 只有dwFlag设置标识的数据才会回调出来：
    /// 0:原始数据(与SaveRealData保存的数据一致);
    /// 1:标准视频数据;
    /// 2:yuv数据
    /// 3:pcm音频数据
    /// </param>
    /// <param name="pBuffer">回调数据,根据数据类型的不同每次回调不同的长度的数据,除类型0, 其他数据类型都是按帧,每次回调一帧数据</param>
    /// <param name="dwBufSize">回调数据的长度, 根据不同的类型, 长度也不同, (单位字节)</param>
    /// <param name="param">回调数据参数结构体, 根据不同的类型, 参数结构也不一致, 当类型为0 (原始数据)和 2 (YUV数据) 时为0</param>
    /// <param name="dwUser">用户数据，就是上面输入的用户数据</param>
    public delegate void fRealDataCallBackEx(int lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, int param, IntPtr dwUser);

    /// <summary>实时监视数据回调(实时监视数据回调)</summary>
    /// <param name="lRealHandle">实时监视ID</param>
    /// <param name="dwDataType">
    /// 标识回调出来的数据类型, 只有dwFlag设置标识的数据才会回调出来：
    /// 0:原始数据(与SaveRealData保存的数据一致);
    /// 1:标准视频数据;
    /// 2:yuv数据
    /// 3:pcm音频数据
    /// </param>
    /// <param name="pBuffer">回调数据,根据数据类型的不同每次回调不同的长度的数据,除类型0, 其他数据类型都是按帧,每次回调一帧数据</param>
    /// <param name="dwBufSize">回调数据的长度, 根据不同的类型, 长度也不同, (单位字节)</param>
    /// <param name="param">回调数据参数结构体, 根据不同的类型, 参数结构也不一致, 当类型为0 (原始数据)和 2 (YUV数据) 时为0</param>
    /// <param name="dwUser">用户数据，就是上面输入的用户数据</param>
    /// <returns>
    /// 本回调函数的返回值会影响SDK的内部操作：
    /// 1:代表回调成功，没什么特殊情况都应该返回这个值
    /// 0:代表应用程序缓冲满，网络SDK将停顿一小会（ms级），然后回调同一数据，一般用于数据速度控制。
    /// -1:代表系统出错，将直接结束回放线程，慎用
    /// </returns>
    public delegate int fDataCallBack(int lRealHandle,UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, UInt32 dwUser);

    /// <summary>
    /// 实时上传智能分析数据－图片  的回调
    /// </summary>
    /// <param name="lAnalyzerHandle"></param>
    /// <param name="dwAlarmType"></param>
    /// <param name="pAlarmInfo"></param>
    /// <param name="pBuffer"></param>
    /// <param name="dwBufSize"></param>
    /// <param name="dwUser"></param>
    /// <param name="nSequence">表示上传的相同图片情况，为0时表示是第一次出现，
    /// 为2表示最后一次出现或仅出现一次，为1表示此次之后还有</param>
    /// <param name="reserved">int nState = (int*) reserved 表示当前回调数据的状态, 为0表示当前数据为实时数据，
    /// 为1表示当前回调数据是离线数据，为2时表示离线数据传送结束</param>
    /// <returns></returns>
    public delegate int fAnalyzerDataCallBack(Int32 lAnalyzerHandle, UInt32 dwAlarmType, IntPtr pAlarmInfo, IntPtr pBuffer, UInt32 dwBufSize, UInt32 dwUser, Int32 nSequence, IntPtr reserved);

    /// <summary>
    /// // 抓图回调函数原形
    /// </summary>
    /// <param name="lLoginID"></param>
    /// <param name="pBuf"></param>
    /// <param name="RevLen"></param>
    /// <param name="EncodeType"></param>
    /// <param name="CmdSerial"></param>
    /// <param name="dwUser"></param>
    public delegate void fSnapRev(Int32 lLoginID, IntPtr pBuf, UInt32 RevLen, UInt32 EncodeType, UInt32 CmdSerial, UInt32 dwUser);


    /// <summary>
    /// 异步搜索设备回调
    /// </summary>
    /// <param name="pDevNetInfo"></param>
    /// <param name="pUserData"></param>
    public delegate void fSearchDevicesCB(IntPtr pDevNetInfo, IntPtr pUserData);

    /// <summary>
    /// 升级设备程序回调函数原形
    /// </summary>
    /// <param name="lLoginID"></param>
    /// <param name="pBuf"></param>
    /// <param name="RevLen"></param>
    /// <param name="EncodeType"></param>
    /// <param name="CmdSerial"></param>
    /// <param name="dwUser"></param>
    public delegate void fUpgradeCallBack(Int32 lLoginID, UInt32 lUpgradechannel, Int32 nTotalSize, Int32 nSendSize, UInt32 dwUser);

    #endregion

 

    #region << 枚举定义>>

    /// <summary>
    /// 实时播放类型
    /// </summary>
    public enum REALPLAY_TYPE
    {
        DH_RType_RealPlay = 0,
        DH_RType_Multiplay,
        DH_RType_RealPlay_0,
        DH_RType_RealPlay_1,
        DH_RType_RealPlay_2,
        DH_RType_RealPlay_3,
        DH_RType_Multiplay_1,
        DH_RType_Multiplay_4,
        DH_RType_Multiplay_8,
        DH_RType_Multiplay_9,
        DH_RType_Multiplay_16,
    }

    /// <summary>
    /// 播放控制命令
    /// </summary>
    public enum PLAY_CONTROL
    {
        /// <summary>
        /// 播放
        /// </summary>
        Play,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 拖动播放[按时间偏移]
        /// </summary>
        SeekByTime,
        /// <summary>
        /// 拖动播放[按字节偏移]
        /// </summary>
        SeekByBit,
        /// <summary>
        /// 单步播放开始[调用一次播放一帧]
        /// </summary>
        StepPlay,
        /// <summary>
        /// 单步播放停止
        /// </summary>
        StepStop,
        /// <summary>
        /// 快放
        /// </summary>
        Fast,
        /// <summary>
        /// 慢放
        /// </summary>
        Slow
    }

    /// <summary>
    /// 云台控制命令
    /// </summary>
    public enum PTZ_CONTROL
    {
        /// <summary>
        /// 上
        /// </summary>
        PTZ_UP_CONTROL = 0,
        /// <summary>
        /// 下
        /// </summary>
        PTZ_DOWN_CONTROL,
        /// <summary>
        /// 左
        /// </summary>
        PTZ_LEFT_CONTROL,
        /// <summary>
        /// 右
        /// </summary>
        PTZ_RIGHT_CONTROL,
        /// <summary>
        /// 变倍+
        /// </summary>
        PTZ_ZOOM_ADD_CONTROL,
        /// <summary>
        /// 变倍-
        /// </summary>
        PTZ_ZOOM_DEC_CONTROL,
        /// <summary>
        /// 调焦+
        /// </summary>
        PTZ_FOCUS_ADD_CONTROL,
        /// <summary>
        /// 调焦- 
        /// </summary>
        PTZ_FOCUS_DEC_CONTROL,
        /// <summary>
        /// 光圈+ 
        /// </summary>
        PTZ_APERTURE_ADD_CONTROL,
        /// <summary>
        /// 光圈- 
        /// </summary>
        PTZ_APERTURE_DEC_CONTROL,
        /// <summary>
        /// 转至预置点 
        /// </summary>
        PTZ_POINT_MOVE_CONTROL,
        /// <summary>
        /// 设置 
        /// </summary>
        PTZ_POINT_SET_CONTROL,
        /// <summary>
        /// 删除 
        /// </summary>
        PTZ_POINT_DEL_CONTROL,
        /// <summary>
        /// 点间轮循 
        /// </summary>
        PTZ_POINT_LOOP_CONTROL,
        /// <summary>
        /// 灯光雨刷	
        /// </summary>
        PTZ_LAMP_CONTROL,
        /// <summary>
        /// 左上:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_LEFTTOP = 0X20,
        /// <summary>
        /// 右上:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_RIGHTTOP,
        /// <summary>
        /// 左下:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_LEFTDOWN,
        /// <summary>
        /// 右下:p1水平速度, p2垂直速度
        /// </summary>
        EXTPTZ_RIGHTDOWN,
        /// <summary>
        /// 加入预置点到巡航:p1巡航线路	p2预置点值
        /// </summary>
        EXTPTZ_ADDTOLOOP,
        /// <summary>
        /// 删除巡航中预置点:p1巡航线路	p2预置点值
        /// </summary>
        EXTPTZ_DELFROMLOOP,
        /// <summary>
        /// 清除巡航:p1巡航线路	
        /// </summary>
        EXTPTZ_CLOSELOOP,
        /// <summary>
        /// 开始水平旋转
        /// </summary>
        EXTPTZ_STARTPANCRUISE,
        /// <summary>
        /// 停止水平旋转	
        /// </summary>
        EXTPTZ_STOPPANCRUISE,
        /// <summary>
        /// 设置左边界
        /// </summary>
        EXTPTZ_SETLEFTBORDER,
        /// <summary>
        /// 设置右边界		
        /// </summary>
        EXTPTZ_RIGHTBORDER,
        /// <summary>
        /// 开始线扫
        /// </summary>
        EXTPTZ_STARTLINESCAN,
        /// <summary>
        /// 停止线扫
        /// </summary>
        EXTPTZ_CLOSELINESCAN,
        /// <summary>
        /// 设置模式开始:模式线路
        /// </summary>
        EXTPTZ_SETMODESTART,
        /// <summary>
        /// 设置模式结束:模式线路
        /// </summary>
        EXTPTZ_SETMODESTOP,
        /// <summary>
        /// 运行模式:p1模式线路
        /// </summary>		
        EXTPTZ_RUNMODE,
        /// <summary>
        /// 停止模式:p1模式线路
        /// </summary>
        EXTPTZ_STOPMODE,
        /// <summary>
        /// 清除模式:p1模式线路
        /// </summary>
        EXTPTZ_DELETEMODE,
        /// <summary>
        /// 翻转命令
        /// </summary>
        EXTPTZ_REVERSECOMM,
        /// <summary>
        /// 快速定位  p1水平坐标 p2垂直坐标	 p3变倍
        /// </summary>
        EXTPTZ_FASTGOTO,
        /// <summary>
        /// x34:辅助开关开	 p1辅助点		
        /// </summary>
        EXTPTZ_AUXIOPEN,
        /// <summary>
        /// 0x35:辅助开关关	p1辅助点
        /// </summary>
        EXTPTZ_AUXICLOSE,
        /// <summary>
        /// 打开球机菜单
        /// </summary>
        EXTPTZ_OPENMENU = 0X36,
        /// <summary>
        /// 关闭菜单
        /// </summary>
        EXTPTZ_CLOSEMENU,
        /// <summary>
        /// 菜单确定
        /// </summary>
        EXTPTZ_MENUOK,
        /// <summary>
        /// 菜单取消
        /// </summary>
        EXTPTZ_MENUCANCEL,
        /// <summary>
        /// 菜单上
        /// </summary>
        EXTPTZ_MENUUP,
        /// <summary>
        /// 菜单下
        /// </summary>
        EXTPTZ_MENUDOWN,
        /// <summary>
        /// 菜单左
        /// </summary>
        EXTPTZ_MENULEFT,
        /// <summary>
        /// 菜单右
        /// </summary>
        EXTPTZ_MENURIGHT,
        /// <summary>
        /// 最大命令值
        /// </summary>
        EXTPTZ_TOTAL
    }

    /// <summary>
    /// 录像文件类型
    /// </summary>
    public enum RECORD_FILE_TYPE
    {
        /// <summary>
        /// 0:所有录像文件
        /// </summary>
        ALLRECORDFILE,
        /// <summary>
        /// 1:外部报警   
        /// </summary>
        OUTALARM,
        /// <summary>
        /// 2:动态检测报警
        /// </summary>
        DYNAMICSCANALARM,
        /// <summary>
        /// 3:所有报警
        /// </summary>
        ALLALARM,
        /// <summary>
        /// 4:卡号查询
        /// </summary>
        CARDNOSEACH,
        /// <summary>
        /// 5:组合条件查询
        /// </summary>
        COMBINEDSEACH
    }

    /// <summary>
    /// 配制命令
    /// </summary>
    public enum CONFIG_COMMAND
    {
        /// <summary>
        /// 1:获取设备参数,参见DHDEV_SYSTEM_ATTR_CFG
        /// </summary>
        DH_DEV_DEVICECFG = 1,
        /// <summary>
        /// 2:获取网络参数,参见DHDEV_NET_CFG
        /// </summary>
        DH_DEV_NETCFG = 2,
        /// <summary>
        /// 3:获取通道配置－图像及压缩参数等,参见DHDEV_CHANNEL_CFG
        /// </summary>
        DH_DEV_CHANNELCFG = 3,
        /// <summary>
        /// 4:获取预览参数,参见DHDEV_PREVIEW_CFG
        /// </summary>
        DH_DEV_PREVIEWCFG = 4,
        /// <summary>
        /// 5:获取录像时间参数,参见DHDEV_RECORD_CFG
        /// </summary>
        DH_DEV_RECORDCFG = 5,
        /// <summary>
        /// 6:获取串口参数,参见DHDEV_COMM_CFG
        /// </summary>
        DH_DEV_COMMCFG = 6,
        /// <summary>
        /// 7:获取报警参数,参见DHDEV_ALARM_SCHEDULE
        /// </summary>
        DH_DEV_ALARMCFG = 7,
        /// <summary>
        /// 8:获取DVR时间,参见NET_TIME结构
        /// </summary>
        DH_DEV_TIMECFG = 8,
        /// <summary>
        /// 9:对讲参数,参见DHDEV_TALK_CFG
        /// </summary>
        DH_DEV_TALKCFG = 9,
        /// <summary>
        /// 10:自动维护配置
        /// </summary>
        DH_DEV_AUTOMTCFG = 10,
        /// <summary>
        /// 11:本机矩阵控制策略配置	
        /// </summary>
        DH_DEV_VEDIO_MARTIX = 11,
        /// <summary>
        /// 12:多ddns服务器配置
        /// </summary>
        DH_DEV_MULTI_DDNS = 12,
        /// <summary>
        /// 13:抓图相关配置
        /// </summary>
        DH_DEV_SNAP_CFG = 13,
        /// <summary>
        /// 14:HTTP路径配置
        /// </summary>
        DH_DEV_WEB_URL_CFG = 14,
        /// <summary>
        /// 15:FTP上传配置
        /// </summary>
        DH_DEV_FTP_PROTO_CFG = 15,
        /// <summary>
        /// 16:平台接入配置，此时channel参数代表平台类型，如 11 代表U网通
        /// </summary>
        DH_DEV_INTERVIDEO_CFG = 16,
        /// <summary>
        /// IP过滤配置扩展(对应结构体DHDEV_IPIFILTER_CFG_EX)
        /// </summary>
        DH_DEV_IPFILTER_CFG_EX = 0x0056,
        /// <summary>
        /// MAC,IP过滤(要求ip,mac是一一对应的)配置(对应结构体DHDEV_MACIPFILTER_CFG)
        /// </summary>
        DH_DEV_MACIPFILTER_CFG = 0x0074,
        /// <summary>
        /// 码流加密配置(对应结构体DHEDV_STREAM_ENCRYPT)
        /// </summary>
        DH_DEV_STREAM_ENCRYPT_CFG = 0x0055,
        /// <summary>
        /// 本地报警配置(结构体DH_ALARMIN_CFG_EX)
        /// </summary>
        DH_DEV_LOCALALARM_CFG = 0x0039,
    }

    /// <summary>
    /// 设备型号(DVR类型)
    /// </summary>
    public enum NET_DEVICE_TYPE
    {
        NET_PRODUCT_NONE = 0,
        /// <summary>
        /// 非实时MACE  
        /// </summary>
        NET_DVR_NONREALTIME_MACE,
        /// <summary>
        /// 非实时 
        /// </summary>
        NET_DVR_NONREALTIME,
        /// <summary>
        /// 网络视频服务器
        /// </summary>
        NET_NVS_MPEG1,
        /// <summary>
        ///  MPEG1二路录像机  
        /// </summary>
        NET_DVR_MPEG1_2,
        /// <summary>
        /// MPEG1八路录像机  
        /// </summary>
        NET_DVR_MPEG1_8,
        /// <summary>
        /// MPEG4八路录像机
        /// </summary>
        NET_DVR_MPEG4_8,
        /// <summary>
        /// MPEG4十六路录像机  
        /// </summary>
        NET_DVR_MPEG4_16,
        /// <summary>
        /// MPEG4视新十六路录像机  
        /// </summary>
        NET_DVR_MPEG4_SX2,
        /// <summary>
        /// MPEG4视通录像机
        /// </summary>
        NET_DVR_MEPG4_ST2,
        /// <summary>
        /// MPEG4视豪录像机
        /// </summary>
        NET_DVR_MEPG4_SH2,
        /// <summary>
        /// MPEG4视通二代增强型录像机
        /// </summary>
        NET_DVR_MPEG4_GBE,
        /// <summary>
        /// MPEG4网络视频服务器II代
        /// </summary>
        NET_DVR_MPEG4_NVSII,
        /// <summary>
        /// 新标准配置协议
        /// </summary>
        NET_DVR_STD_NEW,
        /// <summary>
        /// DDNS服务器
        /// </summary>
        NET_DVR_DDNS
    };

    /// <summary>
    /// 查询日志扩展接口参数：设备日志类型
    /// </summary>
    public enum DH_LOG_QUERY_TYPE
    {
        /// <summary>
        /// 所有日志
        /// </summary>
        DHLOG_ALL = 0,
        /// <summary>
        /// 系统日志
        /// </summary>
        DHLOG_SYSTEM,
        /// <summary>
        /// 配置日志
        /// </summary>
        DHLOG_CONFIG,
        /// <summary>
        /// 存储相关
        /// </summary>
        DHLOG_STORAGE,
        /// <summary>
        /// 报警日志
        /// </summary>
        DHLOG_ALARM,
        /// <summary>
        /// 录像相关
        /// </summary>
        DHLOG_RECORD,
        /// <summary>
        /// 帐号相关
        /// </summary>
        DHLOG_ACCOUNT,
        /// <summary>
        /// 清除日志
        /// </summary>
        DHLOG_CLEAR,
        /// <summary>
        /// 回放相关
        /// </summary>
        DHLOG_PLAYBACK
    }

    /// <summary>
    /// 日志的类型
    /// </summary>
    public enum DH_LOG_TYPE
    {
        /// <summary>
        /// 重起应用程序
        /// </summary>
        DH_LOG_REBOOT = 0x0000,
        /// <summary>
        /// 关闭应用程序
        /// </summary>
        DH_LOG_SHUT,
        /// <summary>
        /// 保存配制
        /// </summary>
        DH_LOG_CONFSAVE = 0x0100,
        /// <summary>
        /// 读取配置
        /// </summary>
        DH_LOG_CONFLOAD,
        /// <summary>
        /// 文件系统错误
        /// </summary>
        DH_LOG_FSERROR = 0x0200,
        /// <summary>
        /// 硬盘写错误
        /// </summary>
        DH_LOG_HDD_WERR,
        /// <summary>
        /// 硬盘读错误
        /// </summary>
        DH_LOG_HDD_RERR,
        /// <summary>
        /// 设置硬盘类型
        /// </summary>
        DH_LOG_HDD_TYPE,
        /// <summary>
        /// 格式化硬盘
        /// </summary>
        DH_LOG_HDD_FORMAT,
        /// <summary>
        /// 外部报警输入
        /// </summary>
        DH_LOG_ALM_IN = 0x0300,
        /// <summary>
        /// 外部报警输入停止
        /// </summary>
        DH_LOG_ALM_END = 0x0302,
        /// <summary>
        /// 手动录像开
        /// </summary>
        DH_LOG_MANUAL_RECORD = 0x0401,
        /// <summary>
        /// 录像停止
        /// </summary>
        DH_LOG_CLOSED_RECORD,
        /// <summary>
        /// 用户管理:登录
        /// </summary>
        DH_LOG_LOGIN = 0x0500,
        /// <summary>
        /// 用户管理:注销
        /// </summary>
        DH_LOG_LOGOUT,
        /// <summary>
        /// 用户管理:添加用户
        /// </summary>
        DH_LOG_ADD_USER,
        /// <summary>
        /// 用户管理:删除用户
        /// </summary>
        DH_LOG_DELETE_USER,
        /// <summary>
        /// 用户管理:修改用户
        /// </summary>
        DH_LOG_MODIFY_USER,
        /// <summary>
        /// 用户管理:添加组
        /// </summary>
        DH_LOG_ADD_GROUP,
        /// <summary>
        /// 用户管理:删除组
        /// </summary>
        DH_LOG_DELETE_GROUP,
        /// <summary>
        /// 用户管理:修改组
        /// </summary>
        DH_LOG_MODIFY_GROUP,
        /// <summary>
        /// 日志清除
        /// </summary>
        DH_LOG_CLEAR = 0x0600,			//clear 
        /// <summary>
        /// 
        /// </summary>
        DH_LOG_TYPE_NR = 7,
    }

    /// <summary>
    /// 配制的类型
    /// </summary>
    /// C++中名称为CFG_INDEX
    public enum CFG_TYPE
    {
        /// <summary>
        /// 普通
        /// </summary>
        CFG_GENERAL = 0,
        /// <summary>
        /// 串口
        /// </summary>
        CFG_COMM,
        /// <summary>
        /// 网络
        /// </summary>
        CFG_NET,
        /// <summary>
        /// 录像
        /// </summary>
        CFG_RECORD,
        /// <summary>
        /// 视频通道
        /// </summary>
        CFG_CAPTURE,
        /// <summary>
        /// 云台
        /// </summary>
        CFG_PTZ,
        /// <summary>
        /// 动态检测
        /// </summary>
        CFG_DETECT,
        /// <summary>
        /// 报警
        /// </summary>
        CFG_ALARM,
        /// <summary>
        /// 显示
        /// </summary>
        CFG_DISPLAY,
        /// <summary>
        /// 通道标题
        /// </summary>
        CFG_TITLE = 10,
        /// <summary>
        /// 邮件
        /// </summary>
        CFG_MAIL,
        /// <summary>
        /// 预览图像配置
        /// </summary>
        CFG_EXCAPTURE,
        /// <summary>
        /// pppoe拨号配置
        /// </summary>
        CFG_PPPOE,
        /// <summary>
        /// ddns配置
        /// </summary>
        CFG_DDNS,
        /// <summary>
        /// 抓包配置
        /// </summary>
        CFG_SNIFFER,
        /// <summary>
        /// DSP编码能力信息
        /// </summary>
        CFG_DSPINFO,
        /// <summary>
        /// 色彩配置	
        /// </summary>
        CFG_COLOR = 126,
        /// <summary>
        /// 所有配置
        /// </summary>
        CFG_ALL,
    }

    /// <summary>
    /// 定时录像类型－定时、动态检测、报警
    /// </summary>
    public enum REC_TYPE
    {
        DH_REC_TYPE_TIM = 0,
        DH_REC_TYPE_MTD,
        DH_REC_TYPE_ALM,
        DH_REC_TYPE_NUM,
    }

    /// <summary>
    /// 用户管理操作命令
    /// </summary>
    public enum USER_OPERATE
    {
        /// <summary>
        /// 新增用户组
        /// </summary>
        DH_GROUP_ADD=0,
        /// <summary>
        /// 删除用户组
        /// </summary>
        DH_GROUP_DELETE,
        /// <summary>
        /// 修改用户组
        /// </summary>
        DH_GROUP_EDIT,
        /// <summary>
        /// 新增用户
        /// </summary>
        DH_USER_ADD,
        /// <summary>
        /// 删除用户
        /// </summary>
        DH_USER_DELETE,
        /// <summary>
        /// 修改用户
        /// </summary>
        DH_USER_EDIT,
        /// <summary>
        /// 修改用户密码
        /// </summary>
        DH_USER_CHANGEPASSWORD
        
    }

    /// <summary>
    /// 回调信息类型
    /// </summary>
    public enum CALLBACK_TYPE
    {        
        /// <summary>
        /// 常规报警信息
        /// </summary>
        DH_COMM_ALARM = 0x1100,
        /// <summary>
        /// 视频遮挡报警
        /// </summary>
        DH_SHELTER_ALARM = 0x1101,
        /// <summary>
        /// 硬盘满报警
        /// </summary>
        DH_DISK_FULL_ALARM = 0X1102,
        /// <summary>
        /// 硬盘故障报警
        /// </summary>
        DH_DISK_ERROR_ALARM = 0x1103,
        /// <summary>
        /// 音频检测报警
        /// </summary>
        DH_SOUND_DETECT_ALARM = 0x1104,
        /// <summary>
        /// 报警解码器报警
        /// </summary>
        DH_ALARM_DECODER_ALARM = 0x1105,

        /*以下为扩展回调报警信息类型，对应CLIENT_StartListenEx*/

        /// <summary>
        /// 外部报警
        /// </summary>
        DH_ALARM_ALARM_EX = 0x2101,
        /// <summary>
        /// 动态检测报警
        /// </summary>
        DH_MOTION_ALARM_EX = 0x2102,
        /// <summary>
        /// /视频丢失报警
        /// </summary>
        DH_VIDEOLOST_ALARM_EX = 0X2103,
        /// <summary>
        /// 遮挡报警
        /// </summary>
        DH_SHELTER_ALARM_EX = 0x2104,
        /// <summary>
        /// 音频检测报警
        /// </summary>
        DH_SOUND_DETECT_ALARM_EX = 0x2105,
        /// <summary>
        /// 硬盘满报警
        /// </summary>    
        DH_DISKFULL_ALARM_EX = 0x2106,
        /// <summary>
        /// 坏硬盘报警
        /// </summary>    
        DH_DISKERROR_ALARM_EX = 0x2107
    }

    /// <summary>
    /// 表示字符编码
    /// </summary>
    public enum LANGUAGE_ENCODING
    {
        /// <summary>
        /// IBM EBCDIC（美国 - 加拿大） 
        /// </summary>
        IBM037 = 37,
        /// <summary>
        /// OEM 美国 
        /// </summary>
        IBM437 = 437,
        /// <summary>
        /// IBM EBCDIC（国际） 
        /// </summary>
        IBM500 = 500,
        /// <summary>
        /// 阿拉伯字符 (ASMO 708) 
        /// </summary>
        ASMO_708 = 708,
        /// <summary>
        /// 阿拉伯字符 (DOS) 
        /// </summary>
        DOS_720 = 720,
        /// <summary>
        /// 希腊字符 (DOS) 
        /// </summary>
        ibm737 = 737,
        /// <summary>
        /// 波罗的海字符 (DOS) 
        /// </summary>
        ibm775 = 775,
        /// <summary>
        /// 西欧字符 (DOS) 
        /// </summary>
        ibm850 = 850,

        /// <summary>
        /// 中欧字符 (DOS) 
        /// </summary>
        ibm852 = 852,
        /// <summary>
        /// OEM 西里尔语 
        /// </summary>
        IBM855 = 855,
        /// <summary>
        /// 土耳其字符 (DOS) 
        /// </summary>
        ibm857 = 857,
        /// <summary>
        /// OEM 多语言拉丁语 I 
        /// </summary>
        IBM00858 = 858,
        /// <summary>
        /// 葡萄牙语 (DOS) 
        /// </summary>
        IBM860 = 860,
        /// <summary>
        /// 冰岛语 (DOS) 
        /// </summary>
        ibm861 = 861,
        /// <summary>
        /// 希伯来字符 (DOS) 
        /// </summary>
        DOS_862 = 862,

        /// <summary>
        /// 加拿大法语 (DOS) 
        /// </summary>
        IBM863 = 863,
        /// <summary>
        /// 阿拉伯字符 (864) 
        /// </summary>
        IBM864 = 864,
        /// <summary>
        /// 北欧字符 (DOS) 
        /// </summary>
        IBM865 = 865,
        /// <summary>
        /// 西里尔字符 (DOS) 
        /// </summary>
        cp866 = 866,
        /// <summary>
        /// 现代希腊字符 (DOS) 
        /// </summary>
        ibm869 = 869,
        /// <summary>
        /// IBM EBCDIC（多语言拉丁语 2） 
        /// </summary>
        IBM870 = 870,
        /// <summary>
        /// 泰语 (Windows) 
        /// </summary>
        windows_874 = 874,
        /// <summary>
        /// IBM EBCDIC（现代希腊语） 
        /// </summary>
        cp875 = 875,
        /// <summary>
        /// 日语 (Shift-JIS)
        /// </summary>
        shift_jis = 932,
        /// <summary>
        /// 简体中文 (GB2312) 
        /// </summary>
        gb2312 = 936,
        /// <summary>
        /// 朝鲜语 
        /// </summary>
        ks_c_5601_1987 = 949,
        /// <summary>
        /// 繁体中文 (Big5) 
        /// </summary>
        big5 = 950,
        /// <summary>
        /// IBM EBCDIC（土耳其拉丁语 5） 
        /// </summary>
        IBM1026 = 1026,
        /// <summary>
        /// IBM 拉丁语 1 
        /// </summary>
        IBM01047 = 1047,
        /// <summary>
        /// IBM EBCDIC（美国 - 加拿大 - 欧洲） 
        /// </summary>
        IBM01140 = 1140,
        /// <summary>
        /// IBM EBCDIC（德国 - 欧洲） 
        /// </summary>
        IBM01141 = 1141,
        /// <summary>
        /// IBM EBCDIC（丹麦 - 挪威 - 欧洲） 
        /// </summary>
        IBM01142 = 1142,
        /// <summary>
        /// IBM EBCDIC（芬兰 - 瑞典 - 欧洲） 
        /// </summary>
        IBM01143 = 1143,
        /// <summary>
        /// IBM EBCDIC（意大利 - 欧洲） 
        /// </summary>
        IBM01144 = 1144,
        /// <summary>
        /// IBM EBCDIC（西班牙 - 欧洲） 
        /// </summary>
        IBM01145 = 1145,
        /// <summary>
        /// IBM EBCDIC（英国 - 欧洲） 
        /// </summary>
        IBM01146 = 1146,
        /// <summary>
        /// IBM EBCDIC（法国 - 欧洲） 
        /// </summary>
        IBM01147 = 1147,
        /// <summary>
        /// IBM EBCDIC（国际 - 欧洲） 
        /// </summary>
        IBM01148 = 1148,
        /// <summary>
        /// IBM EBCDIC（冰岛语 - 欧洲） 
        /// </summary>
        IBM01149 = 1149,
        /// <summary>
        /// Unicode 
        /// </summary>
        utf_16 = 1200,
        /// <summary>
        /// Unicode (Big-Endian) 
        /// </summary>
        UnicodeFFFE = 1201,
        /// <summary>
        /// 中欧字符 (Windows) 
        /// </summary>
        windows_1250 = 1250,
        /// <summary>
        /// 西里尔字符 (Windows)
        /// </summary>
        windows_1251 = 1251,
        /// <summary>
        /// 西欧字符 (Windows) 
        /// </summary>
        Windows_1252 = 1252,
        /// <summary>
        /// 希腊字符 (Windows) 
        /// </summary>
        windows_1253 = 1253,
        /// <summary>
        /// 土耳其字符 (Windows) 
        /// </summary>
        windows_1254 = 1254,
        /// <summary>
        /// 希伯来字符 (Windows) 
        /// </summary>
        windows_1255 = 1255,
        /// <summary>
        /// 阿拉伯字符 (Windows) 
        /// </summary>
        windows_1256 = 1256,
        /// <summary>
        /// 波罗的海字符 (Windows) 
        /// </summary>
        windows_1257 = 1257,
        /// <summary>
        /// 越南字符 (Windows) 
        /// </summary>
        windows_1258 = 1258,
        /// <summary>
        /// 朝鲜语 (Johab) 
        /// </summary>
        Johab = 1361,
        /// <summary>
        /// 西欧字符 (Mac) 
        /// </summary>
        macintosh = 10000,
        /// <summary>
        /// 日语 (Mac) 
        /// </summary>
        x_mac_japanese = 10001,
        /// <summary>
        /// 繁体中文 (Mac) 
        /// </summary>
        x_mac_chinesetrad = 10002,
        /// <summary>
        /// 朝鲜语 (Mac) 
        /// </summary>
        x_mac_korean = 10003,
        /// <summary>
        /// 阿拉伯字符 (Mac) 
        /// </summary>
        x_mac_arabic = 10004,
        /// <summary>
        /// 希伯来字符 (Mac) 
        /// </summary>
        x_mac_hebrew = 10005,
        /// <summary>
        /// 希腊字符 (Mac) 
        /// </summary>
        x_mac_greek = 10006,
        /// <summary>
        /// 西里尔字符 (Mac) 
        /// </summary>
        x_mac_cyrillic = 10007,
        /// <summary>
        /// 简体中文 (Mac) 
        /// </summary>
        x_mac_chinesesimp = 10008,
        /// <summary>
        /// 罗马尼亚语 (Mac) 
        /// </summary>
        x_mac_romanian = 10010,
        /// <summary>
        /// 乌克兰语 (Mac)
        /// </summary>
        x_mac_ukrainian = 10017,
        /// <summary>
        /// 泰语 (Mac) 
        /// </summary>
        x_mac_thai = 10021,
        /// <summary>
        /// 中欧字符 (Mac) 
        /// </summary>
        x_mac_ce = 10029,
        /// <summary>
        /// 冰岛语 (Mac) 
        /// </summary>
        x_mac_icelandic = 10079,
        /// <summary>
        /// 土耳其字符 (Mac) 
        /// </summary>
        x_mac_turkish = 10081,
        /// <summary>
        /// 克罗地亚语 (Mac) 
        /// </summary>
        x_mac_croatian = 10082,
        /// <summary>
        /// 繁体中文 (CNS) 
        /// </summary>
        x_Chinese_CNS = 20000,
        /// <summary>
        /// TCA 台湾 
        /// </summary>
        x_cp20001 = 20001,
        /// <summary>
        /// 繁体中文 (Eten) 
        /// </summary>
        x_Chinese_Eten = 20002,
        /// <summary>
        /// IBM5550 台湾 
        /// </summary>
        x_cp20003 = 20003,
        /// <summary>
        /// TeleText 台湾 
        /// </summary>
        x_cp20004 = 20004,
        /// <summary>
        /// Wang 台湾 
        /// </summary>
        x_cp20005 = 20005,
        /// <summary>
        /// 西欧字符 (IA5)
        /// </summary>
        x_IA5 = 20105,
        /// <summary>
        /// 德语 (IA5) 
        /// </summary>
        x_IA5_German = 20106,
        /// <summary>
        /// 瑞典语 (IA5) 
        /// </summary>
        x_IA5_Swedish = 20107,
        /// <summary>
        /// 挪威语 (IA5) 
        /// </summary>
        x_IA5_Norwegian = 20108,
        /// <summary>
        /// US-ASCII 
        /// </summary>
        us_ascii = 20127,
        /// <summary>
        /// T.61 
        /// </summary>
        x_cp20261 = 20261,
        /// <summary>
        /// ISO-6937 
        /// </summary>
        x_cp20269 = 20269,
        /// <summary>
        /// IBM EBCDIC（德国） 
        /// </summary>
        IBM273 = 20273,
        /// <summary>
        /// IBM EBCDIC（丹麦 - 挪威） 
        /// </summary>
        IBM277 = 20277,
        /// <summary>
        ///  IBM EBCDIC（芬兰 - 瑞典） 
        /// </summary>
        IBM278 = 20278,
        /// <summary>
        /// IBM EBCDIC（意大利） 
        /// </summary>
        IBM280 = 20280,
        /// <summary>
        /// IBM EBCDIC（西班牙） 
        /// </summary>
        IBM284 = 20284,
        /// <summary>
        /// IBM EBCDIC（英国） 
        /// </summary>
        IBM285 = 20285,
        /// <summary>
        /// IBM EBCDIC（日语片假名） 
        /// </summary>
        IBM290 = 20290,
        /// <summary>
        /// IBM EBCDIC（法国） 
        /// </summary>
        IBM297 = 20297,
        /// <summary>
        /// IBM EBCDIC（阿拉伯语） 
        /// </summary>
        IBM420 = 20420,
        /// <summary>
        ///  IBM EBCDIC（希腊语） 
        /// </summary>
        IBM423 = 20423,
        /// <summary>
        /// IBM EBCDIC（希伯来语） 
        /// </summary>
        IBM424 = 20424,
        /// <summary>
        /// IBM EBCDIC（朝鲜语扩展） 
        /// </summary>
        x_EBCDIC_KoreanExtended = 20833,
        /// <summary>
        /// IBM EBCDIC（泰语） 
        /// </summary>
        IBM_Thai = 20838,
        /// <summary>
        /// 西里尔字符 (KOI8-R) 
        /// </summary>
        koi8_r = 20866,
        /// <summary>
        /// IBM EBCDIC（冰岛语） 
        /// </summary>
        IBM871 = 20871,
        /// <summary>
        /// IBM EBCDIC（西里尔俄语） 
        /// </summary>
        IBM880 = 20880,
        /// <summary>
        /// IBM EBCDIC（土耳其语） 
        /// </summary>
        IBM905 = 20905,
        /// <summary>
        /// IBM 拉丁语 1 
        /// </summary>
        IBM00924 = 20924,
        /// <summary>
        /// 日语（JIS 0208-1990 和 0212-1990） 
        /// </summary>
        EUC_JP = 20932,
        /// <summary>
        /// 简体中文 (GB2312-80) 
        /// </summary>
        x_cp20936 = 20936,
        /// <summary>
        /// 朝鲜语 Wansung 
        /// </summary>
        x_cp20949 = 20949,
        /// <summary>
        /// IBM EBCDIC（西里尔塞尔维亚 - 保加利亚语） 
        /// </summary>
        cp1025 = 21025,
        /// <summary>
        /// 西里尔字符 (KOI8-U) 
        /// </summary>
        koi8_u = 21866,
        /// <summary>
        /// 西欧字符 (ISO) 
        /// </summary>
        iso_8859_1 = 28591,
        /// <summary>
        /// 中欧字符 (ISO) 
        /// </summary>
        iso_8859_2 = 28592,
        /// <summary>
        /// 拉丁语 3 (ISO) 
        /// </summary>
        iso_8859_3 = 28593,
        /// <summary>
        /// 波罗的海字符 (ISO) 
        /// </summary>
        iso_8859_4 = 28594,
        /// <summary>
        /// 西里尔字符 (ISO) 
        /// </summary>
        iso_8859_5 = 28595,
        /// <summary>
        /// 阿拉伯字符 (ISO) 
        /// </summary>
        iso_8859_6 = 28596,
        /// <summary>
        /// 希腊字符 (ISO) 
        /// </summary>
        iso_8859_7 = 28597,
        /// <summary>
        /// 希伯来字符 (ISO-Visual) 
        /// </summary>
        iso_8859_8 = 28598,
        /// <summary>
        /// 土耳其字符 (ISO) 
        /// </summary>
        iso_8859_9 = 28599,
        /// <summary>
        /// 爱沙尼亚语 (ISO) 
        /// </summary>
        iso_8859_13 = 28603,
        /// <summary>
        /// 拉丁语 9 (ISO) 
        /// </summary>
        iso_8859_15 = 28605,
        /// <summary>
        /// 欧罗巴 
        /// </summary>
        x_Europa = 29001,
        /// <summary>
        /// 希伯来字符 (ISO-Logical) 
        /// </summary>
        iso_8859_8_i = 38598,
        /// <summary>
        /// 日语 (JIS) 
        /// </summary>
        iso_jp_JIS = 50220,
        /// <summary>
        /// 日语（JIS- 允许 1 字节假名） 
        /// </summary>
        csISO2022JP = 50221,
        /// <summary>
        /// 日语（JIS- 允许 1 字节假名 - SO/SI） 
        /// </summary>
        iso_2022_jp = 50222,
        /// <summary>
        /// 朝鲜语 (ISO) 
        /// </summary>
        iso_2022_kr = 50225,
        /// <summary>
        /// 简体中文 (ISO-2022) 
        /// </summary>
        x_cp50227 = 50227,
        /// <summary>
        /// 日语 (EUC) 
        /// </summary>
        euc_jp = 51932,
        /// <summary>
        /// 简体中文 (EUC) 
        /// </summary>
        EUC_CN = 51936,
        /// <summary>
        /// 朝鲜语 (EUC) 
        /// </summary>
        euc_kr = 51949,
        /// <summary>
        /// 简体中文 (HZ) 
        /// </summary>
        hz_gb_2312 = 52936,
        /// <summary>
        /// 简体中文 (GB18030) 
        /// </summary>
        GB18030 = 54936,
        /// <summary>
        /// ISCII 梵文 
        /// </summary>
        x_iscii_de = 57002,
        /// <summary>
        /// ISCII 孟加拉语 
        /// </summary>
        x_iscii_be = 57003,
        /// <summary>
        /// ISCII 泰米尔语 
        /// </summary>
        x_iscii_ta = 57004,
        /// <summary>
        /// ISCII 泰卢固语 
        /// </summary>
        x_iscii_te = 57005,
        /// <summary>
        /// ISCII 阿萨姆语 
        /// </summary>
        x_iscii_as = 57006,
        /// <summary>
        /// ISCII 奥里雅语 
        /// </summary>
        x_iscii_or = 57007,
        /// <summary>
        /// ISCII 卡纳达语 
        /// </summary>
        x_iscii_ka = 57008,
        /// <summary>
        /// ISCII 马拉雅拉姆语 
        /// </summary>
        x_iscii_ma = 57009,
        /// <summary>
        /// ISCII 古吉拉特语 
        /// </summary>
        x_iscii_gu = 57010,
        /// <summary>
        /// ISCII 旁遮普语 
        /// </summary>
        x_iscii_pa = 57011,
        /// <summary>
        /// Unicode (UTF-7) 
        /// </summary>
        utf_7 = 65000,
        /// <summary>
        /// Unicode (UTF-8) 
        /// </summary>
        utf_8 = 65001,
        /// <summary>
        /// Unicode (UTF-32) 
        /// </summary>
        utf_32 = 65005,
        /// <summary>
        /// Unicode (UTF-32 Big-Endian) 
        /// </summary>
        utf_32BE = 65006
    }

    /// <summary>
    /// media文件查询条件
    /// </summary>
    public enum EM_FILE_QUERY_TYPE
    {
        /// <summary>
        /// 交通车辆信息
        /// </summary>
        DH_FILE_QUERY_TRAFFICCAR,
        /// <summary>
        /// ATM信息
        /// </summary>
        DH_FILE_QUERY_ATM,
        /// <summary>
        /// ATM交易信息
        /// </summary>
        DH_FILE_QUERY_ATMTXN,
    }

    /// <summary>
    /// 控制类型，对应CLIENT_ControlDevice接口
    /// </summary>
    public enum CtrlType
    {
        /// <summary>
        /// 重启设备	
        /// </summary>
        DH_CTRL_REBOOT = 0,
        /// <summary>
        /// 关闭设备
        /// </summary>
        DH_CTRL_SHUTDOWN,
        /// <summary>
        /// 硬盘管理
        /// </summary>
        DH_CTRL_DISK,
        /// <summary>
        /// 网络键盘
        /// </summary>
        DH_KEYBOARD_POWER = 3,
        DH_KEYBOARD_ENTER,
        DH_KEYBOARD_ESC,
        DH_KEYBOARD_UP,
        DH_KEYBOARD_DOWN,
        DH_KEYBOARD_LEFT,
        DH_KEYBOARD_RIGHT,
        DH_KEYBOARD_BTN0,
        DH_KEYBOARD_BTN1,
        DH_KEYBOARD_BTN2,
        DH_KEYBOARD_BTN3,
        DH_KEYBOARD_BTN4,
        DH_KEYBOARD_BTN5,
        DH_KEYBOARD_BTN6,
        DH_KEYBOARD_BTN7,
        DH_KEYBOARD_BTN8,
        DH_KEYBOARD_BTN9,
        DH_KEYBOARD_BTN10,
        DH_KEYBOARD_BTN11,
        DH_KEYBOARD_BTN12,
        DH_KEYBOARD_BTN13,
        DH_KEYBOARD_BTN14,
        DH_KEYBOARD_BTN15,
        DH_KEYBOARD_BTN16,
        DH_KEYBOARD_SPLIT,
        DH_KEYBOARD_ONE,
        DH_KEYBOARD_NINE,
        DH_KEYBOARD_ADDR,
        DH_KEYBOARD_INFO,
        DH_KEYBOARD_REC,
        DH_KEYBOARD_FN1,
        DH_KEYBOARD_FN2,
        DH_KEYBOARD_PLAY,
        DH_KEYBOARD_STOP,
        DH_KEYBOARD_SLOW,
        DH_KEYBOARD_FAST,
        DH_KEYBOARD_PREW,
        DH_KEYBOARD_NEXT,
        DH_KEYBOARD_JMPDOWN,
        DH_KEYBOARD_JMPUP,
        /// <summary>
        /// 触发报警输入
        /// </summary>
        DH_TRIGGER_ALARM_IN = 100,
        /// <summary>
        /// 触发报警输出
        /// </summary>
        DH_TRIGGER_ALARM_OUT,
        /// <summary>
        /// 矩阵控制
        /// </summary>
        DH_CTRL_MATRIX,
        /// <summary>
        /// SD卡控制(IPC产品)参数同硬盘控制
        /// </summary>
        DH_CTRL_SDCARD,
        /// <summary>
        /// 刻录机控制，开始刻录
        /// </summary>
        DH_BURNING_START,
        /// <summary>
        /// 刻录机控制，结束刻录
        /// </summary>
        DH_BURNING_STOP,
        /// <summary>
        /// 刻录机控制，叠加密码(以'\0'为结尾的字符串，最大长度8位)
        /// </summary>
        DH_BURNING_ADDPWD,
        /// <summary>
        /// 刻录机控制，叠加片头(以'\0'为结尾的字符串，最大长度1024字节，支持分行，行分隔符'\n')
        /// </summary>
        DH_BURNING_ADDHEAD,
        /// <summary>
        /// 刻录机控制，叠加打点到刻录信息(参数无)
        /// </summary>
        DH_BURNING_ADDSIGN,
        /// <summary>
        /// 刻录机控制，自定义叠加(以'\0'为结尾的字符串，最大长度1024字节，支持分行，行分隔符'\n')
        /// </summary>
        DH_BURNING_ADDCURSTOMINFO,
        /// <summary>
        /// 恢复设备的默认设置
        /// </summary>
        DH_CTRL_RESTOREDEFAULT,
        /// <summary>
        /// 触发设备抓图
        /// </summary>
        DH_CTRL_CAPTURE_START,
        /// <summary>
        /// 清除日志
        /// </summary>
        DH_CTRL_CLEARLOG,
        /// <summary>
        /// 触发无线报警(IPC产品)
        /// </summary>
        DH_TRIGGER_ALARM_WIRELESS = 200,
        /// <summary>
        /// 标识重要录像文件
        /// </summary>
        DH_MARK_IMPORTANT_RECORD,
        /// <summary>
        /// 网络硬盘分区
        /// </summary>
        DH_CTRL_DISK_SUBAREA,
        /// <summary>
        /// 刻录机控制，附件刻录.
        /// </summary>
        DH_BURNING_ATTACH,
        /// <summary>
        /// 刻录暂停
        /// </summary>
        DH_BURNING_PAUSE,
        /// <summary>
        /// 刻录继续
        /// </summary>
        DH_BURNING_CONTINUE,
        /// <summary>
        /// 刻录顺延
        /// </summary>
        DH_BURNING_POSTPONE,
        /// <summary>
        /// 报停控制
        /// </summary>
        DH_CTRL_OEMCTRL,
        /// <summary>
        /// 设备备份开始
        /// </summary>
        DH_BACKUP_START,
        /// <summary>
        /// 设备备份停止
        /// </summary>
        DH_BACKUP_STOP,
        /// <summary>
        /// 车载手动增加WIFI配置
        /// </summary>
        DH_VIHICLE_WIFI_ADD,
        /// <summary>
        /// 车载手动删除WIFI配置
        /// </summary>
        DH_VIHICLE_WIFI_DEC,
        /// <summary>
        /// 蜂鸣器控制开始
        /// </summary>
        DH_BUZZER_START,
        /// <summary>
        /// 蜂鸣器控制结束
        /// </summary>
        DH_BUZZER_STOP,
        /// <summary>
        /// 剔除用户
        /// </summary>
        DH_REJECT_USER,
        /// <summary>
        /// 屏蔽用户
        /// </summary>
        DH_SHIELD_USER,
        /// <summary>
        /// 智能交通, 雨刷控制 
        /// </summary>
        DH_RAINBRUSH,
        /// <summary>
        /// 智能交通, 手动抓拍 (对应结构体MANUAL_SNAP_PARAMETER)
        /// </summary>
        DH_MANUAL_SNAP,
        /// <summary>
        /// 手动NTP校时
        /// </summary>
        DH_MANUAL_NTP_TIMEADJUST,
        /// <summary>
        /// 导航信息和短消息
        /// </summary>
        DH_NAVIGATION_SMS,
        /// <summary>
        /// 路线点位信息
        /// </summary>
        DH_CTRL_ROUTE_CROSSING,
        /// <summary>
        /// 格式化备份设备
        /// </summary>
        DH_BACKUP_FORMAT,
        /// <summary>
        /// 控制设备端本地预览分割(对应结构体DEVICE_LOCALPREVIEW_SLIPT_PARAMETER)    
        /// </summary>
        DH_DEVICE_LOCALPREVIEW_SLIPT,
        /// <summary>
        /// RAID初始化
        /// </summary>
        DH_CTRL_INIT_RAID,
        /// <summary>
        /// RAID操作
        /// </summary>
        DH_CTRL_RAID,
        /// <summary>
        /// 热备盘操作
        /// </summary>
        DH_CTRL_SAPREDISK,
        /// <summary>
        /// 手动发起WIFI连接(对应结构体WIFI_CONNECT)
        /// </summary>
        DH_WIFI_CONNECT,
        /// <summary>
        /// 手动断开WIFI连接(对应结构体WIFI_CONNECT)
        /// </summary>
        DH_WIFI_DISCONNECT,
    }


    public enum EM_STAFF_TYPE
    {
        EM_STAFF_TYPE_ERR,
        /// <summary>
        /// "Horizontal" 水平线段
        /// </summary>
        EM_STAFF_TYPE_HORIZONTAL,
        /// <summary>
        /// "Vertical" 垂直线段
        /// </summary>
        EM_STAFF_TYPE_VERTICAL,
        /// <summary>
        /// "Any" 任意线段
        /// </summary>
        EM_STAFF_TYPE_ANY,
        /// <summary>
        /// "Cross" 垂直面交线段
        /// </summary>
        EM_STAFF_TYPE_CROSS,
    }

    public enum EM_CALIBRATEAREA_TYPE
    {
	    EM_CALIBRATEAREA_TYPE_ERR,
        /// <summary>
        /// "Groud" : 地面，需要N条竖直线段+M条水平线段（（N=3，M=1）；（N=2，M=2）；今后扩展）。
        /// </summary>
		EM_CALIBRATEAREA_TYPE_GROUD, 
        /// <summary>
        /// "Horizontal"  : 水平面，需要水平面上一点到地面点的投影垂直线段。		
        /// </summary>
		EM_CALIBRATEAREA_TYPE_HORIZONTAL,
        /// <summary>
        /// "Vertical" : 垂直面，需要垂直面与地面的交线。
        /// </summary>
		EM_CALIBRATEAREA_TYPE_VERTICAL,
        /// <summary>
        /// "Any" 任意平面，N条竖直线段，及每条长度（N=3，及今后扩展）。
        /// </summary>
		EM_CALIBRATEAREA_TYPE_ANY,
    };

    /// <summary>
    /// 视频压缩格式
    /// </summary>
    public enum CFG_VIDEO_COMPRESSION
    {
        /// <summary>
        /// MPEG4
        /// </summary>
	    VIDEO_FORMAT_MPEG4,
        /// <summary>
        /// MS-MPEG4
        /// </summary>
	    VIDEO_FORMAT_MS_MPEG4,
        /// <summary>
        /// MPEG2
        /// </summary>
	    VIDEO_FORMAT_MPEG2,
        /// <summary>
        /// MPEG1
        /// </summary>
	    VIDEO_FORMAT_MPEG1,
        /// <summary>
        /// H.263
        /// </summary>
	    VIDEO_FORMAT_H263,
        /// <summary>
        /// MJPG
        /// </summary>
	    VIDEO_FORMAT_MJPG,
        /// <summary>
        /// FCC-MPEG4
        /// </summary>
	    VIDEO_FORMAT_FCC_MPEG4,
        /// <summary>
        /// H.264
        /// </summary>
	    VIDEO_FORMAT_H264,
    } ;

    /// <summary>
    /// 码流控制模式
    /// </summary>
    public enum CFG_BITRATE_CONTROL
    {
        /// <summary>
        /// 固定码流
        /// </summary>
	    BITRATE_CBR, 
        /// <summary>
        /// 可变码流
        /// </summary>
	    BITRATE_VBR, 
    }

    /// <summary>
    /// 画质
    /// </summary>
    public enum CFG_IMAGE_QUALITY
    {
        /// <summary>
        /// 图像质量10%
        /// </summary>
	    IMAGE_QUALITY_Q10 = 1,
        /// <summary>
        /// 图像质量30%
        /// </summary>
	    IMAGE_QUALITY_Q30,
        /// <summary>
        /// 图像质量50%
        /// </summary>
	    IMAGE_QUALITY_Q50,
        /// <summary>
        /// 图像质量60%
        /// </summary>
	    IMAGE_QUALITY_Q60,
        /// <summary>
        /// 图像质量80%
        /// </summary>
	    IMAGE_QUALITY_Q80,
        /// <summary>
        /// 图像质量100%
        /// </summary>
	    IMAGE_QUALITY_Q100,
    }

    /// <summary>
    /// 音频编码模式
    /// </summary>
    public enum CFG_AUDIO_FORMAT
    {
        /// <summary>
        /// G711a
        /// </summary>
	    AUDIO_FORMAT_G711A,
        /// <summary>
        /// PCM
        /// </summary>
        AUDIO_FORMAT_PCM,
        /// <summary>
        /// G711u
        /// </summary>
        AUDIO_FORMAT_G711U,
        /// <summary>
        /// AMR
        /// </summary>
        AUDIO_FORMAT_AMR,
        /// <summary>
        /// AAC
        /// </summary>
        AUDIO_FORMAT_AAC,
    }

    /// <summary>
    /// 云台联动类型
    /// </summary>
    public enum CFG_LINK_TYPE
    {
        /// <summary>
        /// 无联动
        /// </summary>
        LINK_TYPE_NONE,
        /// <summary>
        /// 联动预置点
        /// </summary>
        LINK_TYPE_PRESET,
        /// <summary>
        /// 联动巡航
        /// </summary>
        LINK_TYPE_TOUR,
        /// <summary>
        /// 联动轨迹
        /// </summary>
        LINK_TYPE_PATTERN,
    } 


    // 分辨率枚举
    public enum CFG_CAPTURE_SIZE
    {
        /// <summary>
        /// 704*576(PAL)  704*480(NTSC)
        /// </summary>
	    IMAGE_SIZE_D1,
        /// <summary>
        /// 352*576(PAL)  352*480(NTSC)
        /// </summary>
	    IMAGE_SIZE_HD1,
        /// <summary>
        /// 704*288(PAL)  704*240(NTSC)
        /// </summary>
	    IMAGE_SIZE_BCIF,
        /// <summary>
        /// 352*288(PAL)  352*240(NTSC)
        /// </summary>
	    IMAGE_SIZE_CIF,
        /// <summary>
        /// 176*144(PAL)  176*120(NTSC)
        /// </summary>
	    IMAGE_SIZE_QCIF,
        /// <summary>
        /// 640*480
        /// </summary>
	    IMAGE_SIZE_VGA,
        /// <summary>
        /// 320*240
        /// </summary>
	    IMAGE_SIZE_QVGA,
        /// <summary>
        /// 480*480
        /// </summary>
	    IMAGE_SIZE_SVCD,
        /// <summary>
        /// 160*128
        /// </summary>
	    IMAGE_SIZE_QQVGA,
        /// <summary>
        /// 800*592
        /// </summary>
	    IMAGE_SIZE_SVGA,
        /// <summary>
        /// 1024*768
        /// </summary>
	    IMAGE_SIZE_XVGA,
        /// <summary>
        /// 1280*800
        /// </summary>
	    IMAGE_SIZE_WXGA,
        /// <summary>
        /// 1280*1024  
        /// </summary>
	    IMAGE_SIZE_SXGA,
        /// <summary>
        /// 1600*1024  
        /// </summary>
	    IMAGE_SIZE_WSXGA,
        /// <summary>
        /// 1600*1200
        /// </summary>
	    IMAGE_SIZE_UXGA,
        /// <summary>
        /// 1920*1200
        /// </summary>
	    IMAGE_SIZE_WUXGA,
        /// <summary>
        /// 240*192
        /// </summary>
	    IMAGE_SIZE_LTF,
        /// <summary>
        /// 1280*720
        /// </summary>
	    IMAGE_SIZE_720,
        /// <summary>
        /// 1920*1080
        /// </summary>
	    IMAGE_SIZE_1080,
        /// <summary>
        /// 1280*960
        /// </summary>
	    IMAGE_SIZE_1_3M,
	    IMAGE_SIZE_NR  
    }

    // 抓拍模式
    public enum CFG_TRAFFIC_SNAP_MODE
    {
        /// <summary>
        /// 自动抓拍
        /// </summary>
        TRAFFIC_SNAP_MODE_AUTO = 0,
        /// <summary>
        /// 线圈抓拍
        /// </summary>
        TRAFFIC_SNAP_MODE_COIL,
        /// <summary>
        /// 线圈抓拍, 图片分析
        /// </summary>
        TRAFFIC_SNAP_MODE_COIL_PICANALYSIS,
        /// <summary>
        /// 视频抓拍
        /// </summary>
        TRAFFIC_SNAP_MODE_STREAM,
        /// <summary>
        /// 视频抓拍, 并且识别
        /// </summary>
        TRAFFIC_SNAP_MODE_STREAM_IDENTIFY,
    }

    // 语言类型
    public enum AV_CFG_LanguageType
    {
        /// <summary>
        /// 英文
        /// </summary>
        AV_CFG_Language_English,
        /// <summary>
        /// 简体中文
        /// </summary>
        AV_CFG_Language_SimpChinese,
        /// <summary>
        /// 繁体中文
        /// </summary>
        AV_CFg_Language_TradChinese,
        /// <summary>
        /// 意大利文
        /// </summary>
        AV_CFG_Language_Italian,
        /// <summary>
        /// 西班牙文
        /// </summary>
        AV_CFG_Language_Spanish,
        /// <summary>
        /// 日文
        /// </summary>
        AV_CFG_Language_Janpanese,
        /// <summary>
        /// 俄文
        /// </summary>
        AV_CFG_Language_Russian,
        /// <summary>
        /// 法文
        /// </summary>
        AV_CFG_Language_French,
        /// <summary>
        /// 德文
        /// </summary>
        AV_CFG_Language_German,
    };

    // 云台联动类型
    public enum AV_CFG_PtzLinkType
    {
        /// <summary>
        /// 无联动
        /// </summary>
        AV_CFG_PtzLink_None,
        /// <summary>
        /// 联动预置点
        /// </summary>
        AV_CFG_PtzLink_Preset,
        /// <summary>
        /// 联动巡航
        /// </summary>
        AV_CFG_PtzLink_Tour,
        /// <summary>
        /// 联动轨迹
        /// </summary>
        AV_CFG_PtzLink_Pattern,
    }

    // 分割模式
    public enum AV_CFG_SplitMode
    {
        /// <summary>
        /// 1画面
        /// </summary>
        AV_CFG_Split1 = 1,
        /// <summary>
        /// 2画面
        /// </summary>
        AV_CFG_Split2 = 2,
        /// <summary>
        /// 4画面
        /// </summary>
        AV_CFG_Split4 = 4,
        /// <summary>
        /// 6画面
        /// </summary>
        AV_CFG_Split6 = 6,
        /// <summary>
        /// 8画面
        /// </summary>
        AV_CFG_Split8 = 8,
        /// <summary>
        /// 9画面
        /// </summary>
        AV_CFG_Split9 = 9,
        /// <summary>
        /// 12画面
        /// </summary>
        AV_CFG_Split12 = 12,
        /// <summary>
        /// 16画面
        /// </summary>
        AV_CFG_Split16 = 16,
        /// <summary>
        /// 20画面
        /// </summary>
        AV_CFG_Split20 = 20,
        /// <summary>
        /// 25画面
        /// </summary>
        AV_CFG_Split25 = 25,
        /// <summary>
        /// 36画面
        /// </summary>
        AV_CFG_Split36 = 36,
        /// <summary>
        ///  画中画模式, 1个全屏大画面+1个小画面窗口
        /// </summary>
        AV_CFG_PIP1 = 100 + 1,
        /// <summary>
        /// 画中画模式, 1个全屏大画面+3个小画面窗口
        /// </summary>
        AV_CFG_PIP3 = 100 + 3,	
    }

    ///// 视频诊断任务
    public enum CFG_EM_STREAM_TYPE
    {
        /// <summary>
        /// 其它
        /// </summary>
        CFG_EM_STREAM_ERR,
        /// <summary>
        /// "Main"-主码流
        /// </summary>
        CFG_EM_STREAM_MAIN,
        /// <summary>
        /// "Extra1"-辅码流1
        /// </summary>
        CFG_EM_STREAM_EXTRA_1,
        /// <summary>
        /// "Extra2"-辅码流2
        /// </summary>
        CFG_EM_STREAM_EXTRA_2,
        /// <summary>
        /// "Extra3"-辅码流3
        /// </summary>
        CFG_EM_STREAM_EXTRA_3,
        /// <summary>
        /// "Snapshot"-抓图码流
        /// </summary>
        CFG_EM_STREAM_SNAPSHOT,
        /// <summary>
        /// "Object"-物体流
        /// </summary>
        CFG_EM_STREAM_OBJECT,
    };

    public enum DH_SYS_ABILITY
    {
	    ABILITY_WATERMARK_CFG = 17,			// Watermark configuration capacity
	    ABILITY_WIRELESS_CFG = 18,			// wireless  confuguration capacity
	    ABILITY_DEVALL_INFO = 26,			// Device capacity list 
	    ABILITY_CARD_QUERY = 0x0100,		// Card number search capacity 
	    ABILITY_MULTIPLAY = 0x0101,			// Multiple-window preview capacity 
	    ABILITY_QUICK_QUERY_CFG = 0x0102,	// Fast query configuration Capabilities
	    ABILITY_INFRARED = 0x0121,			// Wireless alarm capacity 
	    ABILITY_TRIGGER_MODE = 0x0131,		// Alarm activation mode function 
	    ABILITY_DISK_SUBAREA = 0x0141,		// Network hard disk partition
	    ABILITY_DSP_CFG = 0x0151,			// Query DSP Capabilities
	    ABILITY_STREAM_MEDIA = 0x0161,		// Query SIP,RTSP Capabilities
    } 

    public enum DH_FUN_SUPPORT
    {
	    EN_FTP = 0,							// FTP bitwise，1：send out record file;  2：Send out snapshot file
	    EN_SMTP,							// SMTP bitwise，1：alarm send out text mail 2：Alarm send out image
	    EN_NTP,								// NTP	 Bitwise：1：Adjust system time 
	    EN_AUTO_MAINTAIN,					// Auto maintenance  Bitwise：1：reboot 2：close  3:delete file
	    EN_VIDEO_COVER,						// Privacy mask Bitwise  ：1：multiple-window privacy mask 
	    EN_AUTO_REGISTER,					// Auto registration	Bitwise：1:SDK auto log in after registration 
	    EN_DHCP,							// DHCP	Bitwise：1：DHCP
	    EN_UPNP,							// UPNP	Bitwise ：1：UPNP
	    EN_COMM_SNIFFER,					// COM sniffer  Bitwise ：1:CommATM
	    EN_NET_SNIFFER,						// Network sniffer Bitwise ： 1：NetSniffer
	    EN_BURN,							// Burn function Bitwise 1：Search burn status 
	    EN_VIDEO_MATRIX,					// Video matrix Bitwise  1：Support video matrix or not
	    EN_AUDIO_DETECT,					// Video detection Bitwise ：1：Support video detection or not 
	    EN_STORAGE_STATION,					// Storage position Bitwise：1：Ftp server (Ips) 2：SBM 3：NFS 16：DISK 17：Flash disk 
	    EN_IPSSEARCH,						// IPS storage search  Bitwise  1：IPS storage search 	
	    EN_SNAP,							// Snapshot Bitwise  1：Resoluiton 2：Frame rate 3：Snapshoot  4：Snapshoot file image; 5：Image quality 
	    EN_DEFAULTNIC,						// Search default network card search  Bitwise  1：Support
	    EN_SHOWQUALITY,						// Image quality configuration time in CBR mode 1:support 
	    EN_CONFIG_IMEXPORT,					// Configuration import& emport function capacity.  Bitwise   1:support 
	    EN_LOG,								// Support search log page by page or not. Bitwise 1：support 
	    EN_SCHEDULE,						// Record setup capacity. Bitwise  1:Redandunce  2:Pre-record 3:Record period
	    EN_NETWORK_TYPE,					// Network type. Bitwise 1:Wire Network 2:Wireless Network 3:CDMA/GPRS
	    EN_MARK_IMPORTANTRECORD,			// Important record. Bitwise 1：Important record mark
	    EN_ACFCONTROL,						// Frame rate control activities. Bitwise 1：support frame rate control activities
	    EN_MULTIASSIOPTION,					// Multiple-channel extra stream. Bitwise：1：support three channel extra stream
	    EN_DAVINCIMODULE,					// Component modules bitwise: 1.Separate processing the schedule 2.Standard I franme Interval setting
	    EN_GPS,                             // GPS function bitwise:1:Gps locate function	
	    EN_MULTIETHERNET,					// Support multi net card query   bitwise: 1: support
	    EN_LOGIN_ATTRIBUTE,                 // Login properties   bitwise: 1: support query login properties  
	    EN_RECORD_GENERAL,					// Recording associated  bitwise: 1:Normal recording; 2:Alarm recording; 
										    // 3:Motion detection recording;  4:Local storage; 5: Network storage ;  
										    // 6:Redundancy storage;  7:Local emergency storage
	    EN_JSON_CONFIG,						// Whether support Json configuration, bitwise: 1: support Json
	    EN_HIDE_FUNCTION,					// Hide function:bitwise::1，hide PTZ function
    }

    // 升级类型
    public enum EM_UPGRADE_TYPE
    {
	    DH_UPGRADE_BIOS_TYPE = 1,					// BIOS升级
	    DH_UPGRADE_WEB_TYPE,						// WEB升级
	    DH_UPGRADE_BOOT_YPE,						// BOOT升级
	    DH_UPGRADE_CHARACTER_TYPE,					// 汉字库
	    DH_UPGRADE_LOGO_TYPE,						// LOGO
	    DH_UPGRADE_EXE_TYPE,						// EXE，例如播放器等
    }

    #endregion


   
}