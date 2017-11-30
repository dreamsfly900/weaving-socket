using Weave.TCPClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Weave.Cloud
{

    /// <summary>
    /// 命令封装对象，有命令名称，IP，端口，client4_10 = new P2Pclient[10, 10, 10, 10];client（List)和Commfun几个字段
    /// </summary>
    public class CommandItem
    {
        byte commName;
        public byte CommName
        {
            get { return commName; }
            set { commName = value; }
        }
        public string Ip
        {
            get;set;
       
        }
        public int Port
        {
            get;set;
        }

        P2Pclient[,,,] client4_10 = new P2Pclient[10, 10, 10, 10];
        public P2Pclient[,,,] Client4_10 {
            get { return client4_10; }
            set { client4_10 = value; }

        }
       // String ip = "";
      //  int port;



       
        String commfun;


        List<P2Pclient> client = new List<P2Pclient>();
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
     
        



    }

}
