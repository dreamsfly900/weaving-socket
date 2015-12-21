using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace P2P
{
    public class Webp2psever
    {
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        UDP udp;
        List<NETcollection> listconn = new List<NETcollection>();
        public delegate void myreceive(byte command, String data, Socket soc);
        public event myreceive receiveevent;
        //public delegate void NATthrough(String data, EndPoint ep);
        //public event NATthrough NATthroughevent;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public delegate void UpdataListSoc(Socket soc);
        public event UpdataListSoc EventUpdataConnSoc;
        public delegate void deleteListSoc(Socket soc);
        public event deleteListSoc EventDeleteConnSoc;
        string loaclip;
        public Webp2psever()
        {
          
           // udp = new UDP(_loaclip);
        }
        public void start(int port)
        {
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener.Bind(localEndPoint);
            listener.Listen(1000000);
            //if (NATthroughevent != null)
            //{
            //    udp.start(port);
            //    udp.receiveevent += udp_receiveevent;
            //}
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Accept));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receive));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(xintiao));
        }
        public NETcollectionUdp[] getNATthrough()
        {
            return udp.getUdpList();
        }
        //void udp_receiveevent(byte command, string data, NETcollectionUdp NETc)
        //{


        //    if (NATthroughevent != null)
        //        NATthroughevent(data, NETc.Iep);

        //}

        public int getNum()
        {
            return listconn.Count;
        }
        public bool p2psend(byte command, string text, IPEndPoint ep)
        {
            return udp.send(command, text, ep);

        }
        public void xintiao(object obj)
        {
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(8000);
                    //  ArrayList al = new ArrayList();
                    // al.Clone()
                    NETcollection[] netlist = new NETcollection[listconn.Count];
                    listconn.CopyTo(netlist);
                    foreach (NETcollection netc in netlist)
                    {
                        try
                        {
                            byte[] b = new byte[] { 0x99 };

                            netc.Soc.Send(b);

                        }
                        catch
                        {

                            try { netc.Soc.Close(); }
                            catch { }
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteConnSoc), netc.Soc);


                            listconn.Remove(netc);
                        }

                    }
                }
                catch { }
            }
        }
        private   byte[] AnalyticData(byte[] recBytes, int recByteLength)
        {
            if (recByteLength < 2) { return new byte[0] ; }

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

            int payload_len = recBytes[1] & 0x7F; // 数据长度  

            byte[] masks = new byte[4];
            byte[] payload_data;

            if (payload_len == 126)
            {
                Array.Copy(recBytes, 4, masks, 0, 4);
                payload_len = (UInt16)(recBytes[2] << 8 | recBytes[3]);
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
                UInt64 len = BitConverter.ToUInt64(uInt64Bytes, 0);

                payload_data = new byte[len];
                for (UInt64 i = 0; i < len; i++)
                {
                    payload_data[i] = recBytes[i + 14];
                }
            }
            else
            {
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

        private void DeleteConnSoc(object state)
        {
            if (EventDeleteConnSoc != null)
                EventDeleteConnSoc(state as Socket);
        }

        void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();
                Socket listener = (Socket)ar.AsyncState;
                if (listener == null)
                    return;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.
                NETcollection netc = new NETcollection();
                netc.Soc = handler;
                listconn.Add(netc);
                System.Threading.Thread.Sleep(10);
                handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback2), netc);
                System.Threading.Thread.Sleep(50);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdataConnSoc), handler);

                // netc.Buffer 

              
            }
            catch { }
            // allDone.Set();

        }
        /// <summary>
        /// 打包握手信息
        /// </summary>
        /// <param name="secKeyAccept">Sec-WebSocket-Accept</param>
        /// <returns>数据包</returns>
        private static byte[] PackHandShakeData(string secKeyAccept)
        {
            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + Environment.NewLine);
            responseBuilder.Append("Upgrade: websocket" + Environment.NewLine);
            responseBuilder.Append("Connection: Upgrade" + Environment.NewLine);
            responseBuilder.Append("Sec-WebSocket-Accept: " + secKeyAccept + Environment.NewLine + Environment.NewLine);
            //如果把上一行换成下面两行，才是thewebsocketprotocol-17协议，但居然握手不成功，目前仍没弄明白！
            //responseBuilder.Append("Sec-WebSocket-Accept: " + secKeyAccept + Environment.NewLine);
            //responseBuilder.Append("Sec-WebSocket-Protocol: chat" + Environment.NewLine);

            return Encoding.UTF8.GetBytes(responseBuilder.ToString());
        }

        /// <summary>
        /// 生成Sec-WebSocket-Accept
        /// </summary>
        /// <param name="handShakeText">客户端握手信息</param>
        /// <returns>Sec-WebSocket-Accept</returns>
        private static string GetSecKeyAccetp(byte[] handShakeBytes, int bytesLength)
        {
            string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, bytesLength);
            string key = string.Empty;
            Regex r = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            Match m = r.Match(handShakeText);
            if (m.Groups.Count != 0)
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
            }
            byte[] encryptionString = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            return Convert.ToBase64String(encryptionString);
        }
        private void UpdataConnSoc(object state)
        {
            if (EventUpdataConnSoc != null)
                EventUpdataConnSoc(state as Socket);
        }
        class modelevent
        {
            byte command;

            public byte Command
            {
                get { return command; }
                set { command = value; }
            }
            string data;

            public string Data
            {
                get { return data; }
                set { data = value; }
            }
            Socket soc;

            public Socket Soc
            {
                get { return soc; }
                set { soc = value; }
            }
        }
        void receiveeventto(object obj)
        {
            modelevent me = (modelevent)obj;
            if (receiveevent != null)
                receiveevent(me.Command, me.Data, me.Soc);
        }
        private void ReadCallback2(IAsyncResult ar)
        {
            NETcollection netc = (NETcollection)ar.AsyncState;
            Socket handler = netc.Soc;
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
                    netc.Soc.Close();
                    listconn.Remove(netc);
                }
                byte[] tempbtye = new byte[bytesRead];

                //netc.Buffer.CopyTo(tempbtye, 0);
                Array.Copy(netc.Buffer, 0, tempbtye, 0, bytesRead);
                handler.Send( PackHandShakeData(GetSecKeyAccetp(tempbtye, bytesRead)));
            }
            catch
            {

            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        }
        private void ReadCallback(IAsyncResult ar)
        {
            NETcollection netc = (NETcollection)ar.AsyncState;
            Socket handler = netc.Soc;
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
                    netc.Soc.Close();
                    listconn.Remove(netc);
                }
                
                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    byte[] tempbtye = new byte[bytesRead];

                    //netc.Buffer.CopyTo(tempbtye, 0);
                    Array.Copy(netc.Buffer, 0, tempbtye, 0, bytesRead);
                    tempbtye=AnalyticData(tempbtye, bytesRead);
                    bytesRead = tempbtye.Length;
                    labe881:
                    int a = tempbtye[1];
                    String temp2 = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                    int len = int.Parse(temp2);
                    //int b = netc.Buffer[2 + a+1];
                    //temp = System.Text.ASCIIEncoding.ASCII.GetString(netc.Buffer, 2 + a + 1, b);
                    //len = int.Parse(temp);
                    String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                    modelevent me = new modelevent();
                    me.Command = tempbtye[0];
                    me.Data = temp;
                    me.Soc = netc.Soc;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveeventto), me);

                    if (bytesRead > (2 + a + len))
                    {
                        byte[] b = new byte[bytesRead - (2 + a + len)];
                        byte[] t = tempbtye;
                        Array.Copy(t, (2 + a + len), b, 0, b.Length);

                        tempbtye = b;
                        bytesRead = bytesRead - (2 + a + len);
                        goto labe881;
                    }

                }
            }
            catch
            {

            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        }
        
        /// <summary>
        /// 打包服务器数据
        /// </summary>
        /// <param name="message">数据</param>
        /// <returns>数据包</returns>
        private   byte[] PackData(byte[] buffer)
        {
              
          byte[] _extend = new byte[0];
          byte[] _mask = new byte[0];
          byte[] _content = buffer;
        //帧头
        DataFrameHeader _header = new DataFrameHeader(buffer);
            int length = _content.Length;

            if (length < 126)
            {
                _extend = new byte[0];
                _header = new DataFrameHeader(true, false, false, false, 2, false, length);
            }
            else if (length < 65536)
            {
                _extend = new byte[2];
                _header = new DataFrameHeader(true, false, false, false, 2, false, 126);
                _extend[0] = (byte)(length / 256);
                _extend[1] = (byte)(length % 256);
            }
            else
            {
                _extend = new byte[8];
                _header = new DataFrameHeader(true, false, false, false, 2, false, 127);

                int left = length;
                int unit = 256;

                for (int i = 7; i > 1; i--)
                {
                    _extend[i] = (byte)(left % unit);
                    left = left / unit;

                    if (left == 0)
                        break;
                }
            }

            return GetBytes(_header, _extend, _mask, _content);
    
        }
        public byte[] GetBytes(DataFrameHeader _header,byte[] _extend, byte[] _mask, byte[] _content)
        {
            byte[] buffer = new byte[2 + _extend.Length + _mask.Length + _content.Length];
            Buffer.BlockCopy(_header.GetBytes(), 0, buffer, 0, 2);
            Buffer.BlockCopy(_extend, 0, buffer, 2, _extend.Length);
            Buffer.BlockCopy(_mask, 0, buffer, 2 + _extend.Length, _mask.Length);
            Buffer.BlockCopy(_content, 0, buffer, 2 + _extend.Length + _mask.Length, _content.Length);
            return buffer;
        }
        private byte[] Mask(byte[] data, byte[] mask)
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ mask[i % 4]);
            }

            return data;
        }
        public bool send(int index, byte command, string text)
        {

            try
            {
                Socket soc = listconn[index].Soc;
                byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                byte[] bp = PackData(b);
                soc.Send(bp);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        public bool WEBsend(Socket soc, byte command, string text)
        {

            try
            {
                byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                byte[] bp = PackData(b);
                soc.Send(bp);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        void receive(object ias)
        {
            while (true)
            {
                try
                {
                    NETcollection[] netlist = new NETcollection[listconn.Count];
                    listconn.CopyTo(netlist);
                    foreach (NETcollection netc in netlist)
                    {
                        if (netc.Soc.Available > 0)
                        {
                            netc.Buffer = new byte[netc.Soc.Available];
                            netc.Soc.BeginReceive(netc.Buffer, 0, netc.Soc.Available, 0, new AsyncCallback(ReadCallback), netc);
                        }
                    }
                }
                catch { }
                System.Threading.Thread.Sleep(100);
            }
        }

        void Accept(object ias)
        {
            while (true)
            {
                // Set the event to nonsignaled state.
                allDone.Reset();

                //开启异步监听socket
                //    Console.WriteLine("Waiting for a connection");
                try
                {
                    listener.BeginAccept(
                              new AsyncCallback(AcceptCallback),
                              listener);
                    System.Threading.Thread.Sleep(100);
                }
                catch { }
                // 让程序等待，直到连接任务完成。在AcceptCallback里的适当位置放置allDone.Set()语句.
                allDone.WaitOne();
            }

        }
    }
 public class DataFrameHeader
{
    private bool _fin;
    private bool _rsv1;
    private bool _rsv2;
    private bool _rsv3;
    private sbyte _opcode;
    private bool _maskcode;
    private sbyte _payloadlength;

    public bool FIN { get { return _fin; } }

    public bool RSV1 { get { return _rsv1; } }

    public bool RSV2 { get { return _rsv2; } }

    public bool RSV3 { get { return _rsv3; } }

    public sbyte OpCode { get { return _opcode; } }

    public bool HasMask { get { return _maskcode; } }

    public sbyte Length { get { return _payloadlength; } }

    public DataFrameHeader(byte[] buffer)
    {
        if (buffer.Length < 2)
            throw new Exception("无效的数据头.");

        //第一个字节
        _fin = (buffer[0] & 0x80) == 0x80;
        _rsv1 = (buffer[0] & 0x40) == 0x40;
        _rsv2 = (buffer[0] & 0x20) == 0x20;
        _rsv3 = (buffer[0] & 0x10) == 0x10;
        _opcode = (sbyte)(buffer[0] & 0x0f);

        //第二个字节
        _maskcode = (buffer[1] & 0x80) == 0x80;
        _payloadlength = (sbyte)(buffer[1] & 0x7f);

    }

    //发送封装数据
    public DataFrameHeader(bool fin, bool rsv1, bool rsv2, bool rsv3, sbyte opcode, bool hasmask, int length)
    {
        _fin = fin;
        _rsv1 = rsv1;
        _rsv2 = rsv2;
        _rsv3 = rsv3;
        _opcode = opcode;
        //第二个字节
        _maskcode = hasmask;
        _payloadlength = (sbyte)length;
    }

    //返回帧头字节
    public byte[] GetBytes()
    {
        byte[] buffer = new byte[2] { 0, 0 };

        if (_fin) buffer[0] ^= 0x80;
        if (_rsv1) buffer[0] ^= 0x40;
        if (_rsv2) buffer[0] ^= 0x20;
        if (_rsv3) buffer[0] ^= 0x10;

        buffer[0] ^= (byte)_opcode;

        if (_maskcode) buffer[1] ^= 0x80;

        buffer[1] ^= (byte)_payloadlength;

        return buffer;
    }
}
}
