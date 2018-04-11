using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Weave.Server;

namespace TCP服务测试
{
    class Program
    {
        static DTUServer wudp = new DTUServer();
        static void Main(string[] args)
        {
            wudp.weaveReceiveDtuEvent += Wudp_weaveReceiveDtuEvent;
            wudp.start(8989);
            Console.ReadLine();
        }

        private static void Wudp_weaveReceiveDtuEvent(byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
               
            }
            catch
            { }
        }
    }
}
