using WeaveSokect;
using WeaveBase;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace client
{
    public enum DataType { json, bytes };
    public class P2Pclient
    {
        DataType DT = DataType.json;
        public event myreceivebit receiveServerEventbit;
        public delegate void myreceivebit(byte command, byte[] data);
        _base_manage xmhelper = new _base_manage();
        public delegate void jump(String text);
        public event jump jumpServerEvent;
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
                InstallFun myattribute = (InstallFun)mi.GetCustomAttribute(typeof(InstallFun));
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
                InstallFun myattribute = (InstallFun)mi.GetCustomAttribute(typeof(InstallFun));
                if (myattribute == null)
                { }
                else
                {
                    Delegate del = mi.CreateDelegate(typeof(RequestData), obj);
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
        public P2Pclient(DataType _DT)
        {
            DT = _DT;
            this.receiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
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
        private void Xmhelper_errorMessageEvent(Socket soc, BaseDataModel _0x01, string message)
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
            if (DT == DataType.json && receiveServerEvent == null)
                throw new Exception("没有注册receiveServerEvent事件");
            if (DT == DataType.bytes && receiveServerEventbit == null)
                throw new Exception("没有注册receiveServerEventbit事件");
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
                int ss = 0;
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
                        ss = 999999;
                        if (e.SocketError == SocketError.ConnectionAborted)
                    {
                        timeoutevent();
                        //Dispatcher.BeginInvoke(() => MessageBox.Show("连接超时请重试！ " + e.SocketError));
                    }
                    else if (e.SocketError == SocketError.ConnectionRefused)
                    {
                            if (ErrorMge!=null )
                        ErrorMge(0, "服务器端未启动"+ e.SocketError);
                        //Dispatcher.BeginInvoke(() => MessageBox.Show("服务器端问启动" + e.SocketError));
                    }
                    else
                    {
                            if (ErrorMge != null)
                                ErrorMge(0, "出错了"+ e.SocketError);
                        // Dispatcher.BeginInvoke(() => MessageBox.Show("出错了" + e.SocketError));
                    }
                }
                else
                {
                    Isline = true;
                    isok = true;
                    timeout = DateTime.Now;
                    if (!isreceives)
                    {
                        isreceives = true;
                            Task.Factory.StartNew(() => { Receive(null); });
                            Task.Factory.StartNew(() => { unup(); });
                            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Receive));
                        }
                    }
                });
                tcpc.ConnectAsync(socketEventArg);
                if (!takon) return true;
                while (Tokan == null)
                {
                     System.Threading.Tasks.Task.Delay(10000000);
                    if (ss > 0)
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
            BaseDataModel b = new BaseDataModel();
            b.Request = Request;
            b.Token = this.Tokan;
            b.SetParameter<T>(Parameter);
            b.Querycount = Querycount;
            send(command, b.Getjson());
            return true;
        }
        public bool SendRoot<T>(byte command, String Request, T Root, int Querycount)
        {
            BaseDataModel b = new BaseDataModel();
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
                saea.SetBuffer(b, 0, b.Length); 
                tcpc.SendAsync(saea);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }
        public void stop()
        {
            //isok = false;
            Isline = false;
            tcpc.Dispose();
        }
        bool reccomed = true;
        SocketAsyncEventArgs socketReceiveArgs = new SocketAsyncEventArgs();
        public async void Receive(object obj)
        {
            //string received = "";
            socketReceiveArgs = new SocketAsyncEventArgs();
            socketReceiveArgs.SetBuffer(new byte[5120], 0, 5120);
            socketReceiveArgs.Completed += SocketReceiveArgs_Completed;
            while (isok)
            {
                try
                {
                    if (reccomed) 
                    {
                        if (tcpc.ReceiveAsync(socketReceiveArgs))
                        {
                            reccomed = false;
                        }
                    }
                    TimeSpan ts = DateTime.Now - timeout;
                    if (ts.TotalSeconds > mytimeout)
                    {
                        stop();
                        //isreceives = false;
                        timeoutevent();
                        return;
                    }
                    await System.Threading.Tasks.Task.Delay(100);
                }
                catch (Exception e)
                {
                }
            }
        }
        List<Byte[]> listtemp = new List<Byte[]>();
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
        //  ManualResetEvent done = new ManualResetEvent(false);
        class temppake { public byte command; public string date; public byte[] datebit; }
        async void unup()
        {
            while (isok)
            {
                await System.Threading.Tasks.Task.Delay(10);
                try
                {
                    int count = ListData.Count;
                    if (count > 0)
                    {
                        int bytesRead = ListData[0] != null ? ListData[0].Length : 0;
                        if (bytesRead == 0) continue;
                        byte[] tempbtye = new byte[bytesRead];
                        Array.Copy(ListData[0], tempbtye, tempbtye.Length);
                        _0x99:
                        if (tempbtye[0] == 0x99)
                        {
                            if (bytesRead > 1)
                            {
                                byte[] b = new byte[bytesRead - 1];
                                byte[] t = tempbtye;
                                Array.Copy(t, 1, b, 0, b.Length);
                                ListData[0] = b;
                                tempbtye = b;
                                goto _0x99;
                            }
                            else
                            {
                                ListData.RemoveAt(0);
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
                                int len = 0;
                                try
                                {
                                    len = int.Parse(temp);
                                    if (len == 0)
                                    { ListData.RemoveAt(0); continue; }
                                }
                                catch
                                { }
                                labered:
                                try
                                {
                                    if ((len + 2 + a) > tempbtye.Length)
                                    {
                                        if (ListData.Count > 1)
                                        {
                                            ListData.RemoveAt(0);
                                            byte[] temps = new byte[ListData[0].Length];
                                            Array.Copy(ListData[0], temps, temps.Length);
                                            byte[] temps2 = new byte[tempbtye.Length + temps.Length];
                                            Array.Copy(tempbtye, 0, temps2, 0, tempbtye.Length);
                                            Array.Copy(temps, 0, temps2, tempbtye.Length, temps.Length);
                                            ListData[0] = temps2;
                                        }
                                        else
                                        {
                                            await System.Threading.Tasks.Task.Delay(20);
                                        }
                                        continue;
                                    }
                                    else if (tempbtye.Length > (len + 2 + a))
                                    {
                                        byte[] temps = new byte[tempbtye.Length - (len + 2 + a)];
                                        Array.Copy(tempbtye, (len + 2 + a), temps, 0, temps.Length);
                                        ListData[0] = temps;
                                    }
                                    else if (tempbtye.Length == (len + 2 + a))
                                    { ListData.RemoveAt(0); }
                                }
                                catch (Exception e)
                                {
                                    if (ErrorMge != null)
                                        ErrorMge(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                                }
                                try
                                {
                                    if (DT == DataType.json)
                                    {
                                        temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                        temppake str = new temppake();
                                        str.command = tempbtye[0];
                                        str.date = temp;
                                        if (tempbtye[0] == 0xff)
                                        {
                                            if (temp.IndexOf("token") >= 0)
                                                Tokan = temp.Split('|')[1];
                                            else if (temp.IndexOf("jump") >= 0)
                                            {
                                                Tokan = "连接数量满";
                                                jumpServerEvent(temp.Split('|')[1]);
                                            }
                                            else
                                            {
                                                receiveServerEvent(str.command, str.date);
                                                //receiveServerEvent.BeginInvoke(str.command, str.date, null, null);
                                                //System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(rec), str);
                                                //receiveServerEvent(str.command, str.date);
                                                //    = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(rec));
                                                //tt.Start(str);
                                            }
                                        }
                                        else if (receiveServerEvent != null)
                                        {
                                            //
                                            // receiveServerEvent.BeginInvoke(str.command, str.date, null, null);
                                            receiveServerEvent?.Invoke(str.command, str.date);
                                            //System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(rec), str);
                                            //System.Threading.Thread tt = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(rec));
                                            //tt.Start(str);
                                            // receiveServerEvent();
                                        }
                                    }
                                    if (DT == DataType.bytes)
                                    {
                                        // temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                        byte[] bs = new byte[len - 2 + a];
                                        Array.Copy(tempbtye, bs, bs.Length);
                                        temppake str = new temppake();
                                        str.command = tempbtye[0];
                                        str.datebit = bs;
                                        // receiveServerEvent.BeginInvoke(str.command, str.date, null, null);
                                        receiveServerEventbit?.Invoke(str.command, str.datebit);
                                    }
                                    continue;
                                }
                                catch (Exception e)
                                {
                                    if (ErrorMge != null)
                                        ErrorMge(3, e.StackTrace + "unup122:" + e.Message);
                                }
                            }
                        }
                        else
                        {
                            if (tempbtye[0] == 0)
                                ListData.RemoveAt(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (ErrorMge != null)
                        ErrorMge(3, "unup:" + e.Message + "---" + e.StackTrace);
                    try
                    {
                        ListData.RemoveAt(0);
                    }
                    catch { }
                }
            }
        }
       // byte[] tempb = null;
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
                        // }
                    if (bytesRead > 0)
                    {
                        timeout = DateTime.Now;
                        byte[] tempbtye = new byte[bytesRead];
                        timeout = DateTime.Now;
                        Array.Copy(receiveArgs.Buffer, tempbtye, tempbtye.Length);
                        // receiveArgs.Dispose();
                        //if (tempb != null)
                        //{
                        //    byte[] tempbtyes = new byte[tempbtye.Length + tempb.Length];
                        //    Array.Copy(tempb, tempbtyes, tempb.Length);
                        //    Array.Copy(tempbtye, 0, tempbtyes, tempb.Length, tempbtye.Length);
                        //    tempbtye = tempbtyes;
                        //}
                        labe881:
                        if (tempbtye[0] == 0x99)
                        {
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
                            { //done.Set(); 
                                return; }
                            // continue;
                        }
                        ListData.Add(tempbtye);
                        timeout = DateTime.Now;
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - timeout;
                        if (ts.TotalSeconds > mytimeout)
                        {
                            Isline = false;
                            stop();
                            //isreceives = false;
                            timeoutevent();
                            //return;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (ErrorMge != null)
                        ErrorMge(1, e.Message);
                }
                finally {
                    reccomed = true; }
                }
            //}
        }
    }
}
