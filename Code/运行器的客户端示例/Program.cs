using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.TCPClient;

namespace 运行器的客户端示例
{
    class Program
    {
        static void Main(string[] args)
        {
            P2Pclient p2pc = new P2Pclient(false);
            p2pc.ReceiveServerEvent += P2pc_receiveServerEvent;//接收数据事件

            p2pc.Timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
            p2pc.Start("127.0.0.1", 8989, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
            p2pc.Tokan = "123";
            p2pc.SendRoot<int>(0x01, "login", 99987, 0);//调用服务端0x01协议的login方法
            System.Threading.Thread.Sleep(5);

        }

        private static void P2pc_timeoutevent()
        {
            
        }
        /// <summary>
        /// 这是接收到数据的方法
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        private static void P2pc_receiveServerEvent(byte command, string text)
        {

        }
    }
}
