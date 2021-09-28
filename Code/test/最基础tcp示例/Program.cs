using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Base;
using Weave.Server;

namespace 最基础tcp示例
{
    class Program
    {

        static WeaveWebServer wudp3 = new WeaveWebServer(WeaveDataTypeEnum.custom);
        static void Main(string[] args)
        {
           
            wudp3.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent1;
            wudp3.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent1;
            wudp3.weaveReceiveBitEvent += Wudp2_weaveReceiveBitEvent;
            wudp3.resttime = 0;
          //  wudp3.waveReceiveEvent += Wudp3_waveReceiveEvent;
            wudp3.Start(9903);
            

            Console.ReadLine();
        }

   
        private static void Wudp2_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            //wudp3.Send(soc, data);
          //  wudp3.Send(soc, 0x01, "{\"cmd\":\"plays\", \"data\":\"ZWD0001, Zqndmls0009, Zqndmls0001\"}");
          //  wudp3.Send(soc, 0x01, new byte[10]);
            Console.WriteLine( System.Text.Encoding.UTF8.GetString(data) );
        }

        private static void Wudp_weaveUpdateSocketListEvent1(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你来了:");
        }

        private static void Wudp_weaveDeleteSocketListEvent1(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你走了:");
        }

        

    }
}
