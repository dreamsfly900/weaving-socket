using Fleck;
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
using System.Threading.Tasks;
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
    
        public event WeaveReceiveSslEvent weaveReceiveSslEvent;
        WeaveDataTypeEnum DT = WeaveDataTypeEnum.Json;
        public X509Certificate2 Certificate { get; set; }
        public SslProtocols EnabledSslProtocols { get; set; }
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
            Port = port;
            if (DT == WeaveDataTypeEnum.Json && waveReceiveEvent == null)
                throw new Exception("没有注册receiveevent事件");
            if (DT == WeaveDataTypeEnum.Bytes && weaveReceiveBitEvent == null)
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
            System.Threading.Thread t = new Thread(new ParameterizedThreadStart(Accept));
            t.Start();
            System.Threading.Thread t1 = new Thread(new ParameterizedThreadStart(receive));
            t1.Start();
            System.Threading.Thread t2 = new Thread(new ParameterizedThreadStart(receivepackageData));
            t2.Start();
            System.Threading.Thread t3 = new Thread(new ParameterizedThreadStart(KeepAliveHander));
            t3.Start();
            System.Threading.Thread t4 = new Thread(new ParameterizedThreadStart(receiveconn));
            t4.Start();
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveconn));
        }
        //void udp_receiveevent(byte command, string data, NETcollectionUdp NETc)
        //{
        //    if (NATthroughevent != null)
        //        NATthroughevent(data, NETc.Iep);
        //}
        public int GetNetworkItemCount()
        {
            return weaveWorkItemsList.Count;
        }
        public void KeepAliveHander(object obj)
        {
            while (true)
            {
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
                              
                                 DataFrame df = new DataFrame();
                                df.setByte(new byte[] { 0x99 });
                                if (Certificate != null)
                                {
                                    workItem.Stream.Write(df.GetBytes());
                                }
                                else
                                  workItem.SocketSession.Send(df.GetBytes());
                                workItem.ErrorNum = 0;
                                 
                            }
                        }
                        catch
                        {
                            workItem.ErrorNum += 1;
                            if (workItem.ErrorNum > 3)
                            {
                                ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteSocketListHander), workItem.SocketSession);
                              
                              
                                // EventDeleteConnSoc.BeginInvoke(netc.Soc, null, null);
                                weaveWorkItemsList.Remove(workItem);
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(8000);
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
                payload_len = (UInt16)(recBytes[2] << 8 | recBytes[3]);
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
                for (UInt64 i = 0; i < len; i++)
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
            try { (state as Socket).Close(); }
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
            weaveReceiveSslEvent?.Invoke(me.Command, me.Data, me.Ssl);
        }
        void ReceiveToBitEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            weaveReceiveBitEvent?.Invoke(me.Command, me.Databit, me.Soc);
        }
        private void ReadCallback2(IAsyncResult ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar.AsyncState;
            Socket handler = netc.SocketSession;
            int bytesRead = 0;
            try
            {
                try
                {
                    bytesRead = handler.EndReceive(ar);
                }
                catch
                {
                    netc.SocketSession.Close();
                    weaveWorkItemsList.Remove(netc);
                }
                byte[] tempbtye = new byte[bytesRead];
                Array.Copy(netc.Buffer, 0, tempbtye, 0, bytesRead);
                //   System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback())
                netc.State = 1;
                SendHeadDelegate sh = new SendHeadDelegate(sendhead);
                IAsyncResult ia = sh.BeginInvoke(handler, tempbtye, null, null);
                sh.EndInvoke(ia);
                //if (EventUpdataConnSoc != null)
                //    EventUpdataConnSoc.BeginInvoke(handler, null, null);
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateSocketListHander), handler);
                return;
                //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(UpdataConnSoc));
                //t.Start(handler);
                // handler.BeginSend(aaa, 0, aaa.Length, 0, HandshakeFinished, handler);
            }
            catch //(Exception e)
            {
            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        }
        bool sendhead(Socket handler, byte[] tempbtye)
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
            System.Text.UTF8Encoding decoder = new System.Text.UTF8Encoding();
            String rawClientHandshake = Encoding.UTF8.GetString(receivedDataBuffer);
            //String rawClientHandshake = decoder.GetString(receivedDataBuffer, 0, HandshakeLength);
            Array.Copy(receivedDataBuffer, HandshakeLength - 8, last8Bytes, 0, 8);
            //现在使用的是比较新的Websocket协议
            if (rawClientHandshake.IndexOf(header) != -1)
            {
                //  isDataMasked = true;
                string[] rawClientHandshakeLines = rawClientHandshake.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
                string acceptKey = "";
                foreach (string Line in rawClientHandshakeLines)
                {
                    // //Console.WriteLine(Line);
                    if (Line.Contains("Sec-WebSocket-Key:"))
                    {
                        acceptKey = ComputeWebSocketHandshakeSecurityHash09(Line.Substring(Line.IndexOf(":") + 2));
                    }
                }
                New_Handshake = string.Format(New_Handshake, acceptKey);
                byte[] newHandshakeText = Encoding.UTF8.GetBytes(New_Handshake);
                //   ConnectionSocket.BeginSend(newHandshakeText, 0, newHandshakeText.Length, 0, HandshakeFinished, null);
                return newHandshakeText;
            }
            string ClientHandshake = decoder.GetString(receivedDataBuffer, 0, HandshakeLength - 8);
            string[] ClientHandshakeLines = ClientHandshake.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            //   logger.Log("新的连接请求来自" + ConnectionSocket.LocalEndPoint + "。正在准备连接 ...");
            // Welcome the new client
            foreach (string Line in ClientHandshakeLines)
            {
                // logger.Log(Line);
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
            //logger.Log("发送握手信息 ...");
            // ConnectionSocket.BeginSend(serverHandshakeResponse, 0, HandshakeText.Length + 16, 0, HandshakeFinished, null);
            //    logger.Log(Handshake);
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
                currentKey = BitConverter.GetBytes((int)(Int64.Parse(partialServerKey) / spacesNum));
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
            System.Security.Cryptography.MD5 MD5Service = System.Security.Cryptography.MD5.Create();
            return MD5Service.ComputeHash(concatenatedKeys);
        }
        public static String ComputeWebSocketHandshakeSecurityHash09(String secWebSocketKey)
        {
            const String MagicKEY = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            String secWebSocketAccept = String.Empty;
            // 1. Combine the request Sec-WebSocket-Key with magic key.
            String ret = secWebSocketKey + MagicKEY;
            // 2. Compute the SHA1 hash
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] sha1Hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ret));
            // 3. Base64 encode the hash
            secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            return secWebSocketAccept;
        }
        private void ReadCallback(IAsyncResult ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar.AsyncState;
            Socket handler = netc.SocketSession;
            
            //if (!netc.Soc.Poll(100, SelectMode.SelectRead))
            //{
            //    listconn.Remove(netc);
            //    return;
            //}
            // Read data from the client socket. 
            try
            {
                int bytesRead = 0;
                try
                {
                    bytesRead = handler.EndReceive(ar);
                }
                catch
                {
                    netc.SocketSession.Close();
                    weaveWorkItemsList.Remove(netc);
                }
                //if (bytesRead > 0)
                //{
                //    // There  might be more data, so store the data received so far.
                byte[] tempbtye = new byte[bytesRead];
                //netc.Buffer.CopyTo(tempbtye, 0);
                Array.Copy(netc.Buffer, 0, tempbtye, 0, bytesRead);
                netc.DataList.Add(tempbtye);
                //  System.Threading.Thread.Sleep(10);
            }
            catch
            {
            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
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

            netc.DataList.Add(listb.ToArray());
            netc.State = 1;
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        }
        private void ReadCallback3(object ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar;
            Socket handler = netc.SocketSession;
            //if (!netc.Soc.Poll(100, SelectMode.SelectRead))
            //{
            //    listconn.Remove(netc);
            //    return;
            //}
            // Read data from the client socket. 
            try
            {
                handler.Receive(netc.Buffer = new byte[netc.SocketSession.Available]);
                //if (bytesRead > 0)
                //{
                //    // There  might be more data, so store the data received so far.
                byte[] tempbtye = new byte[netc.Buffer.Length];
                //netc.Buffer.CopyTo(tempbtye, 0);
                Array.Copy(netc.Buffer, 0, tempbtye, 0, tempbtye.Length);
                netc.DataList.Add(tempbtye);
                //  System.Threading.Thread.Sleep(10);
            }
            catch
            {
            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        }
        public bool send(int index, byte command, string text)
        {
            try
            {
                Socket soc = weaveWorkItemsList[index].SocketSession;
                byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.setByte(b);
                soc.Send(bp.GetBytes());
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        public IPAddress getLocalmachineIPAddress()
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
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.setByte(b);
                if (Certificate != null)
                {

                    var queryResults = from item in weaveWorkItemsList
                                       where item.SocketSession == soc
                                       select item;
                    foreach (var ssl in queryResults)
                        ssl.Stream.Write(bp.GetBytes());
                }
                else
                    soc.Send(bp.GetBytes());
                //soc.Send(bp);
            }
            catch (Exception e)
            {
                return false; }
            // tcpc.Close();
            return true;
        }
        public bool Send(SslStream ssl, byte command, string text)
        {
            try
            {
                byte[] sendb = Encoding.UTF8.GetBytes(text);
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                DataFrame bp = new DataFrame();
                bp.setByte(b);
                if (Certificate != null)
                {

                     
                        ssl.Write(bp.GetBytes());
                }
                
                //soc.Send(bp);
            }
            catch (Exception e)
            {
                return false;
            }
            // tcpc.Close();
            return true;
        }
        public int Partition = 5000;
        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len, int state, int num);
        void receive(object ias)
        {
            while (true)
            {
                try
                {
                    //int c = listconn.Count;
                    //int count = (c / Partition) + 1;
                    //getbufferdelegate[] iagbd = new getbufferdelegate[count];
                    //IAsyncResult[] ia = new IAsyncResult[count];
                    //if (c > 0)
                    //{
                    //    for (int i = 0; i < count; i++)
                    //    {
                    //        c = c - (i * Partition) > Partition ? Partition : c - (i * Partition);
                    //        NETcollection[] netlist = new NETcollection[c];
                    //        listconn.CopyTo(i * Partition, netlist, 0, c);
                    //        iagbd[i] = new getbufferdelegate(getbuffer);
                    //        ia[i] = iagbd[i].BeginInvoke(netlist, 0, Partition,1,30, null, null);
                    //    }
                    //    for (int i = 0; i < count; i++)
                    //    {
                    //        iagbd[i].EndInvoke(ia[i]);
                    //    }
                    //}
                    int c = weaveWorkItemsList.Count;
                    //int count = (c / 2000) + 1;
                    //if (c > 0)
                    //    for (int i = 0; i < count; i++)
                    //    {
                    //        c = c - (i * 2000) > 2000 ? 2000 : c - (i * 2000);
                    //        NETcollection[] netlist2 = new NETcollection[c];
                    //        listconn.CopyTo(i * 2000, netlist2, 0, c);
                    //        System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(gg), netlist2);
                    //        //new getbufferdelegate(getbuffer).BeginInvoke(netlist, 0, 2000,null,null);
                    //    }
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                    weaveWorkItemsList.CopyTo(0, netlist, 0, c);
                    getbuffer(netlist, 0, c, 1, 30);
                    System.Threading.Thread.Sleep(1);
                }
                catch //(Exception ex)
                { }
            }
        }
        void gg(object obj)
        {
            WeaveNetWorkItems[] netlist = obj as WeaveNetWorkItems[];
            getbuffer(netlist, 0, netlist.Length, 1, 100);
        }
        void getbuffer(WeaveNetWorkItems[] netlist, int index, int len, int state, int num)
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
                            if (netc.State == state)
                            {
                                if (netc.SocketSession.Available > num)
                                {
                                  //  if (state == 0)
                                     //   netc.SocketSession.BeginReceive(netc.Buffer = new byte[netc.SocketSession.Available], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback2), netc);
                                    if (state == 1)
                                    {
                                        if (Certificate != null)
                                        {
                                        
                                           SslStream sslStream = netc.Stream;
                                            if (sslStream.IsAuthenticated)
                                            {
                                                // byte[] bb = new byte[2];
                                                //int c= sslStream.Read(bb, 0, 2);
                                                // if (c <= 2)
                                                // {
                                                //     netc.Buffer = new byte[20480];
                                                //     for (int g = 0; g < c; g++)
                                                //         netc.Buffer[g] = bb[g];
                                                //     sslStream.BeginRead(netc.Buffer, c, netc.Buffer.Length, ReadCallbackssl, netc);
                                                // }
                                                netc.State = 2;
                                                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReadCallbackssl), netc);
                                            }
                                        }
                                        else
                                        {
                                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReadCallback3), netc);

                                        }
                                      
                                    }
                                    // netc.Soc.BeginReceive(netc.Buffer = new byte[netc.Soc.Available], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback), netc);
                                    // listconn.Find(p=>p==netc).State = 1;
                                }
                            }
                        }
                    }
                }
                catch
                { }
            }
        }
        void receivepackageData(object ias)
        {
            while (true)
            {
                try
                {
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[weaveWorkItemsList.Count];
                    weaveWorkItemsList.CopyTo(netlist);
                    foreach (WeaveNetWorkItems netc in netlist)
                    {
                        if (netc.DataList.Count > 0)
                        {
                            if (!netc.IsPage)
                            {
                                netc.IsPage = true;
                                //System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(packageData));
                                //t.Start(netc);
                                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(packageData), netc);
                                //packageDataHandler pdh = new packageDataHandler(packageData);
                                //pdh.BeginInvoke(netc, null, null);
                            }
                            // System.Threading.Thread.Sleep(1);
                        }
                        // System.Threading.Thread.Sleep(5);
                    }
                    System.Threading.Thread.Sleep(5);
                }
                catch// (Exception ex)
                { }
            }
        }
        public delegate void packageDataHandler(WeaveNetWorkItems netc);
        private void packageData(object obj)
        {
            WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            try
            {
                //  Array.Copy(netc.Datalist, ListData, count);
                //while (true)
                //{
                int count = netc.DataList.Count;
                List<Byte[]> ListData = netc.DataList;
                int i = 0;
                if (netc.DataList.Count > 0)
                {
                    DataFrameHeader dfh = null;
                    int bytesRead = ListData[i] != null ? ListData[i].Length : 0;
                    if (bytesRead == 0) { if (ListData.Count > 0) ListData.RemoveAt(0); netc.IsPage = false; return; };
                    byte[] tempbtyes = new byte[bytesRead];
                    Array.Copy(ListData[i], tempbtyes, tempbtyes.Length);
                    byte[] masks = new byte[4];
                    int lens = 0;
                    int paylen = 0;
                    byte[] tempbtye = null;
                    try
                    {
                        DataFrame df = new DataFrame();
                        // AnalyticData(tempbtyes, bytesRead, ref masks, ref lens, ref paylen);
                        tempbtye = df.GetData(tempbtyes, ref masks, ref lens, ref paylen, ref dfh);
                        if (dfh.OpCode != 2)
                        {
                            ListData.RemoveAt(i);
                            netc.IsPage = false; return;
                        }
                    }
                    catch
                    {
                        if (paylen > bytesRead)
                        {
                            ListData.RemoveAt(i);
                            byte[] temps = new byte[tempbtyes.Length];
                            Array.Copy(tempbtyes, temps, temps.Length);
                            tempbtyes = new byte[temps.Length + ListData[i].Length];
                            Array.Copy(temps, tempbtyes, temps.Length);
                            Array.Copy(ListData[i], 0, tempbtyes, temps.Length, ListData[i].Length);
                            ListData[i] = tempbtyes;
                        }
                        else
                        {
                            ListData.RemoveAt(i);
                        }
                        netc.IsPage = false; return;
                    }
                    if (tempbtye == null)
                    {
                        netc.IsPage = false; return;
                    }
                  
                    if (tempbtye.Length > 0)
                    {
                        #region MyRegion
                        String temp = "";
                        int a = tempbtye[1];
                        if (bytesRead > 2 + a)
                        {
                            int len = 0;
                            if (DT == WeaveDataTypeEnum.Bytes)
                            {
                                byte[] bb = new byte[a];
                                Array.Copy(tempbtye, 2, bb, 0, a);
                                len = ConvertToInt(bb);
                            }
                            else
                            {
                                temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                                try
                                {
                                    len = int.Parse(temp);
                                }
                                catch
                                {
                                    if (bytesRead > tempbtye.Length + lens)
                                    {
                                        int aa = bytesRead - (tempbtye.Length + lens);
                                        byte[] temptt = new byte[aa];
                                        Array.Copy(tempbtyes, (tempbtye.Length + lens), temptt, 0, temptt.Length);
                                        ListData[i] = temptt;
                                        netc.IsPage = false; return;
                                    }
                                }
                            }
                            if (tempbtye.Length == (len + 2 + a))
                            {
                                if (bytesRead > tempbtye.Length + lens)
                                {
                                    int aa = bytesRead - (tempbtye.Length + lens);
                                    byte[] temptt = new byte[aa];
                                    Array.Copy(tempbtyes, (tempbtye.Length + lens), temptt, 0, temptt.Length);
                                    ListData[i] = temptt;
                                }
                                else if (bytesRead < tempbtye.Length + lens)
                                {
                                }
                                else
                                    ListData.RemoveAt(i);
                                if (DT == WeaveDataTypeEnum.Json)
                                {
                                    temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                }
                            }
                            else
                            {
                                len = tempbtye.Length - 2 - a;
                                if (DT == WeaveDataTypeEnum.Json)
                                {
                                    temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                }
                                if (bytesRead > tempbtye.Length + lens)
                                {
                                    int aa = bytesRead - (tempbtye.Length + lens);
                                    byte[] temptt = new byte[aa];
                                    Array.Copy(tempbtyes, (tempbtye.Length + lens), temptt, 0, temptt.Length);
                                    ListData[i] = temptt;
                                    temp = combine(temp, temptt, ListData);
                                }
                                else
                                {
                                    ListData.RemoveAt(i);
                                    while (!(ListData.Count > 0))
                                        System.Threading.Thread.Sleep(100);
                                    temp = combine(temp, ListData[i], ListData);
                                }
                                // netc.Ispage = false; return;
                            }
                            try
                            {
                                if (DT == WeaveDataTypeEnum.Json)
                                {
                                    WeaveEvent me = new WeaveEvent();
                                    me.Command = tempbtye[0];
                                    me.Data = temp;
                                    me.Soc = netc.SocketSession;
                                    me.Masks = masks;
                                    me.Ssl = netc.Stream;
                                    //System.Threading.Thread t = new Thread(new ParameterizedThreadStart(receiveeventto));
                                    //t.Start(me);
                                    //receiveeventto(me);
                                    if (waveReceiveEvent != null)
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToEventHander), me);
                                    if (weaveReceiveSslEvent != null)
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToEventHanderssl), me); 
                                    //if (receiveevent != null)
                                    //    receiveevent(me.Command, me.Data, me.Soc);
                                }
                                else if (DT == WeaveDataTypeEnum.Bytes)
                                {
                                    byte[] bs = new byte[len - (2 + a)];
                                    Array.Copy(tempbtye, 2 + a, bs, 0, bs.Length);
                                    WeaveEvent me = new WeaveEvent();
                                    me.Command = tempbtye[0];
                                    me.Data = "";
                                    me.Databit = bs;
                                    me.Soc = netc.SocketSession;
                                    me.Ssl = netc.Stream;
                                    if (weaveReceiveBitEvent != null)
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveToBitEventHander), me);
                                }
                                netc.IsPage = false; return;
                            }
                            catch //(Exception e)
                            {
                                netc.IsPage = false; return;
                            }
                        }
                        #endregion
                    }
                }
                // }
            }
            catch //(Exception ex)
            {
                if (netc.DataList.Count > 0) netc.DataList.RemoveAt(0); netc.IsPage = false; return;
            }
        }
        public delegate void TestDelegate(string name);
        String combine(string temp, byte[] tempbtyes, List<byte[]> ListData)
        {
            DataFrameHeader dfh = null;
            //  Array.Copy(ListData[0], tempbtyes, tempbtyes.Length);
            byte[] masks = new byte[4];
            int lens = 0;
            int paylen = 0;
            byte[] tempbtye = null;
            DataFrame df = new DataFrame();
            // AnalyticData(tempbtyes, bytesRead, ref masks, ref lens, ref paylen);
            try
            {
                tempbtye = df.GetData(tempbtyes, ref masks, ref lens, ref paylen, ref dfh);
            }
            catch
            {
                if (paylen > tempbtyes.Length)
                {
                    ListData.RemoveAt(0);
                    byte[] temps = new byte[tempbtyes.Length];
                    Array.Copy(tempbtyes, temps, temps.Length);
                    tempbtyes = new byte[temps.Length + ListData[0].Length];
                    Array.Copy(temps, tempbtyes, temps.Length);
                    Array.Copy(ListData[0], 0, tempbtyes, temps.Length, ListData[0].Length);
                    ListData[0] = tempbtyes;
                    temp = combine(temp, ListData[0], ListData);
                    return temp;
                }
            }
            try
            {
                temp += System.Text.Encoding.UTF8.GetString(tempbtye);
                if (ListData[0].Length > tempbtye.Length + lens)
                {
                    int aa = ListData[0].Length - (tempbtye.Length + lens);
                    byte[] temptt = new byte[aa];
                    Array.Copy(tempbtyes, (tempbtye.Length + lens), temptt, 0, temptt.Length);
                    ListData[0] = temptt;
                }
                else
                {
                    ListData.RemoveAt(0);
                }
                if (!dfh.FIN)
                {
                    while (!(ListData.Count > 0))
                        System.Threading.Thread.Sleep(100);
                    temp = combine(temp, ListData[0], ListData);
                }
            }
            catch //(Exception e)
            {
            }
            return temp;
        }
        delegate void receiveconndele(object ias);
        void receiveconn(object ias)
        {
            //  NETcollection netcs = ias as NETcollection;
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
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(setherd), netc);
                                        connlist.Remove(netc);
                                    }
                                }
                    }
                    catch { }
                }
                System.Threading.Thread.Sleep(1);
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

        void setherd(object obj)
        {
            WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            try
            {

                if (Certificate != null)
                {
                    String httpstr = ReadMessage(netc.Stream);
                    byte[] tempbtye = System.Text.Encoding.Default.GetBytes(httpstr);
                    tempbtye = ManageHandshake(tempbtye, tempbtye.Length);
                    netc.Stream.Write(tempbtye);
                    netc.Stream.Flush();
                }
                else
               if (!sendhead(netc.SocketSession, netc.Buffer))
                {
                   
                    try { netc.SocketSession.Close(); } catch { }
                    return;
                }
                //  System.Threading.Thread.Sleep(50);
                // new sendheaddele(sendhead).BeginInvoke(netc.Soc, netc.Buffer, null, null);
                netc.State = 1;
              
                weaveWorkItemsList.Add(netc);
                if (weaveUpdateSocketListEvent != null)
                    weaveUpdateSocketListEvent(netc.SocketSession);
                // EventUpdataConnSoc.BeginInvoke(netc.Soc, null, null);
            }
            catch {
               
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
                    //if (listener == null)
                    //    return;
                    //Socket handler = listener.EndAccept(ar);
                    //listener.BeginAccept(new AsyncCallback(Accept), listener);
                    // Create the state object.
                    WeaveNetWorkItems netc = new WeaveNetWorkItems();
                    netc.SocketSession = handler;
                    netc.State = 0;
                    // listconn.Add(netc);
                    netc.Lasttime = DateTime.Now;
                    if (Certificate != null)
                    {
                        String error;
                       
                        netc.Stream = Authenticate(handler, Certificate, SslProtocols.Default); 
                        netc.Stream.AuthenticateAsServer(Certificate, false, SslProtocols.Tls, true);
                       
                        connlist.Add(netc);
                    }
                    else
                        connlist.Add(netc);
                    // new receiveconndele(setherd2).BeginInvoke(handler, null, null);
                    // System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(setherd2), handler);
                    //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback2), netc);
                }
                catch// (Exception ex)
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
                bp.setByte(b);
                soc.Send(bp.GetBytes());
                //soc.Send(bp);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
    }
}
