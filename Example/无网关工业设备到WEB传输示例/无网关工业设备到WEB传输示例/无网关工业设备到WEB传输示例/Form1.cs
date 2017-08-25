using client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 无网关工业设备到WEB传输示例
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        P2Pclient p2p = new P2Pclient(false);
        private void Form1_Load(object sender, EventArgs e)
        {
            p2p.receiveServerEvent += P2p_receiveServerEvent;
            p2p.timeoutevent += P2p_timeoutevent;
            p2p.start("127.0.0.1", 8989,false);
            timer1.Start();

        }
         
        private void P2p_timeoutevent()
        {
            timer1.Stop();
            if (p2p.Restart(false))
            {
               
            }
        }

        private void P2p_receiveServerEvent(byte command, string text)
        { 

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tequipment t = new tequipment();
            t.SnID = "99999";
            t.data = new Random().Next(0,1000).ToString();
            label1.Text = "发送数据：" + t.data;
            p2p.SendRoot<tequipment>(0x01, "getequipmentData", t, 0);
        }
    }
    public class tequipment
    {
        public string data;
        public string SnID;
    }
}
