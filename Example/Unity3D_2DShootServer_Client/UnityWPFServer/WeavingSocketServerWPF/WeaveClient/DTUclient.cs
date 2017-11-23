
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace WeaveClient
{
    public class DTUclient
    {
        TcpClient tcpc;
        public delegate void receive(string token, byte[] text);
        public event receive receiveServerEvent;
        public delegate void istimeout();
        public event istimeout timeoutevent;
        public delegate void errormessage(int type, string error);
        public event errormessage ErrorMge;
        bool isok = false;
        bool isreceives = false;
        bool isline = false;
        DateTime timeout;
        int mytimeout = 90;
        public delegate void P2Preceive(byte command, String data, EndPoint ep);
        public event P2Preceive P2PreceiveEvent;
     
      
        String IP; int PORT;
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
        List<object> objlist = new List<object>();
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
        public List<byte[]> ListData
        {
            get
            {
                return listtemp;
            }
            set
            {
                listtemp = value;
            }
        }
        P2Pclient p2p = new P2Pclient(false);
        public DTUclient()
        {
            p2p.timeoutevent += P2p_timeoutevent;
        }
        private void P2p_timeoutevent()
        {
            if (!p2p.Isline)
            {
                p2p.Restart(false);
                Restart(false);
            }
        }
        public bool start(string ip, int port, int _timeout, bool takon)
        {
            mytimeout = _timeout;
            IP = ip;
            PORT = port;
            return start(ip, port, takon);
        }
        public bool Restart(bool takon)
        {
            return start(IP, PORT, takon);
        }
        public bool start(string ip, int port, bool takon)
        {
            try
            {
                IP = ip;
                PORT = port;
                if (!p2p.Isline)
                    p2p.start(IP, PORT, false);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new TcpClient();
                tcpc.ExclusiveAddressUse = false;
                tcpc.Connect(ip, port);
                Isline = true;
                isok = true;
                timeout = DateTime.Now;
                if (!isreceives)
                {
                    isreceives = true;
                    System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(receives));
                    t.Start();
                    System.Threading.Thread t1 = new System.Threading.Thread(new ThreadStart(unup));
                    t1.Start();
                }
                return true;
            }
            catch (Exception ex)
            {
                string ems = ex.Message;
                Isline = false;
                return false;
            }
        }
        void udp_receiveevent(byte command, string data, EndPoint iep)
        {
            if (P2PreceiveEvent != null)
                P2PreceiveEvent(command, data, iep);
        }
       
        private string tokan;
        public void Send(byte[] b)
        {
            tcpc.Client.Send(b);
        }
        public void stop()
        {
            isok = false;
            Isline = false;
            tcpc.Close();
        }
        class temppake { public string command; public byte[] date; }
        void rec(object obj)
        {
            temppake str = obj as temppake;
            receiveServerEvent(str.command, str.date);
        }
        void unup()
        {
            while (isok)
            {
                System.Threading.Thread.Sleep(10);
                try
                {
                    int i = 0;
                    int count = ListData.Count;
                    if (count > 0)
                    {
                        int bytesRead = ListData[i] != null ? ListData[i].Length : 0;
                        if (bytesRead == 0) continue;
                        byte[] tempbtye = new byte[bytesRead];
                        Array.Copy(ListData[i], tempbtye, tempbtye.Length);
                        try
                        {
                            ListData.RemoveAt(0);
                            temppake str = new temppake();
                            str.command = Tokan;
                            str.date = tempbtye;
                            receiveServerEvent(Tokan, str.date);
                            continue;
                        }
                        catch (Exception e)
                        {
                            if (ErrorMge != null)
                                ErrorMge(3, "unup:" + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (ErrorMge != null)
                        ErrorMge(3, "unup:" + e.Message);
                }
            }
        }
        List<Byte[]> listtemp = new List<Byte[]>();
        void receives(object obj)
        {
            while (isok)
            {
                System.Threading.Thread.Sleep(150);
                try
                {
                    int bytesRead = tcpc.Client.Available;
                    if (bytesRead > 0)
                    {
                        byte[] tempbtye = new byte[bytesRead];
                        tcpc.Client.Receive(tempbtye);
                        //lock (this)
                        //{
                        ListData.Add(tempbtye);
                        timeout = DateTime.Now;
                    }
                }
                catch (Exception e)
                {
                    if (ErrorMge != null)
                        ErrorMge(2, e.Message);
                }
            }
        }
    }
}
