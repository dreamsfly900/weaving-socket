using smsForCsharp.CRC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Base;
using Weave.Server;

namespace TCP服务端测试
{
    class Program
    {

        static List<nqdtu> LISTDTU = new List<nqdtu>();
        static DTUServer wudp = new DTUServer();
        static WeaveWebServer wudpweb = new WeaveWebServer(); //这是一般SOCKET
        static void Main(string[] args)
        {
          
            
           // wudp.Certificate= new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\214495009180717.pfx", "214495009180717");
            //wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;
            wudp.weaveReceiveDtuEvent += Wudp_weaveReceiveDtuEvent;
            wudp.start(60001);
          //  wudpweb.weaveUpdateSocketListEvent += Wudpweb_weaveUpdateSocketListEvent;
            //wudpweb.weaveDeleteSocketListEvent += Wudpweb_weaveDeleteSocketListEvent;
            wudpweb.waveReceiveEvent += Wudpweb_waveReceiveEvent;
            wudpweb.Start(18181);
            //   wudp.weaveReceiveSslEvent += Wudp_weaveReceiveSslEvent; ssl加密的接收事件
            //wudp.Start(8181);

            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(gg),null);
            Console.ReadLine();
        }

        private static void Wudpweb_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
           
        }

        private static void Wudpweb_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
           
        }

        private static void Wudpweb_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {

            WeaveSession wsion = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(data);

            try
            {

                foreach (nqdtu nqd in LISTDTU)
                {
                    if (wsion.Root == nqd.ID)
                    {
                        if (nqd.state)
                        {
                            if (wsion.Request == "zheng")
                            {
                                Console.WriteLine("设备连接:" + nqd.ID+":正向打开");
                                openzheng(nqd);
                                return;
                            }
                            else if (wsion.Request == "fan")
                            {
                                openfan(nqd);
                                return;
                            }
                            else if (wsion.Request == "closejuan")
                            {
                                closejuanlian(nqd);
                                return;
                            }
                            if (wsion.Request == "open")
                            {
                                nqd.soc.Send(dtccommand.OPEN(Convert.ToByte(wsion.Parameter)));
                                return;
                            }
                            if (wsion.Request == "close")
                            {
                                nqd.soc.Send(dtccommand.CLOSE(Convert.ToByte(wsion.Parameter)));
                                return;
                            }
                        }
                        else
                        {
                            wsion.Root = nqd.state? "设备正常" : "设备不在线";
                            wsion.Request = "cha";
                            String strs = Newtonsoft.Json.JsonConvert.SerializeObject(wsion);
                            wudpweb.Send(soc, 0x01, strs);
                            return;
                        }
                        if (wsion.Request == "cha")
                        {
                            wsion.Root = nqd.state ? "设备正常" : "设备不在线";
                            String strt = Newtonsoft.Json.JsonConvert.SerializeObject(wsion);
                            wudpweb.Send(soc, 0x01, strt);
                            return;

                        }
                        return;
                    }
                }


                wsion.Root = "设备不在线";
                wsion.Request = "chas";
                String str = Newtonsoft.Json.JsonConvert.SerializeObject(wsion);
                wudpweb.Send(soc, 0x01, str);
            }
            catch
            { }

        }
 
        static void openzheng(nqdtu nqd)
        {
            nqd.soc.Send(dtccommand.CLOSE(1)) ;
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.CLOSE(3));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.OPEN(0));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.OPEN(2));
        }
        static void openfan(nqdtu nqd)
        {
            nqd.soc.Send(dtccommand.CLOSE(0));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.CLOSE(2));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.OPEN(1));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.OPEN(3));
        }
        static void closejuanlian(nqdtu nqd)
        {
            nqd.soc.Send(dtccommand.CLOSE(0));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.CLOSE(1));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.CLOSE(2));
            System.Threading.Thread.Sleep(100);
            nqd.soc.Send(dtccommand.CLOSE(3));
        }
        private static void Wudp_weaveReceiveDtuEvent(byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
                string text = System.Text.ASCIIEncoding.Default.GetString(data);
                if (text == ":138xxxxxxxx.")
                { return; }
                if (text.IndexOf("id:") >= 0)
                {
                    string id = text.Split(':')[1];
                    foreach (nqdtu nqd in LISTDTU)
                    {
                        if (nqd.ID == id)
                        {
                            nqd.soc = soc;
                            nqd.state = true;
                            return;
                        }
                    }
                    nqdtu ndt = new nqdtu();
                    ndt.ID = id;
                    ndt.soc = soc;
                    ndt.state = true;
                    LISTDTU.Add(ndt);

                }
            }
            catch { }
        }

        private static void Wudp_weaveReceiveSslEvent(byte command, string data, System.Net.Security.SslStream soc)
        {
           // wudp.Send(soc, 0x01, "现在我知道你发消息了");
        }

        private static void Wudp_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            try
            {
                foreach (nqdtu nqd in LISTDTU)
                {
                    if (nqd.soc == soc)
                    {

                        nqd.state = false;
                        return;
                    }
                }
            }
            catch { }

            // Console.WriteLine("我知道你走了:");
        }

        private static void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            //wudp.Send(soc, 0x01, "现在我知道你发消息了");
            Console.WriteLine("设备连接:"+DateTime.Now.ToString());
        }

        private static void Wudp_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
           // wudp.Send(soc, 0x01, "现在我知道你发消息了");
          //  Console.WriteLine("指令:" + command + ".内容:" + data);

        }
    }
    public class dtccommand
    {
        /// <summary>
        /// 从零开始
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] OPEN(byte num)
        {
            byte[] one = new byte[] { 0xFE, 0x05, 0x00, num, 0xFF, 0x00 };
            byte[] BB = CRC.CRCCalc(one);
            byte[] temp = new byte[one.Length+2];
            Array.Copy(one, 0, temp, 0, one.Length);
            Array.Copy(BB, 0, temp, one.Length,2);
            return temp;
        }
        public static byte[] CLOSE(byte num)
        {
            byte[] one = new byte[] { 0xFE, 0x05, 0x00, num, 0x00, 0x00 };
            byte[] BB = CRC.CRCCalc(one);
            byte[] temp = new byte[one.Length + 2];
            Array.Copy(one, 0, temp, 0, one.Length);
            Array.Copy(BB, 0, temp, one.Length, 2);
            return temp;
        }
        public static byte[] one = new byte[] { 0xFE, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x98, 0x35 };
        public static byte[] two = new byte[] { 0xFE, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xC9, 0xF5 };//FE 05 00 01 FF 00 C9 F5
        public static byte[] three = new byte[] { 0xFE, 0x05, 0x00, 0x02, 0xFF, 0x00, 0x39, 0xF5 };
        public static byte[] four = new byte[] { 0xFE, 0x05, 0x00, 0x03, 0xFF, 0x00, 0x68, 0x35 };//FE 05 00 03 FF 00 68 35
        public static byte[] five = new byte[] { 0xFE, 0x05, 0x00, 0x04, 0xFF, 0x00, 0xD9, 0xF4 };
        public static byte[] six = new byte[] { 0xFE, 0x05, 0x00, 0x05, 0xFF, 0x00, 0x88, 0x34 };

        public static byte[] closeone = new byte[] { 0xFE, 0x05, 0x00, 0x00, 0x00, 0x00, 0xD9, 0xC5 };//FE 05 00 00 00 00 D9 C5
        public static byte[] closetwo = new byte[] { 0xFE, 0x05, 0x00, 0x01, 0x00, 0x00, 0x88, 0x05 };//88 05
        public static byte[] closethree = new byte[] { 0xFE, 0x05, 0x00, 0x02, 0x00, 0x00, 0x78, 0x05 };//78 05
        public static byte[] closefour = new byte[] { 0xFE, 0x05, 0x00, 0x03, 0x00, 0x00, 0x29, 0xC5 };//29 C5 
        public static byte[] closefive = new byte[] { 0xFE, 0x05, 0x00, 0x04, 0x00, 0x00, 0x98, 0x04 };// 98 04
        public static byte[] closesix = new byte[] { 0xFE, 0x05, 0x00, 0x05, 0x00, 0x00, 0xC9, 0xC4 };// 98 04

        public static byte[] allclose = new byte[] { 0xFE, 0x0F, 0x00, 0x00, 0x00, 0x08, 0x01, 0x00, 0xB1, 0x91 };//C9 C4

    }
    public class nqdtu
    {
       public System.Net.Sockets.Socket soc;
        public string ID;
        public bool state=false;
    }

   
}
