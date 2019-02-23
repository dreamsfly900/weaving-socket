using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Weave.TCPClient
{
    public class DTUclient
    {
        TcpClient tcpc;
        public delegate void receive(string token, byte[] text);
        public event receive ReceiveServerEvent;
        public delegate void istimeout();
        public event istimeout Timeoutevent;
        public delegate void errormessage(int type, string error);
        public event errormessage ErrorMge;
        bool isok = false;
        bool isreceives = false;
        DateTime timeout;
        int mytimeout = 90;
        public delegate void P2Preceive(byte command, string data, EndPoint ep);
        public event P2Preceive P2PreceiveEvent;

        string IP; int PORT;
        public bool Isline { get; set; } = false;
        readonly List<object> objlist = new List<object>();

        public string Tokan { get; set; }

        public List<byte[]> ListData { get; set; } = new List<byte[]>();

        P2Pclient p2p = new P2Pclient(false);
        public DTUclient()
        {
            p2p.Timeoutevent += P2p_timeoutevent;
        }

        private void P2p_timeoutevent()
        {
            if (!p2p.Isline)
            {
                p2p.Restart(false);
                Restart(false);
            }
        }

        public bool Start(string ip, int port, int _timeout, bool takon)
        {
            mytimeout = _timeout;
            IP = ip;
            PORT = port;
            return Start(ip, port, takon);
        }

        public bool Restart(bool takon)
        {
            return Start(IP, PORT, takon);
        }

        public bool Start(string ip, int port, bool takon)
        {
            try
            {
                IP = ip;
                PORT = port;
                if (!p2p.Isline)
                    p2p.Start(IP, PORT, false);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new TcpClient
                {
                    ExclusiveAddressUse = false
                };
                tcpc.Connect(ip, port);
                Isline = true;
                isok = true;
                timeout = DateTime.Now;
                if (!isreceives)
                {
                    isreceives = true;
                    Thread t = new Thread(new ParameterizedThreadStart(Receives));
                    t.Start();
                    Thread t1 = new Thread(new ThreadStart(Unup));
                    t1.Start();
                }
                return true;
            }
            catch
            {
                Isline = false;
                return false;
            }
        }

        void Udp_receiveevent(byte command, string data, EndPoint iep)
        {
            P2PreceiveEvent?.Invoke(command, data, iep);
        }

        public void Send(byte[] b)
        {
            tcpc.Client.Send(b);
        }

        public void Stop()
        {
            isok = false;
            Isline = false;
            tcpc.Close();
        }

        class Temppake { public string command; public byte[] date; }

        void Rec(object obj)
        {
            Temppake str = obj as Temppake;
            ReceiveServerEvent(str.command, str.date);
        }

        void Unup()
        {
            while (isok)
            {
                Thread.Sleep(10);
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
                            Temppake str = new Temppake
                            {
                                command = Tokan,
                                date = tempbtye
                            };
                            ReceiveServerEvent(Tokan, str.date);
                            continue;
                        }
                        catch (Exception e)
                        {
                            ErrorMge?.Invoke(3, "unup:" + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorMge?.Invoke(3, "unup:" + e.Message);
                }
            }
        }

        void Receives(object obj)
        {
            while (isok)
            {
                Thread.Sleep(150);
                try
                {
                    int bytesRead = tcpc.Client.Available;
                    if (bytesRead > 0)
                    {
                        byte[] tempbtye = new byte[bytesRead];
                        tcpc.Client.Receive(tempbtye);
                        ListData.Add(tempbtye);
                        timeout = DateTime.Now;
                    }
                }
                catch (Exception e)
                {
                    ErrorMge?.Invoke(2, e.Message);
                }
            }
        }
    }
}
