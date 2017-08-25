using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WeaveBase;

namespace ClassLibraryTest
{
    public class tequipment
    {
       public string data;
        public string SnID;
    }
    
    public class Class1 : WeaveTCPCommand
    {
        public Class1()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(setdatatoweb));
            t.Start();
        }
        List<Socket> soclist = new List<Socket>();
        Hashtable listtequipment = new Hashtable();
        public void setdatatoweb()
        {
            while (true)
            {
                Socket [] Sockets= soclist.ToArray();
                foreach (Socket soc in Sockets)
                {
                    try
                    {


                        foreach (string item in listtequipment.Keys)
                        {
                            tequipment tq = listtequipment[item] as tequipment;
                            if (tq != null)
                                SendRoot<tequipment>(soc, 0x01, "getdata", tq, 0, "token");
                        }
                       
                    }
                    catch(Exception e)
                    { }
                }
              
                System.Threading.Thread.Sleep(3000);
            }
        }
        public override byte Getcommand()
        {
            return 0x01;
        }
        [InstallFun("forever")]
        public void weblogin(Socket soc, WeaveSession _0x01)
        {
            soclist.Add(soc);
        }

        [InstallFun("forever")]
        public void getequipmentData(Socket soc, WeaveSession _0x01)
        {
           
            tequipment tq= _0x01.GetRoot<tequipment>();
            if (listtequipment[tq.SnID] == null)
                listtequipment.Add(tq.SnID, tq);
            else {
                listtequipment[tq.SnID] = tq;
            }
        }

        public override bool Run(string data, Socket soc)
        {
            return true;
        }

        public override void WeaveBaseErrorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
          
        }

        public override void WeaveDeleteSocketEvent(Socket soc)
        {
            soclist.Remove(soc);
        }

        public override void WeaveUpdateSocketEvent(Socket soc)
        {
            
        }
    }
}
