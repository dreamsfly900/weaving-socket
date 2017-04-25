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
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace cloud
{
    public interface IDataparsing
    {


        _baseModel Get_baseModel(byte[] data);
        //_0x01.Token 给_baseModel的Token对象赋值
        //_0x01.Request 给Request对象赋值，这个从你的data里面解析出来，Request代表后端逻辑的方法名，
        //你可以设定一个文件做对应，比如1==getnum方法，2==setnum方法
        // _0x01.SetRoot<byte[]> 给ROOT 对象赋值，这个是你传输的数据的内容。内容直接赋值这里就行了。
        byte[] Get_Byte(_baseModel bm);
        byte[] Get_ByteBystring(String  str);
        bool socketvalidation(_baseModel bm);
    }
    public class GateWaybyte
    {

        protected ITcpBasehelper p2psev;
        IDataparsing idp;
        List<CommandItem> listcomm = new List<CommandItem>();
        QueueTable qt = new QueueTable();
        private int proportion = 10;
        public event Mylog EventMylog;
        // public List<ConnObj> ConnObjlist = new List<ConnObj>();
        public List<CommandItem> CommandItemS = new List<CommandItem>();
        public List<WayItem> WayItemS = new List<WayItem>();

        protected p2psever p2psev2;
        int max = 5000;
        int counttemp = 0;
        List<int> tempnum = new List<int>();
        #region 初始化
        public GateWaybyte()
        {
            ConnObjlist = new ConnObj[max];
          
                p2psev = new p2psever();
        }
        public GateWaybyte( int _max, IDataparsing _idp)
        {
            idp = _idp;
            newwayEvent += GateWay_newwayEvent;
            max = _max;
            ConnObjlist = new ConnObj[max];
         
                p2psev = new p2psever(P2P.DataType.bytes);
        }

        #endregion


        public bool Run(string loaclIP, int port, int port2)
        {
            // Mycommand comm = new Mycommand(, connectionString);
            ReLoad();

         //   p2psev.receiveevent += p2psev_receiveevent;
            p2psev.receiveeventbit += P2psev_receiveeventbit;
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




        protected void V_ErrorMge(int type, string error)
        {
            if (EventMylog != null)
                EventMylog("V_ErrorMge", type + ":" + error);
        }

        #region 加载配置文件
        public void ReLoad()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFlies));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFliesway));
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

                    p2p.receiveServerEvent += (V_receiveServerEvent);
                    p2p.timeoutevent += (V_timeoutevent);
                    p2p.ErrorMge += (V_ErrorMge);
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
                foreach (XmlNode xn in xml.FirstChild.ChildNodes)
                {
                    WayItem ci = new WayItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.Token = (xn.Attributes["token"].Value);
                    ci.Client = new P2Pclient(false);
                    ci.Client.receiveServerEvent += Client_receiveServerEvent;
                    ci.Client.timeoutevent += Client_timeoutevent;
                    ci.Client.ErrorMge += Client_ErrorMge;
                    if (ci.Client.start(ci.Ip, ci.Port, false))
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
        #endregion

        #region 相互间路由的通信
        public void getwaynum(object obj)
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
                System.Threading.Thread.Sleep(2000);
            }
        }
        public int getnum()
        {
            return counttemp - tempnum.Count;
        }
        protected void Client_ErrorMge(int type, string error)
        {

        }
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
                        _0x01.Root = getnum().ToString();
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
        protected void Client_timeoutevent()
        {
            try
            {
                foreach (WayItem ci in WayItemS)
                {
                    if (!ci.Client.Isline)
                    {
                        if (ci.Client.Restart(false))
                        {
                            Client_timeoutevent();
                            System.Threading.Thread.Sleep(5000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("way节点重新连接", ex.Message);
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
        #endregion

        #region 客户端连接与消息转发
        private void GateWay_newwayEvent(int temp)
        {
            try
            {



                int len = (temp + (Proportion - 1)) % Proportion;
                if (len == 0)
                {

                    foreach (CommandItem ci in CommandItemS)
                    {
                        if (ci.Client.Count > ((temp + Proportion) / Proportion))
                            return;
                        P2Pclient p2p = new P2Pclient(false);
                        p2p.receiveServerEvent += (V_receiveServerEvent);
                        p2p.timeoutevent += (V_timeoutevent);
                        p2p.ErrorMge += (V_ErrorMge);
                        if (p2p.start(ci.Ip, ci.Port, false))
                        {
                            ci.Client.Add(p2p);
                        }

                    }
                }
            }
            catch (Exception e)
            {

                EventMylog("EventUpdataConnSoc---新建连接", "");
            }
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
                    int i = 0;
                    foreach (P2Pclient Client in ci.Client)
                    {
                        // if (Client != null)
                        if (!Client.Isline)
                        {
                            string port = Client.localprot;
                            if (EventMylog != null)
                                EventMylog("节点重新连接--:", Client.IP + ":" + Client.PORT);
                            if (!Client.Restart(false))
                            {
                                V_timeoutevent();
                                Client.localprot = port;
                            }
                            else
                            {
                                try
                                {
                                    EventMylog("节点重新连接-通知下线-:", Client.IP + ":" + Client.PORT);
                                    for (int j = (i * Proportion); j < (i * Proportion) + Proportion; j++)
                                    {
                                        if (ConnObjlist[j] != null)
                                        {
                                            try
                                            {
                                                ConnObjlist[j].Soc.Close();
                                            }
                                            catch { }
                                            // p2psev_EventDeleteConnSoc(ConnObjlist[j].Soc);
                                        }
                                    }
                                }
                                catch (Exception ee) { EventMylog("节点重新连接-Restart-:", ee.Message); }
                                //Client.send(0xff, "Restart|"+ port);
                                EventMylog("节点重新连接-Restart-:", Client.IP + ":" + Client.PORT);
                            }
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("节点重新连接-233-:", ex.Message);
                V_timeoutevent();
            }
        }

        /// <summary>
        /// 这里写接收到内容后，转发
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        protected void V_receiveServerEvent(byte command, string text)
        {

            try
            {
                _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(text);
                int temp = 0;
                try
                {
                    temp = Convert.ToInt32(_0x01.Token.Substring(17));
                    _0x01.Querycount = temp;
                }
                catch
                {
                    EventMylog("转发", temp + "获取编号失败。" + _0x01.Token);
                    return;
                }

                if (ConnObjlist[temp] != null)
                {
                    int error = 0;
                    lb1122:
                   
                    if (!p2psev.send(ConnObjlist[temp].Soc, command,  idp.Get_Byte(_0x01)))
                    {
                        error += 1;
                        EventMylog("转发" + error, "ConnObjlist:" + temp + "发送失败：" + text);
                        if (error < 3) goto lb1122;
                    }
                }
                else
                {
                    EventMylog("转发", "ConnObjlist:" + temp + "是空的");
                }

                return;

            }
            catch (Exception ex) { EventMylog("转发", ex.Message + "112223333333333356464122313" + text + "000000"); }
        }

        protected void p2psev_EventDeleteConnSoc(System.Net.Sockets.Socket soc)
        {
            try
            {


                //ThreadList<ConnObj> coobjs = ConnObjlist.Clone();
                int i = 0;
                for (i = 0; i < ConnObjlist.Length; i++)
                {

                    ConnObj coob = ConnObjlist[i];
                    if (coob != null)
                        if (coob.Soc.Equals(soc))
                        {
                            // ConnObjlist[i] =null;
                            int tempi = Convert.ToInt32(coob.Token.Substring(17));
                            tempnum.Add(tempi);

                            try
                            {
                                List<String> listsercer = new List<string>();
                                bool temp = true;
                                int len = i / Proportion;
                                foreach (CommandItem ci in CommandItemS)
                                {
                                    temp = true;
                                    foreach (string ser in listsercer)
                                    {
                                        if (ser == (ci.Client[len].IP + ci.Client[len].PORT))
                                        {
                                            temp = false;
                                            goto lab881;
                                        }
                                    }
                                    lab881:
                                    if (temp)
                                    {
                                        listsercer.Add(ci.Client[len].IP + ci.Client[len].PORT);
                                        ci.Client[len].send(0xff, "out|" + coob.Token);
                                    }
                                }
                                ConnObjlist[i] = null;
                                return;
                            }
                            catch (Exception e) { EventMylog("移除用户", e.Message); }
                        }

                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("移除用户", ex.Message);
            }
        }
        public ConnObj[] ConnObjlist = new ConnObj[0];
        //ThreadSafeDictionary<string, ConnObj> ConnObjlist = new ThreadSafeDictionary<string, ConnObj>();
        protected void p2psev_EventUpdataConnSoc(System.Net.Sockets.Socket soc)
        {

            ConnObj cobj = new ConnObj();
            cobj.Soc = soc;
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
            //cobj.Token = EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
            int temp = 0;
            lock (tempnum)
            {
                if ((counttemp < max && tempnum.Count > 0) || (counttemp >= max && tempnum.Count > 0))
                {

                    temp = tempnum[0];
                    tempnum.RemoveAt(0);
                }
                else if (counttemp < max && tempnum.Count <= 0)
                {
                    temp = counttemp;
                    counttemp++;
                }
            }
            if (counttemp >= max && tempnum.Count == 0)
            {
                int mincount = int.MaxValue;
                string tempip = "";
                foreach (WayItem ci in WayItemS)
                {
                    if (ci.Num < mincount)
                    {
                        mincount = ci.Num;
                        tempip = ci.Ip + ":" + ci.Port;
                    }
                }
                p2psev.send(soc, 0xff, idp.Get_ByteBystring("jump|" + tempip + ""));
                
                //p2psev.send(soc, 0xff, "jump|" + tempip + "");
               
                soc.Close();
                return;
            }

            try
            {
                //IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
                cobj.Token = DateTime.Now.ToString("yyyyMMddHHmmssfff") + temp;// EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
                cobj.Id = temp;
                cobj.Soc = soc;
                ConnObjlist[temp] = cobj;
                p2psev.send(soc, 0xff, idp.Get_ByteBystring("token|" + cobj.Token + ""));

                //if (p2psev.send(ConnObjlist[temp].Soc, 0xff, "token|" + cobj.Token + ""))
                //{


                //}

                List<String> listsercer = new List<string>();
                bool tempb = true;
                foreach (CommandItem ci in CommandItemS)
                {
                    tempb = true;
                    foreach (string ser in listsercer)
                    {
                        if (ser == (ci.Client[ci.Client.Count - 1].IP + ci.Client[ci.Client.Count - 1].PORT))
                        {
                            tempb = false;
                            goto lab882;
                        }
                    }
                    lab882:
                    if (tempb)
                    {

                        if (ci.Client[ci.Client.Count - 1] != null)
                        {
                            listsercer.Add(ci.Client[ci.Client.Count - 1].IP + ci.Client[ci.Client.Count - 1].PORT);
                            ci.Client[ci.Client.Count - 1].send(0xff, "in|" + cobj.Token);
                        }
                    }

                }


                newwayEvent(temp);



            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("EventUpdataConnSoc", ex.Message);
            }
        }
        event newway newwayEvent;
        delegate void newway(int temp);
        private void P2psev_receiveeventbit(byte command, byte[] data, Socket soc)
        {
           
            try
            {
                // JSON.parse<_baseModel>(data);// 
                _baseModel _0x01;
                try
                {
                    //data 你自己的协议的数据
                    _0x01 = idp.Get_baseModel(data);
                    //_0x01.Token 给_baseModel的Token对象赋值
                    //_0x01.Request 给Request对象赋值，这个从你的data里面解析出来，Request代表后端逻辑的方法名，
                    //你可以设定一个文件做对应，比如1==getnum方法，2==setnum方法
                   // _0x01.SetRoot<byte[]> 给ROOT 对象赋值，这个是你传输的数据的内容。内容直接赋值这里就行了。

                }
                catch
                {
                    EventMylog("JSON解析错误：", "" + data);

                    return;
                }
                if (_0x01.Token == null)
                {
                    EventMylog("Token是NULL：", "" + data);
                    return;
                }
                string key = "";
                string ip = "";
                //try
                //{
                //    key = DecryptDES(_0x01.Token, "lllssscc");
                //     ip = key.Split('|')[0];
                //}
                //catch { return; }
                IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;

                //if (clientipe.Address.ToString() == ip)
                //{

                int count = CommandItemS.Count;

                int temp = 0;
                try
                {
                    temp = Convert.ToInt32(_0x01.Token.Substring(17));
                    _0x01.Querycount = temp;
                }
                catch (Exception e)
                {
                    if (EventMylog != null)
                        EventMylog("p2psev_receiveevent", e.Message);
                    return;
                }
                //CommandItem[] comItems = new CommandItem[count];
                //CommandItemS.CopyTo(0, comItems, 0, count);
              ConnObj  cobj= ConnObjlist[temp];
                if (cobj.Validation == false && command == 0xff)
                {
                    //从这里写验证
                    cobj.Validation = idp.socketvalidation(_0x01);
                    return;
                }
                else if (cobj.Validation == false && command != 0xff)
                {
                    //没有验证过，也不去验证，就直接断开
                    cobj.Soc.Close();
                    return;
                }
                foreach (CommandItem ci in CommandItemS)
                {
                    if (ci != null)
                    {
                        if (ci.CommName == command)
                        {

                            int i = temp;
                            int len = i / Proportion;
                            int index = len >= ci.Client.Count ? ci.Client.Count - 1 : len;
                            if (!ci.Client[index].Isline)
                            { p2psev.send(soc, 0xff, "你所请求的服务暂不能使用，已断开连接！"); }
                            if (!ci.Client[index].send(command, data))
                            {
                                p2psev.send(soc, 0xff, index + "你所请求的服务暂不能使用，发送错误。" + ci.Client[index].Isline);

                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("p2psev_receiveevent----", ex.Message);
            }
        }

         
        #endregion
        private byte[] Keys = { 0xEF, 0xAB, 0x56, 0x78, 0x90, 0x34, 0xCD, 0x12 };

        public int Proportion
        {
            get
            {
                return proportion;
            }

            set
            {
                proportion = value;
            }
        }

      
    }
}
