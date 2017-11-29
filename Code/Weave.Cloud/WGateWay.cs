using client;
using MyInterface;
using P2P;
using StandardModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace cloud
{
    public class WebGateWay
    {
        Webp2psever p2psev;
        XmlDocument xml = new XmlDocument();
        List<CommandItem> listcomm = new List<CommandItem>();
        QueueTable qt = new QueueTable();
        public delegate void Mylog(string type, string log);
        public event Mylog EventMylog;
        public List<ConnObj> ConnObjlist = new List<ConnObj>();
        public List<CommandItem> CommandItemS = new List<CommandItem>();

        public bool Run(string loaclIP, int port)
        {
            // Mycommand comm = new Mycommand(, connectionString);


            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFlies));


            p2psev = new Webp2psever();

            p2psev.receiveevent += p2psev_receiveevent;
            p2psev.EventUpdataConnSoc += p2psev_EventUpdataConnSoc;
            p2psev.EventDeleteConnSoc += p2psev_EventDeleteConnSoc;
            //   p2psev.NATthroughevent += tcp_NATthroughevent;//p2p事件，不需要使用
            p2psev.start(Convert.ToInt32(port));

            if (EventMylog != null)
                EventMylog("连接", "连接启动成功");
            return true;
        }

        public void ReLoad()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFlies));

        }

        private void V_ErrorMge(int type, string error)
        {
            if (EventMylog != null)
                EventMylog("V_ErrorMge", type + ":" + error);
        }
        /// <summary>
        /// 这里写断线后操作
        /// </summary>
        private void V_timeoutevent()
        {
            //int count = CommandItemS.Count;
            //CommandItem[] comItems = new CommandItem[count];
            //CommandItemS.CopyTo(0, comItems, 0, count);
            try
            {
                foreach (CommandItem ci in CommandItemS)
                {
                    if (!ci.Client.Isline)
                    {
                        if (ci.Client.Restart())
                        {
                            V_timeoutevent();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("节点重新连接", ex.Message);
                V_timeoutevent();
            }
        }
        /// <summary>
        /// 这里写接收到内容后，转发
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        private void V_receiveServerEvent(byte command, string text)
        {
            _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(text);

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
                            p2psev.WEBsend(coob.Soc, command, text);
                        }
                }
            }
            catch (Exception ex) { EventMylog("转发", ex.Message); }

        }





        void ReloadFlies(object obj)
        {
            try
            {

                foreach (CommandItem ci in CommandItemS)
                {
                    ci.Client.stop();
                }
                CommandItemS.Clear();
                xml.Load("node.xml");
                foreach (XmlNode xn in xml.ChildNodes)
                {


                    CommandItem ci = new CommandItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.CommName = byte.Parse(xn.Attributes["command"].Value);
                    ci.Client = new P2Pclient(false);
                    ci.Client.receiveServerEvent += V_receiveServerEvent;
                    ci.Client.timeoutevent += V_timeoutevent;
                    ci.Client.ErrorMge += V_ErrorMge;
                    if (ci.Client.start(ci.Ip, ci.Port))
                    {
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



        void p2psev_EventDeleteConnSoc(System.Net.Sockets.Socket soc)
        {
            try
            {
                int count = ConnObjlist.Count;
                ConnObj[] coobjs = new ConnObj[count];
                ConnObjlist.CopyTo(0, coobjs, 0, count);
                foreach (ConnObj coob in coobjs)
                {
                    if (coob != null)
                        if (coob.Soc.Equals(soc))
                        {
                            ConnObjlist.Remove(coob);
                        }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }

        void p2psev_EventUpdataConnSoc(System.Net.Sockets.Socket soc)
        {
            ConnObj cobj = new ConnObj();
            cobj.Soc = soc;
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;
            cobj.Token = EncryptDES(clientipe.Address.ToString() + "|" + DateTime.Now.ToString(), "lllssscc");

            try
            {
                if (p2psev.WEBsend(soc, 0xff, cobj.Token))
                    ConnObjlist.Add(cobj);
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("EventUpdataConnSoc", ex.Message);
            }
        }


        void p2psev_receiveevent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(data);
            string key = DecryptDES(_0x01.Token, "lllssscc");
            string ip = key.Split('|')[0];
            IPEndPoint clientipe = (IPEndPoint)soc.RemoteEndPoint;

            if (clientipe.Address.ToString() == ip)
            {
                int count = CommandItemS.Count;
                CommandItem[] comItems = new CommandItem[count];
                CommandItemS.CopyTo(0, comItems, 0, count);
                foreach (CommandItem ci in comItems)
                {
                    if (ci != null)
                    {
                        if (ci.CommName == command)
                        {
                            if (!ci.Client.send(command, data))
                            {
                                p2psev.WEBsend(soc, 0xff, "你所请求的服务暂不能使用，请联系管理人员。");
                            }
                            return;
                        }
                    }
                }
                p2psev.WEBsend(soc, 0xff, "你所请求的服务是不存在的。");

            }
            else
            {
                p2psev.WEBsend(soc, 0xff, "您的请求是非法的~");
            }

        }
        private byte[] Keys = { 0xEF, 0xAB, 0x56, 0x78, 0x90, 0x34, 0xCD, 0x12 };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }


    }
}
