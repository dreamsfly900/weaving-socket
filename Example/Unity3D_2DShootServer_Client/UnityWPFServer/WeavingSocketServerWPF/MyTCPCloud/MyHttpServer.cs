
using WeaveBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using WeaveSocketServer;
using WeaveClient;

namespace MyTCPCloud
{
    public class MyHttpServer : HttpServer
    {
        public event Mylog EventMylog;
        public MyHttpServer(int port)
            : base(port)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReloadFlies));
        }
        public List<CommandItem> CommandItems = new List<CommandItem>();
        protected void ReloadFlies(object obj)
        {
            try
            {
                
                CommandItems.Clear();
                XmlDocument xml = new XmlDocument();
                xml.Load("node.xml");
                foreach (XmlNode xn in xml.FirstChild.ChildNodes)
                {
                    CommandItem ci = new CommandItem();
                    ci.Ip = xn.Attributes["ip"].Value;
                    ci.Port = Convert.ToInt32(xn.Attributes["port"].Value);
                    ci.CommName = byte.Parse(xn.Attributes["command"].Value);
                    CommandItems.Add(ci);
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }
        private void V_ErrorMge(int type, string error)
        {
            EventMylog("加载异常", error);
        }
        private void V_timeoutevent()
        {
        }
        private void V_receiveServerEvent(byte command, string text)
        {
        }
        public override void handleGETRequest(HttpProcessor p)
        {
            //Console.WriteLine("request: {0}", p.http_url);
            p.http_url = p.http_url.Substring(1);
            string fun = p.http_url.Split('?')[1].Split('=')[0];
            if (p.http_url == "")
                return;
            byte command = Convert.ToByte(p.http_url.Substring(0,1), 16);
            string data = p.http_url.Split('&')[1];
            p.writeSuccess();
            p.outputStream.WriteLine(fun+"(");
            getdata(p, command, data);
            p.outputStream.WriteLine(")");
        }
        public override  bool getdata(HttpProcessor p,byte command,string data)
        {
            string returnstr = "";
            foreach (CommandItem ci in CommandItems)
            {
                if (ci.CommName == command)
                {
                    P2Pclient p2p = new P2Pclient(false);
                    p2p.receiveServerEvent += new P2Pclient.receive((c, text) => {
                        returnstr = text;
                    });
                    //p2p.timeoutevent += (V_timeoutevent);
                    p2p.ErrorMge += (V_ErrorMge);
                    if (p2p.start(ci.Ip, ci.Port, false))
                    {
                        System.Threading.Thread.Sleep(200);
                        p2p.send(command, data);
                        int count = 0;
                        while (returnstr == "")
                        {
                            System.Threading.Thread.Sleep(200);
                            if (count > 450)
                            {
                                p.outputStream.WriteLine("响应超时");
                                return false;
                            }
                            count++;
                        }
                        p.outputStream.WriteLine(returnstr);
                        return true;
                    }
                    else
                    {
                        p.outputStream.WriteLine("不能连接指定的服务");
                        return false;
                    }
                }
                // _0x01.
            }
            p.outputStream.WriteLine("不能找到指定的服务");
            int i = httpProcessorList.Count;
            HttpProcessor[] hps = new HttpProcessor[i];
            httpProcessorList.CopyTo(hps);
            foreach (HttpProcessor hp in hps)
            {
                if (hp == p)
                {
                    httpProcessorList.Remove(p);
                    return true;
                }
            }
            return false;
        }
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            //Console.WriteLine("POST request: {0}", p.http_url);
            p.http_url = p.http_url.Substring(1);
            if (p.http_url == "")
                return;
            byte command = Convert.ToByte(p.http_url);
            string data = inputData.ReadToEnd();
            p.writeSuccess();
            getdata(p, command, data);
            //  _baseModel _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(data);
            //p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            //p.outputStream.WriteLine("<a href=/test>return</a><p>");
            //p.outputStream.WriteLine("{0}", data);
        }
    }
}
