using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows.Forms;
using UserLogin;
using Weave.Base;
using Weave.TCPClient;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        P2Pclient client = new P2Pclient(DataType.json);
        private void button1_Click(object sender, EventArgs e)
        {
            users u = new users();
            u.Name = textBox1.Text;
            u.Pwd = textBox2.Text;
            client.SendRoot<users>(0x01, "login", u, 0);//这样就把这个实体类发送出去了
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client.timeoutevent += Client_timeoutevent;
            client.receiveServerEvent += Client_receiveServerEvent;
            client.AddListenClass(this);//添加方法监听
            client.start("127.0.0.1", 8989, true);
        }
        [InstallFun("forever")]//这是一个接收的方法，注册到了通讯类中
        public void Islogin(Socket soc, WeaveSession _0x01)
        {
            if (_0x01.GetRoot<bool>())
            {
                MessageBox.Show("登录成功");
            }
            else
            {
                MessageBox.Show("登录失败");
            }
        }
        [InstallFun("forever")]//这是一个接收的方法，注册到了通讯类中
        public void getdata(Socket soc, WeaveSession _0x01)
        {
            List<ViewData> listViewData = _0x01.GetRoot<List<ViewData>>();
            listView1.Invoke(new Action(delegate (){
                if (!listView1.InvokeRequired)
                {
                    foreach (ViewData vd in listViewData)
                    {
                        ListViewItem lvi = new ListViewItem(vd.Name);
                        lvi.SubItems.Add(vd.School);
                        lvi.SubItems.Add(vd.Age);
                        listView1.Items.Add(lvi);
                    }
                }

            }));

        }
        private void Client_timeoutevent()
        {
            if (!client.Isline)
            {

                if (client.Restart(true))
                    return;
            }
            System.Threading.Thread.Sleep(1000);

        }

        private void Client_receiveServerEvent(byte command, string text)
        {
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.SendRoot<String>(0x01, "getdata", "", 0);//这样就把这个实体类发送出去了

        }
    }
}
