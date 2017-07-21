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

namespace client
{
    public class P2Pclient
    {
        _base_manage xmhelper = new _base_manage();

        System.Net.Sockets.Socket tcpc;
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
        //UDP udp;
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

        public P2Pclient(bool _NATUDP)
        {
            this.receiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
            NATUDP = _NATUDP;
            if (NATUDP)
            {
                //udp = new UDP();
                //udp.receiveevent += udp_receiveevent;
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
                tcpc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //  tcpc.ExclusiveAddressUse = false;
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = new DnsEndPoint(IP, port);
              
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object o, SocketAsyncEventArgs e)
                {
                    if (e.SocketError != SocketError.Success)
                    {

                        if (e.SocketError == SocketError.ConnectionAborted)
                        {
                            timeoutevent();
                          
                            //Dispatcher.BeginInvoke(() => MessageBox.Show("连接超时请重试！ " + e.SocketError));
                        }
                        else if (e.SocketError == SocketError.ConnectionRefused)
                        {
                            ErrorMge(0, "服务器端未启动");
                         
                            //Dispatcher.BeginInvoke(() => MessageBox.Show("服务器端问启动" + e.SocketError));
                        }
                        else
                        {
                            ErrorMge(0, "出错了");
                            // Dispatcher.BeginInvoke(() => MessageBox.Show("出错了" + e.SocketError));
                        }
                      
                    }
                    else
                    {
                        Isline = true;
                        isok = true;
                        //if (NATUDP)
                        //{
                        //    udp.start(ip, new Random().Next(10000, 60000), port);
                        //    udp.send(0x9c, IPAddress.None.ToString() + ":" + port, localEndPoint);
                        //}
                        timeout = DateTime.Now;
                        if (!isreceives)
                        {
                            isreceives = true;
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Receive));
                        }
                    }
                    

                });

                tcpc.ConnectAsync(socketEventArg);
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
        //public bool p2psend(byte command, string text, IPEndPoint ep)
        //{
        //    return udp.send(command, text, ep);
        //}

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
        private void Saea_Completed(object sender, SocketAsyncEventArgs e)
        {

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
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.Completed += Saea_Completed;
                saea.SetBuffer(b, 0, b.Length);
                tcpc.SendAsync(saea);
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
        SocketAsyncEventArgs socketReceiveArgs = new SocketAsyncEventArgs();
        public void Receive(object obj)
        {
            string received = "";

            while (isok)
            {
                socketReceiveArgs = new SocketAsyncEventArgs();
                socketReceiveArgs.SetBuffer(new byte[5120], 0, 5120);
                socketReceiveArgs.Completed += SocketReceiveArgs_Completed;
                try
                {
                    done.Reset();
                 
                if (tcpc.ReceiveAsync(socketReceiveArgs))
                {
                        done.WaitOne();
                }

                    System.Threading.Thread.Sleep(50);
                }
                catch (Exception e)
                {
                }

            }
        }
        ManualResetEvent done = new ManualResetEvent(false);
        byte[] tempb = null;
        private void SocketReceiveArgs_Completed(object sender, SocketAsyncEventArgs receiveArgs)
        {
            //while (isok)
            //{
                if (receiveArgs.SocketError == SocketError.Success)
                {

                    //System.Threading.Thread.Sleep(100);
                    //received = Encoding.UTF8.GetString(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred);


                    try
                    {
                        
                        labered:
                        int bytesRead = receiveArgs.BytesTransferred;

                        if (bytesRead > 0)
                        {
                        byte[] tempbtye = new byte[bytesRead];
                          
                            Array.Copy(receiveArgs.Buffer, tempbtye, tempbtye.Length);
                        // receiveArgs.Dispose();
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
                                goto labe881;
                            }
                            else
                            { done.Set(); return; }
                                 
                               // continue;
                            }
                            int a = tempbtye[1];
                            String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                            int len = int.Parse(temp);
                        if (len > tempbtye.Length)
                        {
                            tempb = new byte[tempbtye.Length];
                            Array.Copy(tempbtye, tempb, tempbtye.Length);
                            //  goto labered;
                            int lennext = 5120;
                            if ((len - tempbtye.Length) < 5120)
                                lennext = len - tempbtye.Length+2+a;
                            System.Threading.Thread.Sleep(100);
                            socketReceiveArgs = new SocketAsyncEventArgs();
                            socketReceiveArgs.SetBuffer(new byte[lennext], 0, lennext);
                            socketReceiveArgs.Completed += SocketReceiveArgs_Completed;
                            tcpc.ReceiveAsync(socketReceiveArgs);
                            
                            return;
                        }
                            //int b = netc.Buffer[2 + a+1];
                            //temp = System.Text.ASCIIEncoding.ASCII.GetString(netc.Buffer, 2 + a + 1, b);
                            //len = int.Parse(temp);
                            temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                           tempb = null;
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
                        done.Set();  
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
                                //stop();
                                //isreceives = false;
                                timeoutevent();
                                //return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                       done.Set();
                       if (ErrorMge != null)
                            ErrorMge(1, e.Message);
                    }
                }

             
            //}
        }

        void receives(object obj)
        {
            while (isok)
            {
                //System.Threading.Thread.Sleep(50);
                //try
                //{
                //    byte[] tempb = null;
                //    labered:
                //    int bytesRead = tcpc.ReceiveBufferSize;

                //    if (bytesRead > 0)
                //    {
                //        byte[] tempbtye = new byte[bytesRead];
                //        tcpc.Receive(tempbtye);
                //        if (tempb != null)
                //        {
                //            byte[] tempbtyes = new byte[tempbtye.Length + tempb.Length];
                //            Array.Copy(tempb, tempbtyes, tempb.Length);
                //            Array.Copy(tempbtye, 0, tempbtyes, tempb.Length, tempbtye.Length);
                //            tempbtye = tempbtyes;
                //        }
                //        labe881:
                //        if (tempbtye[0] == 0x99)
                //        {
                //            timeout = DateTime.Now;
                //            if (bytesRead > 1)
                //            {
                //                byte[] b = new byte[bytesRead - 1];
                //                byte[] t = tempbtye;
                //                Array.Copy(t, 1, b, 0, b.Length);
                //                tempbtye = b;
                //                bytesRead = bytesRead - (1);
                //            }
                //            continue;
                //        }
                //        int a = tempbtye[1];
                //        String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                //        int len = int.Parse(temp);
                //        if (len > tempbtye.Length)
                //        {
                //            tempb = new byte[tempbtye.Length];
                //            Array.Copy(tempbtye, tempb, tempbtye.Length);
                //            goto labered;
                //        }
                //        //int b = netc.Buffer[2 + a+1];
                //        //temp = System.Text.ASCIIEncoding.ASCII.GetString(netc.Buffer, 2 + a + 1, b);
                //        //len = int.Parse(temp);
                //        temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                //        try
                //        {
                //            if (tempbtye[0] == 0xff)
                //            {
                //                if (temp.IndexOf("token") >= 0)
                //                    Tokan = temp.Split('|')[1];
                //            }
                //            else if (receiveServerEvent != null)
                //                receiveServerEvent(tempbtye[0], temp);
                //        }
                //        catch (Exception e)
                //        {
                //            if (ErrorMge != null)
                //                ErrorMge(1, e.Message);
                //        }

                //        if (bytesRead > (2 + a + len))
                //        {
                //            byte[] b = new byte[bytesRead - (2 + a + len)];
                //            byte[] t = tempbtye;
                //            Array.Copy(t, (2 + a + len), b, 0, b.Length);

                //            tempbtye = b;
                //            bytesRead = bytesRead - (2 + a + len);
                //            goto labe881;
                //        }
                //        timeout = DateTime.Now;
                //    }
                //    else
                //    {
                //        TimeSpan ts = DateTime.Now - timeout;
                //        if (ts.Seconds > mytimeout)
                //        {
                //            Isline = false;
                //            //stop();
                //            //isreceives = false;
                //            timeoutevent();
                //            //return;
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    if (ErrorMge != null)
                //        ErrorMge(1, e.Message);
                //}
            }
        }
    }
}
