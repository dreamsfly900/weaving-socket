using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weave.Base;
using Weave.Server;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        WeaveP2Server wp2ser = new WeaveP2Server(WeaveDataTypeEnum.Bytes);
        private void Form1_Load(object sender, EventArgs e)
        {
          
            wp2ser.weaveReceiveBitEvent += Wp2ser_weaveReceiveBitEvent;//接收
            wp2ser.Start(8989);
        }

        private void Wp2ser_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            wp2ser.Send(soc, 0x01,new byte[]{ 0x11});
        }
    }
}
