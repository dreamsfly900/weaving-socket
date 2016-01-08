using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPclient
{
    public class UDP
    {
        Socket listener;
        public delegate void myreceive(byte command, String data, EndPoint iep);
        public event myreceive receiveevent;
        public string ip;
        public int port, serport;
        public void start(string _ip,int _port,int _serport)
        {
            try
            {
                ip = _ip;
                port = _port;
                serport = _serport;
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 0);
                listener.Bind(localEndPoint);
                // listener.Listen(10000);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receive));
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(xintiao));
            }
            catch (Exception e) { throw e; }
        }

        private void xintiao(object state)
        {
            while (true)
            {
                try
                {
                    send(0x99, "", new IPEndPoint(IPAddress.Parse(ip), serport));
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception e) { throw e; }
            }
        }
        public bool send(byte command, string text,EndPoint ep)
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

                listener.SendTo(b, ep);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }

        public bool send(byte[] b, EndPoint ep)
        {

            try
            { 
                listener.SendTo(b, ep);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        void receiveeventto(object obj)
        {
            modelevent me = (modelevent)obj;
            //if (me.Command == 0x98)
            //{
            //    IPEndPoint iep = new IPEndPoint(IPAddress.Parse(me.Data.Split(':')[0]), int.Parse(me.Data.Split(':')[1]));
            //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(xintiao2), iep);
            //}
            if (receiveevent != null)
                receiveevent(me.Command, me.Data, me.Iep);

        }

        private void xintiao2(object state)
        {
            IPEndPoint me = (IPEndPoint)state;
            while (true)
            {
                try
                {
                    send(0x99, "", me);
                    System.Threading.Thread.Sleep(10000);
                }
                catch (Exception e) { throw e; }
            }
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
            EndPoint iep;

            public EndPoint Iep
            {
                get { return iep; }
                set { iep = value; }
            }
        }
        DateTime timeout;
     //   int mytimeout = 30;
        void receive(object ias)
        {
            while (true)
            {
                try
                {
                    IPEndPoint anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint remoteEndPoint = anyEndPoint;

                    byte[] data = new byte[1024];
                    int recv = listener.ReceiveFrom(data, 0, data.Length, SocketFlags.None, ref remoteEndPoint);
                    int bytesRead = data.Length;
                    byte[] tempbtye = new byte[data.Length];
                    data.CopyTo(tempbtye, 0);
                labe881:
                    if (tempbtye[0] == 0x99)
                    {
                        timeout = DateTime.Now;
                        if (bytesRead > 1)
                        {
                            byte[] b = new byte[bytesRead - 1];
                            byte[] t = tempbtye;
                            Array.Copy(t, 1, b, 0, b.Length);
                            tempbtye = b;
                            bytesRead = bytesRead - (1);
                            goto labe881;
                        }
                        continue;
                    } 
                    int a = tempbtye[1];
                    if (a == 0)
                        continue;
                    String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                    int len = int.Parse(temp);
                    //int b = netc.Buffer[2 + a+1];
                    //temp = System.Text.ASCIIEncoding.ASCII.GetString(netc.Buffer, 2 + a + 1, b);
                    //len = int.Parse(temp);
                    temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                    modelevent me = new modelevent();
                    me.Command = tempbtye[0];
                    me.Data = temp;
                    me.Iep = remoteEndPoint;
                    
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveeventto), me);
                    timeout = DateTime.Now;
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
                catch { } 
                       
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
