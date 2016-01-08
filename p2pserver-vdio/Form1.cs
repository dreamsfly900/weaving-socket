using P2P;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPServer;
namespace p2pserver_vdio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        p2psever p2p = new p2psever();
        private void Form1_Load(object sender, EventArgs e)
        {

            p2p.NATthroughevent += P2p_NATthroughevent;

            p2p.receiveevent += P2p_receiveevent;
            p2p.start(16680);
            timer1.Start();
        }

        private void P2p_receiveevent(byte command, string data, System.Net.Sockets.Socket soc)
        {
           
        }

        private void P2p_NATthroughevent(byte command,string data, System.Net.EndPoint ep)
        {
             
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "在线人数:" + p2p.getNATthrough().Length;
            NETcollectionUdp[] nettl = p2p.getNATthrough();
            listBox1.Items.Clear();
            foreach (NETcollectionUdp netudp in nettl)
            {
              
                listBox1.Items.Add(netudp.Iep.ToString());
            }
        }
    }
}
