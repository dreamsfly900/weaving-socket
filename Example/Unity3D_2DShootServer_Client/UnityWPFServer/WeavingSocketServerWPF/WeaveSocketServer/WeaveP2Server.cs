using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WeaveBase;

namespace WeaveSocketServer
{
    public class WeaveP2Server : WeaveBaseServer
    {
        public WeaveP2Server()
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public WeaveP2Server(string _loaclip):base(_loaclip)
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.loaclip = _loaclip;
        }
        public WeaveP2Server(WeaveDataTypeEnum weaveDataType):base(weaveDataType)
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.weaveDataType = weaveDataType;
        }
        
    }
  
}
