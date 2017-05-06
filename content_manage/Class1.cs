 using C;
using MyInterface;
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
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
             
        }
        [InstallFun("forever")]
        public void getnum(Socket soc, _baseModel _0x01)
        {
            int num = 9987;
            SendRoot<int>(soc, 0x01, _0x01.Request, num, 0, _0x01.Token);
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
