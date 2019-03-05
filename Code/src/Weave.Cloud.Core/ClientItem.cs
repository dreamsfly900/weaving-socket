using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Weave.Cloud
{
    /// <summary>
    /// 客户端封装类 ，拥有Connlist 字典（ string, ConnObj）类型和几个方法，，
    /// </summary>
    public class ClientItem
    {
        // int count = 0;
        public void setconn(ConnObj cb)
        {
            IPEndPoint clientipe = (IPEndPoint)cb.Soc.RemoteEndPoint;
        llbb11:
            string key = clientipe.Address.ToString() + ":" + clientipe.Port;
            Connlist.Add(key, cb);
            if (Connlist.ContainsKey(key))
                return;
            else
                goto llbb11;
        }
        public void removeconn(Socket soc)
        {
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;

            string key = clientipe.Address.ToString() + ":" + clientipe.Port;

            //这里通过IP和PORT获取对象
            if (Connlist.ContainsKey(key))
                Connlist.Remove(key);
        }

        public ConnObj getconn(String ip, int port)
        {
            string key = ip + ":" + port;

            //这里通过IP和PORT获取对象
            if (Connlist.ContainsKey(key))
                return Connlist[key];
            else
                return null;
        }
        public ConnObj getconn(Socket soc)
        {
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;

            string key = clientipe.Address.ToString() + ":" + clientipe.Port;

            //这里通过IP和PORT获取对象
            if (Connlist.ContainsKey(key))
                return Connlist[key];
            else
                return null;
        }
        public Dictionary<string, ConnObj> Connlist { get { return _Connlist; } set { _Connlist = value; } }
        Dictionary<string, ConnObj> _Connlist = new Dictionary<string, ConnObj>();


    }
}
