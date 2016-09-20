using client;
using cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace TCPServer
{
  public  class DTUGateWay
    {
        DTUServer DTUSer;
         
        public int V_ErrorMge { get; private set; }
        public List<CommandItem> CommandItemS { get; private set; }
       
        public event Mylog EventMylog;
        public DTUGateWay()
        {
            DTUSer = new DTUServer();
        }
        public bool Run( int port)
        {
            // Mycommand comm = new Mycommand(, connectionString);
            ReLoad();
            DTUSer.EventDeleteConnSoc += DTUSer_EventDeleteConnSoc;
            DTUSer.EventUpdataConnSoc += DTUSer_EventUpdataConnSoc;
            DTUSer.receiveeventDtu += DTUSer_receiveeventDtu;
            
            DTUSer.start(port);
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

        private void ReLoad()
        {
            ReloadFlies(null);
        }
        protected void ReloadFlies(object obj)
        {
            try
            {
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
                        CommandItemS.Add(ci);
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
        String commfun;
         
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

        public List<P2Pclient> Client
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

        public string Commfun
        {
            get
            {
                return commfun;
            }

            set
            {
                commfun = value;
            }
        }

        String ip = "";
        int port;
        List<P2Pclient> client = new List<P2Pclient>();
    }
}
