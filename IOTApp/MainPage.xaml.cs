using client;
using MyInterface;
using StandardModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace IOTApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
        P2Pclient p2pc = new P2Pclient(false);
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            p2pc.AddListenClass(this);
            p2pc.timeoutevent += P2pc_timeoutevent;
            p2pc.jumpServerEvent += P2pc_jumpServerEvent;
            p2pc.start("122.114.53.233", 11002, true);

            Timer t = new Timer(new TimerCallback(setvalue), null, 0, 1000);
            
        }
        bool gg = true;
        void setvalue(object obj)
        {
           if(gg)
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            { 
                textBlock3.Text = "当前随机数值显示：" + new Random().Next(0, 100) + " / n/r";
            });
        }
        [InstallFun("forever")]//forever
        public void command(Socket soc, _baseModel _0x01)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //to do
                textBlock2.Text += "收到指令--" + _0x01.Root+"/n/r";
            });
            if (_0x01.Root == "开始")
            {
                gg = true;

            }
            else
            {
                gg = false;
            }
        }
        private void P2pc_jumpServerEvent(string text)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
           {
                 //to do
                 textBlock2.Text += "服务器已满：推荐你连接其他服务器--" + text;
           });
             

        }

        private void P2pc_timeoutevent()
        {
            if (!p2pc.Isline)
                p2pc.Restart(true);
        }
    }
}
