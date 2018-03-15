using Fleck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
namespace Weave.Base
{
    /// <summary>
    /// 连接到服务器的客户端Socket封装对象类
    /// </summary>
    public class WeaveNetWorkItems
    {
        SslStream _stream;
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
        List<byte[]> _DataList = new List<byte[]>();
        
        public bool IsPage
        {
            get; set;
        }
        public int ErrorNum
        {
            get; set;
        }

        public List<byte[]> DataList
        {
            get
            {
                return _DataList;
            }

            set
            {
                _DataList = value;
            }
        }

        public EndPoint Ep { get; set; }
        public DateTime Lasttime { get; set; }

        public SslStream Stream
        {
            get
            {
                return _stream;
            }

            set
            {
                _stream = value;
            }
        }
    }
    public class WeaveUdpWorkItems
    {
        public int Port
        {
            get;set;
        }
     
        public System.Net.IPEndPoint Iep
        {
            get;set;
        }
        public System.Net.IPEndPoint Localiep
        {
            get;set;
        }
        public Socket SocketSession
        {
            get;set;
        }
        public DateTime Timeout
        {
            get;set;
        }
    }
}
