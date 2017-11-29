
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Weave.Server;
using Weave.Cloud;


namespace HTTP网关
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        MyHttpServer httpServer;
        private void 启动WEB网关ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            httpServer = new MyHttpServer(Convert.ToInt32(toolStripTextBox1.Text));
            httpServer.EventMylog += HttpServer_EventMylog;
            httpServer.Start(Convert.ToInt32(toolStripTextBox1.Text));
            textBox1.Text = toolStripTextBox1.Text+"端口已启动，可以接收HTTP请求";
        }
        public delegate void Mylog(Control c, string log);
        void addMylog(Control c, string log)
        {
            listBox3.Items.Add(log);
            //c.Text += log + "\r\n";
        }
        private void HttpServer_EventMylog(string type, string log)
        {
            Mylog ml = new Mylog(addMylog);
            listBox3.Invoke(ml, new object[] { listBox3, type + "--" + log });
        }
    }
}
