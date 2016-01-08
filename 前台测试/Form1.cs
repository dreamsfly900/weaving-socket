using C;
using client;
using MyInterface;
using StandardModel;
using System;
using System.Net;
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
        UDP udp = new UDP();
      
        private void Form1_Load(object sender, EventArgs e)
        {
          
            // p2pc.start("127.0.0.1", 8989);

        }

        private void Udp_receiveevent(byte command, string data, System.Net.EndPoint iep)
        {
            
        }

        [InstallFun("forever")]//forever
        public void Send_content(System.Net.Sockets.Socket soc, _baseModel _0x01)
        {
            Gw_EventMylog("",_0x01.Getjson());
        }

     
        private void P2pc_timeoutevent()
        {
            p2pc.start("127.0.0.1", Convert.ToInt32(textBox1.Text),true);
        
        }
        string tokan = "";
        private void P2pc_receiveServerEvent(byte command, string text)
        {
            
           
            
             
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
            String IP = "122.114.56.226";
            int PORT = 9987;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IP), 16680);
            udp.receiveevent += Udp_receiveevent;
            udp.start(IP, PORT, 16680);
            udp.send(0x9c, IPAddress.Any.ToString() + ":" + PORT, localEndPoint);

            p2pc.timeoutevent += P2pc_timeoutevent;
            p2pc.AddListenClass(this);

        }
       
        private void button2_Click(object sender, EventArgs e)
        {
       
            int i = 0;
            while (i < 10)
            {
                i++;
                //_baseModel bm = new _baseModel();
                //bm.Request = "Send_content";//请求的方法名
                //bm.Token = p2pc.Tokan;//服务器登录成功返回的token
                ////c中存放的是传递给服务器的内容
                Ccontext c = new Ccontext();
                c.Content = "张三去干活去。";
                c.Recusername = "张三";
                c.Sendusername = "你老大";
                //bm.SetParameter<Ccontext>(c);
                //向服务器发送数据
                // p2pc.send((byte)0x01, bm.Getjson());
                p2pc.SendParameter<Ccontext>(0x01, "Send_content", c, 0);
                p2pc.SendRoot<Ccontext>(0x01, "Send_content", c, 0);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            p2pc.start("127.0.0.1", Convert.ToInt32(textBox1.Text),true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(test));
        }

        public void test(object obj)
        { }
    }
}
