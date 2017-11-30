using Weave.TCPClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Weave.Cloud
{
    /// <summary>
    /// 网关单个Item类，有IP,端口，P2Pclient，Num,Token几个属性
    /// </summary>
    public class WayItem
    {
        string _Token;
        int num;
        public string Ip
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
            }
        }
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }
        public P2Pclient Client
        {
            get
            {
                return client;
            }
            set
            {
                client = value;
            }
        }
        public int Num
        {
            get
            {
                return num;
            }
            set
            {
                num = value;
            }
        }
        public string Token
        {
            get
            {
                return _Token;
            }
            set
            {
                _Token = value;
            }
        }
        String ip = "";
        int port;
        P2Pclient client;
    }


}
