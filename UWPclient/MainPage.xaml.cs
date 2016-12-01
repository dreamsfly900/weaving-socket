using client;
using MyInterface;
using StandardModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace UWPclient
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        P2Pclient tcp = new P2Pclient(false);
        [InstallFun("forever")]//forever
        public void Send_content(Socket soc, _baseModel _0x01)
        {
            // Gw_EventMylog("", _0x01.Getjson());
            try
            {

                 Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                  {
                    //to do
                    textBlock.Text += _0x01.Root;
                  }
              );


            }
            catch (Exception ex)
            { }
        }
        public MainPage()
        {
            this.InitializeComponent();
            tcp.timeoutevent += Tcp_timeoutevent;
            tcp.AddListenClass(this);
            tcp.start("127.0.0.1", 11001, true);
        }

        private void Tcp_timeoutevent()
        {
             
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
    }
}
