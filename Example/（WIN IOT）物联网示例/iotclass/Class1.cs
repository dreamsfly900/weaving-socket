using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using WeaveBase;

namespace iotclass
{
    public class webclass : WeaveTCPCommand
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
                catch {
                    //this.GlobalQueueTable.Add("iotvalue", "0");
                }
                WeaveOnLine[] ol=   this.GetOnline();

                    foreach (WeaveOnLine o in ol)
                    {
                        try
                        {
                            if (o != null && o.Name == "客户")
                            {
                            SendRoot<int>(o.Socket, 0x02, "getvalue", value, 0, o.Token);
                            }
                        }
                        catch { }
                    }
              
            }
        }
        [InstallFun("forever")]
        public void command(Socket soc, WeaveSession _0x01)
        {
            WeaveOnLine [] ol = this.GetOnline();

            foreach (WeaveOnLine o in ol)
            {
                try
                {
                    if (o != null && o.Name == "设备")
                    {
                        SendRoot<string>(o.Socket, 0x02, "command", _0x01.Root, 0, o.Token);
                    }
                }
                catch { }
            }
        }
        [InstallFun("forever")]
        public void login(Socket soc, WeaveSession _0x01)
        {
            WeaveOnLine ol = GetOnLineByToken(_0x01.Token);
             ol.Name = "客户";

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
