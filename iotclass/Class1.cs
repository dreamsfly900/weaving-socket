using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using StandardModel;
using MyInterface;

namespace iotclass
{
    public class webclass : MyInterface.TCPCommand
    {

        public webclass()
        {
        
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(senddata));
            t.Start();
        }
        void senddata()
        {
            while (true)
            {
                int value = 0;
               System.Threading.Thread.Sleep(500);
                try {
                     value =int.Parse( this.GlobalQueueTable["iotvalue"].ToString());
                }
                catch { this.GlobalQueueTable.Add("iotvalue", "0"); }
                online [] ol=   this.GetOnline();

                    foreach (online o in ol)
                    {
                        try
                        {
                            if (o != null && o.Name == "客户")
                            {
                                SendRoot<int>(o.Soc, 0x02, "getvalue", value, 0, o.Token);
                            }
                        }
                        catch { }
                    }
              
            }
        }
        [InstallFun("forever")]
        public void command(Socket soc, _baseModel _0x01)
        {
            online[] ol = this.GetOnline();

            foreach (online o in ol)
            {
                if (o != null && o.Name=="设备")
                {
                    SendRoot<string>(o.Soc, 0x02, "command", _0x01.Root, 0, o.Token);
                }
            }
        }
        [InstallFun("forever")]
        public void login(Socket soc, _baseModel _0x01)
        {
             online ol= GetonlineByToken(_0x01.Token);
             ol.Name = "客户";

        }
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
           //这里是错误日志
        }

        public override byte Getcommand()
        {
            try
            {
                this.GlobalQueueTable.Add("iotvalue", 0);
            }
            catch { }
            return 0x02;
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
