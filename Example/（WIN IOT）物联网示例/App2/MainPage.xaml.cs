using client;
using System;
using System.Net.Sockets;
using WeaveBase;
using WeaveSokect;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace App2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dt = new DispatcherTimer();
        public MainPage()
        {

            this.InitializeComponent();
            p2pc.AddListenClass(this);
            p2pc.timeoutevent += P2pc_timeoutevent;
            p2pc.jumpServerEvent += P2pc_jumpServerEvent;
            if (p2pc.start("122.114.53.233", 11002, true))
            {

                p2pc.SendRoot<String>(0x03, "login","", 0);
                dt.Interval = new TimeSpan(0, 0, 2);
                dt.Tick += Dt_Tick;
                dt.Start();
            }
        }

        private void Dt_Tick(object sender, object e)
        {
            try
            {
                if (gg)
                {
                    int a = new Random().Next(0, 160);
                    p2pc.SendRoot<int>(0x03, "setvalue", a, 0);
                    textBlock3.Text = "当前随机数值显示：" + a + " ";

                }
            }
            catch (Exception ex){ }
        }

        P2Pclient p2pc = new P2Pclient(false);

        bool gg = true;
       
        [InstallFun("forever")]//forever
        public void command(Socket soc, BaseDataModel _0x01)
        {
            try
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    //to do
                    textBlock2.Text = "收到指令--" + _0x01.Root + "";
                });
                if (_0x01.GetRoot<String>() == "\"开始\"")
                {
                    
                    gg = true;

                }
                else
                {
                   
                    gg = false;
                }
            }
            catch (Exception ex){ }
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
