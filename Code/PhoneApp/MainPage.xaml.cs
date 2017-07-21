using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp.Resources;
using client;
using MyInterface;
using StandardModel;
using System.Net.Sockets;

namespace PhoneApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
            tcp.timeoutevent += Tcp_timeoutevent;
            tcp.AddListenClass(this);
            tcp.start("172.27.35.1", 11001, true);
        }

        private void Tcp_timeoutevent()
        {
           
        }

        P2Pclient tcp = new P2Pclient(false);
        [InstallFun("forever")]//forever
        public void Send_content(Socket soc, _baseModel _0x01)
        {
            Gw_EventMylog("", _0x01.Getjson());
        }
        private void Gw_EventMylog(string type, string log)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //to do
                    textBlock.Text = log;
                }
              );

              
            }
            catch (Exception ex)
            { }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
             
                String str = "老大，老二";
                //bm.SetParameter<Ccontext>(c);
                //向服务器发送数据
                // p2pc.send((byte)0x01, bm.Getjson());
                tcp.SendRoot<String>(0x01, "Send_content", str, 0);
               
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}