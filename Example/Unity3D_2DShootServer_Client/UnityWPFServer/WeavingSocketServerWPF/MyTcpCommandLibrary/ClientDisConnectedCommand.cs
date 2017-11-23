using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WeaveBase;

namespace MyTcpCommandLibrary
{
    public class ClientDisConnectedCommand : WeaveTCPCommand
    {



        public override byte Getcommand()
        {

            //客户端发来 我要 关闭了 程序命令
          
            return (byte)CommandEnum.ClientSendDisConnected; 
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
        public void OneClientDisConnected(Socket soc, WeaveSession wsession)
        {

            // string jsonstr = _0x01.Getjson();
            // LoginModel login = wsession.GetRoot<LoginModel>();

           
           
        }


    }
}
