using client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WeaveBase;

namespace cloud
{

    public class GateHelper
    {

        public static P2Pclient GetP2Pclient(P2Pclient[,,,] P2Pclientlist, Socket soc, WeavePipelineTypeEnum pipeline)
        {
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;

            if (pipeline == WeavePipelineTypeEnum.ten)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 1);
                return P2Pclientlist[0, 0, 0, Convert.ToInt32(t)];
            }
            else if (pipeline == WeavePipelineTypeEnum.hundred)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                return P2Pclientlist[0, 0, Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))];
            }
            else if (pipeline == WeavePipelineTypeEnum.thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Substring(clientipe.Address.ToString().Length - 1);
                return P2Pclientlist[0, Convert.ToInt32(m), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))];
            }
            else if (pipeline == WeavePipelineTypeEnum.ten_thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Split('.')[3];
                if (m.Length < 2)
                    m = "0" + m;
                return P2Pclientlist[Convert.ToInt32(m.Substring(0, 1)), Convert.ToInt32(m.Substring(1, 1)), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))];
            }
            return null;

        }

        /// <summary>
        /// 通过IP+PROT 得到连接的客户端
        /// </summary>
        /// <param name="ConnItemlist"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public static ConnObj GetConnItemlist(clientItem[,,,] ConnItemlist,String ip,int port , WeavePipelineTypeEnum pipeline)
        {
            IPEndPoint clientipe = new IPEndPoint(IPAddress.Parse(ip), port);
          
            if (pipeline == WeavePipelineTypeEnum.ten)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 1);
                return ConnItemlist[0, 0, 0, Convert.ToInt32(t)].getconn(ip, port);
            }
            else if (pipeline == WeavePipelineTypeEnum.hundred)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                return ConnItemlist[0, 0, Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].getconn(ip, port);
            }
            else if (pipeline == WeavePipelineTypeEnum.thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Substring(clientipe.Address.ToString().Length - 1);
                return ConnItemlist[0, Convert.ToInt32(m), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].getconn(ip, port);
            }
            else if (pipeline == WeavePipelineTypeEnum.ten_thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Split('.')[3];
                if (m.Length < 2)
                    m = "0" + m;
              return  ConnItemlist[Convert.ToInt32(m.Substring(0, 1)), Convert.ToInt32(m.Substring(1, 1)), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].getconn(ip, port);
            }
            return null;
            
        }
        /// <summary>
        /// 从列表中移除客户端对象
        /// </summary>
        /// <param name="ConnItemlist"></param>
        /// <param name="soc"></param>
        /// <param name="pipeline"></param>
        public static void removeConnItemlist(clientItem[,,,] ConnItemlist,Socket soc, WeavePipelineTypeEnum pipeline)
        {
            IPEndPoint clientipe =  (IPEndPoint)soc.RemoteEndPoint;

            if (pipeline == WeavePipelineTypeEnum.ten)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 1);
                 ConnItemlist[0, 0, 0, Convert.ToInt32(t)].removeconn(soc);
            }
            else if (pipeline == WeavePipelineTypeEnum.hundred)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                 ConnItemlist[0, 0, Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].removeconn(soc);
            }
            else if (pipeline == WeavePipelineTypeEnum.thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Substring(clientipe.Address.ToString().Length - 1);
                 ConnItemlist[0, Convert.ToInt32(m), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].removeconn(soc);
            }
            else if (pipeline == WeavePipelineTypeEnum.ten_thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Split('.')[3];
                if (m.Length < 2)
                    m = "0" + m;
                 ConnItemlist[Convert.ToInt32(m.Substring(0, 1)), Convert.ToInt32(m.Substring(1, 1)), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].removeconn(soc);
            }
          

        }
        /// <summary>
        /// 将对象存入数组对象中
        /// </summary>
        /// <param name="ConnItemlist"></param>
        /// <param name="connb"></param>
        /// <param name="pipeline"></param>
        public static void SetConnItemlist(clientItem[,,,] ConnItemlist, ConnObj connb, WeavePipelineTypeEnum pipeline)
        {
            
            IPEndPoint clientipe = (IPEndPoint)connb.Soc.RemoteEndPoint;
            if (pipeline == WeavePipelineTypeEnum.ten)
            {
               string t= clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 1);
                connb.Id = Convert.ToInt32(t);
                  ConnItemlist[0, 0, 0, Convert.ToInt32(t)].setconn(connb);
               
            }
            else if (pipeline == WeavePipelineTypeEnum.hundred)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                connb.Id = Convert.ToInt32(t);
                ConnItemlist[0, 0, Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1,1))].setconn(connb);
            }
            else if (pipeline == WeavePipelineTypeEnum.thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Substring(clientipe.Address.ToString().Length - 1);
                connb.Id = Convert.ToInt32(m+t);
                ConnItemlist[0, Convert.ToInt32(m), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].setconn(connb);
            }
            else if (pipeline == WeavePipelineTypeEnum.ten_thousand)
            {
                string t = clientipe.Port.ToString().Substring(clientipe.Port.ToString().Length - 2);
                string m = clientipe.Address.ToString().Split('.')[3];

                if (m.Length < 2)
                    m = "0" + m;
                connb.Id = Convert.ToInt32(m + t);
                ConnItemlist[Convert.ToInt32(m.Substring(0, 1)), Convert.ToInt32(m.Substring(1,1)), Convert.ToInt32(t.Substring(0, 1)), Convert.ToInt32(t.Substring(1, 1))].setconn(connb);
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

        P2Pclient[,,,] client = new P2Pclient[10, 10, 10, 10];
        public P2Pclient[,,,] Client { get => client; set => client = value; }
        String ip = "";
        int port;
     
    }

    public class clientItem
    {
        int count = 0;
        public void setconn(ConnObj cb)
        {
            for (int i = 0; i < Connlist.Length; i++)
            {
                if (Connlist[i]==null)
                {
                    Connlist[i] = cb;
                    return;
                }
            }
            lock (Connlist)
            {
                ConnObj[] temp = new ConnObj[Connlist.Length + 1];

                Array.Copy(Connlist, temp, temp.Length - 1);
                temp[temp.Length-1] = cb; 
            }
        }
        public void removeconn(Socket soc)
        {
            for (int i=0;i< Connlist.Length;i++)
            {
                if(Connlist[i]!=null)
                if (Connlist[i].Soc.Equals(soc))
                    {
                        try
                        {
                            Connlist[i].Soc.Dispose();
                        }
                        catch { }
                    Connlist[i] = null;
                    return;
                }
            }
        }
        
       public ConnObj getconn(String ip, int port)
        {
            foreach (ConnObj cb in Connlist)
            {
                if (cb != null)
                {
                    IPEndPoint clientipe = (IPEndPoint)cb.Soc.RemoteEndPoint;
                    if ((clientipe.Address.ToString()+":"+ clientipe.Port)== (ip+":"+port))
                        return cb;
                }
            }
            return null;
        }
        public ConnObj getconn(Socket soc)
        {
            foreach (ConnObj cb in Connlist)
            {
                if(cb.Soc.Equals(soc))
                return cb;
            }
            return null;
        }
        public ConnObj[] Connlist { get => _Connlist; set => _Connlist = value; } 
        ConnObj[] _Connlist = new ConnObj[0];
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
    [Serializable]
    public class ConnObj
    {
        bool _Validation = false;
        int id = 0;
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
        string _Token;
        Socket soc;
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
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public bool Validation
        {
            get
            {
                return _Validation;
            }
            set
            {
                _Validation = value;
            }
        }
    }
}
