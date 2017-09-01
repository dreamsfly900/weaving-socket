using client;
using System;
using System.Windows.Forms;
using WeaveBase;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {

            WeaveP2Server wp2p = new WeaveP2Server();
            wp2p.waveReceiveEvent += Wp2p_waveReceiveEvent;
            wp2p.Start(8989);

        }

        private void Wp2p_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            MessageBox.Show(data);
        }

        private void P2p_timeoutevent()
        {
            
        }

        private void P2p_receiveServerEvent(byte command, string text)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            P2Pclient p2p = new P2Pclient(false);
            p2p.receiveServerEvent += P2p_receiveServerEvent;
            p2p.timeoutevent += P2p_timeoutevent;
            bool b = p2p.start("127.0.0.1", 8989, false);
            MessageBox.Show(b.ToString() + ":" + p2p.Tokan);
            p2p.send(0x01, "你好啊！我很好的。");
        }
    }
}
