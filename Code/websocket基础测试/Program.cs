using System;
using System.Collections.Generic;
using Weave.Base;

namespace ConsoleApp1
{
    class Program
    {
        static Weave.Server.WeaveWebServer webServer = new Weave.Server.WeaveWebServer(WeaveDataTypeEnum.custom);
        static void Main(string[] args)
        {
            
            webServer.weaveReceiveBitEvent += WebServer_weaveReceiveBitEvent;
            webServer.Start(18181);
            
        }

        private static void WebServer_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            webServer.Send(soc, System.Text.UTF8Encoding.UTF8.GetBytes("asdasdasd"));
        }
    }
     
}
