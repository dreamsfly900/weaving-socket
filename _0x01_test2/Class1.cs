using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace _0x01_test2
{
    public class test : MyInterface.TCPCommand
    {

        _0x1_manage xmhelper = new _0x1_manage();
        public test()
        {
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
            xmhelper.RequestDataEvent += Xmhelper_RequestDataEvent;
        }

        private void Xmhelper_RequestDataEvent(Socket soc, _baseModel _0x01, string Requestkey)
        {
            if (_0x01.Request=="select_tableA")
            {
                modelA.Class1 c= _0x01.GetParameter<modelA.Class1>();
                c.Sex = "人妖";
                //----------------------------------------- 
                _0x01.SetRoot<modelA.Class1>(c);
                send(soc, 0x01, _0x01.Getjson());
            }
        }

        private void Xmhelper_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("error.txt");
            sw.WriteLine(message);
            sw.Close();
        }

        public override byte Getcommand()
        {
            return 0x01;
        }

        /// <summary>
        /// 接收数据方法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="soc"></param>
        /// <returns></returns>
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
