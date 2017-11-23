using MyTcpCommandLibrary.CommandUseTempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WeaveBase;

namespace MyTcpCommandLibrary
{
 

    public class LoginManageCommand  : WeaveTCPCommand
    {
        public delegate void ServerLoginOK(string _u,Socket _s);

        public event ServerLoginOK ServerLoginOKEvent;

        public override byte Getcommand()
        {

            //此CLASS的实例，代表的指令，指令从0-254，0x9c与0xff为内部指令不能使用。
            //0x01的意思是，只要是0x01的指令，都会进入本实例进行处理
            //return 0x01;
            return (byte)CommandEnum.ClientSendLoginModel; //0x02;
        }

        public override bool Run(string data, Socket soc)
        {
           
            //此事件是接收事件，data 是String类型的数据，soc是发送人。
            return true;
        }

        public override void WeaveBaseErrorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
            //错误异常事件，message为错误信息，soc为产生异常的连接
        }

        public override void WeaveDeleteSocketEvent(Socket soc)
        {
            //此事件是当有人中断了连接，此事件会被调用
        }

        public override void WeaveUpdateSocketEvent(Socket soc)
        {
            //此事件是当有人新加入了连接，此事件会被调用
        }

        [InstallFun("forever")]
        public void CheckLogin(Socket soc, WeaveSession wsession)
        {

            // string jsonstr = _0x01.Getjson();
            LoginTempModel get_client_Send_loginModel = wsession.GetRoot<LoginTempModel>();



            //执行查找数据的操作......
            bool loginOk = false;


            AddSystemData();

            loginOk = CheckUserCanLoginIn(get_client_Send_loginModel);
            if (loginOk)
            {
                // UpdatePlayerListSetOnLine  
                ServerLoginOKEvent(get_client_Send_loginModel.userName, soc);


            }
            SendRoot<bool>(soc, (byte)CommandEnum.ServerSendLoginResult, "ServerBackLoginResult", loginOk , 0, wsession.Token);
            //发送人数给客户端
            //参数1，发送给客户端对象，参数2，发送给客户端对应的方法，参数3，人数的实例，参数4，此处无作用，参数5，客户端此次token
        }


        private void AddSystemData()
        {
            GameDataAccess.BLL.UserTableBLL myBLL = new GameDataAccess.BLL.UserTableBLL();

            if(  myBLL.CheckDataBaseIsNull())
            {
                myBLL.AddTestData();
            }
        }

        private bool CheckUserCanLoginIn(LoginTempModel m)
        {
            GameDataAccess.BLL.UserTableBLL myBLL = new GameDataAccess.BLL.UserTableBLL();
            return myBLL.CheckUserNamePassword(m.userName, m.password);
        }



    }
}
