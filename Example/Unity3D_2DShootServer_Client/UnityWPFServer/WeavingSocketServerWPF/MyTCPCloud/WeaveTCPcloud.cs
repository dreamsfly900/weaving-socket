
using MyTcpCommandLibrary;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Xml;
using WeaveBase;
using WeaveSocketServer;
namespace MyTCPCloud
{
    public class WeaveTCPcloud : IWeaveUniversal
    {
        public event WeaveLogDelegate WeaveLogEvent;

        public event WeaveServerReceiveDelegate WeaveReceiveEvent;

        public event WeaveServerUpdateSocketHander WeaveUpdateEvent;

        public event WeaveServerDeleteSocketHander WeaveDeleteEvent;

        public event WeaveServerUpdateUnityPlayerSetOnLineHander WeaveServerUpdateUnityPlayerSetOnLineEvent;


        //public XmlDocument xml
        //{
        //    get;set;
        //}

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

        public List<UnityPlayerOnClient> unityPlayerOnClientList
        {
            get
            {
                return _unityPlayerOnClientList;
            }

            set
            {
                _unityPlayerOnClientList = value;
            }
        }

       // public IWeaveTcpBase P2Server
         public WeaveP2Server P2Server
        {
            get;set;
        }

        public WeaveTcpToken TcpToken
        {
            get
            {
                return _TcpToken;
            }

            set
            {
                _TcpToken = value;
            }
        }

        List<CmdWorkItem> _CmdWorkItems = new List<CmdWorkItem>();
        
        WeaveTable _weaveTable = new WeaveTable();
     
        List<WeaveOnLine> _weaveOnline = new List<WeaveOnLine>();
        
       
       WeaveTcpToken _TcpToken = new WeaveTcpToken();

        //我写的方法
        List<UnityPlayerOnClient> _unityPlayerOnClientList = new List<UnityPlayerOnClient>();
        
       
        

        public bool Run(WevaeSocketSession myI)
        {
            //ReloadFlies();
            AddMyTcpCommandLibrary();


            weaveTable.Add("onlinetoken", weaveOnline);//初始化一个队列，记录在线人员的token
            if (WeaveLogEvent != null)
                WeaveLogEvent("连接", "连接启动成功");
            return true;
        }
        /// <summary>
        /// 读取WeavePortTypeEnum类型后，初始化 new WeaveP2Server("127.0.0.1"),并添加端口;
        /// </summary>
        /// <param name="WeaveServerPort"></param>
        public void StartServer(WeaveServerPort _ServerPort)
        {
           
             // WeaveTcpToken weaveTcpToken = new WeaveTcpToken();
         
               P2Server = new WeaveP2Server("127.0.0.1");

              P2Server.waveReceiveEvent += P2ServerReceiveHander;
               P2Server.weaveUpdateSocketListEvent += P2ServerUpdateSocketHander;
               P2Server.weaveDeleteSocketListEvent += P2ServerDeleteSocketHander;
               //   p2psev.NATthroughevent += tcp_NATthroughevent;//p2p事件，不需要使用
               P2Server.Start( _ServerPort.Port  );//myI.Parameter[4]是端口号

               TcpToken.PortType = _ServerPort.PortType;
               TcpToken.P2Server = P2Server;
              TcpToken.IsToken = _ServerPort.IsToken;
               TcpToken.WPTE = _ServerPort.PortType;
                
               // TcpToken = weaveTcpToken;
            
               // P2Server = p2psev;
            
        }


        public void AddMyTcpCommandLibrary()
        {
            try
            {
                LoginManageCommand loginCmd = new LoginManageCommand();
                loginCmd.ServerLoginOKEvent += UpdatePlayerListSetOnLine;
                AddCmdWorkItems(loginCmd);

                AddCmdWorkItems(new GameScoreCommand());

                AddCmdWorkItems(new ClientDisConnectedCommand());



            }
            catch
            {

            }
        }

        public void AddCmdWorkItems(WeaveTCPCommand cmd)
        {
            cmd.SetGlobalQueueTable(weaveTable, TcpToken);
            CmdWorkItem cmdItem = new CmdWorkItem();
            // Ic.SetGlobalQueueTable(weaveTable, TcpTokenList);
            cmdItem.WeaveTcpCmd = cmd;
            cmdItem.CmdName = cmd.Getcommand();
            GetAttributeInfo(cmd, cmd.GetType(), cmd);
            CmdWorkItems.Add(cmdItem);
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

            /*我写的方法*/
            WeaveOnLine hasOnline = weaveOnline.Find(item => item.Socket == soc);

            if (hasOnline != null)
            {
                
                UnityPlayerOnClient uplayer = ConvertWeaveOnlineToUnityPlayerOnClient(hasOnline);

                WeaveDeleteEvent(uplayer);

                weaveOnline.Remove(hasOnline);

                // unityPlayerOnClientList.Remove(uplayer);
                DeleteUnityPlayerOnClient(hasOnline.Socket);
            }
            /**/


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


        public void UpdatePlayerListSetOnLine(string _userName , System.Net.Sockets.Socket soc)
        {
            foreach(UnityPlayerOnClient oneclient in unityPlayerOnClientList)
            {
                if(oneclient.Socket == soc)
                {
                    oneclient.UserName = _userName;
                    oneclient.isLogin = true;
                    WeaveServerUpdateUnityPlayerSetOnLineEvent(oneclient);
                    break;
                }
            }

           
        }


        void P2ServerUpdateSocketHander(System.Net.Sockets.Socket soc)
        {
           
            #region  读取 Command接口类，每次有新的Socket加入 重新读取并设置
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
            catch
            {

            }

            #endregion  发送Token的代码

            WeaveTcpToken token = TcpToken;
            {
                if (token.IsToken)
                {
                    //生成一个token,后缀带随机数
                    string Token = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(1000, 9999);// EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");
                    if (token.P2Server.Port == ((System.Net.IPEndPoint)soc.LocalEndPoint).Port)
                    {
                        //向客户端发送生成的token
                        bool sendok = false;
                        if (token.PortType == WeavePortTypeEnum.Bytes)
                            sendok = token.P2Server.Send(soc, 0xff, token.BytesDataparsing.Get_ByteBystring("token|" + Token + ""));
                        else
                            sendok = token.P2Server.Send(soc, 0xff, "token|" + Token + "");


                        #region  if(sendok)
                        if (sendok)
                        {
                            WeaveOnLine ol = new WeaveOnLine()
                            {
                                Name = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                Obj = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                            };
                            ol.Token = Token;
                            ol.Socket = soc;
                           
                                WeaveOnLine hasOnline = weaveOnline.Find(item => item.Name == ol.Name);
                                {
                                   if (hasOnline != null)
                                  {
                                    weaveOnline.Remove(hasOnline);

                                    weaveOnline.Add(ol);

                                  }
                                  else
                                  {
                                    weaveOnline.Add(ol);
                                   }
                                }
                               
                           
                            /*我单独写的UnityClient*/
                            /*我写的新方法*/
                            UnityPlayerOnClient hasPlayerIn = unityPlayerOnClientList.Find(item => item.Name == ol.Name);
                                if (hasPlayerIn != null)
                                {
                                    WeaveDeleteEvent(hasPlayerIn);
                                    unityPlayerOnClientList.Remove(hasPlayerIn);

                                }
                                /*我写的方法结束*/

                                UnityPlayerOnClient uplayer = ConvertWeaveOnlineToUnityPlayerOnClient(ol);
                                // unityPlayerOnClientList.Add(uplayer);
                                AddUnityPlayerClient_CheckSameItem(uplayer , ol.Name);
                                WeaveUpdateEvent(uplayer);
                            
                            

                            /**/


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
                        #endregion
                    }
                }
                else
                {
                    WeaveOnLine ol = new WeaveOnLine()
                    {
                        Name = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                        Obj = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                        Socket = soc,
                        Token = DateTime.Now.ToString("yyyyMMddHHmmssfff")

                    };
                    weaveOnline.Add(ol);


                    /*我单独写的UnityClient*/
                    UnityPlayerOnClient hasPlayerIn = unityPlayerOnClientList.Find(item => item.Socket == soc);
                    if (hasPlayerIn != null)
                    {
                        WeaveDeleteEvent(hasPlayerIn);
                        unityPlayerOnClientList.Remove(hasPlayerIn);

                    }

                    UnityPlayerOnClient uplayer = ConvertWeaveOnlineToUnityPlayerOnClient(ol);
                    AddUnityPlayerClient_CheckSameItem(uplayer, ol.Name);
                    WeaveUpdateEvent(uplayer);
                    /**/
                    //  ol.Token = DateTime.Now.ToString();
                    //  ol.Socket = soc;


                }

            }
        }
        void P2ServerReceiveHander(byte command, string data, System.Net.Sockets.Socket soc)
        {
            if(command == (byte)CommandEnum.ClientSendDisConnected)
            {
                P2Server.CliendSendDisConnectedEvent(soc);

                /*我写的方法*/
                WeaveOnLine hasOnline = weaveOnline.Find(item => item.Socket == soc);

                if (hasOnline != null)
                {

                    UnityPlayerOnClient uplayer = ConvertWeaveOnlineToUnityPlayerOnClient(hasOnline);

                    WeaveDeleteEvent(uplayer);

                    weaveOnline.Remove(hasOnline);

                    // unityPlayerOnClientList.Remove(uplayer);
                    DeleteUnityPlayerOnClient(hasOnline.Socket);
                }

              
                /**/
                return;
            }

            try
            {
                //触发接收到信息的事件...
                /*我写的方法*/
                WeaveOnLine hasOnline = weaveOnline.Find(item => item.Socket == soc);
               
                if(hasOnline != null)
                {
                    UnityPlayerOnClient uplayer = ConvertWeaveOnlineToUnityPlayerOnClient(hasOnline);

                    WeaveReceiveEvent(command, data, uplayer);

                }
                /**/

                if (command == 0xff)
                {
                    //如果是网关command 发过来的 命名，那么执行下面的
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
            catch
            {
                return;
            }
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(exec));
        }



      


        /// <summary>
        /// 网关0xff这个command发来的...命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="soc"></param>
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


        /// <summary>
        /// 不是0xff这个command发来的...命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="soc"></param>
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

        public UnityPlayerOnClient ConvertWeaveOnlineToUnityPlayerOnClient(WeaveOnLine  wonline)
        {
            UnityPlayerOnClient uplayer = new UnityPlayerOnClient()
            {
                Obj = wonline.Obj,
                Socket = wonline.Socket,
                Token = wonline.Token,
                Name = wonline.Name
            };



            return uplayer;

        }

        public void DeleteUnityPlayerOnClient(Socket osc)
        {
            try
            {
                if (unityPlayerOnClientList.Count > 0)
                    unityPlayerOnClientList.Remove(unityPlayerOnClientList.Find(u => u.Socket == osc));
            }
            catch
            {

            }
        }



        public void AddUnityPlayerClient_CheckSameItem(UnityPlayerOnClient item ,string itemName)
        {
            System.Threading.Thread.Sleep(500);
            lock (this)
            {
                if (unityPlayerOnClientList.Find(i => i.Name == itemName) != null)
                    return;

                else
                    unityPlayerOnClientList.Add(item);
            }
        }
        //public class CmdWorkItem
        //{
        //    public byte CmdName
        //    {
        //        get;set;
        //    }
        //    public WeaveTCPCommand WeaveTcpCmd
        //    {
        //        get;set;
        //    }
        //}
    }
}
