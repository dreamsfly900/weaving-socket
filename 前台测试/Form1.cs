using C;
using client;
using StandardModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TCPclient;
namespace 前台测试
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
        }
        P2Pclient p2pc = new P2Pclient(false);
        _base_manage xmhelper = new _base_manage();
        private void Form1_Load(object sender, EventArgs e)
        {
          
            // p2pc.start("127.0.0.1", 8989);
          
        }

        private void Xmhelper_RequestDataEvent(System.Net.Sockets.Socket soc, _baseModel _0x01)
        {
            Gw_EventMylog("",_0x01.Getjson());
        }

        private void Xmhelper_errorMessageEvent(System.Net.Sockets.Socket soc, _baseModel _0x01, string message)
        {
            
        }

        private void P2pc_timeoutevent()
        {
            p2pc.start("127.0.0.1", Convert.ToInt32(textBox1.Text));  
        }
        string tokan = "";
        private void P2pc_receiveServerEvent(byte command, string text)
        {
            if (command == 0x01)
                xmhelper.init(text, null);
            
             
        }
        public delegate void Mylog(Control c, string log);
        private void Gw_EventMylog(string type, string log)
        {
            Mylog ml = new Mylog(addMylog);
            txtLog.Invoke(ml, new object[] { txtLog, type + "--" + log });
        }
        void addMylog(Control c, string log)
        {
            c.Text += log + "\r\n";
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            //加载用于处理返回的错误消息事件
            xmhelper.errorMessageEvent += Xmhelper_errorMessageEvent;
           //加载监听处理“服务器返回结果”的事件：Send_content为客户端与服务器端相约的方法名
            xmhelper.AddListen("Send_content", new _base_manage.RequestData(Xmhelper_RequestDataEvent));
            //加载接收“服务器返回结果”事件
            p2pc.receiveServerEvent += P2pc_receiveServerEvent;
            //加载超时处理事件
            p2pc.timeoutevent += P2pc_timeoutevent;
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
       
            int i = 0;
            while (i < 10)
            {
                i++;
                _baseModel bm = new _baseModel();
                bm.Request = "Send_content";//请求的方法名
                bm.Token = p2pc.Tokan;//服务器登录成功返回的token
                //c中存放的是传递给服务器的内容
                Ccontext c = new Ccontext();
                c.Content = "张三去干活去。";
                c.Recusername = "张三";
                c.Sendusername = "你老大";
                bm.SetParameter<Ccontext>(c);
                //向服务器发送数据
                p2pc.send((byte)0x01, bm.Getjson());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            p2pc.start("127.0.0.1", Convert.ToInt32(textBox1.Text));
        }
    }
}
