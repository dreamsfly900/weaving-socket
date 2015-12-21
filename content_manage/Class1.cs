using C;
using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace  manages
{
    public class content_manage : MyInterface.TCPCommand
    {
         
        _base_manage bm = new _base_manage();
        /// <summary>
        /// 构造函数，用于加载事件
        /// </summary>
        public content_manage()
        {
            //加载错误处理事件
            bm.errorMessageEvent += Bm_errorMessageEvent;
            //加载处理Send_content方法的请求事件
            bm.AddListen("Send_content",
                new _base_manage.RequestData(Bm_RequestDataEvent));
            //加载处理Send_content2方法的请求事件
            bm.AddListen("Send_content2",
               new _base_manage.RequestData(Bm_RequestDataEvent2));
        }

        private void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
            //这里处理错误信息
 
           
        }
        private void Bm_RequestDataEvent2(Socket soc, _baseModel _0x01)
        {

        }
        /// <summary>
        /// 处理Send_content方法的请求事件
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="_0x01"></param>
        private void Bm_RequestDataEvent(Socket soc, _baseModel _0x01)
        {
            
                Ccontext c = _0x01.GetParameter<Ccontext>();
                //处理数据库-------------
                //处理数据库-------------
                _0x01.Parameter = "ok";
                send(soc, 0x01, _0x01.Getjson());
            //获取在线人员token
            this.GetOnline();
          
        }

        public override byte Getcommand()
        {
            return 0x01;
        }
        /// <summary>
        /// 接收来自客户端的请求，并验证
        /// </summary>
        /// <param name="data">客户端发送来的数据</param>
        /// <param name="soc">包含IP和端口，用于双方传输</param>
        /// <returns></returns>
        public override bool Run(string data, Socket soc)
        {
            bm.init(data, soc);
            return true;
        }

        public override void TCPCommand_EventDeleteConnSoc(Socket soc)
        {
            
        }

        public override void TCPCommand_EventUpdataConnSoc(Socket soc)
        {
            
        }
    }
}
