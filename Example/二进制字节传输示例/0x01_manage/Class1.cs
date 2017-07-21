using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using StandardModel;
using MyInterface;

namespace _0x01_manage
{
    public class Class1 : MyInterface.TCPCommand
    {
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
            //这里是错误日志事件
        }


        
        [InstallFun("forever")]//这是一个接收的方法，注册到了通讯类中
        public void getdata(Socket soc, _baseModel _0x01)
        {
            SendRoot<byte[]>(soc, 0x01, "getdata", _0x01.GetRoot<byte[]>(), 0, "随便我没用到");
        }
        
        public override byte Getcommand()
        {
            return 0x01;
        }




        public override bool Run(string data, Socket soc)
        {
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
