using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Weave.Server;

namespace TCP服务测试
{
    class Program
    {
        static WeaveWebServer wudp = new WeaveWebServer();
        static void Main(string[] args)
        {
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.Start(8989);
            Console.ReadLine();
        }

        private static void Wudp_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            
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
