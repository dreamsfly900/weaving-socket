using client;
using WeaveBase;
using System;
using System.Windows.Forms;
namespace test2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int count = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
          
          
        }
        [InstallFunAttribute("forever")]//客户端也支持像服务端那样写，刚才看懂返回的内容也是testaabb，所以客户端也要把方法命名testaabb
        public void login(System.Net.Sockets.Socket soc, WeaveBase.WeaveSession _0x01)
        {
           // MessageBox.Show(_0x01.GetRoot<int>().ToString());
            //  Gw_EventMylog("",_0x01.Getjson());
        }
        private void P2pc_timeoutevent()
        {
            
        }
        private void P2pc_receiveServerEvent(byte command, string text)
        {

            label1.Invoke((EventHandler)delegate { count++; });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //在加个发送
            
            //这样就可以了，我们试试
        }

        private void button2_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < 50000; i++)
            {
                P2Pclient p2pc = new P2Pclient(false);
                p2pc.receiveServerEvent += P2pc_receiveServerEvent;//接收数据事件
                p2pc.timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
                p2pc.start("127.0.0.1", 11002, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
                p2pc.Tokan = "123";
                p2pc.SendRoot<int>(0x01, "login", 99987, 0);
            }
            

        }
    }
}
