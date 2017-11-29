using Weave.TCPClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Weave.Cloud
{
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
