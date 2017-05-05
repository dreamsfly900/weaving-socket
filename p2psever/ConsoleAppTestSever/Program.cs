using P2P;
using System;

namespace ConsoleAppTestSever
{
    class Program
    {
        static void Main(string[] args)
        {
            p2psever server = new p2psever();
            server.receiveevent += Server_receiveevent;
            server.start(8989);
             
           
            Console.WriteLine("8989listen:");
            Console.ReadKey();
        }

        private static void Server_receiveevent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            Console.WriteLine(data);
        }
    }
}