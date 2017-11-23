using SocketServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using WeaveBase;

namespace WMQ
{
  
    public partial class MDIParent1 : Form
    { 
        WMQ WMQMANAGE;
        public MDIParent1()
        {
            InitializeComponent();
            WMQMANAGE = new WMQ(listiwtcp);
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
         
        }
        List<WMQMODE> listiwtcp = new List<WMQMODE>();
        List<minForm> listminForm = new List<minForm>();

        private void OpenFile(object sender, EventArgs e)
        {
           
            System.IO.StreamReader sr = new System.IO.StreamReader("port.txt");
            while (!sr.EndOfStream)
            {
                String str = sr.ReadLine();
                minForm childForm = new minForm();
                childForm.Name = str;
                childForm.MdiParent = this;
                childForm.Text = "端口：" + str.Split(':')[0]+"--类型："+ str.Split(':')[1];
                childForm.Show();
                IWeaveTcpBase wps=null;
                if (str.Split(':')[1]=="socket")
                 wps = new WeaveP2Server();
                if (str.Split(':')[1] == "websocket")
                    wps = new WeaveWebServer();
                if (str.Split(':')[1] == "http")
                    wps = new HttpServer(Convert.ToInt32(str.Split(':')[0]));

                wps.waveReceiveEvent += Wps_waveReceiveEvent;
                wps.weaveDeleteSocketListEvent += Wps_weaveDeleteSocketListEvent;
                wps.weaveUpdateSocketListEvent += Wps_weaveUpdateSocketListEvent;
                wps.Start(Convert.ToInt32(str.Split(':')[0]));
                //  wps.GetNetworkItemCount();
                WMQMODE wm = new WMQMODE();
                wm.iwtb = wps;
                wm.mf = childForm;
                listiwtcp.Add(wm);
                listminForm.Add(childForm);
                childForm.listBox1.Items.Add("监听已启动。。。。");
               
            }
            sr.Close();
            timer1.Start();
            openToolStripMenuItem.Enabled = false;
            LayoutMdi(MdiLayout.TileHorizontal);
        }
        int count = 0;
        private void Wps_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            foreach (WMQMODE wm in listiwtcp)
            {
                if (wm.iwtb.Port == ((System.Net.IPEndPoint)soc.LocalEndPoint).Port)
                {
                    wm.count++;
                    return;
                }
            }
            
        }

        private void Wps_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            foreach (WMQMODE wm in listiwtcp)
            {
                if (wm.iwtb.Port == ((System.Net.IPEndPoint)soc.LocalEndPoint).Port)
                {
                    wm.count--;
                    return;
                }
            }
            WMQMANAGE.deletesoc(soc);


        }
    
        private void Wps_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            // WMQData wmqd = Newtonsoft.Json.JsonConvert.DeserializeObject<WMQData>(data);
            WMQMANAGE.EXEC( command,  data,  soc);

        }
      
        private void MDIParent1_Load(object sender, EventArgs e)
        {

        }

        private void 退出xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 配置端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            config cf = new config();
                cf.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "连接总数："+ count;
            foreach (WMQMODE wm in listiwtcp)
            {
                wm.mf.listBox1.Items.Clear();
                wm.mf.listBox1.Items.Add("当前链接数:"+ wm.count);
            }
        }
    }
}
