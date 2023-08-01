using System;
using Weave.Base;
using Weave.Client;
using Weave.Server;

namespace TcpSynClientTest
{
    class Program
    {
        static Weave.Server.WeaveP2Server wudp = new WeaveP2Server(WeaveDataTypeEnum.Bytes);//这是webscoekt服务端

        static void  Main(string[] args)
        {
            wudp.weaveReceiveBitEvent += Wudp_weaveReceiveBitEvent;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;
            wudp.Start(11110);
            test();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        private static void Wudp_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
             
        }

        private static void Wudp_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            
        }

        static int count = 0;
     async static void test()
        {
            Weave.Client.TcpSynClient tcpSynClient = new TcpSynClient(Weave.Client.DataType.bytes, "127.0.0.1", port: 11110);
            tcpSynClient.Start();
           // while (true)
            {
                tcpSynClient.Send(0x01, "asdasd");
                var commdata =  tcpSynClient.Receives(null);
                //if (commdata == null)
                //    break;
                Console.WriteLine(count++ +":"+System.Text.Encoding.UTF8.GetString(commdata.data));
                System.Threading.Thread.Sleep(10);
            }
            tcpSynClient.Stop();
        }
    }
}
