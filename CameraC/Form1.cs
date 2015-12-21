using client;
using DHPlaySDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CameraC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        int tongdao = 0;
        int tongdao2 = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            TCPliuclient tcpl = new TCPliuclient("125.46.77.146", 1314, 0, panel1.Handle);
            TCPliuclient tcpl2 = new TCPliuclient("125.46.77.146", 1315, 1, panel2.Handle);
            //TCPliuclient tcpl3 = new TCPliuclient("125.46.77.146", 1316, 2, panel3.Handle);
            //TCPliuclient tcpl4 = new TCPliuclient("125.46.77.146", 1317, 3, panel4.Handle);
        }
         
    }
}
