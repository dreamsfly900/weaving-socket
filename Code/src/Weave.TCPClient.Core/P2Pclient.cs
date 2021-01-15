using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Weave.Base;

namespace Weave.TCPClient
{
    public class P2Pclient
    {
        public int resttime = 1;
        readonly WeaveBaseManager xmhelper = new WeaveBaseManager();
        public TcpClient tcpc;
        public delegate void receive(byte command, string text);
        public event receive ReceiveServerEvent;
        public delegate void receiveobj(byte command, string text, P2Pclient soc);
        public event receiveobj ReceiveServerEventobj;
        public event myreceivebitobj ReceiveServerEventbitobj;
        public delegate void myreceivebitobj(byte command, byte[] data, P2Pclient soc);
        public delegate void jump(string text);
        public event jump JumpServerEvent;
        public delegate void istimeout();
        public delegate void istimeoutobj(P2Pclient p2pobj);
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
        public byte defaultCommand = 0x0;
        private readonly bool NATUDP = false;
        public string IP;
        public int PORT;
        public WeaveReceivesSpeedMode ReceivesSpeedMode= WeaveReceivesSpeedMode.high;
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

        byte[] alldata = new byte[0];

        public P2Pclient(bool _NATUDP)
        {
            ReceiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.WeaveErrorMessageEvent += Xmhelper_errorMessageEvent;
            NATUDP = _NATUDP;

        }

        public P2Pclient(DataType _DT)
        {
            DT = _DT;
            this.ReceiveServerEvent += P2Pclient_receiveServerEvent;
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
                if (xmhelper.listmode.Count > 0)
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
                acallsend = new AsyncCallback(SendDataEnd);
                if (DT == DataType.json && (ReceiveServerEvent == null && ReceiveServerEventobj == null))
                    throw new Exception("没有注册receiveServerEvent事件");
                if (DT == DataType.bytes && (ReceiveServerEventbit == null && ReceiveServerEventbitobj == null))
                    throw new Exception("没有注册receiveServerEventbit事件");
                if (DT == DataType.custom && (ReceiveServerEventbit == null && ReceiveServerEventbitobj == null))
                    throw new Exception("没有注册receiveeventbit事件");
                IP = ip;
                PORT = port;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new TcpClient
                {
                   // ExclusiveAddressUse = false,
                    ReceiveBufferSize = int.MaxValue
                };
                tcpc.Connect(ip, port);
                localprot = ((IPEndPoint)tcpc.Client.LocalEndPoint).Port.ToString();
                Isline = true;
                isok = true;

                timeout = DateTime.Now;
                if (!isreceives)
                {
                    isreceives = true;
                    Thread t = new Thread(new ParameterizedThreadStart(Receives));
                    t.Start();
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

        private void SendDataEnd(IAsyncResult ar)
        {
            try
            {
                ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch
            {

            }
        }

        AsyncCallback acallsend;

        public bool Send(byte[] b)
        {
            try
            {
                tcpc.Client.BeginSend(b, 0, b.Length, SocketFlags.None, acallsend, tcpc.Client);
                timeout = DateTime.Now;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Send(byte command, string text)
        {
            try
            {
                if (DT == DataType.json)
                {
                    byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                    byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                    byte[] b = new byte[2 + lens.Length + sendb.Length];
                    b[0] = command;
                    b[1] = (byte)lens.Length;
                    lens.CopyTo(b, 2);
                    sendb.CopyTo(b, 2 + lens.Length);
                    int count = (b.Length <= 40960 ? b.Length / 40960 : (b.Length / 40960) + 1);
                    Send(b);
                }
                else if (DT == DataType.bytes)
                {
                    return Send(command, System.Text.Encoding.UTF8.GetBytes(text));
                }
                else
                    return Send(System.Text.Encoding.UTF8.GetBytes(text));
               
            }
            catch (Exception ee)
            {
                if (Isline)
                {
                    Timeoutevent?.Invoke();
                    Timeoutobjevent?.Invoke(this);
                    Send(command, text);
                }
                Isline = false;
                Stop();

                ErrorMge(9, "send:" + ee.Message);
                return false;
            }
            return true;
        }

        public bool Send(byte command, byte[] text)
        {
            bool bb = false;
            try
            {
                if (DT == DataType.json)
                {
                   return Send(command, System.Text.Encoding.UTF8.GetString(text));
                }
                else if (DT == DataType.bytes)
                {

                    byte[] sendb = text;
                    byte[] lens = ConvertToByteList(sendb.Length);
                    byte[] b = new byte[2 + 2 + lens.Length + sendb.Length];
                    b[0] = command;
                    b[1] = (byte)lens.Length;
                    lens.CopyTo(b, 2);
                    CRC.ConCRC(ref b, 2 + lens.Length);
                    sendb.CopyTo(b, 2 + 2 + lens.Length);
                    bb = Send(b);
                   
                }else
                    return Send(text);

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
            return bb;
        }

        public void Stop()
        {
            isok = false;
            Isline = false;
            tcpc.Close();
            alldata = new byte[0];
        }

        byte[] tempp = new byte[0];
         void Unup()
        {
            if (DT == DataType.custom)
            {
                int bytesRead = alldata.Length;

                if (bytesRead == 0)
                {
                    return;
                }
                byte[] tempbtye = new byte[bytesRead];

                Array.Copy(alldata, tempbtye, tempbtye.Length);
                command cc = new command();
                cc.comm = defaultCommand;
                cc.bs = tempbtye;
             
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(Eventbit), cc);
                //ReceiveServerEventbit?.BeginInvoke(defaultCommand, tempbtye, null, null);
                //ReceiveServerEventbitobj?.BeginInvoke(defaultCommand, tempbtye, this, null, null);

                alldata = new byte[0];
                return;
            }
            else if (DT == DataType.json)
            {
                Unupjson();
                return;
            }
            else if (DT == DataType.bytes)
            {
                Unupbyte();
                return;
            }
        }

        void Unupjson()
        {
            try
            {
                {
                 
                    int bytesRead = alldata.Length;

                    if (bytesRead == 0)
                    {
                        return;
                    }


                    byte[] tempbtye = new byte[bytesRead];

                    Array.Copy(alldata, tempbtye, tempbtye.Length);
                    //if (tempbtye[0] == 0x99)
                    //{
                    //    timeout = DateTime.Now;
                    //    if (tempbtye.Length > 1)
                    //    {
                    //        byte[] b = new byte[bytesRead - 1];
                    //        try
                    //        {
                    //            Array.Copy(tempbtye, 1, b, 0, b.Length);
                    //        }
                    //        catch { }
                    //        alldata = b;
                    //        return;
                    //    }
                    //}

                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (bytesRead > 2 + a)
                        {
                            int len = 0;

                            string temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                            len = 0;
                            try
                            {
                                len = int.Parse(temp);
                                if (len == 0)
                                { alldata = new byte[0]; return; }
                            }
                            catch
                            {
                                byte[] temps = new byte[tempbtye.Length - 1];
                                Array.Copy(tempbtye,1, temps, 0, temps.Length);
                                alldata = temps;
                                return;
                            }

                            try
                            {
                                if ((len + 2 + a) > tempbtye.Length)
                                {

                                    return;
                                }
                                else if (tempbtye.Length > (len + 2 + a))
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 2 + a)];
                                    Array.Copy(tempbtye, (len + 2 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                  //  goto lb0x99;
                                }
                                else if (tempbtye.Length == (len + 2 + a))
                                { alldata = new byte[0]; }
                            }
                            catch (Exception e)
                            {
                                ErrorMge?.Invoke(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                                alldata = new byte[0];
                            }
                            try
                            {

                                temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);

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
                                        //ReceiveServerEvent?.Invoke(tempbtye[0], temp);

                                        ReceiveServerEvent?.BeginInvoke(tempbtye[0], temp,null, null);
                                        ReceiveServerEventobj?.BeginInvoke(tempbtye[0], temp, this, null, null);
                                    }
                                }
                                else if (tempbtye[0] == 0x99)
                                    return;
                                else if (ReceiveServerEvent != null || ReceiveServerEventobj != null)
                                {
                                    command cc = new command();
                                    cc.comm = tempbtye[0];
                                    cc.str = temp;
                                    System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(Eventrec), cc);
                                    //ReceiveServerEvent?.BeginInvoke(tempbtye[0], temp, null, null);
                                    //ReceiveServerEventobj?.BeginInvoke(tempbtye[0], temp, this, null, null);
                                }



                            }
                            catch (Exception e)
                            {
                                ErrorMge?.Invoke(3, e.StackTrace + "unup122:" + e.Message);
                                alldata = new byte[0];
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMge?.Invoke(3, "unup:" + e.Message + "---" + e.StackTrace);
                alldata = new byte[0];
            }
        }

        void Unupbyte()
        {
            try
            {
                {
                lb0x99:
                    int bytesRead = alldata.Length;

                    if (bytesRead == 0)
                    {
                        return;
                    }

                    byte[] tempbtye = new byte[bytesRead];

                    Array.Copy(alldata, tempbtye, tempbtye.Length);
                    //if (tempbtye[0] == 0x99)
                    //{
                    //    timeout = DateTime.Now;
                    //    if (tempbtye.Length > 1)
                    //    {
                    //        byte[] b = new byte[bytesRead - 1];
                    //        try
                    //        {
                    //            Array.Copy(tempbtye, 1, b, 0, b.Length);
                    //        }
                    //        catch { }
                    //        alldata = b;
                    //        goto lb0x99;
                    //    }
                    //}

                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (bytesRead > 4 + a)
                        {
                            int len = 0;

                            byte[] bbcrc = new byte[4 + a];
                            Array.Copy(tempbtye, 0, bbcrc, 0, 4 + a);
                            if (CRC.DataCRC(ref bbcrc, 4 + a))
                            {
                                byte[] bb = new byte[a];
                                Array.Copy(tempbtye, 2, bb, 0, a);
                                len = ConvertToInt(bb);
                            }
                            else
                            {
                                byte[] temps = new byte[tempbtye.Length - 1];
                                Array.Copy(tempbtye, 1, temps, 0, temps.Length);
                                alldata = temps;
                                //return;
                                  goto lb0x99;
                            }
                            try
                            {
                                if ((len + 4 + a) > tempbtye.Length)
                                {
                                    return;
                                }
                                else if (tempbtye.Length > (len + 4 + a))
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 4 + a)];
                                    Array.Copy(tempbtye, (len + 4 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                    //return;
                                    //  goto lb0x99;
                                }
                                else if (tempbtye.Length == (len + 4 + a))
                                { alldata = new byte[0]; }
                            }
                            catch (Exception e)
                            {
                                ErrorMge?.Invoke(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                                alldata = new byte[0];
                            }
                            try
                            {
                                byte[] bs = new byte[len];
                                Array.Copy(tempbtye, (4 + a), bs, 0, bs.Length);
                                if (tempbtye[0] == 0x99)
                                    return;

                                //ReceiveServerEventbit?.BeginInvoke(tempbtye[0], bs,null, null);
                                //ReceiveServerEventbitobj?.BeginInvoke(tempbtye[0], bs, this, null, null);
                                command cc = new command();
                                cc.comm = tempbtye[0];
                                cc.bs = bs;
                                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(Eventbit), cc);

                                return;
                            }
                            catch (Exception e)
                            {
                                ErrorMge?.Invoke(3, e.StackTrace + "unup122:" + e.Message);
                                alldata = new byte[0];
                            }
                          
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMge?.Invoke(3, "unup:" + e.Message + "---" + e.StackTrace);
                alldata = new byte[0];
            }
        }
        void Eventrec(object obj)
        {
            command cc = obj as command;
            ReceiveServerEvent?.Invoke(cc.comm, cc.str);
            ReceiveServerEventobj?.Invoke(cc.comm, cc.str, this);
        }
        class command{ public byte comm; public byte[] bs; public string str = ""; }
        void Eventbit(object obj)
        {
            command cc = obj as command;
            ReceiveServerEventbit?.Invoke(cc.comm, cc.bs);
            ReceiveServerEventbitobj?.Invoke(cc.comm, cc.bs, this);
        }
        void Receives(object obj)
        {
          //  var w = new SpinWait();
            while (isok)
            {
                if (ReceivesSpeedMode != WeaveReceivesSpeedMode.high)
                    //sleep(10) 
                    System.Threading.Thread.Sleep((int)ReceivesSpeedMode);
                try
                {
                    if (tcpc.Client == null)
                    {
                        
                        continue;
                      
                    }
                    int bytesRead = tcpc.Client.Available;
                    if (bytesRead > 0)
                    {
                        byte[] tempbtye = new byte[bytesRead];
                        try
                        {
                            timeout = DateTime.Now;

                            tcpc.Client.Receive(tempbtye);
                            tempp = new byte[alldata.Length];
                            alldata.CopyTo(tempp, 0);
                            int lle = alldata.Length;
                            bytesRead = tempbtye.Length;
                            byte[] temp = new byte[lle + bytesRead];
                            Array.Copy(alldata, 0, temp, 0, lle);
                            Array.Copy(tempbtye, 0, temp, lle, bytesRead);
                            alldata = temp;
                        }
                        catch (Exception ee)
                        {
                            ErrorMge(22, ee.Message);
                        }
                      
                    }


                    if (alldata.Length > 3)
                    {

                        Unup();
                    }
                    else
                    {
                        if (tcpc.Client.Available == 0)
                            if (resttime > 0)
                                System.Threading.Thread.Sleep(1);
                        else
                            System.Threading.Thread.Yield();
                    }

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
                catch (Exception e)
                {
                    ErrorMge?.Invoke(2, e.Message);
                }
            }
        }
    }
}
