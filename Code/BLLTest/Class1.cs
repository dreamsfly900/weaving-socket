using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Weave.Base;

namespace Testclass
{
    public class Class1 : WeaveTCPCommand
    {
        public override byte Getcommand()
        {
            return 0x01;
        }

        public override bool Run(string data, Socket soc)
        {
            return true;
        }
        [InstallFun("forever")]
        public void login(Socket soc, WeaveSession _0x01)
        {
            Send(soc, 0x01, _0x01.Getjson());
        }
        public override void WeaveBaseErrorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
          
        }

        public override void WeaveDeleteSocketEvent(Socket soc)
        {
           
        }

        public override void WeaveUpdateSocketEvent(Socket soc)
        {
             
        }
    }
}
