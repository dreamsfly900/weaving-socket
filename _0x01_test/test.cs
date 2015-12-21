using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace _0x01_test
{
    public class test : MyInterface.TCPCommand
    {
        _0x1_manage xmhelper = new _0x1_manage();
        /// <summary>
        /// 构造函数，用来初始化一些内容
        /// </summary>
        public test()
        {
            xmhelper.RequestDataEvent += Xmhelper_RequestDataEvent;
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
        }

        /// <summary>
        /// 解析后错误，的事件
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="_0x01"></param>
        /// <param name="message"></param>
        private void Xmhelper_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
          
        }
        /// <summary>
        /// 解析后正确，的事件
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="_0x01"></param>
        /// <param name="Requestkey"></param>
        private void Xmhelper_RequestDataEvent(Socket soc, _baseModel _0x01, string Requestkey)
        {
           
        }

        public override byte Getcommand()
        {
            return 0x01;
        }

        public override bool Run(string data, Socket soc)
        {

            xmhelper.init(data, soc);
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
