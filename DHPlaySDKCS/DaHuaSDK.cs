/*
 * ************************************************************************
 *                            SDK
 *                      大华PaySDK(C#版)
 * 
 * (c) Copyright 2007, ZheJiang Dahua Technology Stock Co.Ltd.
 *                      All Rights Reserved
 * 版 本 号:0.01
 * 文件名称:DaHuaSDK.cs
 * 功能说明:原始封装[在现有的PlaySDK(C++版)上再一次封装,基本与原C++接口对应]
 * 作    者:李德明
 * 作成日期:2008/01/18
 * 修改日志:    日期        版本号      作者        变更事由
 *              2007/01/18  0.01        李德明      新建作成
 * 
 * ************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;


namespace DHPlaySDK
{
    #region << DHPlay类定义 >>
    public sealed class DHPlay
    {
        /// <summary>
        /// 是否己经初始化
        /// </summary>
        private  bool initialized = false;

        /// <summary>
        /// 是否将错误抛出[默认不抛出，只将错误信息返回给属性LastOperationInfo]
        /// </summary>
        private  bool pShowException = false;

        #region << 属性 >>
        //暂无
        #endregion

        #region << 类方法 >>

        #region << 私有方法 >>

        /// <summary>
        /// SDK调用失败时抛出异常,成功时返回无异常信息,并把操作信息赋给LastOperationInfo
        /// </summary>
        /// <param name="nPort">播放通道</param>
        /// <exception cref="Win32Exception">原生异常,当SDK调用失败时触发。</exception>
        private  OPERATION_INFO DHThrowLastError(int nPort)
        {
            OPERATION_INFO returnValue;
            Int32 errorCode = (int)PLAY_GetLastError(nPort);
            returnValue.errCode = errorCode.ToString();
            returnValue.errMessage = DHGetLastErrorName((uint)errorCode);
            if (pShowException == true)
            {
                throw new Win32Exception(errorCode, returnValue.errMessage);
            }
            return returnValue;
        }

        /// <summary>
        /// 错误代码转换为标准备的错误信息描述
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <returns>标准备错误信息描述</returns>
        private  string DHGetLastErrorName(uint errorCode)
        {
            switch (errorCode)
            {
                case 0x80000000 | 0:
                    return "没有错误";
                case 0x80000000 | 1:
                    return "输入参数非法";
                case 0x80000000 | 2:
                    return "调用顺序不对";
                case 0x80000000 | 3:
                    return "多媒体时钟设置失败";
                case 0x80000000 | 4:
                    return "视频解码失败";
                case 0x80000000 | 5:
                    return "音频解码失败";
                case 0x80000000 | 6:
                    return "分配内存失败";
                case 0x80000000 | 7:
                    return "文件操作失败";
                case 0x80000000 | 8:
                    return "创建线程事件等失败";
                case 0x80000000 | 9:
                    return "创建directDraw失败";
                case 0x80000000 | 10:
                    return "创建后端缓存失败";
                case 0x80000000 | 11:
                    return "缓冲区满，输入流失败";
                case 0x80000000 | 12:
                    return "创建音频设备失败";
                case 0x80000000 | 13:
                    return "设置音量失败";
                case 0x80000000 | 14:
                    return "只能在播放文件时才能使用";
                case 0x80000000 | 15:
                    return "只能在播放流时才能使用";
                case 0x80000000 | 16:
                    return "系统不支持，解码器只能工作在Pentium 3以上";
                case 0x80000000 | 17:
                    return "没有文件头";
                case 0x80000000 | 18:
                    return "解码器和编码器版本不对应";
                case 0x80000000 | 19:
                    return "初始化解码器失败";
                case 0x80000000 | 20:
                    return "文件太短或码流无法识别";
                case 0x80000000 | 21:
                    return "初始化多媒体时钟失败";
                case 0x80000000 | 22:
                    return "位拷贝失败";
                case 0x80000000 | 23:
                    return "显示overlay失败";
                default:
                    return "未知错误";
            }
        }

        #region << 屏蔽代码 >>

        //private  void DHThrowLastError(int returnValue)
        //{
        //    if (returnValue == 0)
        //    {
        //        DHThrowLastError();
        //    }
        //    else
        //    {
        //        pErrInfo.errCode = "0";
        //        pErrInfo.errMessage = "最近操作无异常发生";
        //    }
        //}

        //private  void DHThrowLastError(bool returnValue)
        //{
        //    if (returnValue == false)
        //    {
        //        DHThrowLastError();
        //    }
        //    else
        //    {
        //        pErrInfo.errCode = "0";
        //        pErrInfo.errMessage = "最近操作无异常发生";
        //    }
        //}

        ///// <summary>
        ///// SDK调用失败时抛出异常
        ///// </summary>
        ///// <param name="e"></param>
        //private  void DHThrowLastError(Exception e)
        //{

        //    pErrInfo.errCode = e.ToString();
        //    pErrInfo.errMessage = e.Message;
        //    if (pShowException == true)
        //    {
        //        throw e;
        //    }
        //}

        #endregion

        #endregion

        #region << 公用方法 >>

        #region << 抛错方式设置 >>

        /// <summary>
        /// 设置使用本类的程序中的是否将错误信息抛出[默认不抛出，只将错误信息返回给属性LastOperationInfo]
        /// </summary>
        /// <param name="blnShowException">
        /// true:抛出错误信息,并将错误信息返回给属性LastOperationInfo,由客户自行处理;
        /// false:不抛出错误信息,将错误信息返回给属性LastOperationInfo,不作任何处理,用户可根据方法的返回值
        ///       或LastOperationInfo判断是否有错误发生,然后再作相应处理
        /// </param>
        public  void DHSetShowException(bool blnShowException)
        {
            pShowException = blnShowException;
        }

        #endregion

        #region << 播放文件 >>

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.StopSound:停止声音
        /// </param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom)
        {
            bool returnValue = false;
            if (pPlayCom == PLAY_COMMAND.StopSound)
            {
                returnValue = PLAY_StopSound();
                DHThrowLastError(0);
            }
            return returnValue;    
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.SetVolume:设置音量
        /// 2.PLAY_COMMAND.SetCurrentFrameNum:设置当前播放位置到指定帧号
        /// </param>
        /// <param name="nPort">播放通道</param>
        /// <param name="nValue">命令参数值</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, UInt32 nValue)
        {
            bool returnValue = false;
            switch (pPlayCom)
            {
                case PLAY_COMMAND.SetVolume:
                    returnValue = PLAY_SetVolume(nPort, nValue);
                    break;
                case PLAY_COMMAND.SetCurrentFrameNum:
                    returnValue = PLAY_SetCurrentFrameNum(nPort, nValue);
                    break;
                case PLAY_COMMAND.SetPlayedTimeEx:
                    returnValue = PLAY_SetPlayedTimeEx(nPort, nValue);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;   
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.SetPlayPos:设置文件播放的相对位置(百分比)
        /// </param>
        /// <param name="nPort">播放通道</param>
        /// <param name="nValue">参数值</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, ref float nValue)
        {
            bool returnValue = false;
            switch (pPlayCom)
            {
                case PLAY_COMMAND.SetPlayPos:
                    returnValue = PLAY_SetPlayPos(nPort, nValue);
                    break;
                case PLAY_COMMAND.GetPlayPos:                    
                    nValue = PLAY_GetPlayPos(nPort);
                    returnValue = true;
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;
        }
        

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.Start:开始播放
        /// </param>
        /// <param name="nPort">播放通道号</param>
        /// <param name="hWnd">播放容器句柄</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, IntPtr hWnd)
        {
            bool returnValue = false;
            if (pPlayCom == PLAY_COMMAND.Start)
            {
                returnValue = PLAY_Play(nPort, hWnd);
                DHThrowLastError(nPort);
            }
            return returnValue;            
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.OpenFile:打开文件;
        /// 2.PLAY_COMMAND.CatchPic:抓图;</param>
        /// <param name="nPort">播放通道号</param>
        /// <param name="sFileName">打开/保存文件名</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, string sFileName)
        {
            bool returnValue = false;
            switch (pPlayCom)
            {
                case PLAY_COMMAND.OpenFile:
                    returnValue = PLAY_OpenFile(nPort, sFileName);
                    break;
                case PLAY_COMMAND.CatchPic:
                    returnValue = PLAY_CatchPic(nPort, sFileName);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.Stop:停止;
        /// 2.PLAY_COMMAND.CloseFile:关闭文件;
        /// 3.PLAY_COMMAND.Pause:暂停播放;
        /// 4.PLAY_COMMAND.ReSume:继续播放;
        /// 5.PLAY_COMMAND.Fast:快放;
        /// 6.PLAY_COMMAND.Slow:慢放;
        /// 7.PLAY_COMMAND.OneByOne:单帧播放;
        /// 8.PLAY_COMMAND.PlaySound:播放声音;
        /// 9.PLAY_COMMAND.PlaySoundShare:共享播放声音;
        /// 10.PLAY_COMMAND.StopSoundShare:停止共享播放声音;
        /// 11.PLAY_COMMAND.OneByOneBack:单帧回放
        /// 12.PLAY_COMMAND.CloseStream:关闭数据流
        /// 13.PLAY_COMMAND.CloseStreamEx:关闭数据流(音视频分开方式打开流)
        /// </param>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort)
        {
            bool returnValue = false;

            switch (pPlayCom)
            {
                case PLAY_COMMAND.Stop://停止
                    returnValue = PLAY_Stop(nPort);
                    break;
                case PLAY_COMMAND.CloseFile://关闭文件
                    returnValue = PLAY_CloseFile(nPort);
                    break;
                case PLAY_COMMAND.Pause://暂停播放
                    returnValue = PLAY_Pause(nPort, 1);
                    break;
                case PLAY_COMMAND.ReSume://继续播放
                    returnValue = PLAY_Pause(nPort, 0);
                    break;
                case PLAY_COMMAND.Fast://快放
                    returnValue = PLAY_Fast(nPort);
                    break;
                case PLAY_COMMAND.Slow://慢放
                    returnValue = PLAY_Slow(nPort);
                    break; 
                case PLAY_COMMAND.OneByOne://单帧播放
                    returnValue = PLAY_OneByOne(nPort);
                    break;
                case PLAY_COMMAND.OneByOneBack://单帧回放
                    returnValue = PLAY_OneByOneBack(nPort);
                    break;
                case PLAY_COMMAND.PlaySound://播放声音
                    returnValue = PLAY_PlaySound(nPort);
                    break;
                case PLAY_COMMAND.PlaySoundShare://共享播放声音
                    returnValue = PLAY_PlaySoundShare(nPort);
                    break;
                case PLAY_COMMAND.StopSoundShare://停止共享播放声音
                    returnValue = PLAY_StopSoundShare(nPort);
                    break;                    
                case PLAY_COMMAND.ToBegin:
                    if (!DHPlayControl(PLAY_COMMAND.SetCurrentFrameNum, nPort, 0))
                    {
                        if (!DHPlayControl(PLAY_COMMAND.SetPlayedTimeEx, nPort, 0))
                        {
                            float playBegionPos = 0.00F;
                            if (!DHPlayControl(PLAY_COMMAND.SetPlayPos, nPort, ref playBegionPos))
                            {
                                returnValue = false;
                                break;
                            }
                        }
                    }
                    returnValue = true;
                    break;
                case PLAY_COMMAND.ToEnd:
                    if (!DHPlayControl(PLAY_COMMAND.SetCurrentFrameNum, nPort,DHPlayControl(PLAY_COMMAND.GetFileTotalFrames,nPort,true)-1))
                    {
                        if (!DHPlayControl(PLAY_COMMAND.SetPlayedTimeEx, nPort, DHPlayControl(PLAY_COMMAND.GetFileTotalTime, nPort, true)))
                        {
                            float playEndPos = 0.99F;
                            if (!DHPlayControl(PLAY_COMMAND.SetPlayPos, nPort, ref playEndPos))
                            {
                                returnValue = false;
                                break;
                            }                            
                        }
                    }
                    returnValue = true;
                    break;
                case PLAY_COMMAND.CloseStream:
                    returnValue = PLAY_CloseStream(nPort);
                    break;
                case PLAY_COMMAND.CloseStreamEx:
                    returnValue = PLAY_CloseStreamEx(nPort);
                    break;
                default:
                    return false;
            }

            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.GetCurrentFrameNum:获得当前帧序列号;
        /// 2.PLAY_COMMAND.GetCurrentFrameRate:获得当前帧率;
        /// 3.PLAY_COMMAND.GetFileTotalTime:获得文件总的时间长度,单位秒;
        /// 4.PLAY_COMMAND.GetFileTotalFrames:获得文件中的总帧数;
        /// 5.PLAY_COMMAND.GetPlayedTime:获得当前播放的时间,单位秒;
        /// 6.PLAY_COMMAND.GetPlayedTimeEx:获得当前播放的时间,单位毫秒;
        /// 7.PLAY_COMMAND.GetVolume:获得当前设置的音量;
        /// </param>
        /// <param name="nPort">播放通道号</param>
        /// <param name="blnReturnValue">该参数无效，只是用来区分重载的，必须输入一个bool值</param>
        /// <returns>对应命令的取值</returns>
        public  UInt32 DHPlayControl(PLAY_COMMAND pPlayCom, int nPort,bool blnReturnValue)
        {
            UInt32 returnValue = 0;

            switch (pPlayCom)
            {
                case PLAY_COMMAND.GetCurrentFrameNum:
                    returnValue = PLAY_GetCurrentFrameNum(nPort);
                    break;
                case PLAY_COMMAND.GetCurrentFrameRate:
                    returnValue = PLAY_GetCurrentFrameRate(nPort);
                    break;
                case PLAY_COMMAND.GetFileTotalTime:
                    returnValue = PLAY_GetFileTime(nPort);
                    break;
                case PLAY_COMMAND.GetFileTotalFrames:
                    returnValue = PLAY_GetFileTotalFrames(nPort);
                    break;
                case PLAY_COMMAND.GetPlayedTime:
                    returnValue = PLAY_GetPlayedTime(nPort);
                    break;
                case PLAY_COMMAND.GetPlayedTimeEx:
                    returnValue = PLAY_GetPlayedTimeEx(nPort);
                    break;
                case PLAY_COMMAND.GetVolume:
                    returnValue = PLAY_GetVolume(nPort);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.GetColor:获得颜色值
        /// </param>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nRegionNum">显示区域</param>
        /// <param name="returnColor">颜色返回值</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, UInt32 nRegionNum,ref COLOR_STRUCT returnColor)
        {
            bool returnValue = false;
            switch (pPlayCom)
            { 
                case PLAY_COMMAND.GetColor:
                    returnValue = PLAY_GetColor(nPort, nRegionNum, ref returnColor.pBrightness, ref returnColor.pContrast, ref returnColor.pSaturation, ref returnColor.pHue);
                    break;
                case PLAY_COMMAND.SetColor:
                    returnValue = PLAY_SetColor(nPort, nRegionNum, returnColor.pBrightness, returnColor.pContrast, returnColor.pSaturation, returnColor.pHue);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.GetPictureSize:获得帧原始图像大小
        /// </param>
        /// <param name="nPort">播放通道</param>
        /// <param name="frameInfo">帧信息</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, ref FRAME_INFO frameInfo)
        {
            bool returnValue = false;
            switch (pPlayCom)
            { 
                case PLAY_COMMAND.GetPictureSize:
                    returnValue = PLAY_GetPictureSize(nPort, ref frameInfo.nWidth, ref frameInfo.nHeight);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;
        }

        #endregion

        #region << 播放数据流 >>

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.OpenStream:打开流接口
        /// 2.PLAY_COMMAND.OpenStreamEx:以音视频分开输入的方式打开流
        /// </param>
        /// <param name="nPort">通道号</param>
        /// <param name="pFileHeadBuff">目前不使用，填NULL</param>
        /// <param name="nSize">目前不使用，填0</param>
        /// <param name="nBufPoolSize">
        /// 设置播放器中存放数据流的缓冲区大小。
        /// 范围是SOURCE_BUF_MIN~ SOURCE_BUF_MAX。
        /// 一般设为900*1024，如果数据送过来相对均匀，可调小该值，如果数据传输不均匀，可增大该值</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, IntPtr pFileHeadBuff, UInt32 nSize, UInt32 nBufPoolSize)
        {

            bool returnValue = false;
            switch (pPlayCom)
            {
                case PLAY_COMMAND.OpenStream:
                    returnValue = PLAY_OpenStream(nPort, pFileHeadBuff, nSize, nBufPoolSize);
                    break;
                case PLAY_COMMAND.OpenStreamEx:
                    returnValue = PLAY_OpenStreamEx(nPort, pFileHeadBuff, nSize, nBufPoolSize);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 播放控制
        /// </summary>
        /// <param name="pPlayCom">
        /// 播放命令:
        /// 1.PLAY_COMMAND.OpenStream:输入从卡上得到的流数据；打开流并调用PLAY_Play之后才能输入数据。
        /// 2.PLAY_COMMAND.InputVideoData:输入从卡上得到的视频流 (可以是复合流，但音频数据会被忽略);打开流之后才能输入数据
        /// 3.PLAY_COMMAND.InputAudioData:输入从卡上得到的音频流；打开声音之后才能输入数据
        /// </param>
        /// <param name="nPort">通道号</param>
        /// <param name="pBuf">缓冲区地址</param>
        /// <param name="nSize">缓冲区大小</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort, IntPtr pBuf, UInt32 nSize)
        {
            bool returnValue = false;
            switch (pPlayCom)
            {
                case PLAY_COMMAND.InputData:
                    returnValue = PLAY_InputData(nPort, pBuf, nSize);
                    break;
                case PLAY_COMMAND.InputVideoData:
                    returnValue = PLAY_InputVideoData(nPort, pBuf, nSize);
                    break;
                case PLAY_COMMAND.InputAudioData:
                    returnValue = PLAY_InputAudioData(nPort, pBuf, nSize);
                    break;
            }
            DHThrowLastError(nPort);
            return returnValue;

        }

        public  bool DHPlaySetSecurityKey(int nPort, IntPtr szKey, UInt32 nKeylen)
        {
            return PLAY_SetSecurityKey(nPort, szKey, nKeylen);
        }

        #region << 屏蔽代码 >>

        ///// <summary>
        ///// 播放控制
        ///// </summary>
        ///// <param name="pPlayCom">
        ///// 播放命令:
        ///// 1.PLAY_COMMAND.CloseStream:关闭数据流
        ///// 2.PLAY_COMMAND.CloseStreamEx:关闭数据流(音视频分开方式打开流)
        ///// </param>
        ///// <param name="nPort">通道号</param>
        ///// <returns>true:成功;false:失败</returns>
        //public  bool DHPlayControl(PLAY_COMMAND pPlayCom, int nPort)
        //{ 
        //    bool returnValue = false;
        //    switch (pPlayCom)
        //    {
        //        case PLAY_COMMAND.CloseStream:
        //            returnValue = PLAY_CloseStream(nPort);
        //            break;
        //        case PLAY_COMMAND.CloseStreamEx:
        //            returnValue = PLAY_CloseStreamEx(nPort);
        //            break;
        //    }
        //    DHThrowLastError(nPort);
        //    return returnValue;            
        //}

        #endregion

        #endregion

        #region << 字符叠加 >>

        /// <summary>
        /// 注册一个回调函数，获得当前表面的device context, 你可以在这个DC上画图（或写字），就好像在窗口的客户区DC上绘图，
        /// 但这个DC不是窗口客户区的DC，而是DirectDraw里的Off-Screen表面的DC。注意，如果是使用overlay表面，这个接口无效，
        /// 你可以直接在窗口上绘图，只要不是透明色就不会被覆盖。
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="privateDrawFun">回调函数句柄</param>
        /// <param name="nUser">用户数据</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHRigisterDrawFun(int nPort, DrawFun privateDrawFun, int nUser)
        {
            bool returnValue = false;
            returnValue = PLAY_RigisterDrawFun(nPort, privateDrawFun, nUser);
            DHThrowLastError(nPort);
            return returnValue;
        }

        #endregion

        #region << 多区域显示 >>

        /// <summary>
        /// 设置或增加显示区域。可以做局部放大显示。 
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nRegionNum">显示区域序号，0~(MAX_DISPLAY_WND-1)。如果nRegionNum为0，表示对主要显示窗口中</param>
        /// <param name="pSrcRect">局部显示区域</param>
        /// <param name="hDestWnd">显示窗口句柄</param>
        /// <param name="bEnable">打开（设置）或关闭显示区域</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetDisplayRegion(int nPort, UInt32 nRegionNum, IntPtr pSrcRect, IntPtr hDestWnd, bool bEnable)
        {
            bool returnValue = false;
            returnValue = PLAY_SetDisplayRegion(nPort, nRegionNum, pSrcRect, hDestWnd, bEnable);
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 刷新显示，刷新多区域显示的窗口
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nRegionNum">显示区域序号</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHRefreshPlayEx(int nPort, UInt32 nRegionNum)
        {
            bool returnValue = false;
            returnValue = PLAY_RefreshPlayEx(nPort, nRegionNum);
            DHThrowLastError(nPort);
            return returnValue;

        }

        #endregion

        #region << 数据流录像 >>

        /// <summary>
        /// 开始流数据录像。只对流模式有用，在PLAY_Play之后调用
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="sFileName">录像文件名，如果文件名中有不存在的文件夹，就创建该文件夹</param>
        /// <param name="idataType">0:原始数据流;1:AVI</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHStartDataRecord(int nPort, string sFileName, int idataType)
        {
            bool returnValue = false;
            returnValue = PLAY_StartDataRecord(nPort, sFileName,idataType);
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 停止流数据录像
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHStopDataRecord(int nPort)
        {
            bool returnValue = false;
            returnValue = PLAY_StopDataRecord(nPort);
            DHThrowLastError(nPort);
            return returnValue;
        }

        #endregion

        #region << 数据回调 >>

        /// <summary>
        /// 设置回调函数
        /// </summary>
        /// <param name="nPort">端口</param>
        /// <param name="cbFun">回调函数</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetDecCallBack(int nPort, DecCBFun cbFun)
        {
            bool returnValue=false;
            returnValue = PLAY_SetDecCallBack(nPort,ref cbFun);
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 设置解码回调的流类型
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="streamType">解码回调流类型</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetDecCBStream(int nPort, STREAM_TYPE streamType)
        {
            bool returnValue = false;
            
            if (streamType != STREAM_TYPE.STREAM_WITHOUT)
            {
                returnValue= PLAY_SetDecCBStream(nPort, (uint)streamType);
            }            
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 设置视频图像数据回调，可用作抓图。如果要停止回调，可以把回调函数指针DisplayCBFun设为NULL。
        /// 一旦设置回调函数，则一直有效，直到程序退出。该函数可以在任何时候调用
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="dCBFun">DisplayCBFun的回调函数</param>
        /// <param name="nUser">用户自定义数据,默认为0</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetDisplayCallBack(int nPort, DisplayCBFun dCBFun, int nUser)
        {
            bool returnValue = false;
            returnValue = PLAY_SetDisplayCallBack(nPort, dCBFun,nUser);
            DHThrowLastError(nPort);
            return returnValue;
        }

        /// <summary>
        /// 设置视频图像数据回调，可用作抓图。如果要停止回调，可以把回调函数指针DisplayCBFun设为NULL。一旦设置回调函数，
        /// 则一直有效，直到程序退出。该函数可以在任何时候调用
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="dCBFun">DisplayCBFun的回调函数</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetDisplayCallBack(int nPort, DisplayCBFun dCBFun)
        {
            return DHSetDisplayCallBack(nPort, dCBFun, 0);
        }

        /// <summary>
        /// 音频帧解码后的wave数据回调
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="funAudio">音频回调函数</param>
        /// <param name="nUser">用户自定义数据</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetAudioCallBack(int nPort, FunAudio funAudio, int nUser)
        {
            bool returnValue = false;
            returnValue = PLAY_SetAudioCallBack(nPort, funAudio, nUser);
            DHThrowLastError(nPort);
            return returnValue;
        }

        #endregion

        #region << 获得版本号 >>

        /// <summary>
        /// 得到当前播放器sdk的主版本号、次版本号和补丁号
        /// </summary>
        /// <returns>
        /// 高16位表示当前的主版本号。
        /// 9~16位表示次版本号，1~8位表示次补丁号。
        /// 例：返回值0x00030107表示：主版本号是3，次版本号是1，补丁号是7
        /// </returns>
        public  UInt32 DHGetSdkVersion()
        {
            return PLAY_GetSdkVersion();
        }

        /// <summary>
        /// 按指定格式返回当前播放器sdk的版本号
        /// </summary>
        /// <param name="verFormatStyle">版本格式字符串:A:主版本号;B:次版本号;C:补丁号;</param>
        /// <returns>播放器的SDK的版本号</returns>
        public  string DHGetSdkVersion(string verFormatStyle)
        {
            UInt32 sdkVer = DHGetSdkVersion();
            string strTemp = verFormatStyle.ToUpper();
            //版本号计算
            int AA = (int)sdkVer >> 16;//主版本号:高16位
            int BB = (int)(sdkVer >> 8)&0x0000FF;//次版本号:9~16位
            int CC = (int)sdkVer & 0x000000FF;//补丁号:1~8位
            strTemp = strTemp.Replace("A", AA.ToString()).Replace("B", BB.ToString()).Replace("C",CC.ToString());
            return strTemp;
        }

        #endregion

        #region << 设置属性 >>

        /// <summary>
        /// 设置流播放的模式。必须在播放之前设置
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nMode">播放模式</param>
        /// <returns>true:成功;false:失败</returns>
        public  bool DHSetStreamOpenMode(int nPort, PLAY_MODE nMode)
        {
            bool returnValue = false;
            returnValue = PLAY_SetStreamOpenMode(nPort, (uint)nMode);
            DHThrowLastError(nPort);
            return returnValue;
        }

        #endregion

        #endregion

        #endregion

        #region << 公共函数 >>

        /// <summary>
        /// 将一个整数值转为指定的时间格式
        /// </summary>
        /// <param name="uintValue">时间值</param>
        /// <param name="rate">与秒之间的进率,例:时间值单位为秒时,则进率为1;时间值单位为毫秒,则进率为0.001;时间值单位为分,
        /// 则进率为60,如果时间格式字符串没有时此参数无效</param>
        /// <param name="timeStyle">时间格式字符串,如:HH:mm:ss,参见DateTime.Format字符串定义</param>
        /// <returns>指定格式的字符串时间表示</returns>
        public  string DHConvertToTime(uint  uintValue,decimal rate,string timeStyle)
        {
            uint uintSec=Convert.ToUInt32( uintValue*rate);
            int d=(int)(uintSec/87840);
            int h = (int)((uintSec - d * 87840) / 3600);
            int m = (int)((uintSec - d * 87840 - h * 3600) / 60);
            int s = (int)(uintSec - d * 87840 - h * 3600 - m * 60);
            string DD=(d>=10?d.ToString():"0"+d.ToString());
            string HH = (h>=10?h.ToString():"0"+h.ToString());
            string mm = (m >= 10 ? m.ToString() : "0" + m.ToString());
            string ss = (s >= 10 ? s.ToString() : "0" + s.ToString());
            string strTemp = timeStyle.ToUpper();
            strTemp = strTemp.Replace("DD", DD).Replace("HH", HH).Replace("MM",mm).Replace("SS",ss).Replace("D",d.ToString()).Replace("H",h.ToString()).Replace("M",m.ToString()).Replace("S",s.ToString());
            if (timeStyle.Length == 0)//没有输时间格式的情况
            {
                strTemp = uintValue.ToString();
            }
            return strTemp;
        }
        #region << 屏蔽代码 >>

        ///// <summary>
        ///// 将一个整数值转为指定的时间格式
        ///// </summary>
        ///// <param name="uintValue">时间值</param>
        ///// <param name="rate">与秒之间的进率,例:时间值单位为毫秒,则进率为0.001;时间值单位为分,则进率为60</param>
        ///// <returns>DateTime格式时间</returns>
        //public  DateTime DHConvertToTime(uint uintValue, decimal rate)
        //{
        //    return new DateTime((long)(uintValue * 10 * rate), DateTimeKind.Local);
        //}

        #endregion 

        #endregion

        #region << 原SDK接口调用 >>

        #region << 播放文件 >>

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="sFileName">文件名</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_OpenFile(int nPort, string sFileName);

        /// <summary>
        /// 开始播放。如果已经播放，改变当前播放状态为正常速度播放
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="hWnd">播放窗口句柄</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_Play(int nPort, IntPtr hWnd);

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_Stop(int nPort);

        /// <summary>
        /// 关闭播放文件
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_CloseFile(int nPort);

        #endregion

        #region << 文件定位 >>

        /// <summary>
        /// 设置当前播放播放位置到指定帧号；根据帧号来定位播放位置。此函数必须在文件索引生成之后才能调用
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nFrameNum">帧序号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetCurrentFrameNum(int nPort, UInt32 nFrameNum);

        /// <summary>
        /// 根据时间设置文件播放位置，此接口比PLAY_SetPlayPos费时，但如果用时间来控制进度条（与PLAY_GetPlayedTime(Ex)配合使用），那么可以使进度条平滑滚动
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nTime">设置文件播放位置到指定时间。单位毫秒</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetPlayedTimeEx(int nPort, UInt32 nTime);

        /// <summary>
        /// 设置文件播放指针的相对位置（百分比）
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="fRelativePos">范围0-100%</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetPlayPos(int nPort, float fRelativePos);

        /// <summary>
        /// 获得文件播放指针的相对位置
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <returns>范围0-100%</returns>
        [DllImport("dhplay.dll")]
        private static extern float PLAY_GetPlayPos(int nPort);

        #endregion

        #region << 设置属性 >>

        /// <summary>
        /// 设置图象的视频参数，即时起作用
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nRegionNum">显示区域，参考PLAY_SetDisplayRegion；如果只有一个显示区域(通常情况)设为0</param>
        /// <param name="nBrightness">亮度，默认64； 范围0-128</param>
        /// <param name="nContrast">对比度，默认64； 范围0-128</param>
        /// <param name="nSaturation">饱和度，默认64； 范围0-128</param>
        /// <param name="nHue">色调，默认64； 范围0-128</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetColor(int nPort, UInt32 nRegionNum, int nBrightness, int nContrast, int nSaturation, int nHue);

        /// <summary>
        /// 设置流播放的模式。必须在播放之前设置
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nMode">
        /// STREAME_REALTIME:实时模式（默认）
        /// STREAME_FILE:文件模式
        /// </param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetStreamOpenMode(int nPort, UInt32 nMode);
        #endregion

        #region << 音频控制 >>

        /// <summary>
        /// 打开声音
        /// [同一时刻只能有一路声音。如果现在已经有声音打开，则自动关闭原来已经打开的声音。]
        /// **注意：默认情况下声音是关闭的**
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_PlaySound(int nPort);

        /// <summary>
        /// 关闭声音
        /// </summary>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_StopSound();

        /// <summary>
        /// 以共享方式播放声音，播放本路声音而不去关闭其他路的声音
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_PlaySoundShare(int nPort);

        /// <summary>
        /// 以共享方式关闭声音。PLAY_PlaySound和PLAY_StopSound是以独占方式播放声音的。
        /// **注意：在同一个进程中，所有通道必须使用相同的方式播放或关闭声音**
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_StopSoundShare(int nPort);

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nVolume">音量的值，范围0-0XFFFF</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetVolume(int nPort, UInt32 nVolume);

        /// <summary>
        /// 获得当前设置的音量
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern UInt32 PLAY_GetVolume(int nPort);

        /// <summary>
        /// 调整WAVE波形，可以改变声音的大小。它和PLAY_SetVolume的不同在于，它是调整声音数据，
        /// 只对该路其作用，而PLAY_SetVolume是调整声卡音量，对整个系统起作用。该函数尚未实现
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nCoefficient">调整的参数，范围从MIN_WAVE_COEF 到 MAX_WAVE_COEF，0是不调整</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_AdjustWaveAudio(int nPort, int nCoefficient);

        #endregion

        #region << 播放流数据 >>

        /// <summary>
        /// 打开流接口（类似打开文件）
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="pFileHeadBuf">目前不使用，填IntPtr.Zero</param>
        /// <param name="nSize">目前不使用，填0</param>
        /// <param name="nBufPoolSize">
        /// 设置播放器中存放数据流的缓冲区大小
        /// 范围是DATABUF_SIZE.SOURCE_BUF_MIN~ DATABUF_SIZE.SOURCE_BUF_MAX
        /// 一般设为900*1024，如果数据送过来相对均匀，可调小该值，如果数据传输不均匀，可增大该值
        /// </param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        public static extern bool PLAY_OpenStream(int nPort, IntPtr pFileHeadBuf, UInt32 nSize, UInt32 nBufPoolSize);

        /// <summary>
        /// 输入从卡上得到的流数据；打开流并调用PLAY_Play之后才能输入数据
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="pBuf">缓冲区地址</param>
        /// <param name="nSize">缓冲区大小</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        public static extern bool PLAY_InputData(int nPort, IntPtr pBuf, UInt32 nSize);

        /// <summary>
        /// 关闭数据流（类似关闭文件）
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_CloseStream(int nPort);

        /// <summary>
        /// 以音视频分开输入的方式打开流
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="pFileHeadBuf">用户从卡上得到的文件头数据</param>
        /// <param name="nSize">文件头长度</param>
        /// <param name="nBufPoolSize">设置播放器中存放数据流的缓冲区大小。范围是SOURCE_BUF_MIN~ SOURCE_BUF_MAX</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_OpenStreamEx(int nPort, IntPtr pFileHeadBuf, UInt32 nSize, UInt32 nBufPoolSize);

        /// <summary>
        /// 输入从卡上得到的视频流 (可以是复合流，但音频数据会被忽略)；
        /// 打开流之后才能输入数据
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="pBuf">缓冲区地址</param>
        /// <param name="nSize">缓冲区大小</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_InputVideoData(int nPort, IntPtr pBuf, UInt32 nSize);

        /// <summary>
        /// 输入从卡上得到的音频流；打开声音之后才能输入数据
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="pBuf">缓冲区地址</param>
        /// <param name="nSize">缓冲区大小</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_InputAudioData(int nPort, IntPtr pBuf, UInt32 nSize);

        /// <summary>
        /// 关闭数据流
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_CloseStreamEx(int nPort);

        /// <summary>
        /// 揭秘接口
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_SetSecurityKey(int nPort, IntPtr szKey, UInt32 nKeylen);

        #endregion

        #region << 回放控制 >>

        /// <summary>
        /// 通道号
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nPause">1:暂停;0:恢复</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_Pause(int nPort, UInt32 nPause);

        /// <summary>
        /// 快速播放。播放速度分为九级，播放速度分别为每秒播放
        /// 1,3,6,12,25,50,75,100,125帧图像。每次调用播放速度提
        /// 升一级最多调 用4次，要恢复正常播放调用PLAY_Play,从当前位置开始正常播放
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_Fast(int nPort);

        /// <summary>
        /// 慢速播放，同上。每次调用播放速度降一级；最多调用4次，要恢复正常播放调用PLAY_Play
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
        private static extern bool PLAY_Slow(int nPort);

        /// <summary>
        /// 单帧播放。要恢复正常播放调用PLAY_ Play
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_OneByOne(int nPort);

        /// <summary>
        /// 单帧回放。每调用一次倒退一帧。此函数必须在文件索引生成之后才能调用
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_OneByOneBack(int nPort);

        #endregion

        #region << 获得错误号 >>

        /// <summary>
        /// 获得当前错误的错误码。用户应该在调用某个函数失败时，调用此函数以获得错误的详细信息。
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetLastError(int nPort);

        #endregion

        #region << 数据回调 >>

        /// <summary>
        /// 设置回调函数，替换播放器中的显示部分，由用户自己控制显示，该函数在PLAY_Play之前调用，
        /// 在PLAY_Stop时自动失效，下次调用PLAY_Play之前需要重新设置。
        /// 解码部分不控制速度，只要用户从回调函数中返回，解码器就会解码下一部分数据。
        /// 【注意】这个功能的使用需要用户对视频显示和声音播放有足够的了解，否则请慎重使用。
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="cbFun">回调函数指针</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_SetDecCallBack(int nPort,ref DecCBFun cbFun);

        /// <summary>
        /// 设置解码回调的流类型
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nStream">
        /// 1:视频流;
        /// 2:音频流;
        /// 3:复合流;
        /// </param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_SetDecCBStream(int nPort, UInt32 nStream);

        /// <summary>
        /// 设置视频图像数据回调，可用作抓图。如果要停止回调，可以把回调函数指针DisplayCBFun设为NULL。一旦设置回调函数，则一直有效，直到程序退出。该函数可以在任何时候调用
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="dCBFun">回调函数</param>
        /// <param name="nUser">用户自定义数据</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_SetDisplayCallBack(int nPort, DisplayCBFun dCBFun, int nUser);

        /// <summary>
        /// 音频帧解码后的wave数据回调
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="funAudio">音频回调函数</param>
        /// <param name="nUser">用户自定义数据</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_SetAudioCallBack(int nPort, FunAudio funAudio, int nUser);
                
        #endregion

        #region << 多区域显示 >>
        /// <summary>
        /// 设置或增加显示区域。可以做局部放大显示。 
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nRegionNum">显示区域序号，0~(MAX_DISPLAY_WND-1)。如果nRegionNum为0，表示对主要显示窗口中</param>
        /// <param name="pSrcRect">局部显示区域</param>
        /// <param name="hDestWnd">显示窗口句柄</param>
        /// <param name="bEnable">打开（设置）或关闭显示区域</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_SetDisplayRegion(int nPort, UInt32 nRegionNum,IntPtr pSrcRect, IntPtr hDestWnd, bool bEnable);
        //原C++风格的API定义：BOOL PLAY_SetDisplayRegion(LONG nPort,DWORD nRegionNum, RECT *pSrcRect, HWND hDestWnd, BOOL bEnable)

        /// <summary>
        /// 刷新显示，刷新多区域显示的窗口
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="nRegionNum">显示区域序号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_RefreshPlayEx(int nPort, UInt32 nRegionNum);
        //原C++风格的API定义：BOOL PLAY_RefreshPlayEx(LONG nPort,DWORD nRegionNum)
        #endregion

        #region << 获得属性 >>

        /// <summary>
        /// 得到当前播放的帧序号。而PLAY_GetPlayedFrames是总共解码的帧数。
        /// 如果文件播放位置不被改变，那么这两个函数的返回值应该非常接近，除非码流丢失数据。
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <returns>当前播放的帧序号</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetCurrentFrameNum(int nPort);

        /// <summary>
        /// 得到文件总的时间长度，单位秒
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <returns>文件总的时间长度值</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetFileTime(int nPort);

        /// <summary>
        /// 测试播放器需要的一些系统功能。
        /// </summary>
        /// <returns>属性值，按位取1~9位分别表示以下信息（位与是TRUE表示支持）：
        /// SUPPORT_DDRAW		   	支持DIRECTDRAW；如果不支持，则播放器不能工作。 
        /// SUPPORT_BLT			    显卡支持BLT操作；如果不支持，则播放器不能工作。 
        /// SUPPORT_BLTFOURCC		显卡BLT支持颜色转换；如果不支持，播放器会使用软件方式作RGB转换。 
        /// SUPPORT_BLTSHRINKX      显卡BLT支持X轴缩小；如果不支持，系统使用软件方式转换。 
        /// SUPPORT_BLTSHRINKY      显卡BLT支持Y轴缩小；如果不支持，系统使用软件方式转换。 
        /// SUPPORT_BLTSTRETCHX	    显卡BLT支持X轴放大；如果不支持，系统使用软件方式转换。 
        /// SUPPORT_BLTSTRETCHY	    显卡BLT支持Y轴放大；如果不支持，系统使用软件方式转换。 
        /// SUPPORT_SSE CPU		    支持SSE指令,Intel Pentium3以上支持SSE指令。 
        /// SUPPORT_MMX CPU		    支持MMX指令集。
        /// </returns>
        [DllImport("dhplay.dll")]
       private static extern Int16 PLAY_GetCaps();

        /// <summary>
        /// 相应的获得颜色值
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nRegionNum">显示区域，参考PLAY_SetDisplayRegion；如果只有一个显示区域(通常情况)设为0</param>
        /// <param name="pBrightness">亮度</param>
        /// <param name="pContrast">对比度</param>
        /// <param name="pSaturation">饱和度</param>
        /// <param name="pHue">色调</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_GetColor(int nPort, UInt32 nRegionNum, ref int pBrightness, ref int pContrast, ref int pSaturation, ref int pHue);

        /// <summary>
        /// 得到当前码流中编码时的帧率
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>当前码流中编码时的帧率值</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetCurrentFrameRate(int nPort);

        /// <summary>
        /// 得到文件中的总帧数
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>文件中的总帧数</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetFileTotalFrames(int nPort);

        /// <summary>
        /// 得到文件当前播放的时间，单位秒
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>文件当前播放的时间</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetPlayedTime(int nPort);

        /// <summary>
        /// 得到文件当前播放的时间，单位毫秒
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <returns>文件当前播放的时间</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetPlayedTimeEx(int nPort);

        /// <summary>
        /// 获得码流中原始图像的大小，根据此大小来设置显示窗口的区域，可以不用显卡做缩放工作，
        /// 对于那些不支持硬件缩放的显卡来说非常有用
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="pWidth">原始图像的宽。在PAL制CIF格式下是352</param>
        /// <param name="pHeight">原始图像的高。在PAL制CIF格式下是288</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_GetPictureSize(int nPort, ref int pWidth, ref int pHeight);

        #endregion

        #region << 获得版本号 >>
        /// <summary>
        /// 得到当前播放器sdk的主版本号、次版本号和补丁号
        /// </summary>
        /// <returns>
        /// 高16位表示当前的主版本号。
        /// 9~16位表示次版本号，1~8位表示次补丁号。
        /// 如：返回值0x00030107表示：主版本号是3，次版本号是1，补丁号是7</returns>
        [DllImport("dhplay.dll")]
       private static extern UInt32 PLAY_GetSdkVersion();

        #endregion

        #region << 抓图 >>

        /// <summary>
        /// 将抓图得到的图像数据保存成BMP文件。转换函数占用的cpu资源，如果不需要保存图片，则不要调用。
        /// </summary>
        /// <param name="pBuf">同抓图回调函数中的参数</param>
        /// <param name="nSize">同抓图回调函数中的参数</param>
        /// <param name="nWidth">同抓图回调函数中的参数</param>
        /// <param name="nHeight">同抓图回调函数中的参数</param>
        /// <param name="nType">同抓图回调函数中的参数</param>
        /// <param name="sFileName">要保存的文件名。最好以BMP作为文件扩展名</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_ConvertToBmpFile(ref string pBuf, int nSize, int nWidth, int nHeight, int nType,ref string  sFileName);

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="sFileName">文件名称</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_CatchPic(int nPort, string sFileName);

        #endregion

        #region << 数据流录像 >>
        
        /// <summary>
        /// 开始流数据录像。只对流模式有用，在PLAY_Play之后调用。
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="sFileName">录像文件名，如果文件名中有不存在的文件夹，就创建该文件夹</param>
        /// <param name="idataType">0:原始码流;1:AVI</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_StartDataRecord(int nPort,string sFileName,int idataType);

        /// <summary>
        /// 停止流数据录像
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_StopDataRecord(int nPort);

        #endregion

        #region << 字符叠加 >>

        /// <summary>
        /// 字符叠加
        /// </summary>
        /// <param name="nPort">播放器通道号</param>
        /// <param name="privateDrawFun">回调函数句柄</param>
        /// <param name="nUser">用户数据</param>
        /// <returns>true:成功;false:失败</returns>
        [DllImport("dhplay.dll")]
       private static extern bool PLAY_RigisterDrawFun(int nPort, DrawFun privateDrawFun, int nUser);

        #endregion

        #endregion
    }

    #region << 委托 >>

    /// <summary>
    /// 字符叠加回调函数
    /// </summary>
    /// <param name="nPort">通道号</param>
    /// <param name="hDc">OffScreen表面设备上下文，你可以像操作显示窗口客户区DC那样操作它</param>
    /// <param name="nUser">用户数据</param>
    public delegate void DrawFun(int nPort,IntPtr hDc,int nUser);

    /// <summary>
    /// 播放器数据回调
    /// </summary>
    /// <param name="nPort">播放器通道号</param>
    /// <param name="pBuf">解码后的音视频数据</param>
    /// <param name="nSize">解码后的音视频数据pBuf的长度</param>
    /// <param name="pFrameInfo">图像和声音的帧信息</param>
    /// <param name="nReserved1">保留参数</param>
    /// <param name="nReserved2">保留参数</param>
    public delegate void DecCBFun(int nPort, ref String pBuf, int nSize,ref  FRAME_INFO pFrameInfo, int nReserved1, int nReserved2);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nPort">播放器通道号</param>
    /// <param name="nUser">用户数据</param>
    public delegate void pFileRefDone (UInt32 nPort,IntPtr nUser);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nPort">播放器通道号</param>
    /// <param name="nUser">用户数据</param>
    public delegate void funEncChange (int nPort, int nUser);

    /// <summary>
    /// 设置视频图像数据的回调函数委托
    /// </summary>
    /// <param name="nPort">播放通道号</param>
    /// <param name="pBuf">返回图像数据</param>
    /// <param name="nSize">返回图像数据大小</param>
    /// <param name="nWidth">画面宽，单位像素</param>
    /// <param name="nHeight">画面高</param>
    /// <param name="nStamp">时标信息，单位毫秒</param>
    /// <param name="nType">数据类型， T_RGB32，T_UYVY，详见解码回调视频帧类型说明。</param>
    /// <param name="nReceaved">保留</param>
    public delegate void DisplayCBFun ( int  nPort,String pBuf,int nSize,int nWidth,int nHeight,int nStamp,int nType,int nReceaved);

    /// <summary>
    /// 音频帧解码后的wave数据回调委托
    /// </summary>
    /// <param name="nPort">播放通道号</param>
    /// <param name="pAudioBuf">格式音频数据</param>
    /// <param name="nSize">音频数据长度</param>
    /// <param name="nStamp">时标(ms)</param>
    /// <param name="nType">音频类型T_AUDIO16, 采样率8000，单声道，每个采样点16位表示</param>
    /// <param name="nUser">用户自定义数据</param>
    public delegate void FunAudio(int nPort,String pAudioBuf, int nSize, int nStamp, int nType, int nUser);

    #endregion

    #region << 结构定义 >>

    /// <summary>
    /// 帧位置
    /// </summary>
    public struct FRAME_POS
    {
        /// <summary>
        /// 指定帧在文件中的偏移位置
        /// </summary>
        public int nFilePos;
        /// <summary>
        /// 帧序号
        /// </summary> 
        public int nFrameNum;
        /// <summary>
        /// 帧时间
        /// </summary>
        public int nFrameTime;
        /// <summary>
        /// 错误帧号
        /// </summary>
        public SYSTEM_TIME nErrorFrameNum;
        /// <summary>
        /// 错误帧时间
        /// </summary>
        public int pErrorTime;
        /// <summary>
        /// 错误帧帧号
        /// </summary>
        public int nErrorLostFrameNum;
        /// <summary>
        /// 错误帧大小
        /// </summary>
        public int nErrorFrameSize;
    }

    /// <summary>
    /// 系统时间格式(C++中)
    /// </summary>
    public struct SYSTEM_TIME
    {
        /// <summary>
        /// 年
        /// </summary>
        public UInt16 wYear;
        /// <summary>
        /// 月
        /// </summary>
        public UInt16 wMonth;
        /// <summary>
        /// 星期
        /// </summary>
        public UInt16 wDayOfWeek;
        /// <summary>
        /// 日
        /// </summary>
        public UInt16 wDay;
        /// <summary>
        /// 时
        /// </summary>
        public UInt16 wHour;
        /// <summary>
        /// 分
        /// </summary>
        public UInt16 wMinute;
        /// <summary>
        /// 秒
        /// </summary>
        public UInt16 wSecond;
        /// <summary>
        /// 微秒
        /// </summary>
        public UInt16 wMilliseconds;
    }

    /// <summary>
    /// 自定义颜色结构
    /// </summary>
    public struct COLOR_STRUCT
    {
        /// <summary>
        /// 亮度[范围:0-128]
        /// </summary>
        public int pBrightness;
        /// <summary>
        /// 对比度[范围:0-128]
        /// </summary>
        public int pContrast;
        /// <summary>
        /// 饱和度[范围:0-128]
        /// </summary>
        public int pSaturation;
        /// <summary>
        /// 色调[范围:0-128]
        /// </summary>
        public int pHue;
 
    }

    /// <summary>
    /// 帧信息
    /// </summary>
    public struct FRAME_INFO
    {
        /// <summary>
        /// 画面宽，单位像素。如果是音频数据则为0
        /// </summary>
        public int nWidth;
        /// <summary>
        /// 画面高。如果是音频数据则为0
        /// </summary>
        public int nHeight;
        /// <summary>
        /// 时标信息，单位毫秒
        /// </summary>
        public int nStamp;
        /// <summary>
        /// 数据类型，T_AUDIO16，T_RGB32， T_YV12，详见宏定义说明。
        /// </summary>
        public int nType;
        /// <summary>
        /// 编码时产生的图像帧率
        /// </summary>
        public int nFrameRate;
    }

    /// <summary>
    /// 帧类型
    /// </summary>
    public struct FRAME_TYPE
    {
        /// <summary>
        /// 帧数据
        /// </summary>
        public string pDataBuf;
        /// <summary>
        /// 帧大小
        /// </summary>
        public int nSize;
        /// <summary>
        /// 帧序号
        /// </summary>
        public int nFrameNum;
        /// <summary>
        /// 是否音频帧(TRUE:1;FALSE:0)
        /// </summary>
        public Int16 bIsAudio;
        /// <summary>
        /// 保留字
        /// </summary>
        public int nReserved;
    }

    /// <summary>
    /// 错误内容
    /// </summary>
    public struct OPERATION_INFO
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string errCode;
        /// <summary>
        /// 错误描述
        /// </summary>
        public string errMessage;
        /// <summary>
        /// 按自定义格式返回错误内容字符串
        /// </summary>
        /// <param name="FormatStyle">错误内容字符串格式：errcode代替错误代码;errmsg代替错误描述</param>
        /// <returns>自定义格式的错误内容字符串</returns>
        public string ToString(string FormatStyle)
        {
            string returnValue = FormatStyle;
            if (returnValue.Length == 0)
            {
                returnValue = "errcode:errmsg!";
            }
            returnValue = returnValue.ToUpper();
            returnValue = returnValue.Replace("ERRCODE", errCode).Replace("ERRMSG", errMessage);
            return returnValue;

        }
    }

    ///// <summary>
    ///// 帧原始图像大小
    ///// </summary>
    //public struct FRAME_SIZE
    //{
    //    /// <summary>
    //    /// 帧原始图像大小:宽度
    //    /// </summary>
    //    public int Width;
    //    /// <summary>
    //    /// 帧原始图像大小:高度
    //    /// </summary>
    //    public int Height;
        
    //}


    #endregion

    #region << 枚举定义 >>

    /// <summary>
    /// 解码回调流类型
    /// </summary>
    public enum STREAM_TYPE
    { 
        /// <summary>
        /// 解码回调流:无
        /// </summary>
        STREAM_WITHOUT,
        /// <summary>
        /// 解码回调流:音频
        /// </summary>
        STREAM_AUDIO,
        /// <summary>
        /// 解码回调流:视频
        /// </summary>
        STREAM_VIDEO,
        /// <summary>
        /// 解码回调流:混合
        /// </summary>
        STREAM_MIX
    }

    /// <summary>
    /// 播放控制命令
    /// </summary>
    public enum PLAY_COMMAND
    {
        /// <summary>
        /// 开始播放
        /// </summary>
        Start,
        /// <summary>
        /// 暂停播放
        /// </summary>
        Pause,
        /// <summary>
        /// 继续播放
        /// </summary>
        ReSume,
        /// <summary>
        /// 停止播放
        /// </summary>
        Stop,
        /// <summary>
        /// 打开文件
        /// </summary>
        OpenFile,
        /// <summary>
        /// 关闭文件
        /// </summary>
        CloseFile,
        /// <summary>
        /// 快放(快速播放。播放速度分为九级，播放速度分别为每秒播放1,3,6,12,25,50,75,100,125帧图像。
        /// 每次调用播放速度提升一级最多调 用4次，要恢复正常播放调用PLAY_Play,从当前位置开始正常播放)
        /// </summary>
        Fast,
        /// <summary>
        /// 慢放(同快放，每次调用播放速度降一级；最多调用4次，要恢复正常播放调用PLAY_Play)
        /// </summary>
        Slow,
        /// <summary>
        /// 单帧(单帧播放。要恢复正常播放调用PLAY_ Play)
        /// </summary>
        OneByOne,
        /// <summary>
        /// 单帧回放(每调用一次倒退一帧,要恢复正常播放调用PLAY_ Play)
        /// </summary>
        OneByOneBack,
        /// <summary>
        /// 抓图
        /// </summary>
        CatchPic,
        /// <summary>
        /// 获得当前帧序号
        /// </summary>
        GetCurrentFrameNum,
        /// <summary>
        /// 设置当前播放位置到指定的帧号
        /// </summary>
        SetCurrentFrameNum,
        /// <summary>
        /// 获得当前帧率
        /// </summary>
        GetCurrentFrameRate,
        /// <summary>
        /// 获得文件中的总帧数
        /// </summary>
        GetFileTotalFrames,
        /// <summary>
        /// 获得文件总的时间长度,单位秒
        /// </summary>
        GetFileTotalTime,
        /// <summary>
        /// 得到文件当前的播放的时间,单位秒
        /// </summary>
        GetPlayedTime,
        /// <summary>
        /// 得到文件当前的播放的时间**单位:毫秒**
        /// </summary>
        GetPlayedTimeEx,
        /// <summary>
        /// 根据时间设置文件播放的位置
        /// </summary>
        SetPlayedTimeEx,
        /// <summary>
        /// 获得颜色值
        /// </summary>
        GetColor,
        /// <summary>
        /// 设置颜色属性
        /// </summary>
        SetColor,
        /// <summary>
        /// 播放声音[同一时刻只能有一路声音]
        /// </summary>
        PlaySound,
        /// <summary>
        /// 停止声音
        /// </summary>
        StopSound,
        /// <summary>
        /// 以共享方式播放声音[播放本路声音而不去关闭其他路的声音]
        /// </summary>
        PlaySoundShare,
        /// <summary>
        /// 以共享方式关闭声音
        /// </summary>
        StopSoundShare,
        /// <summary>
        /// 设置音量[范围:0-0XFFFF(0-65535)]
        /// </summary>
        SetVolume,
        /// <summary>
        /// 获得当前设置的音量
        /// </summary>
        GetVolume,
        /// <summary>
        /// 调整WAVE波形，可以改变声音的大小[它和SetVolume的不同在于，它是调整声音数据，只对该路其作用，
        /// 而SetVolume是调整声卡音量，对整个系统起作用]
        /// </summary>
        AdjustWaveAudio,
        /// <summary>
        /// 获得文件播放位置
        /// </summary>
        GetPlayPos,
        /// <summary>
        /// 设置文件播放位置
        /// </summary>
        SetPlayPos,
        /// <summary>
        /// 获取帧原始图像大小
        /// </summary>
        GetPictureSize,
        /// <summary>
        /// 定位到录像开头
        /// </summary>
        ToBegin,
        /// <summary>
        /// 定位到录像结束
        /// </summary>
        ToEnd,
        /// <summary>
        /// 打开流接口
        /// </summary>
        OpenStream,
        /// <summary>
        /// 输入从卡上得到的流数据；打开流并调用PLAY_Play之后才能输入数据
        /// </summary>
        InputData,
        /// <summary>
        /// 关闭数据流
        /// </summary>
        CloseStream,
        /// <summary>
        /// 以音视频分开输入的方式打开流
        /// </summary>
        OpenStreamEx,
        /// <summary>
        /// 输入从卡上得到的视频流 (可以是复合流，但音频数据会被忽略)
        /// 打开流之后才能输入数据
        /// </summary>
        InputVideoData,
        /// <summary>
        /// 输入从卡上得到的音频流；打开声音之后才能输入数据
        /// </summary>
        InputAudioData,
        /// <summary>
        /// 关闭数据流
        /// </summary>
        CloseStreamEx,
    }

    /// <summary>
    /// 系统功能
    /// </summary>
    public enum SYSFUN_TYPE
    {
        /// <summary>
        /// 支持DIRECTDRAW(如果不支持，则播放器不能工作)
        /// </summary>
        SUPPORT_DDRAW = 1,
        /// <summary>
        /// 显卡支持BLT操作(如果不支持，则播放器不能工作)
        /// </summary>
        SUPPORT_BLT = 2,
        /// <summary>
        /// 显卡BLT支持颜色转换
        /// </summary>
        SUPPORT_BLTFOURCC = 4,
        /// <summary>
        /// 显卡BLT支持X轴缩小
        /// </summary>
        SUPPORT_BLTSHRINKX = 8,
        /// <summary>
        /// 显卡BLT支持Y轴缩小
        /// </summary>
        SUPPORT_BLTSHRINKY = 16,
        /// <summary>
        /// 显卡BLT支持X轴放大
        /// </summary>
        SUPPORT_BLTSTRETCHX = 32,
        /// <summary>
        /// 显卡BLT支持Y轴放大
        /// </summary>
        SUPPORT_BLTSTRETCHY = 64,
        /// <summary>
        /// CPU支持SSE指令,Intel Pentium3以上支持SSE指令
        /// </summary>
        SUPPORT_SSE = 128,
        /// <summary>
        /// CPU支持MMX指令集
        /// </summary>
        SUPPORT_MMX = 256
    }

    /// <summary>
    /// 解码回调视频帧类型
    /// </summary>
    public enum VIDEO_TYPE
    {
        T_UYVY = 1,
        T_YV12 = 3,
        T_RGB32 = 7
    }

    /// <summary>
    /// 解码回调音频帧类型
    /// </summary>
    public enum AUDIO_TYPE
    {
        T_AUDIO16 = 101,
        T_AUDIO8 = 100
    }

    /// <summary>
    /// 数据流播放模式
    /// </summary>
    public enum PLAY_MODE
    {
        /// <summary>
        /// 最实时方式
        /// </summary>
        STREAME_REALTIME = 0,
        /// <summary>
        /// 最流畅方式
        /// </summary>
        STREAME_FILE = 1
    }

    /// <summary>
    /// 数据流原始缓冲大小
    /// </summary>
    public enum DATABUF_SIZE
    {
        /// <summary>
        /// 最大原始缓冲(1024*100000)
        /// </summary>
        SOURCE_BUF_MAX = 102400000,
        /// <summary>
        /// 最小原始缓冲(1024*50)
        /// </summary>
        SOURCE_BUF_MIN = 51200
    }

    /// <summary>
    /// 定位类型
    /// </summary>
    public enum GOTO_TYPE
    {
        /// <summary>
        /// 按帧号
        /// </summary>
        BY_FRAMENUM = 1,
        /// <summary>
        /// 按时间
        /// </summary>
        BY_FRAMETIME = 2
    }

    /// <summary>
    /// 解码缓冲帧数
    /// </summary>
    public enum BUF_FRAME_NUM
    {
        /// <summary>
        /// 最大解码缓冲帧数
        /// </summary>
        MAX_DIS_FRAMES = 50,
        /// <summary>
        /// 最小解码缓冲帧数
        /// </summary>
        MIN_DIS_FRAMES = 6
    }

    /// <summary>
    /// 显示类型和最大区域显示数
    /// </summary>
    public enum DISPLAY_MODE
    {
        /// <summary>
        /// 以正常分辨率显示
        /// </summary>
        DISPLAY_NORMAL = 1,
        /// <summary>
        /// 以四分之一分辨率显示
        /// </summary>
        DISPLAY_QUARTER = 2,
        /// <summary>
        /// 最大区域显示数(同时最多打开4个区域显示窗口)
        /// </summary>
        MAX_DISPLAY_WND = 4
    }

    /// <summary>
    /// 缓冲类型
    /// </summary>
    public enum BUF_TYPE
    {
        /// <summary>
        /// 视频源缓冲
        /// </summary>
        BUF_VIDEO_SRC = 1,
        /// <summary>
        /// 音频源缓冲
        /// </summary>
        BUF_AUDIO_SRC = 2,
        /// <summary>
        /// 解码后视频数据缓冲
        /// </summary>
        BUF_VIDEO_RENDER = 3,
        /// <summary>
        /// 解码后音频数据缓冲
        /// </summary>
        BUF_AUDIO_RENDER = 4
    }

    //未完成列表：定时期类型、错误类型、声音波型范围、最大通道数
    #endregion

    #endregion
}

