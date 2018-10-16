using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        //    wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            //wudp.Send(soc, 0X01, "字符串");
            wudp.Start(12233);
           // System.Threading.Thread t=new System.Threading.Thread(new System.Threading.ThreadStart())
            Console.ReadLine();
             
        }
        static WeaveTCPCommand wtcpc;
        private static void Wudp_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            if (wtcpc.Getcommand() == command)
                wtcpc.RunBase(data, soc);
        }
        
        private static void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            string txt = "$GPRSUID,000213";

            wudp.Send(soc, System.Text.ASCIIEncoding.ASCII.GetBytes(txt));
            System.Threading.Thread.Sleep(1000 * 60);
            txt = "$GPRSSTATIM,0,24,01,01";
            wudp.Send(soc, System.Text.ASCIIEncoding.ASCII.GetBytes(txt));
        }
       
        private static void Wudp_weaveReceiveBitEvent1(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {

        


            string text = System.Text.ASCIIEncoding.ASCII.GetString(data);

            if (  DateTime.Now.Second >= 50)
            {
                string txt = "$GPRSTIM,"+ DateTime.Now.ToString("yyyyMMddHHmmss");

                wudp.Send(soc, System.Text.ASCIIEncoding.ASCII.GetBytes(txt) );
                return;
            }
            if (text == "$GPRS,OK")
            {
                return;
            }
            else
            {
                Console.WriteLine(text);
            }
           
        }

      
    }
}
