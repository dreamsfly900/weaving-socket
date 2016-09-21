using cloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TCPServer;

namespace DTU网关程序
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DTUGateWay dtuw = new DTUGateWay();
        private void 启动WEB网关ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtuw.EventMylog += Dtuw_EventMylog;
            dtuw.Run(Convert.ToInt32(toolStripTextBox1.Text));
            textBox1.Text += "启动端口："+ toolStripTextBox1.Text;
        }
        void addMylog(Control c, string log)
        {
            c.Text += log + "\r\n";
        }
        public delegate void Mylog(Control c, string log);
        private void Dtuw_EventMylog(string type, string log)
        {
            Mylog ml = new Mylog(addMylog);
            textBox1.Invoke(ml, new object[] { txtLog, type + "--" + log });
        }
    }
}
