using P2P;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServer
{
  
    public class DTUServer  
    {
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    
        List<NETcollection> listconn = new List<NETcollection>();
         event myreceive receiveevent;
        public event myreceiveDtu receiveeventDtu;
        public event NATthrough NATthroughevent;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public event UpdataListSoc EventUpdataConnSoc;
        public event deleteListSoc EventDeleteConnSoc;
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
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener.Bind(localEndPoint);
            listener.Listen(1000000);
          
            System.Threading.Thread t = new Thread(new ParameterizedThreadStart(Accept));
            t.Start();
            System.Threading.Thread t1 = new Thread(new ParameterizedThreadStart(receive));
            t1.Start();
            System.Threading.Thread t2 = new Thread(new ParameterizedThreadStart(receivepage));
            t2.Start();
            System.Threading.Thread t3 = new Thread(new ParameterizedThreadStart(xintiao));
            t3.Start();
          

        }
       
     

        public int getNum()
        {
            return listconn.Count;
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
                            byte[] b = new byte[0]  ;

                            netc.Soc.Send(b);

                        }
                        catch
                        {

                            try
                            {
                                netc.Soc.Close();


                            }
                            catch { }
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteConnSoc), netc.Soc);
                   

                            listconn.Remove(netc);
                        }

                    }

                    GC.Collect();
                }
                catch { }
            }
        }

        private void DeleteConnSoc(object state)
        {
            if (EventDeleteConnSoc != null)
                EventDeleteConnSoc(state as Socket);
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
                    if (bytesRead == 0)
                    {
                        if (ListData.Count > 0) ListData.RemoveAt(0);
                        netc.Ispage = false; return;
                    };
                    byte[] tempbtye = new byte[bytesRead];
                    Array.Copy(ListData[i], tempbtye, tempbtye.Length);
                     

                    try
                    {
                        dtumodle dd = new dtumodle();
                        dd.Data = tempbtye;
                        dd.Soc = netc.Soc;
                        if (receiveeventDtu != null)
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(objreceiveeventDtu), dd);
                       // receiveeventDtu.BeginInvoke(tempbtye, netc.Soc, null, null);
                        if (ListData.Count > 0) ListData.RemoveAt(i);
                        netc.Ispage = false; return;

                    }
                    catch (Exception e)
                    {
                        netc.Ispage = false; return;
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
        class dtumodle
        {
            byte[] data;
            Socket soc;
            public byte[] Data
            {
                get
                {
                    return data;
                }

                set
                {
                    data = value;
                }
            }

            public Socket Soc
            {
                get
                {
                    return soc;
                }

                set
                {
                    soc = value;
                }
            }
        }
        private void objreceiveeventDtu(object state)
        {
            dtumodle dd = state as dtumodle;
            receiveeventDtu(dd.Data, dd.Soc);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            NETcollection netc = (NETcollection)ar.AsyncState;
            Socket handler = netc.Soc;
            
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
            catch
            {

            } 
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
                                System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(packageData));
                                t.Start(netc);
                                
                            }
                        }

                    }
                    System.Threading.Thread.Sleep(10);
                }
                catch { }

            }
        }
        void receive(object ias)
        {
            while (true)
            {
                try
                {

                    int c = listconn.Count;
                     
                    NETcollection[] netlist = new NETcollection[c];
                    listconn.CopyTo(0, netlist, 0, c);
                    getbuffer(netlist, 0, c);
                }
                catch { }
                System.Threading.Thread.Sleep(10);
            }
        }

        delegate void getbufferdelegate(NETcollection[] netlist, int index, int len);
        void getbuffer(NETcollection[] netlist, int index, int len)
        {
            for (int i = index; i < len; i++)
            {
                NETcollection netc = netlist[i];
                try
                {
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
                
                NETcollection netc = new NETcollection();
                netc.Soc = handler;
                listconn.Add(netc);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdataConnSoc), handler);


            }

        }


    }
}
