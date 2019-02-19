using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Weave.Base.Interface;

namespace Weave.Base
{
    /// <summary>
    /// 继承自IWeaveTcpBase接口的 类
    /// </summary>
    public class WeaveBaseServer : IWeaveTcpBase
    {
        [DefaultValue(WeaveDataTypeEnum.Json)]
        public WeaveDataTypeEnum WeaveDataType
        {
            get; set;
        }

        protected Socket socketLisener = null;
        private readonly List<WeaveNetWorkItems> weaveNetworkItems = new List<WeaveNetWorkItems>();
        public event WaveReceiveEventEvent waveReceiveEvent;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
        public event WeaveReceiveBitEvent weaveReceiveBitEvent;
        public byte defaultCommand = 0x0;
        protected string loaclip;
        public int Port { get; set; }

        public WeaveBaseServer()
        {
        }

        public WeaveBaseServer(string _loaclip)
        {
            loaclip = _loaclip;
        }

        public WeaveBaseServer(WeaveDataTypeEnum weaveDataType)
        {
            WeaveDataType = weaveDataType;
        }

        public void Start(int port)
        {
            acallsend = new AsyncCallback(SendDataEnd);
            Port = port;
            if (WeaveDataType == WeaveDataTypeEnum.Json && waveReceiveEvent == null)
                throw new Exception("没有注册receiveevent事件");
            if (WeaveDataType == WeaveDataTypeEnum.Bytes && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            if (WeaveDataType == WeaveDataTypeEnum.custom && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            socketLisener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socketLisener.Bind(localEndPoint);
            socketLisener.Listen(1000000);
            Thread ThreadAcceptHander = new Thread(new ParameterizedThreadStart(AcceptHander));
            Thread ThreadReceiveHander = new Thread(new ParameterizedThreadStart(ReceiveHander));
            Thread ThreadKeepAliveHander = new Thread(new ParameterizedThreadStart(this.KeepAliveHander));
            ThreadAcceptHander.Start();
            ThreadReceiveHander.Start();
            ThreadKeepAliveHander.Start();
        }

        public int GetNetworkItemCount()
        {
            return weaveNetworkItems.Count;
        }

        void KeepAliveHander(object obj)
        {
            while (true)
            {
                try
                {
                    WeaveNetWorkItems[] workItems = new WeaveNetWorkItems[weaveNetworkItems.Count];
                    weaveNetworkItems.CopyTo(workItems);
                    foreach (WeaveNetWorkItems workItem in workItems)
                    {
                        if (workItem == null)
                            continue;
                        try
                        {
                            byte[] b = new byte[] { 0x99 };
                            lock (workItem.SocketSession)
                            {
                                if (!Send(workItem.SocketSession, b))
                                {
                                    workItem.ErrorNum += 1;
                                    if (workItem.ErrorNum > 3)
                                    {
                                        ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(DeleteSocketListEventHander), workItem.SocketSession);

                                        weaveNetworkItems.Remove(workItem);
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
                                ThreadPool.UnsafeQueueUserWorkItem(
                                    new WaitCallback(DeleteSocketListEventHander),
                                    workItem.SocketSession);

                                try
                                {
                                    weaveNetworkItems.Remove(workItem);
                                }
                                catch (Exception EX_NAME)
                                {
                                    Console.WriteLine(EX_NAME);
                                    throw;
                                }

                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
                catch { }
            }
        }

        private void DeleteSocketListEventHander(object state)
        {
            weaveDeleteSocketListEvent?.Invoke(state as Socket);
            try { (state as Socket).Close(); }
            catch { }
        }

        private void UpdateSocketListEventHander(object state)
        {
            weaveUpdateSocketListEvent?.Invoke(state as Socket);
        }

        void ReceiveEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            waveReceiveEvent?.Invoke(me.Command, me.Data, me.Soc);
        }

        void ReceiveBitEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            weaveReceiveBitEvent?.Invoke(me.Command, me.Databit, me.Soc);
        }

        public int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret += (item << i);
                i += 8;
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
                value >>= 8;
            }
            byte[] bb = new byte[ret.Count];
            ret.CopyTo(bb);
            return bb;
        }

        byte[] PackageDatabtye(byte[] alldata, Socket soc)
        {
            try
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
                            goto lb1122;
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
                        }
                        else if (tempbtye.Length == (len + 4 + a))
                        {
                            alldata = new byte[0];
                        }
                        try
                        {
                            byte[] bs = new byte[len];
                            Array.Copy(tempbtye, (4 + a), bs, 0, bs.Length);
                            WeaveEvent me = new WeaveEvent
                            {
                                Command = tempbtye[0],
                                Data = "",
                                Databit = bs,
                                Soc = soc
                            };
                            if (weaveReceiveBitEvent != null)
                                ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ReceiveBitEventHander), me);

                            return alldata;
                        }
                        catch
                        {
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
            catch
            {
                return new byte[0];
            }
        }

        private byte[] PackageData(byte[] alldata, Socket soc)
        {

            try
            {
                if (WeaveDataType == WeaveDataTypeEnum.Json)
                {
                    return PackageDatajson(alldata, soc);
                }
                else if (WeaveDataType == WeaveDataTypeEnum.Bytes)
                {
                    return PackageDatabtye(alldata, soc);
                }
                return alldata;
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
        /// <returns></returns>
        private byte[] PackageDatajson(byte[] alldata, Socket soc)
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

                            string temp = Encoding.UTF8.GetString(tempbtye, 2, a);
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
                            }
                            else if (tempbtye.Length == (len + 2 + a))
                            {
                                alldata = new byte[0];
                            }
                            try
                            {

                                string temp2 = Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                WeaveEvent me = new WeaveEvent
                                {
                                    Command = tempbtye[0],
                                    Data = temp2,
                                    Soc = soc
                                };
                                if (waveReceiveEvent != null)
                                    ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ReceiveEventHander), me);
                                return alldata;
                            }
                            catch
                            {
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

        private void ReadCallback(IAsyncResult ar)
        {
            WeaveNetWorkItems workItem = (WeaveNetWorkItems)ar.AsyncState;
            Socket handler = workItem.SocketSession;
            try
            {
                int bytesRead = 0;
                try
                {
                    bytesRead = handler.EndReceive(ar);
                }
                catch
                {
                }
                bytesRead = workItem.Buffer.Length;
                byte[] tempbtye = new byte[bytesRead];
                Array.Copy(workItem.Buffer, 0, tempbtye, 0, tempbtye.Length);
                if (WeaveDataType == WeaveDataTypeEnum.custom)
                {
                    if (tempbtye.Length > 0)
                    {
                        WeaveEvent me = new WeaveEvent
                        {
                            Command = defaultCommand,
                            Data = "",
                            Databit = tempbtye,
                            Soc = workItem.SocketSession
                        };
                        if (weaveReceiveBitEvent != null)
                            ThreadPool.UnsafeQueueUserWorkItem(
                                new WaitCallback(ReceiveBitEventHander), me);
                    }
                }
                else
                {
                    int lle = workItem.allDataList.Length;

                    byte[] temp = new byte[lle + tempbtye.Length];
                    Array.Copy(workItem.allDataList, 0, temp, 0, workItem.allDataList.Length);
                    Array.Copy(tempbtye, 0, temp, lle, bytesRead);
                    workItem.allDataList = temp; 
                    workItem.allDataList = PackageData(workItem.allDataList, workItem.SocketSession);
                }

                workItem.IsPage = false;
            }
            catch
            {
            }
        }

        #region 发送

        public bool Send(int index, byte command, string text)
        {
            try
            {
                Socket socket = weaveNetworkItems[index].SocketSession;
                byte[] sendb = Encoding.UTF8.GetBytes(text);
                byte[] lens = Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);

                Send(socket, b);
            }
            catch { return false; }
            return true;
        }

        public bool Send(Socket socket, byte command, string text)
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
                int slen = 40960;
                if (socketLisener.ProtocolType == ProtocolType.Udp)
                    slen = 520;
                int count = (b.Length <= slen ? b.Length / slen : (b.Length / slen) + 1);
                Send(socket, b);
            }
            catch { return false; }
            return true;
        }

        private void SendDataEnd(IAsyncResult ar)
        {
            try
            {
                ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch
            {
            }
        }

        AsyncCallback acallsend;
        public bool Send(Socket socket, byte[] text)
        {
            try
            {
                lock (socket)
                {
                    socket.BeginSend(text, 0, text.Length, SocketFlags.None, acallsend, socket);
                }
                return true;
            }
            catch
            { return false; }
        }

        public bool Send(Socket socket, byte command, byte[] text)
        {
            try
            {
                int slen = 40960;
                if (socketLisener.ProtocolType == ProtocolType.Udp)
                    slen = 520;
                byte[] sendb = text;
                byte[] lens = ConvertToByteList(sendb.Length);
                byte[] b = new byte[2 + 2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                CRC.ConCRC(ref b, 2 + lens.Length);
                sendb.CopyTo(b, 2 + 2 + lens.Length);
                int count = (b.Length <= slen ? b.Length / slen : (b.Length / slen) + 1);
                Send(socket, b);
            }
            catch { return false; }
            return true;
        }
        #endregion

        public int Partition = 20000;
        void ReceiveHander(object ias)
        {
            while (true)
            {
                try
                {
                    int c = weaveNetworkItems.Count;
                    int count = (c / Partition) + 1;
                    if (c > 0)
                    {
                        Getbuffer(weaveNetworkItems, 0, c);
                    }
                }
                catch { }
            }
        }

        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len);

        void Getbuffer(List<WeaveNetWorkItems> netlist, int index, int len)
        {
            for (int i = index; i < len; i++)
            {
                if (i >= netlist.Count)
                    return;
                try
                {
                    WeaveNetWorkItems netc = netlist[i];
                    if (netc.SocketSession != null)
                    {
                        if (netc.SocketSession.Available > 0 && !netc.IsPage)
                        {
                            netc.IsPage = true;
                            netc.SocketSession.BeginReceive(netc.Buffer = new byte[netc.SocketSession.Available], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback), netc);

                        }
                        else if (netc.allDataList.Length > 0 && !netc.IsPage)
                        {
                            netc.IsPage = true;
                            netc.SocketSession.BeginReceive(netc.Buffer = new byte[0], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback), netc);
                        }
                    }
                }
                catch
                { }
            }
        }

        void AcceptHander(object ias)
        {
            while (true)
            {
                Socket handler = socketLisener.Accept();
                //连接到服务器的客户端Socket封装类
                WeaveNetWorkItems netc = new WeaveNetWorkItems
                {
                    SocketSession = handler
                };
                weaveNetworkItems.Add(netc);

                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(UpdateSocketListEventHander),
                      handler);
            }
        }
    }
}
