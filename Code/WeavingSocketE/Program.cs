using System.Net;
using System.Net.Sockets;
using Weave.Server;

namespace WeavingSocketE
{
    internal class Program
    {
       static WeaveP2Server weaveP2Server=new WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        static Weave.TCPClient.P2Pclient tCPClient = new Weave.TCPClient.P2Pclient(Weave.TCPClient.DataType.bytes);
        static void Main(string[] args)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("WeavingSocket 物联网通信架构");
            weaveP2Server.weaveReceiveBitEvent += WeaveP2Server_weaveReceiveBitEvent;
            weaveP2Server.weaveUpdateSocketListEvent += WeaveP2Server_weaveUpdateSocketListEvent;
            weaveP2Server.weaveDeleteSocketListEvent += WeaveP2Server_weaveDeleteSocketListEvent;
            tCPClient.ReceiveServerEventbit += TCPClient_ReceiveServerEventbit;
            if (args.Length > 0)
            {
                if (args[0] == "v")
                {
                    Console.WriteLine("v:2.0.22");
                }
                if (args[0] == "server")
                {
                    Console.WriteLine("监听端口12345");
                    weaveP2Server.Start(12345);
                }
                if (args[0] == "client")
                {
                    Console.WriteLine("连接端口12345");
                  
                    tCPClient.Start("127.0.0.1", 12345, false);

                    tCPClient.Stop();
                }
                if (args[0] == "clientsend")
                {
                    Console.WriteLine("连接端口12345");
                    tCPClient.Start("127.0.0.1", 12345, false);
                    DateTime stattime2 = DateTime.Now;
                    for (var i = 0; i < 50000; i++)
                        tCPClient.Send(0x01, new byte[200]);
                    double shijian = (DateTime.Now - stattime2).TotalMilliseconds;

                    Console.WriteLine("发送50000次200字符内容已完成");
                    Console.WriteLine("耗时：" + shijian + "毫秒");
                    tCPClient.Stop();
                }
            }
        }

        private static void TCPClient_ReceiveServerEventbit(byte command, byte[] data)
        {
             
        }

        static int count=0;
        private static void WeaveP2Server_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("用户离线");
            Console.WriteLine(((soc.RemoteEndPoint.ToString())));
        }

        private static void WeaveP2Server_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("用户上线");
            Console.WriteLine(((soc.RemoteEndPoint.ToString())));
        }
        static DateTime stattime=DateTime.Now;
        private static void WeaveP2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            count++;
            if (count == 1)
            {
                stattime = DateTime.Now;
            }
            if (count >= 50000)
            {
                double shijian = (DateTime.Now - stattime).TotalMilliseconds;
                Console.WriteLine("接收耗时：" + shijian + "毫秒");
            }

        }
    }
}