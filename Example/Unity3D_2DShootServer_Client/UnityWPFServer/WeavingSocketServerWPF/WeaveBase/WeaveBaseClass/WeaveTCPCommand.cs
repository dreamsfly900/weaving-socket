using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using WeaveBase;

namespace WeaveBase
{
    public abstract class WeaveTCPCommand
    {
        #region 基础定义
        
        public List<WeaveTcpToken> WeaveTcpTokenList
        {
            get; set;
        }
     
        public WeaveTable GlobalQueueTable
        {
            get;
            private set;
        }
       
        public WeaveBaseManager Bm
        {
            get; set;
        }
        #endregion
        public WeaveTCPCommand()
        {
            WeaveTcpTokenList = new List<WeaveTcpToken>();
            Bm = new WeaveBaseManager();
            Bm.WeaveErrorMessageEvent += WeaveBaseErrorMessageEvent;
        }
        public bool RunBase(String data, Socket socket)
        {
            Bm.Init(data, socket);
            return true;
        }
        public abstract byte Getcommand();
        public abstract bool Run(String data, System.Net.Sockets.Socket soc);
        public abstract void WeaveUpdateSocketEvent(Socket soc);
        public abstract void WeaveDeleteSocketEvent(Socket soc);
        public abstract void WeaveBaseErrorMessageEvent(Socket soc, WeaveBase.WeaveSession _0x01, string message);
        public void SetGlobalQueueTable(WeaveTable weaveTable, List<WeaveTcpToken> TcpTokenlist)
        {
            WeaveTcpTokenList = TcpTokenlist;
            GlobalQueueTable = weaveTable;
        }
        /// <summary>
        /// 我新增的一个方法
        /// </summary>
        /// <param name="weaveTable"></param>
        /// <param name="_TcpToken"></param>
        public void SetGlobalQueueTable(WeaveTable weaveTable, WeaveTcpToken _TcpToken)
        {
            WeaveTcpTokenList.Add(_TcpToken);
            GlobalQueueTable = weaveTable;
        }

        public WeaveOnLine[] GetOnline()
        {
            WeaveOnLine[] ols = new WeaveOnLine[0];
            try
            {
                List<WeaveOnLine> ol = GlobalQueueTable["onlinetoken"] as List<WeaveOnLine>;
                int i = ol.Count;
                ols = new WeaveOnLine[i];
                ol.CopyTo(0, ols, 0, i);
            }
            catch { }
            return ols;
        }
        /// <summary>
        /// 根据TOKEN 获取online对象
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public WeaveOnLine GetOnLineByToken(String token)
        {
            WeaveOnLine[] ols = GetOnline();
            foreach (WeaveOnLine o in ols)
            {
                if (o != null)
                    if (o.Token == token)
                    {
                        return o;
                    }
            }
            return null;
        }
        /// <summary>
        /// 根据TOKEN 设置 name与OBJ属性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void SetOnLineByToken(String token, string name, object obj)
        {
            WeaveOnLine[] ols = GetOnline();
            foreach (WeaveOnLine o in ols)
            {
                if (o != null)
                    if (o.Token == token)
                    {
                        o.Name = name;
                        o.Obj = obj;
                    }
            }
        }
        /// <summary>
        /// 根据TOKEN 设置 OBJ属性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        public void SetonLineByToken(String token, object obj)
        {
            WeaveOnLine[] ols = GetOnline();
            foreach (WeaveOnLine o in ols)
            {
                if (o != null)
                    if (o.Token == token)
                    {
                        o.Obj = obj;
                    }
            }
        }
        /// <summary>
        /// 新增online
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddOnLine(WeaveOnLine o)
        {
            List<WeaveOnLine> ol = GlobalQueueTable["onlinetoken"] as List<WeaveOnLine>;
            ol.Add(o);
        }
        public bool SendParameter<T>(Socket soc, byte command, String Request, T Parameter, int Querycount, String Tokan)
        {
            WeaveBase.WeaveSession b = new WeaveBase.WeaveSession();
            b.Request = Request;
            b.Token = Tokan;
            b.SetParameter<T>(Parameter);
            b.Querycount = Querycount;
            bool sendok = Send(soc, command, b);
            if (!sendok)
                return Send(soc, command, b.Getjson());
            return sendok;
        }
        public bool SendRoot<T>(Socket soc, byte command, String Request, T Root, int Querycount, String Tokan)
        {
            WeaveBase.WeaveSession b = new WeaveBase.WeaveSession();
            b.Request = Request;
            b.Token = Tokan;
            b.SetRoot<T>(Root);
            b.Querycount = Querycount;
            bool sendok = Send(soc, command, b);
            if (!sendok)
                return Send(soc, command, b.Getjson());
            return sendok;
        }
        public bool SendDtu(Socket soc, byte[] Root, String ip, int port)
        {
            WeaveBase.WeaveSession b = new WeaveBase.WeaveSession();
            b.Request = "dtu";
            b.Token = ip + "|" + port;
            b.SetRoot<byte[]>(Root);
            b.Querycount = 0;
            return Send(soc, 0x00, b.Getjson());
        }
        public bool Send(Socket soc, byte command, WeaveBase.WeaveSession b)
        {
            foreach (WeaveTcpToken itp in WeaveTcpTokenList)
            {
                if (itp.P2Server.Port == ((System.Net.IPEndPoint)soc.LocalEndPoint).Port)
                {
                    if (itp.PortType == WeavePortTypeEnum.Bytes)
                    {
                        return itp.P2Server.Send(soc, command, itp.BytesDataparsing.Get_Byte(b));
                    }
                }
            }
            return false;
        }
        public bool Send(Socket soc, byte command, string text)
        {
            foreach (WeaveTcpToken itp in WeaveTcpTokenList)
            {

                if (itp.WPTE == WeavePortTypeEnum.jsonudp)
                {
                     itp.P2Server.Send(soc, command, text);
                    continue;

                }
                else if (itp.P2Server.Port == ((System.Net.IPEndPoint)soc.LocalEndPoint).Port)
                {
                    return itp.P2Server.Send(soc, command, text);
                }
            }
            return false;
        }
        /// <summary>
        /// 这个方法会被重写
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="soc"></param>
        public virtual void Runcommand(byte command, string data, Socket soc)
        {
        }
        public virtual void Tokenout(WeaveOnLine ol)
        {
        }
        public virtual void TokenIn(WeaveOnLine ol)
        {
        }
    }
}
