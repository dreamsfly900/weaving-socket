using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Weave.Base
{
    /// <summary>
    /// 连接到服务器的客户端Socket封装对象类
    /// </summary>
    public class WeaveNetWorkItems
    {
        public Socket SocketSession
        {
            get; set;
        }

        [DefaultValue(2048)]
        public int BufferSize
        {
            get; set;
        }

        public byte[] Buffer
        {
            get; set;
        }

        public int State
        {
            get; set;
        }

        public int SendState
        {
            get; set;
        }

        public bool IsPage
        {
            get; set;
        }

        public int ErrorNum
        {
            get; set;
        }

        public byte[] allDataList = new byte[0];
        public byte[] tempDataList = new byte[0];
        public List<byte[]> DataList { get; set; } = new List<byte[]>();

        public EndPoint Ep { get; set; }

        public DateTime Lasttime { get; set; }

        public SslStream Stream { get; set; }
    }

}
