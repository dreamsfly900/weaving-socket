using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using StandardModel;
using MyInterface;

namespace iotclass
{
    public class iotclass : MyInterface.TCPCommand
    {
        public iotclass()
        {
            
        }
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
          
        }
      
        public override byte Getcommand()
        {
            return 0x03;
        }
        [InstallFun("forever")]
        public void login(Socket soc, _baseModel _0x01)
        {
            online ol = GetonlineByToken(_0x01.Token);
            ol.Name = "设备";
        }
        [InstallFun("forever")]
        public void setvalue(Socket soc, _baseModel _0x01)
        {
            this.GlobalQueueTable["iotvalue"] = _0x01.Root;
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
