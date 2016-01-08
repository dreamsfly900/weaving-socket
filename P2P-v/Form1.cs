using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPclient;

namespace P2P_v
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        UDP udp = new UDP();
        private void button1_Click(object sender, EventArgs e)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(textBox2.Text.Split(':')[0]), Convert.ToInt32(textBox2.Text.Split(':')[1]));
            udp.send(0x92, IPAddress.Any.ToString() + ":" + 9988, localEndPoint);
        }

        private void Udp_receiveevent(byte command, string data, EndPoint iep)
        {
            textBox1.Text = data;
        }
        public   string Domain2Ip(string str)
        {
            string _return = "";
            try
            {
                IPHostEntry hostinfo = Dns.GetHostEntry(str);
                IPAddress[] aryIP = hostinfo.AddressList;
                _return = aryIP[0].ToString();

            }
            catch (Exception e)
            {
                _return = e.Message;
            }
            return _return;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            string str= Domain2Ip("haiyang0201.imwork.net");
            String IP = "122.114.56.226";
            int PORT = 9987;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IP), 16680);
            udp.receiveevent += Udp_receiveevent;
            udp.start(IP, PORT, 16680);
            udp.send(0x9c, IPAddress.Any.ToString() + ":" + PORT, localEndPoint);
        }
    }
}
