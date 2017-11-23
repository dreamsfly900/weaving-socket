using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WMQ
{
    public partial class config : Form
    {
        public config()
        {
            InitializeComponent();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void config_Load(object sender, EventArgs e)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader("port.txt");
            while (!sr.EndOfStream)
            {
                listBox1.Items.Add(sr.ReadLine());
            }
            sr.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox1.Text+":"+ comboBox1.Text);
        }

        private void config_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.IO.StreamWriter sr = new System.IO.StreamWriter("port.txt");
            foreach (String str in listBox1.Items)
            {
                sr.WriteLine(str);
            }
            sr.Close();
        }
    }
}
