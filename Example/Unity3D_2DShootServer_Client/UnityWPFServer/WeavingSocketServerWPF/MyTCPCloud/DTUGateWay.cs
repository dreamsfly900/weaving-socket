
using WeaveBase;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using WeaveSocketServer;
using WeaveClient;

namespace MyTCPCloud
{
    public  class DTUGateWay
    {
        DTUServer DTUSer;
        public int V_ErrorMge { get; private set; }
        public List<CommandItem> CommandItemS = new List<CommandItem>();
        public List<CommandItem> CommandItemS2 = new List<CommandItem>();
        public event Mylog EventMylog;
        public DTUGateWay()
        {
            DTUSer = new DTUServer();
        }
        public bool Run( int port)
        {
            // Mycommand comm = new Mycommand(, connectionString);
            ReLoad();
            DTUSer.weaveDeleteSocketListEvent += DTUSer_EventDeleteConnSoc;
            DTUSer.weaveUpdateSocketListEvent += DTUSer_EventUpdataConnSoc;
            DTUSer.weaveReceiveDtuEvent += DTUSer_receiveeventDtu;
            DTUSer.start(port);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFliesdtu), null);
            return true;
        }
        int _port = 0;
        string filename;
        public bool Run(int port, String _filename)
        {
            filename = _filename;
            _port = port;
            ReloadFlies2(null);
            DTUSer.weaveDeleteSocketListEvent += DTUSer_EventDeleteConnSoc;
            DTUSer.weaveUpdateSocketListEvent += DTUSer_EventUpdataConnSoc;
            DTUSer.weaveReceiveDtuEvent += DTUSer_receiveeventDtu;
            DTUSer.start(port);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFliesdtu), null);
            return true;
        }
        private void DTUSer_receiveeventDtu(byte[] data, System.Net.Sockets.Socket soc)
        {
            foreach (CommandItem ci in CommandItemS)
            {
                foreach (P2Pclient Client in ci.Client)
                {
                    IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
                    Client.Tokan = clientipe.Address.ToString() + "|" + clientipe.Port;
                    Client.SendRoot<byte[]>(ci.CommName, ci.Commfun, data, 0);
                }
            }
        }
        List<ConnObj> ConnObjlist = new List<ConnObj>();
        private void DTUSer_EventUpdataConnSoc(System.Net.Sockets.Socket soc)
        {
            ConnObj cobj = new ConnObj();
            cobj.Soc = soc;
            //IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
            //cobj.Token = EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
            try
            {
                IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
                cobj.Token = clientipe.Address.ToString() + "|" + clientipe.Port;// EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
                ConnObjlist.Add(cobj);
            }
            catch { }
        }
        private void DTUSer_EventDeleteConnSoc(System.Net.Sockets.Socket soc)
        {
            ConnObj[] coobjs = new ConnObj[0];
            int count = 0;
            try
            {
                count = ConnObjlist.Count;
                coobjs = new ConnObj[count];
                ConnObjlist.CopyTo(coobjs);
            }
            catch { }
            foreach (ConnObj coob in coobjs)
            {
                if (coob != null)
                    if (coob.Soc.Equals(soc))
                    {
                        ConnObjlist.Remove(coob);
                    }
            }
        }
      public  List<dtuclient> listdtu = new List<dtuclient>();
        private void ReLoad()
        {
            ReloadFlies(null);
          //  ReloadFlies2(null);
        }
        protected void ReloadFlies(object obj)
        {
            try
            {
                foreach (CommandItem ci in CommandItemS2)
                {
                    foreach (P2Pclient Client in ci.Client)
                    {
                        Client.stop();
                    }
                }
                CommandItemS2.Clear();
                foreach (CommandItem ci in CommandItemS)
                {
                    foreach (P2Pclient Client in ci.Client)
                    {
                        Client.stop();
                    }
                }
                CommandItemS.Clear();
                XmlDocument xml = new XmlDocument();
                xml.Load("node.xml");
                foreach (XmlNode xn in xml.FirstChild.ChildNodes)
                {
                    CommandItem ci = new CommandItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.CommName = byte.Parse(xn.Attributes["command"].Value);
                    ci.Commfun = xn.Attributes["Commfun"].Value;
                    P2Pclient p2p = new P2Pclient(false);
                    p2p.receiveServerEvent += P2p_receiveServerEvent;
                    p2p.timeoutevent += P2p_timeoutevent;
                    p2p.ErrorMge += P2p_ErrorMge;
                    if (p2p.start(ci.Ip, ci.Port, false))
                    {
                        ci.Client.Add(p2p);
                        if (xn.Attributes["type"].Value == "receive")
                        {
                            CommandItemS.Add(ci);
                        }
                        else if (xn.Attributes["type"].Value == "push")
                        {
                            CommandItemS2.Add(ci);
                        }
                    }
                    else
                    {
                        if (EventMylog != null)
                            EventMylog("节点连接失败", "命令：" + ci.CommName + ":节点连接失败，抛弃此节点");
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }
        protected void ReloadFlies2(object obj)
        {
            try
            {
                foreach (CommandItem ci in CommandItemS2)
                {
                    foreach (P2Pclient Client in ci.Client)
                    {
                        Client.stop();
                    }
                }
                CommandItemS2.Clear();
                foreach (CommandItem ci in CommandItemS)
                {
                    foreach (P2Pclient Client in ci.Client)
                    {
                        Client.stop();
                    }
                }
                CommandItemS.Clear();
                XmlDocument xml = new XmlDocument();
                xml.Load(System.AppDomain.CurrentDomain.BaseDirectory + "/" + filename);
                foreach (XmlNode xn in xml.FirstChild.ChildNodes)
                {
                    CommandItem ci = new CommandItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.CommName = byte.Parse(xn.Attributes["command"].Value);
                    ci.Commfun = xn.Attributes["Commfun"].Value;
                    P2Pclient p2p = new P2Pclient(false);
                    p2p.receiveServerEvent += P2p_receiveServerEvent;
                    p2p.timeoutevent += P2p_timeoutevent;
                    p2p.ErrorMge += P2p_ErrorMge;
                    if (p2p.start(ci.Ip, ci.Port, false))
                    {
                        ci.Client.Add(p2p);
                        if (xn.Attributes["type"].Value == "receive")
                        {
                            CommandItemS.Add(ci);
                        }
                        else if (xn.Attributes["type"].Value == "push")
                        {
                            CommandItemS2.Add(ci);
                        }
                    }
                    else
                    {
                        if (EventMylog != null)
                            EventMylog("节点连接失败", "命令：" + ci.CommName + ":节点连接失败，抛弃此节点");
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }
        protected void ReloadFliesdtu(object obj)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("dtulist.xml");
                foreach (XmlNode xn in xml.FirstChild.ChildNodes)
                {
                    dtuclient dl = new dtuclient();
                    String ip= xn.Attributes["ip"].Value;
                    int Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    string Commfun= xn.Attributes["Commfun"].Value;
                    DTUclient p2p = new DTUclient();
                    p2p.receiveServerEvent += P2p_receiveServerEvent3; ;
                    p2p.timeoutevent += P2p_timeoutevent1;
                    p2p.ErrorMge += P2p_ErrorMge;
                    if (p2p.start(ip, Port, false))
                    {
                        dl.Tcpdtu = p2p;
                        dl.Token = ip + "|" + Port;
                        p2p.Tokan= ip + "|" + Port;
                        listdtu.Add(dl);
                        foreach (CommandItem ci in CommandItemS2)
                        {
                            if(Commfun== ci.Commfun)
                            foreach (P2Pclient Client in ci.Client)
                            {
                                Client.Tokan = dl.Token;
                                Client.SendRoot<byte[]>(ci.CommName, ci.Commfun,new byte[0], 0);
                            }
                        }
                    }
                    else
                    {
                        if (EventMylog != null)
                            EventMylog("DTU客户端连接失败", "DTU客户端："+ ip + "|" + Port+ "连接失败。");
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }
        private void P2p_receiveServerEvent3(string token, byte[] text)
        {
            foreach (CommandItem ci in CommandItemS2)
            {
                foreach (P2Pclient Client in ci.Client)
                {
                    Client.Tokan = token;
                    Client.SendRoot<byte[]>(ci.CommName, ci.Commfun, text, 0);
                }
            }
        }
        private void P2p_receiveServerEvent1(byte command, string text)
        {
            try
            {
                WeaveSession _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(text);
                try
                {
                    int count = listdtu.Count;
                    dtuclient[] coobjs = new dtuclient[count];
                    listdtu.CopyTo(0, coobjs, 0, count);
                    foreach (dtuclient coob in coobjs)
                    {
                        if (coob != null)
                            if (coob.Token == _0x01.Token)
                            {
                                coob.Tcpdtu.Send(_0x01.GetRoot<byte[]>());
                                return;
                            }
                    }
                }
                catch (Exception ex) { EventMylog("转发", ex.Message); }
            }
            catch { }
        }
        private void P2p_timeoutevent1()
        {
            try
            {
                foreach (dtuclient dl in listdtu)
                {
                    if (!dl.Tcpdtu.Isline)
                    {
                        if (!dl.Tcpdtu.Restart(false))
                        {
                            P2p_timeoutevent1();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("节点重新连接--:", ex.Message);
                P2p_timeoutevent1();
            }
        }
        private void P2p_ErrorMge(int type, string error)
        {
        }
        private void P2p_timeoutevent()
        {
            try
            {
                foreach (CommandItem ci in CommandItemS)
                {
                    foreach (P2Pclient Client in ci.Client)
                    {
                        if (Client != null)
                            if (!Client.Isline)
                            {
                                if (!Client.Restart(false))
                                {
                                    P2p_timeoutevent();
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("节点重新连接--:", ex.Message);
                P2p_timeoutevent();
            }
        }
        private void P2p_receiveServerEvent(byte command, string text)
        {
            try
            {
                WeaveSession _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(text);
                try
                {
                    int count = ConnObjlist.Count;
                    ConnObj[] coobjs = new ConnObj[count];
                    ConnObjlist.CopyTo(0, coobjs, 0, count);
                    foreach (ConnObj coob in coobjs)
                    {
                        if (coob != null)
                            if (coob.Token == _0x01.Token)
                            {
                                coob.Soc.Send(_0x01.GetRoot<byte[]>());
                                return;
                            }
                    }
                }
                catch (Exception ex) { EventMylog("转发", ex.Message); }
            }
            catch { }
        }
    }
    public class dtuclient {
        DTUclient tcpdtu;
        public DTUclient Tcpdtu
        {
            get
            {
                return tcpdtu;
            }
            set
            {
                tcpdtu = value;
            }
        }
        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }
        String token;
    }


   
}
