using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Base;
using Weave.Server;

namespace UDP服务端代码测试
{
    class Program
    {
        static WeaveUDPServer wudp = new WeaveUDPServer();
        static void Main(string[] args)
        {
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent1;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            wudp.Start(8989);
            Console.ReadLine();
        }

        private static void Wudp_weaveUpdateSocketListEvent(System.Net.EndPoint ep)
        {
            Console.WriteLine("我知道你来了:");
        }

        private static void Wudp_weaveDeleteSocketListEvent(System.Net.EndPoint ep)
        {
            Console.WriteLine("我知道你走了:");
        }

        private static void Wudp_waveReceiveEvent1(byte command, string data, System.Net.EndPoint ep)
        {
            wudp.Send(ep, 0x01, "现在我知道你发消息了");
            Console.WriteLine("指令:"+ command +".内容:"+ data);
        } 
    }
}
