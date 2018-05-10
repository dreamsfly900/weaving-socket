

using System;
using System.Windows.Forms;
using Weave.TCPClient;
using Weave.Base;
namespace winformclient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int count = 0;
        WeaveUDPclient p2pc = new WeaveUDPclient(DataType.json);
        private void Form1_Load(object sender, EventArgs e)
        {

            p2pc.receiveServerEvent += P2pc_receiveServerEvent;//接收数据事件
            p2pc.timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
            p2pc.start("127.0.0.1", 8989, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
        }
        [InstallFunAttribute("forever")]//客户端也支持像服务端那样写，刚才看懂返回的内容也是testaabb，所以客户端也要把方法命名testaabb
        public void login(System.Net.Sockets.Socket soc, WeaveSession _0x01)
        {
           // MessageBox.Show(_0x01.GetRoot<int>().ToString());
            //  Gw_EventMylog("",_0x01.Getjson());
        }
        private void P2pc_timeoutevent()
        {
            
        }
        private void P2pc_receiveServerEvent(byte command, string text)
        {
            count++;
         //   MessageBox.Show(text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //在加个发送
            
            //这样就可以了，我们试试
        }

        private void button2_Click(object sender, EventArgs e)
        {
             

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(tt));

            t.Start();
            timer1.Start();
        }
         
        void tt()
        {
            for (int i = 0; i < 300; i++)
            {
                P2Pclient p2pc = new P2Pclient(false);
                p2pc.receiveServerEvent += P2pc_receiveServerEvent;//接收数据事件
                
                p2pc.timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
                p2pc.start("127.0.0.1", 8989, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
                p2pc.Tokan = "123";
                p2pc.SendRoot<int>(0x01, "login", 99987, 0);
                System.Threading.Thread.Sleep(5);
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Invoke((EventHandler)delegate { label1.Text = count.ToString(); });
        }
    
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            P2Pclient p2pc = new P2Pclient(false);
            p2pc.receiveServerEvent += P2pc_receiveServerEvent;//接收数据事件

            p2pc.timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
            p2pc.start("127.0.0.1", 8989, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
            p2pc.Tokan = "123";
            p2pc.SendRoot<int>(0x01, "login", 99987, 0);
            System.Threading.Thread.Sleep(5);

         //   p2pc.Tokan = "123";
         //  p2pc.SendRoot<String>(0x01, "login", "999899987999879998799987999879998799987999879999989998799987999879998799987999879998799987999879998799987999879998799987999879998799987999879998799987999879998799987798799987999879999899987999879998799987999879998799987999879999989998799987999879998799987999879998799987999879998799987999879998799987999879998799987999879998799987999879998799987798799987999879998799987999879998799987999879998799987999879998799987999877999899987999879998799987999879998799987999879999989998799987999879998799987999879998799987999879998799987999879998799987999879998799987999879998799987999879998799987798799987999879998799987999879998799987999879998799987999879998799987999877998799987999879998799987999879998799987999879998799987999877", 0);
         ////   p2pc.SendRoot<int>(0x01, "login", 34534534, 0);
         //   System.Threading.Thread.Sleep(5);
        }
        WeaveUDPclient wudp = new WeaveUDPclient();
        private void button3_Click(object sender, EventArgs e)
        {
            wudp.receiveServerEvent += Wudp_receiveServerEvent;
            wudp.start("127.0.0.1", 8989,false);

        }

        private void Wudp_receiveServerEvent(byte command, string text)
        {
            MessageBox.Show(text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            wudp.send(0x11, "不知道说什么好，反正成功了。");
        }
    }
}
