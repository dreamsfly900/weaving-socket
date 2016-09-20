using client;
using cloud;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return true;
        }

        private void DTUSer_receiveeventDtu(byte[] data, System.Net.Sockets.Socket soc)
        {
            
        }

        private void DTUSer_EventUpdataConnSoc(System.Net.Sockets.Socket soc)
        {
            
        }

        private void DTUSer_EventDeleteConnSoc(System.Net.Sockets.Socket soc)
        {
            
        }

        private void ReLoad()
        {
              
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
            
        }

        private void P2p_receiveServerEvent(byte command, string text)
        {
            
        }
    }
}
