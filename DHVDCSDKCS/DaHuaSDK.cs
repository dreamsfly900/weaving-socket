/*
 * ************************************************************************
 *                            SDK
 *                      解码卡VDC(C#版)
 * 
 * (c) Copyright 2007, ZheJiang Dahua Technology Stock Co.Ltd.
 *                      All Rights Reserved
 * 版 本 号:0.01
 * 文件名称:DaHuaSDK.cs
 * 功能说明:原始封装[在现有的VDCSDK(C++版)上再一次封装,基本与原C++接口对应]
 * 作    者:李德明
 * 作成日期:2008/01/18
 * 修改日志:    日期        版本号      作者        变更事由
 *              2007/03/10  0.01        李德明      新建作成
 * 
 * ************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DHVDCSDK
{
    public class DHVDC
    {
        #region << 公开方法 >>

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="pDeviceTotal">初始化成功的通道个数</param>
        /// <returns>错误码</returns>
        public static int DHInitDecDevice(ref int pDeviceTotal)
        {
            int returnValue = 0;
            returnValue = DH_InitDecDevice(ref pDeviceTotal);
            return returnValue;
        }

        /// <summary>
        /// 关闭设备，在程序退出时调用
        /// </summary>
        /// <returns>错误码</returns>
        public static int DHReleaseDecDevice()
        {
            int returnValue = 0;
            returnValue = DH_ReleaseDecDevice();
            return returnValue;
        }

        /// <summary>
        /// 初始化DirectDraw[有关DirectDraw部分已经由SDK内部创建，用户不需要调用该接口函数]
        /// </summary>
        /// <param name="hParent">窗口句柄</param>
        /// <param name="colorKey">用户设置的透明色</param>
        /// <returns>错误码</returns>
        public static int DHInitDirectDraw(IntPtr hParent, UInt32 colorKey)
        {
            int returnValue = 0;
            returnValue = DH_InitDirectDraw(hParent,colorKey);
            return returnValue;
        }
        /// <summary>
        /// 释放DirectDraw
        /// </summary>
        /// <returns>错误码</returns>
        public static int DHReleaseDirectDraw()
        {
            int returnValue = 0;
            returnValue = DH_ReleaseDirectDraw();
            return returnValue;
        }

        /// <summary>
        /// 打开通道，获取相关的操作句柄，与通道相关的操作必须使用该句柄
        /// </summary>
        /// <param name="nChannelNum">通道号（从0开始）</param>
        /// <param name="phChannel">操作句柄</param>
        /// <returns>错误码</returns>
        public static int DHChannelOpen(int nChannelNum, ref UInt32 phChannel)
        {
            int returnValue = 0;
            returnValue = DH_ChannelOpen(nChannelNum, ref phChannel);
            return returnValue;
        }

        /// <summary>
        /// 关闭通道，释放相关资源
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <returns>错误码</returns>
        public static int DHChannelClose(UInt32 hChannel)
        {
            int returnValue = 0;
            returnValue = DH_ChannelClose(hChannel);
            return returnValue;
        }

        /// <summary>
        /// 打开流接口（类似于打开文件）
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <param name="pFileHeadBuf">文件头数据</param>
        /// <param name="dwSize">文件头长度</param>
        /// <returns>错误码</returns>
        public static int DHOpenStream(UInt32 hChannel, IntPtr pFileHeadBuf, UInt32 dwSize)
        {
            int returnValue = 0;            
            System.Console.WriteLine("dwSize:"+dwSize.ToString());
            System.IO.FileStream fs= System.IO.File.Open(@"c:\Test.txt",System.IO.FileMode.OpenOrCreate);
            char[] buffer = new char[256];
            buffer = dwSize.ToString().ToCharArray();
            byte[] buffer1 = new byte[256];
            for (int i = 0; i < 6; i++)
            {
                buffer1[i] = (byte)buffer[i];
            }
                fs.Write(buffer1, 0, 256);        
            fs.Close();
            returnValue = DH_OpenStream(hChannel, pFileHeadBuf, dwSize);
            return returnValue;
        }

        /// <summary>
        /// 打开流接口(增强版)
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <param name="pFileHeadBuf">文件头数据</param>
        /// <param name="dwSize">文件头长度</param>
        /// <returns>错误码</returns>
        public static int DHOpenStreamEx(UInt32 hChannel, IntPtr pFileHeadBuf, UInt32 dwSize)
        {
            int returnValue = 0;
            returnValue = DH_OpenStreamEx(hChannel, pFileHeadBuf, dwSize);
            return returnValue;
        }

        /// <summary>
        /// 清空缓冲
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <returns>错误码</returns>
        public static int DHResetStream(UInt32 hChannel)
        {
            int returnValue = 0;
            returnValue = DH_ResetStream(hChannel);
            return returnValue;
        }
        /// <summary>
        /// 关闭数据流
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <returns>错误码</returns>
        public static int DHCloseStream(UInt32 hChannel)
        {
            int returnValue = 0;
            returnValue = DH_CloseStream(hChannel);
            return returnValue;
        }

         /// <summary>
        /// 关闭数据流
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <returns>错误码</returns>
        public static int DHCloseStreamEx(UInt32 hChannel)
        {
            int returnValue = 0;
            returnValue = DH_CloseStreamEx(hChannel);
            return returnValue;
        }

        /// <summary>
        /// 输入数据，打开流之后才能输入数据
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <param name="pBuf">缓冲区地址</param>
        /// <param name="dwSize">缓冲区大小</param>
        /// <returns>错误码</returns>
        public static int DHInputData(UInt32 hChannel, IntPtr pBuf, UInt32 dwSize)
        {
            int returnValue = 0;
            returnValue = DH_InputData(hChannel, pBuf, dwSize);
            return returnValue;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <param name="szFileName">文件名</param>
        /// <returns>错误码</returns>
        public static int DHOpenFile(UInt32 hChannel,string szFileName)
        {
            int returnValue = 0;
            returnValue = DH_OpenFile(hChannel,szFileName);
            return returnValue;
        }
        /// <summary>
        /// 关闭文件
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <returns>错误码</returns>
        public static int DHCloseFile(UInt32 hChannel)
        {
            int returnValue = 0;
            returnValue = DH_CloseFile(hChannel);
            return returnValue;
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns>错误码</returns>
        public static int DHPlay(UInt32 hChannel, IntPtr hwnd)
        {
            int returnValue = 0;
            returnValue = DH_Play(hChannel, hwnd);
            return returnValue;
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <param name="bPause">true:暂停;false:继续</param>
        /// <returns>错误码</returns>
        public static int DHPause(UInt32 hChannel, bool bPause)
        {
            int returnValue = 0;            
            returnValue = DH_Pause(hChannel,(uint) (bPause==true?1:0));
            return returnValue;
        }
        /// <summary>
        /// 结束播放
        /// </summary>
        /// <param name="hChannel">通道句柄</param>
        /// <returns>错误码</returns>
        public static int DHStop(UInt32 hChannel)
        {
            int returnValue = 0;
            returnValue = DH_Stop(hChannel);
            return returnValue;
        }

        #endregion

        #region << 委托 >>
        public delegate void fnDecProgressCB(int nCurrent, int nTotal, IntPtr dwUser);
        public delegate void FILE_REF_DONE_CALLBACK(uint nChannel, uint nSize);
        public delegate void IMAGE_STREAM_CALLBACK(uint nDisplayChannel, IntPtr context);
        #endregion

        #region << 枚举定义 >>

        /// <summary>
        /// 视频标准
        /// </summary>
        public enum VIDEO_STANDARD
        {
            PAL,
            NTSC,
            UNKNOW,
        }

        #endregion

        #region << 结构定义 >>
        //结构定义
        public struct tagDSPDetail
        {
            public uint encodeChannelCount;			//板卡包含的编码通道个数
            public uint firstEncodeChannelIndex;		//板卡上第一个编码通道的索引
            public uint decodeChannelCount;			//板卡包含的解码通道个数
            public uint firstDecodeChannelIndex;		//板卡上第一个解码通道的索引
            public uint displayChannelCount;			//板卡包含的视频输出通道个数
            public uint firstDisplayChannelIndex;		//板卡上第一个视频输出通道的索引
            public uint reserved1;						/*reserve*/
            public uint reserved2;						/*reserve*/
            public uint reserved3;						/*reserve*/
            public uint reserved4;						/*reserve*/
        };

        public struct DS_BOARD_DETAIL
        {
            public int /*BOARD_TYPE_DS*/ type; //板卡类型
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] sn; //序列号
            public uint dspCount; //板卡包含的DSP个数
            public uint firstDspIndex; //板卡上第一个DSP在所有DSP中的索引
            public uint encodeChannelCount; //板卡包含的编码通道个数
            public uint firstEncodeChannelIndex; //板卡上第一个编码通道在所有编码通道中的索引
            public uint decodeChannelCount; //板卡包含的解码通道个数
            public uint firstDecodeChannelIndex; //板卡上第一个解码通道在所有解码通
            public uint displayChannelCount; //板卡包含的视频输出通道个数
            public uint firstDisplayChannelIndex; //板卡上第一个视频输出通道在所有视频输出通道中的索引
            public uint reserved1;
            public uint reserved2;
            public uint reserved3;
            public uint reserved4;
        };

        public struct REGION_PARAM
        {
            public uint left;// 区域左边界
            public uint top; //区域上边界
            public uint width; //区域宽度
            public uint height; //区域高度
            public UInt32 color; //区域背景色
            public uint param; //区域扩展参数
        };
        public struct SYSTEMTIME
        {
            public UInt16 wYear;
            public UInt16 wMonth;
            public UInt16 wDayOfWeek;
            public UInt16 wDay;
            public UInt16 wHour;
            public UInt16 wMinute;
            public UInt16 wSecond;
            public UInt16 wMilliseconds;
        };
        public struct tagVersion
        {
            public UInt32 nSDKVer;
            public UInt32 nSDKBuildTime;

            public UInt32 nDriverVer;
            public UInt32 nDriBuildTime;

            public UInt32 nDSPVer;
            public UInt32 nDSPBuildTime;
        };

        #endregion
        #region << 原SDK调用 >>
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="pDeviceTotal">初始化成功的通道个数</param>
        /// <returns>错误码</returns>
        [DllImport("VDCDecode.dll")]
        private static extern int DH_InitDecDevice(ref int pDeviceTotal);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDECProgressCB(fnDecProgressCB fnCallback, UInt32 dwUser);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ReleaseDecDevice();

        [DllImport("VDCDecode.dll")]
        private static extern int DH_InitDirectDraw(IntPtr hParent, UInt32 colorKey);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_ReleaseDirectDraw();
        [DllImport("VDCDecode.dll")]
        private static extern int DH_ChannelOpen(Int32 nChannelNum, ref UInt32 phChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ChannelClose(UInt32 hChannel);

        //Open Part
        [DllImport("VDCDecode.dll")]
        private static extern int DH_OpenStream(UInt32 hChannel, IntPtr pFileHeadBuf, UInt32 dwSize);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ResetStream(UInt32 hChannel);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_CloseStream(UInt32 hChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_InputData(UInt32 hChannel, IntPtr pBuf, UInt32 dwSize);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_OpenFile(UInt32 hChannel, string szFileName);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_CloseFile(UInt32 hChannel);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDisplayPara(UInt32 hChannel, IntPtr pPara);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_Play(UInt32 hChannel, IntPtr hwnd);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_Stop(UInt32 hChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_Pause(UInt32 hChannel, UInt32 bPause);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_PlaySound(UInt32 hChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_StopSound(UInt32 hChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetVolume(UInt32 hChannel, UInt32 nVolume);

        //overlay part(not been used)
        [DllImport("VDCDecode.dll")]
        private static extern int DH_RefreshSurface();
        [DllImport("VDCDecode.dll")]
        private static extern int DH_ReStoreSurface();
        [DllImport("VDCDecode.dll")]
        private static extern int DH_ClearSurface();


        [DllImport("VDCDecode.dll")]
        private static extern int DH_ZoomOverlay(IntPtr pSrcClientRect, IntPtr pDecScreenRect);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_StartCapFile(UInt32 hChannel, string sFileName);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_StopCapFile(UInt32 hChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetYV12Image(UInt32 hChannel, ref string pBuffer, UInt32 nSize);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetPictureSize(UInt32 hChannel, ref UInt32 pWidth, ref UInt32 pHeight);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ConvertToBmpFile(ref byte[] pBuf, UInt32 nSize, UInt32 nWidth, UInt32 nHeight,
                                                      ref  string sFileName, UInt32 nReserved);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_Jump(UInt32 hChannel, UInt32 nDirection);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetJumpInterval(UInt32 hChannel, UInt32 nSecond);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetSpeed(UInt32 hChannel, ref Int32 pSpeed);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetSpeed(UInt32 hChannel, Int32 nSpeed);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetPlayPos(UInt32 hChannel, UInt32 nPos);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetPlayPos(UInt32 hChannel, ref UInt32 pPos);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetVersion(ref tagVersion pVersion);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetCurrentFrameRate(UInt32 hChannel, ref UInt32 pFrameRate);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetCurrentFrameNum(UInt32 hChannel, ref UInt32 pFrameNum);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetFileTotalFrames(UInt32 hChannel, ref UInt32 pTotalFrames);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetFileTime(UInt32 hChannel, ref UInt32 pFileTime);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetCurrentFrameTime(UInt32 hChannel, ref UInt32 pFrameTime);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetPlayedFrames(UInt32 hChannel, ref UInt32 pDecVFrames);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetDeviceSerialNo(UInt32 hChannel, ref UInt32 pDeviceSerialNo);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetFileEndMsg(UInt32 hChannel, IntPtr hWnd, uint nMsg);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetStreamOpenMode(UInt32 hChannel, UInt32 nMode);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetStreamOpenMode(UInt32 hChannel, ref UInt32 pMode);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetVideoOutStandard(UInt32 hChannel, UInt32 nStandard);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDspDeadlockMsg(IntPtr hWnd, uint nMsg);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetChannelNum(Int32 nDspNum, ref Int32 pChannelNum, UInt32 nNumsToGet, ref UInt32 pNumsGotten);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ResetDsp(Int32 nDspNum);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetAudioPreview(UInt32 hChannel, bool bEnable);

        [DllImport("VDCDecode.dll")]
        private static extern uint DH_GetDspCount();

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetDspDetail(uint dspNum, ref tagDSPDetail pDspDetail);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetBoardDetail(uint boardNum, ref DS_BOARD_DETAIL pBoardDetail);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_OpenStreamEx(UInt32 hChannel, IntPtr pFileHeadBuf, UInt32 nSize);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_CloseStreamEx(UInt32 hChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_InputVideoData(UInt32 hChannel, ref string pBuf, UInt32 dwSize);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_InputAudioData(UInt32 hChannel, ref string pBuf, UInt32 nSize);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetBoardCount();



        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDefaultVideoStandard(VIDEO_STANDARD VideoStandard);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetDisplayChannelCount();

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDisplayStandard(uint nDisplayChannel, VIDEO_STANDARD VideoStandard);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDisplayRegion(uint nDisplayChannel, uint nRegionCount,
                                                        ref REGION_PARAM pParam, uint nReserved);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ClearDisplayRegion(uint nDisplayChannel, uint nRegionFlag);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDisplayRegionPosition(uint nDisplayChannel,
                                                               uint nRegion, uint nLeft, uint nTop);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_FillDisplayRegion(uint nDisplayChannel, uint nRegion, ref byte[] pImage);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDecoderAudioOutput(uint nDecodeChannel, bool bOpen, uint nOutputChannel);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDecoderVideoOutput(uint nDecodeChannel, uint nPort, bool bOpen,
                                                            uint nDisplayChannel, uint nDisplayRegion, uint nReserved);
        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDecoderVideoExtOutput(uint nDecodeChannel, uint nPort, bool bOpen,
                                                               uint nDisplayChannel, uint nDisplayRegion, uint nReserved);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetFileRef(UInt32 hChannel, bool bEnable, FILE_REF_DONE_CALLBACK FileRefDoneCallback);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetFileAbsoluteTime(UInt32 hChannel,ref  SYSTEMTIME pStartTime,ref  SYSTEMTIME pEndTime);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_GetCurrentAbsoluteTime(UInt32 hChannel, ref SYSTEMTIME pTime);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_LocateByAbsoluteTime(UInt32 hChannel, SYSTEMTIME time);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_LocateByFrameNumber(UInt32 hChannel, uint frmNum);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ImportFileRef(UInt32 hChannel, byte[] pBuffer, uint nSize);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_ExportFileRef(UInt32 hChannel, byte[] pBuffer, uint nSize);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDisplayVideoCapture(uint nDisplayChannel, bool bStart, uint fps,
                                                             uint width, uint height, ref byte[] imageBuffer);


        [DllImport("VDCDecode.dll")]
        private static extern int RegisterDisplayVideoCaptureCallback(IMAGE_STREAM_CALLBACK DisplayVideoCaptureCallback, IntPtr context);

        [DllImport("VDCDecode.dll")]
        private static extern int SetDisplayVideoBrightness(uint chan, int Brightness);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetDecoderPostProcess(UInt32 hChannel, uint param);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetTVMargin(UInt32 hChannel, int left, int top, int right, int bottom);


        //Raw data play part

        [DllImport("VDCDecode.dll")]
        private static extern int DH_SetRawPlayMode(UInt32 hChannel, bool bRaw);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_UpdateVideoFormat(UInt32 hChannel, int nWidth, int nHeight, int nFrameRate, int videoFormat);

        [DllImport("VDCDecode.dll")]
        private static extern int DH_InputRawData(UInt32 hChannel, ref byte[] pBuf, int nSize);

        #endregion
    }
}
