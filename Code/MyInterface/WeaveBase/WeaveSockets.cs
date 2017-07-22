using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
namespace WeaveBase
{
    public class WeaveSockets
    {
        public Socket Sck
        {
            get; set;
        }
        public WeaveScheduling Sch
        {
            get; set;
        }
    }
}
