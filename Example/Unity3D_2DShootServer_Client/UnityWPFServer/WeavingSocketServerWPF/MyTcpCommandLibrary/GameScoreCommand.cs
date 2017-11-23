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
    public class GameScoreCommand : WeaveTCPCommand
    {
        public override byte Getcommand()
        {

            //此CLASS的实例，代表的指令，指令从0-254，0x9c与0xff为内部指令不能使用。
            //0x02，0x02，都会进入本实例进行处理
            return (byte)CommandEnum.ClientSendGameScoreModel; //0x02;
        }



        public override bool Run(string data, Socket soc)
        {

           // base.SendRoot<string>(soc, 0x02, "checkpos","testetsetesteste",0,null);
            //此事件是接收事件，data 是String类型的数据，soc是发送人。
            return true;
        }

        public override void WeaveBaseErrorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
            //throw new NotImplementedException();
        }

        public override void WeaveDeleteSocketEvent(Socket soc)
        {
            //throw new NotImplementedException();
        }

        public override void WeaveUpdateSocketEvent(Socket soc)
        {
           // throw new NotImplementedException();
        }

        [InstallFun("forever")]
        public void UpdateScore(Socket soc, WeaveSession wsession)
        {
             GameScoreTempModel gsModel = wsession.GetRoot<GameScoreTempModel>();
          

            //执行数据更新的操作......
            bool updateSocreResult = UpdateUserScore(gsModel.userName, gsModel.score , gsModel.missenemy);
            
            if (updateSocreResult)
            {
                // 向客户端发送 更新积分成功的信息
                SendRoot<bool>(soc, (byte)CommandEnum.ServerSendUpdateGameScoreResult, "ServerSendUpdateGameScoreResult", updateSocreResult, 0, wsession.Token);

            }
            else
            {
                // 向客户端发送 更新积分失败的信息
                SendRoot<bool>(soc, (byte)CommandEnum.ServerSendUpdateGameScoreResult, "ServerSendUpdateGameScoreResult", updateSocreResult, 0, wsession.Token);
                //发送人数给客户端
                //参数1，发送给客户端对象，参数2，发送给客户端对应的方法，参数3，人数的实例，参数4，此处无作用，参数5，客户端此次token
            }
        }

        [InstallFun("forever")]
        public void GetUserScore(Socket soc, WeaveSession wsession)
        {
            MyTcpCommandLibrary.CommandUseTempModel.LoginTempModel loginModel = wsession.GetRoot<LoginTempModel>();


            //执行查找数据的操作......
            MyTcpCommandLibrary.CommandUseTempModel.GameScoreTempModel gsModel = GetUserGameScore(loginModel.userName);

            if (gsModel!=null)
            {
                // 向客户端发送 查找积分成功的信息
                SendRoot<GameScoreTempModel>(soc, (byte)CommandEnum.ServerSendGetGameScoreResult, "ServerSendGameScore", gsModel, 0, wsession.Token);

            }
            else
            {
                // 向客户端发送 查找积分失败的信息
                //SendRoot<bool>(soc, (byte)CommandEnum.ServerSendUpdateGameScoreResult, "ServerSendUpdateGameScoreResult", updateSocreResult, 0, wsession.Token);
                //发送人数给客户端
                //参数1，发送给客户端对象，参数2，发送给客户端对应的方法，参数3，人数的实例，参数4，此处无作用，参数5，客户端此次token
            }
        }



        private bool UpdateUserScore(string _userName , int _score , int _missed)
        {
            GameDataAccess.BLL.GameScoreTabelBLL gsBLL = new GameDataAccess.BLL.GameScoreTabelBLL();
            return gsBLL.UpdateGameScore(_userName, _score , _missed);
        }

        private GameScoreTempModel GetUserGameScore(string _userName)
        {
            GameDataAccess.BLL.GameScoreTabelBLL gsBLL = new GameDataAccess.BLL.GameScoreTabelBLL();
            Model.GameScoreTabel gsModel = gsBLL.GetGameScore(_userName);
            GameScoreTempModel scoreModel = new GameScoreTempModel()
            {
                userName = _userName,
                missenemy = gsModel.missenemy,
                score = gsModel.score
            };

            return scoreModel;
        }

    }
}
