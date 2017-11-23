using client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WMQClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            wmqcl2.receivetopicEvent += Wmqcl2_receivetopicEvent;
            wmqcl2.Reg("0607080910");
            wmqcl4.receiveQueueEvent += Wmqcl4_receiveQueueEvent;
            wmqcl4.Reg("123456789000");
            wmqcl3.Reg("是我发言的");

        }

        private void Wmqcl4_receiveQueueEvent(WMQ.WMQData text)
        {
            listBox2.Invoke(new EventHandler(delegate { listBox2.Items.Add("来自:" + text.form + ";消息：" + text.message); }));
            count2++;

            label7.Invoke(new EventHandler(delegate { label7.Text = (count2).ToString(); }));
        }

        int count = 0,count2=0;
        private void Wmqcl2_receivetopicEvent(WMQ.WMQData text)
        {
            listBox1.Invoke(new EventHandler(delegate { listBox1.Items.Add ("topic:"+ text.to +";消息："+text.message); }));
            count++;

            label4.Invoke(new EventHandler(delegate { label4.Text = (count).ToString(); }));
        }

        WMQclient wmqcl = new WMQclient("127.0.0.1", 8989, WMQType.topic);
        WMQclient wmqcl2 = new WMQclient("127.0.0.1", 8989, WMQType.topic);
        private void button1_Click(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(send));
        }
        void send(object obj)
        {

            for (int i = 0; i < 500; i++)
            {
                wmqcl.Send<String>("0607080910", "我也不知道为什么", 500);
                label2.Invoke(new EventHandler(delegate { label2.Text = (i + 1).ToString(); }));
                System.Threading.Thread.Sleep(10);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(sendQueue));

        }
        WMQclient wmqcl3 = new WMQclient("127.0.0.1", 8989, WMQType.Queue);
        WMQclient wmqcl4 = new WMQclient("127.0.0.1", 8989, WMQType.Queue);
        void sendQueue(object obj)
        {

            for (int i = 0; i < 500; i++)
            {
                wmqcl3.Send<String>("123456789000", "什么是Queue", 500);
                label6.Invoke(new EventHandler(delegate { label6.Text = (i + 1).ToString(); }));
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
