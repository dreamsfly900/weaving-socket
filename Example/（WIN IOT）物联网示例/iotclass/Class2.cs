using System;
using System.Net.Sockets;
using WeaveBase;

namespace iotclass
{
    public class iotclass : WeaveTCPCommand
    {
        public iotclass()
        {
            
        }
       
      
        public override byte Getcommand()
        {
            return 0x03;
        }
        [InstallFun("forever")]
        public void login(Socket soc, WeaveSession _0x01)
        {
            WeaveOnLine ol = GetOnLineByToken(_0x01.Token);
            ol.Name = "设备";
        }
        [InstallFun("forever")]
        public void setvalue(Socket soc, WeaveSession _0x01)
        {
            try
            {
                this.GlobalQueueTable["iotvalue"] = _0x01.Root;
            }
            catch { }
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
