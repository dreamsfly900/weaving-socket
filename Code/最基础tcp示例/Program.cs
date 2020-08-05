using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Base;
using Weave.Server;

namespace 最基础tcp示例
{
    class Program
    {
        static WeaveP2Server wudp = new WeaveP2Server(WeaveDataTypeEnum.custom);//自定义类型，最普通的类型，可以用TCP测试工具发送消息
        static WeaveP2Server wudp2 = new WeaveP2Server(WeaveDataTypeEnum.Bytes);//有格式的Bytes类型，发送和接收内容格式为内置的Byte[]内容。
        static WeaveP2Server wudp3 = new WeaveP2Server(WeaveDataTypeEnum.custom);//有格式的json，发送内容和接收内容为string
        static void Main(string[] args)
        {
            wudp.weaveReceiveBitEvent += Wudp_weaveReceiveBitEvent;
            wudp3.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent1;
            wudp3.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent1;
            wudp3.weaveReceiveBitEvent += Wudp2_weaveReceiveBitEvent;
            wudp3.waveReceiveEvent += Wudp3_waveReceiveEvent;
            wudp3.Start(8989);
            

            Console.ReadLine();
        }

        private static void Wudp3_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            wudp3.Send(soc, 0x01, "现在我知道你发消息了");
            Console.WriteLine(data);
        }

        private static void Wudp2_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            wudp3.Send(soc, 0x01, "现在我知道你发消息了");
          //  wudp3.Send(soc, 0x01, new byte[10]);
            Console.WriteLine( System.Text.Encoding.UTF8.GetString(data) );
        }

        private static void Wudp_weaveUpdateSocketListEvent1(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你来了:");
        }

        private static void Wudp_weaveDeleteSocketListEvent1(System.Net.Sockets.Socket soc)
        {
            Console.WriteLine("我知道你走了:");
        }

        private static void Wudp_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            wudp3.Send(soc, 0x01, "现在我知道你发消息了");
            wudp3.Send(soc, 0x01, new byte[10]);
            Console.WriteLine("指令:" + command + ".内容:" + data);
        }

    }
}
