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
        static Weave.Server.WeaveWebServer wudp = new WeaveWebServer(WeaveDataTypeEnum.Json);

        static P2Pclient ptcp = new P2Pclient(DataType.custom);

        static void Main(string[] args)
        {
            wudp.weaveReceiveBitEvent += Wudp_weaveReceiveBitEvent;
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.WeaveReceiveSslEvent += Wudp_WeaveReceiveSslEvent1;
            wudp.Start(11110);

            Console.ReadLine();

            ptcp.ReceiveServerEventbit += Ptcp_receiveServerEventbit;
            ptcp.Timeoutobjevent += Ptcp_timeoutobjevent;
            ptcp.Start("127.0.0.1", 11110, false);
            Console.ReadLine();
        }

        private static void Wudp_waveReceiveEvent(byte command, string data, Socket soc)
        {
            wudp.Send(soc,command, data);
        }

        private static void Wudp_WeaveReceiveSslEvent1(byte command, string data, System.Net.Security.SslStream soc)
        {
        
        }

        

        private static void Ptcp_timeoutobjevent(P2Pclient p2pobj)
        {
            p2pobj.Restart(false);
        }

        private static void Ptcp_receiveServerEventbit(byte command, byte[] data)
        {
            ptcp.Send(data);
        }

        private static void Wudp_weaveReceiveBitEvent(byte command, byte[] data, Socket soc)
        {
            wudp.Send(soc, data);
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
