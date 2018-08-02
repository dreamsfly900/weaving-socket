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
        static WeaveP2Server wudp = new WeaveP2Server(WeaveDataTypeEnum.custom);
     
        static void Main(string[] args)
        {
            wudp.weaveReceiveBitEvent += Wudp_weaveReceiveBitEvent1;
            //wudp.Send(soc, 0X01, "字符串");
            wudp.Start(8989);
            Console.ReadLine();
        }

        private static void Wudp_weaveReceiveBitEvent1(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            wudp.Send(soc, data);
        }

      
    }
}
