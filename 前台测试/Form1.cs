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
           
          //  Gw_EventMylog("",_0x01.Getjson());
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
            try
            {
                byte[] b = new byte[1999999999];
                String IP = "122.114.56.226";
                int PORT = 9987;
                //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IP), 16680);
                //udp.receiveevent += Udp_receiveevent;
                //udp.start(IP, PORT, 16680);
                //udp.send(0x9c, IPAddress.Any.ToString() + ":" + PORT, localEndPoint);

                p2pc.timeoutevent += P2pc_timeoutevent;
                p2pc.AddListenClass(this);
                timer1.Start();
            }
            catch (Exception ex)

            { }
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
            int a = Convert.ToInt32(textBox2.Text);
            int i =0;
            while (i < a)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(test));
                t.Start();
                System.Threading.Thread.Sleep(10);
                i++;
            }
        }
        int okcount = 0;
        int errorcount = 0;
        public void test(object obj)
        {
            P2Pclient p2pc2 = new P2Pclient(false);
            p2pc2.timeoutevent += P2pc_timeoutevent;
            p2pc2.receiveServerEvent += new P2Pclient.receive(delegate 
                (byte command, string text) {
                    lock (this)
                    {
                        reccount = reccount + 1;
                    }
            });
            if (p2pc2.start("127.0.0.1", Convert.ToInt32(textBox1.Text), true))
            {
                lock (this)
                {
                    okcount = okcount + 1;
                }
            }
            else
            {
                lock (this)
                {
                    errorcount = errorcount + 1;
                }
            }
            Ccontext c = new Ccontext();
            c.Content = "张三去干活去。";
            c.Recusername = "张三";
            c.Sendusername = "你老大";
            //bm.SetParameter<Ccontext>(c);
            //向服务器发送数据
            // p2pc.send((byte)0x01, bm.Getjson());
            p2pc2.SendParameter<Ccontext>(0x01, "Send_content", c, 0);
            
        }

        private void P2pc2_receiveServerEvent(byte command, string text)
        {
           
        }

        int reccount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "成功" + okcount;
            label3.Text = "失败" + errorcount;
            label4.Text = "接收" + reccount;
        }
    }
}
