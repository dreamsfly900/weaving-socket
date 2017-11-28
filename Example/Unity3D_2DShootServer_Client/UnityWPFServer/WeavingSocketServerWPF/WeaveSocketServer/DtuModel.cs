using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WeaveBase;
namespace WeaveSocketServer
{
        public  class DtuModel
        {
            byte[] data;
            Socket soc;
            public byte[] Data
            {
                get
                {
                    return data;
                }
                set
                {
                    data = value;
                }
            }
            public Socket Soc
            {
                get
                {
                    return soc;
                }
                set
                {
                    soc = value;
                }
            }
        }
}