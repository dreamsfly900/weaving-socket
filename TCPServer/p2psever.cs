using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P2P 
{
    public delegate void myreceive(byte command, String data, Socket soc);
    public delegate void myreceiveDtu(byte[] data, Socket soc);
    public delegate void NATthrough(byte command,String data, EndPoint ep);
    public delegate void UpdataListSoc(Socket soc);
    public delegate void deleteListSoc(Socket soc);
    public delegate void deleteSoc(Socket soc,string token);
    public interface ITcpBasehelper
    {
        
        void start(int port);
        int getNum();
        void xintiao(object obj);
        bool send(Socket soc, byte command, string text);
         event myreceive receiveevent;
         
        event NATthrough NATthroughevent; 

         event UpdataListSoc EventUpdataConnSoc;

         event deleteListSoc EventDeleteConnSoc;
       
    }
    public class p2psever: ITcpBasehelper
    {
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        UDP udp;
        List<NETcollection> listconn = new List<NETcollection>();
    
        public event myreceive receiveevent; 
        public event NATthrough NATthroughevent;
        public static ManualResetEvent allDone = new ManualResetEvent(false); 
        public event UpdataListSoc EventUpdataConnSoc; 
        public event deleteListSoc EventDeleteConnSoc;
   
        string loaclip;
        public p2psever(string _loaclip)
        {
            loaclip = _loaclip;
            udp = new UDP(_loaclip);
        }
        public p2psever()
        {
            udp = new UDP("127.0.0.1");
        }
        public   void start(int port)
        {
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener.Bind(localEndPoint); 
            listener.Listen(1000000);
            if (NATthroughevent != null)
            {
                udp.start(port);
                udp.receiveevent += udp_receiveevent;
            }
            System.Threading.Thread t = new Thread(new ParameterizedThreadStart(Accept));
            t.Start();
            System.Threading.Thread t1 = new Thread(new ParameterizedThreadStart(receive));
            t1.Start();
            System.Threading.Thread t2 = new Thread(new ParameterizedThreadStart(receivepage));
            t2.Start();
            System.Threading.Thread t3 = new Thread(new ParameterizedThreadStart(xintiao));
            t3.Start();
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Accept));
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receive));
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receivepage));
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(xintiao));

        }
     public NETcollectionUdp[]  getNATthrough()
     {
        return udp.getUdpList();
     }
     void udp_receiveevent(byte command, string data, NETcollectionUdp NETc)
     {
          
             
             if (NATthroughevent != null)
                 NATthroughevent(command,data, NETc.Iep);
       
     }

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
                          
                            try {
                                netc.Soc.Close();

                               
                            }
                         catch { }
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteConnSoc), netc.Soc);
                            //try
                            //{
                            //    if (EventDeleteConnSoc != null)
                            //        EventDeleteConnSoc.BeginInvoke(netc.Soc, null, null);
                            //}
                            //catch { }


                            listconn.Remove(netc);
                     }

                 }

                   // GC.Collect();
             }
             catch { }
         }
     }

     private void DeleteConnSoc(object state)
     {
         if (EventDeleteConnSoc!=null)
         EventDeleteConnSoc(state as Socket);
     }
    
        //void AcceptCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        allDone.Set();
        //        Socket listener = (Socket)ar.AsyncState;
        //        if (listener == null)
        //            return;
        //        Socket handler = listener.EndAccept(ar);

        //        // Create the state object.
        //        NETcollection netc = new NETcollection();
        //        netc.Soc = handler;
        //        listconn.Add(netc);
        //        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdataConnSoc), handler);

        //       // netc.Buffer 

        //        handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
        //    }
        //    catch(Exception ex)
        //    { }
        //   // allDone.Set();

        //}

        private void UpdataConnSoc(object state)
        {
            if (EventUpdataConnSoc!=null)
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

        private void packageData(object obj)
        {

            NETcollection netc = obj as NETcollection;
            List<byte[]> ListData = netc.Datalist;
            try
            {
                int i = 0;
                int count = ListData.Count;

                if (count > 0)
                {

                    int bytesRead = ListData[i] != null ? ListData[i].Length : 0;
                    if (bytesRead == 0) {
                        if (ListData.Count > 0) ListData.RemoveAt(0);
                        netc.Ispage = false; return; }; 
                    byte[] tempbtye = new byte[bytesRead];
                    Array.Copy(ListData[i], tempbtye, tempbtye.Length);
                    
                   
                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (bytesRead > 2 + a)
                        {
                            String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                            int len = int.Parse(temp);


                            if ((len + 2 + a) > tempbtye.Length)
                            {
                                try
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
                                }
                                catch
                                {
                                }
                                netc.Ispage = false; return;

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
                               
                                    temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                            }
                            catch
                            { }
                            try
                            {
                                modelevent me = new modelevent();
                                me.Command = tempbtye[0];
                                me.Data = temp;
                                me.Soc = netc.Soc;
                                if (receiveevent != null)
                                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveeventto), me);
                                //receiveeventto(me);
                                //if (receiveevent != null)
                                //    receiveevent.BeginInvoke(tempbtye[0], temp, netc.Soc, null, null);
                                //if (ListData.Count > 0) ListData.RemoveAt(i);
                                netc.Ispage = false; return;

                            }
                            catch (Exception e)
                            {
                                netc.Ispage = false; return;
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
                            netc.Ispage = false; return;
                        }
                    }

                }

            }
            catch (Exception e)
            {
                if (netc.Datalist.Count > 0)
                    netc.Datalist.RemoveAt(0);
                netc.Ispage = false;
                return;

            }
            finally { netc.Ispage = false; }
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
                byte[] tempbtye = new byte[bytesRead];

                //netc.Buffer.CopyTo(tempbtye, 0);
                if (bytesRead > 0)
                {
                    Array.Copy(netc.Buffer, 0, tempbtye, 0, bytesRead);
                    netc.Datalist.Add(tempbtye);
                }
            }
            catch {
                
            }
            //handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
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
        void receivepage(object ias)
        {
            while (true)
            {
                try
                {
                    NETcollection[] netlist = new NETcollection[listconn.Count];
                    listconn.CopyTo(netlist);
                    foreach (NETcollection netc in netlist)
                    {
                        
                        if (netc.Datalist.Count > 0)
                        {
                            if (!netc.Ispage)
                            {
                                netc.Ispage = true;
                                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(packageData), netc);
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
        void receive(object ias)
        {
            while (true)
            {
                try
                {

                    int c = listconn.Count;
                    int count = (c / Partition) + 1;
                    getbufferdelegate[] iagbd = new getbufferdelegate[count];
                    IAsyncResult[] ia = new IAsyncResult[count];

                    if (c > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {

                            c = c - (i * Partition) > Partition ? Partition : c - (i * Partition);
                            NETcollection[] netlist = new NETcollection[c];
                            listconn.CopyTo(i * Partition, netlist, 0, c);
                            getbuffer(netlist, 0, Partition);
                            //  iagbd[i] = new getbufferdelegate(getbuffer);
                            //ia[i]= iagbd[i].BeginInvoke(netlist, 0, Partition, null, null);


                        }
                        //for (int i = 0; i < count; i++)
                        //{
                        //    iagbd[i].EndInvoke(ia[i]);
                        //}
                    }
                    //NETcollection[] netlist = new NETcollection[c];
                    //listconn.CopyTo(0, netlist, 0, c);
                    //getbuffer(netlist, 0, c);
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
        }

        delegate void getbufferdelegate(NETcollection[] netlist, int index, int len);
        void getbuffer(NETcollection[] netlist, int index, int len)
        {
            for (int i = index; i < len; i++)
            {
                if (i >= netlist.Length)
                    return;
               
                try
                {
                    NETcollection netc = netlist[i];
                    if (netc.Soc != null)
                    {
                        if (netc.Soc.Available > 0)
                        {

                             
                                netc.Soc.BeginReceive(netc.Buffer = new byte[netc.Soc.Available], 0, netc.Buffer.Length, 0, new AsyncCallback(ReadCallback), netc);

                        }

                    }
                }
                catch
                { }

            }
        }

        void Accept(object ias)
        {
            while (true)
            {

                Socket handler = listener.Accept();
                //if (listener == null)
                //    return;
                //Socket handler = listener.EndAccept(ar);

                // Create the state object.
                NETcollection netc = new NETcollection();
                netc.Soc = handler;
                listconn.Add(netc);
                //if (EventUpdataConnSoc != null)
                //    EventUpdataConnSoc.BeginInvoke(handler, null, null);
                //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(UpdataConnSoc));
                //t.Start(handler);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdataConnSoc), handler);

                    // Set the event to nonsignaled state.
                    //allDone.Reset();

                    ////开启异步监听socket
                    ////    //Console.WriteLine("Waiting for a connection");
                    //try
                    //{
                    //    listener.BeginAccept(
                    //              new AsyncCallback(AcceptCallback),
                    //              listener);
                    //System.Threading.Thread.Sleep(1);
                    //}
                    //catch { }
                    //// 让程序等待，直到连接任务完成。在AcceptCallback里的适当位置放置allDone.Set()语句.
                    //allDone.WaitOne();
            }

        }

        
    }
}
