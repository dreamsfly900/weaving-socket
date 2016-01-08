using client;
using MyInterface;
using StandardModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍 
namespace WPTestApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        P2Pclient tcp = new P2Pclient(false);

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tcp.timeoutevent += Tcp_timeoutevent;
            tcp.start("127.0.0.1", 8989, true);
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
        }

        private void Tcp_timeoutevent()
        {
            
        }
        [InstallFun("forever")]//forever
        public void Send_content(object soc, _baseModel _0x01)
        {
            Gw_EventMylog("", _0x01.Getjson());
        }
        private void Gw_EventMylog(string type, string log)
        {

            textBox.Text = log;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            while (i < 10)
            {
                i++;
                //_baseModel bm = new _baseModel();
                //bm.Request = "Send_content";//请求的方法名
                //bm.Token = p2pc.Tokan;//服务器登录成功返回的token
                ////c中存放的是传递给服务器的内容
                String str = "老大，老二";
                //bm.SetParameter<Ccontext>(c);
                //向服务器发送数据
                // p2pc.send((byte)0x01, bm.Getjson());
                tcp.SendParameter<String>(0x01, "Send_content", str, 0);
                tcp.SendRoot<String>(0x01, "Send_content", str, 0);
            }
        }
    }
}
