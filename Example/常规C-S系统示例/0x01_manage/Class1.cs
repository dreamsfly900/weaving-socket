using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using StandardModel;
using MyInterface;
using UserLogin;

namespace _0x01_manage
{
    public class Class1 : MyInterface.TCPCommand
    {
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
            //这里是错误日志事件
        }


        [InstallFun("forever")]//这是一个接收的方法，注册到了通讯类中
        public void login(Socket soc, _baseModel _0x01)
        {
            //在这里你可以用
            users u = _0x01.GetRoot<users>();
            //去验证登录是否正确、
            //并且用_0x01.Token当成SESSION进行验证。
            SendRoot<bool>(soc, 0x01, "Islogin",Convert.ToBoolean(new Random().Next(0, 1)), 0, _0x01.Token);

        }
        [InstallFun("forever")]//这是一个接收的方法，注册到了通讯类中
        public void getdata(Socket soc, _baseModel _0x01)
        {
            //这里可以验证_0x01.Token是否是登录过的。

            List<ViewData> listViewData = new List<ViewData>();
            ViewData vdata = new ViewData();
            vdata.Name = "张三";
            vdata.School = "小学";
            vdata.Age = "18";
            listViewData.Add(vdata);
            vdata = new ViewData();
            vdata.Name = "李四";
            vdata.School = "中学";
            vdata.Age = "28";
            listViewData.Add(vdata);
            SendRoot<List<ViewData>>(soc, 0x01, "getdata", listViewData, 0, _0x01.Token);
        }
        
        public override byte Getcommand()
        {
            return 0x01;
        }




        public override bool Run(string data, Socket soc)
        {
            return true;
        }

        public override void TCPCommand_EventDeleteConnSoc(Socket soc)
        {
             
        }

        public override void TCPCommand_EventUpdataConnSoc(Socket soc)
        {
            
        }
    }
}
