using System;
using Weave.Client;

namespace TcpSynClientTest
{
    class Program
    {
          static  void  Main(string[] args)
        {
            test();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
        static int count = 0;
     async static void test()
        {
            Weave.Client.TcpSynClient tcpSynClient = new TcpSynClient(Weave.Client.DataType.bytes, "116.255.252.181", 9903);
            tcpSynClient.Start();
            while (true)
            {
                tcpSynClient.Send(0x01, "asdasd");
                var commdata =  tcpSynClient.Receives(null);
                if (commdata == null)
                    break;
                Console.WriteLine(count++ +":"+System.Text.Encoding.UTF8.GetString(commdata.data));
                System.Threading.Thread.Sleep(10);
            }
            tcpSynClient.Stop();
        }
    }
}
