
/*
 * ************************************************************************
 * 
 * $Id: DaHuaSDKStruct.cs 7238 2012-12-17 00:02:20Z liu_hai $
 * 
 *                            SDK
 *                      大华网络SDK(C#版)
 * 
 * Copyright(c)1992-2012, ZheJiang Dahua Technology Stock Co.Ltd.
 *                      All Rights Reserved
 * 版 本 号:0.01
 * 文件名称:DaHuaSDKStruct.cs
 * 功能说明:原始封装[在现有的SDK(C++版)上再一次封装,基本与原C++接口对应]
 * 作    者:刘海
 * 作成日期:2012年5月26日
 * 修改日志:    日期          版本号        作者        变更事由
 *              2012年5月26日  0.01         刘海        新建作成
 * 
 * ************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Net;


namespace DHNetSDK
{

    #region << 结构定义 >>

    /// <summary>
    /// 网络设备信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NET_DEVICEINFO
    {
        /// <summary>
        /// 序列号[长度48]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] sSerialNumber;

        /// <summary>
        /// DVR报警输入个数
        /// </summary>
        public byte byAlarmInPortNum;

        /// <summary>
        /// DVR报警输出个数
        /// </summary>
        public byte byAlarmOutPortNum;

        /// <summary>
        /// DVR硬盘个数
        /// </summary>
        public byte byDiskNum;

        /// <summary>
        /// DVR类型
        /// </summary>
        public byte byDVRType;

        /// <summary>
        /// DVR通道个数
        /// </summary>
        public byte byChanNum;

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

        /// <summary>
        /// 将日期按格式转换
        /// </summary>
        /// <param name="FormatStyle">日期格式字符串:yyyy为年的值格式[固定四位]；mm为月的值格式[固定两位]；dd为日的值格式[固定两位]；d为日的值格式[不限定位数]；m为月的格式[不限定位数]；yy为年的格式[固定两位]；y为年的格式[不限定位数]；hh为时的值格式[固定两位]；h为时的值格式[不限定位数]；MM为分的值格式[固定两位]；M为分的值格式[不限定位数]；ss为秒的值格式[固定两位]；s为秒的值格式[不限定位数]；</param>
        /// <returns></returns>
        public string ToString(string FormatStyle)
        {
            string returnValue = FormatStyle;
            //年的格式处理
            string strY = "";
            if (returnValue.IndexOf("yyyy") != -1)
            {
                strY = "0000".Remove(4 - dwYear.ToString().Length) + dwYear.ToString();
                returnValue = returnValue.Replace("yyyy", strY);
            }
            else if (returnValue.IndexOf("yy") != -1)
            {
                if (dwYear.ToString().Length > 2)
                {
                    strY = dwYear.ToString().Substring(dwYear.ToString().Length - 2);
                    returnValue = returnValue.Replace("yy", strY);
                }
                else
                {
                    strY = "00".Remove(2 - dwYear.ToString().Length) + dwYear.ToString();
                    returnValue = returnValue.Replace("yy", strY);
                }
            }
            else if (returnValue.IndexOf("y") != -1)
            {
                strY = dwYear.ToString();
                returnValue = returnValue.Replace("y", strY);
            }
            //月的格式处理
            string strM = "";
            if (returnValue.IndexOf("mm") != -1)
            {
                strM = "00".Remove(2 - dwMonth.ToString().Length) + dwMonth.ToString();
                returnValue = returnValue.Replace("mm", strM);
            }
            else if (returnValue.IndexOf("m") != -1)
            {
                strM = dwMonth.ToString();
                returnValue = returnValue.Replace("m", strM);
            }
            //日的格式处理
            string strD = "";
            if (returnValue.IndexOf("dd") != -1)
            {
                strD = "00".Remove(2 - dwDay.ToString().Length) + dwDay.ToString();
                returnValue = returnValue.Replace("dd", strD);
            }
            else if (returnValue.IndexOf("d") != -1)
            {
                strD = dwDay.ToString();
                returnValue = returnValue.Replace("d", strD);
            }
            //时的格式处理
            string strH = "";
            if (returnValue.IndexOf("hh") != -1)
            {
                strH = "00".Remove(2 - dwHour.ToString().Length) + dwHour.ToString();
                returnValue = returnValue.Replace("hh", strH);
            }
            else if (returnValue.IndexOf("h") != -1)
            {
                strH = dwHour.ToString();
                returnValue = returnValue.Replace("h", strH);
            }
            //分的格式处理
            string strMM = "";
            if (returnValue.IndexOf("MM") != -1)
            {
                strMM = "00".Remove(2 - dwMinute.ToString().Length) + dwMinute.ToString();
                returnValue = returnValue.Replace("MM", strMM);
            }
            else if (returnValue.IndexOf("M") != -1)
            {
                strMM = dwMinute.ToString();
                returnValue = returnValue.Replace("M", strMM);
            }
            //秒的格式处理
            string strS = "";
            if (returnValue.IndexOf("ss") != -1)
            {
                strS = "00".Remove(2 - dwSecond.ToString().Length) + dwSecond.ToString();
                returnValue = returnValue.Replace("ss", strS);
            }
            else if (returnValue.IndexOf("s") != -1)
            {
                strS = dwSecond.ToString();
                returnValue = returnValue.Replace("s", strS);
            }
            return returnValue;
        }

        /// <summary>
        /// 转为标准备的系统时间格式
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            try
            {
                return new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
            }
            catch
            {
                return DateTime.Now;
            }
        }

    }

    /// <summary>
    /// 录像文件信息
    /// </summary>
    public struct NET_RECORDFILE_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public uint ch;
        /// <summary>
        /// 文件名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] filename;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 128)]
        public byte[] filename;
        //public string filename;
        /// <summary>
        /// 文件长度
        /// </summary>
        public uint size;
        /// <summary>
        /// 开始时间
        /// </summary>
        public NET_TIME starttime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public NET_TIME endtime;
        /// <summary>
        /// 磁盘号
        /// </summary>
        public uint driveno;
        /// <summary>
        /// 起始簇号
        /// </summary>
        public uint startcluster;
        /// <summary>
        /// 录像文件类型  0：普通录像；1：报警录
        /// </summary>
        public int nRecordFileType;
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
        /// <returns></returns>
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

    /// <summary>
    /// 设备状态信息
    /// </summary>
    public struct NET_CLIENT_STATE
    {
        /// <summary>
        /// 通道数
        /// </summary>
        public int channelcount;

        /// <summary>
        /// 报警输入数
        /// </summary>
        public int alarminputcount;

        /// <summary>
        /// 外部报警
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] alarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] alarm;

        /// <summary>
        /// 动态检测
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] motiondection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] motiondection;

        /// <summary>
        /// 视频丢失
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] videolost;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] videolost;
    }

    /// <summary>
    /// 设备通道状态信息
    /// </summary>
    public struct NET_DEV_CHANNELSTATE
    {
        /// <summary>
        /// 通道是否在录像,0-不录像,1-录像
        /// </summary>
        public byte byRecordStatic;
        /// <summary>
        /// 连接的信号状态,0-正常,1-信号丢失
        /// </summary>
        public byte bySignalStatic;
        /// <summary>
        /// 通道硬件状态,0-正常,1-异常,例如DSP死掉
        /// </summary>
        public byte byHardwareStatic;
        /// <summary>
        /// 暂时无效
        /// </summary>
        public char reserve;
        /// <summary>
        /// 实际码率,暂时无效
        /// </summary>
        public UInt32 dwBitRate;
        /// <summary>
        /// 客户端连接的个数, 暂时无效
        /// </summary>
        public UInt32 dwLinkNum;
        /// <summary>
        /// 客户端的IP地址,暂时无效
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public UInt32[] dwClientIP;
    }

    /// <summary>
    /// 设备硬盘状态信息
    /// </summary>
    public struct NET_DEV_DISKSTATE
    {
        /// <summary>
        /// 硬盘的容量
        /// </summary>
        public UInt32 dwVolume;
        /// <summary>
        /// 硬盘的剩余空间
        /// </summary>
        public UInt32 dwFreeSpace;
        /// <summary>
        /// 硬盘的状态,休眠,活动,不正常等
        /// </summary>
        public UInt32 dwStatus;
    }

    /// <summary>
    /// 设备工作状态信息
    /// </summary>
    public struct NET_DEV_WORKSTATE
    {
        /// <summary>
        /// 设备状态0x00 正常,0x01 CPU占用过高, 0x02 硬件错误
        /// </summary>
        public UInt32 dwDeviceStatic;
        /// <summary>
        /// 设备暂时不支持
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public NET_DEV_DISKSTATE[] stHardDiskStatic;
        /// <summary>
        /// 通道的状态
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public NET_DEV_CHANNELSTATE[] stChanStatic;
        /// <summary>
        /// 报警端口的状态0-无报警,1-有报警
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] byAlarmInStatic;
        /// <summary>
        /// 报警输出端口的状态0-无输出,1-有输出
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] byAlarmOutStatic;
        /// <summary>
        /// 本地显示状态0-正常,1-不正常
        /// </summary>
        public UInt32 dwLocalDisplay;
    }

    /// <summary>
    /// 串口协议信息(232和485
    /// </summary>
    public struct PROTOCOL_INFO
    {
        /// <summary>
        /// 协议名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 12)]
        //public char[] protocolname;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 12)]
        public byte[] protocolname;
        /// <summary>
        /// 波特率
        /// </summary>
        public uint baudbase;
        /// <summary>
        /// 数据位
        /// </summary>
        public char databits;
        /// <summary>
        /// 停止位
        /// </summary>
        public char stopbits;
        /// <summary>
        /// 校验位
        /// </summary>
        public char parity;
        /// <summary>
        /// 暂不支持
        /// </summary>
        public char reserve;
    }

    /// <summary>
    /// 报警IO控制(报警输出和报警输入使能)
    /// </summary>
    public struct ALARM_CONTROL
    {
        /// <summary>
        /// 端口序号
        /// </summary>
        public ushort index;
        /// <summary>
        /// 端口状态
        /// </summary>
        public ushort state;
    }

    
    public struct OPR_RIGHT
    {
        public UInt32 dwID;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] name;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] memo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] memo;
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public struct USER_INFO
    {
        public UInt32 dwID;
        public UInt32 dwGroupID;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 8)]
        //public char[] name;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 8)]
        public byte[] name;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 8)]
        //public char[] passWord;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] passWord;
        public UInt32 dwRightNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public UInt32[] rights;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] memo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] memo;
        /// <summary>
        /// 本用户是否允许复用:1.复用;0:不复用;
        /// </summary>
        public UInt32 dwReusable;
        public USER_INFO(string st)
        {
            dwID = new UInt32();
            dwGroupID = new UInt32();
            dwRightNum = new UInt32();
            dwReusable = new UInt32();
            name = new byte[8];
            passWord = new byte[8];
            rights = new UInt32[100];
            memo = new byte[32];
        }
    }

    /// <summary>
    /// 用户组结构
    /// </summary>
    public struct USER_GROUP_INFO
    {
        public UInt32 dwID;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 8)]
        //public char[] name;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 8)]
        public byte[] name;
        public UInt32 dwRightNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public UInt32[] rights;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] memo;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] memo;
        public USER_GROUP_INFO(string strP)
        {
            dwID = new UInt32();
            name = new byte[8];
            dwRightNum = new UInt32();
            rights = new UInt32[100];
            memo = new byte[32];
        }
    }


    /// <summary>
    /// 用户信息配置结构
    /// </summary>
    public struct USER_MANAGE_INFO
    {
        public UInt32 dwRightNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public OPR_RIGHT[] rightList;
        public UInt32 dwGroupNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public USER_GROUP_INFO[] groupList;
        public UInt32 dwUserNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public USER_INFO[] userList;
        /// <summary>
        /// 特殊信息:1.支持用户复用;0.不支持用户复用;
        /// </summary>
        public UInt32 dwSpecial;
    }

    /// <summary>
    /// 日志结构
    /// </summary>
    public struct DH_LOG_ITEM
    {
        /// <summary>
        /// 日期
        /// </summary>
        public NET_TIME time;
        /// <summary>
        /// 类型
        /// </summary>
        public UInt16 type;
        /// <summary>
        /// 保留
        /// </summary>
        public byte reserved;
        /// <summary>
        /// 数据
        /// </summary>
        public byte data;
        /// <summary>
        /// 内容
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] context;
    }


    /// <summary>
    /// 查询硬盘信息的返回数据结构
    /// </summary>
    public struct DH_HARDDISK_STATE
    {
        /// <summary>
        /// 硬盘个数
        /// </summary>
        public UInt32 dwDiskNum;
        /// <summary>
        /// 各硬盘信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public NET_DEV_DISKSTATE[] stDisks;
    }

    /// <summary>
    /// 音频数据的格式结构
    /// </summary>
    public struct DH_AUDIO_FORMAT
    {
        /// <summary>
        /// 编码类型，0-PCM
        /// </summary>
        public byte byFormatTag;
        /// <summary>
        /// 声道数
        /// </summary>
        public UInt16 nChannels;
        /// <summary>
        /// 采样深度
        /// </summary>
        public UInt16 wBitsPerSample;
        /// <summary>
        /// 采样率
        /// </summary>
        public UInt32 nSamplesPerSec;
    }

    /// <summary>
    /// 版本信息
    /// 关于时间的数据组织是：yyyymmdd
    /// </summary>
    public struct DH_VERSION_INFO
    {
        /// <summary>
        /// 版本号:高16位表示主版本号，低16位表示次版本号
        /// </summary>
        public UInt32 dwSoftwareVersion;
        /// <summary>
        /// 创建时间
        /// </summary>
        public UInt32 dwSoftwareBuildDate;
        /// <summary>
        /// DSP版本号
        /// </summary>
        public UInt32 dwDspSoftwareVersion;
        /// <summary>
        /// DSP版本创建时间
        /// </summary>
        public UInt32 dwDspSoftwareBuildDate;
        /// <summary>
        /// 面板版本
        /// </summary>
        public UInt32 dwPanelVersion;
        /// <summary>
        /// 面板软件创建日期
        /// </summary>
        public UInt32 dwPanelSoftwareBuildDate;
        /// <summary>
        /// 硬件版本
        /// </summary>
        public UInt32 dwHardwareVersion;
        /// <summary>
        /// 硬件制作日期
        /// </summary>
        public UInt32 dwHardwareDate;
        /// <summary>
        /// Web版本
        /// </summary>
        public UInt32 dwWebVersion;
        /// <summary>
        /// Web创建日期
        /// </summary>
        public UInt32 dwWebBuildDate;
    }



    /// <summary>
    /// DSP能力描述
    /// </summary>
    public struct DH_DSP_ENCODECAP
    {
        /// <summary>
        /// 视频制式掩码，按位表示设备能够支持的视频制式
        /// </summary>
        public UInt32 dwVideoStandardMask;
        /// <summary>
        /// 分辨率掩码，按位表示设备能够支持的分辨率设置
        /// </summary>
        public UInt32 dwImageSizeMask;
        /// <summary>
        /// 编码模式掩码，按位表示设备能够支持的编码模式设置
        /// </summary>
        public UInt32 dwEncodeModeMask;
        /// <summary>
        /// 按位表示设备支持的多媒体功能 第一位表示支持主码流,第二位表示支持辅码流1,第三位表示支持辅码流2,第四位表示支持jpg抓图
        /// </summary>
        public UInt32 dwStreamCap;
        /// <summary>
        /// 表示主码流为各分辨率时，支持的辅码流分辨率掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt32[] dwImageSizeMask_Assi;
        /// <summary>
        /// DSP 支持的最高编码能力
        /// </summary>
        public UInt32 dwMaxEncodePower;
        /// <summary>
        /// 每块 DSP 支持最多输入视频通道数 
        /// </summary>
        public UInt16 wMaxSupportChannel;
        /// <summary>
        /// DSP 每通道的最大编码设置是否同步 0-不同步, 1-同步
        /// </summary>
        public UInt16 wChannelMaxSetSync;
    }

    /// <summary>
    /// 设备信息配置
    /// </summary>
    public struct DHDEV_SYSTEM_ATTR_CFG
    {
        /// <summary>
        /// 
        /// </summary>
        public UInt32 dwSize;
        /*下面是设备的只读部分*/
        /// <summary>
        /// 版本
        /// </summary>
        public DH_VERSION_INFO stVersion;
        /// <summary>
        /// DSP能力描述
        /// </summary>
        public DH_DSP_ENCODECAP stDspEncodeCap;
        /// <summary>
        /// 序列号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] szDevSerialNo;
        /// <summary>
        /// 序列号转成字符串
        /// </summary>
        /// <returns></returns>
        public string DevSerialNo()
        {
            string result = "";
            foreach (byte bt in szDevSerialNo)
            {
                result += bt.ToString("D");
            }
            return result;
        }
        /// <summary>
        /// 设备类型，见枚举NET_DEVICE_TYPE
        /// </summary>
        public byte byDevType;
        /// <summary>
        /// 显示标准备的设备类型描述
        /// </summary>
        /// <returns></returns>
        public string DevType()
        {
            string result = "";
            switch(int.Parse(byDevType.ToString()))
            {
                case (int)NET_DEVICE_TYPE.NET_DVR_DDNS:
                    result = "DDNS服务器";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MEPG4_SH2:
                    result = "MPEG4视豪录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MEPG4_ST2:
                    result = "MPEG4视通录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG1_2:
                    result = "MPEG1二路录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG1_8:
                    result = "MPEG4八路录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG4_16:
                    result = "MPEG4十六录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG4_8:
                    result = "MPEG4八路录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG4_GBE:
                    result = "MPEG4视通二代增强型录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG4_NVSII:
                    result = "MPEG4网络视频服务器II代";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_MPEG4_SX2:
                    result = "MPEG4视新十六路录像机";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_NONREALTIME:
                    result = "非实时";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_NONREALTIME_MACE:
                    result = "非实时MACE";
                    break;
                case (int)NET_DEVICE_TYPE.NET_DVR_STD_NEW:
                    result = "新标准配置协议";
                    break;
                case (int)NET_DEVICE_TYPE.NET_NVS_MPEG1:
                    result = "网络视频服务器";
                    break;
                case (int)NET_DEVICE_TYPE.NET_PRODUCT_NONE:
                    result = "无";
                    break;
            }
            return result;
        }
        /// <summary>
        /// 设备详细型号，字符串格式，可能为空
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szDevType;
        /// <summary>
        /// 视频口数量
        /// </summary>
        public byte byVideoCaptureNum;
        /// <summary>
        /// 音频口数量
        /// </summary>
        public byte byAudioCaptureNum;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byTalkInChanNum;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byTalkOutChanNum;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byDecodeChanNum;
        /// <summary>
        /// 报警输入口数
        /// </summary>
        public byte byAlarmInNum;
        /// <summary>
        /// 报警输出口数
        /// </summary>
        public byte byAlarmOutNum;
        /// <summary>
        /// 网络口数
        /// </summary>
        public byte byNetIONum;
        /// <summary>
        /// USB口数量
        /// </summary>
        public byte byUsbIONum;
        /// <summary>
        /// IDE数量
        /// </summary>
        public byte byIdeIONum;
        /// <summary>
        /// 串口数量
        /// </summary>
        public byte byComIONum;
        /// <summary>
        /// 并口数量
        /// </summary>
        public byte byLPTIONum;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byVgaIONum;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byIdeControlNum;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byIdeControlType;
        /// <summary>
        /// NSP，扩展描述
        /// </summary>
        public byte byCapability;
        /// <summary>
        /// 视频矩阵输出口数
        /// </summary>
        public byte byMatrixOutNum;
        /*下面是设备的可写部分*/
        /// <summary>
        /// 硬盘满处理方式（覆盖、停止）
        /// </summary>
        public byte byOverWrite;
        /// <summary>
        /// 录像打包长度
        /// </summary>
        public byte byRecordLen;
        /// <summary>
        /// NSP
        /// </summary>
        public byte byStartChanNo;
        /// <summary>
        /// 设备编号，用于遥控
        /// </summary>
        public UInt16 wDevNo;
        /// <summary>
        /// 视频制式
        /// </summary>
        public byte byVideoStandard;
        /// <summary>
        /// 日期格式
        /// </summary>
        public byte byDateFormat;
        /// <summary>
        /// 日期分割符(0-"."， 1-"-"， 2-"/")
        /// </summary>
        public byte byDateSprtr;
        /// <summary>
        /// 时间格式 (0-24小时，1－12小时)
        /// </summary>
        public byte byTimeFmt;
        /// <summary>
        /// 保留字
        /// </summary>
        public byte byReserved;
    }

    /// <summary>
    /// 时间段结构  
    /// </summary>
    public struct DH_TSECT
    {
        public bool bEnable;
        public int iBeginHour;
        public int iBeginMin;
        public int iBeginSec;
        public int iEndHour;
        public int iEndMin;
        public int iEndSec;
    }

    /// <summary>
    /// 时间段结构[长度6]
    /// </summary>
    public struct DH_REC_TSECT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public DH_TSECT[] sTSECT;
    }
    
    /// <summary>
    /// 区域:各边距按整长8192的比例 
    /// </summary>
    public struct DH_RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// OSD属性结构 
    /// </summary>
    public struct DH_ENCODE_WIDGET
    {
        /// <summary>
        /// 物件的前景RGB，和透明度  
        /// </summary>
        public UInt32 rgbaFrontground;
        /// <summary>
        /// 物件的后景RGB，和透明度 
        /// </summary>
        public UInt32 rgbaBackground;
        /// <summary>
        /// 位置  
        /// </summary>
        public DH_RECT rcRect;
        /// <summary>
        /// 物件显示
        /// </summary>
        public byte bShow;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
    }

    /// <summary>
    /// 通道音视频属性 
    /// </summary>
    public struct DH_VIDEOENC_OPT
    {
        //视频参数
        /// <summary>
        /// 视频使能:1－打开，0－关闭 
        /// </summary>
        public byte byVideoEnable;
        /// <summary>
        /// 码流控制,参照常量定义
        /// </summary>
        public byte byBitRateControl;
        /// <summary>
        /// 帧率
        /// </summary>
        public byte byFramesPerSec;
        /// <summary>
        /// 编码模式,参照常量定义 
        /// </summary>
        public byte byEncodeMode;
        /// <summary>
        /// 分辨率参,参照常量定义 
        /// </summary>
        public byte byImageSize;
        /// <summary>
        /// 档次1-6 
        /// </summary>
        public byte byImageQlty;
        /// <summary>
        /// 限码流参数, 范围：50~4*1024 (k)
        /// </summary>
        public UInt16 wLimitStream;

        //音频参数
        /// <summary>
        /// 音频使能:1－打开，0－关闭
        /// </summary>
        public byte byAudioEnable;
        /// <summary>
        /// 编码类型，如PCM
        /// </summary>
        public byte wFormatTag;
        /// <summary>
        /// 声道数
        /// </summary>
        public UInt16 nChannels;
        /// <summary>
        /// 采样深度	
        /// </summary>
        public UInt16 wBitsPerSample;
        /// <summary>
        /// 采样率
        /// </summary>
        public UInt32 nSamplesPerSec;
        /// <summary>
        /// 保留字
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
    }

    /// <summary>
    /// 画面颜色属性
    /// </summary>
    public struct DH_COLOR_CFG
    {
        public DH_TSECT stSect;
        /// <summary>
        /// 亮度	0-100
        /// </summary>
        public byte byBrightness;
        /// <summary>
        /// 对比度	0-100
        /// </summary>
        public byte byContrast;
        /// <summary>
        /// 饱和度	0-100
        /// </summary>
        public byte bySaturation;
        /// <summary>
        /// 色度	0-100
        /// </summary>
        public byte byHue;
        /// <summary>
        /// 增益使能	  
        /// </summary>
        public byte byGainEn;
        /// <summary>
        /// 增益	0-100
        /// </summary>
        public byte byGain;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved;
    }



    /// <summary>
    /// 图像通道属性结构体
    /// </summary>
    public struct DHDEV_CHANNEL_CFG
    {
        public UInt32 dwSize;
        /// <summary>
        /// 
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szChannelName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szChannelName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public DH_VIDEOENC_OPT[] stMainVideoEncOpt;
        /// <summary>
        ///  通常指网传码流	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public DH_VIDEOENC_OPT[] stAssiVideoEncOpt;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public DH_COLOR_CFG[] stColorCfg;
        public DH_ENCODE_WIDGET stTimeOSD;
        public DH_ENCODE_WIDGET stChannelOSD;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public DH_ENCODE_WIDGET[] stBlindCover;

        /// <summary>
        ///  区域遮盖开关,0x00不使能遮盖，0x01仅遮盖,设备本地预览，0x10仅遮盖录像（及网络预览），0x11都遮盖
        ///  </summary>
        public byte byBlindEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
    }

    /// <summary>
    /// 预览图像参数
    /// </summary>
    public struct DHDEV_PREVIEW_CFG
    {
        public UInt32 dwSize;
        public DH_VIDEOENC_OPT stPreView;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public DH_COLOR_CFG[] stColorCfg;
    }

    /// <summary>
    /// 语音对讲音频属性
    /// </summary>
    public struct DHDEV_TALK_CFG
    {
        #region << 音频输入参数 >>
        /// <summary>
        /// 编码类型，如PCM
        /// </summary>
        public byte byInFormatTag;
        /// <summary>
        /// 声道数
        /// </summary>
        public byte byInChannels;
        /// <summary>
        /// 采样深度	
        /// </summary>
        public UInt16 wInBitsPerSample;
        /// <summary>
        /// 采样率
        /// </summary>
        public UInt32 dwInSamplesPerSec;

        #endregion

        #region << 音频输出参数 >>
        /// <summary>
        /// 编码类型，如PCM
        /// </summary>
        public byte byOutFormatTag;
        /// <summary>
        /// 声道数
        /// </summary>
        public byte byOutChannels;
        /// <summary>
        /// 采样深度
        /// </summary>
        public UInt16 wOutBitsPerSample;
        /// <summary>
        /// 采样率
        /// </summary>
        public UInt32 dwOutSamplesPerSec;
        #endregion

    }

    /// <summary>
    /// 定时录像
    /// </summary>
    public struct DHDEV_RECORD_CFG
    {
        
        public UInt32 dwSize;
        /// <summary>
        /// 时间段结构
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stSect;
        /// <summary>
        /// 预录时间,单位是s,0表示不预录 
        /// </summary>
        public byte byPreRecordLen;
        /// <summary>
        /// 录像冗余开关
        /// </summary>
        public byte byRedundancyEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved;
    }

    /// <summary>
    /// 报警配置
    /// </summary>
    public struct DH_PTZ_LINK
    {
        public int iType;
        public int iValue;
    }

    /// <summary>
    /// 消息触发配置
    /// 消息处理方式,可以同时多种处理方式,包括
    /// 0x00000001 - 网络:上传管理服务器
    /// 0x00000002 - 录像:触发
    /// 0x00000004 - 云台联动
    /// 0x00000008 - 发送邮件
    /// 0x00000010 - 设备本地报警轮巡
    /// 0x00000020 - 设备提示使能
    /// 0x00000040 - 设备报警输出使能
    /// 0x00000080 - Ftp上传使能
    /// 0x00000100 - 蜂鸣
    /// 0x00000200 - 语音提示
    /// 0x00000400 - 抓图使能
    /// </summary>
    public struct DH_MSG_HANDLE
    {
        /// <summary>
        /// 当前报警所支持的处理方式，按位掩码表示
        /// </summary>
        public UInt32 dwActionMask;
        /// <summary>
        /// 触发动作，按位掩码表示，具体动作所需要的参数在各自的配置中体现
        /// </summary>
        public UInt32 dwActionFlag;
        /// <summary>
        /// 报警触发的输出通道,为 1 表示触发该输出
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] byRelAlarmOut;
        /// <summary>
        /// 报警持续时间
        /// </summary>
        public UInt32 dwDuration;
        /// <summary>
        /// 联动录像	, 报警触发的录像通道,为1表示触发该通道	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] byRecordChannel;
        /// <summary>
        /// 录像持续时间 
        /// </summary>
        public UInt32 dwRecLatch;
        /// <summary>
        /// 抓图通道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] bySnap;
        /// <summary>
        /// 轮巡通道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] byTour;
        /// <summary>
        /// 云台联动
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_PTZ_LINK[] struPtzLink;
    }

    /// <summary>
    /// 外部报警
    /// </summary>
    public struct DH_ALARMIN_CFG
    {
        /// <summary>
        /// 报警器类型,0：常闭,1：常开  	
        /// </summary>
        public byte byAlarmType;
        /// <summary>
        /// 报警使能
        /// </summary>
        public byte byAlarmEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stSect;
        /// <summary>
        /// 处理方式
        /// </summary>
        public DH_MSG_HANDLE struHandle;
    }

    /// <summary>
    /// 检测区域[长度16]
    /// </summary>
    public struct DH_DETECT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =32)]
        public byte[] Detected;
    }

    /// <summary>
    /// 动态检测
    /// </summary>
    public struct DH_MOTION_DETECT_CFG
    {
        /// <summary>
        /// 动态检测报警使能
        /// </summary>
        public byte byMotionEn;

        public byte byReserved;
        /// <summary>
        /// 灵敏度
        /// </summary>
        public UInt16 wSenseLevel;
        /// <summary>
        /// 动态检测区域的行数
        /// </summary>
        public UInt16 wMotionRow;
        /// <summary>
        /// 动态检测区域的列数
        /// </summary>
        public UInt16 wMotionCol;
        /// <summary>
        /// 检测区域，共32*32块区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public DH_DETECT[] byDetected;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stSect;
        /// <summary>
        /// 处理方式
        /// </summary>
        public DH_MSG_HANDLE struHandle;
    }

    /// <summary>
    /// 视频丢失报警
    /// </summary>
    public struct DH_VIDEO_LOST_CFG
    {
        /// <summary>
        /// 视频丢失报警使能
        /// </summary>
        public byte byAlarmEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stSect;
        /// <summary>
        /// 处理方式
        /// </summary>
        public DH_MSG_HANDLE struHandle;
    }

    /// <summary>
    /// 图像遮挡报警
    /// </summary>
    public struct DH_BLIND_CFG
    {
        /// <summary>
        /// 使能
        /// </summary>
        public byte byBlindEnable;
        /// <summary>
        /// 灵敏度1-6 
        /// </summary>
        public byte byBlindLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stSect;
        /// <summary>
        /// 处理方式
        /// </summary>
        public DH_MSG_HANDLE struHandle;
    }

    /// <summary>
    /// 硬盘消息(内部报警)
    /// </summary>
    public struct DH_DISK_ALARM_CFG
    {
        /// <summary>
        /// 无硬盘时报警
        /// </summary>
        public byte byNoDiskEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved_1;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stNDSect;
        /// <summary>
        /// 处理方式 
        /// </summary>
        public DH_MSG_HANDLE struNDHandle;
        /// <summary>
        /// 硬盘低容量时报警
        /// </summary>
        public byte byLowCapEn;
        /// <summary>
        /// 容量阀值 0-99
        /// </summary>
        public byte byLowerLimit;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved_2;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stLCSect;
        /// <summary>
        /// 处理方式  
        /// </summary>
        public DH_MSG_HANDLE struLCHandle;
        /// <summary>
        /// 硬盘故障报警
        /// </summary>
        public byte byDiskErrEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved_3;
        /// <summary>
        /// NSP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public DH_REC_TSECT[] stEDSect;
        /// <summary>
        /// 处理方式 
        /// </summary>
        public DH_MSG_HANDLE struEDHandle;
    }

    public struct DH_NETBROKEN_ALARM_CFG
    {
        public byte byEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
        public DH_MSG_HANDLE struHandle;
    }

    /// <summary>
    /// 报警布防
    /// </summary>
    public struct DHDEV_ALARM_SCHEDULE
    {
        public UInt32 dwSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_ALARMIN_CFG[] struLocalAlmIn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_ALARMIN_CFG[] struNetAlmIn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_MOTION_DETECT_CFG[] struMotion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_VIDEO_LOST_CFG[] struVideoLost;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_BLIND_CFG[] struBlind;
        public DH_DISK_ALARM_CFG struDiskAlarm;
        public DH_NETBROKEN_ALARM_CFG struNetBrokenAlarm;
    }


    // 报警联动扩展结构体
    public struct DH_MSG_HANDLE_EX
    {
	    /* 消息处理方式，可以同时多种处理方式，包括
	     * 0x00000001 - 报警上传
	     * 0x00000002 - 联动录象
	     * 0x00000004 - 云台联动
	     * 0x00000008 - 发送邮件
	     * 0x00000010 - 本地轮巡
	     * 0x00000020 - 本地提示
	     * 0x00000040 - 报警输出
	     * 0x00000080 - Ftp上传
	     * 0x00000100 - 蜂鸣
	     * 0x00000200 - 语音提示
	     * 0x00000400 - 抓图
	    */

        /// <summary>
        /// 当前报警所支持的处理方式，按位掩码表示
        /// </summary>
	    public UInt32				dwActionMask;

        /// <summary>
        /// 触发动作，按位掩码表示，具体动作所需要的参数在各自的配置中体现
        /// </summary>
	    public UInt32				dwActionFlag;
    	
        /// <summary>
        /// 报警触发的输出通道，报警触发的输出，为1表示触发该输出
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]				byRelAlarmOut;
        /// <summary>
        /// 报警持续时间
        /// </summary>
	    public UInt32				dwDuration;

        /// <summary>
        /// 联动录象
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]				byRecordChannel; /* 报警触发的录象通道，为1表示触发该通道 */
        /// <summary>
        /// 录象持续时间
        /// </summary>
	    public UInt32				dwRecLatch;

        /// <summary>
        /// 抓图通道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]				bySnap;
        /// <summary>
        /// 轮巡通道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]				byTour;

        /// <summary>
        /// 云台联动
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public DH_PTZ_LINK[]			struPtzLink;
        /// <summary>
        /// 联动开始延时时间，s为单位，范围是0~15，默认值是0
        /// </summary>
	    public UInt32				dwEventLatch;
        /// <summary>
        /// 报警触发的无线输出通道，报警触发的输出，为1表示触发该输出
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]			byRelWIAlarmOut;
	    public byte				bMessageToNet;
        /// <summary>
        /// 短信报警使能
        /// </summary>
	    public byte                bMMSEn;
        /// <summary>
        /// 短信发送抓图张数
        /// </summary>
	    public byte                bySnapshotTimes;
        /// <summary>
        /// 矩阵使能
        /// </summary>
	    public byte				bMatrixEn;
        /// <summary>
        /// 矩阵掩码
        /// </summary>
	    public UInt32				dwMatrix;
	    /// <summary>
	    /// 日志使能，目前只有在WTN动态检测中使用
	    /// </summary>
	    public byte				bLog;
        /// <summary>
        /// 抓图帧间隔，每隔多少帧抓一张图片，一定时间内抓拍的张数还与抓图帧率有关。0表示不隔帧，连续抓拍。
        /// </summary>
	    public byte				bSnapshotPeriod;
        /// <summary>
        /// 轮巡通道 32-63路
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]				byTour2;
        /// <summary>
        /// 0，图片附件，1，录像附件
        /// </summary>
	    public byte                byEmailType;
        /// <summary>
        /// 附件录像时的最大长度，单位MB
        /// </summary>
	    public byte                byEmailMaxLength;
        /// <summary>
        /// 附件是录像时最大时间长度，单位秒
        /// </summary>
	    public byte                byEmailMaxTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 475)]
	    public byte[]				byReserved;   
    }



    /// <summary>
    /// 外部报警扩展
    /// </summary>
    public struct DH_ALARMIN_CFG_EX
    {
        /// <summary>
        /// 报警器类型，0：常闭，1：常开
        /// </summary>
	    public byte				byAlarmType;
        /// <summary>
        /// 报警使能
        /// </summary>
	    public byte				byAlarmEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public byte[]				byReserved;				
        /// <summary>
        /// NSP
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7*6)]
	    public DH_TSECT[]			stSect;
        /// <summary>
        /// 处理方式
        /// </summary>
	    public DH_MSG_HANDLE_EX	struHandle;
    }

    /// <summary>
    /// 以太网配置
    /// </summary>
    public struct DH_ETHERNET
    {
        /// <summary>
        /// DVR IP 地址  
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sDevIPAddr;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 16)]
        public byte[] sDevIPAddr;
        /// <summary>
        /// DVR IP 地址掩码
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sDevIPMask;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sDevIPMask;
        /// <summary>
        ///  网关地址   
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sGatewayIP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sGatewayIP;
        /// <summary>
        /// NSP
        /// 10M/100M  自适应,索引 
        /// 1-10MBase - T
        /// 2-10MBase-T 全双工 
        /// 3-100MBase - TX
        /// 4-100M 全双工
        /// 5-10M/100M  自适应 
        /// </summary>
        public UInt32 dwNetInterface;
        /// <summary>
        /// MAC地址，只读   
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 40)]
        //public char[] byMACAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] byMACAddr;
    }

    /// <summary>
    /// 远程主机配置
    /// </summary>
    public struct DH_REMOTE_HOST
    {
        /// <summary>
        /// 连接使能
        /// </summary>
        public byte byEnable;
        public byte byReserved;
        /// <summary>
        /// 远程主机端口
        /// </summary>
        public UInt16 wHostPort;
        /// <summary>
        /// 远程主机 IP 地址
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sHostIPAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sHostIPAddr;
        /// <summary>
        /// 远程主机 用户名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 64)]
        //public char[] sHostUser;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] sHostUser;
        /// <summary>
        /// 远程主机 密码
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] sHostPassword;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] sHostPassword;
    }

    /// <summary>
    /// 邮件配置
    /// </summary>
    public struct DH_MAIL_CFG
    {
        /// <summary>
        /// 邮件服务器IP地址
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sMailIPAddr;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        public byte[] sMailIPAddr;

        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public UInt16 wMailPort;
        /// <summary>
        /// 保留
        /// </summary>
        public UInt16 wReserved;
        /// <summary>
        /// 发送地址
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] sSenderAddr;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        public byte[] sSenderAddr;
        /// <summary>
        /// 用户名 
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sUserName;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 16)]
        public byte[] sUserName;
        ///// <summary>
        ///// 用户名的字符串
        ///// </summary>
        ///// <returns></returns>
        //public string UserName()
        //{
        //    string result = "";
        //    foreach (char chr in sUserName)
        //    {
        //        if (chr != '\0')
        //        {
        //            result += chr.ToString();
        //        }
        //    }
        //    return result;
        //}
        /// <summary>
        /// 用户密码
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sUserPsw;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sUserPsw;

        ///// <summary>
        ///// 用户密码的字符串
        ///// </summary>
        ///// <returns></returns>
        //public string UserPsw()
        //{
        //    string result = "";
        //    foreach (char chr in sUserPsw)
        //    {
        //        if (chr != '\0')
        //        {
        //            result += chr.ToString();
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        /// 目的地址
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] sDestAddr;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 128)]
        public byte[] sDestAddr;
        /// <summary>
        /// 抄送地址
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] sCcAddr;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 128)]
        public byte[] sCcAddr;
        /// <summary>
        /// 抄送地址的字符串
        /// </summary>
        /// <returns></returns>
        public string CcAddr()
        {
            string result = "";
            foreach (char chr in sCcAddr)
            {
                if (chr != '\0')
                {
                    result += chr.ToString();
                }
            }
            return result;
        }
        /// <summary>
        /// 暗抄地址
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] sBccAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] sBccAddr;
        /// <summary>
        /// 暗抄地址的字符串
        /// </summary>
        /// <returns></returns>
        public string BccAddr()
        {
            string result = "";
            foreach (char chr in sBccAddr)
            {
                if (chr != '\0')
                {
                    result += chr.ToString();
                }
            }
            return result;
        }
        /// <summary>
        /// 标题
        /// </summary>        
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 64)]
        //public char[] sSubject;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] sSubject;       
        /// <summary>
        /// 标题的字符串
        /// </summary>
        /// <returns></returns>
        public string Subject()
        {
            string result = "";
            foreach (byte chr in sSubject)
            {
                if (chr != '\0')
                {
                    result += (char)chr;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 网络配置结构体
    /// </summary>
    public struct DHDEV_NET_CFG
    {
        public UInt32 dwSize;
        /// <summary>
        /// 设备主机名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sDevName;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 16)]
        public byte[] sDevName;
        /// <summary>
        /// 设备主机名的字符串表达
        /// </summary>
        /// <returns>主机名的字符串</returns>
        public string DevName()
        {
            string result = "";
            foreach (char chr in sDevName)
            {
                if (chr != '\0')
                {
                    result += chr.ToString();
                }
            }
            return result;
        }
        /// <summary>
        /// TCP最大连接数(一般指视频数据请求数) 
        /// </summary>
        public UInt16 wTcpMaxConnectNum;
        /// <summary>
        /// TCP帧听端口
        /// </summary>
        public UInt16 wTcpPort;
        /// <summary>
        /// UDP侦听端口
        /// </summary>
        public UInt16 wUdpPort;
        /// <summary>
        /// HTTP端口号 
        /// </summary>
        public UInt16 wHttpPort;
        /// <summary>
        /// HTTPS端口号 
        /// </summary>
        public UInt16 wHttpsPort;
        /// <summary>
        /// SSL端口号
        /// </summary>
        public UInt16 wSslPort;
        /// <summary>
        /// 以太网口
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public DH_ETHERNET[] stEtherNet;
        /// <summary>
        /// 报警服务器
        /// </summary>
        public DH_REMOTE_HOST struAlarmHost;
        /// <summary>
        /// 日志服务器 
        /// </summary>
        public DH_REMOTE_HOST struLogHost;
        /// <summary>
        /// SMTP服务器
        /// </summary>
        public DH_REMOTE_HOST struSmtpHost;
        /// <summary>
        /// 多播组
        /// </summary>
        public DH_REMOTE_HOST struMultiCast;
        /// <summary>
        /// NFS服务器
        /// </summary>
        public DH_REMOTE_HOST struNfs;
        /// <summary>
        /// PPPoE服务器
        /// </summary>
        public DH_REMOTE_HOST struPppoe;
        /// <summary>
        /// PPPoE注册返回的IP
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] sPppoeIP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sPppoeIP;
        /// <summary>
        /// DDNS服务器
        /// </summary>
        public DH_REMOTE_HOST struDdns;
        /// <summary>
        /// DDNS主机名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 64)]
        //public char[] sDdnsHostName;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 64)]
        public byte[] sDdnsHostName;
        /// <summary>
        /// DNS服务器
        /// </summary>
        public DH_REMOTE_HOST struDns;
        /// <summary>
        /// 邮件配置
        /// </summary>
        public DH_MAIL_CFG struMail;
    }

    /// <summary>
    /// 串口基本属性
    /// </summary>
    public struct DH_COMM_PROP
    {
        /// <summary>
        /// 数据位 0:5;1:6;2:7;3-8;
        /// </summary>
        public byte byDataBit;
        /// <summary>
        /// 停止位 0:1位;1:1.5位; 2:2位;
        /// </summary>
        public byte byStopBit;
        /// <summary>
        /// 校验位 0:不校验;1:奇校验; 2:偶校验;
        /// </summary>
        public byte byParity;
        /// <summary>
        /// 波特率 0:300;1:600;2:1200;3:2400;4:4800;5:9600;6:19200;7:38400;8:57600;9:115200;
        /// </summary>
        public byte byBaudRate;
    }

    /// <summary>
    /// 485解码器配置
    /// </summary>
    public struct DH_485_CFG
    {
        public DH_COMM_PROP struComm;
        /// <summary>
        /// 协议类型 保存协议的下标，动态变化
        /// </summary>
        public UInt16 wProtocol;
        /// <summary>
        /// 解码器地址:0 - 255
        /// </summary>
        public UInt16 wDecoderAddress;
    }

    /// <summary>
    /// 232串口配置
    /// </summary>
    public struct DH_RS232_CFG
    {
        public DH_COMM_PROP struComm;
        public byte byFunction;			// 串口功能，对应串口配置取到的功能名列表 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
    }


    /// <summary>
    /// 协议名
    /// </summary>
    public struct DH_PROANDFUN_NAME
    {
        /// <summary>
        /// 协议名[长度16]
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] ProName;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 16)]
        public byte[] ProName;
    }


    /// <summary>
    /// 串口配置结构体
    /// </summary>
    public struct DHDEV_COMM_CFG
    {
        /// <summary>
        /// 解码器协议
        /// </summary>
        public UInt32 dwSize;
        /// <summary>
        /// 协议个数
        /// </summary>
        public UInt32 dwDecProListNum;
        /// <summary>
        /// 协议名列表100
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public DH_PROANDFUN_NAME[] DecProName;
        /// <summary>
        /// 各解码器当前属性
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_485_CFG[] stDecoder;
        /// <summary>
        /// 232功能个数
        /// </summary>
        public UInt32 dw232FuncNameNum;
        /// <summary>
        /// 功能名列表10*16
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 10)]
        public DH_PROANDFUN_NAME[] s232FuncName;
        /// <summary>
        /// 各232串口当前属性
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public DH_RS232_CFG[] st232;
    }

    /// <summary>
    /// 自动维护属性
    /// </summary>
    public struct DHDEV_AUTOMT_CFG
    {
        /// <summary>
        /// 自动重启
        /// </summary>
        public UInt32 dwSize;
        /// <summary>
        ///  自动重启日期设定 0:从不;1:每天;2:每星期日;3:每星期一;......
        /// </summary>
        public byte byAutoRebootDay;
        /// <summary>
        /// 自动重启时间设定 0:0:00;1:1:00;........23:23:00;
        /// </summary>
        public byte byAutoRebootTime;
        /// <summary>
        /// 自动删除文件 0:从不;1:24H;2:48H;3:72H;4:96H:5:一周;6:一月
        /// </summary>
        public byte byAutoDeleteFilesTime;
        /// <summary>
        /// 保留位
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] reserved;
    }

    /// <summary>
    /// 本机控制策略配置
    /// </summary>
    public struct DH_VIDEOGROUP_CFG
    {
        /// <summary>
        /// 视频输出 0:无效;1:设备通道数表示对应通道;设备通道数+1代表all;
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] byVideoOut;
        /// <summary>
        /// 轮巡间隔，单位秒, 5-300 
        /// </summary>
        public int iInterval;
        /// <summary>
        /// 是否轮巡 
        /// </summary>
        public byte byEnableTour;
        /// <summary>
        /// 联动报警通道 0:无效;1:报警通道数表示对应通道;报警通道数+1代表1-4;报警通道数+2代表5-8...;参考本地界面
        /// </summary>
        public byte byAlarmChannel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved;
    }

    /// <summary>
    /// 本机矩阵控制策略配置
    /// </summary>
    public struct DHDEV_VIDEO_MATRIX_CFG
    {
        public UInt32 dwSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public DH_VIDEOGROUP_CFG[] struVideoGroup;
    }

    /// <summary>
    /// ddns配置
    /// </summary>
    public struct DH_DDNS_SERVER_CFG
    {
        /// <summary>
        /// ddns服务器id号
        /// </summary>
        public UInt32 dwId;
        /// <summary>
        /// 使能，同一时间只能有一个ddns服务器处于使能状态
        /// </summary>
        public bool bEnable;
        /// <summary>
        /// 服务器类型，希网..
        /// </summary>	
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szServerType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szServerType;
        /// <summary>
        /// 服务器ip
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] szServerIp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szServerIp;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public UInt32 dwServerPort;
        /// <summary>
        /// dvr域名如jecke.3322.org
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 256)]
        //public char[] szDomainName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] szDomainName;
        /// <summary>
        /// 用户名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szUserName;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] szUserName;
        /// <summary>
        /// 密码
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szUserPsw;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] szUserPsw;
        /// <summary>
        /// 服务器别名，如"dahua ddns"
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szAlias;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] szAlias;
    }
    /// <summary>
    /// 多ddns配置
    /// </summary>
    public struct DHDEV_MULTI_DDNS_CFG
    {
        public UInt32 dwSize;
        public UInt32 dwDdnsServerNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public DH_DDNS_SERVER_CFG[] struDdnsServer;
    }

    /// <summary>
    /// 抓图功能配置
    /// </summary>
    public struct DHDEV_SNAP_CFG
    {
        public UInt32 dwSize;
        /// <summary>
        /// 定时抓图开关（报警抓图开关在各报警联动配置中体现）
        /// </summary>
        public byte bTimingEnable;
        public byte bReserved;
        /// <summary>
        /// 定时抓图时间间隔，单位为秒,目前设备支持最大的抓图时间间隔为30分钟
        /// </summary>
        public ushort PicTimeInterval;
        /// <summary>
        /// 抓图编码配置，现支持其中的分辨率、画质、帧率设置，帧率在这里是负数，表示一秒抓图的次数。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public DH_VIDEOENC_OPT[] struSnapEnc;
    }

    /// <summary>
    /// web路径配置
    /// </summary>
    public struct DHDEV_URL_CFG
    {
        public UInt32 dwSize;
        /// <summary>
        /// 是否抓图
        /// </summary>
        public bool bSnapEnable;
        /// <summary>
        /// 抓图周期
        /// </summary>
        public int iSnapInterval;
        /// <summary>
        /// HTTP主机IP
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] szHostIp;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 16)]
        public byte[] szHostIp;
        public UInt16 wHostPort;
        /// <summary>
        /// 状态消息发送间隔
        /// </summary>
        public int iMsgInterval;
        /// <summary>
        /// 状态消息上传URL
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] szUrlState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szUrlState;
        /// <summary>
        /// 图片上传URL
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 128)]
        //public char[] szUrlImage;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 128)]
        public byte[] szUrlImage;
        /// <summary>
        /// 机器的web编号
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 48)]
        //public char[] szDevId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] szDevId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] byReserved;
    }


    /// <summary>
    /// 
    /// </summary>
    public struct struPeriod
    {
        /// <summary>
        /// 该时间段内的“使能”无效，可忽略
        /// </summary>
        public DH_TSECT struSect;
        /// <summary>
        /// 上传动态检测录像
        /// </summary>
        public bool bMdEn;
        /// <summary>
        /// 上传外部报警录像
        /// </summary>
        public bool bAlarmEn;
        /// <summary>
        /// 上传普通定时录像
        /// </summary>	
        public bool bTimerEn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] dwRev;
    }
    /// <summary>
    /// FTP上传配置
    /// </summary>
    public struct DH_FTP_UPLOAD_CFG
    {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public struPeriod[] Period;
    }

    /// <summary>
    /// FTP上传配置
    /// </summary>
    public struct DHDEV_FTP_PROTO_CFG
    {
        public UInt32 dwSize;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool bEnable;
        /// <summary>
        /// 主机IP
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] szHostIp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szHostIp;
        /// <summary>
        /// 主机端口
        /// </summary>
        public UInt32 wHostPort;
        /// <summary>
        /// FTP目录路径
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 240)]
        //public char[] szDirName;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 240)]
        public byte[] szDirName;
        /// <summary>
        /// 用户名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 64)]
        //public char[] szUserName;
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 64)]
        public byte[] szUserName;
        /// <summary>
        /// 密码
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 64)]
        //public char[] szPassword;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 64)]
        public byte[] szPassword;
        /// <summary>
        /// 文件长度
        /// </summary>
        public int iFileLen;
        /// <summary>
        /// 相邻文件时间间隔
        /// </summary>
        public int iInterval;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
        public DH_FTP_UPLOAD_CFG[] struUploadCfg;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 130)]
        public byte[] reserved;
    }

    /// <summary>
    /// 平台接入配置 － U网通平台
    /// </summary>
    public struct DH_INTERVIDEO_UCOM_CHN_CFG
    {
        public bool bChnEn;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szChnId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szChnId;
    }

    /// <summary>
    /// 平台接入配置 － U网通平台
    /// </summary>
    public struct DHDEV_INTERVIDEO_UCOM_CFG
    {
        public UInt32 dwSize;
        /// <summary>
        /// 接入功能使能与否 0:使能
        /// </summary>
        public bool bFuncEnable;
        /// <summary>
        /// 心跳使能与否
        /// </summary>
        public bool bAliveEnable;
        /// <summary>
        /// 心跳周期，单位秒，0-3600
        /// </summary>
        public UInt32 dwAlivePeriod;
        /// <summary>
        /// CMS的IP
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] szServerIp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szServerIp;
        /// <summary>
        /// CMS的Port
        /// </summary>
        public UInt16 wServerPort;
        /// <summary>
        /// 注册密码
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 16)]
        //public char[] szRegPwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szRegPwd;
        /// <summary>
        /// 设备id
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szDeviceId;
        [MarshalAs(UnmanagedType.ByValArray,  SizeConst = 32)]
        public byte[] szDeviceId;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szUserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szUserName;
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 32)]
        //public char[] szPassWord;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szPassWord;
        /// <summary>
        /// 通道id,en
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_INTERVIDEO_UCOM_CHN_CFG[] struChnInfo;
    }


    // IP信息扩展
    public struct DHDEV_IPIFILTER_INFO_EX
    {
        /// <summary>
        /// IP个数
        /// </summary>
	    public UInt32 dwIPNum;
        /// <summary>
        /// IP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512*16)]
	    public byte[] SZIP;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] byReserve; 
    } ;

    // IP过滤配置结构体扩展
    public struct DHDEV_IPIFILTER_CFG_EX
    {
	    public UInt32 dwSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public UInt32 dwEnable;
        /// <summary>
        /// 当前名单类型：0：白名单 1：黑名单（设备只能使一种名单生效，或者是白名单或者是黑名单）
        /// </summary>
	    public UInt32 dwType;
        /// <summary>
        /// 黑名单
        /// </summary>
	    public DHDEV_IPIFILTER_INFO_EX BannedIP;
        /// <summary>
        /// 白名单
        /// </summary>
	    public DHDEV_IPIFILTER_INFO_EX TrustIP;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] byReserve;
    }


    // MAC，IP过滤信息
    public struct MACIP_INFO
    {
        /// <summary>
        /// 使用时，用初始化为本结构体大小
        /// </summary>
	    public UInt32	dwSize;
        /// <summary>
        /// MAC
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
	    public byte[]	szMac;
        /// <summary>
        /// IP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[]	szIp;
    }

    // MAC,IP过滤配置结构体
    public struct DHDEV_MACIPFILTER_CFG
    {
        /// <summary>
        /// 使用时，用初始化为本结构体大小
        /// </summary>
	    public UInt32   dwSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public UInt32   dwEnable;
        /// <summary>
        /// 当前名单类型：0：白名单 1：黑名单（设备只能使一种名单生效，或者是白名单或者是黑名单）
        /// </summary>
	    public UInt32   dwType;
        /// <summary>
        /// 黑名单MAC,IP个数(MAC,IP一一对应)
        /// </summary>
	    public UInt32   dwBannedMacIpNum;
        /// <summary>
        /// 黑名单Mac,IP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
	    public MACIP_INFO[]  stuBannedMacIp;
        /// <summary>
        /// 白名单MAC,IP个数(MAC,IP一一对应)
        /// </summary>
	    public UInt32   dwTrustMacIpNum;
        /// <summary>
        /// 白名单Mac,IP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
	    public MACIP_INFO[]  stuTrustMacIp;
    } 

    /// <summary>
    /// 具体密钥信息36个字节
    /// </summary>
    public struct ENCRYPT_KEY_INFO
    {
        /// <summary>
        /// 是否加密0:不加密, 1:加密
        /// </summary>
	    public byte         byEncryptEnable;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[]       byReserved;
        /// <summary>
        /// union
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[]       byKey; 
        //public ENCRYPT_TYPE_UNION  stuEncryptTypeUinon;
    }


    /// <summary>
    /// 加密算法参数
    /// </summary>
    public struct ALGO_PARAM_INFO
    {
        /// <summary>
        /// 密钥长度，当前为AES算法类型时，表示密钥位数(目前支持128，192，256位三种, 
        /// 如: wEncryptLenth为128，则密钥信息ENCRYPT_KEY_INFO里的byAesKey[0]~[15])
        /// 为DES算法类型时，密钥长度固定为64位
        /// 为3DES算法类型时，表示密钥的个数(2或3个密钥)
        /// </summary>
	    public UInt16   wEncryptLenth; 
        /// <summary>
        /// 工作模式,参考枚举类型 EM_ENCRYPT_ALOG_WORKMODE 
        /// </summary>
	    public byte     byAlgoWorkMode; 
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
	    public byte[]   reserved; 
    }

    /// <summary>
    /// 码流加密配置信息 
    /// </summary>
    public struct DHEDV_STREAM_ENCRYPT
    {
        /// <summary>
        /// 加密算法类型：00: AES、01:DES、02: 3DES
        /// </summary>
        public byte    byEncrptAlgoType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[]   byReserved1;
        /// <summary>
        /// 加密算法参数
        /// </summary>
        public ALGO_PARAM_INFO     stuEncrptAlgoparam;
        /// <summary>
        /// 各通道的密钥信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ENCRYPT_KEY_INFO[]    stuEncryptKeys; 
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1388)]
        public byte[]       reserved2;
    }

    #endregion

    #region <<物体的检测模块配置--相关结构体>>

    /// <summary>
    /// 区域顶点信息
    /// </summary>
    public struct CFG_POLYGON
    {
        public Int32 nX; //0~8191
        public Int32 nY;
    }

    /// <summary>
    /// Size
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
    public struct CFG_SIZE
    {
        /// <summary>
        /// 宽或面积(C++中是用union表示的)
        /// </summary>
        [FieldOffset(0)]
        public float nWidth;
        [FieldOffset(0)]
        public float nArea;
        /// <summary>
        /// 高
        /// </summary>
        [FieldOffset(4)]
        public float nHeight;

    }

    /// <summary>
    /// 区域信息
    /// </summary>
    public struct CFG_REGION
    {
        public Int32 nPointNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYGON[] stuPolygon;
    }

    /// <summary>
    /// 校准框信息
    /// </summary>
    public struct CFG_CALIBRATEBOX_INFO
    {
        /// <summary>
        /// 校准框中心点坐标(点的坐标归一化到[0,8191]区间)
        /// </summary>
        public CFG_POLYGON stuCenterPoint;
        /// <summary>
        /// 相对基准校准框的比率(比如1表示基准框大小，0.5表示基准框大小的一半)
        /// </summary>
        public float fRatio;
    }

    /// <summary>
    /// 尺寸过滤器
    /// </summary>
    public struct CFG_SIZEFILTER_INFO
    {
        /// <summary>
        /// 校准框个数
        /// </summary>
        public Int32 nCalibrateBoxNum;
        /// <summary>
        /// 校准框(远端近端标定模式下有效)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEBOX_INFO[] stuCalibrateBoxs;
        /// <summary>
        /// 计量方式参数是否有效
        /// </summary>
        public byte bMeasureModeEnable;
        /// <summary>
        /// 计量方式,0-像素，不需要远端、近端标定, 1-实际长度，单位：米, 2-远端近端标定后的像素
        /// </summary>
        public byte bMeasureMode;
        /// <summary>
        /// 过滤类型参数是否有效
        /// </summary>
        public byte bFilterTypeEnable;
        // ByArea,ByRatio仅作兼容，使用独立的ByArea和ByRatio选项代替 2012/03/06
        /// <summary>
        /// 过滤类型:0:"ByLength",1:"ByArea", 2"ByWidthHeight"
        /// </summary>
        public byte bFilterType;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved;
        /// <summary>
        /// 物体最小尺寸参数是否有效
        /// </summary>
        public byte bFilterMinSizeEnable;
        /// <summary>
        /// 物体最大尺寸参数是否有效
        /// </summary>
        public byte bFilterMaxSizeEnable;
        /// <summary>
        /// 物体最小尺寸 "ByLength"模式下表示宽高的尺寸，"ByArea"模式下宽表示面积，高无效(远端近端标定模式下表示基准框的宽高尺寸)。
        /// </summary>
        public CFG_SIZE stuFilterMinSize;
        /// <summary>
        /// 物体最大尺寸 "ByLength"模式下表示宽高的尺寸，"ByArea"模式下宽表示面积，高无效(远端近端标定模式下表示基准框的宽高尺寸)。
        /// </summary>
        public CFG_SIZE stuFilterMaxSize;

        public byte abByArea;
        public byte abMinArea;
        public byte abMaxArea;
        public byte abMinAreaSize;
        public byte abMaxAreaSize;
        /// <summary>
        /// 是否按面积过滤 通过能力ComplexSizeFilter判断是否可用
        /// </summary>
        public byte bByArea;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved1;
        /// <summary>
        /// 最小面积
        /// </summary>
        public Int32 nMinArea;
        /// <summary>
        /// 最大面积
        /// </summary>
        public Int32 nMaxArea;
        /// <summary>
        /// 最小面积矩形框尺寸 "计量方式"为"像素"时，表示最小面积矩形框的宽高尺寸；"计量方式"为"远端近端标定模式"时，表示基准框的最小宽高尺寸；
        /// </summary>
        public CFG_SIZE stuMinAreaSize;
        /// <summary>
        /// 最大面积矩形框尺寸, 同上
        /// </summary>
        public CFG_SIZE stuMaxAreaSize;

        public byte abByRatio;
        public byte abMinRatio;
        public byte abMaxRatio;
        public byte abMinRatioSize;
        public byte abMaxRatioSize;
        /// <summary>
        /// 是否按宽高比过滤 通过能力ComplexSizeFilter判断是否可用
        /// </summary>
        public byte bByRatio;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved2;
        /// <summary>
        /// 最小宽高比
        /// </summary>
        public double dMinRatio;
        /// <summary>
        /// 最大宽高比
        /// </summary>
        public double dMaxRatio;
        /// <summary>
        /// 最小宽高比矩形框尺寸，最小宽高比对应矩形框的宽高尺寸。
        /// </summary>
        public CFG_SIZE stuMinRatioSize;
        /// <summary>
        /// 最大宽高比矩形框尺寸，同上
        /// </summary>
        public CFG_SIZE stuMaxRatioSize;
        /// <summary>
        /// 面积校准框个数
        /// </summary>
        public Int32 nAreaCalibrateBoxNum;
        /// <summary>
        /// 面积校准框
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEBOX_INFO[] stuAreaCalibrateBoxs;
        /// <summary>
        /// 宽高校准框个数
        /// </summary>
        public Int32 nRatioCalibrateBoxs;
        /// <summary>
        ///  宽高校准框个数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEBOX_INFO[] stuRatioCalibrateBoxs;
        /// <summary>
        /// 长宽过滤使能参数是否有效
        /// </summary>
        public byte abBySize;
        /// <summary>
        /// 长宽过滤使能
        /// </summary>
        public byte bBySize;
    };

    // 各种物体特定的过滤器
    public struct CFG_OBJECT_SIZEFILTER_INFO
    {
        /// <summary>
        /// 物体类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szObjectType;
        /// <summary>
        /// 对应的尺寸过滤器
        /// </summary>
        public CFG_SIZEFILTER_INFO stSizeFilter;
    };

    /// <summary>
    /// 不同区域各种类型物体的检测模块配置
    /// </summary>
    public struct CFG_MODULE_INFO
    {
        // 信息
        /// <summary>
        /// 默认物体类型,详见"支持的检测物体类型列表"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szObjectType;
        /// <summary>
        /// 是否对识别物体抓图
        /// </summary>
        public byte bSnapShot;
        /// <summary>
        /// 灵敏度,取值1-10，值越小灵敏度越低
        /// </summary>
        public byte bSensitivity;
        /// <summary>
        /// 计量方式参数是否有效
        /// </summary>
        public byte bMeasureModeEnable;
        /// <summary>
        /// 计量方式,0-像素，不需要远端、近端标定, 1-实际长度，单位：米, 2-远端近端标定后的像素
        /// </summary>
        public byte bMeasureMode;
        /// <summary>
        /// 检测区域顶点数
        /// </summary>
        public Int32 nDetectRegionPoint;
        /// <summary>
        /// 检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 跟踪区域顶点数
        /// </summary>
        public Int32 nTrackRegionPoint;
        /// <summary>
        /// 跟踪区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYGON[] stuTrackRegion;
        /// <summary>
        /// 过滤类型参数是否有效
        /// </summary>
        public byte bFilterTypeEnable;
        // ByArea,ByRatio仅作兼容，使用独立的ByArea和ByRatio选项代替 2012/03/06
        /// <summary>
        /// 过滤类型:0:"ByLength",1:"ByArea", 2:"ByWidthHeight", 3:"ByRatio": 按照宽高比，宽度除以高度的结果小于某个值或者大于某个值的问题将被过滤掉。
        /// </summary>
        public byte nFilterType;
        /// <summary>
        /// 区域的背景类型参数是否有效
        /// </summary>
        public byte bBackgroudEnable;
        /// <summary>
        /// 区域的背景类型, 0-普通类型, 1-高光类型
        /// </summary>
        public byte bBackgroud;
        /// <summary>
        /// 长宽过滤使能参数是否有效
        /// </summary>
        public byte abBySize;
        /// <summary>
        /// 长宽过滤使能
        /// </summary>
        public byte bBySize;
        /// <summary>
        /// 物体最小尺寸参数是否有效
        /// </summary>
        public byte bFilterMinSizeEnable;
        /// <summary>
        /// 物体最大尺寸参数是否有效
        /// </summary>
        public byte bFilterMaxSizeEnable;
        /// <summary>
        /// 物体最小尺寸 "ByLength"模式下表示宽高的尺寸，"ByArea"模式下宽表示面积，高无效。
        /// </summary>
        public CFG_SIZE stuFilterMinSize;
        /// <summary>
        /// 物体最大尺寸 "ByLength"模式下表示宽高的尺寸，"ByArea"模式下宽表示面积，高无效。
        /// </summary>
        public CFG_SIZE stuFilterMaxSize;
        /// <summary>
        /// 排除区域数
        /// </summary>
        public Int32 nExcludeRegionNum;
        /// <summary>
        /// 排除区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_REGION[] stuExcludeRegion;
        /// <summary>
        /// 校准框个数
        /// </summary>
        public Int32 nCalibrateBoxNum;
        /// <summary>
        /// 校准框(远端近端标定模式下有效)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEBOX_INFO[] stuCalibrateBoxs;
        /// <summary>
        /// 检测精度是否有效
        /// </summary>
        public byte bAccuracy;
        /// <summary>
        /// 检测精度
        /// </summary>
        public byte byAccuracy;
        /// <summary>
        /// 算法移动步长是否有效
        /// </summary>
        public byte bMovingStep;
        /// <summary>
        /// 算法移动步长
        /// </summary>
        public byte byMovingStep;
        /// <summary>
        /// 算法缩放因子是否有效
        /// </summary>
        public byte bScalingFactor;
        /// <summary>
        /// 算法缩放因子
        /// </summary>
        public byte byScalingFactor;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] bReserved2;
        /// <summary>
        /// 漏检和误检平衡参数是否有效
        /// </summary>
        public byte abDetectBalance;
        /// <summary>
        /// 漏检和误检平衡	0-折中模式(默认)1-漏检更少2-误检更少
        /// </summary>
        public Int32 nDetectBalance;

        public byte abByRatio;
        public byte abMinRatio;
        public byte abMaxRatio;
        public byte abMinAreaSize;
        public byte abMaxAreaSize;
        /// <summary>
        /// 是否按宽高比过滤 通过能力ComplexSizeFilter判断是否可用 可以和nFilterType复用
        /// </summary>
        public byte bByRatio;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved1;
        /// <summary>
        /// 最小宽高比
        /// </summary>
        public double dMinRatio;
        /// <summary>
        /// 最大宽高比
        /// </summary>
        public double dMaxRatio;
        /// <summary>
        /// 最小面积矩形框尺寸 "计量方式"为"像素"时，表示最小面积矩形框的宽高尺寸；"计量方式"为"远端近端标定模式"时，表示基准框的最小宽高尺寸；
        /// </summary>
        public CFG_SIZE stuMinAreaSize;
        /// <summary>
        /// 最大面积矩形框尺寸, 同上
        /// </summary>
        public CFG_SIZE stuMaxAreaSize;

        public byte abByArea;
        public byte abMinArea;
        public byte abMaxArea;
        public byte abMinRatioSize;
        public byte abMaxRatioSize;
        /// <summary>
        /// 是否按面积过滤 通过能力ComplexSizeFilter判断是否可用  可以和nFilterType复用
        /// </summary>
        public byte bByArea;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved3;
        /// <summary>
        /// 最小面积
        /// </summary>
        public Int32 nMinArea;
        /// <summary>
        /// 最大面积
        /// </summary>
        public Int32 nMaxArea;
        /// <summary>
        /// 最小宽高比矩形框尺寸，最小宽高比对应矩形框的宽高尺寸。
        /// </summary>
        public CFG_SIZE stuMinRatioSize;
        /// <summary>
        /// 最大宽高比矩形框尺寸，同上
        /// </summary>
        public CFG_SIZE stuMaxRatioSize;
        /// <summary>
        /// 面积校准框个数
        /// </summary>
        public Int32 nAreaCalibrateBoxNum;
        /// <summary>
        /// 面积校准框
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEBOX_INFO[] stuAreaCalibrateBoxs;
        /// <summary>
        /// 比例校准框个数
        /// </summary>
        public int nRatioCalibrateBoxs;
        /// <summary>
        /// 比例校准框个数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEBOX_INFO[] stuRatioCalibrateBoxs;
        /// <summary>
        /// 是否开启去扰动模块
        /// </summary>
        public byte bAntiDisturbance;
        /// <summary>
        /// 是否有逆光
        /// </summary>
        public byte bBacklight;
        /// <summary>
        /// 是否有阴影
        /// </summary>
        public byte bShadow;
        /// <summary>
        /// 云台预置点，0~255，0表示固定场景，忽略预置点。大于0表示在此预置点时模块有效
        /// </summary>
        public int nPtzPresetId;
        /// <summary>
        /// 物体特定的过滤器个数
        /// </summary>
        public int nObjectFilterNum;
        /// <summary>
        /// 物体特定的过滤器信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public CFG_OBJECT_SIZEFILTER_INFO[] stObjectFilter;
    }

    public struct CFG_ANALYSEMODULES_INFO
    {
        /// <summary>
        /// 检测模块数
        /// </summary>
        public Int32 nMoudlesNum;
        /// <summary>
        /// 每个视频输入通道对应的各种类型物体的检测模块配置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public CFG_MODULE_INFO[] stuModuleInfo;

    }


    #endregion

    #region <<视频分析全局配置--相关结构体>>

    /// <summary>
    /// 折线的端点信息
    /// </summary>
    public struct CFG_POLYLINE
    {
        /// <summary>
        /// 0~8191
        /// </summary>
        public Int32 nX;
        public Int32 nY;
    }

    /// <summary>
    /// 车道信息
    /// </summary>
    public struct CFG_LANE
    {
        /// <summary>
        /// 车道编号
        /// </summary>
        public Int32 nLaneId;
        /// <summary>
        /// 车道方向(车开往的方向),0-北 1-东北 2-东 3-东南 4-南 5-西南 6-西 7-西北
        /// </summary>
        public Int32 nDirection;
        /// <summary>
        /// 左车道线，车道线的方向表示车道方向，沿车道方向左边的称为左车道线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYLINE[] stuLeftLine;
        /// <summary>
        /// 左车道线顶点数
        /// </summary>
        public Int32 nLeftLineNum;
        /// <summary>
        /// 右车道线，车道线的方向表示车道方向，沿车道方向右边的称为右车道线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYLINE[] stuRightLine;
        /// <summary>
        /// 右车道线顶点数
        /// </summary>
        public Int32 nRightLineNum;
        /// <summary>
        /// 左车道线属性，1-表示白实线，2- 白虚线，3- 黄线
        /// </summary>
        public Int32 nLeftLineType;
        /// <summary>
        /// 右车道线属性，1-表示白实线，2- 白虚线，3- 黄线
        /// </summary>
        public Int32 nRightLineType;
        /// <summary>
        /// 车道行驶方向使能 c++中的类型是BOOL
        /// </summary>
        public Int32 bDriveDirectionEnable;
        /// <summary>
        /// 车道行驶方向数 
        /// </summary>
        public Int32 nDriveDirectionNum;
        /// <summary>
        /// 车道行驶方向，"Straight" 直行，"TurnLeft" 左转，"TurnRight" 右转,"U-Turn":掉头
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 32)]
        public byte[] szDriveDirection;
        /// <summary>
        /// 车道对应停止线顶点数
        /// </summary>
        public Int32 nStopLineNum;
        /// <summary>
        /// 车道对应停止线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYLINE[] stuStopLine;
        /// <summary>
        /// 车道对应的红绿灯组编号
        /// </summary>
        public Int32 nTrafficLightNumber;
    };

    // 交通灯属性
    public struct CFG_LIGHTATTRIBUTE
    {
        /// <summary>
        /// 当前交通灯是否有效，与车辆通行无关的交通需要设置无效
        /// c++下类型为BOOL
        /// </summary>
        public Int32 bEnable;
        public int nTypeNum;
        /// <summary>
        /// 当前交通灯显现内容（包括:红-Red,黄-Yellow,绿-Green,倒计时-Countdown），如某交通灯可以显示红黄绿三种颜色，某交通灯只显示倒计时
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 32)]
        public byte[] szLightType;
        public int nDirectionNum;
        /// <summary>
        /// 交通灯指示的行车方向,"Straight": 直行，"TurnLeft":左转，"TurnRight":右转，"U-Turn": 掉头
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 32)]
        public byte[] szDirection;
        /// <summary>
        /// 黄灯亮时间
        /// </summary>
        public int nYellowTime;
    }

    /// <summary>
    /// 区域信息
    /// </summary>
    public struct CFG_RECT
    {
        public int nLeft;
        public int nTop;
        public int nRight;
        public int nBottom;
    }

    /// <summary>
    /// 交通灯组配置信息  
    /// </summary>
    public struct CFG_LIGHTGROUPS
    {
        /// <summary>
        /// 灯组编号
        /// </summary>
        public int nLightGroupId;
        /// <summary>
        /// 灯组坐标
        /// </summary>
        public CFG_RECT stuLightLocation;
        /// <summary>
        /// 灯组的方向,1- 灯组水平向,2- 灯组垂直向
        /// </summary>
        public int nDirection;
        /// <summary>
        /// 是否为外接红绿灯信号,当外接红绿灯时，以外界信号为判断依据。外界信号每次跳变时通知
        /// c++中的类型为BOOL
        /// </summary>
        public Int32 bExternalDetection;
        /// <summary>
        /// 是否支持自适应灯组摇摆检测,在风吹或者容易震动的场景下，位置会进行一定的浮动偏差。如果由算法自行检测，会增加检测时间
        /// c++中的类型为BOOL
        /// </summary>
        public Int32 bSwingDetection;
        /// <summary>
        /// 灯组中交通灯的数量
        /// </summary>
        public int nLightNum;
        /// <summary>
        /// 灯组中各交通灯的属性
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public CFG_LIGHTATTRIBUTE[] stuLightAtrributes;

    }

    public struct CFG_STAFF
    {
        /// <summary>
        /// 起始坐标点
        /// </summary>
        public CFG_POLYLINE stuStartLocation;
        /// <summary>
        /// 终止坐标点
        /// </summary>
        public CFG_POLYLINE stuEndLocation;
        /// <summary>
        /// 实际长度,单位米
        /// </summary>
        public float nLenth;
        /// <summary>
        /// 标尺类型
        /// </summary>
        public EM_STAFF_TYPE emType;
    }

    /// <summary>
    /// 标定区域,普通场景使用
    /// </summary>
    public struct CFG_CALIBRATEAREA_INFO
    {
        /// <summary>
        /// 水平方向标尺线顶点数
        /// </summary>
        public int nLinePoint;
        /// <summary>
        /// 水平方向标尺线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYGON[] stuLine;
        /// <summary>
        /// 实际长度
        /// </summary>
        public float fLenth;
        /// <summary>
        /// 区域
        /// </summary>
        public CFG_REGION stuArea;
        /// <summary>
        /// 垂直标尺数
        /// </summary>
        public int nStaffNum;
        /// <summary>
        /// 垂直标尺 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_STAFF[] stuStaffs;
        /// <summary>
        /// 区域类型
        /// </summary>
        public EM_CALIBRATEAREA_TYPE emType;
    }

    /// <summary>
    /// 人脸检测场景
    /// </summary>
    public struct CFG_FACERECOGNITION_SCENCE_INFO
    {
        /// <summary>
        /// 摄像头离地高度 单位：米
        /// </summary>
        public double dbCameraHeight;
        /// <summary>
        /// 摄像头离地面检测区域中心的水平距离 单位：米
        /// </summary>
        public double dbCameraDistance;
        /// <summary>
        /// 人流主要方向顶点数
        /// </summary>
        public int nMainDirection;
        /// <summary>
        /// 人流主要方向，第一个点是起始点，第二个点是终止点
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_POLYGON[] stuMainDirection;
        /// <summary>
        /// 单位度，-45~45，负数表示人脸向画面上边，正数表示人脸向画面下边，0表示人脸垂直方向上正对着摄像头。
        /// </summary>
        public byte byFaceAngleDown;
        /// <summary>
        /// 单位度，-45~45，负数表示人脸向画面上边，正数表示人脸向画面下边，0表示人脸垂直方向上正对着摄像头。
        /// </summary>
        public byte byFaceAngleUp;
        /// <summary>
        /// 单位度，-45~45，负数表示人脸向画面上边，正数表示人脸向画面下边，0表示人脸垂直方向上正对着摄像头。
        /// </summary>
        public byte byFaceAngleLeft;
        /// <summary>
        /// 单位度，-45~45，负数表示人脸向画面上边，正数表示人脸向画面下边，0表示人脸垂直方向上正对着摄像头。
        /// </summary>
        public byte byFaceAngleRight;
    }
    #endregion

    #region <<智能交通中三个配置：场景、模块、交通规则 配置>>

    /// <summary>
    /// 视频分析全局配置
    /// </summary>
    //[StructLayout(LayoutKind.Sequential, public byte[]Set=public byte[]Set.Unicode)]
    //[StructLayout(LayoutKind.Sequential, public byte[]Set = public byte[]Set.Ansi)]
    public struct CFG_ANALYSEGLOBAL_INFO
    {
        /// <summary>
        /// 信息
        /// 应用场景,详见"支持的场景列表" public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szSceneType;

        /// <summary>
        /// 交通场景信息
        /// 摄像头离地高度	单位：米
        /// </summary>
        public double CameraHeight;
        /// <summary>
        /// 摄像头离地面检测区域中心的水平距离	单位：米
        /// </summary>
        public double CameraDistance;
        /// <summary>
        /// 近景检测点
        /// </summary>
        public CFG_POLYGON stuNearDetectPoint;
        /// <summary>
        /// 远景检测点
        /// </summary>
        public CFG_POLYGON stuFarDectectPoint;
        /// <summary>
        /// NearDetectPoint,转换到实际场景中时,离摄像头垂直线的水平距离
        /// </summary>
        int nNearDistance;
        /// <summary>
        /// FarDectectPoint,转换到实际场景中时,离摄像头垂直线的水平距离
        /// </summary>
        int nFarDistance;
        /// <summary>
        /// 交通场景的子类型,"Gate",卡口类型,"Junction" 路口类型public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szSubType;
        /// <summary>
        /// 车道数
        /// </summary>
        public int nLaneNum;
        /// <summary>
        /// 车道信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public CFG_LANE[] stuLanes;
        /// <summary>
        /// 车牌字符暗示个数
        /// </summary>
        public int nPlateHintNum;
        /// <summary>
        /// 车牌字符暗示数组，在拍摄图片质量较差车牌识别不确定时，根据此数组中的字符进行匹配，数组下标越小，匹配优先级越高public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 32)]
        public byte[] szPlateHints;

        /// <summary>
        /// 灯组数
        /// </summary>
        public int nLightGroupNum;
        /// <summary>
        /// 灯组配置信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public CFG_LIGHTGROUPS[] stLightGroups;

        // 一般场景信息 
        /// <summary>
        /// 标尺数
        /// </summary>
        public int nStaffNum;
        /// <summary>
        /// 标尺
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CFG_STAFF[] stuStaffs;
        /// <summary>
        /// 标定区域数
        /// </summary>
        public int nCalibrateAreaNum;
        /// <summary>
        /// 标定区域(若该字段不存在，则以整幅场景为标定区域)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CFG_CALIBRATEAREA_INFO[] stuCalibrateArea;
        /// <summary>
        /// 人脸识别场景是否有效
        /// C++中类型为BOOL
        /// </summary>
        public Int32 bFaceRecognition;
        /// <summary>
        /// 人脸识别场景
        /// </summary>
        public CFG_FACERECOGNITION_SCENCE_INFO stuFaceRecognitionScene;

        public byte abJitter;
        public byte abDejitter;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved;
        /// <summary>
        /// 摄像机抖动率 : 摄像机抖动率，取值0-100，反应静止摄像机抖动程度，抖动越厉害，值越大。
        /// </summary>
        public Int32 nJitter;
        /// <summary>
        /// 是否开启去抖动模块 目前不实现,C++下类型是BOOL
        /// </summary>
        public Int32 bDejitter;
    }

    public struct CFG_RULE_INFO
    {
        /// <summary>
        /// 事件类型，详见dhnetsdk.h中"智能分析事件类型"
        /// </summary>
	    public UInt32 dwRuleType;
        /// <summary>
        /// 该事件类型规则配置结构体大小
        /// </summary>
	    public Int32 nRuleSize;
    	
    } 

    /// <summary>
    /// 每个视频输入通道对应的所有事件规则：缓冲区pRuleBuf填充多个事件规则信息，
    /// 每个事件规则信息内容为CFG_RULE_INFO+"事件类型对应的规则配置结构体"。
    /// </summary>
    public struct CFG_ANALYSERULES_INFO
    {
        /// <summary>
        /// 事件规则个数
        /// </summary>
        public Int32 nRuleCount;
        /// <summary>
        /// 每个视频输入通道对应的视频分析事件规则配置缓冲
        /// C++下格式是char*
        /// </summary>
        public IntPtr pRuleBuf;
        /// <summary>
        /// 缓冲大小
        /// </summary>
        public Int32 nRuleLen;

    }

    /// <summary>
    /// 视频分析能力集
    /// </summary>
    public struct CFG_CAP_ANALYSE_INFO
    {
        /// <summary>
        /// 支持的场景个数
        /// </summary>
        public Int32 nSupportedSceneNum;
        /// <summary>
        /// 支持的场景列表public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 32)]
        public byte[] szSceneName;
        /// <summary>
        /// 每通道支持最大分析模块数
        /// </summary>
        public Int32 nMaxMoudles;
        /// <summary>
        /// 支持的检测物体类型个数
        /// </summary>
        public Int32 nSupportedObjectTypeNum;
        /// <summary>
        /// 支持的检测物体类型列表public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
        public byte[] szObjectTypeName;
        /// <summary>
        /// 每通道支持最大规则条数
        /// </summary>
        public Int32 nMaxRules;
        /// <summary>
        /// 支持的事件类型规则个数
        /// </summary>
        public Int32 nSupportedRulesNum;
        /// <summary>
        /// 支持的事件类型规则列表，事件类型，详见dhnetsdk.h中"智能分析事件类型"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public UInt32[] dwRulesType;
        /// <summary>
        /// 支持的最大标尺个数
        /// </summary>
        public Int32 nMaxStaffs;
        /// <summary>
        /// 折线最大顶点数
        /// </summary>
        public Int32 nMaxPointOfLine;
        /// <summary>
        /// 区域最大顶点数
        /// </summary>
        public Int32 nMaxPointOfRegion;
        /// <summary>
        /// 最大内部选项个数
        /// </summary>
        public Int32 nMaxInternalOptions;
        /// <summary>
        /// 是否支持复杂尺寸过滤器	复杂尺寸过滤器使用独立的面积过滤和宽高比过滤参数。      
        /// </summary>
        public byte bComplexSizeFilter;
        /// <summary>
        /// 是否支持特定的物体过滤器
        /// </summary>
        public byte bSpecifiedObjectFilter;
        /// <summary>
        /// 支持模块中的最大排除区域个数
        /// </summary>
        public Int32 nMaxExcludeRegionNum;
        /// <summary>
        /// 支持的模块中的最大校准框个数
        /// </summary>
        public Int32 nMaxCalibrateBoxNum;
        /// <summary>
        /// 模块中至少需要设置的校准框个数
        /// </summary>
        public Int32 nMinCalibrateBoxNum;
    }
    #endregion

    #region <<事件类型EVENT_IVS_TRAFFICGATE(交通卡口事件)对应的数据块描述信息>>

    public struct NET_TIME_EX
    {
        /// <summary>
        /// 年
        /// </summary>
        public UInt32 dwYear;
        /// <summary>
        /// 月
        /// </summary>
        public UInt32 dwMonth;
        /// <summary>
        /// 日
        /// </summary>
        public UInt32 dwDay;
        /// <summary>
        /// 时
        /// </summary>
        public UInt32 dwHour;
        /// <summary>
        /// 分
        /// </summary>
        public UInt32 dwMinute;
        /// <summary>
        /// 秒
        /// </summary>
        public UInt32 dwSecond;
        /// <summary>
        /// 毫秒
        /// </summary>
        public UInt32 dwMillisecond;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt32[] dwReserved;
    }

    /// <summary>
    /// 二维空间点
    /// </summary>
    public struct DH_POINT
    {
        short nx;
        short ny;
    }


    /// <summary>
    /// 物体对应图片文件信息
    /// </summary>
    public struct DH_PIC_INFO
    {
        /// <summary>
        /// 文件在二进制数据块中的偏移位置, 单位:字节
        /// </summary>
        public UInt32 dwOffSet;
        /// <summary>
        /// 文件大小, 单位:字节
        /// </summary>
        public UInt32 dwFileLenth;
        /// <summary>
        /// 图片宽度, 单位:像素
        /// </summary>
        public UInt16 wWidth;
        /// <summary>
        /// 图片高度, 单位:像素
        /// </summary>
        public UInt16 wHeight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] bReserved;
    };



    /// <summary>
    /// 视频分析物体信息结构体
    /// </summary>
    public struct DH_MSG_OBJECT
    {
        /// <summary>
        /// 物体ID,每个ID表示一个唯一的物体
        /// </summary>
        public Int32 nObjectID;
        /// <summary>
        /// 物体类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szObjectType;
        /// <summary>
        /// 置信度(0~255)，值越大表示置信度越高
        /// </summary>
        public Int32 nConfidence;
        /// <summary>
        /// 物体动作:1:Appear 2:Move 3:Stay 4:Remove 5:Disappear 6:Split 7:Merge 8:Rename
        /// </summary>
        public Int32 nAction;
        /// <summary>
        /// 包围盒
        /// </summary>
        public DH_RECT BoundingBox;
        /// <summary>
        /// 物体型心
        /// </summary>
        public DH_POINT Center;
        /// <summary>
        /// 多边形顶点个数
        /// </summary>
        public Int32 nPolygonNum;
        /// <summary>
        /// 较精确的轮廓多边形
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public DH_POINT[] Contour;
        /// <summary>
        /// 物体主要颜色；按字节表示，分别为红、绿、蓝和透明度,例如:RGB值为(0,255,0),透明度为0时, 其值为0x00ff0000.
        /// </summary>
        public UInt32 rgbaMainColor;
        /// <summary>
        /// 物体上相关的带0结束符文本，比如车牌，集装箱号等等public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szText;
        /// <summary>
        /// 物体子类别，根据不同的物体类型，可以取以下子类型：
        /// Vehicle Category:"Unknown"  未知,"Motor" 机动车,"Non-Motor":非机动车,"Bus": 公交车,"Bicycle" 自行车,"Motorcycle":摩托车
        /// Plate Category："Unknown" 未知,"Normal" 蓝牌黑牌,"Yellow" 黄牌,"DoubleYellow" 双层黄尾牌,"Police" 警牌"Armed" 武警牌,
        /// "Military" 部队号牌,"DoubleMilitary" 部队双层,"SAR" 港澳特区号牌,"Trainning" 教练车号牌
        /// "Personal" 个性号牌,"Agri" 农用牌,"Embassy" 使馆号牌,"Moto" 摩托车号牌,"Tractor" 拖拉机号牌,"Other" 其他号牌
        /// HumanFace Category:"Normal" 普通人脸,"HideEye" 眼部遮挡,"HideNose" 鼻子遮挡,"HideMouth" 嘴部遮挡
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szObjectSubType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved1;
        /// <summary>
        /// 是否有物体对应图片文件信息
        /// </summary>
        public byte bPicEnble;
        /// <summary>
        /// 物体对应图片信息
        /// </summary>
        public DH_PIC_INFO stPicInfo;
        /// <summary>
        /// 是否是抓拍张的识别结果
        /// </summary>
        public byte bShotFrame;
        /// <summary>
        /// 物体颜色(rgbaMainColor)是否可用
        /// </summary>
        public byte bColor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 222)]
        public byte[] byReserved;
    }

    /// <summary>
    /// 事件对应文件信息
    /// </summary>
    public struct DH_EVENT_FILE_INFO
    {
        /// <summary>
        /// 当前文件所在文件组中的文件总数
        /// </summary>
        public byte bCount;
        /// <summary>
        /// 当前文件在文件组中的文件编号
        /// </summary>
        public byte bIndex;
        /// <summary>
        /// 文件标签，具体说明见枚举类型EM_EVENT_FILETAG
        /// </summary>
        public byte bFileTag;
        /// <summary>
        /// 文件类型，0-普通 1-合成 2-抠图
        /// </summary>
        public byte bFileType;
        /// <summary>
        /// 文件时间
        /// </summary>
        public NET_TIME_EX stuFileTime;
        /// <summary>
        /// 同一组抓拍文件的唯一标识
        /// </summary>
        public int nGroupId;
    }

    /// <summary>
    /// 图片分辨率
    /// </summary>
    public struct DH_RESOLUTION_INFO
    {
        /// <summary>
        /// 宽
        /// C++下类型是unsigned short
        /// </summary>
        public UInt16 snWidth;
        /// <summary>
        /// 高
        /// C++下类型是unsigned short
        /// </summary>
        public UInt16 snHight;
    }

    /// <summary>
    /// 车检器冗余信息
    /// </summary>
    public struct DH_SIG_CARWAY_INFO_EX
    {
        /// <summary>
        /// 由车检器产生抓拍信号冗余信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] byRedundance;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)]
        public byte[] bReserved;
    }


    public struct DEV_EVENT_TRAFFICGATE_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public Int32 nChannelID;
        /// <summary>
        /// 事件名称public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
        public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
        public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
        public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
        public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 对应车道号
        /// </summary>
        public Int32 nLane;
        /// <summary>
        /// 车辆实际速度Km/h
        /// </summary>
        public Int32 nSpeed;
        /// <summary>
        /// 速度上限 单位：km/h
        /// </summary>
        public Int32 nSpeedUpperLimit;
        /// <summary>
        /// 速度下限 单位：km/h 
        /// </summary>
        public Int32 nSpeedLowerLimit;
        /// <summary>
        /// 违反规则掩码,第一位:逆行; 
        /// 第二位:压线行驶; 第三位:超速行驶; 
        /// 第四位：欠速行驶; 第五位:闯红灯;第六位:穿过路口(卡口事件)
        /// 第七位: 压黄线; 第八位: 有车占道; 第九位: 黄牌占道;否则默认为:交通卡口事件
        /// </summary>
        public UInt32 dwBreakingRule;
        /// <summary>
        /// 事件对应文件信息  
        /// </summary>
        public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 车身信息
        /// </summary>
        public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 手动抓拍序号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szManualSnapNo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
        public int nSequence;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束; 
        /// </summary>
        public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
        /// <summary>
        /// 设备产生的抓拍标识
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szSnapFlag;
        /// <summary>
        /// 抓拍方式，0-未分类 1-全景 2-近景 4-同向抓拍 8-反向抓拍 16-号牌图像
        /// </summary>
        public byte bySnapMode;
        /// <summary>
        /// 超速百分比
        /// </summary>
        public byte byOverSpeedPercentage;
        /// <summary>
        /// 欠速百分比
        /// </summary>
        public byte byUnderSpeedingPercentage;
        /// <summary>
        /// 红灯容许间隔时间,单位：秒
        /// </summary>
        public byte byRedLightMargin;
        /// <summary>
        /// 行驶方向，0-上行(即车辆离设备部署点越来越近)，1-下行(即车辆离设备部署点越来越远)
        /// </summary>
        public byte byDriveDirection;
        /// <summary>
        /// 道路编号public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szRoadwayNo;
        /// <summary>
        /// 违章代码public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szViolationCode;
        /// <summary>
        /// 违章描述public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szViolationDesc;
        /// <summary>
        /// 对应图片的分辨率
        /// </summary>
        public DH_RESOLUTION_INFO stuResolution;
        /// <summary>
        /// 车辆大小类型，"Motor" 摩托车车 "Light-duty" 小型车 "Medium" 中型车 "Oversize" 大型车 "Huge" 超大车 "Other" 其他类型
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szVehicleType;
        /// <summary>
        /// 车辆长度, 单位米
        /// </summary>
        public byte byVehicleLenth;
        /// <summary>
        /// 保留字节,留待扩展
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved1;
        /// <summary>
        /// 限高速宽限值	单位：km/h 
        /// </summary>
        public Int32 nOverSpeedMargin;
        /// <summary>
        /// 限低速宽限值	单位：km/h 
        /// </summary>
        public Int32 nUnderSpeedMargin;
        /// <summary>
        /// "DrivingDirection" : ["Approach", "上海", "杭州"],行驶方向
        /// "Approach"-上行，即车辆离设备部署点越来越近；"Leave"-下行，
        /// 即车辆离设备部署点越来越远，第二和第三个参数分别代表上行和
        /// 下行的两个地点，UTF-8编码
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3 * 256)]
        public byte[] szDrivingDirection;
        /// <summary>
        /// 本地或远程设备名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] szMachineName;
        /// <summary>
        /// 机器部署地点、道路编码
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] szMachineAddress;
        /// <summary>
        /// 机器分组、设备所属单位
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] szMachineGroup;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 由车检器产生抓拍信号冗余信息
        /// </summary>
        public DH_SIG_CARWAY_INFO_EX stuSigInfo;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3964)]
        public byte[] bReserved;
    }

    // 对应CLIENT_StartSearchDevices接口
    public struct DEVICE_NET_INFO_EX
    {
        /// <summary>
        /// 4 for IPV4, 6 for IPV6
        /// </summary>
	    public Int32    iIPVersion;
        /// <summary>
        /// IP IPV4形如"192.168.0.1" IPV6形如"2008::1/64"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szIP;
        /// <summary>
        /// tcp端口
        /// </summary>
	    public Int32    nPort;
        /// <summary>
        /// 子网掩码 IPV6无子网掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szSubmask;
        /// <summary>
        /// 网关
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szGateway;
        /// <summary>
        /// MAC地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
	    public byte[] szMac;
        /// <summary>
        /// 设备类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szDeviceType;
        /// <summary>
        /// 目标设备的生产厂商,具体参考EM_IPC_TYPE类	
        /// </summary>
	    public byte byManuFactory;
        /// <summary>
        /// 1-标清 2-高清
        /// </summary>
	    public byte byDefinition;
        /// <summary>
        /// 原型是bool，一个字节
        /// Dhcp使能状态, true-开, false-关
        /// </summary>
	    public byte bDhcpEn;
        /// <summary>
        /// 字节对齐
        /// </summary>
	    public byte byReserved1;
        /// <summary>
        /// 校验数据 通过异步搜索回调获取(在修改设备IP时会用此信息进行校验)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 88)]
	    public byte[] verifyData;
        /// <summary>
        /// 序列号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
	    public byte[] szSerialNo;
        /// <summary>
        /// 设备软件版本号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szDevSoftVersion;
        /// <summary>
        /// 设备型号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szDetailType;
        /// <summary>
        /// OEM客户类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szVendor;
        /// <summary>
        /// 设备名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDevName;
        /// <summary>
        /// 登陆设备用户名（在修改设备IP时需要填写）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szUserName;
        /// <summary>
        /// 登陆设备密码（在修改设备IP时需要填写）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szPassWord;
        /// <summary>
        /// HTTP服务端口号
        /// </summary>
	    public short    nHttpPort;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 254)]
	    public byte[] cReserved;
    }

    #endregion

    #region 结构体--智能分析事件类型

    /// <summary>
    /// 事件类型EVENT_IVS_CROSSLINEDETECTION(警戒线事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_CROSSLINE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 规则检测线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectLine;
        /// <summary>
        /// 规则检测线顶点数
        /// </summary>
	    public int nDetectLineNum;
        /// <summary>
        /// 物体运动轨迹
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] TrackLine;
        /// <summary>
        /// 物体运动轨迹顶点数
        /// </summary>
	    public int nTrackLineNum;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public Byte bEventAction;
        /// <summary>
        /// 表示入侵方向, 0-由左至右, 1-由右至左
        /// </summary>
	    public Byte bDirection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public Byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON
        /// </summary>
	    public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 876)]
	    public Byte[] bReserved; 

    }

    /// <summary>
    /// 区域或曲线顶点信息
    /// </summary>
    public struct DH_POLY_POINTS
    {
        /// <summary>
        /// 顶点数
        /// </summary>
	    public Int32 nPointNum;
        /// <summary>
        /// 顶点信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] stuPoints; 
    }


    /// <summary>
    /// 事件类型EVENT_IVS_CROSSREGIONDETECTION(警戒区事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_CROSSREGION_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public int nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public int nDetectRegionNum;
        /// <summary>
        /// 物体运动轨迹
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] TrackLine;
        /// <summary>
        /// 物体运动轨迹顶点数
        /// </summary>
	    public int nTrackLineNum;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 表示入侵方向, 0-进入, 1-离开，2-出现，3-离开
        /// </summary>
	    public byte bDirection;
        /// <summary>
        /// 表示检测动作类型,0-出现 1-消失 2-在区域内 3-穿越区域
        /// </summary>
	    public byte bActionType;
        /// <summary>
        /// 对齐字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
	    public byte[] bReserved1; 
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON
        /// </summary>
        public UInt32 dwSnapFlagMask;
 	   /// <summary>
 	   /// 保留字节,留待扩展.
 	   /// </summary>
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 804)]
	    public byte[] bReserved; 
        /// <summary>
        /// 检测到的物体个数
        /// </summary>
	    public int nObjectNum;
        /// <summary>
        /// 检测到的物体
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_MSG_OBJECT[] stuObjectIDs;
        /// <summary>
        /// 轨迹数(与检测到的物体个数对应)
        /// </summary>
	    public int nTrackNum;
        /// <summary>
        /// 轨迹信息(与检测到的物体对应)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_POLY_POINTS[] stuTrackInfo;
    }

    /// <summary>
    /// 事件类型EVENT_IVS_PASTEDETECTION(贴条事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_PASTE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public int nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName; 
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 规则检测区域顶点数public 
        /// </summary>
	    public int nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion; 
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON
        /// </summary>
        public UInt32 dwSnapFlagMask;
 	   /// <summary>
 	   /// 保留字节,留待扩展.
 	   /// </summary>
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_LEFTDETECTION(物品遗留事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_LEFT_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public int nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public int nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]  
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved;

    }

    /// <summary>
    /// 事件类型EVENT_IVS_PRESERVATION(物品保全事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_PRESERVATION_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public int nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public int nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON
        /// </summary>
        public UInt32 dwSnapFlagMask;
 	   /// <summary>
 	   /// 保留字节,留待扩展.
 	   /// </summary>
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_STAYDETECTION(停留事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_STAY_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public int nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public int nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_WANDERDETECTION(徘徊事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_WANDER_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public int nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public int nEventID;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved; 
        /// <summary>
        /// 检测到的物体个数
        /// </summary>
	    public int nObjectNum;
        /// <summary>
        /// 检测到的物体
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_MSG_OBJECT[] stuObjectIDs;
        /// <summary>
        /// 轨迹数(与检测到的物体个数对应)
        /// </summary>
	    public Int32 nTrackNum;
        /// <summary>
        /// 轨迹信息(与检测到的物体对应)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_POLY_POINTS[] stuTrackInfo;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1020)]
	    public byte[] bReserved;
 
    }

    /// <summary>
    /// 事件类型EVENT_IVS_MOVEDETECTION(移动事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_MOVE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName; 
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON
        /// </summary>
        public UInt32 dwSnapFlagMask;
 	   /// <summary>
 	   /// 保留字节,留待扩展.
 	   /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_TAILDETECTION(尾随事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TAIL_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved; 

    } 


    /// <summary>
    /// 事件类型EVENT_IVS_RIOTERDETECTION(聚众事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_RIOTERL_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体个数
        /// </summary>
	    public Int32 nObjectNum;
        /// <summary>
        /// 检测到的物体列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_MSG_OBJECT[] stuObjectIDs;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved; 
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_FIREDETECTION(火警事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_FIRE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
    	/// <summary>
    	/// 抓图标志(按位)，具体见NET_RESERVED_COMMON
    	/// </summary>
        public UInt32 dwSnapFlagMask;
 	   /// <summary>
 	   /// 保留字节,留待扩展
 	   /// </summary>
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 888)]
	    public byte[] bReserved;

    }

    /// <summary>
    /// 事件类型EVENT_IVS_SMOKEDETECTION(烟雾报警事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_SMOKE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName; 
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID 
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON
        /// </summary>
        public UInt32 dwSnapFlagMask;
 	   /// <summary>
 	   /// 保留字节,留待扩展.
 	   /// </summary>
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 972)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_FLOWSTAT(流量统计事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_FLOWSTAT_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 从左边穿越的人的个数
        /// </summary>
	    public Int32 nNumberLeft;
        /// <summary>
        /// 从右边穿越的人的个数
        /// </summary>
	    public Int32 nNumberRight;
        /// <summary>
        /// 设置的上限
        /// </summary>
	    public Int32 nUpperLimit;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 972)]
	    public byte[] bReserved; 

    }


    
    /// <summary>
    /// 事件类型EVENT_IVS_NUMBERSTAT(数量统计事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_NUMBERSTAT_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName; 
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 区域内物体的个数
        /// </summary>
	    public Int32 nNumber;
        /// <summary>
        /// 设置的上限
        /// </summary>
	    public Int32 nUpperLimit;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved1;
        /// <summary>
        /// 表示进入区域或者出入口的内物体的个数
        /// </summary>
	    public Int32 nEnteredNumber;
        /// <summary>
        /// 表示出来区域或者出入口的内物体的个数
        /// </summary>
	    public Int32 nExitedNumber;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 964)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFICCONTROL(交通管制事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFICCONTROL_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 972)]
	    public byte[] bReserved; 
    }

    
    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFICACCIDENT(交通事故事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFICACCIDENT_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒) 
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体个数
        /// </summary>
	    public Int32 nObjectNum;
        /// <summary>
        /// 检测到的物体列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_MSG_OBJECT[] stuObjectIDs;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 972)]
	    public byte[] bReserved; 

    }

    /// <summary>
    /// TrafficCar 交通车辆信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO
    {
        /// <summary>
        /// 车牌号码
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szPlateNumber;
        /// <summary>
        /// 号牌类型	参见VideoAnalyseRule中车牌类型定义
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szPlateType;
        /// <summary>
        /// 车牌颜色	"Blue","Yellow", "White","Black"
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szPlateColor;
        /// <summary>
        /// 车身颜色	"White", "Black", "Red", "Yellow", "Gray", "Blue","Green"
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szVehicleColor;
        /// <summary>
        /// 速度	单位Km/H
        /// </summary>
	    public int nSpeed;
        /// <summary>
        /// 触发的相关事件	参见事件列表Event List，只包含交通相关事件。
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szEvent;
        /// <summary>
        /// 违章代码	详见TrafficGlobal.ViolationCode
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szViolationCode;
        /// <summary>
        /// 违章描述
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szViolationDesc; 
        /// <summary>
        /// 速度下限
        /// </summary>
	    public Int32 nLowerSpeedLimit;
        /// <summary>
        /// 速度上限
        /// </summary>
	    public Int32 nUpperSpeedLimit;
        /// <summary>
        /// 限高速宽限值	单位：km/h 
        /// </summary>
	    public Int32 nOverSpeedMargin;
        /// <summary>
        /// 限低速宽限值	单位：km/h 
        /// </summary>
	    public Int32 nUnderSpeedMargin;
        /// <summary>
        /// 车道	参见事件列表Event List中卡口和路口事件。
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车辆大小	按位第0位:"Light-duty",第1位:"Medium",第2位:"Oversize" 
        /// </summary>
	    public Int32 nVehicleSize;
        /// <summary>
        /// 车辆长度	单位米
        /// </summary>
	    public float fVehicleLength;
        /// <summary>
        /// 抓拍方式	0-未分类，1-全景，2-近景，4-同向抓拍，8-反向抓拍，16-号牌图像
        /// </summary>
	    public Int32 nSnapshotMode;
        /// <summary>
        /// 本地或远程的通道名称，可以是地点信息	来源于通道标题配置ChannelTitle.Name 
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szChannelName;
        /// <summary>
        /// 本地或远程设备名称	来源于普通配置General.MachineName
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szMachineName;
        /// <summary>
        /// 机器分组或叫设备所属单位	默认为空，用户可以将不同的设备编为一组，便于管理，可重复。
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szMachineGroup;
        /// <summary>
        /// 道路编号	UTF-8编码
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szRoadwayNo;
        /// <summary>
        /// "DrivingDirection" : ["Approach", "上海", "杭州"],行驶方向
        /// "Approach"-上行，即车辆离设备部署点越来越近；"Leave"-下行，
        /// 即车辆离设备部署点越来越远，第二和第三个参数分别代表上行和
        /// 下行的两个地点，UTF-8编码
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3 * 256)]
	    public byte[] szDrivingDirection;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
    	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[] bReserved; 
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFICJUNCTION(交通路口事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFICJUNCTION_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体 
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 违反规则掩码,第一位:闯红灯
        /// 第二位:不按规定车道行驶;
        /// 第三位:逆行; 第四位：违章掉头;
        /// 第五位:交通堵塞; 第六位:交通异常空闲
        /// 第七位:压线行驶; 否则默认为:交通路口事件
        /// </summary>
	    public UInt32 dwBreakingRule;
        /// <summary>
        /// 红灯开始UTC时间
        /// </summary>
	    public NET_TIME_EX RedLightUTC;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 车辆实际速度Km/h                 
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
	    public byte[] byReserved;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 476)]
        public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar; 

    }

    /// <summary>
    /// 抓拍信息
    /// </summary>
    public struct DH_SIG_CARWAY_INFO
    {
        /// <summary>
        /// 当前车的速度，km/h
        /// </summary>
	    public short snSpeed;
        /// <summary>
        /// 当前车长,分米为单位
        /// </summary>
	    public short snCarLength;
        /// <summary>
        /// 当前车道红灯时间,秒.毫秒
        /// </summary>
	    public float fRedTime;
        /// <summary>
        /// 当前车道抓拍时间,秒.毫秒 
        /// </summary>
	    public float fCapTime;
        /// <summary>
        /// 当前抓拍序号
        /// </summary>
	    public byte bSigSequence;
        /// <summary>
        /// 当前车道的抓拍类型
        /// 0: 雷达高限速;1: 雷达低限速;2: 车检器高限速;3:车检器低限速
        /// 4: 逆向;5: 闯红灯;6: 红灯亮;7: 红灯灭;8: 全部抓拍或者卡口
        /// </summary>
	    public byte bType;
        /// <summary>
        /// 闯红灯类型:01:左转红灯;02:直行红灯;03:右转红灯
        /// </summary>
	    public byte bDirection;
        /// <summary>
        /// 当前车道的红绿灯状态，0: 绿灯, 1: 红灯, 2: 黄灯
        /// </summary>
	    public byte bLightColor;
        /// <summary>
        /// 设备产生的抓拍标识
        /// </summary>
	    public byte[] bSnapFlag;
    }

    /// <summary>
    /// 每个车道的相关信息
    /// </summary>
    public struct DH_CARWAY_INFO
    {
        /// <summary>
        /// 当前车道号 
        /// </summary>
	    public byte bCarWayID;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public byte[] bReserve;
        /// <summary>
        /// 被触发抓拍的个数
        /// </summary>
	    public byte bSigCount;
        /// <summary>
        /// 当前车道上，被触发抓拍对应的抓拍信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public DH_SIG_CARWAY_INFO[]  stuSigInfo;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
	    public byte[] bReserved; 
    };

    /// <summary>
    /// 事件类型EVENT_TRAFFICSNAPSHOT(交通抓拍事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFICSNAPSHOT_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 保留字节 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserv;
        /// <summary>
        /// 触发抓拍的车道个数
        /// </summary>
	    public byte bCarWayCount;
        /// <summary>
        /// 触发抓拍的车道相关信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public DH_CARWAY_INFO[] stuCarWayInfo;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 340)]
	    public byte[] bReserved;
    }

    /// <summary>
    /// 事件类型EVENT_IVS_FACEDETECT(人脸检测事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_FACEDETECT_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] reserved;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 932)]
	    public byte[] bReserved;
    } 

    
    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_RUNREDLIGHT(交通-闯红灯事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_RUNREDLIGHT_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车牌信息
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息 
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 红绿灯状态 0:未知 1：绿灯 2:红灯 3:黄灯
        /// </summary>
	    public Int32 nLightState;
        /// <summary>
        /// 车速,km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1016)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    } 


    
    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_OVERLINE(交通-压线事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_OVERLINE_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车牌信息
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 车辆实际速度,Km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
    	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_RETROGRADE(交通-逆行事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_RETROGRADE_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车牌信息
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 车辆实际速度,Km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;

    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_TURNLEFT(交通-违章左转)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_TURNLEFT_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车牌信息
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 车辆实际速度,Km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;

    }


    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_TURNRIGHT(交通-违章右转)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_TURNRIGHT_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车牌信息
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 车辆实际速度,Km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    } 

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_UTURN(违章调头事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_UTURN_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 车辆实际速度,Km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
    	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_OVERSPEED(交通超速事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_OVERSPEED_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 车辆实际速度Km/h
        /// </summary>
        public Int32 nSpeed;
        /// <summary>
        /// 速度上限 单位：km/h
        /// </summary>
	    public Int32 nSpeedUpperLimit;
        /// <summary>
        /// 速度下限 单位：km/h 
        /// </summary>
	    public Int32 nSpeedLowerLimit;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;	
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    }

    
    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_UNDERSPEED(交通欠速事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_UNDERSPEED_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 事件对应文件信息
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 车辆实际速度Km/h
        /// </summary>
        public Int32 nSpeed;
        /// <summary>
        /// 速度上限 单位：km/h
        /// </summary>
	    public Int32 nSpeedUpperLimit;
        /// <summary>
        /// 速度下限 单位：km/h 
        /// </summary>
	    public Int32 nSpeedLowerLimit;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] bReserved1;
        /// <summary>
        /// 欠速百分比
        /// </summary>
	    public Int32 nUnderSpeedingPercentage;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节
        /// </summary>
    	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;

    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_PARKING(交通违章停车事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_PARKING_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] reserved;
        /// <summary>
        /// 开始停车时间
        /// </summary>
	    public NET_TIME_EX stuStartParkingTime;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束(bEventAction=2时此参数有效)
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 报警时间间隔，单位:秒。(此事件为连续性事件，在收到第一个此事件之后，
        /// 若在超过间隔时间后未收到此事件的后续事件，则认为此事件异常结束了)
        /// </summary>
	    public Int32 nAlarmIntervalTime;
        /// <summary>
        /// 允许停车时长，单位：秒。
        /// </summary>
	    public Int32 nParkingAllowedTime;
        /// <summary>
        /// 规则检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionNum;
        /// <summary>
        /// 规则检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public DH_POINT[] DetectRegion;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 924)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    	
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFICJAM(交通拥堵事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFICJAM_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束; 
        /// </summary>
	    public byte bEventAction;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] reserved;
        /// <summary>
        /// 开始停车时间
        /// </summary>
	    public NET_TIME_EX stuStartJamTime;
        /// <summary>
        /// 表示抓拍序号，如3,2,1,1表示抓拍结束,0表示异常结束(bEventAction=2时此参数有效)
        /// </summary>
	    public Int32 nSequence;
        /// <summary>
        /// 报警时间间隔，单位:秒。(此事件为连续性事件，在收到第一个此事件之后，
        /// 若在超过间隔时间后未收到此事件的后续事件，则认为此事件异常结束了)
        /// </summary>
	    public Int32 nAlarmIntervalTime;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息	
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_WRONGROUTE(交通违章-不按车道行驶)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_WRONGROUTE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 车辆实际速度，km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;

    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_CROSSLANE(交通违章-违章变道)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_CROSSLANE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;           
        /// <summary>
        /// 车辆实际速度，km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	 
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
	    public byte[] bReserved;
    	
    }


    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_OVERYELLOWLINE(交通违章-压黄线)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_OVERYELLOWLINE_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 车辆实际速度，km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    	
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_DRIVINGONSHOULDER(交通违章-路肩行驶事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_DRIVINGONSHOULDER_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID; 
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 车辆实际速度，km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
	    public byte[] bReserved;
    	
    }

    /// <summary>
    /// 事件类型EVENT_IVS_TRAFFIC_YELLOWPLATEINLANE(交通违章-黄牌车占道事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_TRAFFIC_YELLOWPLATEINLANE_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 事件名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// 时间戳(单位是毫秒)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// 事件发生的时间
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// 事件ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// 检测到的物体
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// 车身信息
        /// </summary>
	    public DH_MSG_OBJECT stuVehicle;
        /// <summary>
        /// 对应车道号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 事件对应文件信息               
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// 事件动作，0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 车辆实际速度，km/h
        /// </summary>
	    public Int32 nSpeed;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        /// <summary>
        /// 保留字节,留待扩展.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1020)]
	    public byte[] bReserved;
        /// <summary>
        /// 交通车辆信息
        /// </summary>
	    public DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;
    	
    }

    /// <summary>
    /// 事件类型 EVENT_IVS_FIREDETECTION(电火花事件)对应的数据块描述信息
    /// </summary>
    public struct DEV_EVENT_ELECTROSPARK_INFO 
    {
        /// <summary>
        /// ChannelId
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// event name
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szName;
        /// <summary>
        /// PTS(ms)
        /// </summary>
	    public double PTS;
        /// <summary>
        /// the event happen time
        /// </summary>
	    public NET_TIME_EX UTC;
        /// <summary>
        /// event ID
        /// </summary>
	    public Int32 nEventID;
        /// <summary>
        /// have being detected object
        /// </summary>
	    public DH_MSG_OBJECT stuObject;
        /// <summary>
        /// event file info
        /// </summary>
	    public DH_EVENT_FILE_INFO  stuFileInfo;
        /// <summary>
        /// Event action，0 means pulse event,1 means continuous event's begin,2means continuous event's end;
        /// </summary>
	    public byte bEventAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 抓图标志(按位)，具体见NET_RESERVED_COMMON	
        /// </summary>
        public UInt32 dwSnapFlagMask;
        //reserved
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 972)]
	    public byte[] bReserved;
    	
    }

    #endregion

    #region <<CtrlType枚举相关结构体>>

    /// <summary>
    /// 手动抓拍参数
    /// </summary>
    public struct MANUAL_SNAP_PARAMETER
    {
        /// <summary>
        /// 抓图通道,从0开始
        /// </summary>
        public Int32 nChannel;
        /// <summary>
        /// 抓图序列号字符串
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] bySequence;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public byte[] byReserved;
    }

    /// <summary>
    /// 控制设备端本地预览分割参数
    /// </summary>
    public struct DEVICE_LOCALPREVIEW_SLIPT_PARAMETER
    {
        /// <summary>
        /// 分割模式，见设备端本地预览支持的分割模式
        /// </summary>
        public Int32 nSliptMode;
        /// <summary>
        /// 当前要预览的子分割,从1开始
        /// </summary>
        public Int32 nSubSliptNum;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] byReserved;
    }

    public struct WIFI_CONNECT
    {
        /// <summary>
        /// SSID
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szSSID;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] bReserved;
    };

    /// <summary>
    /// 硬盘操作
    /// </summary>
    public struct DISKCTRL_PARAM
    {
        /// <summary>
        /// 结构体大小，版本控制用
        /// </summary>
	    public UInt32 dwSize; 
        /// <summary>
        /// 为硬盘信息结构体DH_HARDDISK_STATE里的数组stDisks下标，从0开始
        /// </summary>
	    public Int32 nIndex;
        /// <summary>
        /// 操作类型，
        /// 0 - 清除数据, 1 - 设为读写盘, 2 - 设为只读盘
        /// 3 - 设为冗余盘, 4 - 恢复错误, 5 - 设为快照盘，7 - 弹出SD卡（对SD卡操作有效）
        /// </summary>
	    public Int32 ctrlType;
        /// <summary>
        /// 磁盘信息, 由于磁盘顺序可能改变导致下标不准, 用来代替下标
        /// </summary>
	    NET_DEV_DISKSTATE	stuDisk;
    }

    public struct  DISKCTRL_SUBAREA
    {
        /// <summary>
        /// 预分区的个数
        /// </summary>
	    public byte bSubareaNum;
        /// <summary>
        /// 为硬盘信息结构体DH_HARDDISK_STATE里的数组stDisks下标，从0开始
        /// </summary>
	    public byte bIndex;
        /// <summary>
        /// 分区大小（百分比）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] bSubareaSize;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
	    public byte[] bReserved; 
    }

    /// <summary>
    /// 报警状态
    /// </summary>
    public struct ALARMCTRL_PARAM
    {
	    public UInt32 dwSize;
        /// <summary>
        /// 报警通道号，从0开始
        /// </summary>
	    public Int32 nAlarmNo; 
        /// <summary>
        ///  1：触发报警，0：停止报警
        /// </summary>
	    public Int32 nAction;
    }

    /// <summary>
    /// 矩阵控制
    /// </summary>
    public struct MATRIXCTRL_PARAM
    {
	    public UInt32 dwSize;
        /// <summary>
        /// 视频输入号，从0开始
        /// </summary>
	    public Int32 nChannelNo;
        /// <summary>
        /// 矩阵输出号，从0开始
        /// </summary>
	    public Int32 nMatrixNo;
    }
 
    /// <summary>
    /// 刻录控制
    /// </summary>
    public struct BURNNG_PARM
    {
        /// <summary>
        /// 通道掩码，按位表示要刻录的通道
        /// </summary>
	    public Int32 channelMask;
        /// <summary>
        /// 刻录机掩码，根据查询到的刻录机列表，按位表示
        /// </summary>
	    public byte devMask;
        /// <summary>
        /// 画中画通道(通道数+32)
        /// </summary>
	    public byte bySpicalChannel;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public byte[] byReserved; 
    }

    /// <summary>
    /// 附件刻录
    /// </summary>
    public struct BURNING_PARM_ATTACH
    {
        /// <summary>
        /// 是否为附件刻录，0:不是; 1:是
        /// </summary>
	    public Int32 bAttachBurn;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
	    public byte[] bReserved; 
    } 

    /// <summary>
    /// 触发设备抓图，叠加卡号信息
    /// </summary>
    public struct NET_SNAP_COMMANDINFO 
    {
        /// <summary>
        /// 卡号信息
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szCardInfo;
        /// <summary>
        /// 保留
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] reserved;
    }

    public struct  BACKUP_RECORD
    {
        /// <summary>
        /// 备份设备名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szDeviceName;
        /// <summary>
        /// 备份记录数量
        /// </summary>
	    public Int32 nRecordNum;
        /// <summary>
        /// 备份记录信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public NET_RECORDFILE_INFO[]	stuRecordInfo;
    } 

    public struct DHDEV_VEHICLE_WIFI_CONFIG
    {
        /// <summary>
        /// SSID
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szSSID;
        /// <summary>
        /// 优先级,(1-32)
        /// </summary>
        public int nPriority;
        /// <summary>
        /// 校验类型
        /// 0:OPEN 
        /// 1:RESTRICTE
        /// 2:WEP
        /// 3:WPA
        /// 4:WPA2
        /// 5:WPA-PSK
        /// 6:WPA2-PSK
        /// </summary>
	    public int nSafeType;
        /// <summary>
        /// 加密方式
        /// 0:OPEN
        /// 1:TKIP
        /// 2:WEP
        /// 3:AES
        /// 4:NONE(不校验)
        /// </summary>
	    public int nEncryprion;
        /// <summary>
        /// 连接密钥
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szKey;
        /// <summary>
        /// 主机地址
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[]	szHostIP;
        /// <summary>
        /// 主机掩码
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[]	szHostNetmask;
        /// <summary>
        /// 主机网关
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[]	szHostGateway;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[]	bReserved;
    } 

    public struct DHDEV_USER_REJECT_INFO
    {
        /// <summary>
        /// ip
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szIpAddress;
        /// <summary>
        /// User Group
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szUserGroup;
        /// <summary>
        /// User Name
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szUserName;
        /// <summary>
        /// c++下类型为public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] reserved;
    }

    /// <summary>
    /// 剔除用户
    /// </summary>
    public struct DHDEV_REJECT_USER
    { 
        /// <summary>
        /// 用户数量
        /// </summary>
	    public Int32 nUserCount; 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
	    public DHDEV_USER_REJECT_INFO[] stuUserInfo;
        /// <summary>
        /// c++下类型为public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] reserved;
    }

    public struct DHDEV_USER_SHIELD_INFO
    {
        /// <summary>
        /// ip地址
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szIpAddress;
        /// <summary>
        /// 用户组
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szUserGroup;
        /// <summary>
        /// 用户名
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szUserName;
        /// <summary>
        /// 屏蔽时间
        /// public byte[]
        /// </summary>
	    public Int32 nForbiddenTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] reserved;
    }

    /// <summary>
    /// 屏蔽用户
    /// </summary>
    public struct DHDEV_SHIELD_USER
    { 
        /// <summary>
        /// 用户数量
        /// </summary>
	    public Int32 nUserCount; 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
	    public DHDEV_USER_SHIELD_INFO[] stuUserInfo;     
        /// <summary>
        /// c++下类型为public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] reserved;
    }

    #endregion

    #region <<结构体--能力集命令--对应CLIENT_QueryNewSystemInfo>>


    // 场景支持的规则
    public struct SCENE_SUPPORT_RULE
    {
        /// <summary>
        /// 规则类型
        /// </summary>
	    public UInt32 dwSupportedRule;
        /// <summary>
        /// 当前规则类型支持的检测物体类型个数
        /// </summary>
	    public Int32 nSupportedObjectTypeNum;
        /// <summary>
        /// 当前规则类型支持的检测物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypeName;
        /// <summary>
        /// 当前规则类型支持的检测物体动作个数
        /// </summary>
	    public int nSupportedActionsNum;
        /// <summary>
        /// 当前规则类型支持的检测物体动作列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szSupportedActions;
        /// <summary>
        /// 当前规则类型支持的检测类型个数
        /// </summary>
	    public Int32 nSupportedDetectTypeNum;
        /// <summary>
        /// 当前规则类型支持的检测类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szSupportedDetectTypes; 
    }

    /// <summary>
    /// 标定区域能力信息
    /// </summary>
    public struct CFG_CAP_CELIBRATE_AREA
    {
        /// <summary>
        /// 标定区域类型
        /// </summary>
	    public EM_CALIBRATEAREA_TYPE  emType;
        /// <summary>
        /// 支持的水平标尺最大个数
        /// </summary>
        public byte byMaxHorizontalStaffNum;
        /// <summary>
        /// 支持的水平标尺最小个数
        /// </summary>
        public byte byMinHorizontalStaffNum;
        /// <summary>
        /// 支持的垂直标尺最大个数
        /// </summary>
        public byte byMaxVerticalStaffNum;
        /// <summary>
        /// 支持的垂直标尺最小个数
        /// </summary>
        public byte byMinVerticalStaffNum;
    }

    /// <summary>
    /// 场景能力
    /// </summary>
    public struct CFG_CAP_SCENE
    {
        /// <summary>
        /// 场景名称
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
	    public byte[] szSceneName; 
        /// <summary>
        /// 当前规则类型支持的检测物体类型个数
        /// </summary>
	    public Int32 nSupportedObjectTypeNum;
        /// <summary>
        /// 当前规则类型支持的检测物体类型列表
        /// public byte[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypeName;
        /// <summary>
        /// 支持的规则个数
        /// </summary>
	    public Int32 nSupportRules;
        /// <summary>
        /// 支持的规则列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public SCENE_SUPPORT_RULE[] stSpportRules;

	    //支持的模块参数
        /// <summary>
        /// 是否支持扰动强度设置
        /// </summary>
	    public byte bDisturbance;
        /// <summary>
        /// 是否支持去扰动处理
        /// </summary>
	    public byte bAntiDisturbance;
        /// <summary>
        /// 是否支持逆光处理
        /// </summary>
	    public byte bBacklight;
        /// <summary>
        /// 是否支持阴影处理
        /// </summary>
	    public byte bShadow;
        /// <summary>
        /// 是否支持检测精度
        /// </summary>
	    public byte bAccuracy;
        /// <summary>
        /// 是否支持检测步长
        /// </summary>
	    public byte bMovingStep;
        /// <summary>
        /// 是否支持检测缩放
        /// </summary>
	    public byte bScalingFactor;
        /// <summary>
        /// 是否支持Y分量判定阈值
        /// </summary>
	    public byte bThresholdY;
        /// <summary>
        /// 是否支持UV分量判定阈值
        /// </summary>
	    public byte bThresholdUV;
        /// <summary>
        /// 是否支持边缘检测判定阈值
        /// </summary>
	    public byte bThresholdEdge;
        /// <summary>
        /// 是否支持检测平衡
        /// </summary>
	    public byte bDetectBalance;
        /// <summary>
        /// 是否支持算法序号
        /// </summary>
	    public byte bAlgorithmIndex;
        /// <summary>
        /// 是否支持高光处理，即Backgroud参数 
        /// </summary>
	    public byte bHighlight;
        /// <summary>
        /// 是否支持物体抓图
        /// </summary>
	    public byte bSnapshot;

	    //支持的场景参数
        /// <summary>
        /// 是否摄像头位置参数
        /// </summary>
	    public byte bCameraAspect;
        /// <summary>
        /// 是否支持抖动参数
        /// </summary>
	    public byte bJitter;
        /// <summary>
        /// 是否支持去抖动处理参数
        /// </summary>
	    public byte bDejitter;

	    // 支持的标定能力集
        /// <summary>
        /// 最大标定区域个数
        /// </summary>
	    public int nMaxCalibrateAreaNum;
        /// <summary>
        /// 标定区域能力信息个数
        /// </summary>
	    public int nCalibrateAreaNum;
        /// <summary>
        /// 标定区域能力信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	    public CFG_CAP_CELIBRATE_AREA[] stCalibrateAreaCap;

    };


    /// <summary>
    /// 场景列表
    /// </summary>
    public struct CFG_VACAP_SUPPORTEDSCENES
    {
        /// <summary>
        /// 支持的场景个数
        /// </summary>
	    public Int32 nScenes;
        /// <summary>
        /// 支持的场景列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public CFG_CAP_SCENE[] stScenes;
    }


    public struct DEVICE_STATUS
    {
        /// <summary>
        /// 远程设备的名字
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[]szDeviceName;
        /// <summary>
        /// 远程设备的状态 0：断线 1：在线
        /// </summary>
	    public byte	bDeviceStatus;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
	    public byte[] bReserved;
    }


    public struct CFG_REMOTE_DEVICE_STATUS
    {
        /// <summary>
        /// 设备状态
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public DEVICE_STATUS[] devStatus;
        /// <summary>
        /// 设备数量
        /// </summary>
	    public UInt32 dwDevCount;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[] bReserved;
    }

    /// <summary>
    /// 视频分析设备能力信息
    /// </summary>
    public struct CFG_CAP_DEVICE_ANALYSE_INFO
    {
        /// <summary>
        /// 支持智能分析的最大通道数
        /// </summary>
	    public Int32 nMaxChannels;
    }

    /// <summary>
    /// 视频输入能力集(CFG_CAP_CMD_VIDEOINPUT)配置
    /// </summary>
    public struct CFG_CAP_VIDEOINPUT_INFO
    {
        /// <summary>
        /// 最大测光区域个数
        /// </summary>
	    public Int32 nMeteringRegionCount;
    };


    /// <summary>
    /// 时间
    /// </summary>
    public struct CFG_NET_TIME
    {
	    Int32 nStructSize;
        /// <summary>
        /// 年
        /// </summary>
	    UInt32 dwYear;
        /// <summary>
        /// 月
        /// </summary>
        UInt32 dwMonth;
        /// <summary>
        /// 日
        /// </summary>
        UInt32 dwDay;
        /// <summary>
        /// 时
        /// </summary>
        UInt32 dwHour;
        /// <summary>
        /// 分
        /// </summary>
        UInt32 dwMinute;
        /// <summary>
        /// 秒
        /// </summary>
        UInt32 dwSecond;
    }


    public struct CFG_ACTIVEUSER_INFO
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 活动用户ID，一般是会话ID
        /// </summary>
	    public Int32 nUserID;
        /// <summary>
        /// 用户名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szUser;
        /// <summary>
        /// 用户所在组名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szGroupName;
        /// <summary>
        /// 用户所在组等级
        /// </summary>
	    public int  nGroupLevel;
        /// <summary>
        /// 客户端类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szClientType;
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szClientAddr;
        /// <summary>
        /// 用户登入时间
        /// </summary>
	    public CFG_NET_TIME  stuLoginTime;
    };


    public struct CFG_NET_TIME_EX
    {
        /// <summary>
        /// 年
        /// </summary>
	    public UInt32 dwYear;
        /// <summary>
        /// 月
        /// </summary>
	    public UInt32 dwMonth;
        /// <summary>
        /// 日
        /// </summary>
	    public UInt32 dwDay;
        /// <summary>
        /// 时
        /// </summary>
	    public UInt32 dwHour;
        /// <summary>
        /// 分
        /// </summary>
	    public UInt32 dwMinute;
        /// <summary>
        /// 秒
        /// </summary>
	    public UInt32 dwSecond;
        /// <summary>
        /// 毫秒
        /// </summary>
	    public UInt32 dwMillisecond;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public UInt32[] dwReserved;
    }

    /// <summary>
    /// 获取视频统计摘要信息结构体
    /// </summary>
    public struct CFG_VIDEOSATA_SUMMARY_INFO
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 统计通道号
        /// </summary>
        public Int32 nChannelID;
        /// <summary>
        /// 规则名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szRuleName;
        /// <summary>
        /// 统计时间，转换到UTC
        /// </summary>
        public CFG_NET_TIME_EX stuStatTime;
        /// <summary>
        /// 进入总计
        /// </summary>
        public Int32 nEnteredTotal;
        /// <summary>
        /// 今天进入总计
        /// </summary>
        public Int32 nEnteredToday;
        /// <summary>
        /// 本月进入总计
        /// </summary>
        public Int32 nEnteredMonth;
        /// <summary>
        /// 今年进入总计
        /// </summary>
        public Int32 nEnteredYear;
        /// <summary>
        /// 每日最大进入总计
        /// </summary>
        public Int32 nEnteredDaily;
        /// <summary>
        /// 出去总计
        /// </summary>
        public Int32 nExitedTotal;
        /// <summary>
        /// 今天出去总计
        /// </summary>
        public Int32 nExitedToday;
        /// <summary>
        /// 本月出去总计
        /// </summary>
        public Int32 nExitedMonth;
        /// <summary>
        /// 今年出去总计
        /// </summary>
        public Int32 nExitedYear;
        /// <summary>
        /// 每日最大出去总计
        /// </summary>
        public Int32 nExitedDaily;
        /// <summary>
        /// 平均所有保有统计(除去零值)
        /// </summary>
        public Int32 nAvgTotal;
        /// <summary>
        /// 平均今天保有统计
        /// </summary>
        public Int32 nAvgToday;
        /// <summary>
        /// 平均本月保有统计
        /// </summary>
        public Int32 nAvgMonth;
        /// <summary>
        /// 平均今年保有统计         
        /// </summary>
        public Int32 nAvgYear;
        /// <summary>
        /// 最大所有保有统计(除去零值)
        /// </summary>
        public Int32 nMaxTotal;
        /// <summary>
        /// 最大今天保有统计
        /// </summary>
        public Int32 nMaxToday;
        /// <summary>
        /// 最大本月保有统计
        /// </summary>
        public Int32 nMaxMonth;
        /// <summary>
        /// 最大今年保有统计
        /// </summary>
        public Int32 nMaxYear;
    }

    // 视频诊断服务能力集(CFG_CAP_CMD_VIDEODIAGNOSIS_SERVER)
    public struct CFG_VIDEODIAGNOSIS_CAPS_INFO
    {
        /// <summary>
        /// 支持的视频诊断类型 个数
        /// </summary>
	    public Int32 nTypeCount;
        /// <summary>
        /// 支持的视频诊断类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11 * 260)]
	    public byte[] szSupportedType;
        /// <summary>
        /// 最大参数表个数
        /// </summary>
	    public Int32 nMaxProfiles;
        /// <summary>
        /// 最大任务个数
        /// </summary>
	    public Int32 nMaxTasks;
        /// <summary>
        /// 最大单个任务的视频源个数
        /// </summary>
	    public Int32 nMaxSourcesOfTask;
        /// <summary>
        /// 最大方案个数
        /// </summary>
	    public Int32 nMaxProjects;
    }

    #endregion

    #region <<结构体--配置命令 对应CLIENT_GetNewDevConfig和CLIENT_SetNewDevConfig接口>>



    /// <summary>
    /// 视频格式
    /// </summary>
    public struct CFG_VIDEO_FORMAT
    {
	    // 能力
	    public byte abCompression;
        public byte abWidth;
        public byte abHeight;
        public byte abBitRateControl;
        public byte abBitRate;
        public byte abFrameRate;
        public byte abIFrameInterval;
        public byte abImageQuality;
        public byte abFrameType;

	    // 信息
        /// <summary>
        /// 视频压缩格式
        /// </summary>
        public CFG_VIDEO_COMPRESSION emCompression;
        /// <summary>
        /// 视频宽度
        /// </summary>
        public Int32 nWidth;
        /// <summary>
        /// 视频高度
        /// </summary>
        public Int32 nHeight;
        /// <summary>
        /// 码流控制模式
        /// </summary>
        public CFG_BITRATE_CONTROL emBitRateControl;
        /// <summary>
        /// 视频码流(kbps)
        /// </summary>
        public Int32 nBitRate;
        /// <summary>
        /// 视频帧率
        /// </summary>
        public Int32 nFrameRate;
        /// <summary>
        /// I帧间隔(1-100)，比如50表示每49个B帧或P帧，设置一个I帧。
        /// </summary>
        public Int32 nIFrameInterval;
        /// <summary>
        /// 图像质量
        /// </summary>
        public CFG_IMAGE_QUALITY emImageQuality;
        /// <summary>
        /// 打包模式，0－DHAV，1－"PS"
        /// </summary>
        public Int32 nFrameType;
    } 

    /// <summary>
    /// 视频编码参数
    /// </summary>
    public struct CFG_VIDEOENC_OPT
    {
	    // 能力
	    public byte abVideoEnable;
        public byte abAudioEnable;
        public byte abSnapEnable;
        /// <summary>
        /// 音频叠加能力
        /// </summary>
        public byte abAudioAdd;

	    // 信息
        /// <summary>
        /// 视频使能
        /// </summary>
        public bool bVideoEnable;
        /// <summary>
        /// 视频格式
        /// </summary>
        public CFG_VIDEO_FORMAT stuVideoFormat;
        /// <summary>
        /// 音频使能
        /// </summary>
        public bool bAudioEnable;
        /// <summary>
        /// 定时抓图使能
        /// </summary>
        public bool bSnapEnable;
        /// <summary>
        /// 音频叠加使能
        /// </summary>
        public bool bAudioAddEnable;
    } 

    /// <summary>
    /// RGBA信息
    /// </summary>
    public struct CFG_RGBA
    {
	    Int32 nRed;
	    Int32 nGreen;
	    Int32 nBlue;
	    Int32 nAlpha;
    }

    /// <summary>
    /// 遮挡信息
    /// </summary>
    public struct CFG_COVER_INFO
    {
	    // 能力
	    public byte abBlockType;
        public byte abEncodeBlend;
        public byte abPreviewBlend;

	    // 信息
        /// <summary>
        /// 覆盖的区域坐标
        /// </summary>
        public CFG_RECT stuRect;
        /// <summary>
        /// 覆盖的颜色
        /// </summary>
        public CFG_RGBA stuColor;
        /// <summary>
        /// 覆盖方式；0－黑块，1－马赛克
        /// </summary>
        public Int32 nBlockType;
        /// <summary>
        /// 编码级遮挡；1－生效，0－不生效
        /// </summary>
        public Int32 nEncodeBlend;
        /// <summary>
        /// 预览遮挡；1－生效，0－不生效
        /// </summary>
        public Int32 nPreviewBlend;
    } 

    /// <summary>
    /// 多区域遮挡配置
    /// </summary>
    public struct CFG_VIDEO_COVER
    {
        /// <summary>
        /// 支持的遮挡块数
        /// </summary>
	    public Int32 nTotalBlocks;
        /// <summary>
        /// 已设置的块数
        /// </summary>
	    public Int32 nCurBlocks;
        /// <summary>
        /// 覆盖的区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public CFG_COVER_INFO[] stuCoverBlock;
    } 

    /// <summary>
    /// OSD信息
    /// </summary>
    public struct CFG_OSD_INFO
    {
	    // 能力
	    public byte abShowEnable;

	    // 信息
        /// <summary>
        /// 前景颜色
        /// </summary>
        public CFG_RGBA stuFrontColor;
        /// <summary>
        /// 背景颜色
        /// </summary>
        public CFG_RGBA stuBackColor;
        /// <summary>
        /// 矩形区域
        /// </summary>
        public CFG_RECT stuRect;
        /// <summary>
        /// 显示使能
        /// </summary>
        public bool bShowEnable;
    }

    /// <summary>
    /// 画面颜色属性
    /// </summary>
    public struct CFG_COLOR_INFO
    {
        /// <summary>
        /// 亮度(0-100)
        /// </summary>
	    public Int32 nBrightness;
        /// <summary>
        /// 对比度(0-100) 
        /// </summary>
        public Int32 nContrast;
        /// <summary>
        /// 饱和度(0-100)
        /// </summary>
        public Int32 nSaturation;
        /// <summary>
        /// 色度(0-100)
        /// </summary>
        public Int32 nHue;
        /// <summary>
        /// 增益(0-100)
        /// </summary>
        public Int32 nGain;
        /// <summary>
        /// 增益使能
        /// </summary>
        public bool bGainEn;
    } ;

    /// <summary>
    /// 图像通道属性信息
    /// </summary>
    public struct CFG_ENCODE_INFO
    {
        /// <summary>
        /// 通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 通道名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szChnName;
        /// <summary>
        /// 主码流，0－普通录像，1-动检录像，2－报警录像
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public CFG_VIDEOENC_OPT[]	stuMainStream;
        /// <summary>
        /// 辅码流，0－辅码流1，1－辅码流2，2－辅码流3
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public CFG_VIDEOENC_OPT[]	stuExtraStream;
        /// <summary>
        /// 抓图，0－普通抓图，1－动检抓图，2－报警抓图
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public CFG_VIDEOENC_OPT[]	stuSnapFormat;
        /// <summary>
        /// 区域遮盖能力掩码，按位分别是本地预览、录像及网络监视
        /// </summary>
	    public UInt32 dwCoverAbilityMask;
        /// <summary>
        /// 区域遮盖使能掩码，按位分别是本地预览、录像及网络监视
        /// </summary>
	    public UInt32 dwCoverEnableMask;
        /// <summary>
        /// 区域覆盖
        /// </summary>
	    public CFG_VIDEO_COVER stuVideoCover;
        /// <summary>
        /// 通道标题
        /// </summary>
	    public CFG_OSD_INFO stuChnTitle;
        /// <summary>
        /// 时间标题
        /// </summary>
	    public CFG_OSD_INFO stuTimeTitle;
        /// <summary>
        /// 画面颜色
        /// </summary>
	    public CFG_COLOR_INFO stuVideoColor;
        /// <summary>
        /// 音频格式: 0:G711A,1:PCM,2:G711U,3:AMR,4:AAC
        /// </summary>
	    public CFG_AUDIO_FORMAT emAudioFormat;
    } ;

    /// <summary>
    /// 编码格式, 包括音频和视频
    /// </summary>
    public struct AV_CFG_EncodeFormat
    {
        public Int32 nStructSize;
        /// <summary>
        /// 音频使能
        /// </summary>
        public bool bAudioEnable;
        /// <summary>
        /// 音频比特率
        /// </summary>
        public Int32 nAudioBitRate;
        /// <summary>
        /// 音频压缩模式
        /// </summary>
        public CFG_AUDIO_FORMAT emAudioCompression;
        /// <summary>
        /// 音频采样深度
        /// </summary>
        public Int32 nAudioDepth;
        /// <summary>
        /// 音频采样频率
        /// </summary>
        public Int32 nAudioFrequency;
        /// <summary>
        /// 音频编码模式
        /// </summary>
        public Int32 nAudioMode;
        /// <summary>
        /// 音频打包模式, 0-DHAV, 1-PS
        /// </summary>
        public Int32 nAudioPack;
        /// <summary>
        /// 视频使能
        /// </summary>
        public bool bVideoEnable;
        /// <summary>
        /// 视频比特率
        /// </summary>
        public Int32 nVideoBitRate;
        /// <summary>
        /// 码流控制模式
        /// </summary>
        public CFG_BITRATE_CONTROL emVideoBitRateControl;
        /// <summary>
        /// 视频压缩模式
        /// </summary>
        public CFG_VIDEO_COMPRESSION emVideoCompression;
        /// <summary>
        /// 视频帧率
        /// </summary>
        public Int32 nVideoFPS;
        /// <summary>
        /// 视频I帧间隔
        /// </summary>
        public Int32 nVideoGOP;
        /// <summary>
        /// 视频宽度
        /// </summary>
        public Int32 nVideoWidth;
        /// <summary>
        /// 视频高度
        /// </summary>
        public Int32 nVideoHeight;
        /// <summary>
        /// 视频图像质量
        /// </summary>
        public CFG_IMAGE_QUALITY emVideoQuality;
        /// <summary>
        /// 视频打包模式, 0-DHAV, 1-PS	
        /// </summary>
        public Int32 nVideoPack;
    };

    /// <summary>
    /// 编码配置
    /// </summary>
    public struct AV_CFG_Encode 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 主码流, 包括普通编码, 动检编码, 报警编码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public AV_CFG_EncodeFormat[] stuMainFormat;
        /// <summary>
        /// 辅码流, 包括辅码流1, 辅码流2, 辅码流3
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public AV_CFG_EncodeFormat[]	stuExtraFormat;
        /// <summary>
        /// 抓图, 包括普通抓图, 动检抓图, 报警抓图
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public AV_CFG_EncodeFormat[]	stuSnapFormat;
    }

    /// <summary>
    /// 时间段信息
    /// </summary>
    public struct CFG_TIME_SECTION 
    {
        /// <summary>
        /// 录像掩码，按位分别为动态检测录像、报警录像、定时录像、Bit3~Bit15保留、
        /// Bit16动态检测抓图、Bit17报警抓图、Bit18定时抓图
        /// </summary>
	    UInt32 dwRecordMask;
	    Int32 nBeginHour;
	    Int32 nBeginMin;
	    Int32 nBeginSec;
	    Int32 nEndHour;
	    Int32 nEndMin;
	    Int32 nEndSec;	
    } 


    /// <summary>
    /// 定时录像配置信息
    /// </summary>
    public struct CFG_RECORD_INFO
    {
        /// <summary>
        /// 通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 时间表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
        public CFG_TIME_SECTION[] stuTimeSection;
        /// <summary>
        /// 预录时间，为零时表示关闭(0~300)
        /// </summary>
        public Int32 nPreRecTime;
        /// <summary>
        /// 录像冗余开关
        /// </summary>
        public bool bRedundancyEn;
        /// <summary>
        /// 0－主码流，1－辅码流1，2－辅码流2，3－辅码流3
        /// </summary>
        public Int32 nStreamType;
    }

    /// <summary>
    /// 时间段
    /// </summary>
    public struct AV_CFG_TimeSection
    {
        public Int32 nStructSize;
        /// <summary>
        /// 掩码
        /// </summary>
        public Int32 nMask;
        /// <summary>
        /// 开始时间
        /// </summary>
        public Int32 nBeginHour;
        public Int32 nBeginMinute;
        public Int32 nBeginSecond;
        /// <summary>
        /// 结束时间
        /// </summary>
        public Int32 nEndHour;
        public Int32 nEndMinute;
        public Int32 nEndSecond;
    };
    
    // 录像配置
    public struct AV_CFG_Record 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 预录时间, 单位s
        /// </summary>
        public Int32 nPreRecord; 
        /// <summary>
        /// 时间段 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
        public AV_CFG_TimeSection[] stuTimeSection;
    };

    /// <summary>
    /// 联动云台信息
    /// </summary>
    public struct CFG_PTZ_LINK
    {
        /// <summary>
        /// 联动类型
        /// </summary>
	    public CFG_LINK_TYPE emType;
        /// <summary>
        /// 联动取值分别对应预置点号，巡航号等等
        /// </summary>
	    public Int32 nValue; 
    }

    /// <summary>
    /// 报警联动信息
    /// </summary>
    public struct CFG_ALARM_MSG_HANDLE
    {
	    //能力
	    public byte abRecordMask;
	    public byte abRecordEnable;
	    public byte abRecordLatch;
	    public byte abAlarmOutMask;
	    public byte abAlarmOutEn;
	    public byte abAlarmOutLatch;	
	    public byte abExAlarmOutMask;
	    public byte abExAlarmOutEn;
	    public byte abPtzLinkEn;
	    public byte abTourMask;
	    public byte abTourEnable;
	    public byte abSnapshot;
	    public byte abSnapshotEn;
	    public byte abSnapshotPeriod;
	    public byte abSnapshotTimes;
	    public byte abTipEnable;
	    public byte abMailEnable;
	    public byte abMessageEnable;
	    public byte abBeepEnable;
	    public byte abVoiceEnable;
	    public byte abMatrixMask;
	    public byte abMatrixEnable;
	    public byte abEventLatch;
	    public byte abLogEnable;
	    public byte abDelay;
	    public byte abVideoMessageEn;
	    public byte abMMSEnable;
	    public byte abMessageToNetEn;
	    public byte abTourSplit;
	    public byte abSnapshotTitleEn;

	    //信息
        /// <summary>
        /// 设备的视频通道数
        /// </summary>
	    public Int32 	nChannelCount;
        /// <summary>
        /// 设备的报警输出个数
        /// </summary>
	    public Int32 	nAlarmOutCount;
        /// <summary>
        /// 录像通道掩码(按位)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public UInt32[] dwRecordMask; 
        /// <summary>
        /// 录像使能
        /// </summary>
	    public bool bRecordEnable;
        /// <summary>
        /// 录像延时时间(秒)
        /// </summary>
	    public Int32 	nRecordLatch;
        /// <summary>
        /// 报警输出通道掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public UInt32[] dwAlarmOutMask;
        /// <summary>
        /// 报警输出使能
        /// </summary>
	    public bool bAlarmOutEn;
        /// <summary>
        /// 报警输出延时时间(秒)
        /// </summary>
	    public Int32 	nAlarmOutLatch;
        /// <summary>
        /// 扩展报警输出通道掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public  UInt32[] dwExAlarmOutMask;
        /// <summary>
        /// 扩展报警输出使能
        /// </summary>
	    public bool bExAlarmOutEn;
        /// <summary>
        /// 云台联动项
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public CFG_PTZ_LINK[] stuPtzLink;
        /// <summary>
        /// 云台联动使能
        /// </summary>
	    public bool bPtzLinkEn;
        /// <summary>
        /// 轮询通道掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public UInt32[] dwTourMask;
        /// <summary>
        /// 轮询使能
        /// </summary>
	    public bool bTourEnable;
        /// <summary>
        /// 快照通道号掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public UInt32[] dwSnapshot;
        /// <summary>
        /// 快照使能
        /// </summary>
	    public bool bSnapshotEn;
        /// <summary>
        /// 连拍周期(秒)
        /// </summary>
	    public Int32 	nSnapshotPeriod;
        /// <summary>
        /// 连拍次数
        /// </summary>
	    public Int32 	nSnapshotTimes;
        /// <summary>
        /// 本地消息框提示
        /// </summary>
	    public bool bTipEnable;
        /// <summary>
        /// 发送邮件，如果有图片，作为附件
        /// </summary>
	    public bool bMailEnable;
        /// <summary>
        /// 上传到报警服务器
        /// </summary>
	    public bool bMessageEnable;
        /// <summary>
        /// 蜂鸣
        /// </summary>
	    public bool bBeepEnable;
        /// <summary>
        /// 语音提示
        /// </summary>
	    public bool bVoiceEnable;
        /// <summary>
        /// 联动视频矩阵通道掩码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public UInt32[] dwMatrixMask;
        /// <summary>
        /// 联动视频矩阵
        /// </summary>
	    public bool bMatrixEnable;
        /// <summary>
        /// 联动开始延时时间(秒)，0－15
        /// </summary>
	    public Int32 	nEventLatch;
        /// <summary>
        /// 是否记录日志
        /// </summary>
	    public bool bLogEnable;
        /// <summary>
        /// 设置时先延时再生效，单位为秒
        /// </summary>
	    public Int32 	nDelay;
        /// <summary>
        /// 叠加提示字幕到视频。叠加的字幕包括事件类型，通道号，秒计时。
        /// </summary>
	    public bool bVideoMessageEn;
        /// <summary>
        /// 发送彩信使能
        /// </summary>
	    public bool bMMSEnable;
        /// <summary>
        /// 消息上传给网络使能
        /// </summary>
	    public bool bMessageToNetEn;
        /// <summary>
        /// 轮巡时的分割模式 0: 1画面; 1: 8画面
        /// </summary>
	    public Int32 	nTourSplit;
        /// <summary>
        /// 是否叠加图片标题
        /// </summary>
	    public bool bSnapshotTitleEn;
    }

    // 外部报警配置
    public struct CFG_ALARMIN_INFO
    {
        /// <summary>
        /// 报警通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警通道名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szChnName;
        /// <summary>
        /// 报警器类型，0：常闭，1：常开
        /// </summary>
	    public Int32 nAlarmType;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
    }


    // 网络输入报警配置
    public struct CFG_NETALARMIN_INFO 
    {
        /// <summary>
        /// 报警通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警通道名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szChnName;
        /// <summary>
        /// 报警器类型，0：常闭，1：常开
        /// </summary>
	    public Int32 nAlarmType;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
    }

    // 动态检测报警配置
    public struct CFG_MOTION_INFO 
    {
        /// <summary>
        /// 报警通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 灵敏度1～6
        /// </summary>
	    public Int32 nSenseLevel;
        /// <summary>
        /// 动态检测区域的行数
        /// </summary>
	    public Int32 nMotionRow;
        /// <summary>
        /// 动态检测区域的列数
        /// </summary>
	    public Int32 nMotionCol;
        /// <summary>
        /// 检测区域，最多32*32块区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 32)]
	    public byte[] byRegion;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
    } 

    // 视频丢失报警配置
    public struct CFG_VIDEOLOST_INFO 
    {
        /// <summary>
        /// 报警通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
    } 

    // 视频遮挡报警配置
    public struct CFG_SHELTER_INFO 
    {
        /// <summary>
        /// 报警通道号(0开始)
        /// </summary>
        public Int32 nChannelID;
        /// <summary>
        /// 使能开关
        /// </summary>
        public bool bEnable;
        /// <summary>
        /// 灵敏度
        /// </summary>
        public Int32 nSenseLevel;
        /// <summary>
        /// 报警联动
        /// </summary>
        public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
        public CFG_TIME_SECTION[] stuTimeSection;
    }

    // 无存储设备报警配置
    public struct CFG_STORAGENOEXIST_INFO 
    {
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
    } ;

    // 存储设备访问出错报警配置
    public struct CFG_STORAGEFAILURE_INFO 
    {
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
    } 

    // 存储设备空间不足报警配置
    public struct CFG_STORAGELOWSAPCE_INFO 
    {
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 硬盘剩余容量下限，百分数(0~99)
        /// </summary>
	    public Int32 nLowerLimit;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
    } 

    // 网络断开报警配置
    public struct CFG_NETABORT_INFO 
    {
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警联动
        /// </summary>
        public CFG_ALARM_MSG_HANDLE stuEventHandler;
    } 

    // IP冲突报警配置
    public struct CFG_IPCONFLICT_INFO 
    {
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
    } 

    // 抓图配置能力
    public struct CFG_SNAPCAPINFO_INFO  
    {
        /// <summary>
        /// 抓图通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 支持的分别率信息
        /// </summary>
        public UInt32 dwIMageSizeNum;
        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public CFG_CAPTURE_SIZE[] emIMageSizeList;
        /// <summary>
        /// 支持的帧率信息
        /// </summary>
        public UInt32 dwFramesPerSecNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public Int32[] nFramesPerSecList;
        /// <summary>
        /// 支持的画质信息
        /// </summary>
        public UInt32 dwQualityMun;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public CFG_IMAGE_QUALITY[] emQualityList;
        /// <summary>
        /// 模式,按位：第一位：定时；第二位：手动。
        /// </summary>
        public UInt32 dwMode;
        /// <summary>
        /// 图片格式模式,按位：第一位：bmp；第二位：jpg。
        /// </summary>
        public UInt32 dwFormat;
    } 

    // 网络存储服务器配置
    public struct CFG_CHANNEL_TIME_SECTION 
    {
        /// <summary>
        /// 存储时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 2)]
	    public CFG_TIME_SECTION[] stuTimeSection;
    } 

    public struct CFG_NAS_INFO
    {
        /// <summary>
        /// 使能开关
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 网络存储服务器版本0=老的FTP，1=NAS存储
        /// </summary>
	    public Int32 nVersion;
        /// <summary>
        /// 协议类型0=FTP 1=SMB
        /// </summary>
	    public Int32 nProtocol;
        /// <summary>
        /// IP地址或网络名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szAddress;
        /// <summary>
        /// 端口号
        /// </summary>
	    public Int32 nPort;
        /// <summary>
        /// 帐户名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szUserName;
        /// <summary>
        /// 密码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szPassword;
        /// <summary>
        /// 共享的目录名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szDirectory;
        /// <summary>
        /// 文件长度
        /// </summary>
	    public Int32 nFileLen;
        /// <summary>
        /// 相邻文件时间间隔
        /// </summary>
	    public Int32 nInterval;
        /// <summary>
        /// 存储时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public CFG_CHANNEL_TIME_SECTION[] stuChnTime;
        /// <summary>
        /// 有效的存储时间段数
        /// </summary>
	    public Int32 nChnTimeCount;
    } 

    // 协议名
    public struct CFG_PRONAME
    {
        /// <summary>
        /// 协议名 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name;
    } 

    // 串口基本属性
    public struct CFG_COMM_PROP
    {
        /// <summary>
        /// 数据位；0：5，1：6，2：7，3：8
        /// </summary>
	    public byte byDataBit;
        /// <summary>
        /// 停止位；0：1位，1：1.5位，2：2位
        /// </summary>
        public byte byStopBit;
        /// <summary>
        /// 校验位；0：无校验，1：奇校验；2：偶校验
        /// </summary>
        public byte byParity;
        /// <summary>
        /// 波特率；0：300，1：600，2：1200，3：2400，4：4800，
        /// 5：9600，6：19200，7：38400，8：57600，9：115200
        /// </summary>
        public byte byBaudRate;
	        
    } 


    // 云台配置
    public struct CFG_PTZ_INFO
    {
	    // 能力
	    public byte abMartixID;
        public byte abCamID;
        public byte abPTZType;

	    // 信息
        /// <summary>
        /// 通道号(0开始)	
        /// </summary>
        public Int32 nChannelID;
        /// <summary>
        /// 协议名列表(只读)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public CFG_PRONAME[] stuDecProName;
        /// <summary>
        /// 协议类型，对应"协议名列表"下标
        /// </summary>
        public Int32 nProName;
        /// <summary>
        /// 解码器地址；0 - 255
        /// </summary>
        public Int32 nDecoderAddress;
        public CFG_COMM_PROP struComm;
        /// <summary>
        /// 矩阵号
        /// </summary>
        public Int32 nMartixID;
        /// <summary>
        /// 云台类型0-兼容，本地云台 1-远程网络云台
        /// </summary>
        public Int32 nPTZType;
        /// <summary>
        /// 摄像头ID
        /// </summary>
        public Int32 nCamID;
    }   

    // 水印配置
    public struct CFG_WATERMARK_INFO 
    {
        /// <summary>
        /// 通道号(0开始)
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 使能开关
        /// </summary>
        public bool bEnable;
        /// <summary>
        /// 码流类型(1～n)，0－所有码流
        /// </summary>
        public Int32 nStreamType;
        /// <summary>
        /// 数据类型，1－文字，2－图片
        /// </summary>
        public Int32 nDataType;
        /// <summary>
        /// 字符串水印数据
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
        public byte[] pData;
    } 

    // 每个视频输入通道对应的视频分析资源配置信息
    public struct CFG_ANALYSESOURCE_INFO
    {
        /// <summary>
        /// 视频分析使能
        /// </summary>
	    public byte bEnable;
        /// <summary>
        /// 保留对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 智能分析的前端视频通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 智能分析的前端视频码流类型，0:抓图码流; 1:主码流; 2:子码流1; 3:子码流2; 4:子码流3; 5:物体流
        /// </summary>
	    public Int32 nStreamType;
        /// <summary>
        /// 设备名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRemoteDevice;
    	
    } 

    public struct CFG_RAINBRUSH_INFO
    {
        /// <summary>
        /// 雨刷使能
        /// </summary>
	    public byte bEnable;
        /// <summary>
        /// 雨刷速度,1:快速;2:中速;3:慢速
        /// </summary>
	    public byte bSpeedRate;
        /// <summary>
        /// 保留对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public byte[] bReserved;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;

    }

    // BreakingSnapTimes
    public struct BREAKINGSNAPTIMES_INFO
    {
        /// <summary>
        /// 正常
        /// </summary>
	    Int32 nNormal;
        /// <summary>
        /// 闯红灯
        /// </summary>
	    Int32 nRunRedLight;
        /// <summary>
        /// 压线
        /// </summary>
	    Int32 nOverLine;
        /// <summary>
        /// 压黄线
        /// </summary>
	    Int32 nOverYellowLine;
        /// <summary>
        /// 逆向
        /// </summary>
	    Int32 nRetrograde;
        /// <summary>
        /// 欠速
        /// </summary>
	    Int32 nUnderSpeed;
        /// <summary>
        /// 超速
        /// </summary>
        Int32 nOverSpeed;
        /// <summary>
        /// 有车占道
        /// </summary>
	    Int32 nWrongRunningRoute;
        /// <summary>
        /// 黄牌占道
        /// </summary>
	    Int32 nYellowInRoute;
        /// <summary>
        /// 特殊逆行
        /// </summary>
	    Int32 nSpecialRetrograde;
        /// <summary>
        /// 违章左转
        /// </summary>
	    Int32 nTurnLeft;
        /// <summary>
        /// 违章右转
        /// </summary>
	    Int32 nTurnRight;
        /// <summary>
        /// 违章变道
        /// </summary>
	    Int32 nCrossLane;
        /// <summary>
        /// 违章调头
        /// </summary>
	    Int32 nU_Turn;
        /// <summary>
        /// 违章停车
        /// </summary>
	    Int32 nParking;
        /// <summary>
        /// 违章进入待行区
        /// </summary>
	    Int32 nWaitingArea;
        /// <summary>
        /// 不按车道行驶
        /// </summary>
	    Int32 nWrongRoute;
    }

    public struct COILCONFIG_INFO
    {
        /// <summary>
        /// 延时闪光灯序号	每个线圈对应的延时闪关灯序号，范围0~5，0表示不延时任何闪光灯
        /// </summary>
	    public Int32 nDelayFlashID;
        /// <summary>
        /// 闪光灯序号	范围0~5，0表示不打开闪光灯（鄞州项目用）
        /// </summary>
        public Int32 nFlashSerialNum;
        /// <summary>
        /// 红灯方向	每个线圈对应的红灯方向：0-不关联,1-左转红灯,2-直行红灯,3-右转红灯,4-待左,5-待直,6-待右, 只在电警中有效
        /// </summary>
        public Int32 nRedDirection;
        /// <summary>
        /// 线圈触发模式	触发模式：0-入线圈触发1-出线圈触发（鄞州项目用）
        /// </summary>
        public Int32 nTriggerMode;
    }

    public struct DETECTOR_INFO
    {
        /// <summary>
        /// 违章类型掩码	从低位到高位依次是：0-正常1-闯红灯2-压线3-逆行4-欠速5-超速6-有车占道7-黄牌占道
        /// </summary>
	    public Int32 nDetectBreaking;
        /// <summary>
        /// 线圈配置数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public COILCONFIG_INFO[]    arstCoilCfg;
        /// <summary>
        /// 车道号	0-16 
        /// </summary>
	    public Int32 nRoadwayNumber;
        /// <summary>
        /// 车道方向（车开往的方向）	0-南向北 1-西南向东北 2-东 3-西北向东南 
        /// 4-北向南 5-东北向西南 6-东向西 7-东南向西北 8-忽略
        /// </summary>
	    public Int32 nRoadwayDirection;
        /// <summary>
        /// 卡口图片序号	表示将电警中的某一张图片作为卡口图片（共三张），
        /// 0表示不采用，1~3,表示采用对应序号的图片
        /// </summary>
	    public Int32 nRedLightCardNum;
        /// <summary>
        /// 线圈个数	1-3
        /// </summary>
	    public Int32 nCoilsNumber;
        /// <summary>
        /// 业务模式	0-卡口电警1-电警2-卡口
        /// </summary>
	    public Int32 nOperationType;
        /// <summary>
        /// 两两线圈的间隔	范围0-1000，单位为厘米
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public Int32[] arnCoilsDistance;
        /// <summary>
        /// 每个线圈的宽度	0~200cm
        /// </summary>
	    public Int32 nCoilsWidth;
        /// <summary>
        /// 小型车辆速度下限和上限	0~255km/h，不启用大小车限速时作为普通车辆限速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public Int32[] arnSmallCarSpeedLimit;
        /// <summary>
        /// 大型车辆速度下限和上限	0~255km/h，启用大小车限速时有效
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public Int32[] arnBigCarSpeedLimit;
        /// <summary>
        /// 限高速宽限值	单位：km/h
        /// </summary>
	    public Int32 nOverSpeedMargin;
        /// <summary>
        /// 大车限高速宽限值	单位：km/h，启用大小车限速时有效
        /// </summary>
	    public Int32 nBigCarOverSpeedMargin;
        /// <summary>
        /// 限低速宽限值	单位：km/h
        /// </summary>
	    public Int32 nUnderSpeedMargin;
        /// <summary>
        /// 大车限低速宽限值	单位：km/h，启用大小车限速时有效
        /// </summary>
	    public Int32 nBigCarUnderSpeedMargin;
        /// <summary>
        /// 是否启用大小车限速
        /// </summary>
	    public byte bSpeedLimitForSize;
        /// <summary>
        /// 逆行是否视为违章行为
        /// </summary>
	    public byte bMaskRetrograde;
        /// <summary>
        /// 保留对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public byte[] byReserved;
        /// <summary>
        /// "DrivingDirection" : ["Approach", "上海", "杭州"],行驶方向
        /// "Approach"-上行，即车辆离设备部署点越来越近；"Leave"-下行，
        /// 即车辆离设备部署点越来越远，第二和第三个参数分别代表上行和
        /// 下行的两个地点，UTF-8编码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3 * 256)]
        public byte[] szDrivingDirection;
        /// <summary>
        /// 超速百分比，超过限速百分比后抓拍
        /// </summary>
	    public Int32 nOverPercentage; 
     
    }

    public struct CFG_TRAFFICSNAPSHOT_INFO
    {
        /// <summary>
        /// 设备地址	UTF-8编码，256字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szDeviceAddress;
        /// <summary>
        /// OSD叠加类型掩码	从低位到高位分别表示：0-时间 1-地点 2-车牌3-车长 4-车速 5-限速
        /// 6-大车限速 7-小车限速8-超速 9-违法代码10-车道号 
        /// 11-车身颜色 12-车牌类型 13-车牌颜色14-红灯点亮时间 15-违章类型 
        /// 16-雷达方向 17-设备编号 18-标定到期时间 19-车型 20-行驶方向
        /// </summary>
	    public Int32 nVideoTitleMask;
        /// <summary>
        /// 红灯冗余间隔时间	红灯开始的一段时间内，车辆通行不算闯红灯，单位：秒
        /// </summary>
	    public Int32 nRedLightMargin;
        /// <summary>
        /// 超长车长度最小阈值	单位：米，包含
        /// </summary>
	    public float fLongVehicleLengthLevel;
        /// <summary>
        /// 大车长度阈值	单位：米，包含小值
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public float[] arfLargeVehicleLengthLevel;
        /// <summary>
        /// 中型车长度阈值	单位：米，包含小值
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public float[] arfMediumVehicleLengthLevel;
        /// <summary>
        /// 小车长度阈值	单位：米，包含小值
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public float[] arfSmallVehicleLengthLevel;
        /// <summary>
        /// 摩托车长度最大阈值	单位：米，不包含
        /// </summary>
	    public float fMotoVehicleLengthLevel;
        /// <summary>
        /// 违章抓拍张数
        /// </summary>
	    public BREAKINGSNAPTIMES_INFO   stBreakingSnapTimes;
        /// <summary>
        /// 车检器配置，前三个用于放卡口的3个线圈，后三个放电警的3个线圈
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
	    public DETECTOR_INFO[] arstDetector;
        /// <summary>
        /// 抓拍车辆类型	0-大小车都抓拍1-抓拍小车2-抓拍大车3-大小车都不抓拍
        /// </summary>
	    public Int32 nCarType;
        /// <summary>
        /// 当测得的速度超过最大速度时，则以最大速度计	0~255km/h
        /// </summary>
	    public Int32 nMaxSpeed;
        /// <summary>
        /// 帧间隔模式	1-速度自适应（超过速度上限取0间隔，低于速度下限取2间隔，中间取1间隔）2-由联动参数决定
        /// </summary>
	    public Int32 nFrameMode;
        /// <summary>
        /// 速度自适应下限和上限 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public Int32[] arnAdaptiveSpeed;
        /// <summary>
        /// 交通抓拍联动参数
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE     stuEventHandler;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 768)]
	    public byte[] bReserved;
    }


    public struct CFG_OVERSPEED_INFO
    {
        /// <summary>
        /// 超速百分比区间要求区间不能重叠。有效值为0,正数,-1，-1表示无穷大值
        /// 如果是欠速：要求区间不能重叠。有效值为0,正数,-1，
        /// -1表示无穷大值，欠速百分比的计算方式：限低速-实际车速/限低速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public Int32[] nSpeedingPercentage;
        /// <summary>
        /// 违章代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szCode;
        /// <summary>
        /// 违章描述
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDescription; 

    }

    public struct CFG_UNDERSPEED_INFO
    {
        /// <summary>
        /// 超速百分比区间要求区间不能重叠。有效值为0,正数,-1，-1表示无穷大值
        /// 如果是欠速：要求区间不能重叠。有效值为0,正数,-1，
        /// -1表示无穷大值，欠速百分比的计算方式：限低速-实际车速/限低速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public Int32[] nSpeedingPercentage;
        /// <summary>
        /// 违章代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szCode;
        /// <summary>
        /// 违章描述
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDescription; 

    }

    public struct CFG_OVERSPEED_HIGHWAY_INFO
    {
        /// <summary>
        /// 超速百分比区间要求区间不能重叠。有效值为0,正数,-1，-1表示无穷大值
        /// 如果是欠速：要求区间不能重叠。有效值为0,正数,-1，
        /// -1表示无穷大值，欠速百分比的计算方式：限低速-实际车速/限低速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Int32[] nSpeedingPercentage;
        /// <summary>
        /// 违章代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szCode;
        /// <summary>
        /// 违章描述
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szDescription;

    }

    /// <summary>
    /// ViolationCode 违章代码配置表
    /// </summary>
    public struct VIOLATIONCODE_INFO
    {
        /// <summary>
        /// 逆行
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szRetrograde;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szRetrogradeDesc; 
        /// <summary>
        /// 逆行-高速公路
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szRetrogradeHighway;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szRetrogradeHighwayDesc;
        /// <summary>
        /// 闯红灯
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szRunRedLight;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szRunRedLightDesc;
        /// <summary>
        /// 违章变道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szCrossLane;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szCrossLaneDesc;
        /// <summary>
        /// 违章左转
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szTurnLeft;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szTurnLeftDesc;
        /// <summary>
        /// 违章右转
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szTurnRight;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szTurnRightDesc;
        /// <summary>
        /// 违章掉头
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szU_Turn;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szU_TurnDesc;
        /// <summary>
        /// 交通拥堵
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szJam;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szJamDesc;
        /// <summary>
        /// 违章停车
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szParking;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szParkingDesc;

	    // 超速 和 超速比例 只需且必须有一个配置
        /// <summary>
        /// 超速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szOverSpeed;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szOverSpeedDesc;
        /// <summary>
        /// 超速比例代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
	    public CFG_OVERSPEED_INFO[]  stOverSpeedConfig;

	    // 超速(高速公路) 和 超速比例(高速公路) 只需且必须有一个配置
        /// <summary>
        /// 超速-高速公路
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szOverSpeedHighway;
        /// <summary>
        /// 超速-违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szOverSpeedHighwayDesc;
        /// <summary>
        /// 超速比例代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
	    public CFG_OVERSPEED_HIGHWAY_INFO[] stOverSpeedHighwayConfig;

	    // 欠速 和 欠速比例 只需且必须有一个配置
        /// <summary>
        /// 欠速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szUnderSpeed;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szUnderSpeedDesc;
        /// <summary>
        /// 欠速配置信息	是一个数组，不同的欠速比违章代码不同，为空表示违章代码不区分超速比
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
	    public CFG_UNDERSPEED_INFO[] stUnderSpeedConfig;
        /// <summary>
        /// 压线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szOverLine;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szOverLineDesc;
        /// <summary>
        /// 压黄线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szOverYellowLine;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szOverYellowLineDesc;
        /// <summary>
        /// 黄牌占道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szYellowInRoute;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szYellowInRouteDesc;
        /// <summary>
        /// 不按车道行驶
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szWrongRoute;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szWrongRouteDesc;
        /// <summary>
        /// 路肩行驶
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szDrivingOnShoulder;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDrivingOnShoulderDesc;
        /// <summary>
        /// 正常行驶
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szPassing;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szPassingDesc;
        /// <summary>
        /// 禁止行驶
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szNoPassing;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szNoPassingDesc;
        /// <summary>
        /// 套牌
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szFakePlate;
        /// <summary>
        /// 违章描述信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szFakePlateDesc;
    	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[] bReserved;
    }

    /// <summary>
    /// 交通全局配置配置表
    /// </summary>
    public struct CFG_TRAFFICGLOBAL_INFO
    {
        /// <summary>
        /// 违章代码配置表
        /// </summary>
	    public VIOLATIONCODE_INFO   stViolationCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[] bReserved;
    }


    // 普通配置 (CFG_CMD_DEV_GENERRAL) General 
    public struct CFG_DEV_DISPOSITION_INFO
    {
        /// <summary>
        /// 本机编号，主要用于遥控器区分不同设备	0~998
        /// </summary>
	    public Int32 nLocalNo;
        /// <summary>
        /// 机器名称或编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szMachineName;
        /// <summary>
        /// 机器部署地点即道路编码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szMachineAddress; 
        /// <summary>
        /// 机器分组或叫设备所属单位	默认为空，用户可以将不同的设备编为一组，便于管理，可重复。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szMachineGroup;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[] bReserved; 
    }

    public struct CFG_ATMMOTION_INFO
    {
	    public Int32 nTimeLimit;
    }

    // 设备状态信息
    public struct CFG_DEVICESTATUS_INFO
    {
        /// <summary>
        /// 电源个数
        /// </summary>
	    public Int32 nPowerNum;
        /// <summary>
        /// 电源状态，1:正常 2:异常 3:未知
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] byPowerStatus;
        /// <summary>
        /// CPU个数
        /// </summary>
        public Int32 nCPUNum;
        /// <summary>
        /// CPU温度
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Int32[] nCPUTemperature;
        /// <summary>
        /// 风扇个数
        /// </summary>
        public Int32 nFanNum;
        /// <summary>
        /// 风扇转速
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Int32[] nRotatoSpeed;
    }

    // 扩展柜信息
    public struct CFG_HARDDISK_INFO 
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannel;
        /// <summary>
        /// 硬盘容量
        /// </summary>
	    public Int32 nCapacity;
        /// <summary>
        /// 硬盘状态，0:unknown 1:running 2:fail
        /// </summary>
	    public byte byStatus;
        /// <summary>
        /// 使用状态，1.空闲 2.在用 3.未知
        /// </summary>
	    public byte byUsedStatus;
        /// <summary>
        /// 是否是热备盘，0:热备盘 1:非热备盘
        /// </summary>
        public byte byHotBack;
        /// <summary>
        /// 字节对齐
        /// </summary>
	    public byte byReserved;
        /// <summary>
        /// 所在Raid(磁盘组)的名称	"RaidName" : "Raid0",
        /// 所在Raid(磁盘组)的名称。如不属于任何Raid，则字段为null。
        /// 比如热备盘，属于全局热备盘的，则传null。 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRaidName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szType;                      // 硬盘型号
	    public Int32 nTank;                           // 扩展柜, 0:主机;1:扩展柜1; 2:扩展柜2	……
	    public Int32 nRemainSpace;					 // 剩余容量，单位M
    }

    public struct CFG_HARDDISKTANK_INFO
    {
        /// <summary>
        /// 存储柜名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szTankName;
        /// <summary>
        /// 硬盘个数
        /// </summary>
        public Int32 nHardDiskNum;
        /// <summary>
        /// 硬盘信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public CFG_HARDDISK_INFO[] stuHarddiskInfo;
    }

    public struct CFG_HARDISKTANKGROUP_INFO
    {
        /// <summary>
        /// 硬盘存储柜个数
        /// </summary>
	    public Int32  nTankNum;
        /// <summary>
        /// 硬盘存储柜数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public CFG_HARDDISKTANK_INFO[]    stuHarddisktank;
    }

    // Raid组信息
    public struct CFG_RAID_INFO
    {
        /// <summary>
        /// Raid名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRaidName;
        /// <summary>
        /// 类型 1:Jbob, 2:Raid0, 3:Raid1, 4:Raid5
        /// </summary>
	    public byte byType;
        /// <summary>
        /// 状态  0:unknown, 1:active, 2:degraded, 3:inactive, 4:recovering 同步中
        /// </summary>
	    public byte byStatus;
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public byte[] byReserved;
        /// <summary>
        /// 组成磁盘通道
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public Int32[] nMember;
        /// <summary>
        /// 磁盘个数
        /// </summary>
        public Int32 nDiskNUM;
        /// <summary>
        /// 容量
        /// </summary>
        public Int32 nCapacity;
        /// <summary>
        /// 扩展柜
        /// </summary>
        public Int32 nTank;
        /// <summary>
        /// 剩余容量，单位M
        /// </summary>
        public Int32 nRemainSpace;
    }

    public struct CFG_RAIDGROUP_INFO
    {
        /// <summary>
        /// Raid个数
        /// </summary>
	    public Int32 nRaidNum;
        /// <summary>
        /// Raid组信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public CFG_RAID_INFO[] stuRaidInfo;
    }

    // 存储池组信息
    public struct CFG_STORAGEPOOL_INFO
    {
        /// <summary>
        /// 存储池名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szName;
        /// <summary>
        /// 设备数量
        /// </summary>
        public Int32 nMemberNum;
        /// <summary>
        /// 组成设备
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
        public byte[] szMember;
        /// <summary>
        /// 已用容量
        /// </summary>
        public Int32 nUsed;
        /// <summary>
        /// 总容量
        /// </summary>
        public Int32 nCapacity;
        /// <summary>
        /// 状态	0:unknown 1:active 2:degraded 3:inactive
        /// </summary>
        public Int32 nStatus;
        /// <summary>
        /// 扩展柜	0:主机, 1:扩展柜1, 2:扩展柜2 ……
        /// </summary>
        public Int32 nTank;
    }

    public struct CFG_STORAGEPOOLGROUP_INFO
    {
        /// <summary>
        /// 存储池个数
        /// </summary>
	    public Int32 nStroagePoolNum; 
        /// <summary>
        /// 存储池信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public CFG_STORAGEPOOL_INFO[] stuStoragePoolInfo;
    }


    // 文件系统组信息
    public struct CFG_STORAGEPOSITION_INFO
    {
        /// <summary>
        /// 存储位置名称	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szName;
        /// <summary>
        /// 存储池名称	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szStoragePoolName;
        /// <summary>
        /// 已用容量,单位G	
        /// </summary>
        public Int32 nUsedCapacity;
        /// <summary>
        /// 容量,单位G	
        /// </summary>
        public Int32 nTotalCapacity;
        /// <summary>
        /// 状态 0.未知 1.正常 2.配置异常 3.挂载异常
        /// </summary>
	    public byte byStatus;
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
    }

    public struct CFG_STORAGEPOSITIONGROUP_INFO
    {
        /// <summary>
        /// 存储信息个数
        /// </summary>
	    public Int32 nStoragePositionNum;
        /// <summary>
        /// 文件系统组信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public CFG_STORAGEPOSITION_INFO[] stuStoragePositionInfo;      
    }

    // 前端设备组信息
    public struct CFG_VIDEOINDEV_INFO
    {
        /// <summary>
        /// 前端设备名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szDevName; 
        /// <summary>
        /// 设备ID	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szDevID;
        /// <summary>
        /// 设备类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szDevType;
        /// <summary>
        /// 总通道数
        /// </summary>
        public Int32 nTotalChan;
        /// <summary>
        /// 报警通道总数
        /// </summary>
	    public Int32 nTotalAlarmChan;
        /// <summary>
        /// 设备IP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] szIP;
        /// <summary>
        /// 状态 0:未知 1:在线 2:离线
        /// </summary>
	    public byte byStatus;
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] byReserved;
    }

    public  struct CFG_VIDEOINDEVGROUP_INFO
    {
        /// <summary>
        /// 前端设备个数
        /// </summary>
	    public Int32 nVideoDevNum; 
        /// <summary>
        /// 前端设备组信息      
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public CFG_VIDEOINDEV_INFO[] stuVideoInDevInfo;
    }

    // 通道录像组状态
    public struct CFG_DEVRECORD_INFO
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szDevName;
        /// <summary>
        /// 设备IP	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szIP;
        /// <summary>
        /// 通道号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szChannel;
        /// <summary>
        /// 通道名称	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szChannelName;
        /// <summary>
        /// 存储位置信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szStoragePosition;
        /// <summary>
        /// 状态 0:未知 1:录像 2:停止
        /// </summary>
	    public byte byStatus;
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
    }

    public  struct CFG_DEVRECORDGROUP_INFO
    {
        /// <summary>
        /// 通道个数
        /// </summary>
	    public Int32 nChannelNum;
        /// <summary>
        /// 通道录像状态信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public CFG_DEVRECORD_INFO[] stuDevRecordInfo;
    }

    // 服务状态
    public struct CFG_IPSERVER_STATUS
    {
        /// <summary>
        /// 提供的服务个数
        /// </summary>
	    public Int32  nSupportedServerNum;
        /// <summary>
        /// 提供的服务名称 Svr Svrd(SVR守护服务) DataBase DataBased(DataBase守护服务) 
        /// NtpServer NtpServerd(NtpServer守护服务) DahuaII DahuaIId(DahuaII守护服务) Samba Nfs Ftp iScsi 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)] 
	    public byte[] szSupportServer; 
        /// <summary>
        /// Svr提供的子服务信息个数
        /// </summary>
	    public Int32  nSvrSuppSubServerNum;
        /// <summary>
        /// Svr提供的子服务信息 CMS DMS	MTS	SS RMS DBR
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szSvrSuppSubServer;
        /// <summary>
        /// 0:未知 1:运行 2:未运行
        /// 下面字段都是这个意思
        /// </summary>
        public byte byCMS;
	    public byte byDMS;
	    public byte byMTS;
	    public byte bySS;
	    public byte byRMS;
	    public byte byDBR;
	    public byte bySvrd;
	    public byte byDataBase;
	    public byte byDataBased;
	    public byte byNtpServer;
	    public byte byNtpServerd;
	    public byte byDahuaII;
	    public byte byDahuaIId;
	    public byte bySAMBA;  
	    public byte byNFS;    
	    public byte byFTP;    
	    public byte byISCSI;  
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved; 
    }


    public struct SNAPSOURCE_INFO_SINGLE_CHANNEL
    {
        /// <summary>
        /// 使能
        /// </summary>
	    public byte		bEnable;
        /// <summary>
        /// 保留字段，对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved1;
        /// <summary>
        /// 设备名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] bDevice;
        /// <summary>
        /// 视频通道号 
        /// </summary>
	    public UInt32		dwChannel;
        /// <summary>
        /// 抓图通道对应的视频通道号
        /// </summary>
	    public UInt32		dwLinkVideoChannel;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
	    public byte[]	bReserved;
    }

    public struct CFG_SNAPSOURCE_INFO
    {
        /// <summary>
        /// 要配置的通道的个数
        /// </summary>
	    public UInt32 dwCount;
        /// <summary>
        /// SNAPSOURCE_INFO_SINGLE_CHANNEL数组的地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public SNAPSOURCE_INFO_SINGLE_CHANNEL[] singleChnSanpInfo;
    }


    // 轮巡模式
    public struct CFG_TOUR_MODE
    {
        /// <summary>
        /// 画面分割模式,参考枚举类型MATRIX_VIEW_SPLITMODE
        /// </summary>
	    public Int32 nViewMode;
        /// <summary>
        /// 表示ViewMod例如:0x00000007:表示模式3(SPLIT8)的分隔1,2,3使能开启,其他未使能,
        /// 0x0000000F表示分隔1,2,3,4使能e指定模式下,使能的几个分隔方法,使用掩码表达方式
        /// </summary>
	    public UInt32 dwViewSplitMask;
    }

    // SPOT视频矩阵方案
    public  struct CFG_VIDEO_MATRIX_PLAN
    {
        /// <summary>
        /// 矩阵配置方案使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 矩阵轮巡间隔,单位秒,>=1
        /// </summary>
	    public Int32 nTourPeriod;
        /// <summary>
        /// 轮巡队列个数
        /// </summary>
	    public Int32 nTourModeNum;
        /// <summary>
        /// 轮巡队列
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public CFG_TOUR_MODE[]   stuTourMode;
    }

    // SPOT矩阵配置
    public  struct CFG_VIDEO_MATRIX
    {
        /// <summary>
        /// 支持的画面分割的能力数
        /// </summary>
	    public Int32 nSupportSplitModeNumber;
        /// <summary>
        /// 支持的画面分割的能力
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] bySupportSplitMode;
        /// <summary>
        /// 矩阵方案数
        /// </summary>
	    public Int32 nMatrixPlanNumber;
        /// <summary>
        /// 矩阵方案
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	    public CFG_VIDEO_MATRIX_PLAN[] stuMatrixPlan;
    }


    // dsp配置
    public struct CFG_DSPENCODECAP_INFO
    {
        /// <summary>
        /// 视频制式掩码，按位表示设备能够支持的视频制式
        /// </summary>
	    public UInt32 dwVideoStandardMask;
        /// <summary>
        /// 分辨率掩码，按位表示设备能够支持的分辨率
        /// </summary>
	    public UInt32 dwImageSizeMask;
        /// <summary>
        /// 编码模式掩码，按位表示设备能够支持的编码模式
        /// </summary>
	    public UInt32 dwEncodeModeMask;
        /// <summary>
        /// 按位表示设备支持的多媒体功能，
        /// 第一位表示支持主码流
        /// 第二位表示支持辅码流1
        /// 第三位表示支持辅码流2
        /// 第五位表示支持jpg抓图
        /// </summary>
	    public UInt32 dwStreamCap;
        /// <summary>
        /// 表示主码流为各分辨率时，支持的辅码流分辨率掩码。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public UInt32[] dwImageSizeMask_Assi;
        /// <summary>
        /// DSP支持的最高编码能力
        /// </summary>
	    public UInt32 dwMaxEncodePower;
        /// <summary>
        /// 每块DSP支持最多输入视频通道数 
        /// </summary>
	    public UInt16 wMaxSupportChannel;
        /// <summary>
        /// DSP每通道的最大编码设置是否同步；0：不同步，1：同步
        /// </summary>
	    public UInt16 wChannelMaxSetSync;
        /// <summary>
        /// 不同分辨率下的最大采集帧率，与dwVideoStandardMask按位对应
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] bMaxFrameOfImageSize;
        /// <summary>
        /// 标志，配置时要求符合下面条件，否则配置不能生效；
        /// 0：主码流的编码能力+辅码流的编码能力 <= 设备的编码能力，
        /// 1：主码流的编码能力+辅码流的编码能力 <= 设备的编码能力，
        /// 辅码流的编码能力 <= 主码流的编码能力，
        /// 辅码流的分辨率 <= 主码流的分辨率，
        /// 主码流和辅码流的帧率 <= 前端视频采集帧率
        /// 2：N5的计算方法
        /// 辅码流的分辨率 <= 主码流的分辨率
        /// 查询支持的分辨率和相应最大帧率
        /// </summary>
	    public byte bEncodeCap;
        /// <summary>
        /// bResolution的长度
        /// </summary>
	    public byte byResolutionNum;
        /// <summary>
        /// bResolution_1的长度
        /// </summary>
	    public byte byResolutionNum_1;
	    public byte byReserved;
        /// <summary>
        /// 主码流,按照分辨率进行索引，如果支持该分辨率，则bResolution[n]等于支持的最大帧率.否则为0.	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] byResolution;
        /// <summary>
        /// 辅助码流1,同主码流说明.
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]			
	    public byte[] byResolution_1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]			
	    public byte[] reserved;
    }

    public  struct VIDEO_INMETERING_INFO_CHANNEL
    {
	    // 能力
	    public  byte bRegion;
	    public  byte bMode;
        /// <summary>
        /// 保留字段1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	    public  byte[] bReserved1;
    	/// <summary>
    	/// 测光区域个数
    	/// </summary>
	    public  Int32 nRegionNum; 
        /// <summary>
        /// 测光区域, 局部测光使用，支持多个测光区域，使用相对坐标体系，取值均为0~8191
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public  CFG_RECT[] stuRegions;
        /// <summary>
        /// 测光模式,0:平均测光,1:局部测光
        /// </summary>
	    public  byte byMode;
        /// <summary>
        /// 保留字段2
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public  byte[] bReserved2;
        /// <summary>
        /// 保留字段3
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public  byte[] bReserved3;
    }

    // 测光配置(CFG_CMD_VIDEO_INMETERING)是一个数组，每个视频输入通道一个配置
    public struct CFG_VIDEO_INMETERING_INFO
    {
        /// <summary>
        /// 通道数
        /// </summary>
	    public  Int32 nChannelNum;
        /// <summary>
        /// 每个通道的测光信息，下标对应通道号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public  VIDEO_INMETERING_INFO_CHANNEL[]	stuMeteringMode;
    }


    // 流量统计报警信息配置
    public struct CFG_TRAFFIC_FLOWSTAT_ALARM_INFO
    {
        /// <summary>
        /// 是否使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 统计周期，单位：分钟
        /// </summary>
	    public int nPeriod;
        /// <summary>
        /// 统计周期内车辆数上下限，单位：辆
        /// </summary>
	    public int nLimit;
        /// <summary>
        /// 统计周期内报警恢复车辆数，单位：辆 
        /// </summary>
	    public int nRestore;
        /// <summary>
        /// 检测到报警发生到开始上报的时间, 单位:秒,范围1~65535
        /// </summary>
        public int nDelay;
        /// <summary>
        /// 报警间隔时间, 单位:秒, 范围1~65535
        /// </summary>
	    public int nInterval;
        /// <summary>
        /// 上报次数,1~255
        /// </summary>
	    public int nReportTimes;
        /// <summary>
        /// 当前计划时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[] stCurrentTimeSection;
    }

    public struct CFG_TRAFFIC_FLOWSTAT_INFO_LANE
    {
	    // 能力
	    public byte abEnable;
        /// <summary>
        /// 是否使能
        /// </summary>
	    public byte bEnable;
        /// <summary>
        /// 报警上线参数
        /// </summary>
	    public CFG_TRAFFIC_FLOWSTAT_ALARM_INFO  stuAlarmUpperInfo;
        /// <summary>
        /// 报警下线参数
        /// </summary>
	    public CFG_TRAFFIC_FLOWSTAT_ALARM_INFO  stuAlarmLowInfo; 
    }


    // 视频输入夜晚特殊配置选项，在晚上光线较暗时自动切换到夜晚的配置参数
    public struct CFG_VIDEO_IN_NIGHT_OPTIONS
    {
        /// <summary>
        /// 0-不切换；1-根据亮度切换；2-根据时间切换
        /// </summary>
	    public byte bySwitchMode;
        /// <summary>
        /// 亮度阈值 0~100	
        /// </summary>
	    public byte byBrightnessThreshold ;
        /// <summary>
        /// 大致日出和日落时间，日落之后日出之前，将采用夜晚特殊的配置。
        /// </summary>
	    public byte bySunriseHour;
        /// <summary>
        /// 00:00:00 ~ 23:59:59
        /// </summary>
	    public byte bySunriseMinute;
	    public byte bySunriseSecond;
	    public byte bySunsetHour;
	    public byte bySunsetMinute;
	    public byte bySunsetSecond;  
        /// <summary>
        /// 红色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainRed;
        /// <summary>
        /// 绿色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainBlue;
        /// <summary>
        /// 蓝色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainGreen;
        /// <summary>
        /// 曝光模式；取值范围取决于设备能力集：0-自动曝光，1-曝光等级1，
        /// 2-曝光等级2…n-1最大曝光等级数 n带时间上下限的自动曝光 n+1自定义时间手动曝光 (n==byExposureEn）
        /// </summary>
	    public byte byExposure;
        /// <summary>
        /// 自动曝光时间下限或者手动曝光自定义时间,毫秒为单位，取值0.1ms~80ms
        /// </summary>
	    public float fExposureValue1;
        /// <summary>
        /// 自动曝光时间上限,毫秒为单位，取值0.1ms~80ms	
        /// </summary>
	    public float fExposureValue2;
        /// <summary>
        /// 白平衡, 0-"Disable", 1-"Auto", 2-"Custom", 3-"Sunny", 4-"Cloudy", 5-"Home",
        /// 6-"Office", 7-"Night", 8-"HighColorTemperature", 9-"LowColorTemperature", 10-"AutoColorTemperature", 
        /// 11-"CustomColorTemperature"
        /// </summary>
	    public byte byWhiteBalance ;
        /// <summary>
        /// 0~100, GainAuto为true时表示自动增益的上限，否则表示固定的增益值
        /// </summary>
	    public byte byGain;
        /// <summary>
        /// 自动增益
        /// </summary>
	    public byte bGainAuto;
        /// <summary>
        /// 自动光圈
        /// </summary>
	    public byte bIrisAuto;
        /// <summary>
        /// 外同步的相位设置 0~360
        /// </summary>
	    public float fExternalSyncPhase;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
	    public byte[] reserved; 
    } 


    // 闪光灯配置
    public struct CFG_FLASH_CONTROL
    {
        /// <summary>
        /// 工作模式，0-禁止闪光，1-始终闪光，2-自动闪光
        /// </summary>
	    public byte byMode;
        /// <summary>
        /// 工作值, 0-0us, 1-64us, 2-128us, 3-192...15-960us
        /// </summary>
	    public byte byValue;
        /// <summary>
        /// 触发模式, 0-低电平 1-高电平 2-上升沿 3-下降沿
        /// </summary>
	    public byte byPole;
        /// <summary>
        /// 亮度预设值  区间0~100
        /// </summary>
	    public byte byPreValue;
        /// <summary>
        /// 占空比, 0~100
        /// </summary>
	    public byte byDutyCycle;
        /// <summary>
        /// 倍频, 0~10
        /// </summary>
	    public byte byFreqMultiple;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 122)]
	    public byte[] reserved;
    }

        // 抓拍参数特殊配置
    public struct CFG_VIDEO_IN_SNAPSHOT_OPTIONS
    {
        /// <summary>
        /// 红色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainRed;
        /// <summary>
        /// 绿色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainBlue;
        /// <summary>
        /// 蓝色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainGreen;
        /// <summary>
        /// 曝光模式；取值范围取决于设备能力集：0-自动曝光，1-曝光等级1，
        /// 2-曝光等级2…n-1最大曝光等级数 n带时间上下限的自动曝光 n+1自定义时间手动曝光 (n==byExposureEn）
        /// </summary>
	    public byte byExposure;
        /// <summary>
        /// 自动曝光时间下限或者手动曝光自定义时间,毫秒为单位，取值0.1ms~80ms
        /// </summary>
	    public float fExposureValue1;
        /// <summary>
        /// 自动曝光时间上限,毫秒为单位，取值0.1ms~80ms	
        /// </summary>
	    public float fExposureValue2;
        /// <summary>
        /// 白平衡, 0-"Disable", 1-"Auto", 2-"Custom", 3-"Sunny", 4-"Cloudy",
        /// 5-"Home", 6-"Office", 7-"Night", 8-"HighColorTemperature", 9-"LowColorTemperature", 
        /// 10-"AutoColorTemperature", 11-"CustomColorTemperature"
        /// </summary>
	    public byte byWhiteBalance;
        /// <summary>
        /// 色温等级, 白平衡为"CustomColorTemperature"模式下有效
        /// </summary>
	    public byte byColorTemperature;
        /// <summary>
        /// 自动增益
        /// </summary>
	    public byte bGainAuto;
        /// <summary>
        /// 增益调节, GainAuto为true时表示自动增益的上限，否则表示固定的增益值
        /// </summary>
	    public byte byGain;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)]
	    public byte[] reversed;
    } 

    // 视频输入前端选项
    public struct CFG_VIDEO_IN_OPTIONS
    {
        /// <summary>
        /// 背光补偿：背光补偿等级取值范围取决于设备能力集，0-关闭，1-背光补偿强度1，
        /// 2-背光补偿强度2…n-最大背光补偿等级数
        /// </summary>
	    public byte byBacklight;
        /// <summary>
        /// 日/夜模式；0-总是彩色，1-根据亮度自动切换，2-总是黑白
        /// </summary>
	    public byte byDayNightColor;
        /// <summary>
        /// 白平衡, 0-"Disable", 1-"Auto", 2-"Custom", 3-"Sunny", 4-"Cloudy", 5-"Home",
        /// 6-"Office", 7-"Night", 8-"HighColorTemperature", 9-"LowColorTemperature",
        /// 10-"AutoColorTemperature", 11-"CustomColorTemperature"
        /// </summary>
	    public byte byWhiteBalance;
        /// <summary>
        /// 色温等级, 白平衡为"CustomColorTemperature"模式下有效
        /// </summary>
	    public byte byColorTemperature;
        /// <summary>
        /// 镜像
        /// </summary>
	    public byte bMirror;
        /// <summary>
        /// 翻转
        /// </summary>
	    public byte bFlip;
        /// <summary>
        /// 自动光圈
        /// </summary>
	    public byte bIrisAuto;
        /// <summary>
        /// 根据环境光自动开启红外补偿灯	
        /// </summary>
	    public byte bInfraRed;
        /// <summary>
        /// 红色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainRed;
        /// <summary>
        /// 绿色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainBlue;
        /// <summary>
        /// 蓝色增益调节，白平衡为"Custom"模式下有效 0~100
        /// </summary>
	    public byte byGainGreen;
        /// <summary>
        /// 曝光模式；取值范围取决于设备能力集：0-自动曝光，1-曝光等级1，
        /// 2-曝光等级2…n-1最大曝光等级数 n带时间上下限的自动曝光 n+1自定义时间手动曝光 (n==byExposureEn）
        /// </summary>
	    public byte byExposure;
        /// <summary>
        /// 自动曝光时间下限或者手动曝光自定义时间,毫秒为单位，取值0.1ms~80ms
        /// </summary>
	    public float fExposureValue1;
        /// <summary>
        /// 自动曝光时间上限,毫秒为单位，取值0.1ms~80ms	
        /// </summary>
	    public float fExposureValue2;
        /// <summary>
        /// 自动增益
        /// </summary>
	    public byte bGainAuto;
        /// <summary>
        /// 增益调节, GainAuto为true时表示自动增益的上限，否则表示固定的增益值
        /// </summary>
	    public byte byGain;
        /// <summary>
        /// 信号格式, 0-Inside(内部输入) 1-BT656 2-720p 3-1080p  4-1080i  5-1080sF
        /// </summary>
	    public byte bySignalFormat;
        /// <summary>
        /// 0-不旋转，1-顺时针90°，2-逆时针90°	
        /// </summary>
	    public byte byRotate90;
        /// <summary>
        /// 外同步的相位设置 0~360	
        /// </summary>
	    public float fExternalSyncPhase;
        /// <summary>
        /// 外部同步信号输入,0-内部同步 1-外部同步
        /// </summary>
	    public byte byExternalSync;
        /// <summary>
        /// 保留
        /// </summary>
	    public byte reserved0;
        /// <summary>
        /// 双快门, 0-不启用，1-双快门全帧率，即图像和视频只有快门参数不同，2-双快门半帧率，即图像和视频快门及白平衡参数均不同
        /// </summary>
	    public byte byDoubleExposure;
        /// <summary>
        /// 宽动态值
        /// </summary>
	    public byte byWideDynamicRange;
        /// <summary>
        /// 夜晚参数
        /// </summary>
	    public CFG_VIDEO_IN_NIGHT_OPTIONS stuNightOptions;
        /// <summary>
        /// 闪光灯配置
        /// </summary>
	    public CFG_FLASH_CONTROL	stuFlash;
        /// <summary>
        /// 抓拍参数, 双快门时有效
        /// </summary>
	    public CFG_VIDEO_IN_SNAPSHOT_OPTIONS stuSnapshot;
        /// <summary>
        /// 保留
        /// </summary>
       /// [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
	    public byte[] reserved;
    }

    public struct CFG_RTSP_INFO_IN
    {
	    Int32 nStructSize;
        /// <summary>
        /// 整个功能是否使能
        /// </summary>
	    bool bEnable;
        /// <summary>
        /// RTSP服务端口
        /// </summary>
        Int32 nPort;
        /// <summary>
        /// RTP起始端口
        /// </summary>
        Int32 nRtpStartPort;
        /// <summary>
        /// RTP结束端口
        /// </summary>
        Int32 nRtpEndPort;
        /// <summary>
        /// RtspOverHttp使能
        /// </summary>
	    bool bHttpEnable;
        /// <summary>
        /// RtspOverHttp端口
        /// </summary>
        Int32 nHttpPort; 
    }

    public struct CFG_RTSP_INFO_OUT
    {
	    Int32 nStructSize;
        /// <summary>
        /// 整个功能是否使能
        /// </summary>
	    bool bEnable; 
        /// <summary>
        /// RTSP服务端口
        /// </summary>
        Int32 nPort;
        /// <summary>
        /// RTP起始端口
        /// </summary>
        Int32 nRtpStartPort;
        /// <summary>
        /// RTP结束端口
        /// </summary>
        Int32 nRtpEndPort;
        /// <summary>
        /// RtspOverHttp使能
        /// </summary>
        bool bHttpEnable;
        /// <summary>
        /// RtspOverHttp端口
        /// </summary>
        Int32 nHttpPort;
    }

    public struct CFG_TRAFFICSNAPSHOT_NEW_INFO
    {
        /// <summary>
        /// 有效成员个数
        /// </summary>
	    public Int32 nCount; 
        /// <summary>
        /// 交通抓拍表数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public CFG_TRAFFICSNAPSHOT_INFO[]	stInfo;
    }

    public struct CFG_MULTICASTS_INFO_IN
    {
	    public Int32 nStructSize;
        /// <summary>
        /// TS的组播配置
        /// CFG_MULTICAST_INFO*
        /// </summary>
        public IntPtr pTSMulticast;
        /// <summary>
        /// 有效TS数组个数
        /// </summary>
        public Int32 nTSCount;
        /// <summary>
        /// RTP的组播配置
        /// CFG_MULTICAST_INFO*
        /// </summary>
        public IntPtr pRTPMulticast;
        /// <summary>
        /// 有效RTP数组个数
        /// </summary>
        public Int32 nRTPCount;
    }

    // RTSP输入参数和输出参数配置结构体
    public struct CFG_MULTICAST_INFO
    {
	    public Int32  nStructSize;
	    public byte abStreamType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] byReserved;
        /// <summary>
        /// 是否使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 组播地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szMulticastAddr;
        /// <summary>
        /// 组播端口
        /// </summary>
	    public Int32  nPort;
        /// <summary>
        /// 单播地址，用于组播指定详细网卡
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szLocalAddr; 
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32  nChannelID;
        /// <summary>
        /// 码流类型 0-主码流, 1-辅码流1,2-辅码流2,3-辅码流3
        /// </summary>
	    public Int32  nStreamType; 
    }

    public struct CFG_MULTICASTS_INFO
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 最大组播配置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public CFG_MULTICAST_INFO[] stuMultiInfo;
        /// <summary>
        /// 有效数组个数
        /// </summary>
	    public Int32 nCount;
    }

    public struct CFG_MULTICASTS_INFO_OUT
    {
	    public Int32                  nStructSize;
        /// <summary>
        /// TS的组播配置
        /// </summary>
	    public CFG_MULTICASTS_INFO  stuTSMulticast;
        /// <summary>
        /// RTP的组播配置
        /// </summary>
	    public CFG_MULTICASTS_INFO  stuRTPMulticast;
    }

    ///////////////////////////////////视频诊断配置///////////////////////////////////////
    // 视频抖动检测
    public struct CFG_VIDEO_DITHER_DETECTION
    {
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
    }

    // 条纹检测
    public struct CFG_VIDEO_STRIATION_DETECTION 
    {
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
        /// <summary>
        /// 字节对齐
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]   
	    public byte[] byReserved1;
        /// <summary>
        /// UV分量是否检测					
        /// </summary>
	    public bool bUVDetection;
    }

    // 视频丢失检测
    public struct CFG_VIDEO_LOSS_DETECTION
    {
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
    }

    // 视频遮挡检测
    public struct CFG_VIDEO_COVER_DETECTION
    {
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
    }

    // 画面冻结检测
    public struct CFG_VIDEO_FROZEN_DETECTION
    {
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
    }

    // 亮度异常检测
    public struct CFG_VIDEO_BRIGHTNESS_DETECTION
    {	
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte bylowerThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte bylowerThrehold2;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byUpperThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byUpperThrehold2;
    }

    // 对比度异常检测
    public struct CFG_VIDEO_CONTRAST_DETECTION
    {	
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte bylowerThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte bylowerThrehold2;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byUpperThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byUpperThrehold2;
    }

    // 偏色检测
    public struct CFG_VIDEO_UNBALANCE_DETECTION
    {	
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
    }

    // 噪声检测
    public struct CFG_VIDEO_NOISE_DETECTION
    {	
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
    }

    // 模糊检测
    public struct CFG_VIDEO_BLUR_DETECTION
    {
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
    }

    // 场景变化检测
    public struct CFG_VIDEO_SCENECHANGE_DETECTION
    {	
        /// <summary>
        /// 使能配置
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 最短持续时间 单位：秒 0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 预警阀值 取值1-100
        /// </summary>
	    public byte byThrehold1;
        /// <summary>
        /// 报警阀值 取值1-100
        /// </summary>
	    public byte byThrehold2;
    }

    public struct CFG_VIDEO_DIAGNOSIS_PROFILE
    {
        /// <summary>
        /// 名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]     
	    public byte[] szName;

        /// <summary>
        /// 视频抖动检测
        /// CFG_VIDEO_DITHER_DETECTION*
        /// </summary>
	    public IntPtr pstDither;
        /// <summary>
        /// 视频条纹检测
        /// CFG_VIDEO_STRIATION_DETECTION*
        /// </summary>
	    public IntPtr	pstStriation;
        /// <summary>
        /// 视频丢失检测
        /// CFG_VIDEO_LOSS_DETECTION*
        /// </summary>
	    public IntPtr pstLoss;
        /// <summary>
        /// 视频遮挡检测
        /// CFG_VIDEO_COVER_DETECTION*
        /// </summary>
	    public IntPtr pstCover;
        /// <summary>
        /// 视频冻结检测
        /// CFG_VIDEO_FROZEN_DETECTION*
        /// </summary>
	    public IntPtr pstFrozen;
        /// <summary>
        /// 视频亮度异常检测
        /// CFG_VIDEO_BRIGHTNESS_DETECTION*
        /// </summary>
	    public IntPtr pstBrightness;
        /// <summary>
        /// 对比度异常检测
        /// CFG_VIDEO_CONTRAST_DETECTION*
        /// </summary>
	    public IntPtr pstContrast;
        /// <summary>
        /// 偏色异常检测
        /// CFG_VIDEO_UNBALANCE_DETECTION*
        /// </summary>
	    public IntPtr pstUnbalance;
        /// <summary>
        /// 噪声检测
        /// CFG_VIDEO_NOISE_DETECTION*
        /// </summary>
	    public IntPtr pstNoise;
        /// <summary>
        ///模糊检测
        /// CFG_VIDEO_BLUR_DETECTION*
        /// </summary>
	    public IntPtr pstBlur;
        /// <summary>
        /// 场景变化检测
        /// CFG_VIDEO_SCENECHANGE_DETECTION*
        /// </summary>
	    public IntPtr pstSceneChange;
    }

    // 视频诊断参数表(CFG_CMD_VIDEODIAGNOSIS_PROFILE)，支持多种参数表，用表名称来索引   调用者申请内存并初始化
    public struct CFG_VIDEODIAGNOSIS_PROFILE
    {
        /// <summary>
        /// 调用者分配参数表数 根据能力集获取
        /// </summary>
	    public Int32 nTotalProfileNum;
        /// <summary>
        /// 返回的实际参数表数
        /// </summary>
	    public Int32 nReturnProfileNum;
        /// <summary>
        /// 调用者分配nProfileCount个VIDEO_DIAGNOSIS_PROFILE
        /// CFG_VIDEO_DIAGNOSIS_PROFILE*
        /// </summary>
	    public IntPtr pstProfiles;
    }

    // 设备详细信息
    public struct CFG_TASK_REMOTEDEVICE
    {
        /// <summary>
        /// 设备地址或域名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]  
	    public byte[] szAddress;
        /// <summary>
        /// 端口号
        /// </summary>
	    public UInt32 dwPort;
        /// <summary>
        /// 用户名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]  
	    public byte[] szUserName;
        /// <summary>
        /// 密码明文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]  
	    public byte[] szPassword;
        /// <summary>
        /// 连接设备的协议类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]  
	    public byte[] szProtocolType;
    }
    
    public struct CFG_TAST_SOURCES
    {
	    // 能力
	    public byte abDeviceID;
	    public byte abRemoteDevice;
        /// <summary>
        /// 设备ID
        /// </summary>
	    public byte[] szDeviceID;
        /// <summary>
        /// 设备详细信息
        /// </summary>
	    public CFG_TASK_REMOTEDEVICE stRemoteDevice;
        /// <summary>
        /// 视频通道号
        /// </summary>
	    public Int32 nVideoChannel;
        /// <summary>
        /// 视频码流类型
        /// </summary>
	    public CFG_EM_STREAM_TYPE emVideoStream;
        /// <summary>
        /// 持续诊断时间
        /// </summary>
	    public Int32 nDuration;
    }

    public struct CFG_DIAGNOSIS_TASK
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]         
	    public byte[] szTaskName;
        /// <summary>
        /// 本任务使用的诊断参数表名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]         
	    public byte[] szProfileName;
        /// <summary>
        /// 调用者分配任务数据源的个数  根据能力集获取
        /// </summary>
	    public Int32 nTotalSourceNum;
        /// <summary>
        /// 返回实际任务数据源的个数
        /// </summary>
	    public Int32 nReturnSourceNum;
        /// <summary>
        /// 任务数据源 调用者分配内存nTotalSourceNum个
        /// CFG_TAST_SOURCES*
        /// </summary>
	    public IntPtr pstSources;
    };

    // 视频诊断任务表(CFG_CMD_VIDEODIAGNOSIS_TASK),不同的任务通过名子索引  调用者申请内存并初始化
    public struct CFG_VIDEODIAGNOSIS_TASK
    {
        /// <summary>
        /// 调用者分配任务个数  根据能力集获取
        /// </summary>
	    public Int32 nTotalTaskNum;
        /// <summary>
        /// 返回实际任务个数
        /// </summary>
	    public Int32 nReturnTaskNum;
        /// <summary>
        /// 任务配置 调用者分配内存nTotalTaskNum个
        /// CFG_DIAGNOSIS_TASK*
        /// </summary>
	    public IntPtr pstTasks;
    }

    // 视频诊断计划
    public struct CFG_PROJECT_TASK
    {
        /// <summary>
        /// 任务是否使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 任务名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
	    public byte[] szTaskName;
        /// <summary>
        /// 任务时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[] stTimeSection; 
    }

    public struct CFG_DIAGNOSIS_PROJECT
    {
        /// <summary>
        /// 计划名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
	    public byte[] szProjectName;
        /// <summary>
        /// 调用者分配任务列表个数  根据能力集获取
        /// </summary>
	    public Int32 nTotalTaskNum;
        /// <summary>
        /// 返回实际任务列表个数
        /// </summary>
	    public Int32 nReturnTaskNum;
        /// <summary>
        /// 任务列表 调用者分配内存nTotalTaskNum个
        /// CFG_PROJECT_TASK*
        /// </summary>
	    public IntPtr pstProjectTasks;
    }

    // 频诊断计划表(CFG_CMD_VIDEODIAGNOSIS_PROJECT),不同的计划通过名字索引 调用者申请内存并初始化
    public struct CFG_VIDEODIAGNOSIS_PROJECT
    {
        /// <summary>
        /// 调用者分配计划个数  根据能力集获取
        /// </summary>
	    public Int32 nTotalProjectNum;
        /// <summary>
        /// 返回实际计划个数
        /// </summary>
	    public Int32 nReturnProjectNum;
        /// <summary>
        /// 计划配置 调用者分配内存nTotalProjectNum个
        /// CFG_DIAGNOSIS_PROJECT*
        /// </summary>
	    public IntPtr pstProjects;
    }

    // 视频诊断全局表(CFG_CMD_VIDEODIAGNOSIS_GLOBAL),每个通道支持一个诊断配置 
    public struct CFG_VIDEODIAGNOSIS_GLOBAL_CHNNL
    {
        /// <summary>
        /// 计划是否使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 配置立即应用	立即应用表示参数表，任务，计划的修改立即生效，否则等到当前正在执行的任务完成后生效。
        /// </summary>
	    public bool bApplyNow;
        /// <summary>
        /// 计划名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)] 
	    public byte[] szProjectName;
    }

    public struct CFG_VIDEODIAGNOSIS_GLOBAL
    {
        /// <summary>
        /// 调用者分配全局配置个数  根据能力集获取
        /// </summary>
	    public Int32 nTotalGlobalNum;
        /// <summary>
        /// 返回实际全局配置个数
        /// </summary>
	    public Int32 nReturnGlobalNum;
        /// <summary>
        /// 视频诊断全局配置 调用者分配内存nGlobalCount个CFG_VIDEODIAGNOSIS_GLOBAL_CHNNL
        /// CFG_VIDEODIAGNOSIS_GLOBAL_CHNNL	*
        /// </summary>
	    public IntPtr pstGlobals;
    }

    // 存储组信息
    public struct CFG_STORAGEGROUP_INFO
    {
        /// <summary>
        /// 存储组名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szGroupName;
        /// <summary>
        /// 物理磁盘序号缓冲区
        /// BYTE*
        /// </summary>
	    public IntPtr byDisks;
        /// <summary>
        /// 缓冲区byDisks的长度
        /// </summary>
	    public Int32 nBufSize;
        /// <summary>
        /// 存储组中的磁盘数
        /// </summary>
	    public Int32 nDiskNum;
        /// <summary>
        /// 存储组序号(1~最大硬盘数)
        /// </summary>
	    public Int32 nGroupIndex;
    }

    // 设备工作状态信息
    public struct CFG_TRAFFIC_WORKSTATE_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 抓拍模式
        /// </summary>
	    public CFG_TRAFFIC_SNAP_MODE   emSnapMode;
        /// <summary>
        /// 抓拍匹配模式: 0-非实时匹配方式，先报警后抓拍，抓拍帧不是报警帧;  1-实时匹配模式，报警帧和抓拍帧是同一帧 
        /// </summary>
	    public Int32 nMatchMode;
    }

    // 录像-存储组 对应信息
    public struct CFG_RECORDTOGROUP_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 使能    
        /// </summary>
	    public bool bEnable; 
        /// <summary>
        /// 存储组名称, 只读
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]         
	    public byte[] szGroupName;
        /// <summary>
        /// 存储组序号(1~最大盘组, 0则表示无对应盘组)，通过此参数与CFG_STORAGE_GROUP_INFO关联
        /// </summary>
	    public Int32 nGroupIndex;
    }

    // 通道名称
    public struct AV_CFG_ChannelName
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 摄像头唯一编号
        /// </summary>
	    public Int32 nSerial;
        /// <summary>
        /// 通道名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szName;
    };

    // 录像模式
    public struct AV_CFG_RecordMode
    {
        public Int32 nStructSize;
        /// <summary>
        /// 录像模式, 0-自动录像，1-手动录像，2-关闭录像
        /// </summary>
        public Int32 nMode;	
    }

    // 视频输出属性
    public struct AV_CFG_VideoOutAttr
    {
        public Int32 nStructSize;
        /// <summary>
        /// 左边距, 比率, 0~100
        /// </summary>
        public Int32 nMarginLeft;
        /// <summary>
        /// 上边距, 比率, 0~100
        /// </summary>
        public Int32 nMarginTop;
        /// <summary>
        /// 右边距, 比率, 0~100
        /// </summary>
        public Int32 nMarginRight;
        /// <summary>
        /// 下边距, 比率, 0~100
        /// </summary>
        public Int32 nMarginBottom;
        /// <summary>
        /// 亮度, 0~100
        /// </summary>
        public Int32 nBrightness;
        /// <summary>
        /// 对比度, 0~100
        /// </summary>
        public Int32 nContrast;
        /// <summary>
        /// 饱和度, 0~100
        /// </summary>
        public Int32 nSaturation;
        /// <summary>
        /// 色调, 0~100
        /// </summary>
        public Int32 nHue;
        /// <summary>
        /// 水平分辨率
        /// </summary>
        public Int32 nWidth;
        /// <summary>
        /// 垂直分辨率
        /// </summary>
        public Int32 nHeight;
        /// <summary>
        /// 颜色深度
        /// </summary>
        public Int32 nBPP;
        /// <summary>
        /// 0-Auto, 1-TV, 2-VGA, 3-DVI
        /// </summary>
        public Int32 nFormat;
        /// <summary>
        /// 刷新频率
        /// </summary>
        public Int32 nRefreshRate;
        /// <summary>
        /// 输出图像增强
        /// </summary>
        public bool bIQIMode;
    };

    // 远程设备
    public  struct AV_CFG_RemoteDevice 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 设备ID
        /// </summary> 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szID;
        /// <summary>
        /// 设备IP
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] 
	    public byte[] szIP;
        /// <summary>
        /// 端口
        /// </summary>
	    public Int32 nPort;
        /// <summary>
        /// 协议类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] 
	    public byte[] szProtocol;
        /// <summary>
        /// 用户名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] 
	    public byte[] szUser;
	    /// <summary>
	    /// 密码
	    /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] 
	    public byte[] szPassword;
        /// <summary>
        /// 设备序列号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] 
	    public byte[] szSerial;
        /// <summary>
        /// 设备类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] 
	    public byte[] szDevClass;
        /// <summary>
        /// 设备型号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] 
	    public byte[] szDevType;
        /// <summary>
        /// 机器名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] 
	    public byte[] szName;
        /// <summary>
        /// 机器部署地点
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] 
	    public byte[] szAddress;
        /// <summary>
        /// 机器分组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szGroup;
        /// <summary>
        /// 清晰度, 0-标清, 1-高清
        /// </summary>
	    public Int32 nDefinition;
        /// <summary>
        /// 视频输入通道数
        /// </summary>
	    public Int32 nVideoChannel;
        /// <summary>
        /// 音频输入通道数
        /// </summary>
	    public Int32 nAudioChannel;
    };

    // 远程通道
    public struct AV_CFG_RemoteChannel 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 设备ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szDeviceID;
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannel;
    };

    // 显示源
    public struct AV_CFG_DisplaySource 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 使能
        /// </summary>
        public bool bEnable;
        /// <summary>
        /// 设备ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] szDeviceID;
        /// <summary>
        /// 视频通道号
        /// </summary>
        public Int32 nVideoChannel;
        /// <summary>
        /// 视频码流
        /// </summary>
        public Int32 nVideoStream;
        /// <summary>
        /// 音频通道号
        /// </summary>
        public Int32 nAudioChannle;
        /// <summary>
        /// 音频码流
        /// </summary>
        public Int32 nAudioStream;
    };

    // 通道分割显示源
    public struct AV_CFG_ChannelDisplaySource 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 分割窗口数量
        /// </summary>
	    public Int32 nWindowNum;
        /// <summary>
        /// 分割窗口显示源
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public AV_CFG_DisplaySource[]	stuSource;
    };

    // Raid信息
    public struct AV_CFG_Raid 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
        /// <summary>
        /// 等级
        /// </summary>
	    public Int32 nLevel;
        /// <summary>
        /// 磁盘成员数量
        /// </summary>
	    public Int32 nMemberNum;
        /// <summary>
        /// 磁盘成员
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 260)]
        public byte[] szMembers;
    };

    // 录像源
    public struct AV_CFG_RecordSource
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 设备ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDeviceID;
        /// <summary>
        /// 视频通道号
        /// </summary>
	    public Int32 nVideoChannel;
        /// <summary>
        /// 视频码流
        /// </summary>
	    public Int32 nVideoStream;
        /// <summary>
        /// 音频通道号
        /// </summary>
	    public Int32 nAudioChannle;
        /// <summary>
        /// 音频码流
        /// </summary>
	    public Int32 nAudioStream;
    };

    // 视频输入颜色配置, 每个视频输入通道对应多个颜色配置
    public struct AV_CFG_VideoColor
    {
        public Int32 nStructSize;
        /// <summary>
        /// 时间段
        /// </summary>
        public AV_CFG_TimeSection stuTimeSection;
        /// <summary>
        /// 亮度, 1~100
        /// </summary>
        public Int32 nBrightness;
        /// <summary>
        /// 对比度, 1~100
        /// </summary>
        public Int32 nContrast;
        /// <summary>
        /// 饱和度, 1~100
        /// </summary>
        public Int32 nSaturation;
        /// <summary>
        /// 色调, 1~100
        /// </summary>
        public Int32 nHue;
    };

    // 通道视频输入颜色配置
    public struct AV_CFG_ChannelVideoColor 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 通道颜色配置数
        /// </summary>
	    public Int32 nColorNum;
        /// <summary>
        /// 通道颜色配置, 每个通道对应多个颜色配置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
	    public AV_CFG_VideoColor[]	stuColor;
    };

    // 颜色
    public struct AV_CFG_Color
    {
        public Int32 nStructSize;
        /// <summary>
        /// 红
        /// </summary>
        public Int32 nRed;
        /// <summary>
        /// 绿
        /// </summary>
        public Int32 nGreen;
        /// <summary>
        /// 蓝
        /// </summary>
        public Int32 nBlue;
        /// <summary>
        /// 透明
        /// </summary>
        public Int32 nAlpha;
    };


    // 区域
    public struct AV_CFG_Rect
    {
        public Int32 nStructSize;
        public Int32 nLeft;
        public Int32 nTop;
        public Int32 nRight;
        public Int32 nBottom;
    };

    // 编码物件-通道标题
    public struct AV_CFG_VideoWidgetChannelTitle
    {
        public Int32 nStructSize;
        /// <summary>
        /// 叠加到主码流
        /// </summary>
        public bool bEncodeBlend;
        /// <summary>
        /// 叠加到辅码流1
        /// </summary>
        public bool bEncodeBlendExtra1;
        /// <summary>
        /// 叠加到辅码流2
        /// </summary>
        public bool bEncodeBlendExtra2;
        /// <summary>
        /// 叠加到辅码流3
        /// </summary>
        public bool bEncodeBlendExtra3;
        /// <summary>
        /// 叠加到抓图
        /// </summary>
        public bool bEncodeBlendSnapshot;
        /// <summary>
        /// 前景色
        /// </summary>
        public AV_CFG_Color stuFrontColor;
        /// <summary>
        /// 背景色
        /// </summary>
        public AV_CFG_Color stuBackColor;
        /// <summary>
        /// 区域, 坐标取值0~8191, 仅使用left和top值, 点(left,top)应和(right,bottom)设置成同样的点
        /// </summary>
        public AV_CFG_Rect stuRect;
    };

    // 编码物件-时间标题
    public struct AV_CFG_VideoWidgetTimeTitle
    {
        public Int32 nStructSize;
        /// <summary>
        /// 叠加到主码流
        /// </summary>
        public bool bEncodeBlend;
        /// <summary>
        /// 叠加到辅码流1
        /// </summary>
        public bool bEncodeBlendExtra1;
        /// <summary>
        /// 叠加到辅码流2
        /// </summary>
        public bool bEncodeBlendExtra2;
        /// <summary>
        /// 叠加到辅码流3
        /// </summary>
        public bool bEncodeBlendExtra3;
        /// <summary>
        /// 叠加到抓图
        /// </summary>
        public bool bEncodeBlendSnapshot;
        /// <summary>
        /// 前景色
        /// </summary>
        public AV_CFG_Color stuFrontColor;
        /// <summary>
        /// 背景色
        /// </summary>
        public AV_CFG_Color stuBackColor;
        /// <summary>
        /// 区域, 坐标取值0~8191, 仅使用left和top值, 点(left,top)应和(right,bottom)设置成同样的点
        /// </summary>
        public AV_CFG_Rect stuRect;
        /// <summary>
        /// 是否显示星期
        /// </summary>
        public bool bShowWeek;
    };

    // 编码物件-区域覆盖配置
    public struct AV_CFG_VideoWidgetCover
    {
        public Int32 nStructSize;
        /// <summary>
        /// 叠加到主码流
        /// </summary>
        public bool bEncodeBlend;
        /// <summary>
        /// 叠加到辅码流1
        /// </summary>
        public bool bEncodeBlendExtra1;
        /// <summary>
        /// 叠加到辅码流2
        /// </summary>
        public bool bEncodeBlendExtra2;
        /// <summary>
        /// 叠加到辅码流3
        /// </summary>
        public bool bEncodeBlendExtra3;
        /// <summary>
        /// 叠加到抓图
        /// </summary>
        public bool bEncodeBlendSnapshot;
        /// <summary>
        /// 前景色
        /// </summary>
        public AV_CFG_Color stuFrontColor;
        /// <summary>
        /// 背景色
        /// </summary>
        public AV_CFG_Color stuBackColor;
        /// <summary>
        /// 区域, 坐标取值0~8191
        /// </summary>
        public AV_CFG_Rect stuRect;
    };

    // 编码物件-自定义标题
    public struct AV_CFG_VideoWidgetCustomTitle 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 叠加到主码流
        /// </summary>
	    public bool bEncodeBlend;
        /// <summary>
        /// 叠加到辅码流1
        /// </summary>
	    public bool bEncodeBlendExtra1;
        /// <summary>
        /// 叠加到辅码流2
        /// </summary>
	    public bool bEncodeBlendExtra2;
        /// <summary>
        /// 叠加到辅码流3
        /// </summary>
	    public bool bEncodeBlendExtra3;
        /// <summary>
        /// 叠加到抓图
        /// </summary>
	    public bool bEncodeBlendSnapshot;
        /// <summary>
        /// 前景色
        /// </summary>
	    public AV_CFG_Color stuFrontColor;
        /// <summary>
        /// 背景色
        /// </summary>
	    public AV_CFG_Color stuBackColor;
        /// <summary>
        /// 区域, 坐标取值0~8191, 仅使用left和top值, 点(left,top)应和(right,bottom)设置成同样的点
        /// </summary>
	    public AV_CFG_Rect stuRect;
        /// <summary>
        /// 标题内容
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public byte[] szText;
    };

    // 视频编码物件配置
    public struct AV_CFG_VideoWidget 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 通道标题
        /// </summary>
	    public AV_CFG_VideoWidgetChannelTitle	stuChannelTitle;
        /// <summary>
        /// 时间标题
        /// </summary>
	    public AV_CFG_VideoWidgetTimeTitle		stuTimeTitle;
        /// <summary>
        /// 区域覆盖数量
        /// </summary>
	    public Int32 nConverNum;
        /// <summary>
        /// 覆盖区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public AV_CFG_VideoWidgetCover[] stuCovers;
        /// <summary>
        /// 自定义标题数量
        /// </summary>
	    public Int32 nCustomTitleNum;
        /// <summary>
        /// 自定义标题
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	    public AV_CFG_VideoWidgetCustomTitle[]	stuCustomTitle;
    };

    // 存储组通道相关配置
    public struct AV_CFG_StorageGroupChannel 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 每个通道文件夹图片存储上限, 超过就覆盖
        /// </summary>
	    public Int32 nMaxPictures;
        /// <summary>
        /// 通道在命名规则里的字符串表示, %c对应的内容
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szPath;
    };

    // 存储组配置
    public struct AV_CFG_StorageGroup 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 分组名称
        /// </summary>
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
        /// <summary>
        /// 分组说明
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] szMemo;
        /// <summary>
        /// 文件保留时间
        /// </summary>
	    public Int32 nFileHoldTime;
        /// <summary>
        /// 存储空间满是否覆盖
        /// </summary>
	    public bool bOverWrite;
        /// <summary>
        /// 录像文件路径命名规则
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
	    public byte[] szRecordPathRule;
        /// <summary>
        /// 图片文件路径命名规则
        /// %y年, %M月, %d日, %h时, %m分, %s秒, %c通道路径
        /// 如果年月日时分秒出现两次, 第一次表示开始时间, 第二次表示结束时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
	    public byte[] szPicturePathRule;
        /// <summary>
        /// 通道相关配置 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public AV_CFG_StorageGroupChannel[]	stuChannels;
        /// <summary>
        /// 通道配置数
        /// </summary>
	    public Int32 nChannelCount;
    };

    // DST时间
    public struct AV_CFG_DSTTime
    {
        public Int32 nStructSize;
        /// <summary>
        /// 年, 2000~2038
        /// </summary>
        public Int32 nYear;
        /// <summary>
        /// 月, 1~12
        /// </summary>
        public Int32 nMonth;
        /// <summary>
        /// 第几周, 1-第一周,2-第二周,...,-1-最后一周,0-按日期计算
        /// </summary>
        public Int32 nWeek;
        /// <summary>
        /// 星期几或日期
        ///按周计算时, 0-周日, 1-周一,..., 6-周六
        /// 按日期算时, 表示几号, 1~31
        /// </summary>
        public Int32 nDay;
        /// <summary>
        /// 小时
        /// </summary>
        public Int32 nHour;
        /// <summary>
        /// 分钟
        /// </summary>
        public Int32 nMinute;
    };

    // 区域配置
    public struct AV_CFG_Locales 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 时间格式
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szTimeFormat;
        /// <summary>
        /// 夏令时时能
        /// </summary>
	    public bool bDSTEnable;
        /// <summary>
        /// 夏令时起始时间
        /// </summary>
	    public AV_CFG_DSTTime stuDstStart;
        /// <summary>
        /// 夏令时结束时间
        /// </summary>
	    public AV_CFG_DSTTime stuDstEnd;
    };

    // 语言配置
    public struct AV_CFG_Language
    {
        public Int32 nStructSize;
        public AV_CFG_LanguageType emLanguage;						// 当前语言
    };

    // 访问地址过滤
    public struct AV_CFG_AccessFilter
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 类型, 0-黑名单, 1-白名单
        /// </summary>
	    public Int32 nType;
        /// <summary>
        /// 白名单IP数量
        /// </summary>
	    public Int32 nWhiteListNum;
        /// <summary>
        /// 白名单
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 96)]
	    public byte[] szWhiteList;
        /// <summary>
        /// 黑名单IP或IP段数量
        /// </summary>
	    public Int32 nBlackListNum;
        /// <summary>
        /// 黑名单
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 96)]
	    public byte[] szBlackList;
    };


    // 自动维护
    public struct AV_CFG_AutoMaintain
    {
        public Int32 nStructSize;
        /// <summary>
        /// 自动重启日期, -1永不, 0~6周日~周六, 7每天
        /// </summary>
        public Int32 nAutoRebootDay;
        /// <summary>
        /// 自动重启小时, 0~23
        /// </summary>
        public Int32 nAutoRebootHour;
        /// <summary>
        /// 自动重启分钟, 0~59
        /// </summary>
        public Int32 nAutoRebootMinute;
        /// <summary>
        /// 自动关机日期
        /// </summary>
        public Int32 nAutoShutdownDay;
        /// <summary>
        /// 自动关机小时
        /// </summary>
        public Int32 nAutoShutdownHour;
        /// <summary>
        /// 自动关机分钟
        /// </summary>
        public Int32 nAutoShutdownMinute;
        /// <summary>
        /// 自动启动日期
        /// </summary>
        public Int32 nAutoStartupDay;
        /// <summary>
        /// 自动启动小时
        /// </summary>
        public Int32 nAutoStartupHour;
        /// <summary>
        /// 自动启动分钟
        /// </summary>
        public Int32 nAutoStartupMinute;
    };

    // 远程设备事件处理
    public struct AV_CFG_RemoteEvent 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 设备ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDeviceID;
        /// <summary>
        /// 事件码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szCode;
        /// <summary>
        /// 序号
        /// </summary>
	    public Int32 nIndex;
    };

    // 电视墙输出通道信息
    public struct AV_CFG_MonitorWallTVOut
    {
	    public Int32		nStructSize;
        /// <summary>
        /// 设备ID, 为空或"Local"表示本地设备
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szDeviceID;
        /// <summary>
        /// 通道ID
        /// </summary>
	    public Int32		nChannelID;				
        /// <summary>
        /// 屏幕名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
    };

    // 电视墙区块
    public struct AV_CFG_MonitorWallBlock 
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 单个TV占的网格行数
        /// </summary>
	    public Int32 nLine;
        /// <summary>
        /// 单个TV占的网格列数
        /// </summary>
	    public Int32 nColumn;
        /// <summary>
        /// 区块的区域坐标
        /// </summary>
	    public AV_CFG_Rect stuRect;
        /// <summary>
        /// TV数量
        /// </summary>
	    public Int32 nTVCount;
        /// <summary>
        /// TV数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public AV_CFG_MonitorWallTVOut[]	stuTVs;
    };

    // 电视墙
    public struct AV_CFG_MonitorWall
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
        /// <summary>
        /// 网络行数
        /// </summary>
	    public Int32 nLine;
        /// <summary>
        /// 网格列数
        /// </summary>
	    public Int32 nColumn;
        /// <summary>
        /// 区块数量
        /// </summary>
	    public Int32 nBlockCount;
        /// <summary>
        /// 区块数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public AV_CFG_MonitorWallBlock[] stuBlocks;

    };

    // 拼接屏
    public struct AV_CFG_SpliceScreen
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 拼接屏名称	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
        /// <summary>
        /// 所属电视墙名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szWallName;
        /// <summary>
        /// 所属区块序号
        /// </summary>
	    public Int32 nBlockID;
        /// <summary>
        /// 区域坐标(0~8191)
        /// </summary>
	    public AV_CFG_Rect stuRect;
    };

    // 联动云台信息
    public struct AV_CFG_PtzLink
    {
        public Int32 nStructSize;
        /// <summary>
        /// 联动类型 
        /// </summary>
        public AV_CFG_PtzLinkType emType;
        /// <summary>
        /// 联动参数1
        /// </summary>
        public Int32 nParam1;
        /// <summary>
        /// 联动参数2
        /// </summary>
        public Int32 nParam2;
        /// <summary>
        /// 联动参数3
        /// </summary>
        public Int32 nParam3;
        /// <summary>
        /// 所联动云台通道
        /// </summary>
        public Int32 nChannelID;
    } 

    // 坐标点
    public struct AV_CFG_Point
    {
	    public Int32 nStructSize;
	    public Int32 nX;
	    public Int32 nY;
    } 

    // 宽高
    public struct AV_CFG_Size
    {
	    public Int32 nStructSize;
	    public UInt32 nWidth;
	    public UInt32 nHeight;
    } 	


    // 事件标题内容
    public struct AV_CFG_EventTitle
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 标题文本
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szText;
        /// <summary>
        /// 标题左上角坐标, 采用0-8191相对坐标系
        /// </summary>
	    public AV_CFG_Point		stuPoint;
        /// <summary>
        /// 标题的宽度和高度,采用0-8191相对坐标系，某项或者两项为0表示按照字体自适应宽高
        /// </summary>
	    public AV_CFG_Size			stuSize;
        /// <summary>
        /// 前景颜色
        /// </summary>
        public AV_CFG_Color		stuFrontColor;
        /// <summary>
        /// 背景颜色
        /// </summary>
        public AV_CFG_Color		stuBackColor;
    } 

    // 轮巡联动配置
    public struct AV_CFG_TourLink
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 轮巡使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 轮巡时的分割模式
        /// </summary>
	    public AV_CFG_SplitMode emSplitMode;
        /// <summary>
        /// 轮巡通道号列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public Int32[] nChannels;
        /// <summary>
        /// 轮巡通道数量
        /// </summary>
        public Int32 nChannelCount;
    } 


    // 报警联动
    public  struct AV_CFG_EventHandler
    {
	    public Int32			nStructSize;
        /// <summary>
        /// 事件响应时间表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public AV_CFG_TimeSection[]  stuTimeSect; 
        /// <summary>
        /// 录像使能
        /// </summary>
	    public bool bRecordEnable;
        /// <summary>
        /// 录像通道号列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public UInt32[] nRecordMask;
        /// <summary>
        /// 能力集, 标识nRecordLatch是否有效
        /// </summary>
	    public bool abRecordLatch;
        /// <summary>
        /// 录像延时时间（10~300秒）
        /// </summary>
	    public Int32 nRecordLatch;
        /// <summary>
        /// 报警输出使能
        /// </summary>
	    public bool bAlarmOutEn;
        /// <summary>
        /// 报警输出通道号列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public UInt32[] nAlarmOutMask;
        /// <summary>
        /// 能力集, 标识nAlarmOutLatch是否有效
        /// </summary>
	    public bool abAlarmOutLatch;
        /// <summary>
        /// 报警输出延时时间（10~300秒）
        /// </summary>
	    public Int32 nAlarmOutLatch;
        /// <summary>
        /// 扩展报警输出使能
        /// </summary>
	    public bool bExAlarmOutEn;
        /// <summary>
        /// 扩展报警输出通道列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public UInt32[] nExAlarmOutMask;
        /// <summary>
        /// 云台联动使能
        /// </summary>
	    public bool bPtzLinkEn;
        /// <summary>
        /// 有效联动项数目
        /// </summary>
	    public Int32 nPtzLinkNum;
        /// <summary>
        /// 云台联动项
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public AV_CFG_PtzLink[]		stuPtzLink;
        /// <summary>
        /// 快照使能
        /// </summary>
	    public bool bSnapshotEn;
        /// <summary>
        /// 快照通道号列表	
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public UInt32[] nSnapshotMask;
        /// <summary>
        /// // 能力集, 标识nSnapshotPeriod是否有效
        /// </summary>
	    public bool abSnapshotPeriod;	
        /// <summary>
        /// 帧间隔，每隔多少帧抓一张图片，一定时间内抓拍的张数还与抓图帧率有关。0表示不隔帧，连续抓拍。
        /// </summary>
	    public Int32 nSnapshotPeriod;	
        /// <summary>
        /// 能力集, nSnapshotTimes有效性
        /// </summary>
	    public bool abSnapshotTimes;	
        /// <summary>
        /// 连拍次数, 在SnapshotEnable为true的情况下，SnapshotTimes为0则表示持续抓拍，直到事件结束。
        /// </summary>
	    public Int32 nSnapshotTimes;		
        /// <summary>
        /// 是否叠加图片标题
        /// </summary>
	    public bool bSnapshotTitleEn;	
        /// <summary>
        /// 有效图片标题数目
        /// </summary>
	    public Int32 nSnapTitleNum; 
        /// <summary>
        /// 图片标题内容
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public AV_CFG_EventTitle[]   stuSnapTitles;
        /// <summary>
        /// 本地消息框提示
        /// </summary>
	    public bool bTipEnable;
        /// <summary>
        /// 发送邮件，如果有图片，作为附件
        /// </summary>
	    public bool bMailEnable;
        /// <summary>
        /// 上传到报警服务器
        /// </summary>
	    public bool bMessageEnable;
        /// <summary>
        /// 蜂鸣
        /// </summary>
	    public bool bBeepEnable;
        /// <summary>
        /// 语音提示
        /// </summary>
	    public bool bVoiceEnable;
        /// <summary>
        /// 能力集, nDejitter有效性
        /// </summary>
	    public bool abDejitter;
        /// <summary>
        /// 信号去抖动时间，单位为秒,0~100
        /// </summary>
	    public Int32 nDejitter;
        /// <summary>
        /// 是否记录日志
        /// </summary>
	    public bool bLogEnable;
        /// <summary>
        /// nDelay有效性
        /// </summary>
	    public bool abDelay;
        /// <summary>
        /// 设置时先延时再生效, 单位为秒
        /// </summary>
	    public Int32 nDelay;
        /// <summary>
        /// 是否叠加视频标题，主要指主码流
        /// </summary>
	    public bool bVideoTitleEn;
        /// <summary>
        /// 有效视频标题数目
        /// </summary>
	    public Int32 nVideoTitleNum;
        /// <summary>
        /// 视频标题内容
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public AV_CFG_EventTitle[]	stuVideoTitles;
        /// <summary>
        /// 发送彩信使能
        /// </summary>
	    public bool bMMSEnable;
        /// <summary>
        /// 轮巡联动数目，和视频输出一致
        /// </summary>
	    public Int32 nTourNum;
        /// <summary>
        /// 轮巡联动配置, 每个视频输出对应一个配置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public AV_CFG_TourLink[]	    stuTour;
        /// <summary>
        /// 关键字数量
        /// </summary>
	    public Int32 nDBKeysNum;
        /// <summary>
        /// 关键字
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 32)]
	    public byte[] szDBKeys;
        /// <summary>
        /// 能力集, 标识byJpegSummary是否有效
        /// </summary>
	    public bool abJpegSummary;
        /// <summary>
        /// 叠加到JPEG图片的摘要信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
	    public byte[] byJpegSummary;
    } 

    // 温度报警配置
    public  struct AV_CFG_TemperatureAlarm
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 报警使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 传感器名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
        /// <summary>
        /// 正常温度最小值
        /// </summary>
	    public float fNormalTempMin;
        /// <summary>
        /// 正常温度最大值
        /// </summary>
	    public float fNormalTempMax;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public  AV_CFG_EventHandler stuEventHandler;
    } 

    // 风扇转速报警配置
    public  struct AV_CFG_FanSpeedAlarm
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 报警使能
        /// </summary>
	    public bool bEnable;
        /// <summary>
        /// 传感器名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	    public byte[] szName;
        /// <summary>
        /// 正常转速最小值
        /// </summary>
	    public UInt32 nNormalSpeedMin;
        /// <summary>
        /// 正常转速最大值
        /// </summary>
	    public UInt32 nNormalSpeedMax;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public AV_CFG_EventHandler stuEventHandler;
    } 

    // 录像回传配置
    public struct AV_CFG_RecordBackup
    {
	    public Int32 nStructSize;
        /// <summary>
        /// 最大流量配置, 单位Kbps
        /// </summary>
	    public UInt32 nBitrateLimit;
    } ;

    #endregion

    #region <<交通卡口事件 -- 对应的规则配置>>


    // 事件类型EVENT_IVS_TRAFFICGATE(交通卡口事件)对应的规则配置
    public struct CFG_TRAFFICGATE_INFO
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 车道编号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 检测线(虚拟线圈)1顶点数
        /// </summary>
	    public Int32 nDetectLinePoint1;
        /// <summary>
        /// 检测线1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuDetectLine1;
        /// <summary>
        /// 检测线(虚拟线圈)2顶点数
        /// </summary>
	    public Int32 nDetectLinePoint2;
        /// <summary>
        /// 检测线2
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuDetectLine2;
        /// <summary>
        /// 左车道线顶点数
        /// </summary>
	    public Int32 nLeftLinePoint;
        /// <summary>
        /// 左车道线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuLeftLine;
        /// <summary>
        /// 右车道线顶点数
        /// </summary>
	    public Int32 nRightLinePoint;
        /// <summary>
        /// 右车道线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuRightLine;
        /// <summary>
        /// 速度权重系数(最终车速=测量车速*权重系数)
        /// </summary>
	    public Int32 nSpeedWeight;
        /// <summary>
        /// 两条检测线实际距离,单位：米
        /// </summary>
	    public double MetricDistance;
        /// <summary>
        /// 速度上限 0表示不限上限 单位：km/h
        /// </summary>
	    public Int32 nSpeedUpperLimit;
        /// <summary>
        /// 速度下限 0表示不限下限 单位：km/h
        /// </summary>
	    public Int32 nSpeedLowerLimit;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 车道方向(车开往的方向)，0-北 1-东北 2-东 3-东南 4-南 5-西南 6-西 7-西北
        /// </summary>
	    public Int32 nDirection;
        /// <summary>
        /// 触发模式个数
        /// </summary>
	    public Int32 nTriggerModeNum;
        /// <summary>
        /// 触发模式，"OverLine":压线,"Retrograde":逆行,"OverSpeed":超速,"UnderSpeed":欠速
        /// "Passing":穿过路口，属正常抓拍, "WrongRunningRoute":有车占道(单独使用),"YellowVehicleInRoute": 黄牌占道
        /// "OverYellowLine":压黄线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 32)]
	    public byte[] szTriggerMode;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
        /// <summary>
        /// 是否屏蔽逆行，即将逆行当作正常处理
        /// </summary>
	    public bool bMaskRetrograde;
    } 

    // 事件类型EVENT_IVS_TRAFFICJUNCTION(交通路口事件)对应的规则配置
    public struct CFG_TRAJUNCTION_INFO
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 车道编号
        /// </summary>
	    public Int32 nLane;
        /// <summary>
        /// 车道方向(车开往的方向),0-北 1-东北 2-东 3-东南 4-南 5-西南 6-西 7-西北
        /// </summary>
	    public Int32 nDirection;
        /// <summary>
        /// 前置检测线顶点数
        /// </summary>
	    public Int32 nPreLinePoint;
        /// <summary>
        /// 前置检测线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuPreLine;
        /// <summary>
        /// 中间检测线顶点数
        /// </summary>
	    public Int32 nMiddleLinePoint;
        /// <summary>
        /// 中间检测线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuMiddleLine;
        /// <summary>
        /// 后置检测线顶点数
        /// </summary>
	    public Int32 nPostLinePoint;
        /// <summary>
        /// 后置检测线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuPostLine;
        /// <summary>
        /// 流量上限，单位辆/秒
        /// </summary>
	    public Int32 nFlowLimit;
        /// <summary>
        /// 速度下限，若为0，则表示不设下限，km/h
        /// </summary>
	    public Int32 nSpeedDownLimit;
        /// <summary>
        /// 速度上限，若为0，则表示不设上限，km/h
        /// </summary>
	    public Int32 nSpeedUpLimit;
        /// <summary>
        /// 触发模式个数
        /// </summary>
	    public Int32 nTriggerModeNum;
        /// <summary>
        /// 触发模式，"Passing" : 穿过路口(以中间检测线为准，只能单独使用),"RunRedLight" : 闯红灯
        /// "Overline":压白车道线,"OverYellowLine": 压黄线, "Retrograde":逆行
        /// "TurnLeft":违章左转, "TurnRight":违章右转, "CrossLane":违章变道
        /// "U-Turn" 违章掉头, "Parking":违章停车, "WaitingArea" 违章进入待行区
        /// "OverSpeed": 超速,"UnderSpeed":欠速,"Overflow" : 流量过大
        /// "Human":行人,"NoMotor":非机动车
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 32)]
	    public byte[] szTriggerMode;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
        /// <summary>
        /// 是否屏蔽逆行，即将逆行当作正常处理
        /// </summary>
	    public bool bMaskRetrograde;
    			
    } 

    // 事件类型EVENT_IVS_TRAFFICACCIDENT(交通事故事件)对应的规则配置
    public struct CFG_TRAACCIDENT_INFO
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 检测区顶点数
        /// </summary>
	    public Int32 nDetectRegionPoint;
        /// <summary>
        /// 检测区
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
    	
    } 

    // 事件类型EVENT_IVS_TRAFFICCONTROL(交通管制事件)对应的规则配置
    public struct CFG_TRAFFICCONTROL_INFO 
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 检测线顶点数
        /// </summary>
	    public Int32 nDetectLinePoint;
        /// <summary>
        /// 检测线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuDetectLine;
        /// <summary>
        /// 限行时间表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSchedule;
        /// <summary>
        /// 车辆大小类型个数
        /// </summary>
	    public Int32 nVehicleSizeNum;
        /// <summary>
        /// 车辆大小类型列表"Light-duty":小型车;	"Medium":中型车; "Oversize":大型车
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 32)]
	    public byte[] szVehicleSizeList;
        /// <summary>
        /// 车牌类型个数
        /// </summary>
	    public Int32 nPlateTypeNum;
        /// <summary>
        /// 车牌类型列表"Unknown" 未知; "Normal" 蓝牌黑牌; "Yellow" 黄牌; "DoubleYellow" 双层黄尾牌
        /// "Police" 警牌; "Armed" 武警牌; "Military" 部队号牌; "DoubleMilitary" 部队双层
        /// "SAR" 港澳特区号牌; "Trainning" 教练车号牌; "Personal" 个性号牌; "Agri" 农用牌
        /// "Embassy" 使馆号牌; "Moto" 摩托车号牌; "Tractor" 拖拉机号牌; "Other" 其他号牌
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 32)]
	    public byte[] szPlateTypesList;
        /// <summary>
        /// 车牌单双号 0:单号; 1:双号; 2:单双号;	
        /// </summary>
	    public Int32 nPlateNumber;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
    } 


    // 事件类型EVENT_IVS_FACEDETECT(人脸检测事件)对应的规则配置
    public struct CFG_FACEDETECT_INFO
    {
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public bool bRuleEnable;
	    /// <summary>
	    /// 保留字段
	    /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public int nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 检测区顶点数
        /// </summary>
	    public int nDetectRegionPoint;
        /// <summary>
        /// 检测区
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 触发事件的人脸类型个数
        /// </summary>
	    public Int32 nHumanFaceTypeCount;
        /// <summary>
        /// 触发事件的人脸类型
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 32)]
	    public byte[] szHumanFaceType;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
    }

    // 事件类型EVENT_IVS_PASTEDETECTION(ATM贴条事件)对应的规则配置
    public struct CFG_PASTE_INFO
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 最短持续时间	单位：秒，0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionPoint;
        /// <summary>
        /// 检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
    		
    } 

    // 视频分析事件规则配置
    // 事件类型EVENT_IVS_CROSSLINEDETECTION(警戒线事件)对应的规则配置
    public struct CFG_CROSSLINE_INFO
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 检测方向:0:由左至右;1:由右至左;2:两者都可以
        /// </summary>
	    public Int32 nDirection;
        /// <summary>
        /// 警戒线顶点数
        /// </summary>
	    public Int32 nDetectLinePoint;
        /// <summary>
        /// 警戒线
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYLINE[] stuDetectLine;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
        /// <summary>
        /// 规则特定的尺寸过滤器是否有效
        /// </summary>
	    public bool bSizeFileter;
        /// <summary>
        /// 规则特定的尺寸过滤器
        /// </summary>
	    public CFG_SIZEFILTER_INFO stuSizeFileter;
        /// <summary>
        /// 触发报警位置数
        /// </summary>
	    public Int32 nTriggerPosition;      
        /// <summary>
        /// 触发报警位置,0-目标外接框中心, 1-目标外接框左端中心, 2-目标外接框顶端中心, 3-目标外接框右端中心, 4-目标外接框底端中心
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public byte[] bTriggerPosition;

    } 

    // 事件类型EVENT_IVS_CROSSREGIONDETECTION(警戒区事件)对应的规则配置
    public struct CFG_CROSSREGION_INFO
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16* 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 检测方向:0:Enter;1:Leave;2:Both
        /// </summary>
	    public Int32 nDirection;
        /// <summary>
        /// 警戒区顶点数
        /// </summary>
	    public Int32 nDetectRegionPoint;
        /// <summary>
        /// 警戒区
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
        /// <summary>
        /// 规则特定的尺寸过滤器是否有效
        /// </summary>
	    public bool bSizeFileter;
        /// <summary>
        /// 规则特定的尺寸过滤器
        /// </summary>
	    public CFG_SIZEFILTER_INFO stuSizeFileter;
        /// <summary>
        /// 检测动作数
        /// </summary>
	    public Int32 nActionType;
        /// <summary>
        /// 检测动作列表,0-出现 1-消失 2-在区域内 3-穿越区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	    public byte[] bActionType;
        /// <summary>
        /// 最小目标个数(当bActionType中包含"2-在区域内"时有效)
        /// </summary>
	    public Int32 nMinTargets;
        /// <summary>
        /// 最大目标个数(当bActionType中包含"2-在区域内"时有效)
        /// </summary>
	    public Int32 nMaxTargets;
        /// <summary>
        /// 最短持续时间， 单位秒(当bActionType中包含"2-在区域内"时有效)
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 报告时间间隔， 单位秒(当bActionType中包含"2-在区域内"时有效)
        /// </summary>
	    public Int32 nReportInterval;
    		
    } 

    // 事件类型EVENT_IVS_WANDERDETECTION(徘徊事件)对应的规则配置
    public struct CFG_WANDER_INFO 
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 最短持续时间	单位：秒，0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionPoint;
        /// <summary>
        /// 检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;
        /// <summary>
        /// 触发报警位置数
        /// </summary>
	    public Int32 nTriggerPosition;
        /// <summary>
        /// 触发报警位置,0-目标外接框中心, 1-目标外接框左端中心, 2-目标外接框顶端中心, 3-目标外接框右端中心, 4-目标外接框底端中心 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public byte[] bTriggerPosition;
        /// <summary>
        /// 触发报警的徘徊或滞留人数
        /// </summary>
	    public Int32 nTriggerTargetsNumber;
        /// <summary>
        /// 报告时间间隔,单位秒
        /// </summary>
	    public Int32 nReportInterval;
        /// <summary>
        /// 规则特定的尺寸过滤器是否有效
        /// </summary>
	    public bool bSizeFileter;
        /// <summary>
        /// 规则特定的尺寸过滤器
        /// </summary>
	    public CFG_SIZEFILTER_INFO stuSizeFileter;

    } 

    // 事件类型EVENT_IVS_RIOTERDETECTION(聚众事件)对应的规则配置
    public struct CFG_RIOTER_INFO 
    {
	    // 信息
        /// <summary>
        /// 规则名称,不同规则不能重名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szRuleName;
        /// <summary>
        /// 规则使能
        /// </summary>
	    public byte bRuleEnable;
        /// <summary>
        /// 聚集所占区域面积百分比
        /// </summary>
	    public byte bAreaPercent;
        /// <summary>
        /// 灵敏度，取值1-10，值越小灵敏度越低，对应人群的密集程度越高(取代bAreaPercent)
        /// </summary>
	    public byte bSensitivity;
        /// <summary>
        /// 保留字段
        /// </summary>
	    public byte bReserved;
        /// <summary>
        /// 相应物体类型个数
        /// </summary>
	    public Int32 nObjectTypeNum;
        /// <summary>
        /// 相应物体类型列表
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 32)]
	    public byte[] szObjectTypes;
        /// <summary>
        /// 最短持续时间	单位：秒，0~65535
        /// </summary>
	    public Int32 nMinDuration;
        /// <summary>
        /// 检测区域顶点数
        /// </summary>
	    public Int32 nDetectRegionPoint;
        /// <summary>
        /// 检测区域
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public CFG_POLYGON[] stuDetectRegion;
        /// <summary>
        /// 报警联动
        /// </summary>
	    public CFG_ALARM_MSG_HANDLE stuEventHandler;
        /// <summary>
        /// 事件响应时间段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 6)]
	    public CFG_TIME_SECTION[]	stuTimeSection;
        /// <summary>
        /// 云台预置点编号	0~65535
        /// </summary>
	    public Int32 nPtzPresetId;

    }

    public struct ReservedDataIntelBox
    {
        /// <summary>
        /// 事件个数
        /// </summary>
        public UInt32 dwEventCount;
        /// <summary>
        /// 指向连续的事件类型的值。空间由用户自己申请。
        /// DWORD* dwPtrEventType;
        /// </summary>
        public IntPtr dwPtrEventType;
        /// <summary>
        /// 保留字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] bReserved;
    }

    public struct NET_RESERVED_COMMON
    {
        public UInt32 dwStructSize;
        /// <summary>
        /// 兼容RESERVED_TYPE_FOR_INTEL_BOX
        /// c++下定义：
        /// ReservedDataIntelBox* pIntelBox;
        /// </summary>
        public IntPtr pIntelBox;
        /// <summary>
        /// 抓图标志(按位)，0位:"*",1位:"Timing",2位:"Manual",3位:"Marked",4位:"Event",5位:"Mosaic",6位:"Cutout"
        /// </summary>
        public UInt32 dwSnapFlagMask;
    }

    public struct ReservedPara
    {
        /// <summary>
        /// pData的数据类型
        /// </summary>
        public UInt32 dwType;
        /// <summary>
        ///当[dwType]为RESERVED_TYPE_FOR_INTEL_BOX 时，pData 对应为结构体ReservedDataIntelBox的地址					
        /// 当[dwType]为...时，[pData]对应...
        /// 数据
        /// c++下定义为：
        /// void*	pData;	//数据
        /// </summary>
        public IntPtr pData;

    }
    #endregion

    #region <<智能交通--下载图片的相关结构体>>

    // DH_MEDIA_QUERY_TRAFFICCAR对应的查询条件
    public struct  MEDIA_QUERY_TRAFFICCAR_PARAM
    {
        /// <summary>
        /// 通道号从0开始，-1表示查询所有通道
        /// </summary>
	    public Int32 nChannelID;
        /// <summary>
        /// 开始时间	
        /// </summary>
	    public NET_TIME StartTime;
        /// <summary>
        /// 结束时间
        /// </summary>
	    public NET_TIME EndTime;
        /// <summary>
        /// 文件类型，0:查询任意类型,1:查询jpg图片
        /// </summary>
	    public Int32 nMediaType;
        /// <summary>
        /// 事件类型，详见"智能分析事件类型", 0:表示查询任意事件
        /// </summary>
	    public Int32 nEventType;
        /// <summary>
        /// 车牌号, "\0"则表示查询任意车牌号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szPlateNumber;
        /// <summary>
        /// 查询的车速范围; 速度上限 单位: km/h
        /// </summary>
	    public Int32 nSpeedUpperLimit;
        /// <summary>
        /// 查询的车速范围; 速度下限 单位: km/h 
        /// </summary>
	    public Int32 nSpeedLowerLimit;
        /// <summary>
        /// 是否按速度查询; TRUE:按速度查询,nSpeedUpperLimit和nSpeedLowerLimit有效。
        /// </summary>
	    public bool bSpeedLimit;
        /// <summary>
        /// 违章类型：
        /// 当事件类型为 EVENT_IVS_TRAFFICGATE时
        /// 第一位:逆行;  第二位:压线行驶; 第三位:超速行驶; 
        /// 第四位：欠速行驶; 第五位:闯红灯;
        /// 当事件类型为 EVENT_IVS_TRAFFICJUNCTION
        /// 第一位:闯红灯;  第二位:不按规定车道行驶;  
        /// 第三位:逆行; 第四位：违章掉头;
        /// 第五位:压线行驶;
        /// </summary>
        public UInt32 dwBreakingRule;
        /// <summary>
        /// 车牌类型，"Unknown" 未知,"Normal" 蓝牌黑牌,"Yellow" 黄牌,"DoubleYellow" 双层黄尾牌,"Police" 警牌"Armed" 武警牌,
        /// "Military" 部队号牌,"DoubleMilitary" 部队双层,"SAR" 港澳特区号牌,"Trainning" 教练车号牌
        /// "Personal" 个性号牌,"Agri" 农用牌,"Embassy" 使馆号牌,"Moto" 摩托车号牌,"Tractor" 拖拉机号牌,"Other" 其他号牌
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] szPlateType;
        /// <summary>
        /// 车牌颜色, "Blue"蓝色,"Yellow"黄色, "White"白色,"Black"黑色
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szPlateColor;
        /// <summary>
        /// 车身颜色:"White"白色, "Black"黑色, "Red"红色, "Yellow"黄色, "Gray"灰色, "Blue"蓝色,"Green"绿色
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szVehicleColor;
        /// <summary>
        /// 车辆大小类型:"Light-duty":小型车;"Medium":中型车; "Oversize":大型车
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] szVehicleSize;
        /// <summary>
        /// 事件组编号(此值>=0时有效)
        /// </summary>
	    public Int32 nGroupID;
        /// <summary>
        /// 车道号(此值>=0时有效)
        /// </summary>
	    public Int16 byLane;
        /// <summary>
        /// 文件标志, 0xFF-使用nFileFlagEx, 0-表示所有录像, 1-定时文件, 2-手动文件, 3-事件文件, 4-重要文件, 5-合成文件
        /// </summary>
	    public byte byFileFlag;
        /// <summary>
        /// 文件标志, 按位表示: bit0-定时文件, bit1-手动文件, bit2-事件文件, bit3-重要文件, bit4-合成文件, 0xFFFFFFFF-所有录像
        /// </summary>
	    public Int32 nFileFlagEx;
	    public byte reserved;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
	    public Int32[] bReserved;
    }

    // DH_MEDIA_QUERY_TRAFFICCAR查询出来的media文件信息
    public struct MEDIAFILE_TRAFFICCAR_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public UInt32 ch;
        /// <summary>
        /// 文件路径
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szFilePath;
        /// <summary>
        /// 文件长度
        /// </summary>
        public UInt32 size;
        /// <summary>
        /// 开始时间
        /// </summary>
        public NET_TIME starttime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public NET_TIME endtime;
        /// <summary>
        /// 工作目录编号									
        /// </summary>
        public UInt32 nWorkDirSN;
        /// <summary>
        /// 文件类型  1：jpg图片
        /// </summary>
        public byte nFileType;
        /// <summary>
        /// 文件定位索引
        /// </summary>
        public byte bHint;
        /// <summary>
        /// 磁盘号
        /// </summary>
        public byte bDriveNo;
        public byte bReserved2;
        /// <summary>
        /// 簇号
        /// </summary>
        public UInt32 nCluster;
        /// <summary>
        /// 图片类型, 0-普通, 1-合成, 2-抠图
        /// </summary>
        public byte byPictureType;
        /// <summary>
        /// 保留字段
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] bReserved;

        //以下是交通车辆信息
        /// <summary>
        /// 车牌号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szPlateNumber;
        /// <summary>
        /// 号牌类型"Unknown" 未知; "Normal" 蓝牌黑牌; "Yellow" 黄牌; "DoubleYellow" 双层黄尾牌
        /// "Police" 警牌; "Armed" 武警牌; "Military" 部队号牌; "DoubleMilitary" 部队双层
        /// "SAR" 港澳特区号牌; "Trainning" 教练车号牌; "Personal" 个性号牌; "Agri" 农用牌
        /// "Embassy" 使馆号牌; "Moto" 摩托车号牌; "Tractor" 拖拉机号牌; "Other" 其他号牌
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szPlateType;
        /// <summary>
        /// 车牌颜色:"Blue","Yellow", "White","Black"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szPlateColor;
        /// <summary>
        /// 车身颜色:"White", "Black", "Red", "Yellow", "Gray", "Blue","Green"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szVehicleColor;
        /// <summary>
        /// 车速,单位Km/H
        /// </summary>
        public Int32 nSpeed;
        /// <summary>
        /// 关联的事件个数
        /// </summary>
        public Int32 nEventsNum;
        /// <summary>
        /// 关联的事件列表,数组值表示相应的事件，详见"智能分析事件类型"		
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] nEvents;
        /// <summary>
        /// 具体违章类型掩码,第一位:闯红灯; 第二位:不按规定车道行驶;
        /// 第三位:逆行; 第四位：违章掉头;否则默认为:交通路口事件
        /// </summary>
        public UInt32 dwBreakingRule;
        /// <summary>
        /// 车辆大小类型:"Light-duty":小型车;"Medium":中型车; "Oversize":大型车
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szVehicleSize;
        /// <summary>
        /// 本地或远程的通道名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szChannelName;
        /// <summary>
        /// 本地或远程设备名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] szMachineName;
        /// <summary>
        /// 速度上限 单位: km/h
        /// </summary>
        public Int32 nSpeedUpperLimit;
        /// <summary>
        /// 速度下限 单位: km/h	
        /// </summary>
        public Int32 nSpeedLowerLimit;
        /// <summary>
        /// 事件里的组编号
        /// </summary>
        public Int32 nGroupID;
        /// <summary>
        /// 一个事件组内的抓拍张数
        /// </summary>
        public byte byCountInGroup;
        /// <summary>
        /// 一个事件组内的抓拍序号
        /// </summary>
        public byte byIndexInGroup;
        /// <summary>
        /// 车道
        /// </summary>
        public byte byLane;
        /// <summary>
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 49)]
        public byte[] bReserved1;
    } 


    public struct DH_DEV_ENABLE_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
	    public UInt32[] IsFucEnable;				// Function list capacity set. Corresponding to the above mentioned enumeration. Use bit to represent sub-function.
    }

    // 抓图功能属性结构体
    public struct DH_QUERY_SNAP_INFO
    {
        /// <summary>
        /// 通道号
        /// </summary>
	    public Int32 nChannelNum; 
        /// <summary>
        /// 分辨率(按位)，具体查看枚举CAPTURE_SIZE
        /// </summary>
	    public UInt32 dwVideoStandardMask;
		/// <summary>
		/// Frequence[128]数组的有效长度
		/// </summary>
	    public Int32 nFramesCount;
        /// <summary>
        /// 帧率(按数值)
        /// -25：25秒1帧；-24：24秒1帧；-23：23秒1帧；-22：22秒1帧
        /// ……
        /// 0：无效；1：1秒1帧；2：1秒2帧；3：1秒3帧
        /// 4：1秒4帧；5：1秒5帧；17：1秒17帧；18：1秒18帧
        /// 19：1秒19帧；20：1秒20帧
        /// ……
        /// 25: 1秒25帧
        /// char
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public sbyte[] Frames; 
        /// <summary>
        /// SnapMode[16]数组的有效长度
        /// </summary>
	    public Int32 nSnapModeCount;
        /// <summary>
        /// (按数值)0：定时触发抓图，1：手动触发抓图
        /// char
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] SnapMode;
        /// <summary>
        /// Format[16]数组的有效长度
        /// </summary>
	    public Int32 nPicFormatCount;
        /// <summary>
        /// (按数值)0：BMP格式，1：JPG格式
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] PictureFormat;
        /// <summary>
        /// Quality[32]数组的有效长度
        /// </summary>
	    public Int32 nPicQualityCount;
        /// <summary>
        /// 按数值
        /// 100：图象质量100%；80:图象质量80%；60:图象质量60%
        /// 50:图象质量50%；30:图象质量30%；10:图象质量10%
        /// char
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public byte[] PictureQuality;
        /// <summary>
        /// 保留
        /// char
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    public byte[] nReserved; 
    } 

    public struct DH_SNAP_ATTR_EN
    {
        /// <summary>
        /// 通道个数
        /// </summary>
	    public Int32 nChannelCount; 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public DH_QUERY_SNAP_INFO[]  stuSnap;
    } 

    
    // 抓图参数结构体
    public struct SNAP_PARAMS
    {
        /// <summary>
        /// 抓图的通道
        /// </summary>
	    public UInt32 Channel;
        /// <summary>
        /// 画质；1~6
        /// </summary>
	    public UInt32 Quality;
        /// <summary>
        /// 画面大小；0：QCIF，1：CIF，2：D1
        /// </summary>
	    public UInt32 ImageSize;
        /// <summary>
        /// 抓图模式；0：表示请求一帧，1：表示定时发送请求，2：表示连续请求
        /// </summary>
	    public Int32 mode;
        /// <summary>
        /// 时间单位秒；若mode=1表示定时发送请求时，此时间有效
        /// </summary>
	    public UInt32 InterSnap;
        /// <summary>
        /// 请求序列号
        /// </summary>
	    public UInt32 CmdSerial;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	    public UInt32[] Reserved;
    }

    #endregion

}