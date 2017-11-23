using client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMQ;

namespace WMQClient
{
    public enum WMQType {  Queue, topic }
   public   class WMQclient
    {
        P2Pclient client = new P2Pclient(false);
        public delegate void receive(WMQData text);
        public event receive receivetopicEvent;
        public event receive receiveQueueEvent;
        public WMQclient(String ip,int port, WMQType type)
        {
            Type = type;
            client.receiveServerEvent += Client_receiveServerEvent;
            client.timeoutevent += Client_timeoutevent;
            client.start(ip, port, false);

        }
        private void Client_timeoutevent()
        {
            if (!client.Isline)
            {
                if (!client.Restart(false))
                    System.Threading.Thread.Sleep(3000);
            }
        }
        WMQType Type;
        String Token;
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        public bool Reg(String token)
        {
            Token = token;
            RegData rd = new RegData();
            if (Type == WMQType.topic)
            {
                rd.to = token;
                rd.type = "topic";
            }
            else
            {
                rd.to = token;
                rd.type = "Queue";
            }
            return Send(0x0, rd);
           // RegData
        }
         bool Send<T>( byte command, T t)
        {
            String str = Newtonsoft.Json.JsonConvert.SerializeObject(t); 
            return client.send(command, str);  
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to">topic则是组名，</param>
        /// <param name="t"></param>
        ///  /// <param name="Validityperiod">时效（毫秒）</param>
        /// <returns></returns>
        public bool Send<T>(String to,T t,int Validityperiod)
        {
            String str = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            WMQData wmd = new WMQData();

            byte command = 0x01;
            if (Type == WMQType.topic)
            {
                command = 0x02;
                wmd.to = to;
                wmd.message = str;
                wmd.Validityperiod = Validityperiod;
            }
            else
            {
                command = 0x01;
                wmd.to = to;
                wmd.form = Token;
                wmd.message = str;
                wmd.Validityperiod = Validityperiod;
               
            }
            str = Newtonsoft.Json.JsonConvert.SerializeObject(wmd);
            System.Threading.Thread.Sleep(10);
            return client.send(command, str);

        }
        private void Client_receiveServerEvent(byte command, string text)
        {

            if (command == 0x01)
            {
                WMQData wd = Newtonsoft.Json.JsonConvert.DeserializeObject<WMQData>(text);
                receiveQueueEvent(wd);
            }
            else if(command == 0x02)
            {
                WMQData wd = Newtonsoft.Json.JsonConvert.DeserializeObject<WMQData>(text);
                receivetopicEvent(wd);
            }
        }
    }
}
