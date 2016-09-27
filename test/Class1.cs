using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using StandardModel;
using MyInterface;

namespace test
{
    class DTUDATA
    {
        //多加一个说明的参数把，更明显
        public String content { set; get; }
        int data=0;

        public int Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }
    }
    class Datauser
    {
      public  Socket soc { set; get; }
        public String token { set; get; }
    }
    public class Class1 : MyInterface.TCPCommand
    {

        public Class1()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(senddata));
            t.Start();//开一个线程
            DTUALL.content = "";
        }
        DTUDATA DTUALL = new DTUDATA();
        void senddata()
        {
            while (true)//永远循环
            {
                try {

                    System.Threading.Thread.Sleep(1000);
                    Datauser[] listsoctemp = new Datauser[listsoc.Count];
                    listsoc.CopyTo(0, listsoctemp, 0, listsoctemp.Length);
                    //为什么写这两句，是因为多线程中，添加和删除集合的操作，都会对其他线程有影响，所以先
                    //拷贝一份副本
                    foreach (Datauser soc in listsoctemp)
                    {
                        SendRoot<DTUDATA>(soc.soc, 0x2, "getdata", DTUALL, 0, soc.token);//把接收到的数据发送给客户端
                    }

                } catch { }

            }

        }
        List<Datauser> listsoc = new List<Datauser>();
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
          
        }
        /// <summary>
        /// 我把这个类所又逻辑数据，命名为0x23，支持0x00-0xfe
        /// </summary>
        /// <returns></returns>
        public override byte Getcommand()
        {
            return 0x23;
        }
        [InstallFun("forever",true)]
        public void DTUtest(Socket soc, byte[] _0x01, String ip, int prot)
        {

            //在主动连接模式中. _0x01.Length,如果等于0，则说明，通知服务端设备连接成功了。
            // SendDtu(soc, new byte[] { 22, 22, 22, 22, }, ip, prot);
            //
            DTUALL.Data = _0x01[0];//我只取第一个字节，因为我是模拟的，这样简单省事。
            if (_0x01.Length == 0)
            {
                DTUALL.content = ip + prot + "上线了。";
            }

        }
        /// <summary>
        /// 现在我要写自己的逻辑处理内容了，我写简单一点，比如收到A，返回AB
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="_0x01"></param>
        [InstallFun("forever")]
        public void testaabb(Socket soc, _baseModel _0x01)
        {
          
            _0x01.Parameter = "ok";
            _0x01.Root = "ok";
            send(soc, 0x01, _0x01.Getjson());

        }
        /// <summary>
        /// 这个方法可以获得0XFF的命令，这个命令是保留字，系统把它用来路由通知服务端，客户端谁上线了。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="soc"></param>
        public override void Runcommand(byte command, string data, Socket soc)
        {
            string[] temp = data.Split('|');
            if (temp[0] == "in")//in是上线，out是下线
            {
                String Token = temp[1];//这个就是上线人员的Token了
                //我暂时不考虑谁上线的问题，我把只要上线的都推送，做个简单例子。
                Datauser du = new Datauser();
                du.soc = soc;
                du.token = Token;
                listsoc.Add(du);
            }
            else if (temp[0] == "out")
            {
                String Token = temp[1];//这个就是下线人员的Token了
            }
        }
        public override bool Run(string data, Socket soc)
        {
            return true;
        }
        /// <summary>
        /// 这个方法的意思是，有人走了--但是我们使用路由这里就不好用了。还好我们还有办法
        /// </summary>
        /// <param name="soc"></param>
        public override void TCPCommand_EventDeleteConnSoc(Socket soc)
        {
           
        }
        /// <summary>
        /// 这个方法的意思是，有人来了
        /// </summary>
        /// <param name="soc"></param>
        public override void TCPCommand_EventUpdataConnSoc(Socket soc)
        {
            //listsoc.Add(soc);
        }
    }
}
