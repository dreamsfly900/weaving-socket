
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Weave.Base.Interface;
using Weave.Base;
using Weave.Server;

namespace Weave.Cloud
{
    /// <summary>
    /// 继承自 IWeaveUniversal的类，拥有weaveOnline（在线客户端集合）,P2ServerList(IWeaveTcpBase集合)，TcpTokenList（WeaveTcpToken集合），这个类可以同时启动多个IWeaveTcpBase接口实现类，实现多个监听端口，不同的服务器端（只要是继承自IWeaveTcpBase接口的）
    /// </summary>
    public class WeaveTCPcloud : IWeaveUniversal
    {
        public event WeaveLogDelegate WeaveLogEvent;
        public XmlDocument xml
        {
            get;set;
        }

        public List<CmdWorkItem> CmdWorkItems
        {
            get
            {
                return _CmdWorkItems;
            }

            set
            {
                _CmdWorkItems = value;
            }
        }

        public WeaveTable weaveTable
        {
            get
            {
                return _weaveTable;
            }

            set
            {
                _weaveTable = value;
            }
        }

        public List<WeaveOnLine> weaveOnline
        {
            get
            {
                return _weaveOnline;
            }

            set
            {
                _weaveOnline = value;
            }
        }

        public List<IWeaveTcpBase> P2ServerList
        {
            get
            {
                return _P2ServerList;
            }

            set
            {
                _P2ServerList = value;
            }
        }

        public List<WeaveTcpToken> TcpTokenList
        {
            get
            {
                return _TcpTokenList;
            }

            set
            {
                _TcpTokenList = value;
            }
        }

        List<CmdWorkItem> _CmdWorkItems = new List<CmdWorkItem>();
        
        WeaveTable _weaveTable = new WeaveTable();
     
        List<WeaveOnLine> _weaveOnline = new List<WeaveOnLine>();
        
        List<IWeaveTcpBase> _P2ServerList = new List<IWeaveTcpBase>();
       
        List<WeaveTcpToken> _TcpTokenList = new List<WeaveTcpToken>();
       
        public bool Run(WevaeSocketSession myI)
        {
            ReloadFlies();
            weaveTable.Add("onlinetoken", weaveOnline);//初始化一个队列，记录在线人员的token
            if (WeaveLogEvent != null)
                WeaveLogEvent("连接", "连接启动成功");
            return true;
        }
        public void AddProt(List<WeaveServerPort> listServerPort)
        {
            foreach (WeaveServerPort sp in listServerPort)
            {
                IWeaveTcpBase p2psev = null;
                WeaveTcpToken tt = new WeaveTcpToken();
                if (sp.PortType == WeavePortTypeEnum.Web)
                {
                    p2psev = new WeaveWebServer();
                    if(sp.Certificate!=null)
                    ((WeaveWebServer)p2psev).Certificate = sp.Certificate;
                }
                else if (sp.PortType == WeavePortTypeEnum.Json)
                {
                    p2psev = new WeaveP2Server("127.0.0.1");
                }
                else if (sp.PortType == WeavePortTypeEnum.Bytes)
                {
                    p2psev = new WeaveP2Server(WeaveDataTypeEnum.Bytes);
                    if (sp.BytesDataparsing == null) { throw new Exception("BytesDataparsing对象不能是空，请完成对应接口的实现。"); }
                    tt.BytesDataparsing = sp.BytesDataparsing;
                    p2psev.weaveReceiveBitEvent += P2psev_receiveeventbit;
                }
                //else if (sp.PortType == WeavePortTypeEnum.jsonudp)
                //{
                //    p2psev = new WeaveUDPServer(WeaveDataTypeEnum.Json);
                     
                //}
                //else if (sp.PortType == WeavePortTypeEnum.Json)
                //{
                //    p2psev = new WeaveUDPServer("127.0.0.1");
                //}
                else if (sp.PortType == WeavePortTypeEnum.Http)
                {
                    p2psev = new HttpServer(sp.Port);
                }
                p2psev.waveReceiveEvent += P2ServerReceiveHander;
                p2psev.weaveUpdateSocketListEvent += P2ServerUpdateSocketHander;
                p2psev.weaveDeleteSocketListEvent += P2ServerDeleteSocketHander;
                //   p2psev.NATthroughevent += tcp_NATthroughevent;//p2p事件，不需要使用
                p2psev.Start(Convert.ToInt32(sp.Port));//myI.Parameter[4]是端口号
                tt.PortType = sp.PortType;
                tt.P2Server = p2psev;
                tt.IsToken = sp.IsToken;
                tt.WPTE = sp.PortType;
                TcpTokenList.Add(tt);
                P2ServerList.Add(p2psev);
            }
        }
        private void P2psev_receiveeventbit(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
                // JSON.parse<_baseModel>(data);// 
                WeaveSession _0x01 = null;
                try
                {
                    foreach (WeaveTcpToken itp in TcpTokenList)
                    {
                        if (itp.PortType == WeavePortTypeEnum.Bytes)
                        {
                            _0x01 = itp.BytesDataparsing.GetBaseModel(data);//二进制转_baseModel
                            if (command == 0xff)
                            {
                                WeaveExcCmdNoCheckCmdName(command, _0x01.Getjson(), soc);
                            }
                            else
                            {
                                if (itp.BytesDataparsing.socketvalidation(_0x01)) //验证_baseModel
                                    WeaveExcCmd(command, _0x01.Getjson(), soc);
                            }
                        }
                    }
                }
                catch
                {
                    WeaveLogEvent("JSON解析错误：", "" + data);
                    return;
                }
            }
            catch (Exception ex)
            {
                if (WeaveLogEvent != null)
                    WeaveLogEvent("p2psev_receiveevent----", ex.Message);
            }
        }
        public void ReloadFlies()
        {
            try
            {
               
                String[] strfilelist = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "command");
                foreach (string file in strfilelist)
                {
                    Assembly ab = Assembly.LoadFile(file);
                    Type[] types = ab.GetExportedTypes();
                    foreach (Type t in types)
                    {
                        try
                        {
                            if (t.IsSubclassOf(typeof(WeaveTCPCommand)))
                            {
                                CmdWorkItem ci = new CmdWorkItem();
                                object obj = ab.CreateInstance(t.FullName);
                                WeaveTCPCommand Ic = obj as WeaveTCPCommand;
                                Ic.SetGlobalQueueTable(weaveTable, TcpTokenList);
                                ci.WeaveTcpCmd = Ic;
                                ci.CmdName = Ic.Getcommand();
                                GetAttributeInfo(Ic, obj.GetType(), obj);
                                CmdWorkItems.Add(ci);
                            }
                        }
                        catch //(Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                if (WeaveLogEvent != null)
                    WeaveLogEvent("加载异常", ex.Message);
            }
        }
        public void GetAttributeInfo(WeaveTCPCommand Ic, Type t, object obj)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute == null)
                {
                }
                else
                {
                    if (myattribute.Dtu)
                    {
                        Delegate del = Delegate.CreateDelegate(typeof(WeaveRequestDataDtuDelegate), obj, mi, true);
                        Ic.Bm.AddListen(mi.Name, del as WeaveRequestDataDtuDelegate, myattribute.Type, true);
                    }
                    else
                    {
                        Delegate del = Delegate.CreateDelegate(typeof(WeaveRequestDataDelegate), obj, mi, true);
                        Ic.Bm.AddListen(mi.Name, del as WeaveRequestDataDelegate, myattribute.Type);
                    }
                }
            }
        }
        void P2ServerDeleteSocketHander(System.Net.Sockets.Socket soc)
        {
            try
            {
                int count = CmdWorkItems.Count;
                CmdWorkItem[] cilist = new CmdWorkItem[count];
                CmdWorkItems.CopyTo(0, cilist, 0, count);
                foreach (CmdWorkItem CI in cilist)
                {
                    try
                    {
                        CI.WeaveTcpCmd.WeaveDeleteSocketEvent(soc);
                    }
                    catch (Exception ex)
                    {
                        if (WeaveLogEvent != null)
                            WeaveLogEvent("EventDeleteConnSoc", ex.Message);
                    }
                }
            }
            catch { }
            try
            {
                int count = weaveOnline.Count;
                WeaveOnLine[] ols = new WeaveOnLine[count];
                weaveOnline.CopyTo(0, ols, 0, count);
                foreach (WeaveOnLine ol in ols)
                {
                    if (ol.Socket.Equals(soc))
                    {
                        foreach (CmdWorkItem CI in CmdWorkItems)
                        {
                            try
                            {
                                WeaveExcCmdNoCheckCmdName(0xff, "out|" + ol.Token, ol.Socket);
                                CI.WeaveTcpCmd.Tokenout(ol);
                            }
                            catch (Exception ex)
                            {
                                if (WeaveLogEvent != null)
                                    WeaveLogEvent("Tokenout", ex.Message);
                            }
                        }
                        weaveOnline.Remove(ol);
                        return;
                    }
                }
            }
            catch { }
        }
        void P2ServerUpdateSocketHander(System.Net.Sockets.Socket soc)
        {
            try
            {
                int count = CmdWorkItems.Count;
                CmdWorkItem[] cilist = new CmdWorkItem[count];
                CmdWorkItems.CopyTo(0, cilist, 0, count);
                foreach (CmdWorkItem CI in cilist)
                {
                    try
                    {
                        CI.WeaveTcpCmd.WeaveUpdateSocketEvent(soc);
                    }
                    catch (Exception ex)
                    {
                        if (WeaveLogEvent != null)
                            WeaveLogEvent("EventUpdataConnSoc", ex.Message);
                    }
                }
            }
            catch { }
            foreach (WeaveTcpToken token in TcpTokenList)
            {
                
                    string Token = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(1000, 9999);// EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
                if (token.P2Server.Port == ((System.Net.IPEndPoint)soc.LocalEndPoint).Port)
                {
                    bool sendok = false;
                    if (token.IsToken)
                    {

                        if (token.PortType == WeavePortTypeEnum.Bytes)
                            sendok = token.P2Server.Send(soc, 0xff, token.BytesDataparsing.Get_ByteBystring("token|" + Token + ""));
                        else
                            sendok = token.P2Server.Send(soc, 0xff, "token|" + Token + "");
                    }
                    else
                        sendok = true;
                    if (sendok)
                    {
                        WeaveOnLine ol = new WeaveOnLine();
                        ol.Token = Token;
                        ol.Socket = soc;
                        weaveOnline.Add(ol);
                        foreach (CmdWorkItem cmdItem in CmdWorkItems)
                        {
                            try
                            {
                                WeaveExcCmdNoCheckCmdName(0xff, "in|" + ol.Token, ol.Socket);
                                cmdItem.WeaveTcpCmd.TokenIn(ol);
                            }
                            catch (Exception ex)
                            {
                                if (WeaveLogEvent != null)
                                    WeaveLogEvent("Tokenin", ex.Message);
                            }
                        }
                        return;
                    }
                }
                
            }
        }
        void P2ServerReceiveHander(byte command, string data, System.Net.Sockets.Socket soc)
        {
            try
            {
                if (command == 0xff)
                {
                    WeaveExcCmdNoCheckCmdName(command, data, soc);
                    try
                    {
                        string[] temp = data.Split('|');
                        if (temp[0] == "in")
                        {
                            //加入onlinetoken
                            WeaveOnLine ol = new WeaveOnLine();
                            ol.Token = temp[1];
                            ol.Socket = soc;
                            weaveOnline.Add(ol);
                            foreach (CmdWorkItem CI in CmdWorkItems)
                            {
                                try
                                {
                                    CI.WeaveTcpCmd.TokenIn(ol);
                                }
                                catch (Exception ex)
                                {
                                    WeaveLogEvent?.Invoke("Tokenin", ex.Message);
                                }
                            }
                            return;
                        }
                        else if (temp[0] == "Restart")
                        {
                            int count = weaveOnline.Count;
                            WeaveOnLine[] ols = new WeaveOnLine[count];
                            weaveOnline.CopyTo(0, ols, 0, count);
                            string IPport = ((System.Net.IPEndPoint)soc.RemoteEndPoint).Address.ToString() + ":" + temp[1];
                            foreach (WeaveOnLine ol in ols)
                            {
                                try
                                {
                                    if (ol.Socket != null)
                                    {
                                        String IP = ((System.Net.IPEndPoint)ol.Socket.RemoteEndPoint).Address.ToString() + ":" + ((System.Net.IPEndPoint)ol.Socket.RemoteEndPoint).Port;
                                        if (IP == IPport)
                                        {
                                            ol.Socket = soc;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        else if (temp[0] == "out")
                        {
                            ////移出onlinetoken
                            int count = weaveOnline.Count;
                            WeaveOnLine[] ols = new WeaveOnLine[count];
                            weaveOnline.CopyTo(0, ols, 0, count);
                            foreach (WeaveOnLine onlinesession in ols)
                            {
                                if (onlinesession.Token == temp[1])
                                {
                                    foreach (CmdWorkItem cmdItem in CmdWorkItems)
                                    {
                                        try
                                        {
                                            cmdItem.WeaveTcpCmd.Tokenout(onlinesession);
                                        }
                                        catch (Exception ex)
                                        {
                                            WeaveLogEvent?.Invoke("Tokenout", ex.Message);
                                        }
                                    }
                                    weaveOnline.Remove(onlinesession);
                                    return;
                                }
                            }
                        }
                    }
                    catch { }
                    return;
                }
                else
                    WeaveExcCmd(command, data, soc);
            }
            catch { return; }
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(exec));
        }
        public void WeaveExcCmdNoCheckCmdName(byte command, string data, System.Net.Sockets.Socket soc)
        {
            foreach (CmdWorkItem cmd in CmdWorkItems)
            {
                try
                {
                    cmd.WeaveTcpCmd.Runcommand(command, data, soc);
                }
                catch (Exception ex)
                {
                    WeaveLogEvent?.Invoke("receiveevent", ex.Message);
                }
            }
        }
        public void WeaveExcCmd(byte command, string data, System.Net.Sockets.Socket soc)
        {
            foreach (CmdWorkItem cmd in CmdWorkItems)
            {
                if (cmd.CmdName == command)
                {
                    try
                    {
                        cmd.WeaveTcpCmd.Run(data, soc);
                        cmd.WeaveTcpCmd.RunBase(data, soc);
                    }
                    catch (Exception ex)
                    {
                        WeaveLogEvent?.Invoke("receiveevent", ex.Message);
                    }
                }
            }
        }
        public class CmdWorkItem
        {
            public byte CmdName
            {
                get;set;
            }
            public WeaveTCPCommand WeaveTcpCmd
            {
                get;set;
            }
        }
    }
}
