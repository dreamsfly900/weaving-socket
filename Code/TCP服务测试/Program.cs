using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Weave.Base;
using Weave.Server;

namespace TCP服务测试
{
    class Program
    {
        static DTUServer wudp = new DTUServer();
     
        static void Main(string[] args)
        {
            wudp.weaveReceiveDtuEvent += Wudp_weaveReceiveDtuEvent1;
            //wudp.Send(soc, 0X01, "字符串");
            wudp.start(8989);
            Console.ReadLine();
        }

        private static void Wudp_weaveReceiveDtuEvent1(byte[] data, System.Net.Sockets.Socket soc)
        {
            wudp.send(soc, data);
        }

        private static void Wudp_waveReceiveEvent1(byte command, string data, System.Net.Sockets.Socket soc)
        {
            
        }

        private static void Wudp_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            
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
