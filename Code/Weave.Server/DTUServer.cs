using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Weave.Base;
namespace Weave.Server
{

    /// <summary>
    /// DTU服务器类
    /// </summary>
    public class DTUServer  
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<WeaveNetWorkItems> netWorkList = new List<WeaveNetWorkItems>();
        public event WeaveReceiveDtuEvent weaveReceiveDtuEvent;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
        string loaclip;
        public DTUServer(string _loaclip)
        {
            loaclip = _loaclip;
        }
        public DTUServer()
        {
        }
        public void start(int port)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(localEndPoint);
            socket.Listen(1000000);
            Thread AcceptHanderThread = new Thread(new ParameterizedThreadStart(AcceptHander));
            Thread ReceiveHanderThread = new Thread(new ParameterizedThreadStart(ReceiveHander));
            Thread ReceivePageHanderThread = new Thread(new ParameterizedThreadStart(ReceivePageHander));
            Thread KeepAliveThread = new Thread(new ParameterizedThreadStart(KeepAlive));
            AcceptHanderThread.Start();
            ReceiveHanderThread.Start();
            ReceivePageHanderThread.Start();
            KeepAliveThread.Start();
        }

        /// <summary>
        /// 获取连接到服务器的客户端Socket数量
        /// </summary>
        /// <returns></returns>
        public int GetNetWorkListCount()
        {
            return netWorkList.Count;
        }
        public void KeepAlive(object obj)
        {
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(8000);
                    //  ArrayList al = new ArrayList();
                    // al.Clone()
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[netWorkList.Count];
                    netWorkList.CopyTo(netlist);
                    foreach (WeaveNetWorkItems netc in netlist)
                    {
                        try
                        {
                            byte[] b = new byte[1]  ;
                            netc.SocketSession.Send(b);
                        }
                        catch
                        {
                            try
                            {
                                netc.SocketSession.Close();
                            }
                            catch { }
                            ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteSocketListHander), netc.SocketSession);
                            netWorkList.Remove(netc);
                        }
                    }
                    GC.Collect();
                }
                catch { }
            }
        }
        private void DeleteSocketListHander(object state)
        {
            weaveDeleteSocketListEvent?.Invoke(state as Socket);
        }
        private void UpdateSocketListHander(object state)
        {
            weaveUpdateSocketListEvent?.Invoke(state as Socket);
        }
        private void PackageData(object obj)
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
                    try
                    {
                        DtuModel dd = new DtuModel();
                        dd.Data = tempbtye;
                        dd.Soc = netc.SocketSession;
                        if (weaveReceiveDtuEvent != null)
                            ThreadPool.QueueUserWorkItem(new WaitCallback(DtuReceiveEventCallBack), dd);
                       // receiveeventDtu.BeginInvoke(tempbtye, netc.Soc, null, null);
                        if (ListData.Count > 0) ListData.RemoveAt(i);
                        netc.IsPage = false; return;
                    }
                    catch //(Exception e)
                    {
                        netc.IsPage = false; return;
                    }
                }
            }
            catch //(Exception e)
            {
                if (netc.DataList.Count > 0)
                    netc.DataList.RemoveAt(0);
                netc.IsPage = false;
                return;
            }
            finally { netc.IsPage = false; }
        }
        private void DtuReceiveEventCallBack(object state)
        {
            DtuModel dd = state as DtuModel;
            weaveReceiveDtuEvent(dd.Data, dd.Soc);
        }
        private void ReadCallback(IAsyncResult ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar.AsyncState;
            Socket handler = netc.SocketSession;
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
                    netWorkList.Remove(netc);
                }
                bytesRead = netc.Buffer.Length;
                byte[] tempbtye = new byte[netc.Buffer.Length];
                //netc.Buffer.CopyTo(tempbtye, 0);
                if (bytesRead > 0)
                {
                    try
                    {
                        Array.Copy(netc.Buffer, 0, tempbtye, 0, bytesRead);
                       // handler.Send(tempbtye);
                        netc.DataList.Add(tempbtye);
                    }
                    catch
                    { }
                }
            }
            catch(Exception e)
            {
            } 
        }
        public bool send(int index, byte command, string text)
        {
            try
            {
                Socket soc = netWorkList[index].SocketSession;
                byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                soc.Send(b);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        public bool send(Socket soc, byte command, string text)
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
                soc.Send(b);
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
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[netWorkList.Count];
                    netWorkList.CopyTo(netlist);
                    foreach (WeaveNetWorkItems netc in netlist)
                    {
                        if (netc.DataList.Count > 0)
                        {
                            if (!netc.IsPage)
                            {
                                netc.IsPage = true;
                                ThreadPool.QueueUserWorkItem(new WaitCallback(PackageData), netc);
                                //System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(PackageData));
                                //t.Start(netc);
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(10);
                }
                catch { }
            }
        }
        void ReceiveHander(object ias)
        {
            while (true)
            {
                try
                {
                    int c = netWorkList.Count;
                    WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                    netWorkList.CopyTo(0, netlist, 0, c);
                    getbuffer(netlist, 0, c);
                }
                catch { }
                System.Threading.Thread.Sleep(10);
            }
        }
        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len);
        void getbuffer(WeaveNetWorkItems[] netlist, int index, int len)
        {
            for (int i = index; i < len; i++)
            {
                WeaveNetWorkItems netc = netlist[i];
                try
                {
                    if (netc.SocketSession != null)
                    {
                        if (netc.SocketSession.Available > 0)
                        {
                            netc.SocketSession.BeginReceive(netc.Buffer = new byte[netc.SocketSession.Available], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback), netc);
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
                Socket handler = socket.Accept();
                WeaveNetWorkItems netc = new WeaveNetWorkItems();
                netc.SocketSession = handler;
                netWorkList.Add(netc);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdateSocketListHander), handler);
            }
        }
    }
}
