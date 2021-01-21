using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Weave.Base;

namespace Weave.Server
{
   public class WeaveWebServer : WeaveBaseServer
    {
        protected string Handshake = "";
        void init(int port)
        {
            Handshake = "HTTP/1.1 101 Web Socket Protocol Handshake" + Environment.NewLine;
            Handshake += "Upgrade: WebSocket" + Environment.NewLine;
            Handshake += "Connection: Upgrade" + Environment.NewLine;
            Handshake += "Sec-WebSocket-Origin: " + "{0}" + Environment.NewLine;
            Handshake += string.Format("Sec-WebSocket-Location: " + "ws://{0}:" + port + "" + Environment.NewLine, "127.0.0.1");
            Handshake += Environment.NewLine;
        }
        /// <summary>
        /// 没有传递参数，那么默认是new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// </summary>
        public WeaveWebServer()
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
        } 

        /// <summary>
        /// 传递了weaveDataType枚举类型，那么是new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);定义了基类的weaveDataType类型
        /// </summary>
        /// <param name="weaveDataType"></param>
        public WeaveWebServer(WeaveDataTypeEnum weaveDataType) : base(weaveDataType)
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.weaveDataType = weaveDataType;

        }
        public override void Start(int port) {
            init(port);
            base.Start(port); 
        }
        protected override byte[] sendpage(byte command, byte[] text)
        {
            byte[] data = base.sendpage(command, text);
            DataFrame bp = new DataFrame();
            bp.SetByte(data);
            return bp.GetBytes();
        }
        protected override byte[] packageData(byte[] alldata, Socket soc, SslStream Stream, byte[] tempDataList)
        {
            if (alldata.Length > 0)
            {
                DataFrameHeader dfh = null;
                byte[] tempbtyes=null;
                if (tempDataList.Length > 0)
                {
                    tempbtyes = new byte[tempDataList.Length+alldata.Length];
                    Array.Copy(tempDataList, 0, tempbtyes, 0, tempDataList.Length);
                    Array.Copy(alldata, 0, tempbtyes, tempDataList.Length, tempbtyes.Length);
                }
                else
                {
                    tempbtyes = new byte[alldata.Length];
                    Array.Copy(alldata, 0, tempbtyes, 0, tempbtyes.Length);
                }
                byte[] masks = new byte[4];
                int lens = 0;
                int paylen = 0;
                DataFrame df = new DataFrame();
                byte[] tempbtye = df.GetData(tempbtyes, ref masks, ref lens, ref paylen, ref dfh);
                if (dfh == null || dfh.OpCode != 2)
                {
                    tempDataList = tempbtyes;
                    return new byte[0];
                }
                else
                {
                    if (tempbtyes.Length > lens + paylen)
                    {
                        tempDataList = new byte[tempbtyes.Length- (lens + paylen)];
                        Buffer.BlockCopy(tempbtyes, lens+ paylen, tempDataList, 0, tempDataList.Length);

                    } else if (tempbtyes.Length == lens + paylen)
                    {
                        tempDataList = new byte[0];
                    }
                }
                
                alldata = tempbtye;
            }
           return base.packageData(alldata, soc, Stream, tempDataList);
         }
        static string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {

                bytes = sslStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);

                if (messageData.ToString().IndexOf("client_max_window_bits") != -1)
                {
                    break;
                }
            } while (bytes <= 2);

            return messageData.ToString();
        }
       protected override bool Setherd(WeaveNetWorkItems netc, int xintiao = 0)
        {
            //WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            try
            {
                if (xintiao == 1)
                {
                    if (netc.State == 1)
                        return true;
                    return false;
                }
                if (netc.State == 1)
                    return true;
                if (Certificate != null)
                {
                    string httpstr = ReadMessage(netc.Stream);
                    byte[] tempbtye = Encoding.Default.GetBytes(httpstr);
                    tempbtye = ManageHandshake(tempbtye, tempbtye.Length);
                    netc.Stream.Write(tempbtye);
                    netc.Stream.Flush();
                    netc.State = 1;
                    return false;
                }
                else
                {
                    //lb11220:
                    //    while (netc.SocketSession.Available < 200)
                    //    {
                    //        System.Threading.Thread.Sleep(10);
                    //        goto lb11220;
                    //    }
                    if (netc.SocketSession.Available < 200)
                        return false;

                     netc.Buffer = new byte[netc.SocketSession.Available];
                    //if (netc.SocketSession.Available == 0)
                    //    return false;
                    netc.SocketSession.Receive(netc.Buffer);
                    if (Sendhead(netc.SocketSession, netc.Buffer))
                    {
                        netc.Buffer = new byte[0];
                        netc.State = 1;
                        System.Threading.ThreadPool.QueueUserWorkItem(
                  new System.Threading.WaitCallback(UpdateSocketListEventHander),
                     netc.SocketSession);
                       // UpdateSocketListEventHander(netc.SocketSession);
                     // base.weaveUpdateSocketListEvent()
                        // base.weaveUpdateSocketListEvent?.Invoke(netc.SocketSession);
                        return false;


                    }
                    else
                    {
                        try { netc.SocketSession.Close(); } catch { }
                    }
                }

                return false;

                //weaveWorkItemsList.Add(netc);
                //weaveUpdateSocketListEvent?.Invoke(netc.SocketSession);
            }
            catch
            {

                //   try { netc.SocketSession.Close(); } catch { }
                return false;
            }
            finally
            { netc.IsPage = false; }
        }
        bool Sendhead(Socket handler, byte[] tempbtye)
        {
            byte[] aaa = ManageHandshake(tempbtye, tempbtye.Length);
            Sendbyte(handler, aaa);
          //  handler.Send(aaa);
            return true;
        }


        #region websocket编码
        public byte[] ManageHandshake(byte[] receivedDataBuffer, int HandshakeLength)
        {
            string New_Handshake = "";
            New_Handshake = "HTTP/1.1 101 Switching Protocols" + Environment.NewLine;
            New_Handshake += "Upgrade: WebSocket" + Environment.NewLine;
            New_Handshake += "Connection: Upgrade" + Environment.NewLine;
            New_Handshake += "Sec-WebSocket-Accept: {0}" + Environment.NewLine;
            New_Handshake += Environment.NewLine;
            string header = "Sec-WebSocket-Version:";
            byte[] last8Bytes = new byte[8];
            UTF8Encoding decoder = new UTF8Encoding();
            String rawClientHandshake = Encoding.UTF8.GetString(receivedDataBuffer);
            Array.Copy(receivedDataBuffer, HandshakeLength - 8, last8Bytes, 0, 8);
            //现在使用的是比较新的Websocket协议
            if (rawClientHandshake.IndexOf(header) != -1)
            {
                string[] rawClientHandshakeLines = rawClientHandshake.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
                string acceptKey = "";
                foreach (string Line in rawClientHandshakeLines)
                {
                    if (Line.Contains("Sec-WebSocket-Key:"))
                    {
                        acceptKey = ComputeWebSocketHandshakeSecurityHash09(Line.Substring(Line.IndexOf(":") + 2));
                    }
                }
                New_Handshake = string.Format(New_Handshake, acceptKey);
                byte[] newHandshakeText = Encoding.UTF8.GetBytes(New_Handshake);
                return newHandshakeText;
            }
            string ClientHandshake = decoder.GetString(receivedDataBuffer, 0, HandshakeLength - 8);
            string[] ClientHandshakeLines = ClientHandshake.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            // Welcome the new client
            foreach (string Line in ClientHandshakeLines)
            {
                if (Line.Contains("Sec-WebSocket-Key1:"))
                    BuildServerPartialKey(1, Line.Substring(Line.IndexOf(":") + 2));
                if (Line.Contains("Sec-WebSocket-Key2:"))
                    BuildServerPartialKey(2, Line.Substring(Line.IndexOf(":") + 2));
                if (Line.Contains("Origin:"))
                    try
                    {
                        Handshake = string.Format(Handshake, Line.Substring(Line.IndexOf(":") + 2));
                    }
                    catch
                    {
                        Handshake = string.Format(Handshake, "null");
                    }
            }
            // Build the response for the client
            byte[] HandshakeText = Encoding.UTF8.GetBytes(Handshake);
            byte[] serverHandshakeResponse = new byte[HandshakeText.Length + 16];
            byte[] serverKey = BuildServerFullKey(last8Bytes);
            Array.Copy(HandshakeText, serverHandshakeResponse, HandshakeText.Length);
            Array.Copy(serverKey, 0, serverHandshakeResponse, HandshakeText.Length, 16);
            return serverHandshakeResponse;
        }
        private void BuildServerPartialKey(int keyNum, string clientKey)
        {
            string partialServerKey = "";
            byte[] currentKey;
            int spacesNum = 0;
            char[] keyChars = clientKey.ToCharArray();
            foreach (char currentChar in keyChars)
            {
                if (char.IsDigit(currentChar)) partialServerKey += currentChar;
                if (char.IsWhiteSpace(currentChar)) spacesNum++;
            }
            try
            {
                currentKey = BitConverter.GetBytes((int)(long.Parse(partialServerKey) / spacesNum));
                if (BitConverter.IsLittleEndian) Array.Reverse(currentKey);
                if (keyNum == 1) ServerKey1 = currentKey;
                else ServerKey2 = currentKey;
            }
            catch
            {
                if (ServerKey1 != null) Array.Clear(ServerKey1, 0, ServerKey1.Length);
                if (ServerKey2 != null) Array.Clear(ServerKey2, 0, ServerKey2.Length);
            }
        }

        private byte[] ServerKey1;
        private byte[] ServerKey2;
        private byte[] BuildServerFullKey(byte[] last8Bytes)
        {
            byte[] concatenatedKeys = new byte[16];
            Array.Copy(ServerKey1, 0, concatenatedKeys, 0, 4);
            Array.Copy(ServerKey2, 0, concatenatedKeys, 4, 4);
            Array.Copy(last8Bytes, 0, concatenatedKeys, 8, 8);
            // MD5 Hash
            MD5 MD5Service = MD5.Create();
            return MD5Service.ComputeHash(concatenatedKeys);
        }

        public static string ComputeWebSocketHandshakeSecurityHash09(string secWebSocketKey)
        {
            const string MagicKEY = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            string secWebSocketAccept = string.Empty;
            // 1. Combine the request Sec-WebSocket-Key with magic key.
            string ret = secWebSocketKey + MagicKEY;
            // 2. Compute the SHA1 hash
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] sha1Hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ret));
            // 3. Base64 encode the hash
            secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            return secWebSocketAccept;
        }
        #endregion
    }
}
