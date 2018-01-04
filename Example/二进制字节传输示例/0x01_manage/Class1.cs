using System;
using System.Net.Sockets;
using Weave.Base;
 

namespace _0x01_manage
{
    public class Class1 : WeaveTCPCommand
    {
         

        
        [InstallFun("forever")]//这是一个接收的方法，注册到了通讯类中
        public void getdata(Socket soc, WeaveSession _0x01)
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

      

        public override void WeaveUpdateSocketEvent(Socket soc)
        {
             
        }

        public override void WeaveDeleteSocketEvent(Socket soc)
        {
            
        }

        public override void WeaveBaseErrorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
          
        }
    }
}
