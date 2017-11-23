using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WeaveBase.Helper;

namespace WeaveBase
{
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
            Port = port;
            if (weaveDataType == WeaveDataTypeEnum.Json && waveReceiveEvent == null)
                throw new Exception("没有注册receiveevent事件");
            if (weaveDataType == WeaveDataTypeEnum.Bytes && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");

            //将指定的 Socket 选项设置为指定的 Boolean 值。
            // 请注意这一句。ReuseAddress选项设置为True将允许将套接字绑定到已在使用中的地址
            socketLisener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socketLisener.Bind(localEndPoint);
            //backlog 参数指定队列中最多可容纳的等待接受的传入连接数
            socketLisener.Listen(1000000);

            // ParameterizedThreadStart(object state)，使用这个这个委托定义的线程的启动函数可以接受一个输入参数
            Thread ThreadAcceptHander = new Thread(new ParameterizedThreadStart(AcceptHander));
            Thread ThreadReceiveHander = new Thread(new ParameterizedThreadStart(ReceiveHander));
            Thread ThreadReceivePageHander = new Thread(new ParameterizedThreadStart(ReceivePageHander));
            Thread ThreadKeepAliveHander = new Thread(new ParameterizedThreadStart(this.KeepAliveHander));
            ThreadAcceptHander.Start();
            ThreadReceiveHander.Start();
            ThreadReceivePageHander.Start();
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
                        Thread.Sleep(1);
                        try
                        {

                            byte[] b = new byte[] { 0x99 };
                            //初始化连接，这里 报错，socket没有，客户端已经异常断开了，如断电，断网，，
                            workItem.SocketSession.Send(b);
                            workItem.ErrorNum = 0;
                        }
                        catch
                        {
                            workItem.ErrorNum += 1;
                            if (workItem.ErrorNum > 3)
                            {
                                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteSocketListEventHander), workItem.SocketSession);
                               
                              
                                weaveNetworkItems.Remove(workItem);
                            }
                        }
                    }
                    Thread.Sleep(5000);
                    // GC.Collect();
                }
                catch { }
            }
        }


        /// <summary>
        /// 客户端发送过来信息，表示我要主动断开连接，我点击了结束游戏，告诉你一声 
        /// </summary>
        /// <param name="_client"></param>
        public void CliendSendDisConnectedEvent(Socket _client)
        {
            WeaveNetWorkItems[] workItems = new WeaveNetWorkItems[weaveNetworkItems.Count];
            weaveNetworkItems.CopyTo(workItems);
            foreach (WeaveNetWorkItems workItem in workItems)
            {
                if (workItem == null)
                    continue;
                Thread.Sleep(1);
                try
                {
                    if (workItem.SocketSession == _client)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteSocketListEventHander), workItem.SocketSession);


                        weaveNetworkItems.Remove(workItem);
                    }
                }
                catch
                {
                   
                }
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
      
        private void packageData(object obj)
        {
            WeaveNetWorkItems netc = obj as WeaveNetWorkItems;
            List<byte[]> ListData = netc.DataList;
            try
            {
                int i = 0;
                int count = ListData.Count;
                if (count > 0)
                {
                    int bytesRead = ListData[i] != null ? ListData[i].Length : 0;
                    if (bytesRead == 0)
                    {
                        if (ListData.Count > 0) ListData.RemoveAt(0);
                        netc.IsPage = false; return;
                    };
                    byte[] tempbtye = new byte[bytesRead];
                    Array.Copy(ListData[i], tempbtye, tempbtye.Length);
                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (bytesRead > 2 + a)
                        {
                            int len = 0;
                            if (weaveDataType == WeaveDataTypeEnum.Bytes)
                            {
                                byte[] bb = new byte[a];
                                Array.Copy(tempbtye, 2, bb, 0, a);
                                len = WeaveServerHelper.ConvertToInt(bb);
                            }
                            else
                            {
                                String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                                len = int.Parse(temp);
                            }
                            if ((len + 2 + a) > tempbtye.Length)
                            {
                                try
                                {
                                    if (ListData.Count > 1)
                                    {
                                        ListData.RemoveAt(i);
                                        byte[] temps = new byte[tempbtye.Length];
                                        Array.Copy(tempbtye, temps, temps.Length);
                                        byte[] tempbtyes = new byte[temps.Length + ListData[i].Length];
                                        Array.Copy(temps, tempbtyes, temps.Length);
                                        Array.Copy(ListData[i], 0, tempbtyes, temps.Length, ListData[i].Length);
                                        ListData[i] = tempbtyes;
                                    }
                                }
                                catch
                                {
                                }
                                netc.IsPage = false; return;
                            }
                            else if (tempbtye.Length > (len + 2 + a))
                            {
                                try
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 2 + a)];
                                    Array.Copy(tempbtye, (len + 2 + a), temps, 0, temps.Length);
                                    ListData[i] = temps;
                                }
                                catch
                                { }
                                // netc.Ispage = false; return;
                            }
                            else if (tempbtye.Length == (len + 2 + a))
                            { if (ListData.Count > 0) ListData.RemoveAt(i); }
                            try
                            {
                                if (weaveDataType == WeaveDataTypeEnum.Json)
                                {
                                    String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                    WeaveEvent me = new WeaveEvent();
                                    me.Command = tempbtye[0];
                                    me.Data = temp;
                                    me.Soc = netc.SocketSession;
                                    if (waveReceiveEvent != null)
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveEventHander), me);
                                    //receiveeventto(me);
                                    //if (receiveevent != null)
                                    //    receiveevent.BeginInvoke(tempbtye[0], temp, netc.Soc, null, null);
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
                                    me.Soc = netc.SocketSession;
                                    if (weaveReceiveBitEvent != null)
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveBitEventHander), me);
                                }
                                netc.IsPage = false; return;
                            }
                            catch (Exception ex)
                            {
                                string ems = ex.Message;
                                netc.IsPage = false; return;
                            }
                        }
                        else
                        {
                            if (ListData.Count > 0)
                            {
                                ListData.RemoveAt(i);
                                byte[] temps = new byte[tempbtye.Length];
                                Array.Copy(tempbtye, temps, temps.Length);
                                byte[] tempbtyes = new byte[temps.Length + ListData[i].Length];
                                Array.Copy(temps, tempbtyes, temps.Length);
                                Array.Copy(ListData[i], 0, tempbtyes, temps.Length, ListData[i].Length);
                                ListData[i] = tempbtyes;
                            }
                            netc.IsPage = false; return;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (ListData.Count > 1)
                            {
                                ListData.RemoveAt(i);
                                byte[] temps = new byte[tempbtye.Length];
                                Array.Copy(tempbtye, temps, temps.Length);
                                byte[] tempbtyes = new byte[temps.Length + ListData[i].Length];
                                Array.Copy(temps, tempbtyes, temps.Length);
                                Array.Copy(ListData[i], 0, tempbtyes, temps.Length, ListData[i].Length);
                                ListData[i] = tempbtyes;
                            }
                        }
                        catch
                        {
                        }
                        netc.IsPage = false; return;
                    }
                }
            }
            catch (Exception ex)
            {
                string ems = ex.Message;
                if (netc.DataList.Count > 0)
                    netc.DataList.RemoveAt(0);
                netc.IsPage = false;
                return;
            }
            finally { netc.IsPage = false; }
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
                    //EndReceive结束挂起的异步读取
                    /*
                     如果您使用的是面向连接的协议，则 EndReceive 方法将读取所有可用的数据，
                     直到达到 BeginReceive 方法的 size 参数所指定的字节数为止。
                     如果远程主机使用 Shutdown 方法关闭了 Socket 连接，
                     并且所有可用数据均已收到，则 EndReceive 方法将立即完成并返回零字节。
                     */
                }
                catch
                {
                    //netc.Soc.Close();
                    //listconn.Remove(netc);
                }
                byte[] tempbtye = new byte[bytesRead];
                if (bytesRead > 0)
                {
                    Array.Copy(workItem.Buffer, 0, tempbtye, 0, bytesRead);
                    workItem.DataList.Add(tempbtye);
                }
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
                Socket socket = weaveNetworkItems[index].SocketSession;
                //byte[] sendb = Encoding.UTF8.GetBytes(text);
                //byte[] lens = Encoding.UTF8.GetBytes(sendb.Length.ToString());
                //byte[] b = new byte[2 + lens.Length + sendb.Length];
                //b[0] = command;
                //b[1] = (byte)lens.Length;
                //lens.CopyTo(b, 2);
                //sendb.CopyTo(b, 2 + lens.Length);
                byte[] b = WeaveServerHelper.CodingProtocol(command, text);

                socket.Send(b);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        public bool Send(Socket socket, byte command, string text)
        {
            try
            {
                //byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                //byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                //byte[] b = new byte[2 + lens.Length + sendb.Length];
                //b[0] = command;
                //b[1] = (byte)lens.Length;
                //从指定的目标数组索引处开始，将当前一维数组的所有元素复制到指定的一维数组中 ,将lens所有元素复制到b中，从b的第2个元素开始
                //lens.CopyTo(b, 2);
                //将sendb所有元素复制到b中，从（2+lens长度）开始
                // sendb.CopyTo(b, 2 + lens.Length);

                //最终b的结构是[0] 是command的byte
                // [1]是text数据转换为byte后的长度length
                //[2]从2往后是实际的text数据转换为byte的数据

                byte[] b = WeaveServerHelper.CodingProtocol(command, text);

                int slen = 40960;  //40kb左右
                if (socketLisener.ProtocolType == ProtocolType.Udp)
                    slen = 520;

                int count = (b.Length <= slen ? b.Length / slen : (b.Length / slen) + 1);
                if (count == 0)
                {
                    socket.Send(b);
                    
                }
                else
                {
                    //只有在slen 大于40959,约40kb 才会进入这个方法，实现数据分页发送
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * slen) > slen ? slen : b.Length - (i * slen);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * slen, temp, 0, zz);
                        socket.Send(temp);
                        System.Threading.Thread.Sleep(1);
                    }
                }
             
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        public bool Send(Socket socket, byte command, byte[] text)
        {
            try
            {
                int slen = 40960;
                if (socketLisener.ProtocolType == ProtocolType.Udp)
                    slen = 520;

                //byte[] sendb = text;
                //byte[] lens = WeaveServerHelper.ConvertToByteList(sendb.Length);
                //byte[] b = new byte[2 + lens.Length + sendb.Length];
                //b[0] = command;
                //b[1] = (byte)lens.Length;
                //lens.CopyTo(b, 2);
                //sendb.CopyTo(b, 2 + lens.Length);
                //最终b的结构是[0] 是command的byte
                // [1]是text数据转换为byte后的长度length
                //[2]从2往后是实际的text数据转换为byte的数据
                byte[] b = WeaveServerHelper.CodingProtocol(command, text);

                int count = (b.Length <= slen ? b.Length / slen : (b.Length / slen) + 1);
                if (count == 0)
                {
                    socket.Send(b);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * slen) > slen ? slen : b.Length - (i * slen);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * slen, temp, 0, zz);
                        socket.Send(temp);
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        void ReceivePageHander(object ias)
        {
            while (true)
            {
                try
                {
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[weaveNetworkItems.Count];
                    weaveNetworkItems.CopyTo(netlist);
                    foreach (WeaveNetWorkItems netc in netlist)
                    {
                        if (netc.DataList.Count > 0)
                        {
                            if (!netc.IsPage)
                            {
                                netc.IsPage = true;
                                //将方法排入队列以便执行
                                ThreadPool.QueueUserWorkItem(new WaitCallback(packageData), netc);
                                //System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(packageData));
                                //t.Start(netc);
                                //Webp2psever.packageDataHandler pdh = new Webp2psever.packageDataHandler(packageData);
                                //pdh.BeginInvoke(netc, endpackageData, null);
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(10);
                }
                catch { }
            }
        }
        //void endpackageData(IAsyncResult ia)
        //{
        //    ia.AsyncState
        //}
        //public int Partition=20000;
        //void receive(object ias)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            int c = listconn.Count;
        //            int count = (c / Partition) + 1;
        //            getbufferdelegate[] iagbd = new getbufferdelegate[count];
        //            IAsyncResult[] ia = new IAsyncResult[count];
        //            if (c > 0)
        //            {
        //                for (int i = 0; i < count; i++)
        //                {
        //                    c = c - (i * Partition) > Partition ? Partition : c - (i * Partition);
        //                    NETcollection[] netlist = new NETcollection[c];
        //                    listconn.CopyTo(i * Partition, netlist, 0, c);
        //                      iagbd[i] = new getbufferdelegate(getbuffer);
        //                    ia[i]= iagbd[i].BeginInvoke(netlist, 0, Partition, null, null);
        //                }
        //                for (int i = 0; i < count; i++)
        //                {
        //                    iagbd[i].EndInvoke(ia[i]);
        //                }
        //            }
        //            //NETcollection[] netlist = new NETcollection[c];
        //            //listconn.CopyTo(0, netlist, 0, c);
        //            //getbuffer(netlist, 0, c);
        //        }
        //        catch { }
        //        System.Threading.Thread.Sleep(1);
        //    }
        //}
        public int Partition = 20000;
        void ReceiveHander(object ias)
        {
            while (true)
            {
                try
                {
                    int c = weaveNetworkItems.Count;
                    int count = (c / Partition) + 1;
                    getbufferdelegate[] iagbd = new getbufferdelegate[count];
                    IAsyncResult[] ia = new IAsyncResult[count];
                    if (c > 0)
                    {
                        WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                        weaveNetworkItems.CopyTo(0, netlist, 0, c);
                        getbuffer(netlist, 0, Partition);
                   
                    }
                 
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
        }
        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len);
        void getbuffer(WeaveNetWorkItems[] netlist, int index, int len)
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
                        //Available获取已经从网络接收且可供读取的数据量
                        if (netc.SocketSession.Available > 0)
                        {
                            //开始异步接收数据从连接 Socket。
                            netc.SocketSession.BeginReceive(netc.Buffer = new byte[netc.SocketSession.Available], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback), netc);

                            /*
                             异步 BeginReceive 操作必须通过调用 EndReceive 方法。 通常情况下，由调用该方法 callback 委托。
                            此方法不会阻止，直到该操作已完成。 若要阻止在完成该操作之前，请使用之一 Receive 方法重载。
                           若要取消挂起 BeginReceive, ，调用 Close 方法。
                               */

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
                /*
                 Accept 以同步方式从侦听套接字的连接请求队列中提取第一个挂起的连接请求，然后创建并返回一个新 Socket。 
                 不能使用此返回 Socket 以接受来自连接队列的任何其他连接。 
                 但是，你可以调用 RemoteEndPoint 方法所返回的 Socket 来标识远程主机的网络地址和端口号。
在阻止模式下， Accept 被阻止，直至传入的连接尝试将会排队。
一旦接受连接，原始 Socket 继续队列的传入连接请求，直到您关闭则。
                 */
                Socket handler = socketLisener.Accept();
                 
                WeaveNetWorkItems netc = new WeaveNetWorkItems();
                netc.SocketSession = handler;
                weaveNetworkItems.Add(netc);
                //将方法排入队列以便执行。 此方法在有线程池线程变得可用时执行。
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdateSocketListEventHander), handler);
                //WaitCallback表示要由线程池线程执行的回调方法

                //系统资源释放慢了
                System.Threading.Thread.Sleep(150);
            }
        }
    }
}
