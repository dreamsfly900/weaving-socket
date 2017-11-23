using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
namespace WeaveBase
{
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
