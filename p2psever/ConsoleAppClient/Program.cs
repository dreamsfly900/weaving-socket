using client;
using System;

namespace ConsoleAppClient
{
    class Program
    {
        static void  Main(string[] args)
        {
            P2Pclient client = new P2Pclient(false);
            client.timeoutevent += Client_timeoutevent;
            client.receiveServerEvent += Client_receiveServerEvent;
              client.start("127.0.0.1", 8989, false);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("server link OK:");
            client.send(0x1, "test2017-5-5");
            Console.WriteLine("send:test2017-5-5");
            Console.ReadKey();
        }

        private static void Client_receiveServerEvent(byte command, string text)
        {
          
        }

        private static void Client_timeoutevent()
        {
         
        }
    }
}