using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaveBase;

namespace TCP服务端测试
{
    class Program
    {
        static WeaveP2Server wudp = new WeaveP2Server();
        static void Main(string[] args)
        {
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;
            wudp.Start(8989);
            Console.ReadLine();
        }

        private static void Wudp_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你来了:");
        }

        private static void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你走了:");
        }

        private static void Wudp_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            wudp.Send(soc, 0x01, "现在我知道你发消息了");
            Console.WriteLine("指令:" + command + ".内容:" + data);

        }
    }
}
