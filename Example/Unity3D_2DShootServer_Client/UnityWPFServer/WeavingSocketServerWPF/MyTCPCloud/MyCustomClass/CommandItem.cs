using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaveClient;

namespace MyTCPCloud
{
    public class CommandItem
    {
        byte commName;
        public byte CommName
        {
            get { return commName; }
            set { commName = value; }
        }
        String commfun;
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
        public List<P2Pclient> Client
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
        public string Commfun
        {
            get
            {
                return commfun;
            }
            set
            {
                commfun = value;
            }
        }
        String ip = "";
        int port;
        List<P2Pclient> client = new List<P2Pclient>();
    }

  
}




namespace MyTCPCloud
{
    public class GateWayCommandItem
    {
        byte commName;
        public byte CommName
        {
            get { return commName; }
            set { commName = value; }
        }
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

        P2Pclient[,,,] client = new P2Pclient[10, 10, 10, 10];
        public P2Pclient[,,,] Client { get { return client; } set { client = value; } }
        String ip = "";
        int port;
    }


}
