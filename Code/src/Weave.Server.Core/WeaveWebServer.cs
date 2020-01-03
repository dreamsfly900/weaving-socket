using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Weave.Base;
using Weave.Base.Interface;

namespace Weave.Server
{
    /// <summary>
    /// 继承自IWeaveTcpBase接口的HTTP Web服务器
    /// </summary>
    public class WeaveWebServer : IWeaveTcpBase
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public List<WeaveNetWorkItems> weaveWorkItemsList = new List<WeaveNetWorkItems>();
        public event WaveReceiveEventEvent waveReceiveEvent;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
        public event WeaveReceiveBitEvent weaveReceiveBitEvent;

        public event WeaveReceiveSslEvent WeaveReceiveSslEvent;
        readonly WeaveDataTypeEnum DT = WeaveDataTypeEnum.Json;
        public X509Certificate2 Certificate { get; set; }
        public SslProtocols EnabledSslProtocols { get; set; }
        public byte defaultCommand = 0x0;

        public WeaveWebServer(WeaveDataTypeEnum _DT)
        {
            DT = _DT;
        }

        public WeaveWebServer()
        {
        }

        public int Port { get; set; }

        public void Start(int port)
        {
            callsend = new AsyncCallback(SendDataEnd);
            Port = port;
            if (DT == WeaveDataTypeEnum.Json && waveReceiveEvent == null)
                throw new Exception("没有注册receiveevent事件");
            if (DT == WeaveDataTypeEnum.Bytes && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            if (DT == WeaveDataTypeEnum.custom && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            string New_Handshake = "";
            //Switching Protocols
            New_Handshake = "HTTP/1.1 101 Switching Protocols" + Environment.NewLine;
            New_Handshake += "Upgrade: WebSocket" + Environment.NewLine;
            New_Handshake += "Connection: Upgrade" + Environment.NewLine;
            New_Handshake += "Sec-WebSocket-Accept: {0}" + Environment.NewLine;
            New_Handshake += Environment.NewLine;
            Handshake = "HTTP/1.1 101 Web Socket Protocol Handshake" + Environment.NewLine;
            Handshake += "Upgrade: WebSocket" + Environment.NewLine;
            Handshake += "Connection: Upgrade" + Environment.NewLine;
            Handshake += "Sec-WebSocket-Origin: " + "{0}" + Environment.NewLine;
            Handshake += string.Format("Sec-WebSocket-Location: " + "ws://{0}:" + port + "" + Environment.NewLine, "127.0.0.1");
            Handshake += Environment.NewLine;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(localEndPoint);
            socket.Listen(1000000);
            Thread t = new Thread(new ParameterizedThreadStart(Accept));
            t.Start();
            Thread t1 = new Thread(new ParameterizedThreadStart(Receive));
            t1.Start();
            Thread t3 = new Thread(new ParameterizedThreadStart(KeepAliveHander));
            t3.Start();
            Thread t4 = new Thread(new ParameterizedThreadStart(Receiveconn));
            t4.Start();
        }

        public int GetNetworkItemCount()
        {
            return weaveWorkItemsList.Count;
        }

        public void KeepAliveHander(object obj)
        {
            while (true)
            {
                byte[] xintiao = new byte[1];
                try
                {
                    WeaveNetWorkItems[] weaveWorkItems = new WeaveNetWorkItems[weaveWorkItemsList.Count];
                    weaveWorkItemsList.CopyTo(weaveWorkItems);
                    foreach (WeaveNetWorkItems workItem in weaveWorkItems)
                    {
                        try
                        {
                            if (workItem == null)
                                continue;
                            if (workItem.State != 0)
                            {
                                var ok = false;
                                if (DT == WeaveDataTypeEnum.custom)
                                {
                                    xintiao[0] = 0x99;
                                    ok = Send(workItem.SocketSession, xintiao);
                                }
                                else {
                                    if(Certificate==null)
                                    ok = Send(workItem.SocketSession, 0x99, "000");
                                    else
                                     ok = Send(workItem.Stream, 0x99, "000");
                                } 
                                if (!ok)
                                {
                                    workItem.ErrorNum += 1;
                                    if (workItem.ErrorNum > 3)
                                    {
                                        ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteSocketListHander), workItem.SocketSession);

                                        weaveWorkItemsList.Remove(workItem);
                                    }
                                }
                                else
                                {
                                    workItem.ErrorNum = 0;
                                }
                            }
                        }
                        catch
                        {
                            workItem.ErrorNum += 1;
                            if (workItem.ErrorNum > 3)
                            {
                                ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteSocketListHander), workItem.SocketSession);

                                weaveWorkItemsList.Remove(workItem);
                            }
                        }
                    }
                    Thread.Sleep(8000);
                }
                catch { }
            }
        }

        public int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret = ret + (item << i);
                i = i + 8;
            }
            return ret;
        }

        public byte[] ConvertToByteList(int v)
        {
            List<byte> ret = new List<byte>();
            int value = v;
            while (value != 0)
            {
                ret.Add((byte)value);
                value = value >> 8;
            }
            byte[] bb = new byte[ret.Count];
            ret.CopyTo(bb);
            return bb;
        }

        private byte[] AnalyticData(byte[] recBytes, int recByteLength, ref byte[] masks, ref int lens, ref int payload_len)
        {
            lens = 0;
            if (recByteLength < 2) { return new byte[0]; }
            bool fin = (recBytes[0] & 0x80) == 0x80; // 1bit，1表示最后一帧  
            if (!fin)
            {
                return new byte[0];// 超过一帧暂不处理 
            }
            bool mask_flag = (recBytes[1] & 0x80) == 0x80; // 是否包含掩码  
            if (!mask_flag)
            {
                return new byte[0];// 不包含掩码的暂不处理
            }
            payload_len = recBytes[1] & 0x7F; // 数据长度  
            byte[] payload_data;
            if (payload_len == 126)
            {
                Array.Copy(recBytes, 4, masks, 0, 4);
                payload_len = (ushort)(recBytes[2] << 8 | recBytes[3]);
                lens = 8;
                payload_data = new byte[payload_len];
                Array.Copy(recBytes, 8, payload_data, 0, payload_len);
            }
            else if (payload_len == 127)
            {
                Array.Copy(recBytes, 10, masks, 0, 4);
                byte[] uInt64Bytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    uInt64Bytes[i] = recBytes[9 - i];
                }
                ulong len = BitConverter.ToUInt64(uInt64Bytes, 0);
                lens = 14;
                payload_data = new byte[len];
                for (ulong i = 0; i < len; i++)
                {
                    payload_data[i] = recBytes[i + 14];
                }
            }
            else
            {
                lens = 6;
                Array.Copy(recBytes, 2, masks, 0, 4);
                payload_data = new byte[payload_len];
                Array.Copy(recBytes, 6, payload_data, 0, payload_len);
            }
            for (var i = 0; i < payload_len; i++)
            {
                payload_data[i] = (byte)(payload_data[i] ^ masks[i % 4]);
            }
            return (payload_data);
        }

        private void DeleteSocketListHander(object state)
        {
            weaveDeleteSocketListEvent?.Invoke(state as Socket);
            try { (state as Socket).Close();
                (state as Socket).Dispose();
            }
            catch { }
        }

        private void UpdateSocketListHander(object state)
        {
            weaveUpdateSocketListEvent?.Invoke(state as Socket);
        }

        void ReceiveToEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            waveReceiveEvent?.Invoke(me.Command, me.Data, me.Soc);
        }

        void ReceiveToEventHanderssl(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            WeaveReceiveSslEvent?.Invoke(me.Command, me.Data, me.Ssl);
        }

        void ReceiveToBitEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            weaveReceiveBitEvent?.Invoke(me.Command, me.Databit, me.Soc);
        }

        bool Sendhead(Socket handler, byte[] tempbtye)
        {
            byte[] aaa = ManageHandshake(tempbtye, tempbtye.Length);
            handler.Send(aaa);
            return true;
        }

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

        private string Handshake;
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

        private void ReadCallbackssl(object ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar;
            SslStream stream = netc.Stream;
            byte[] buffer = new byte[20480];
            StringBuilder messageData = new StringBuilder();
            int byteCount = -1;
            List<byte> listb = new List<byte>();
            do
            {
                byteCount = stream.Read(buffer, 0, buffer.Length);

                listb.AddRange(buffer.Take(byteCount));

            } while (byteCount <= 2);

            // netc.DataList.Add(listb.ToArray());
            netc.Buffer = listb.ToArray();
            PackageData(netc);
            netc.State = 1;
        }

        private void ReadCallback3(object ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar;
           
            try
            {
                netc.SocketSession.Receive(netc.Buffer = new byte[netc.SocketSession.Available]);
                byte[] tempbtye = new byte[netc.Buffer.Length];
                Array.Copy(netc.Buffer, 0, tempbtye, 0, tempbtye.Length);
               // netc.DataList.Add(tempbtye);
                PackageData(netc);
            }
            catch
            {
            }
        }

        public bool Send(int index, byte command, string text)
        {
            try
            {
                Socket soc = weaveWorkItemsList[index].SocketSession;
                byte[] sendb = Encoding.UTF8.GetBytes(text);
                byte[] lens = Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.SetByte(b);
                soc.Send(bp.GetBytes());
            }
            catch { return false; }
            return true;
        }

        public IPAddress GetLocalmachineIPAddress()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ip in ipEntry.AddressList)
            {
                //IPV4
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }
            return ipEntry.AddressList[0];
        }

        public bool Send(Socket soc, byte command, string text)
        {
            try
            {
                byte[] sendb = Encoding.UTF8.GetBytes(text);
                byte[] lens = Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.SetByte(b);
                if (Certificate != null)
                {

                    var queryResults = from item in weaveWorkItemsList
                                       where item.SocketSession == soc
                                       select item;
                    foreach (var ssl in queryResults)
                        ssl.Stream.Write(bp.GetBytes());
                }
                else
                   return Send(soc, b);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void SendDataEnd(IAsyncResult ar)
        {
            try
            {
                ((Socket)ar.AsyncState).EndSend(ar);
               // ar.AsyncWaitHandle.Close();
            }
            catch
            {
            }
        }

        AsyncCallback callsend;
        public bool Send(Socket soc, byte[] text)
        {
            try
            {
                DataFrame bp = new DataFrame();
                bp.SetByte(text);
                if (Certificate != null)
                {
                    var queryResults = from item in weaveWorkItemsList
                                       where item.SocketSession == soc
                                       select item;
                    foreach (var ssl in queryResults)
                        ssl.Stream.Write(bp.GetBytes());
                }
                else
                    text = bp.GetBytes();
                soc.BeginSend(text, 0, text.Length, SocketFlags.None, callsend, soc);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Send(SslStream ssl, byte command, string text)
        {
            try
            {
                byte[] sendb = Encoding.UTF8.GetBytes(text);
                byte[] lens = Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.SetByte(b);
                if (Certificate != null)
                {
                    ssl.Write(bp.GetBytes());
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public int Partition = 5000;
        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len, int state, int num);

        void Receive(object ias)
        {
            var w = new SpinWait();
            while (true)
            {
                try
                {
                    int c = weaveWorkItemsList.Count;
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                    weaveWorkItemsList.CopyTo(0, netlist, 0, c);
                    Getbuffer(netlist, 0, c, 1, 30);
                    w.SpinOnce();
                }
                catch
                { }
            }
        }

        void Gg(object obj)
        {
            WeaveNetWorkItems[] netlist = obj as WeaveNetWorkItems[];
            Getbuffer(netlist, 0, netlist.Length, 1, 100);
        }

        void Getbuffer(WeaveNetWorkItems[] netlist, int index, int len, int state, int num)
        {
            for (int i = index; i < len; i++)
            {
                if (i >= netlist.Length)
                    return;
                try
                {
                    WeaveNetWorkItems netc = netlist[i];
                    if (netc.SocketSession != null)
                    {
                        if (netc.SocketSession.Available > 0)
                        {
                            if (netc.State == state && !netc.IsPage)
                            {
                                if (netc.SocketSession.Available > num)
                                {
                                    netc.IsPage = true;
                                    if (state == 1)
                                    {
                                        if (Certificate != null)
                                        {

                                            SslStream sslStream = netc.Stream;
                                            if (sslStream.IsAuthenticated)
                                            {
                                                netc.State = 2;
                                                ThreadPool.QueueUserWorkItem(new WaitCallback(ReadCallbackssl), netc);
                                            }
                                        }
                                        else
                                        {
                                            ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ReadCallback3), netc);

                                        }
                                    }
                                }
                            }
                        }
                        else if (netc.allDataList.Length > 0 && !netc.IsPage)
                        {
                            netc.IsPage = true;
                            ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(PackageData), netc);
                        }
                    }
                }
                catch
                { }
            }
        }
        byte[] packageDatabtye(byte[] alldata, Socket soc, SslStream ssl)
        {
            try
            {
                
                {
                lb1122:
                    int bytesRead = alldata.Length;
                    if (bytesRead == 0)
                    {
                        
                        return alldata;
                    };

                    byte[] tempbtye = new byte[bytesRead];
                    Array.Copy(alldata, tempbtye, tempbtye.Length);
                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (a == 0)
                        {
                            byte[] temps = new byte[tempbtye.Length - 2];
                            Array.Copy(tempbtye, 2, temps, 0, temps.Length);
                            alldata = temps;
                            return alldata;
                        }
                        else if (bytesRead > 4 + a)
                        {
                            int len = 0;

                            byte[] bbcrc = new byte[4 + a];
                            Array.Copy(tempbtye, 0, bbcrc, 0, 4 + a);
                            if (CRC.DataCRC(ref bbcrc, 4 + a))
                            {
                                byte[] bb = new byte[a];
                                Array.Copy(tempbtye, 2, bb, 0, a);
                                len = ConvertToInt(bb);
                            }
                            else
                            {
                                byte[] temps = new byte[tempbtye.Length - 1];
                                Array.Copy(tempbtye, 1, temps, 0, temps.Length);
                                alldata = temps;
                                return alldata;
                            }



                            if ((len + 4 + a) > tempbtye.Length)
                            {

                                return alldata;
                            }
                            else if (tempbtye.Length > (len + 4 + a))
                            {
                                try
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 4 + a)];
                                    Array.Copy(tempbtye, (len + 4 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                }
                                catch
                                {

                                    return alldata;
                                }
                                //netc.IsPage = false; return;
                            }
                            else if (tempbtye.Length == (len + 4 + a))
                            {
                                alldata = new byte[0];
                            }
                            try
                            {

                                //  temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                byte[] bs = new byte[len];
                                Array.Copy(tempbtye, (4 + a), bs, 0, bs.Length);
                                WeaveEvent me = new WeaveEvent();
                                me.Command = tempbtye[0];
                                me.Data = "";
                                me.Databit = bs;
                                me.Soc = soc;
                                me.Ssl = ssl;
                                if (ssl != null)
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToEventHanderssl), me);
                                if (weaveReceiveBitEvent != null)
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToBitEventHander), me);
                                //weaveReceiveBitEvent?.Invoke(tempbtye[0], bs, soc);
                                //weaveReceiveBitEvent?.BeginInvoke(tempbtye[0], bs, soc,null,null);

                                return alldata;
                            }
                            catch //(Exception e)
                            {
                                // netc.IsPage = false;
                                return new byte[0];
                            }
                        }
                        else
                        {

                            return alldata;
                        }
                    }
                    else
                    {

                        return alldata;
                    }
                }
            }
            catch
            {

                return new byte[0];
            }

        }

        /// <summary>
        /// 对粘包，分包的处理方法
        /// </summary>
        /// <param name="alldata"></param>
        /// <param name="soc"></param>
        /// <param name="obj"></param>
        private byte[] packageDatajson(byte[] alldata, Socket soc, SslStream ssl)
        {

            try
            {

                {
                lb1122:
                    int bytesRead = alldata.Length;
                    if (bytesRead == 0)
                    {

                        return alldata;
                    };

                    byte[] tempbtye = new byte[bytesRead];
                    Array.Copy(alldata, tempbtye, tempbtye.Length);
                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (a == 0)
                        {
                            byte[] temps = new byte[tempbtye.Length - 2];
                            Array.Copy(tempbtye, 2, temps, 0, temps.Length);
                            alldata = temps;
                            goto lb1122;
                        }
                        else if (bytesRead > 2 + a)
                        {
                            int len = 0;

                            String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                            len = int.Parse(temp);


                            if ((len + 2 + a) > tempbtye.Length)
                            {

                                return alldata;
                            }
                            else if (tempbtye.Length > (len + 2 + a))
                            {
                                try
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 2 + a)];
                                    Array.Copy(tempbtye, (len + 2 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                }
                                catch
                                {

                                    return alldata;
                                }
                                //netc.IsPage = false; return;
                            }
                            else if (tempbtye.Length == (len + 2 + a))
                            {
                                alldata = new byte[0];
                            }
                            try
                            {

                                String temp2 = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                WeaveEvent me = new WeaveEvent();
                                me.Command = tempbtye[0];
                                me.Data = temp2;
                                me.Soc = soc;
                                me.Ssl = ssl;
                                if(ssl!=null)
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToEventHanderssl), me);
                                if (waveReceiveEvent != null)
                                    // waveReceiveEvent?.Invoke(tempbtye[0], temp, soc);
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToEventHander), me);
                                 


                                return alldata;
                            }
                            catch //(Exception e)
                            {
                                // netc.IsPage = false;
                                return new byte[0];
                            }
                        }
                        else
                        {

                            return alldata;
                        }
                    }
                    else
                    {

                        return alldata;
                    }
                }
            }
            catch
            {

                return new byte[0];
            }


        }
        public delegate void packageDataHandler(WeaveNetWorkItems netc);
        private void PackageData(object obj)
        {
            WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            byte[] tempbtyes ;
            try
            {
               int   bytesRead = netc.Buffer.Length;
                if (bytesRead > 0)
                {
                    tempbtyes = new byte[bytesRead];
                    Array.Copy(netc.Buffer, 0, tempbtyes, 0, tempbtyes.Length);
                    netc.Buffer = new byte[0];
                    //int lle = netc.allDataList.Length;

                    //byte[] temp = new byte[lle + tempbtyes.Length];
                    //Array.Copy(netc.allDataList, 0, temp, 0, netc.allDataList.Length);
                    //Array.Copy(tempbtyes, 0, temp, lle, bytesRead);
                    //netc.allDataList = temp; //workItem.DataList.Add(tempbtye);
                }
                else {
                    tempbtyes = new byte[netc.allDataList.Length];
                    Array.Copy(netc.allDataList, 0, tempbtyes, 0, tempbtyes.Length);
                }
                int i = 0;
                
                {

                    DataFrameHeader dfh = null;
                   
                    byte[] masks = new byte[4];
                    int lens = 0;
                    int paylen = 0;
                  
                    try
                    {
                        DataFrame df = new DataFrame();
                        if (tempbtyes.Length <= 2)
                        {
                            netc.IsPage = false; return;
                        }
                        byte[]  tempbtye = df.GetData(tempbtyes, ref masks, ref lens, ref paylen, ref dfh);
                        if (dfh.OpCode != 2)
                        {
                            netc.allDataList = new byte[0];
                             netc.IsPage = false; return;
                        }
                        //if (tempbtyes.Length >= (lens + paylen))
                        //{
                        //    tempbtye = new byte[tempbtyes.Length- (lens + paylen)];
                        //    Array.Copy(tempbtyes, lens + paylen, tempbtye, 0, tempbtye.Length);
                           
                        //    netc.allDataList = tempbtye;
                        //}
                        if (DT == WeaveDataTypeEnum.custom)
                        {

                            WeaveEvent me = new WeaveEvent
                            {
                                Command = defaultCommand,
                                Data = "",
                                Databit = tempbtye,
                                Soc = netc.SocketSession,
                                Ssl = netc.Stream
                            };
                            if (weaveReceiveBitEvent != null)
                                ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ReceiveToBitEventHander), me);
                           
                        }
                        else if (DT == WeaveDataTypeEnum.Json)
                        {
                            netc.allDataList= packageDatajson(tempbtye, netc.SocketSession, netc.Stream);
                        }
                        else if (DT == WeaveDataTypeEnum.Bytes)
                        {
                            netc.allDataList= packageDatabtye(tempbtye, netc.SocketSession, netc.Stream);
                        }
                        
                        netc.IsPage = false; return;
                    
                    }
                    catch(Exception e)
                    {
                    
                        
                         netc.IsPage = false; return;
                    }
                    //if (tempbtye == null)
                    //{
                    //    netc.IsPage = false; return;
                    //}

                  
                }
            }
            catch
            {
                
                netc.IsPage = false; return;
            }
            finally
            {
                netc.IsPage = false;
            }
        }




        public delegate void TestDelegate(string name);
        //string Combine(string temp, byte[] tempbtyes, List<byte[]> ListData)
        //{
        //    DataFrameHeader dfh = null;
        //    byte[] masks = new byte[4];
        //    int lens = 0;
        //    int paylen = 0;
        //    byte[] tempbtye = null;
        //    DataFrame df = new DataFrame();
        //    try
        //    {
        //        tempbtye = df.GetData(tempbtyes, ref masks, ref lens, ref paylen, ref dfh);
        //    }
        //    catch
        //    {
        //        if (paylen > tempbtyes.Length)
        //        {
        //            ListData.RemoveAt(0);
        //            byte[] temps = new byte[tempbtyes.Length];
        //            Array.Copy(tempbtyes, temps, temps.Length);
        //            tempbtyes = new byte[temps.Length + ListData[0].Length];
        //            Array.Copy(temps, tempbtyes, temps.Length);
        //            Array.Copy(ListData[0], 0, tempbtyes, temps.Length, ListData[0].Length);
        //            ListData[0] = tempbtyes;
        //            temp = Combine(temp, ListData[0], ListData);
        //            return temp;
        //        }
        //    }
        //    try
        //    {
        //        temp += Encoding.UTF8.GetString(tempbtye);
        //        if (ListData[0].Length > tempbtye.Length + lens)
        //        {
        //            int aa = ListData[0].Length - (tempbtye.Length + lens);
        //            byte[] temptt = new byte[aa];
        //            Array.Copy(tempbtyes, (tempbtye.Length + lens), temptt, 0, temptt.Length);
        //            ListData[0] = temptt;
        //        }
        //        else
        //        {
        //            ListData.RemoveAt(0);
        //        }
        //        if (!dfh.FIN)
        //        {
        //            while (!(ListData.Count > 0))
        //                Thread.Sleep(100);
        //            temp = Combine(temp, ListData[0], ListData);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return temp;
        //}

        delegate void receiveconndele(object ias);
        void Receiveconn(object ias)
        {
            var w = new SpinWait();
            while (true)
            {
                int c = connlist.Count;
                WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                connlist.CopyTo(0, netlist, 0, c);
                foreach (WeaveNetWorkItems netc in netlist)
                {
                    try
                    {
                        if (netc != null)
                            if (netc.SocketSession != null)
                                if (netc.State == 0)
                                {
                                    if ((DateTime.Now - netc.Lasttime).Seconds > 2)
                                    {
                                        connlist.Remove(netc);
                                        try { netc.SocketSession.Close(); } catch { }
                                    }
                                    else if (netc.SocketSession.Available > 200)
                                    {
                                        if (Certificate != null)
                                        {
                                        }
                                        else
                                        {
                                            netc.SocketSession.Receive(netc.Buffer = new byte[netc.SocketSession.Available]);

                                        }
                                        ThreadPool.QueueUserWorkItem(new WaitCallback(Setherd), netc);
                                        connlist.Remove(netc);
                                    }
                                }
                    }
                    catch { }
                }
                w.SpinOnce();
            }
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

        void Setherd(object obj)
        {
            WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            try
            {

                if (Certificate != null)
                {
                    string httpstr = ReadMessage(netc.Stream);
                    byte[] tempbtye = Encoding.Default.GetBytes(httpstr);
                    tempbtye = ManageHandshake(tempbtye, tempbtye.Length);
                    netc.Stream.Write(tempbtye);
                    netc.Stream.Flush();
                }
                else
               if (!Sendhead(netc.SocketSession, netc.Buffer))
                {

                    try { netc.SocketSession.Close(); } catch { }
                    return;
                }
                netc.State = 1;

                weaveWorkItemsList.Add(netc);
                weaveUpdateSocketListEvent?.Invoke(netc.SocketSession);
            }
            catch
            {

                try { netc.SocketSession.Close(); } catch { }
                return;
            }
        }

        List<WeaveNetWorkItems> connlist = new List<WeaveNetWorkItems>();
        void Accept(object ias)
        {
            while (true)
            {
                try
                {
                    Socket handler = socket.Accept();
                    WeaveNetWorkItems netc = new WeaveNetWorkItems();
                    netc.SocketSession = handler;
                    netc.State = 0;
                    netc.Lasttime = DateTime.Now;
                    if (Certificate != null)
                    {

                        netc.Stream = Authenticate(handler, Certificate, SslProtocols.Default);
                        netc.Stream.AuthenticateAsServer(Certificate, false, SslProtocols.Tls, true);

                        connlist.Add(netc);
                    }
                    else
                        connlist.Add(netc);
                }
                catch
                {
                }
            }
        }

        public SslStream Authenticate(Socket _socket, X509Certificate2 certificate, SslProtocols enabledSslProtocols)
        {
            Stream _stream = new NetworkStream(_socket);
            var ssl = new SslStream(_stream, false);

            return ssl;
        }

        public bool Send(Socket soc, byte command, byte[] data)
        {
            try
            {
                byte[] sendb = data;
                byte[] lens = ConvertToByteList(sendb.Length); ;
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.SetByte(b);
                soc.Send(bp.GetBytes());
            }
            catch { return false; }
            return true;
        }
    }
}
