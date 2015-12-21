using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCPclient;

namespace client
{
    public class P2Pclient
    {
        TcpClient tcpc;
        public delegate void receive(byte command, String text);
        public event receive receiveServerEvent;
        public delegate void istimeout();
        public event istimeout timeoutevent;
        public delegate void errormessage(int type,string error);
        public event errormessage ErrorMge;
        bool isok = false;
        bool isreceives = false;
        bool isline = false;
        DateTime timeout;
        int mytimeout = 30;
        public delegate void P2Preceive(byte command, String data, EndPoint ep);
        public event P2Preceive P2PreceiveEvent;
        UDP udp;
        bool NATUDP = false;
        String IP;int PORT;
        public bool Isline
        {
            get
            {
                return isline;
            }

            set
            {
                isline = value;
            }
        }

        public string Tokan
        {
            get
            {
                return tokan;
            }

            set
            {
                tokan = value;
            }
        }

        public P2Pclient(bool _NATUDP)
        {
            NATUDP = _NATUDP;
            if (NATUDP)
            {
                udp = new UDP();
                udp.receiveevent += udp_receiveevent;
            }
        }
        public bool start(string ip, int port, int _timeout)
        {
            mytimeout = _timeout;
            IP = ip;
            PORT = port;
          return  start(ip, port);
        }
        public bool Restart()
        {
            return start(IP, PORT);
        }
        public bool start(string ip, int port)
        {
            try
            {
                IP = ip;
                PORT = port;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new TcpClient();
                tcpc.ExclusiveAddressUse = false;
                tcpc.Connect(ip, port);
                Isline = true;
                isok = true;
                if (NATUDP)
                {
                    udp.start(ip, new Random().Next(10000, 60000), port);
                    udp.send(0x9c, IPAddress.None.ToString() + ":" + port, localEndPoint);
                }
                timeout = DateTime.Now;
                if (!isreceives)
                {
                    isreceives = true;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receives));
                }
                return true;
            }
            catch (Exception e)
            {
                Isline = false;
                if (ErrorMge != null)
                    ErrorMge(1, e.Message);
                return false;
            }
        }

        void udp_receiveevent(byte command, string data, EndPoint iep)
        {
            if (P2PreceiveEvent != null)
                P2PreceiveEvent(command, data, iep);
        }
        public bool p2psend(byte command, string text, IPEndPoint ep)
        {
            return udp.send(command, text, ep);

        }

        private string tokan;
        public bool send(byte command, string text)
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

                tcpc.Client.Send(b);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }

        public void stop()
        {
            isok = false;
            Isline = false;
            tcpc.Close();
            
        }
        void receives(object obj)
        {
            while (isok)
            {
                System.Threading.Thread.Sleep(50);
                try
                {
                    byte[] tempb = null;
                    labered:
                    int bytesRead = tcpc.Client.Available;

                    if (bytesRead > 0)
                    {


                        byte[] tempbtye = new byte[bytesRead];
                        tcpc.Client.Receive(tempbtye);
                        if (tempb != null)
                        {
                            byte[] tempbtyes = new byte[tempbtye.Length + tempb.Length];
                            Array.Copy(tempb, tempbtyes, tempb.Length);
                            Array.Copy(tempbtye, 0, tempbtyes, tempb.Length, tempbtye.Length);
                            tempbtye = tempbtyes;
                        }
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
                            }
                            continue;
                        }
                        int a = tempbtye[1];
                        String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                        int len = int.Parse(temp);
                        if (len > tempbtye.Length)
                        {
                            tempb = new byte[tempbtye.Length];
                            Array.Copy(tempbtye, tempb, tempbtye.Length);
                            goto labered;
                        }
                        //int b = netc.Buffer[2 + a+1];
                        //temp = System.Text.ASCIIEncoding.ASCII.GetString(netc.Buffer, 2 + a + 1, b);
                        //len = int.Parse(temp);
                        temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                        try
                        {
                            if (tempbtye[0] == 0xff)
                            {
                                if (temp.IndexOf("token") >= 0)
                                    Tokan = temp.Split('|')[1];
                                
                            }
                            else if (receiveServerEvent != null)
                                receiveServerEvent(tempbtye[0], temp);
                        }
                        catch (Exception e)
                        {
                            if (ErrorMge != null)
                                ErrorMge(1, e.Message);
                        }

                        if (bytesRead > (2 + a + len))
                        {
                            byte[] b = new byte[bytesRead - (2 + a + len)];
                            byte[] t = tempbtye;
                            Array.Copy(t, (2 + a + len), b, 0, b.Length);

                            tempbtye = b;
                            bytesRead = bytesRead - (2 + a + len);
                            goto labe881;
                        }
                        timeout = DateTime.Now;
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - timeout;
                        if (ts.Seconds > mytimeout)
                        {
                            Isline = false;
                            // stop();
                            //  isreceives = false;
                            timeoutevent();

                            //   return;

                        }
                    }
                }
                catch (Exception e)
                {
                    
                    if (ErrorMge != null)
                        ErrorMge(1, e.Message);
                     
                }



            }
        }

      
    }
}
