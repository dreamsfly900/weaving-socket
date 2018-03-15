using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Base;
using Weave.Server;

namespace TCP服务端测试
{
    class Program
    {
        static WeaveWebServer wudp = new WeaveWebServer();
      //  static WeaveP2Server wudp = new WeaveP2Server(); //这是一般SOCKET
        static void Main(string[] args)
        {
            wudp.Certificate= new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\214495009180717.pfx", "214495009180717");
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;
         //   wudp.weaveReceiveSslEvent += Wudp_weaveReceiveSslEvent; ssl加密的接收事件
            wudp.Start(8181);
            Console.ReadLine();
        }

        private static void Wudp_weaveReceiveSslEvent(byte command, string data, System.Net.Security.SslStream soc)
        {
            wudp.Send(soc, 0x01, "现在我知道你发消息了");
        }

        private static void Wudp_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你走了:");
        }

        private static void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            wudp.Send(soc, 0x01, "现在我知道你发消息了");
            Console.WriteLine("我知道你来了:");
        }

        private static void Wudp_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            wudp.Send(soc, 0x01, "现在我知道你发消息了");
            Console.WriteLine("指令:" + command + ".内容:" + data);

        }
    }
}
