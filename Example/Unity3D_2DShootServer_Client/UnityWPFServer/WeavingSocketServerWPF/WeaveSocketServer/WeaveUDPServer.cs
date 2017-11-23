using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using WeaveBase;

namespace WeaveSocketServer
{
    public class WeaveUDPServer 
    {
        WeaveDataTypeEnum weaveDataType = WeaveDataTypeEnum.Json;
        String loaclip = "127.0.0.1";
           Socket socketLisener = null;
        List<WeaveNetWorkItems> weaveNetworkItems = new List<WeaveNetWorkItems>();
        public event WaveReceivedupEvent waveReceiveEvent;
        public event WeaveReceiveBitdupEvent weaveReceiveBitEvent;
       
        public event WeaveUpdateudpListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteudpListEvent weaveDeleteSocketListEvent;

        public int Port
        {
            get;
            set;
        }

        public WeaveUDPServer()
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public WeaveUDPServer(string _loaclip) 
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.loaclip = _loaclip;
        }
        public WeaveUDPServer(WeaveDataTypeEnum weaveDataType)  
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.weaveDataType = weaveDataType;
        }

        public void Start(int port)
        {
            Port = port;
            if (weaveDataType == WeaveDataTypeEnum.Json && waveReceiveEvent == null)
                throw new Exception("没有注册receiveevent事件");
            if (weaveDataType == WeaveDataTypeEnum.Bytes && weaveReceiveBitEvent == null)
                throw new Exception("没有注册receiveeventbit事件");
            socketLisener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socketLisener.Bind(localEndPoint);
            //socketLisener.Listen(1000000);
            Thread ThreadAcceptHander = new Thread(new ParameterizedThreadStart(AcceptHander));
            Thread ThreadReceiveHander = new Thread(new ParameterizedThreadStart(ReceiveHander));
            Thread ThreadReceivePageHander = new Thread(new ParameterizedThreadStart(ReceivePageHander));
            Thread ThreadKeepAliveHander = new Thread(new ParameterizedThreadStart(this.KeepAliveHander));
            ThreadAcceptHander.Start();
            ThreadReceiveHander.Start();
            ThreadReceivePageHander.Start();
            ThreadKeepAliveHander.Start();
        }

        private void AcceptHander(object obj)
        {
            //throw new NotImplementedException();
        }

        private void ReceiveHander(object obj)
        {
            while (true)
            {
                try
                {
                    if (socketLisener.Available > 0)
                    {
                        WeaveNetWorkItems netc = new WeaveNetWorkItems();
                        byte[] b = new byte[socketLisener.Available];
                        netc.Buffer = b;
                         IPEndPoint server = new IPEndPoint(IPAddress.Any,Port);
                        EndPoint ep = (EndPoint)server;
                        socketLisener.BeginReceiveFrom(b, 0, b.Length,SocketFlags.None,ref ep, new AsyncCallback(ReadCallback), netc);

                    }

                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
        }
        //private static readonly ConditionalWeakTable<Socket, StrongBox<EndPoint>> _extensionField_f=new ConditionalWeakTable<Socket, StrongBox<EndPoint>>();
        //  EndPoint get_f(Socket obj)
        //{
        //    StrongBox<EndPoint> box;
        //    if (!_extensionField_f.TryGetValue(obj, out box))
        //        return default(EndPoint);
        //    return box.Value;
        //}
        //  void set_f(Socket obj, EndPoint value)
        //{
        //    StrongBox<EndPoint> box = _extensionField_f.GetOrCreateValue(obj);
        //    box.Value = value;
        //}
        // void Remove(Socket obj)
        //{
        //    _extensionField_f.Remove(obj);
           
        //}
        private void ReadCallback(IAsyncResult ar)
        {
            
            int bytesRead = 0;
            IPEndPoint server = new IPEndPoint(IPAddress.Any, Port);
            EndPoint ep = (EndPoint)server;
            bytesRead = socketLisener.EndReceiveFrom(ar, ref ep);
            WeaveNetWorkItems workItem = (WeaveNetWorkItems)ar.AsyncState;
           
            // Socket handler = workItem.SocketSession;
            workItem.Ep = ep;
            try
            {
               
                
                byte[] tempbtye = new byte[bytesRead];
                if (bytesRead > 0)
                {
                    Array.Copy(workItem.Buffer, 0, tempbtye, 0, bytesRead);
                    var query = weaveNetworkItems.Where(p => p.Ep.Equals(ep));
                    foreach (WeaveNetWorkItems wnw in query)
                    {
                        if (tempbtye.Length == 1 && tempbtye[0] == 0x99)
                        {
                            wnw.Lasttime = DateTime.Now;
                            return;
                        }
                        wnw.DataList.Add(tempbtye);
                        return;
                    }
                    ;

                    // Socket soc = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);


                    //  set_f(soc, ep);
                    //EndPoint ee = get_f(soc);
                    //System.Windows.Forms.MessageBox.Show(ee.ToString());
                    if (weaveUpdateSocketListEvent != null)
                    {
                        weaveUpdateSocketListEvent(ep);
                    }
                    workItem.Lasttime = DateTime.Now;
                    workItem.Ep = ep;
                    workItem.DataList.Add(tempbtye);
                    weaveNetworkItems.Add(workItem);
                    return;
                }
            }
            catch
            {
            }
        }

        private void ReceivePageHander(object obj)
        {
            // throw new NotImplementedException();
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
                        netc.Lasttime = DateTime.Now;
                        int a = tempbtye[1];
                        if (bytesRead > 2 + a)
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
                                    me.Ep = netc.Ep;
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
                                    me.Ep = netc.Ep;
                                    if (weaveReceiveBitEvent != null)
                                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveBitEventHander), me);
                                }
                                netc.IsPage = false; return;
                            }
                            catch 
                            {
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
            catch 
            {
                if (netc.DataList.Count > 0)
                    netc.DataList.RemoveAt(0);
                netc.IsPage = false;
                return;
            }
            finally { netc.IsPage = false; }
        }

        void ReceiveEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            waveReceiveEvent(me.Command, me.Data, me.Ep);
        }
        void ReceiveBitEventHander(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            weaveReceiveBitEvent(me.Command, me.Databit, me.Ep);
        }

        public int GetNetworkItemCount()
        {
            return weaveNetworkItems.Count;
            //throw new NotImplementedException();
            //return 0;
        }

        public void KeepAliveHander(object obj)
        {
            // throw new NotImplementedException();
            // Remove()
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
                            EndPoint ep = workItem.Ep;
                            byte[] b = new byte[] { 0x99 };
                            socketLisener.SendTo(b, ep);
                            if ((DateTime.Now - workItem.Lasttime).TotalSeconds > 90)
                            { 
                                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteSocketListEventHander), workItem.Ep);

                                //  Remove(workItem.SocketSession);
                                weaveNetworkItems.Remove(workItem);
                            }
                        }
                        catch
                        {
                            workItem.ErrorNum += 1;
                          
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
            weaveDeleteSocketListEvent?.Invoke(state as EndPoint);
            
        }

        
        public bool Send(EndPoint ep, byte command, string text)
        {
            try
            {
               
                EndPoint ee = ep;

                if (ee == null)
                    return false;
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

                    socketLisener.SendTo(b, ee);

                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * slen) > slen ? slen : b.Length - (i * slen);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * slen, temp, 0, zz);
                        socketLisener.SendTo(temp,ee);
                        System.Threading.Thread.Sleep(100);
                    }
                }

            }
            catch {
                return false; }
            // tcpc.Close();
            return true;
        }

        public bool Send(EndPoint ep, byte command, byte[] text)
        {
            try
            {
                EndPoint ee = ep;
                if (ee == null)
                    return false;
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
                    socketLisener.SendTo(b, ee);
                }
                else
                {
                    throw new Exception( "发送数据不得大于520byte");
                    //return false;
                    //for (int i = 0; i < count; i++)
                    //{
                    //    int zz = b.Length - (i * slen) > slen ? slen : b.Length - (i * slen);
                    //    byte[] temp = new byte[zz];
                    //    Array.Copy(b, i * slen, temp, 0, zz);
                    //    socketLisener.SendTo(temp, ee);
                    //    System.Threading.Thread.Sleep(100);
                    //}
                }
            }
            catch { return false; }
            // tcpc.Close();
            return true;
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
    }
}
