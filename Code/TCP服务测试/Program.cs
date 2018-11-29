using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Weave.Base;
using Weave.Server;
using Weave.TCPClient;

namespace TCP服务测试
{
    class Program
    {
        static Weave.Server.WeaveWebServer wudp = new WeaveWebServer();

      
        static List<Socket> listsoc = new List<Socket>();
        static List<P2Pclient> listp2p = new List<P2Pclient>();
           static void Main(string[] args)
        {
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;
       
             wudp.Start(12404);
            System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Directory.GetCurrentDirectory()+ "\\config.txt", Encoding.GetEncoding("gb2312"));
         
            while (!sr.EndOfStream)
            {
                Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
             //   Console.WriteLine(sr.ReadLine());
                string str = sr.ReadLine();
                string ip = str.Split('|')[0];
                int port = Convert.ToInt32(str.Split('|')[1]);
          
                Weave.TCPClient.P2Pclient p2pc = new Weave.TCPClient.P2Pclient(DataType.custom);
                listp2p.Add(p2pc);
                p2pc.receiveServerEventbitobj += P2pc_receiveServerEventbitobj;
                p2pc.timeoutobjevent += P2pc_timeoutobjevent;
                p2pc.ErrorMge += P2pc_ErrorMge;
             
                bool ss = p2pc.start(ip, port, 60 * 10, false);
                if (ss)
                {
                    byte[] bbs = strToToHexByte("680407000000");
                    p2pc.Send(bbs);
                    Console.WriteLine("已链接" + ss.ToString());
                }
                Console.WriteLine(ip+":"+port);
            }
            sr.Close();
            //wudp.Send();
            // System.Threading.Thread t=new System.Threading.Thread(new System.Threading.ThreadStart())
            //while (true)
            //{
            //  String str=  Console.ReadLine();

            //        byte[] bb = strToToHexByte(str);
            //        bool v = p2pc.Send( bb);
            //        Console.WriteLine("已发送：" + v.ToString());

            //}
            Console.ReadLine();
        }

        private static void P2pc_ErrorMge(int type, string error)
        {
            Console.WriteLine( error);
        }

        private static void P2pc_receiveServerEventbitobj(byte command, byte[] data, P2Pclient p2p)
        {
            Console.WriteLine("收到数据：");
            String str = byteToHexStr(data);
            
            try
            {
                if (data[0] == 0x68)
                {
                    if (data[1] >= 4)
                    //if (data[6] == 0x01 || data[6] == 0x03 || data[6] == 0x14)
                    {
                        int len = listsoc.Count;
                        if (len > 0)
                        {
                            Socket[] socs = new Socket[len];
                            listsoc.CopyTo(0, socs, 0, len);
                            foreach (Socket soc in socs)
                            {
                                try
                                {
                                    if (soc != null)
                                    {
                                        //data
                                        WeaveSession wsee = new WeaveSession();
                                        wsee.Token = "server";
                                        wsee.Request = p2p.PORT.ToString();
                                        wsee.SetRoot<string>(str);
                                        wudp.Send(soc, data[6], wsee.Getjson());
                                        Console.WriteLine("转发成功");
                                    }
                                }
                                catch (Exception e) { Console.WriteLine(e.Message); }
                            }
                        }
                    }
                }
            }
            catch (Exception ee) { Console.WriteLine(ee.Message); }
            Console.WriteLine(str);
        }

        private static void P2pc_timeoutobjevent(P2Pclient p2pobj)
        {
            lb1111:
            if (!p2pobj.Restart(false))
            {
                goto lb1111;
            }
        }

        private static void Wudp_waveReceiveEvent(byte command, string data, Socket soc)
        {
           
        }

        private static void Wudp_weaveDeleteSocketListEvent(Socket soc)
        {
            try
            {
                listsoc.Remove(soc);
            }
            catch { }
        }

      

        

       
        /// <summary> 
        /// 字符串转16进制字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private static void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            try
            {
                listsoc.Add(soc);
                Console.WriteLine("websocket");
            }
            catch { }
        }
        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2")+"";
                }
            }
            return returnStr;
        }
       
      
    }
}
