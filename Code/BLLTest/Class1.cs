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
            return 0x01;//头指令
        }

        public override bool Run(string data, Socket soc)
        {
            return true;
        }
        /// <summary>
        /// 这是一个标记了InstallFun的方法，使客户端可以通过远程RPC模式调用该方法。
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="_0x01"></param>
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
