using client;
using MyInterface;
using P2P;
using StandardModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
namespace cloud
{
    public delegate void Mylog(string type, string log);
    public  class GateWay
    {
        protected ITcpBasehelper p2psev;
     
        List<CommandItem> listcomm = new List<CommandItem>();
        QueueTable qt = new QueueTable();
       
        public event Mylog EventMylog;
        public List<ConnObj> ConnObjlist = new List<ConnObj>();
        public   List<CommandItem> CommandItemS = new List<CommandItem>();
        public List<WayItem> WayItemS = new List<WayItem>();

        protected p2psever p2psev2;

        public GateWay(bool IsWeb)
        {
            if(IsWeb)
                p2psev = new Webp2psever();
            else
            p2psev = new p2psever();
        }
        public bool Run(string loaclIP,int port,int port2)
        {
            // Mycommand comm = new Mycommand(, connectionString);


            ReLoad();

          

            p2psev.receiveevent += p2psev_receiveevent;
            p2psev.EventUpdataConnSoc += p2psev_EventUpdataConnSoc;
            p2psev.EventDeleteConnSoc += p2psev_EventDeleteConnSoc;
            //   p2psev.NATthroughevent += tcp_NATthroughevent;//p2p事件，不需要使用
            p2psev.start(Convert.ToInt32(port));
            p2psev2 = new p2psever(loaclIP);
            p2psev2.EventDeleteConnSoc += P2psev2_EventDeleteConnSoc;
            p2psev2.EventUpdataConnSoc += P2psev2_EventUpdataConnSoc;
            p2psev2.receiveevent += P2psev2_receiveevent;
            //   p2psev.NATthroughevent += tcp_NATthroughevent;//p2p事件，不需要使用
            p2psev2.start(Convert.ToInt32(port2));
            if (EventMylog != null)
                EventMylog("连接", "连接启动成功");
            return true;
        }
        string token = "";
        protected void P2psev2_receiveevent(byte command, string data, Socket soc)
        {
            try
            {
                _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(data);

                if (command == 0xff)
                {
                    if (_0x01.Request == "token")
                    {
                        token = _0x01.Root;
                    }
                     else if (_0x01.Request == "getnum")
                    {
                        _0x01.Request = "setnum";
                        _0x01.Token = token;
                        _0x01.Root = ConnObjlist.Count.ToString();
                        p2psev2.send(soc, 0xff, _0x01.Getjson());
                    }
                  
                }
            }
            catch { }
        }

        protected void P2psev2_EventUpdataConnSoc(Socket soc)
        {
            
        }

        protected void P2psev2_EventDeleteConnSoc(Socket soc)
        {
            
        }

        public void ReLoad()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFlies));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFliesway));
            
        }

        protected void V_ErrorMge(int type, string error)
        {
            if (EventMylog != null)
                EventMylog("V_ErrorMge", type+":"+error);
        }
        /// <summary>
        /// 这里写断线后操作
        /// </summary>
        protected void V_timeoutevent()
        {
            //int count = CommandItemS.Count;
            //CommandItem[] comItems = new CommandItem[count];
            //CommandItemS.CopyTo(0, comItems, 0, count);
            try
            {
                foreach (CommandItem ci in CommandItemS)
                {
                    if (!ci.Client.Isline)
                    {
                        if (ci.Client.Restart())
                        {
                            V_timeoutevent();
                        }
                    }
                }
            }
            catch(Exception ex) {
                if (EventMylog != null)
                    EventMylog("节点重新连接", ex.Message);
                V_timeoutevent(); }
        }
        /// <summary>
        /// 这里写接收到内容后，转发
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        protected void V_receiveServerEvent(byte command, string text)
        {
            _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(text);
          
            try
            {
                int count = ConnObjlist.Count;
                ConnObj[] coobjs = new ConnObj[count];
                ConnObjlist.CopyTo(0, coobjs, 0, count);
                foreach (ConnObj coob in coobjs)
                {
                    if(coob!=null)
                    if (coob.Token == _0x01.Token)
                    {
                        p2psev.send(coob.Soc, command, text);
                    }
                }
            }
            catch(Exception ex) { EventMylog("转发", ex.Message); }
           
        }





        protected void ReloadFlies(object obj)
        {
            try
            {
                
                foreach (CommandItem ci in CommandItemS)
                {
                    ci.Client.stop();
                }
                CommandItemS.Clear();
                XmlDocument xml = new XmlDocument();
                xml.Load("node.xml");
                foreach (XmlNode xn in xml.ChildNodes)
                {


                    CommandItem ci = new CommandItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port =Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.CommName = byte.Parse(xn.Attributes["command"].Value);
                    ci.Client = new P2Pclient(false);
                    ci.Client.receiveServerEvent += V_receiveServerEvent;
                    ci.Client.timeoutevent += V_timeoutevent;
                    ci.Client.ErrorMge += V_ErrorMge;
                    if (ci.Client.start(ci.Ip, ci.Port))
                    {
                        CommandItemS.Add(ci);
                    }
                    else
                    {
                        if (EventMylog != null)
                            EventMylog("节点连接失败", "命令："+ci.CommName + ":节点连接失败，抛弃此节点");
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }

        protected void ReloadFliesway(object obj)
        {
            try
            {

                foreach (WayItem ci in WayItemS)
                {
                    ci.Client.stop();
                }
                WayItemS.Clear();
                XmlDocument xml = new XmlDocument();
                xml.Load("nodeway.xml");
                foreach (XmlNode xn in xml.ChildNodes)
                {


                    WayItem ci = new WayItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.Token =  (xn.Attributes["token"].Value);
                    ci.Client = new P2Pclient(false);
                    ci.Client.receiveServerEvent += Client_receiveServerEvent;
                    ci.Client.timeoutevent += Client_timeoutevent;
                    ci.Client.ErrorMge += Client_ErrorMge;
                    if (ci.Client.start(ci.Ip, ci.Port))
                    {
                        _baseModel oxff = new _baseModel();
                        oxff.Request = "token";
                        oxff.Root = ci.Token;
                        ci.Client.send(0xff, oxff.Getjson());
                        WayItemS.Add(ci);
                    }
                    else
                    {
                        if (EventMylog != null)
                            EventMylog("从网关连接失败", "从网关：" + ci.Ip + ":节点连接失败，抛弃此节点");
                    }
                }

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(getwaynum));
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }
     public   void getwaynum(object obj)
        {
            while (true)
            {
                try
                {
                    int count = WayItemS.Count;
                    WayItem[] coobjs = new WayItem[count];
                    WayItemS.CopyTo(0, coobjs, 0, count);
                    foreach (WayItem wi in coobjs)
                    {
                        _baseModel oxff = new _baseModel();
                        oxff.Request = "getnum";
                        oxff.Root = wi.Token;
                        wi.Client.send(0xff, oxff.Getjson());
                    }
                }
                catch { }
                System.Threading.Thread.Sleep(1000);
            }
        }
        protected void Client_ErrorMge(int type, string error)
        {
            
        }

        protected void Client_timeoutevent()
        {
            try
            {
                foreach (WayItem ci in WayItemS)
                {
                    if (!ci.Client.Isline)
                    {
                        if (ci.Client.Restart())
                        {
                            Client_timeoutevent();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("节点重新连接", ex.Message);
                Client_timeoutevent();
            }
        }

        protected void Client_receiveServerEvent(byte command, string text)
        {
            try
            {
                _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(text);

                if (_0x01.Request == "setnum")
                {
                    int count = WayItemS.Count;
                    WayItem[] coobjs = new WayItem[count];
                    WayItemS.CopyTo(0, coobjs, 0, count);
                    foreach (WayItem wi in coobjs)
                    {
                        if (wi.Token == _0x01.Token)
                            wi.Num = int.Parse(_0x01.Root);
                    }
                }
            }
            catch { }
        }

        protected void p2psev_EventDeleteConnSoc(System.Net.Sockets.Socket soc)
        {
            try {
                int count = ConnObjlist.Count;
                ConnObj[] coobjs = new ConnObj[count];
                ConnObjlist.CopyTo(0, coobjs, 0, count);
                foreach (ConnObj coob in coobjs)
                {
                    if (coob != null)
                        if (coob.Soc.Equals(soc))
                    {
                            ConnObjlist.Remove(coob);
                            try
                            {
                                count = CommandItemS.Count;
                                CommandItem[] comItems = new CommandItem[count];
                                CommandItemS.CopyTo(0, comItems, 0, count);
                          
                            foreach (CommandItem ci in comItems)
                                {
                                   
                                    ci.Client.send(0xff, "out|"+ coob.Token);
                                }
                            }
                            catch { }
                        
                        }
                }
               } catch(Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }

        protected void p2psev_EventUpdataConnSoc(System.Net.Sockets.Socket soc)
        {
            ConnObj cobj = new ConnObj();
            cobj.Soc = soc;
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
            cobj.Token = EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
           
            try
            {
                if(p2psev.send(soc,0xff,"token|" + cobj.Token + ""))
                ConnObjlist.Add(cobj);
               int count = CommandItemS.Count;
                CommandItem[] comItems = new CommandItem[count];
                CommandItemS.CopyTo(0, comItems, 0, count); 
                foreach (CommandItem ci in comItems)
                {
                     
                    ci.Client.send(0xff, "in|"+ cobj.Token);
                }
            }
            catch(Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("EventUpdataConnSoc", ex.Message);
            }
        }


        protected void p2psev_receiveevent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(data);
            string key = DecryptDES(_0x01.Token, "lllssscc");
            string ip = key.Split('|')[0];
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;

            if (clientipe.Address.ToString() == ip)
            {
                int count = CommandItemS.Count;
                CommandItem[] comItems = new CommandItem[count];
                CommandItemS.CopyTo(0, comItems, 0, count);
                foreach (CommandItem ci in comItems)
                {
                    if (ci != null)
                    {
                        if (ci.CommName == command)
                        {
                            if (!ci.Client.send(command, data))
                            {
                                p2psev.send(soc, 0xff, "你所请求的服务暂不能使用，请联系管理人员。");
                            }
                            return;
                        }
                    }
                }
                p2psev.send(soc, 0xff, "你所请求的服务是不存在的。");

            }
            else {
                p2psev.send(soc, 0xff, "您的请求是非法的~");
            }

        }
        private byte[] Keys = { 0xEF, 0xAB, 0x56, 0x78, 0x90, 0x34, 0xCD, 0x12 };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
         

    }
    public class CommandItem
    {
        byte commName;

        public byte CommName
        {
            get { return commName; }
            set { commName = value; }
        }
      
        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public P2Pclient Client
        {
            get
            {
                return client;
            }

            set
            {
                client = value;
            }
        }
       

        String ip = "";
        int port;
        P2Pclient client;
    }
    public class WayItem
    {
        string _Token;
        int num;

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public P2Pclient Client
        {
            get
            {
                return client;
            }

            set
            {
                client = value;
            }
        }

        public int Num
        {
            get
            {
                return num;
            }

            set
            {
                num = value;
            }
        }

        public string Token
        {
            get
            {
                return _Token;
            }

            set
            {
                _Token = value;
            }
        }

        String ip = "";
        int port;
        P2Pclient client;
    }
    public class ConnObj
    {
        byte[] makes;
        public byte[] Makes
        {
            get
            {
                return makes;
            }

            set
            {
                makes = value;
            }
        }
        Socket soc;
        string _Token;

        public Socket Soc
        {
            get
            {
                return soc;
            }

            set
            {
                soc = value;
            }
        }

        public string Token
        {
            get
            {
                return _Token;
            }

            set
            {
                _Token = value;
            }
        }
    }

}
