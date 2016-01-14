using client;
using cloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 统一登录服务
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        GateWay gw;
       
        bool isweb = true;
        private void Form1_Load(object sender, EventArgs e)
        {
          
         
        }
        public delegate void Mylog(Control c, string log);
        private void Gw_EventMylog(string type, string log)
        {
            Mylog ml = new Mylog(addMylog);
            txtLog.Invoke(ml,new object[]{ txtLog , type+"--"+log });
        }
        void addMylog(Control c, string log)
        {
            c.Text += log+"\r\n";
        }
        private void 启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isweb = false;
            gw = new GateWay(isweb);
            gw.Proportion = Convert.ToInt32(toolStripTextBox3.Text);
            gw.EventMylog += Gw_EventMylog;
            if (gw.Run("127.0.0.1", Convert.ToInt32(toolStripTextBox1.Text),int.Parse(toolStripTextBox2.Text)))
            {
                toolStripStatusLabel1.Text = "服务启动成功，端口"+  (toolStripTextBox1.Text) + "。";
                timer1.Start();
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            try
            {
                if (!isweb)
                {
                    foreach (CommandItem ci in gw.CommandItemS)
                    {
                        foreach (P2Pclient cl in ci.Client)
                        {
                            listBox1.Items.Add("指令状态：" + ci.CommName + ":" + "ip:" + ci.Ip + "-状态：" + cl.Isline + "-未处理消息：" + cl.ListData.Count);
                        }
                    }
                    foreach (WayItem ci in gw.WayItemS)
                    {
                        listBox2.Items.Add("ip:" + ci.Ip + "端口：" + ci.Port + "-状态：" + ci.Client.Isline + "-在线人数:" + ci.Num);
                    }
                    toolStripStatusLabel2.Text = "连接人数：" + gw.ConnObjlist.Count + "  ";
                }
            }
            catch { }
             
        }

        private void 重写加载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gw.ReLoad();
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
     
        private void 启动WEB网关ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isweb = true;
           gw = new GateWay(isweb);
            gw.EventMylog += Gw_EventMylog;
            if (gw.Run("127.0.0.1", Convert.ToInt32(toolStripTextBox1.Text), int.Parse(toolStripTextBox2.Text)))
            {
                toolStripStatusLabel1.Text = "WEB服务启动成功，端口" + (toolStripTextBox1.Text) + "。";
                timer1.Start();
            }
        }

        private void 重写加载WEB节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gw.ReLoad();
        }
    }
}
