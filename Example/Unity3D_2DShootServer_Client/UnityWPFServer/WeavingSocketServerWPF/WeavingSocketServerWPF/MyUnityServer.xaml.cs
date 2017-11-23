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
using System.Windows.Shapes;
using WeaveBase;
using System.Net.Sockets;
using MyTCPCloud;
using System.Windows.Threading;

namespace WeavingSocketServerWPF
{
    /// <summary>
    /// MyUnityServer.xaml 的交互逻辑
    /// </summary>
    public partial class MyUnityServer : Window
    {
        public MyUnityServer()
        {
            InitializeComponent();

           // DispatcherFunction();
        }
        /// <summary>
        /// 监听端口列表，，可以选择监听多个端口
        /// </summary>
        WeaveServerPort wserverport = new WeaveServerPort();
        WeaveTCPcloud weaveTCPcloud = new WeaveTCPcloud();

        List<MyListBoxItem> loginedUserList = new List<MyListBoxItem>();

        List<MyListBoxItem> connectedSocketItemList = new List<MyListBoxItem>();
        // DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private void StartListen_button_Click(object sender, RoutedEventArgs e)
        {
            //设置登陆后的用户列表Listbox的数据源
            LoginedUser_listBox.ItemsSource = loginedUserList;
            //设置连接到服务器的Socket列表的Listbox的数据源
            ConnectedSocket_listBox.ItemsSource = connectedSocketItemList;

            WevaeSocketSession mif = new WevaeSocketSession();
           
            weaveTCPcloud.Run(mif);


            wserverport.IsToken = true;
            wserverport.Port = Convert.ToInt32(Port_textBox.Text);
            wserverport.PortType = WeavePortTypeEnum.Json;
         
            weaveTCPcloud.StartServer(wserverport);


            weaveTCPcloud.WeaveReceiveEvent += OnWeaveReceiveMessage;

            weaveTCPcloud.WeaveDeleteEvent += OnWeaveDeleteSocket;

            weaveTCPcloud.WeaveUpdateEvent += OnWeaveUpdateSocket;

            weaveTCPcloud.WeaveServerUpdateUnityPlayerSetOnLineEvent += OnWeaveServerUpdateUnityPlayerSetOnLineEvent;

            StartListen_button.Content = "正在监听";

            StartListen_button.IsEnabled = false;
            
        }

        private void OnWeaveServerUpdateUnityPlayerSetOnLineEvent(UnityPlayerOnClient gamer)
        {

            
            //throw new NotImplementedException();
            //当有用户 账号密码登陆成功的时候
            AddListBoxItemAction(loginedUserList, CopyUnityPlayerOnClient(gamer));
            SetServerReceiveText("--触发了一次（OnWeaveServerUpdateUnityPlayerSetOnLineEvent）" + Environment.NewLine);

        }

        private void OnWeaveUpdateSocket(UnityPlayerOnClient gamer)
        {
            SetServerReceiveText("--触发了一次（OnWeaveUpdateSocket）" + Environment.NewLine);

            //有 Sokcet客户端连接到服务器的时候，暂未 账号，密码认证状态

            AddListBoxItemAction(connectedSocketItemList, CopyUnityPlayerOnClient(gamer) );
           
        }

       

        private void OnWeaveDeleteSocket(UnityPlayerOnClient gamer)
        {
            SetServerReceiveText("--退出事件，，触发了一次（OnWeaveDeleteSocket）" + Environment.NewLine);

            RemoveListBoxItemAction(connectedSocketItemList, CopyUnityPlayerOnClient(gamer));

            RemoveListBoxItemAction(loginedUserList, CopyUnityPlayerOnClient(gamer));


        }

        private void OnWeaveReceiveMessage(byte command, string data, UnityPlayerOnClient  gamer)
        {
           

            WeaveSession ws = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(data);

            SetServerReceiveText("接收到新信息：  "  + ws.Root  + Environment.NewLine );

         
        }

        private void StopListen_button_Click(object sender, RoutedEventArgs e)
        {

            weaveTCPcloud.P2Server = null;

            weaveTCPcloud = null;

            Application.Current.Shutdown();
            Environment.Exit(0);// 可以立即中断程序执行并退出
        }

        private void SendMsg_button_Click(object sender, RoutedEventArgs e)
        {
           
            string serverMsg = InputSendMessage_textBox.Text;
            int unityGamecount = weaveTCPcloud.unityPlayerOnClientList.Count;
            if (string.IsNullOrEmpty(serverMsg) || weaveTCPcloud.weaveOnline.Count==0)
                return;

            WeaveOnLine[] _allWeaveOnLine = new WeaveOnLine[weaveTCPcloud.weaveOnline.Count];

            weaveTCPcloud.weaveOnline.CopyTo(_allWeaveOnLine);

            foreach (WeaveOnLine oneWeaveOnLine in _allWeaveOnLine)
            {
                weaveTCPcloud.P2Server.Send(oneWeaveOnLine.Socket, 0x01, "服务器主动给所有客户端发消息了: " + serverMsg);
            }

           // MessageBox.Show("客户端在线数量："+ _allWeaveOnLine.Length);
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

        public MyListBoxItem CopyUnityPlayerOnClient(UnityPlayerOnClient one)
        {
            MyListBoxItem item = new MyListBoxItem()
            {
                UIName_Id = one.Socket.RemoteEndPoint.ToString(),
                ShowMsg = "UserIP:" + one.Socket.RemoteEndPoint.ToString() + " -Token:" + one.Token,
                UserName = one.UserName,
                Ip = one.Socket.RemoteEndPoint.ToString()
            };
            return item;
        }

        public void AddListBoxItem(List<MyListBoxItem> sList  , MyListBoxItem one)
        {
          

            sList.Add(one);
            CheckListBoxSource();
        }

        public void AddListBoxItemAction(List<MyListBoxItem> sList, MyListBoxItem one)
        {
            Action< List < MyListBoxItem >  , MyListBoxItem>  addListBoxItemAction =
            
                new Action<List<MyListBoxItem>  , MyListBoxItem>(AddListBoxItem);

            this.Dispatcher.BeginInvoke(addListBoxItemAction,sList , one);
        }


        public void RemoveListBoxItem(List<MyListBoxItem> sList, MyListBoxItem one)
        {
            MyListBoxItem item = sList.Find(i=>i.Ip == one.Ip);

            if(item != null)
            {
                sList.Remove(item);
            }


            CheckListBoxSource();
        }

        public void RemoveListBoxItemAction(List<MyListBoxItem> sList, MyListBoxItem one)
        {
            Action<List<MyListBoxItem>, MyListBoxItem> removeListBoxItemAction =

                new Action<List<MyListBoxItem>, MyListBoxItem>(RemoveListBoxItem);

            this.Dispatcher.BeginInvoke(removeListBoxItemAction, sList , one);
        }

        public void CheckListBoxSource()
        {
            //数据发生变化后，重新设置登陆后的用户列表Listbox的数据源
            LoginedUser_listBox.ItemsSource = null;
            LoginedUser_listBox.ItemsSource = loginedUserList;
            //数据发生变化后，重新设置连接到服务器的Socket列表的Listbox的数据源
            ConnectedSocket_listBox.ItemsSource = null;
           ConnectedSocket_listBox.ItemsSource = connectedSocketItemList;
        }

        protected override void OnClosed(EventArgs e)
        {
            weaveTCPcloud.P2Server = null;

            weaveTCPcloud = null;

            //Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //if (this.IsAfreshLogin == true) return;
            Application.Current.Shutdown();
            Environment.Exit(0);// 可以立即中断程序执行并退出
            base.OnClosed(e);
        }


    }

    public class MyListBoxItem
    {
        public string UIName_Id { get; set; }

        public string Ip { get; set; }
        public string ShowMsg { get; set; }

        public string UserName { get; set; }
    }
}
