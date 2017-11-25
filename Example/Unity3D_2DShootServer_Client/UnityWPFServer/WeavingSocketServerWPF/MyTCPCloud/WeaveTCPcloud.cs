
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

        public event WeaveServerReceiveSocketMessageCallBack WeaveServerReceiveSocketMessageCallBackEvent;


    public event WeaveServerDeleteSocketCallBack WeaveServerDeleteSocketCallBackEvent;


    public event WeaveServerUpdateSocketCallBack WeaveServerUpdateSocketCallBackEvent;


    public event WeaveServerReceiveOnLineUnityPlayerMessageCallBack WeaveServerReceiveOnLineUnityPlayerMessageCallBackEvent;


    public event WeaveServerGetUnityPlayerOnLineCallBack WeaveServerGetUnityPlayerOnLineCallBackEvent;

    public event WeaveServerGetUnityPlayerOffLineCallBack WeaveServerGetUnityPlayerOffLineCallBackEvent;


        //public XmlDocument xml
        //{
        //    get;set;
        //}

        public List<WeaveTCPCommandItem> weaveTCPCommandItems
        {
            get
            {
                return _weaveTCPCommandItems;
            }

            set
            {
                _weaveTCPCommandItems = value;
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

        List<WeaveTCPCommandItem> _weaveTCPCommandItems = new List<WeaveTCPCommandItem>();
        
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
                loginCmd.ServerLoginOKEvent += OnUnityPlayerLoginOK;
                AddCmdWorkItems(loginCmd);

                AddCmdWorkItems(new GameScoreCommand());

                AddCmdWorkItems(new ClientDisConnectedCommand());



            }
            catch
            {

            }
        }

        private void OnUnityPlayerLoginOK(string _u, Socket _socket)
        {
            WeaveOnLine onLineSocket = weaveOnline.Find(w => w.Socket == _socket);

            // throw new NotImplementedException();
            UnityPlayerOnClient gamer = new UnityPlayerOnClient()
            {
                UserName = _u,
                isLogin = true,
                 Name = onLineSocket.Name,
                  Obj =onLineSocket.Obj,
                    Socket = _socket,
                     Token = onLineSocket.Token
            };

            WeaveServerGetUnityPlayerOnLineCallBackEvent(gamer);

        }

        public bool CheckIsOnLinePlayerMessage(Socket _socket)
        {
            WeaveOnLine onLineSocket = weaveOnline.Find(w => w.Socket == _socket);
            if (unityPlayerOnClientList.Count == 0)
                return false;
            else
            {
                UnityPlayerOnClient player = unityPlayerOnClientList.Find(u => u.Name == onLineSocket.Name);
                if (player != null)
                    return true;
                else
                    return false;
            }
            
        }

        public void Check_ReceiveMessageCallBackEvent(byte command, string data, Socket _socket)
        {
            if(CheckIsOnLinePlayerMessage(_socket))
            {
                UnityPlayerOnClient player = unityPlayerOnClientList.Find(u => u.Socket == _socket);
                WeaveServerReceiveOnLineUnityPlayerMessageCallBackEvent(command, data, player);
            }
            else
            {
                WeaveOnLine onLine = weaveOnline.Find(w => w.Socket == _socket);
                WeaveServerReceiveSocketMessageCallBackEvent( command, data, onLine);
            }
        }


        public void AddCmdWorkItems(WeaveTCPCommand cmd)
        {
            cmd.SetGlobalQueueTable(weaveTable, TcpToken);
            WeaveTCPCommandItem cmdItem = new WeaveTCPCommandItem();
            // Ic.SetGlobalQueueTable(weaveTable, TcpTokenList);
            cmdItem.WeaveTcpCmd = cmd;
            cmdItem.CmdName = cmd.Getcommand();
            GetAttributeInfo(cmd, cmd.GetType(), cmd);
            weaveTCPCommandItems.Add(cmdItem);
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
       

        void P2ServerUpdateSocketHander(System.Net.Sockets.Socket soc)
        {
            CallWeaveServerUpdateSocketCallBackEvent(soc);
           

            CallWeaveTCPCommandClassUpdateSocketEvent(soc);


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


                        #region  if(sendok) 如果发送token成功
                        if (sendok)
                        {
                            WeaveOnLine onLine = CreatWeaveOnLine(Token, soc);

                            AddWeaveOnLine(onLine);



                            ForeachWeaveExecuteRuncommandMethod(onLine);
                            
                            return;
                        }
                        #endregion
                    }
                }
                else
                {
                    WeaveOnLine online = CreatWeaveOnLine(soc);
                    weaveOnline.Add(online);

                    

                }

            }
        }


        void P2ServerDeleteSocketHander(System.Net.Sockets.Socket soc)
        {


            CallWeaveServerDeleteSocketCallBackEvent(soc);

            // WeaveServerGetUnityPlayerOffLineCallBackEvent()
            CallWeaveServerGetUnityPlayerOffLineCallBackEvent(soc);

            CallWeaveTCPCommandClassDeleteSocketEvent(soc);

            WeaveExecuteRuncommandMethod_Tokenout_RemoveWeaveOnline(soc);

        }


        void P2ServerReceiveHander(byte command, string data, System.Net.Sockets.Socket soc)
        {
            //这里待定判定，，是登陆用户的 还是，，，原始Socket发过来的,,来判定执行不同的事件
            //普通Socket数据接收事件，还是 登陆的Unity客户端发过来的数据，进入数据接收处理事件
            Check_ReceiveMessageCallBackEvent(command, data, soc);
            //客户端 登陆 事件要 在这里执行

            //客户端 登陆的 Unity用户发来的消息 会 在这里执行
            if(command == (byte)CommandEnum.ClientSendDisConnected)
            {
                P2Server.CliendSendDisConnectedEvent(soc);

                
                return;
            }

            try
            {
                if (command == 0xff)
                {
                    //如果是网关command 发过来的 命名，那么执行下面的
                    WeaveExecuteRuncommandMethod(command, data, soc);
                   
                    try
                    {
                        string[] temp = data.Split('|');
                        if (temp[0] == "in")
                        {
                         
                            WeaveAddOnLineTokenIn(temp[1] , soc);
                            return;
                        }
                        else if (temp[0] == "Restart")
                        {
                            RestartWeaveOnLineSocket(temp[1], soc);
                           
                        }
                        else if (temp[0] == "out")
                        {
                            WeaveExecuteTokenoutMethod(temp[1]);
                            ////移出onlinetoken
                        }
                    }
                    catch
                    {
                    }
                    return;
                }

                else
                    WeaveExecuteRunMethod(command, data, soc);
            }
            catch
            {
                return;
            }
        }

        public void CallWeaveServerUpdateSocketCallBackEvent(Socket _soc)
        {
            WeaveOnLine online = weaveOnline.Find(s => s.Socket == _soc);
            if (online == null)
            {
                WeaveOnLine temponline = CreatWeaveOnLine(_soc);
                WeaveServerUpdateSocketCallBackEvent(temponline);
            }
            else
            {
                WeaveServerUpdateSocketCallBackEvent(online);
            }
                
        }

        public void CallWeaveServerDeleteSocketCallBackEvent(Socket _soc)
        {
            WeaveOnLine online = weaveOnline.Find(s => s.Socket == _soc);
            if(online !=null)
                WeaveServerDeleteSocketCallBackEvent(online);
        }


        public void CallWeaveServerGetUnityPlayerOffLineCallBackEvent(Socket _soc)
        {
            UnityPlayerOnClient unitygm = unityPlayerOnClientList.Find(p => p.Socket == _soc);

            WeaveServerGetUnityPlayerOffLineCallBackEvent(unitygm);
        }


        public void WeaveExecuteRuncommandMethod_Tokenout_RemoveWeaveOnline(Socket _soc)
        {
            try
            {
                int count = weaveOnline.Count;
                WeaveOnLine[] ols = new WeaveOnLine[count];
                weaveOnline.CopyTo(0, ols, 0, count);
                foreach (WeaveOnLine ol in ols)
                {
                    if (ol.Socket.Equals(_soc))
                    {
                        foreach (WeaveTCPCommandItem weaveTCPCommandItem in weaveTCPCommandItems)
                        {
                            try
                            {
                                WeaveExecuteRuncommandMethod(0xff, "out|" + ol.Token, ol.Socket);
                                weaveTCPCommandItem.WeaveTcpCmd.Tokenout(ol);
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

        public void  CallWeaveTCPCommandClassDeleteSocketEvent(Socket soc)
        {
            try
            {
                int count = weaveTCPCommandItems.Count;
                WeaveTCPCommandItem[] commandlist = new WeaveTCPCommandItem[count];
                weaveTCPCommandItems.CopyTo(0, commandlist, 0, count);
                foreach (WeaveTCPCommandItem weaveTCPCommandItem in commandlist)
                {
                    try
                    {
                        weaveTCPCommandItem.WeaveTcpCmd.WeaveDeleteSocketEvent(soc);
                    }
                    catch (Exception ex)
                    {
                        if (WeaveLogEvent != null)
                            WeaveLogEvent("EventDeleteConnSoc", ex.Message);
                    }
                }
            }
            catch { }
        }
        

        public void CallWeaveTCPCommandClassUpdateSocketEvent(Socket soc)
        {
            try
            {
                int count = weaveTCPCommandItems.Count;
                WeaveTCPCommandItem[] commandlist = new WeaveTCPCommandItem[count];
                weaveTCPCommandItems.CopyTo(0, commandlist, 0, count);
                foreach (WeaveTCPCommandItem weaveTCPCommandItem in commandlist)
                {
                    try
                    {
                        weaveTCPCommandItem.WeaveTcpCmd.WeaveUpdateSocketEvent(soc);
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
        }


        public void ForeachWeaveExecuteRuncommandMethod(WeaveOnLine onLine)
        {


            foreach (WeaveTCPCommandItem cmdItem in weaveTCPCommandItems)
            {
                try
                {
                    WeaveExecuteRuncommandMethod(0xff, "in|" + onLine.Token, onLine.Socket);
                    cmdItem.WeaveTcpCmd.TokenIn(onLine);
                }
                catch (Exception ex)
                {
                    if (WeaveLogEvent != null)
                        WeaveLogEvent("Tokenin", ex.Message);
                }
            }
        }


        /// <summary>
        /// 执行继承自WeaveTCPCommand类里面的Runcommand方法
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="soc"></param>
        public void WeaveExecuteRuncommandMethod(byte command, string data, System.Net.Sockets.Socket soc)
        {
            foreach (WeaveTCPCommandItem cmd in weaveTCPCommandItems)
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
        public void WeaveExecuteRunMethod(byte command, string data, System.Net.Sockets.Socket soc)
        {
            foreach (WeaveTCPCommandItem cmd in weaveTCPCommandItems)
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

        public void WeaveExecuteTokenoutMethod(string _token)
        {

            ////移出onlinetoken
            int count = weaveOnline.Count;
            WeaveOnLine[] ols = new WeaveOnLine[count];
            weaveOnline.CopyTo(0, ols, 0, count);
            foreach (WeaveOnLine onlinesession in ols)
            {
                if (onlinesession.Token == _token)//temp[1])
                {
                    foreach (WeaveTCPCommandItem cmdItem in weaveTCPCommandItems)
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
        

        public void RestartWeaveOnLineSocket(string _s , Socket _soc)
        {
            int count = weaveOnline.Count;
            WeaveOnLine[] ols = new WeaveOnLine[count];
            weaveOnline.CopyTo(0, ols, 0, count);
            string IPport = ((System.Net.IPEndPoint)_soc.RemoteEndPoint).Address.ToString() + ":" + _s;// temp[1];
            foreach (WeaveOnLine ol in ols)
            {
                try
                {
                    if (ol.Socket != null)
                    {
                        String IP = ((System.Net.IPEndPoint)ol.Socket.RemoteEndPoint).Address.ToString() + ":" + ((System.Net.IPEndPoint)ol.Socket.RemoteEndPoint).Port;
                        if (IP == IPport)
                        {
                            ol.Socket = _soc;
                        }
                    }
                }
                catch { }
            }
        }

        public void WeaveAddOnLineTokenIn(string _s , Socket soc)
        {
            WeaveOnLine ol = new WeaveOnLine();
            ol.Token = _s;
            ol.Socket = soc;
            weaveOnline.Add(ol);
            foreach (WeaveTCPCommandItem weaveTCPCommandItem in weaveTCPCommandItems)
            {
                try
                {
                    weaveTCPCommandItem.WeaveTcpCmd.TokenIn(ol);
                }
                catch (Exception ex)
                {
                    WeaveLogEvent?.Invoke("Tokenin", ex.Message);
                }
            }
        }


        public WeaveOnLine CreatWeaveOnLine(Socket _socket)
        {
            WeaveOnLine ol = new WeaveOnLine()
            {
                Name = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Obj = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Socket = _socket,
                Token = DateTime.Now.ToString("yyyyMMddHHmmssfff")

            };

            return ol;
        }

        public WeaveOnLine CreatWeaveOnLine(string _token,Socket _socket)
        {
            WeaveOnLine ol = new WeaveOnLine()
            {
                Name = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Obj = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Socket = _socket,
                Token = _token

            };

            return ol;
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

        public void AddWeaveOnLine(WeaveOnLine item)
        {
            WeaveOnLine hasOnline = weaveOnline.Find(w => w.Name == item.Name);
            {
                if (hasOnline != null)
                {
                    weaveOnline.Remove(hasOnline);
                    //添加修改参数后的WeaveOnLine
                    weaveOnline.Add(item);

                }
                else
                {
                    weaveOnline.Add(item);
                }
            }
        }
    }
}
