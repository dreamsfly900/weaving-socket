using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public WeaveDataTypeEnum weaveDataType
        {
            get; set;
        }
        protected Socket socketLisener = null;
        List<WeaveNetWorkItems> weaveNetworkItems = new List<WeaveNetWorkItems>();
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
            this.weaveDataType = weaveDataType;
        }
        public void Start(int port)
        {
            acallsend = new AsyncCallback(SendDataEnd);
            Port = port;
            if (weaveDataType == WeaveDataTypeEnum.Json && waveReceiveEvent == null)
                throw new Exception("没有注册receiveevent事件");
            if (weaveDataType == WeaveDataTypeEnum.Bytes && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            if (weaveDataType == WeaveDataTypeEnum.custom && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            socketLisener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socketLisener.Bind(localEndPoint);
            socketLisener.Listen(1000000);
            Thread ThreadAcceptHander = new Thread(new ParameterizedThreadStart(AcceptHander));
            Thread ThreadReceiveHander = new Thread(new ParameterizedThreadStart(ReceiveHander));
          //  Thread ThreadReceivePageHander = new Thread(new ParameterizedThreadStart(ReceivePageHander));
            Thread ThreadKeepAliveHander = new Thread(new ParameterizedThreadStart(this.KeepAliveHander));
            ThreadAcceptHander.Start();
            ThreadReceiveHander.Start();
          //  ThreadReceivePageHander.Start();
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
                      //  Thread.Sleep(1);
                        try
                        {
                            byte[] b = new byte[] { 0x99 };
                            if (weaveDataType == WeaveDataTypeEnum.custom)
                                b = new byte[1];
                            if (!Send(workItem.SocketSession, b))
                            {
                                workItem.ErrorNum += 1;
                                if (workItem.ErrorNum > 3)
                                {
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(DeleteSocketListEventHander), workItem.SocketSession);


                                    weaveNetworkItems.Remove(workItem);
                                }
                            }
                            else
                            {
                                workItem.ErrorNum = 0;
                            }

                            // workItem.SocketSession.Send(b);
                           
                        }
                        catch
                        {
                            workItem.ErrorNum += 1;
                            if (workItem.ErrorNum > 3)
                            {
                                System.Threading.ThreadPool.UnsafeQueueUserWorkItem(
                                    new System.Threading.WaitCallback(DeleteSocketListEventHander),
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
                    // GC.Collect();
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

        /// <summary>
        /// 对粘包，分包的处理方法
        /// </summary>
        /// <param name="obj"></param>
        private byte [] packageData(byte[] alldata,Socket soc)
        {
            //WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            //List<byte[]> ListData = netc.DataList;
            try
            {
                //int i = 0;
                //int count = ListData.Count;
                //  if (count > 0)
                {
                    lb1122:
                    int bytesRead = alldata.Length;
                    if (bytesRead == 0)
                    {
                        //  if (ListData.Count > 0) ListData.RemoveAt(0);
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
                            if (weaveDataType == WeaveDataTypeEnum.Bytes)
                            {
                                byte[] bb = new byte[a];
                                Array.Copy(tempbtye, 2, bb, 0, a);
                                len = ConvertToInt(bb);
                            }
                            else
                            {
                                String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                                len = int.Parse(temp);
                            }

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
                                if (weaveDataType == WeaveDataTypeEnum.Json)
                                {
                                    String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                    WeaveEvent me = new WeaveEvent();
                                    me.Command = tempbtye[0];
                                    me.Data = temp;
                                    me.Soc =soc;
                                    if (waveReceiveEvent != null)
                                        //  waveReceiveEvent?.Invoke(tempbtye[0], temp, soc);
                                         System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(ReceiveEventHander), me);
                                        //receiveeventto(me);
                                        //if (receiveevent != null)
                                       // waveReceiveEvent.BeginInvoke(tempbtye[0], temp, soc, null, null);
                                    //if (ListData.Count > 0) ListData.RemoveAt(i);
                                }
                                else if (weaveDataType == WeaveDataTypeEnum.Bytes)
                                {
                                    //  temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                    byte[] bs = new byte[len];
                                    Array.Copy(tempbtye, (2 + a), bs, 0, bs.Length);
                                    WeaveEvent me = new WeaveEvent();
                                    me.Command = tempbtye[0];
                                    me.Data = "";
                                    me.Databit = bs;
                                    me.Soc = soc;
                                    if (weaveReceiveBitEvent != null)
                                          System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new System.Threading.WaitCallback(ReceiveBitEventHander), me);
                                       // weaveReceiveBitEvent?.Invoke(tempbtye[0], bs, soc);
                                        //weaveReceiveBitEvent?.BeginInvoke(tempbtye[0], bs, soc,null,null);
                                }
                                //netc.IsPage = false;
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
        void allpack(object obj)
        {
            WeaveNetWorkItems workItem = (WeaveNetWorkItems)obj;
            workItem.allDataList = packageData(workItem.allDataList, workItem.SocketSession);
            workItem.IsPage = false;
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
                    //netc.Soc.Close();
                    //listconn.Remove(netc);
                }
                bytesRead=workItem.Buffer.Length;
                byte[] tempbtye = new byte[bytesRead];
                //if (bytesRead > 0)
                {
                     
                    Array.Copy(workItem.Buffer, 0, tempbtye, 0, tempbtye.Length);
                    if (weaveDataType == WeaveDataTypeEnum.custom)
                    {
                        if (tempbtye.Length > 0)
                        {
                            WeaveEvent me = new WeaveEvent();
                            me.Command = defaultCommand;
                            me.Data = "";
                            me.Databit = tempbtye;
                            me.Soc = workItem.SocketSession;
                            if (weaveReceiveBitEvent != null)
                                // weaveReceiveBitEvent?.BeginInvoke(defaultCommand, tempbtye, workItem.SocketSession,null,null);
                            System.Threading.ThreadPool.UnsafeQueueUserWorkItem(
                                new System.Threading.WaitCallback(ReceiveBitEventHander), me);

                            //netc.IsPage = false;

                        }

                    }
                    else
                    {
                        int lle = workItem.allDataList.Length;

                        byte[] temp = new byte[lle + tempbtye.Length];
                        Array.Copy(workItem.allDataList, 0, temp, 0, workItem.allDataList.Length);
                        Array.Copy(tempbtye, 0, temp, lle, bytesRead);
                        workItem.allDataList = temp; //workItem.DataList.Add(tempbtye);
                        workItem.allDataList = packageData(workItem.allDataList, workItem.SocketSession);
                    }

                    workItem.IsPage = false;
                }
            }
            catch
            {
            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        }

        #region 发送

    
        public bool send(int index, byte command, string text)
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
            // tcpc.Close();
            return true;
        }
        public bool Send(Socket socket, byte command, string text)
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
                int slen = 40960;
                if (socketLisener.ProtocolType == ProtocolType.Udp)
                    slen = 520;
                int count = (b.Length <= slen ? b.Length / slen : (b.Length / slen) + 1);
                if (count == 0)
                {
                    Send( socket,b);
                    
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * slen) > slen ? slen : b.Length - (i * slen);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * slen, temp, 0, zz); 
                        Send(socket, temp);
                      //  System.Threading.Thread.Sleep(1);
                    }
                }
             
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        private  void SendDataEnd(IAsyncResult ar)
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
                socket.BeginSend(text, 0, text.Length, SocketFlags.None, acallsend, socket);
                //socket.Send(text);
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
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                int count = (b.Length <= slen ? b.Length / slen : (b.Length / slen) + 1);
                if (count == 0)
                {
                   // socket.Send(b);
                    Send(socket, b);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * slen) > slen ? slen : b.Length - (i * slen);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * slen, temp, 0, zz);
                        Send(socket, temp);
                      //  System.Threading.Thread.Sleep(1);
                    }
                }
            }
            catch { return false; }
            // tcpc.Close();
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
                    //getbufferdelegate[] iagbd = new getbufferdelegate[count];
                    //IAsyncResult[] ia = new IAsyncResult[count];
                    if (c > 0)
                    {
                        //WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                        //weaveNetworkItems.CopyTo(0, netlist, 0, c);
                        getbuffer(weaveNetworkItems, 0, c);
                   
                    }
                 
                }
                catch { }
              //  System.Threading.Thread.Sleep(1);
            }
        }
        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len);
        void getbuffer(List< WeaveNetWorkItems> netlist, int index, int len)
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
                          //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(allpack), netc);
                        }
                        //if (!netc.IsPage)
                        //{
                        //    netc.IsPage = true;
                        //    netc.IsPage = packageData(netc.allDataList, netc.SocketSession);
                        //}
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
                WeaveNetWorkItems netc = new WeaveNetWorkItems();
                netc.SocketSession = handler;
                weaveNetworkItems.Add(netc);
              
                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(UpdateSocketListEventHander),
                      handler );
                
            }
        }
    }
}
