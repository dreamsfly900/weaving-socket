//using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Weave.Base;
using Weave.TCPClient;
namespace ConsoleApplication1
{
    class Program
    {

        static void Main(string[] args)
        {
            
           P2Pclient client = new P2Pclient(false);
             client.receiveServerEvent += Client_receiveServerEvent;//接收服务端消息，也可以不用这么写
            //client.AddListenClass(this);静态方法，麻烦，算了，还是正常写把
            client.timeoutevent += Client_timeoutevent;//连接断开事件
            client.start("127.0.0.1", 8989, true);
            client.SendRoot<String>(0x31, "login", "C/S测试", 0);//刚才忘了一件事情，这个逻辑要求，必须注册一下，才能收到内容。这句话的意思是注册一下。在刚才看到的那个部分。
            Console.ReadKey();
        }

        private static void Client_timeoutevent()
        {
           
        }

        private static void Client_receiveServerEvent(byte command, string text)
        {
            WeaveSession bm= Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(text);//我的传输类型是这样的类
            if (bm.Request == "say")
            {
                Console.WriteLine(bm.Root);
            }


        }
    }
}
