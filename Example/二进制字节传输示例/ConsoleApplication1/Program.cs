using System;
using Weave.TCPClient;

namespace ConsoleApplication1
{
    class Program
    {
        //这个客户端 只要遵守协议规则，可以参考文档中 二进制协议解释，你可以用各种语言，各种设备完成。
        static P2Pclient p2pc = new P2Pclient(DataType.bytes);
        static void Main(string[] args)
        {

            p2pc.receiveServerEventbit += P2pc_receiveServerEventbit;
            p2pc.start("127.0.0.1", 8989, false);
            //随便发一个内容，前两BIT位，我认为是后台对应的方法。后三位是数据值
            byte[] b = new byte[] {0x00,0x01,0x02,0xff,0x99 };
            p2pc.send(0x01, b);
            Console.ReadKey();
        }

        private static void P2pc_receiveServerEventbit(byte command, byte[] data)
        {
            //接收到的数据打印出来。
            foreach(byte b in data)
                Console.Write(b);

            Console.WriteLine("");
        }
    }
}
