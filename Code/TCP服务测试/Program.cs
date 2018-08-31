using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Weave.Base;
using Weave.Server;

namespace TCP服务测试
{
    class Program
    {
        static WeaveP2Server wudp = new WeaveP2Server(WeaveDataTypeEnum.custom);
     
        static void Main(string[] args)
        {
            wudp.weaveReceiveBitEvent += Wudp_weaveReceiveBitEvent1;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            //wudp.Send(soc, 0X01, "字符串");
            wudp.Start(12233);
           // System.Threading.Thread t=new System.Threading.Thread(new System.Threading.ThreadStart())
            Console.ReadLine();
        }

        private static void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
             
        }

        private static void Wudp_weaveReceiveBitEvent1(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {

            string text = System.Text.ASCIIEncoding.ASCII.GetString(data);

            if (DateTime.Now.Minute == 0 && DateTime.Now.Second >= 50)
            {
                string txt = "$GPRSTIM,"+ DateTime.Now.ToString("yyyyMMddHHmmss");

                wudp.Send(soc, System.Text.ASCIIEncoding.ASCII.GetBytes(txt) );
                return;
            }
            if (text == "$GPRS,OK")
            {
                return;
            }
           
        }

      
    }
}
