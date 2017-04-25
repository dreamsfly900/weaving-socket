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
    public enum DataType { json, bytes };
    public class P2Pclient
    {
        _base_manage xmhelper = new _base_manage();
      
        public TcpClient tcpc;
        public delegate void receive(byte command, String text);
        public event receive receiveServerEvent;
        public delegate void jump(String text);
        public event jump jumpServerEvent;
        public delegate void istimeout();
        public event istimeout timeoutevent;
        public delegate void errormessage(int type, string error);
        DataType DT = DataType.json;
        public event myreceivebit receiveServerEventbit;
        public delegate void myreceivebit(byte command, byte[] data);

        public event errormessage ErrorMge;
        bool isok = false;
        bool isreceives = false;
        bool isline = false;
        DateTime timeout;
        int mytimeout = 90;
        public delegate void P2Preceive(byte command, String data, EndPoint ep);
        public event P2Preceive P2PreceiveEvent;
        UDP udp;
        bool NATUDP = false;
     public   String IP;
       public int PORT;
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
            deleteAttributeInfo(obj.GetType(), obj);
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
        public P2Pclient(DataType _DT)
        {
            DT = _DT;
            this.receiveServerEvent += P2Pclient_receiveServerEvent;
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
        

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
       public string localprot;
        public bool start(string ip, int port, bool takon)
        {
            try
            {
                if (DT == DataType.json && receiveServerEvent == null)
                    throw new Exception("没有注册receiveServerEvent事件");
                if (DT == DataType.bytes && receiveServerEventbit == null)
                    throw new Exception("没有注册receiveServerEventbit事件");
                IP = ip;
                PORT = port;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new TcpClient();
                tcpc.ExclusiveAddressUse = false;
                 
                tcpc.Connect(ip, port);
                localprot = ((System.Net.IPEndPoint)tcpc.Client.LocalEndPoint).Port.ToString();
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

            return send(command, b.Getjson());
        }
        public bool SendRoot<T>(byte command, String Request, T Root, int Querycount)
        {
            _baseModel b = new _baseModel();
            b.Request = Request;
            b.Token = this.Tokan;
            b.SetRoot<T>(Root);
            b.Querycount = Querycount;

            return send(command, b.Getjson());
        }

        public void Send(byte[] b)
        {
            tcpc.Client.Send(b);
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
                int count =(b.Length<=40960? b.Length/40960: (b.Length/40960)+1);
                if (count == 0)
                {
                    tcpc.Client.Send(b);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                       int zz= b.Length - (i * 40960) > 40960 ? 40960 : b.Length - (i * 40960);
                        byte[] temp = new byte[zz];
               
                         Array.Copy(b, i * 40960, temp, 0, zz);
                        tcpc.Client.Send(temp);
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
            catch (Exception ee){

                Isline = false;
                stop();
                timeoutevent();
                send(command, text);
                ErrorMge(9, "send:" + ee.Message);
                return false;

            }
            // tcpc.Close();
            return true;
        }
        public bool send(byte command, byte[] text)
        {

            try
            {
          
                byte[] sendb = text;
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);
                int count = (b.Length <= 40960 ? b.Length / 40960 : (b.Length / 40960) + 1);
                if (count == 0)
                {
                    tcpc.Client.Send(b);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * 40960) > 40960 ? 40960 : b.Length - (i * 40960);
                        byte[] temp = new byte[zz];

                        Array.Copy(b, i * 40960, temp, 0, zz);
                        tcpc.Client.Send(temp);
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
            catch(Exception ee)
            {
                Isline = false;
                stop();
                timeoutevent();
                send(command, text);
                ErrorMge(9, "send:" + ee.Message);
                return false; }
            // tcpc.Close();
            return true;
        }
        public void stop()
        {
          //  isok = false;
            Isline = false;
            tcpc.Close();
        }
        class temppake { public byte command; public string date; public byte [] datebit; }
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
                
                    int count = ListData.Count;

                    if (count > 0)
                    {
                        
                        int bytesRead = ListData[0]!=null? ListData[0].Length:0;
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
                            else {
                              
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
                                                Array.Copy(tempbtye,0, temps2,0, tempbtye.Length);
                                                Array.Copy(temps, 0, temps2, tempbtye.Length, temps.Length);
                                                ListData[0] = temps2; 
                                            }
                                            else
                                            {
                                                System.Threading.Thread.Sleep(20);
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
                                        ErrorMge(3, e.StackTrace + "unup001:" + e.Message+ "2 + a"+ 2 + a+ "---len"+ len+ "--tempbtye"+ tempbtye.Length);
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
                                        ErrorMge(3,e.StackTrace+ "unup122:" + e.Message);
                                }
                            }
                        }
                        else
                        {
                            if (tempbtye[0]==0)
                            ListData.RemoveAt(0);
                        }

                    }

                }
                catch (Exception e)
                {
                   
                    if (ErrorMge != null)
                        ErrorMge(3, "unup:" + e.Message+"---"+e.StackTrace);
                    try
                    {
                        ListData.RemoveAt(0);
                    }
                    catch { }
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
                        timeout = DateTime.Now;
                        byte[] tempbtye = new byte[bytesRead];
                        tcpc.Client.Receive(tempbtye);
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
                            }else
                             continue;
                        }
                        //lock (this)
                        //{
                            ListData.Add(tempbtye);
                       // }
                      
                        
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
                            if (ErrorMge != null)
                                ErrorMge(2,"连接超时，未收到服务器指令");
                            continue;
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
