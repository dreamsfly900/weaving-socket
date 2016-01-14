using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace P2P 
{
    public delegate void myreceive(byte command, String data, Socket soc);
    public delegate void NATthrough(byte command,String data, EndPoint ep);
    public delegate void UpdataListSoc(Socket soc);
    public delegate void deleteListSoc(Socket soc);
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
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Accept));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receive));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(xintiao));
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

     private void DeleteConnSoc(object state)
     {
         if (EventDeleteConnSoc!=null)
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
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdataConnSoc), handler);

               // netc.Buffer 

                handler.BeginReceive(netc.Buffer, 0, netc.BufferSize, 0, new AsyncCallback(ReadCallback), netc);
            }
            catch(Exception ex)
            { }
           // allDone.Set();

        }

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
                    System.Threading.Thread tt = new Thread(new ParameterizedThreadStart(receiveeventto));
                    tt.Start(me);
                        //.QueueUserWorkItem(new System.Threading.WaitCallback(receiveeventto), me);

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
                System.Threading.Thread.Sleep(10);
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
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(UpdataConnSoc));
                t.Start(handler);
                //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdataConnSoc), handler);

                // Set the event to nonsignaled state.
                //allDone.Reset();

                ////开启异步监听socket
                ////    Console.WriteLine("Waiting for a connection");
                //try
                //{
                //    listener.BeginAccept(
                //              new AsyncCallback(AcceptCallback),
                //              listener);
                System.Threading.Thread.Sleep(1);
                //}
                //catch { }
                //// 让程序等待，直到连接任务完成。在AcceptCallback里的适当位置放置allDone.Set()语句.
                //allDone.WaitOne();
            }

        }

        
    }
}
