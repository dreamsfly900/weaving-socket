using MyInterface;
using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using TCPclient;

namespace client
{
    public class P2Pclient
    {
        _base_manage xmhelper = new _base_manage();

        TcpClient tcpc;
        public delegate void receive(byte command, String text);
        public event receive receiveServerEvent;
        public delegate void istimeout();
        public event istimeout timeoutevent;
        public delegate void errormessage(int type, string error);
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
        public void AddListenClass(object obj)
        {
            GetAttributeInfo(obj.GetType(), obj);
            //xmhelper.AddListen()
            //objlist.Add(obj);
        }
        public void DeleteListenClass(object obj)
        {
            GetAttributeInfo(obj.GetType(), obj);
            //xmhelper.AddListen()
            //objlist.Add(obj);
        }
        public void deleteAttributeInfo(Type t, object obj)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                InstallFun myattribute = (InstallFun)Attribute.GetCustomAttribute(mi, typeof(InstallFun));
                if (myattribute == null)
                { }
                else
                {
                    xmhelper.DeleteListen(mi.Name);
                }
            }
        }
        public void GetAttributeInfo(Type t, object obj)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                InstallFun myattribute = (InstallFun)Attribute.GetCustomAttribute(mi, typeof(InstallFun));
                if (myattribute == null)
                { }
                else
                {
                    Delegate del = Delegate.CreateDelegate(typeof(RequestData), obj, mi, true);
                    xmhelper.AddListen(mi.Name, del as RequestData, myattribute.Type);
                }
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

        public P2Pclient(bool _NATUDP)
        {
            this.receiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
            NATUDP = _NATUDP;
            if (NATUDP)
            {
                udp = new UDP();
                udp.receiveevent += udp_receiveevent;
            }
        }

        private void Xmhelper_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
            if (ErrorMge != null)
                ErrorMge(0, message);
        }

        private void P2Pclient_receiveServerEvent(byte command, string text)
        {
            xmhelper.init(text, null);
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
                    System.Threading.Thread t=new System.Threading.Thread(new ParameterizedThreadStart(receives));
                    t.Start();
                    System.Threading.Thread t1 = new System.Threading.Thread(new ThreadStart (unup));
                    t1.Start();
                }
                int ss = 0;
                if (!takon) return true;
                while (Tokan == null)
                {
                    System.Threading.Thread.Sleep(1000);
                    ss++;
                    if (ss > 10)
                        return false;
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
        public bool SendParameter<T>(byte command, String Request, T Parameter, int Querycount)
        {
            _baseModel b = new _baseModel();
            b.Request = Request;
            b.Token = this.Tokan;
            b.SetParameter<T>(Parameter);
            b.Querycount = Querycount;
            send(command, b.Getjson());
            return true;
        }
        public bool SendRoot<T>(byte command, String Request, T Root, int Querycount)
        {
            _baseModel b = new _baseModel();
            b.Request = Request;
            b.Token = this.Tokan;
            b.SetRoot<T>(Root);
            b.Querycount = Querycount;
            send(command, b.Getjson());
            return true;
        }
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
        class temppake { public byte command; public string date; }
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
                        
                        int bytesRead = ListData[i]!=null? ListData[i].Length:0;
                        if (bytesRead == 0) continue;
                        byte[] tempbtye = new byte[bytesRead];
                        Array.Copy(ListData[i], tempbtye, tempbtye.Length);
                        _0x99:
                        if (tempbtye[0] == 0x99)
                        {
                            
                            if (bytesRead > 1)
                            {
                                byte[] b = new byte[bytesRead - 1];
                                byte[] t = tempbtye;
                                Array.Copy(t, 1, b, 0, b.Length);
                                ListData[i] = b;
                                tempbtye = b;
                                goto _0x99;
                            }
                            else {
                                ListData.RemoveAt(i);
                                continue;
                            } 
                        }
                        labe881:
                        if (bytesRead > 2)
                        {
                            int a = tempbtye[1];
                            if (bytesRead > 2 + a)
                            {
                                String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                                int len = int.Parse(temp);

                                labered:
                                if ((len + 2 + a) > tempbtye.Length)
                                {
                                    ListData.RemoveAt(i);
                                    byte[] temps = new byte[tempbtye.Length];
                                    Array.Copy(tempbtye, temps, temps.Length);
                                    tempbtye = new byte[temps.Length + ListData[i].Length];
                                    Array.Copy(ListData[i], tempbtye, tempbtye.Length);
                                  
                                    goto labered;
                                }
                                else if (tempbtye.Length > (len + 2 + a))
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 2 + a)];
                                    Array.Copy(tempbtye, (len + 2 + a), temps, 0, temps.Length);
                                    ListData[i] = temps;

                                } else if (tempbtye.Length == (len + 2 + a)) 
                                    { ListData.RemoveAt(i); }
                               
                                temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                try
                                {
                                    temppake str = new temppake();
                                    str.command = tempbtye[0];
                                    str.date = temp;
                                    if (tempbtye[0] == 0xff)
                                    {
                                        if (temp.IndexOf("token") >= 0)
                                            Tokan = temp.Split('|')[1];
                                        else
                                        {
                                            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(rec), str);
                                                
                                            //    = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(rec));
                                            //tt.Start(str);
                                        }
                                    }
                                    else if (receiveServerEvent != null)
                                    {
                                        System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(rec), str);

                                        //System.Threading.Thread tt = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(rec));
                                        //tt.Start(str);
                                        // receiveServerEvent();
                                    }
                                    continue;
                                }
                                catch (Exception e)
                                {
                                    if (ErrorMge != null)
                                        ErrorMge(3, "unup:" + e.Message);
                                }
                            }
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
                System.Threading.Thread.Sleep(50);
                try
                {
                 
                
                    int bytesRead = tcpc.Client.Available;
                  
                    if (bytesRead > 0)
                    {
                        byte[] tempbtye = new byte[bytesRead];
                        tcpc.Client.Receive(tempbtye);
                        _0x99:
                        if (tempbtye[0] == 0x99)
                        {
                            timeout = DateTime.Now;
                            if (bytesRead > 1)
                            {
                                byte[] b = new byte[bytesRead - 1];
                                byte[] t = tempbtye;
                                Array.Copy(t, 1, b, 0, b.Length);
                                tempbtye = b;
                               // bytesRead = bytesRead - (1);
                                goto _0x99;
                            }else
                             continue;
                        }
                        lock (this)
                        {
                            ListData.Add(tempbtye);
                        }
                        timeout = DateTime.Now;
                        
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - timeout;
                        if (ts.Seconds > mytimeout)
                        {
                            Isline = false;
                            //stop();
                            //isreceives = false;
                            timeoutevent();
                            //return;
                        }
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
