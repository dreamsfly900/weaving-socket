using DHDVR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cpanel
{
    public partial class Form1 : Form
    {
        CameraData cd = new CameraData();
        public Form1(CameraData cc)
        {
            cd = cc;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = cd.IP;
            //cd.IP = "192.168.1.100";
            //cd.Port = 37777;
            //cd.UserName = "admin";
            //cd.Pwd = "admin";
            //cd.Code = "1314";
            DHCamera dhc1 = new DHCamera();
            cd.Handle = panel1.Handle;
            dhc1.Init(cd);
           
        }
    }
}
