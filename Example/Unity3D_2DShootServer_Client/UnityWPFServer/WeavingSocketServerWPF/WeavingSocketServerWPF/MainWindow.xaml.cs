using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeaveBase;
using System.Net.Sockets;
using WeaveSocketServer;

namespace WeavingSocketServerWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        WeaveP2Server   wudp;
        public MainWindow()
        {
            InitializeComponent();

            StopListen_button.IsEnabled = false;
        }

        public List<Socket> AllConnectedClient = new List<Socket>();


        private void StartListen_button_Click(object sender, RoutedEventArgs e)
        {
            int port =   int.Parse( Port_textBox.Text );

            wudp = new WeaveP2Server();
            wudp.waveReceiveEvent += Wudp_waveReceiveEvent;
            wudp.weaveUpdateSocketListEvent += Wudp_weaveUpdateSocketListEvent;
            wudp.weaveDeleteSocketListEvent += Wudp_weaveDeleteSocketListEvent;

            
            wudp.Start(port);


            StartListen_button.IsEnabled = false;

            StopListen_button.IsEnabled = true;
        }

        private void StopListen_button_Click(object sender, RoutedEventArgs e)
        {

            wudp = null;
            

            StopListen_button.IsEnabled = false;

            StartListen_button.IsEnabled = true;

        }
        /// <summary>
        /// 服务器发送信息给 所有客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMsg_button_Click(object sender, RoutedEventArgs e)
        {
            string serverMsg = InputSendMessage_textBox.Text;
            if (string.IsNullOrEmpty(serverMsg))
                return;

            Socket[] _allSocketArray = new Socket[AllConnectedClient.Count];
            
            AllConnectedClient.CopyTo(_allSocketArray);

            foreach (Socket oneclient  in _allSocketArray)
            {
                 wudp.Send(oneclient, 0x01, "服务器主动给所有客户端发消息了: "+ serverMsg );
            }
           
        }


        private  void Wudp_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
          
            SetServerReceiveText("有人退出了");
            AllConnectedClient.Remove(soc);
        }

        private  void Wudp_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
           
            SetServerReceiveText("有人加入了");
            AllConnectedClient.Add(soc);
        }

        private  void Wudp_waveReceiveEvent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            wudp.Send(soc, 0x01, "现在我知道你发消息了");
            // Console.WriteLine("指令:" + command + ".内容:" + data);
            string all_src = "指令:" + command + ".内容:" + data;
            SetServerReceiveText(all_src);
        }


        private void UpdateServerReceiveTb(TextBlock tb, string text)
        {
            tb.Text += text;
        }

        private void SetServerReceiveText(string newtext)
        {
            Action<TextBlock, String> updateAction = new Action<TextBlock, string>(UpdateServerReceiveTb);
            ServerReceive_textBlock.Dispatcher.BeginInvoke(updateAction, ServerReceive_textBlock, newtext);
            
        }


        protected override void OnClosed(EventArgs e)
        {
            wudp = null;

           
            //Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //if (this.IsAfreshLogin == true) return;
            Application.Current.Shutdown();
            Environment.Exit(0);// 可以立即中断程序执行并退出
            base.OnClosed(e);
        }



    }
}
