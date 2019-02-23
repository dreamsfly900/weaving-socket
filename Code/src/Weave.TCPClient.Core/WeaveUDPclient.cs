using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Weave.Base;

namespace Weave.TCPClient
{
    public class WeaveUDPclient
    {
        readonly WeaveBaseManager xmhelper = new WeaveBaseManager();
        public UdpClient tcpc;
        public delegate void receive(byte command, string text);
        public event receive ReceiveServerEvent;
        public delegate void jump(string text);
        public event jump JumpServerEvent;
        public delegate void istimeout();
        public delegate void istimeoutobj(WeaveUDPclient p2pobj);
        public event istimeout Timeoutevent;
        public event istimeoutobj Timeoutobjevent;
        public delegate void errormessage(int type, string error);
        readonly DataType DT = DataType.json;
        public event myreceivebit ReceiveServerEventbit;
        public delegate void myreceivebit(byte command, byte[] data);
        public event errormessage ErrorMge;
        bool isok = false;
        bool isreceives = false;
        DateTime timeout;
        int mytimeout = 90;
        public delegate void P2Preceive(byte command, string data, EndPoint ep);
        public event P2Preceive P2PreceiveEvent;

        public string IP;
        public int PORT;
        public bool Isline { get; set; } = false;

        private readonly List<object> objlist = new List<object>();

        public void AddListenClass(object obj)
        {
            GetAttributeInfo(obj.GetType(), obj);
        }

        public void DeleteListenClass(object obj)
        {
            DeleteAttributeInfo(obj.GetType(), obj);
        }

        public void DeleteAttributeInfo(Type t, object obj)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
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
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute == null)
                { }
                else
                {
                    Delegate del = Delegate.CreateDelegate(typeof(WeaveRequestDataDelegate), obj, mi, true);
                    xmhelper.AddListen(mi.Name, del as WeaveRequestDataDelegate, myattribute.Type);
                }
            }
        }

        public string Tokan { get; set; }

        public List<byte[]> ListData { get; set; } = new List<Byte[]>();

        public WeaveUDPclient()
        {
            ReceiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.WeaveErrorMessageEvent += Xmhelper_errorMessageEvent;

        }

        public WeaveUDPclient(DataType _DT)
        {
            DT = _DT;
            ReceiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.WeaveErrorMessageEvent += Xmhelper_errorMessageEvent;
        }

        private void Xmhelper_errorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
            ErrorMge?.Invoke(0, message);
        }

        private void P2Pclient_receiveServerEvent(byte command, string text)
        {
            try
            {
                xmhelper.Init(text, null);
            }
            catch { }
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

        public string localprot;
        public bool Start(string ip, int port, bool takon)
        {
            try
            {
                if (DT == DataType.json && ReceiveServerEvent == null)
                    throw new Exception("没有注册receiveServerEvent事件");
                if (DT == DataType.bytes && ReceiveServerEventbit == null)
                    throw new Exception("没有注册receiveServerEventbit事件");
                IP = ip;
                PORT = port;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new UdpClient
                {
                    ExclusiveAddressUse = false
                };

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
                    Thread t2 = new Thread(new ThreadStart(KeepAliveHander));
                    t2.Start();
                }
                int ss = 0;
                if (!takon) return true;
                while (Tokan == null)
                {
                    Thread.Sleep(1000);
                    ss++;
                    if (ss > 10)
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Isline = false;
                ErrorMge?.Invoke(1, e.Message);
                return false;
            }
        }

        public void KeepAliveHander()
        {
            while (true)
            {
                Thread.Sleep(8000);
                try
                {
                    IPEndPoint server = new IPEndPoint(IPAddress.Parse(IP), PORT);
                    EndPoint ep = server;
                    tcpc.Client.SendTo(new byte[] { 0x99 }, ep);
                }
                catch { }
            }
        }

        public int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret += (item << i);
                i += 8;
            }
            return ret;
        }

        public byte[] ConvertToByteList(int v)
        {
            List<byte> ret = new List<byte>();
            int value = v;
            while (value != 0)
            {
                ret.Add((byte)value);
                value >>= 8;
            }
            byte[] bb = new byte[ret.Count];
            ret.CopyTo(bb);
            return bb;
        }

        private void Udp_receiveevent(byte command, string data, EndPoint iep)
        {
            P2PreceiveEvent?.Invoke(command, data, iep);
        }

        public bool SendParameter<T>(byte command, string Request, T Parameter, int Querycount)
        {
            WeaveSession b = new WeaveSession
            {
                Request = Request,
                Token = Tokan
            };
            b.SetParameter(Parameter);
            b.Querycount = Querycount;
            return Send(command, b.Getjson());
        }

        public bool SendRoot<T>(byte command, string Request, T Root, int Querycount)
        {
            WeaveSession b = new WeaveSession
            {
                Request = Request,
                Token = Tokan
            };
            b.SetRoot(Root);
            b.Querycount = Querycount;
            return Send(command, b.Getjson());
        }

        public void Send(byte[] b)
        {
            tcpc.Client.Send(b);
        }
        
        public bool Send(byte command, string text)
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
                int count = (b.Length <= 520 ? b.Length / 520 : (b.Length / 520) + 1);
                if (count == 0)
                {
                    IPEndPoint server = new IPEndPoint(IPAddress.Parse(IP), PORT);
                    EndPoint ep = server;
                    tcpc.Client.SendTo(b, ep);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * 520) > 520 ? 520 : b.Length - (i * 520);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * 520, temp, 0, zz);
                        IPEndPoint server = new IPEndPoint(IPAddress.Parse(IP), PORT);
                        EndPoint ep = server;
                        tcpc.Client.SendTo(temp, ep);

                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception ee)
            {
                Isline = false;
                Stop();
                Timeoutevent?.Invoke();
                Timeoutobjevent?.Invoke(this);
                Send(command, text);
                ErrorMge(9, "send:" + ee.Message);
                return false;
            }
            return true;
        }

        public bool Send(byte command, byte[] text)
        {
            try
            {
                byte[] sendb = text;
                byte[] lens = ConvertToByteList(sendb.Length);
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                int count = (b.Length <= 520 ? b.Length / 520 : (b.Length / 520) + 1);
                if (count == 0)
                {
                    IPEndPoint server = new IPEndPoint(IPAddress.Parse(IP), PORT);
                    EndPoint ep = server;
                    tcpc.Client.SendTo(b, ep);
                }
                else
                {
                    throw new Exception("发送数据不得大于520byte");
                }
            }
            catch (Exception ee)
            {
                Isline = false;
                Stop();
                Timeoutevent?.Invoke();
                Timeoutobjevent?.Invoke(this);
                Send(command, text);
                ErrorMge(9, "send:" + ee.Message);
                return false;
            }
            return true;
        }

        public void Stop()
        {
            Isline = false;
            tcpc.Close();
        }

        class Temppake { public byte command; public string date; public byte[] datebit; }

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
                    int count = ListData.Count;
                    if (count > 0)
                    {
                        int bytesRead = ListData[0] != null ? ListData[0].Length : 0;
                        if (bytesRead == 0) continue;
                        byte[] tempbtye = new byte[bytesRead];
                        Array.Copy(ListData[0], tempbtye, tempbtye.Length);

                        if (bytesRead > 2)
                        {
                            int a = tempbtye[1];
                            if (bytesRead > 2 + a)
                            {
                                int len = 0;
                                if (DT == DataType.bytes)
                                {
                                    byte[] bb = new byte[a];
                                    Array.Copy(tempbtye, 2, bb, 0, a);
                                    len = ConvertToInt(bb);
                                }
                                else
                                {
                                    string temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                                    len = 0;
                                    try
                                    {
                                        len = int.Parse(temp);
                                        if (len == 0)
                                        { ListData.RemoveAt(0); continue; }
                                    }
                                    catch
                                    { }
                                }

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
                                            Thread.Sleep(20);
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
                                    ErrorMge?.Invoke(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                                }
                                try
                                {
                                    if (len == 0)
                                    {
                                        ListData.RemoveAt(0);
                                        return;
                                    }
                                    if (DT == DataType.json)
                                    {
                                        string temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                        Temppake str = new Temppake
                                        {
                                            command = tempbtye[0],
                                            date = temp
                                        };
                                        if (tempbtye[0] == 0xff)
                                        {
                                            if (temp.IndexOf("token") >= 0)
                                                Tokan = temp.Split('|')[1];
                                            else if (temp.IndexOf("jump") >= 0)
                                            {
                                                Tokan = "连接数量满";
                                                JumpServerEvent(temp.Split('|')[1]);
                                            }
                                            else
                                            {
                                                ThreadPool.QueueUserWorkItem(new WaitCallback(Rec), str);
                                            }
                                        }
                                        else if (ReceiveServerEvent != null)
                                        {
                                            ThreadPool.QueueUserWorkItem(new WaitCallback(Rec), str);
                                        }
                                    }
                                    if (DT == DataType.bytes)
                                    {
                                        byte[] bs = new byte[len - 2 + a];
                                        Array.Copy(tempbtye, bs, bs.Length);
                                        Temppake str = new Temppake
                                        {
                                            command = tempbtye[0],
                                            datebit = bs
                                        };
                                        ReceiveServerEvent.BeginInvoke(str.command, str.date, null, null);
                                    }
                                    continue;
                                }
                                catch (Exception e)
                                {
                                    ErrorMge?.Invoke(3, e.StackTrace + "unup122:" + e.Message);
                                }
                            }
                        }
                        else
                        {
                            ListData.RemoveAt(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorMge?.Invoke(3, "unup:" + e.Message + "---" + e.StackTrace);
                    try
                    {
                        ListData.RemoveAt(0);
                    }
                    catch { }
                }
            }
        }

        void Receives(object obj)
        {
            while (isok)
            {
                Thread.Sleep(50);
                try
                {

                    int bytesRead = tcpc.Available;
                    if (bytesRead > 0)
                    {
                        byte[] tempbtye = new byte[bytesRead];
                        try
                        {
                            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                            EndPoint Remote = sender;
                            timeout = DateTime.Now;

                            tcpc.Client.ReceiveFrom(tempbtye, ref Remote);

                        _0x99:
                            if (tempbtye[0] == 0x99)
                            {
                                timeout = DateTime.Now;
                                if (tempbtye.Length > 1)
                                {
                                    byte[] b = new byte[bytesRead - 1];
                                    try
                                    {
                                        Array.Copy(tempbtye, 1, b, 0, b.Length);
                                    }
                                    catch { }
                                    tempbtye = b;
                                    goto _0x99;
                                }
                                else
                                    continue;
                            }
                        }
                        catch (Exception ee)
                        {
                            ErrorMge(22, ee.Message);
                        }
                        ListData.Add(tempbtye);
                    }
                    else
                    {
                        try
                        {
                            TimeSpan ts = DateTime.Now - timeout;
                            if (ts.TotalSeconds > mytimeout)
                            {
                                Isline = false;
                                Stop();
                                Timeoutevent?.Invoke();
                                Timeoutobjevent?.Invoke(this);
                                ErrorMge?.Invoke(2, "连接超时，未收到服务器指令");
                                continue;
                            }
                        }
                        catch (Exception ee)
                        {
                            ErrorMge(21, ee.Message);
                        }
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
