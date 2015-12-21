using cloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 智信构建结构
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(txt_IP.Text!="" && txt_port.Text!="")
            {
                //验证IP地址和端口号的格式
                int portNum;
                bool isPort = Int32.TryParse(txt_port.Text, out portNum);
                if (Regex.IsMatch(txt_IP.Text.Trim(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$") && isPort && portNum >= 0 && portNum <= 65535)
                {
                    String str = "|||"+txt_IP.Text+"|"+txt_port.Text;
                    MyInterface.MyInterface mif = new MyInterface.MyInterface();
                    mif.Parameter = str.Split('|');
                    TCPcloud t = new TCPcloud();
                    if (t.Run(mif))
                    {
                        lab_info.Text = "连接启动成功！";
                        t.ReloadFlies();//重新加载插件
                    }
                }else
                {
                    lab_info.Text = "请检查IP地址或端口号的格式！";
                }

               
            }else
            {
                lab_info.Text = "IP地址和端口号不能为空！";
            }
       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lab_info.Text = "";
        }
    }
}
