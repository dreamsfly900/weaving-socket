using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Weave.Base.Interface;
using Weave.Base.WeaveBase;

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
        protected List<WeaveNetWorkItems> weaveNetworkItems = new List<WeaveNetWorkItems>();
        public event WaveReceiveEventEvent waveReceiveEvent;
        public int resttime = 0;
   //     public static ManualResetEvent allDone = new ManualResetEvent(false);
        public event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
        public event WeaveReceiveBitEvent weaveReceiveBitEvent;
        public event WeaveReceiveSslEvent WeaveReceiveSslEvent;
        void ReceiveToEventHanderssl(object obj)
        {
            WeaveEvent me = (WeaveEvent)obj;
            WeaveReceiveSslEvent?.Invoke(me.Command, me.Data, me.Ssl);
        }
        public byte defaultCommand = 0x0;
        public X509Certificate2 Certificate { get; set; }
        public SslProtocols EnabledSslProtocols { get; set; }
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
           // Console.WriteLine(weaveDataType.ToString());
            this.weaveDataType = weaveDataType;
        }
        WaitCallback ReceiveBitEventHandercback, ReceiveEventHandercback;
        public virtual void Start(int port)
        {
            acallsend = new AsyncCallback(SendDataEnd);
            ReceiveBitEventHandercback = new System.Threading.WaitCallback(ReceiveBitEventHander);
            ReceiveEventHandercback = new System.Threading.WaitCallback(ReceiveEventHander);
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
            System.Threading.ThreadPool.SetMaxThreads(100, 100);
            Thread ThreadAcceptHander = new Thread(new ParameterizedThreadStart(AcceptHander));
             ThreadReceiveHander = new Thread(new ParameterizedThreadStart(ReceiveHander));
            //Thread ThreadReceivePageHander = new Thread(new ParameterizedThreadStart(ReceivePageHander));
            Thread ThreadKeepAliveHander = new Thread(new ParameterizedThreadStart(this.KeepAliveHander));

            ThreadAcceptHander.Start();
            ThreadReceiveHander.Start();
           // ThreadReceivePageHander.Start();
            ThreadKeepAliveHander.Start();
        }
        Thread ThreadReceiveHander;
        protected virtual bool Setherd(WeaveNetWorkItems netc,int xintiao=0)
        {
            return true;
        }
        public int GetNetworkItemCount()
        {
            return weaveNetworkItems.Count;
        }
      protected virtual   void KeepAliveHander(object obj)
        {
          var  DeleteSocketListEventHandercallback =   new System.Threading.WaitCallback(DeleteSocketListEventHander);
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
                            var ok = false;
                            if (!Setherd(workItem, 1))
                            {
                                ok = true;
                            }
                            else
                            {
                                if (Certificate != null)
                                {
                                    if (weaveDataType == WeaveDataTypeEnum.custom)
                                        //    b = new byte[1];
                                        ok = Send(workItem.Stream, new byte[1]);
                                    else
                                        ok = Send(workItem.Stream, 0x99, b);
                                }
                                else
                                {
                                    if (weaveDataType == WeaveDataTypeEnum.custom)
                                        //    b = new byte[1];
                                        ok = Send(workItem.SocketSession, new byte[1]);
                                    else
                                        ok = Send(workItem.SocketSession, 0x99, b);
                                }
                            }
                           // ok = Send(workItem.SocketSession, new byte[1]);
                            if (!ok)
                                {
                                    workItem.ErrorNum += 1;
                                    if (workItem.ErrorNum >= 1)
                                    {
                                        System.Threading.ThreadPool.UnsafeQueueUserWorkItem(DeleteSocketListEventHandercallback, workItem.SocketSession);


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
                            if (workItem.ErrorNum >= 1)
                            {
                                System.Threading.ThreadPool.UnsafeQueueUserWorkItem(
                                   DeleteSocketListEventHandercallback,
                                    workItem.SocketSession);

                                try
                                {
                                    weaveNetworkItems.Remove(workItem);
                                }
                                catch (Exception EX_NAME)
                                {
                                   // Console.WriteLine(EX_NAME);
                                    //throw;
                                }



                            }
                        }
                    }
                    Thread.Sleep(5000);
                     GC.Collect();
                }
                catch { }
            }
        }



        protected void DeleteSocketListEventHander(object state)
        {
            weaveDeleteSocketListEvent?.Invoke(state as Socket);
            try { (state as Socket).Close();
                (state as Socket).Dispose();
            }
            catch { }
        }
        protected void UpdateSocketListEventHander(object state)
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
        encoder myencoder = new encoder();
        protected virtual byte[] packageData(byte[] alldata, Socket soc, SslStream ssl,byte[] tempDataList=null)
        {

            try
            {
                byte[] aall=new byte[0];
                if (weaveDataType == WeaveDataTypeEnum.Json)
                {
                    aall= myencoder.packageDatajson(alldata, soc, ReceiveEventHandercback, ssl, ReceiveToEventHanderssl);
                }
                else if (weaveDataType == WeaveDataTypeEnum.Bytes)
                {
                    aall= myencoder.packageDatabtye(alldata, soc, ReceiveBitEventHandercback, ssl, ReceiveToEventHanderssl);
                }
                if (weaveDataType == WeaveDataTypeEnum.custom)
                {
                    if (alldata.Length > 0)
                    {
                        //WeaveEvent me = new WeaveEvent();
                        //me.Command = defaultCommand;
                        //me.Data = "";
                        //me.Databit = alldata;
                        //me.Soc = soc;
                        if (weaveReceiveBitEvent != null)
                            weaveReceiveBitEvent?.Invoke(defaultCommand, alldata, soc);
                        //ReceiveBitEventHander(me);
                        //System.Threading.ThreadPool.UnsafeQueueUserWorkItem(
                        //   ReceiveBitEventHandercback, me);

                        //netc.IsPage = false;


                    }

                     aall=new byte[0];
                }
                //netc.IsPage = false;
                if (tempDataList != null && tempDataList.Length > 0)
                {
                    if (aall.Length == 0)
                        return tempDataList;
                    byte[]  tempbtyes = new byte[tempDataList.Length + aall.Length];
                    Array.Copy(tempDataList, 0, tempbtyes, 0, tempDataList.Length);
                    Array.Copy(aall, 0, tempbtyes, tempDataList.Length, tempbtyes.Length);
                    return tempbtyes;
                }
                //Console.WriteLine((soc.RemoteEndPoint as IPEndPoint).Address.ToString() + "+packageData：" + aall.Length);
                return aall;
            }
            catch (Exception e)
            {
               // Console.WriteLine("packageData:" + e.Message);
                // netc.IsPage = false;
                return new byte[0];
            }
            finally
            {
               
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
                    if (bytesRead <= 0)
                    {
                        if(workItem.allDataList.Length>3)
                          workItem.allDataList = packageData(workItem.allDataList, workItem.SocketSession, workItem.Stream, workItem.tempDataList);
                        workItem.IsPage = false;
                        //DateTime dt3 = DateTime.Now;
                        //Console.WriteLine((dt3 - dt).TotalMilliseconds);
                        //ar.AsyncWaitHandle.Close();
                        return;
                    }
                   
                }
                catch
                {
                    //netc.Soc.Close();
                    //listconn.Remove(netc);
                }

                bytesRead =workItem.Buffer.Length;
                byte[] tempbtye = new byte[bytesRead];
                //if (bytesRead > 0)
                {

                            Buffer.BlockCopy(workItem.Buffer, 0, tempbtye, 0, tempbtye.Length);
                     
                       
                            int lle = workItem.allDataList.Length;

                            byte[] temp = new byte[lle + tempbtye.Length];
                             Buffer.BlockCopy(workItem.allDataList, 0, temp, 0, workItem.allDataList.Length);
                    // Array.Copy(workItem.allDataList, 0, temp, 0, workItem.allDataList.Length);

                             Buffer.BlockCopy(tempbtye, 0, temp, lle, bytesRead);
                            workItem.allDataList = temp; //workItem.DataList.Add(tempbtye);
                            workItem.allDataList = packageData(workItem.allDataList, workItem.SocketSession, workItem.Stream, workItem.tempDataList);
                   // Console.WriteLine((workItem.SocketSession.RemoteEndPoint as IPEndPoint).Address.ToString() + "+ReadCallback：" + workItem.allDataList.Length);
                    //if (workItem.SocketSession.Available > 0)
                    //{
                    //    workItem.SocketSession.BeginReceive(workItem.Buffer = new byte[workItem.SocketSession.Available], 0, workItem.Buffer.Length, 0, ReadCallbackasty, workItem);
                    //}
                    //else 
                    workItem.IsPage = false;
                }
                //DateTime dt2 = DateTime.Now;
                //Console.WriteLine((dt2 - dt).TotalMilliseconds);
            }
            catch(Exception ee)
            {
               // Console.WriteLine("ReadCallback:" + ee.Message);
            }
           
        }

        #region 发送

     
        public bool Send(Socket socket, byte command, string text)
        {
            try
            { 
                    return Send(socket, command, System.Text.Encoding.UTF8.GetBytes(text));
            }
            catch { return false; }
            // tcpc.Close();
             
        }
        private  void SendDataEnd(IAsyncResult ar)
        {
            try
            {
                ((Socket)ar.AsyncState).EndSend(ar);
                
                //ar.AsyncWaitHandle.Close();
            }
            catch
            {

            }
         
        }

        AsyncCallback acallsend;
        public bool Send(SslStream ssl, byte command, string text)
        {
            try
            {
                
                if (Certificate != null)
                {
                    byte[] data = sendpage(command, System.Text.Encoding.UTF8.GetBytes(text));
                    ssl.Write(data);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool Send(SslStream ssl, byte[] vs)
        {
            try
            {

                if (Certificate != null)
                {
                    byte[] data = sendpage(0, vs);
                    Sendbyte(ssl, data);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool Sendbyte(SslStream ssl, byte[] vs)
        {
            try
            {

                if (Certificate != null)
                {
                     
                    ssl.Write(vs);
                }
            }
            catch
            {
                return false;
            }
            return true;   
        }
        public bool Send(SslStream ssl, byte command, byte[] text)
        {
            try
            {

                if (Certificate != null)
                {
                    byte[] data = sendpage(command, text);
                    ssl.Write(data);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
       protected  bool Sendbyte(Socket socket, byte[] text)
        {
            try
            {
                //socket.Send(text);
                //lock (socket)
                {
                     socket.BeginSend(text, 0, text.Length, SocketFlags.None, acallsend, socket);
                }
                //socket.Send(text);
                return true;
            }
            catch
            { return false; }
        }
        public bool Send(Socket socket, byte[] text)
        {
            try
            {
                //socket.Send(text);
                //lock (socket)
                {
                    byte[] data = sendpage(0, text);
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None, acallsend, socket);
                }
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
                byte[] data = sendpage(command, text);
                
                
                return Sendbyte(socket, data);
            }
            catch { return false; }
            // tcpc.Close();
             
        }


        protected virtual byte[] sendpage(byte command, byte[] text)
        {
            byte[] data = new byte[0];
            if (weaveDataType == WeaveDataTypeEnum.Json)
            {
                byte[] sendb = text;
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                data = b;

            }
            else if (weaveDataType == WeaveDataTypeEnum.Bytes)
            {
                byte[] sendb = text;
                byte[] lens = myencoder.ConvertToByteList(sendb.Length);
                byte[] b = new byte[2 + 2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                CRC.ConCRC(ref b, 2 + lens.Length);
                sendb.CopyTo(b, 2 + 2 + lens.Length);
                data = b;
            }
            if (weaveDataType == WeaveDataTypeEnum.custom)
            {

                data = text; 
            }

            return data;
        }


        #endregion

        public int Partition = 20000;
        delegate void packageDataEvent(WeaveNetWorkItems netc);
        packageDataEvent packageDatacall;
        void packageDataHander(WeaveNetWorkItems netc)
        {
            //  WeaveNetWorkItems netc= ar.AsyncState as WeaveNetWorkItems;
           // Console.WriteLine("packageDataHander:" + netc.allDataList.Length);
            netc.allDataList = packageData(netc.allDataList, netc.SocketSession, netc.Stream, netc.tempDataList);
       
            netc.IsPage = false;
           // Console.WriteLine("netc.IsPage:" + netc.IsPage);
        }
        void packageDataHanderobj(object obj)
        {
              WeaveNetWorkItems netc= obj as WeaveNetWorkItems;
           // Console.WriteLine("packageDataHanderobj:" + netc.allDataList.Length);
            netc.allDataList = packageData(netc.allDataList, netc.SocketSession, netc.Stream, netc.tempDataList);

            netc.IsPage = false;
           // Console.WriteLine("netc.IsPage:" + netc.IsPage);
        }
        private static void callBackMethod(IAsyncResult ar)
        {
           // Console.WriteLine("callBackMethod:");
            packageDataEvent caller = ar.AsyncState as packageDataEvent;
            caller.EndInvoke(ar);
        }
        void ReceiveHander(object ias)
        {
           // var w = new SpinWait();
            ReadCallbackasty = new AsyncCallback(ReadCallback);
            packageDatacall = new packageDataEvent(packageDataHander);
            //MM_BeginPeriod(1);
            int a = 0;
            while (true)
            {
              
                try
                {
                    a++;
                    int c = weaveNetworkItems.Count;
                    int count = (c / Partition) + 1;
                    //getbufferdelegate[] iagbd = new getbufferdelegate[count];
                    //IAsyncResult[] ia = new IAsyncResult[count];
                    if (c > 0)
                    {
                        
                        //WeaveNetWorkItems[] netlist = new WeaveNetWorkItems[c];
                        //weaveNetworkItems.CopyTo(0, netlist, 0, c);
                        getbuffer(weaveNetworkItems, 0, c);
                        //if (weaveDataType == WeaveDataTypeEnum.custom)
                        //    Thread.Sleep(5);
                        //else

                       DateTime dt = DateTime.Now;
                        if (resttime>0)
                        Thread.Sleep(resttime);
                        else
                        Thread.Yield();
                        if (a > 500)
                        {
                            Thread.Sleep(1);
                            a = 0;
                        }
                        DateTime dt2 = DateTime.Now;
                      // // Console.WriteLine((dt2-dt).TotalMilliseconds);
                        //w.SpinOnce();
                    }
                    else {
                        
                            Thread.Sleep(1);
                        
                        // System.Threading.Thread.Sleep(resttime);
                    }
                }
                catch { }
                 
                //  System.Threading.Thread.Sleep(1);
            }
           // MM_EndPeriod(1);
        }
        //[DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        //public static extern uint MM_BeginPeriod(uint uMilliseconds);
        //[DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        //public static extern uint MM_EndPeriod(uint uMilliseconds);

        public DateTime dt = DateTime.Now;
        AsyncCallback ReadCallbackasty;
        delegate void getbufferdelegate(WeaveNetWorkItems[] netlist, int index, int len);
        bool getbuffer(List< WeaveNetWorkItems> netlist, int index, int len)
        {

            var bb = true;
            for (int i = index; i < len; i++)
            {
                if (i >= netlist.Count)
                    return bb;
                try
                {
                    WeaveNetWorkItems netc = netlist[i];
                    if (netc.SocketSession != null)
                    {
                        if (netc.SocketSession.Available > 0 || netc.allDataList.Length > 0)
                            bb = false;
                        if (!netc.IsPage && Setherd(netc))
                        {
                            //  if (netc.SocketSession.Available > 0 || netc.allDataList.Length > 3)
                            if (Certificate != null)
                            {

                                SslStream sslStream = netc.Stream;
                                if (sslStream.IsAuthenticated)
                                {
                                    netc.IsPage = true;
                                    ThreadPool.QueueUserWorkItem(new WaitCallback(ReadCallbackssl), netc);
                                }
                            }
                            else
                            {
                                if (netc.SocketSession.Available > 0 )
                                {
                                    dt = DateTime.Now;
                                    netc.IsPage = true;
                                    // netc.SocketSession.ReceiveAsync
                                   // Console.WriteLine((netc.SocketSession.RemoteEndPoint as IPEndPoint).Address.ToString() + "+接收：" + netc.SocketSession.Available);
                                    netc.SocketSession.BeginReceive(netc.Buffer = new byte[netc.SocketSession.Available], 0, netc.Buffer.Length, 0, ReadCallbackasty, netc);
                                }
                                else if (netc.allDataList.Length > 0)
                                {
                                    dt = DateTime.Now;
                                    netc.IsPage = true;
                                   // Console.WriteLine((netc.SocketSession.RemoteEndPoint as IPEndPoint).Address.ToString() + "+剩余：" + netc.allDataList.Length);
                                    // netc.SocketSession.ReceiveAsync
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(packageDataHanderobj), netc);
                                   // packageDatacall.BeginInvoke(netc, callBackMethod, packageDatacall);
                                  
                                     //netc.SocketSession.BeginReceive(netc.Buffer = new byte[netc.SocketSession.Available], 0, netc.Buffer.Length, 0, ReadCallbackasty, netc);
                                }
                              // // Console.WriteLine((netc.SocketSession.RemoteEndPoint as IPEndPoint).Address.ToString() + "+剩余：" + netc.allDataList.Length);
                            }
                          


                        }
                      


                    }
                }
                catch
                { }
            }
            return bb;


        }
        private void ReadCallbackssl(object ar)
        {
            WeaveNetWorkItems netc = (WeaveNetWorkItems)ar;
            SslStream stream = netc.Stream;
            byte[] buffer = new byte[20480];
          //  StringBuilder messageData = new StringBuilder();
            int byteCount = -1;
            List<byte> listb = new List<byte>();
            do
            {
                byteCount = stream.Read(buffer, 0, buffer.Length);

                listb.AddRange(buffer.Take(byteCount));

            } while (byteCount <= 2);

            // netc.DataList.Add(listb.ToArray());
            netc.Buffer = listb.ToArray();
            if (netc.Buffer.Length <= 0)
            {
                netc.allDataList = packageData(netc.allDataList, netc.SocketSession, netc.Stream, netc.tempDataList);
                netc.IsPage = false;
                //ar.AsyncWaitHandle.Close();
                return;
            }
            else {
               int   bytesRead = netc.Buffer.Length;
                byte[] tempbtye = new byte[bytesRead];
                Array.Copy(netc.Buffer, 0, tempbtye, 0, tempbtye.Length); 
                int lle = netc.allDataList.Length; 
                byte[] temp = new byte[lle + tempbtye.Length];
                Array.Copy(netc.allDataList, 0, temp, 0, netc.allDataList.Length);
                Array.Copy(tempbtye, 0, temp, lle, bytesRead);
                netc.allDataList = temp; //workItem.DataList.Add(tempbtye)
                netc.allDataList = packageData(netc.allDataList, netc.SocketSession, netc.Stream, netc.tempDataList);
                netc.IsPage = false;
                return;
            }
           
        }


        public SslStream Authenticate(Socket _socket, X509Certificate2 certificate, SslProtocols enabledSslProtocols)
        {
            Stream _stream = new NetworkStream(_socket);
            var ssl = new SslStream(_stream, false);

            return ssl;
        }

        void AcceptHander(object ias)
        {
            var UpdateSocketListEventHandercback = new System.Threading.WaitCallback(UpdateSocketListEventHander);
            while (true)
            {
                Socket handler = socketLisener.Accept();
                //连接到服务器的客户端Socket封装类
                WeaveNetWorkItems netc = new WeaveNetWorkItems();
                if (Certificate != null)
                {

                    netc.Stream = Authenticate(handler, Certificate, SslProtocols.Default);
                    netc.Stream.AuthenticateAsServer(Certificate, false, SslProtocols.Tls, true);

                   
                }
             
                netc.SocketSession = handler;
                weaveNetworkItems.Add(netc);
                if (Setherd(netc, 1))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(
                   UpdateSocketListEventHandercback,
                      handler);
                }
                
                
            }
        }
    }
}
