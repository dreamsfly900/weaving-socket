using DHDVR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
      
        List<CameraData> listcamera = new List<CameraData>();
        private void Form1_Load(object sender, EventArgs e)
        {
           
            System.IO.StreamReader sr = new System.IO.StreamReader("config.txt",System.Text.Encoding.GetEncoding("gb2312"));
            
            while (!sr.EndOfStream)
            {

                //Panel p = new Panel();
                RadioButton p = new RadioButton();
                
                Padding p1 = new Padding();
                p1.Left = 10;
                p1.Top = 10;
                p.Margin = p1;
                flowLayoutPanel1.Controls.Add(p);
                String str=sr.ReadLine();
              //  DHCamera dhc1 = new DHCamera();
                CameraData cd = new CameraData();
                p.Text = str.Split('|')[0];
                cd.Handle = p.Handle;
                cd.IP = str.Split('|')[0];
                cd.Port =Convert.ToInt32( str.Split('|')[1]) ;
                cd.UserName = str.Split('|')[2];
                cd.Pwd = str.Split('|')[3];
                cd.Code= str.Split('|')[4];
                cd.ImagesPath= str.Split('|')[5];
                //dhc1.Init(cd);
                listcamera.Add(cd);
            }
            sr.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (CameraData cd in listcamera)
            {
                Process.Start(cd.ImagesPath+"Cpanel.exe", cd.IP+"|"+ cd.Port+"|"+ cd.UserName+"|"+ cd.Pwd+"|"+cd.Code);
            }
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if((c as RadioButton).Checked)
                    foreach (CameraData cd in listcamera)
                    {
                        if(cd.IP==c.Text)
                        Process.Start(cd.ImagesPath+"Cpanel.exe", cd.IP + "|" + cd.Port + "|" + cd.UserName + "|" + cd.Pwd + "|" + cd.Code);
                    }
            }
        }
    }
}
