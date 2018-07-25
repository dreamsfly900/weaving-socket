using DigitalRuby.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using UnityEngine;
using WeaveBase;

namespace MyTcpClient
{
    public class WeaveSocketGameClientUseEZThread
    {

        //public Thread threadA;
        //public Thread threadB;

        //private ThreadPoolScheduler myThreadScheduler;


        WeaveBaseManager xmhelper = new WeaveBaseManager();

        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool isok = false;
        /// <summary>
        /// 在接收数据
        /// </summary>
        public bool isReceives = false;
        /// <summary>
        /// 是否在线了
        /// </summary>
        public bool IsOnline = false;

        DateTime timeout;
        /// <summary>
        /// 数据超时时间
        /// </summary>
        int mytimeout = 90;

        /// <summary>
        /// 队列中没有排队的方法需要执行
        /// </summary>
        List<TempPakeage> mytemppakeList = new List<TempPakeage>();

        public List<byte[]> ListData = new List<byte[]>();

        public string tokan;

        public String ip;
        public int port;

        public event ReceiveMessage ReceiveMessageEvent;
        public event ConnectOk ConnectOkEvent;

        public event ReceiveBit ReceiveBitEvent;
        public event TimeOut TimeOutEvent;
        public event ErrorMessage ErrorMessageEvent;

        public event JumpServer JumpServerEvent;

        public TcpClient tcpClient;

        // System.Threading.Thread receives_thread1;

        // System.Threading.Thread checkToken_UpdateList_thread2;


        SocketDataType s_datatype = SocketDataType.Json;
        public WeaveSocketGameClientUseEZThread(SocketDataType _type)
        {
            s_datatype = _type;

        }

        #region  客户端注册类，，服务端可以按方法名调用

        public void AddListenClass(object obj)
        {
            GetAttributeInfo(obj.GetType(), obj);
            //xmhelper.AddListen()

            //objlist.Add(obj);

        }
        public void DeleteListenClass(object obj)
        {
            deleteAttributeInfo(obj.GetType(), obj);
            //xmhelper.AddListen()

            //objlist.Add(obj);

        }
        public void deleteAttributeInfo(Type t, object obj)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute == null)
                {

                }
                else
                {
                    xmhelper.DeleteListen(mi.Name);
                }
            }
        }
        public void GetAttributeInfo(Type t, object obj)
        {
            foreach (MethodInfo mi in t.GetMethods())
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute == null)
                { }
                else
                {
                    Delegate del = Delegate.CreateDelegate(typeof(WeaveRequestDataDelegate), obj, mi, true);
                    xmhelper.AddListen(mi.Name, del as WeaveRequestDataDelegate, myattribute.Type);
                }
            }
        }

        #endregion



        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="_ip">IP地址</param>
        /// <param name="_port">端口号</param>
        /// <param name="_timeout">过期时间</param>
        /// <param name="_takon">是否takon</param>
        /// <returns></returns>
        public bool StartConnect(string _ip, int _port, int _timeout, bool _takon)
        {
            mytimeout = _timeout;
            ip = _ip;
            port = _port;
            return StartConnectToServer(ip, port, _takon);
        }
        public bool RestartConnectToServer(bool takon)
        {
            return StartConnectToServer(ip, port, takon);
        }


        private bool StartConnectToServer(string _ip, int _port, bool _takon)
        {
            try
            {
                if (s_datatype == SocketDataType.Json && ReceiveMessageEvent == null)
                    Debug.Log("没有注册receiveServerEvent事件");

                if (s_datatype == SocketDataType.Json && ReceiveBitEvent == null)
                    Debug.Log("没有注册receiveServerEventbit事件");
                ip = _ip;
                port = _port;

                //tcpClient = new TcpClient(ip, port);
                tcpClient = new TcpClient();
                //  tcpc.ExclusiveAddressUse = false;

                try
                {
                    tcpClient.Connect(ip, port);
                }
                catch
                {
                    return false;
                }

                IsOnline = true;
                isok = true;

                timeout = DateTime.Now;
                if (!isReceives)
                {
                    isReceives = true;




                    /*开始执行线程开始*/
                    // myThreadScheduler = Loom.CreateThreadPoolScheduler();

                    //--------------- Ending Single threaded routine 单线程程序结束--------------------
                    // threadA = Loom.StartSingleThread(ReceivesThread, null, System.Threading.ThreadPriority.Normal, true);
                    //--------------- Ending Single threaded routine 单线程程序结束--------------------

                    //--------------- Continues Single threaded routine 在单线程的程序--------------------
                    // threadB = Loom.StartSingleThread(CheckToken_UpdateListDataThread, System.Threading.ThreadPriority.Normal, true);
                    //--------------- Continues Single threaded routine  在单线程的程序--------------------
                    /*开始执行线程结束*/
                    /*改用新的EZThreading插件的方法开始*/
                    EZThread.ExecuteInBackground(() => ReceivesThread());

                    EZThread.ExecuteInBackground(() => CheckToken_UpdateListDataThread());


                    /*改用新的EZTheading插件的方法结束*/

                }
                int ss = 0;

                if (!_takon)
                    return true;

                while (tokan == null)
                {
                     System.Threading.Thread.Sleep(1000);
                    // Loom.WaitForNextFrame(10);
                    //Loom.WaitForSeconds(1);
                    ss++;
                    if (ss > 10)
                        return false;
                }

                if (ConnectOkEvent != null)
                    ConnectOkEvent();

                return true;
            }
            catch (Exception e)
            {
                IsOnline = false;
                if (ErrorMessageEvent != null)
                    ErrorMessageEvent(1, e.Message);
                return false;
            }
        }


        #region  几个方法的方法

        public bool SendParameter<T>(byte command, String Request, T Parameter, int Querycount)
        {
            WeaveBase.WeaveSession b = new WeaveBase.WeaveSession();
            b.Request = Request;
            b.Token = this.tokan;
            b.SetParameter<T>(Parameter);
            b.Querycount = Querycount;
            return SendStringCheck(command, b.Getjson());
        }
        public bool SendRoot<T>(byte command, String Request, T Root, int Querycount)
        {
            WeaveBase.WeaveSession b = new WeaveBase.WeaveSession();
            b.Request = Request;
            b.Token = this.tokan;
            b.SetRoot<T>(Root);
            b.Querycount = Querycount;
            return SendStringCheck(command, b.Getjson());
        }
        public void Send(byte[] b)
        {
            tcpClient.Client.Send(b);
        }
        public bool SendStringCheck(byte command, string text)
        {
            try
            {
                //byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                //byte[] part3_length = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                //byte[] b = new byte[2 + part3_length.Length + sendb.Length];
                //b[0] = command;
                //b[1] = (byte)part3_length.Length;
                //part3_length.CopyTo(b, 2);
                //扩充 第四部分数据（待发送的数据）的长度，扩充到b数组第三位开始的后面
                //sendb.CopyTo(b, 2 + part3_length.Length);
                //扩充 第四部分数据实际的数据，扩充到b数组第三部分结尾后面...

                byte[] b = MyGameClientHelper.CodingProtocol(command, text);

                int count = (b.Length <= 40960 ? b.Length / 40960 : (b.Length / 40960) + 1);
                if (count == 0)
                {
                    //判定数据长度，，实际指的是大小是不是小于40kb,,,
                    tcpClient.Client.Send(b);

                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * 40960) > 40960 ? 40960 : b.Length - (i * 40960);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * 40960, temp, 0, zz);
                        tcpClient.Client.Send(temp);
                        //分割发送......
                         System.Threading.Thread.Sleep(1);
                      
                    }
                }
            }
            catch (Exception ee)
            {
                IsOnline = false;
                CloseConnect();
                if (TimeOutEvent != null)
                    TimeOutEvent();

                SendStringCheck(command, text);
                if (ErrorMessageEvent != null)
                    ErrorMessageEvent(9, "send:" + ee.Message);
                return false;
            }
            // tcpc.Close();

            return true;
        }
        public bool SendByteCheck(byte command, byte[] text)
        {
            try
            {
                //byte[] sendb = text;
                //byte[] lens = MyGameClientHelper.ConvertToByteList(sendb.Length);
                //byte[] b = new byte[2 + lens.Length + sendb.Length];
                //b[0] = command;
                //b[1] = (byte)lens.Length;
                //lens.CopyTo(b, 2);
                //sendb.CopyTo(b, 2 + lens.Length);
                byte[] b = MyGameClientHelper.CodingProtocol(command, text);

                int count = (b.Length <= 40960 ? b.Length / 40960 : (b.Length / 40960) + 1);
                if (count == 0)
                {
                    tcpClient.Client.Send(b);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int zz = b.Length - (i * 40960) > 40960 ? 40960 : b.Length - (i * 40960);
                        byte[] temp = new byte[zz];
                        Array.Copy(b, i * 40960, temp, 0, zz);
                        tcpClient.Client.Send(temp);
                        System.Threading.Thread.Sleep(1);
                        // Loom.WaitForNextFrame(10);
                       // Loom.WaitForSeconds(0.001f);
                    }
                }
            }
            catch (Exception ee)
            {
                IsOnline = false;
                CloseConnect();
                if (TimeOutEvent != null)
                    TimeOutEvent();

                SendByteCheck(command, text);
                if (ErrorMessageEvent != null)
                    ErrorMessageEvent(9, "send:" + ee.Message);
                return false;
            }
            // tcpc.Close();

            return true;
        }


        #endregion



        /// <summary>
        /// 通过主线程执行方法避免跨线程UI问题
        /// </summary>
        public void OnTick()
        {
            if (mytemppakeList.Count > 0)
            {
                try
                {
                    TempPakeage str = mytemppakeList[0];
                    xmhelper.Init(str.date, null);
                    //receiveServerEvent(str.command, str.date);

                }
                catch
                {
                }
                try
                {
                    mytemppakeList.RemoveAt(0);
                }
                catch { }
            }
            Debug.Log("队列中没有排队的方法需要执行。");
        }


        public void CloseConnect()
        {
            try
            {
                isok = false;

                IsOnline = false;
                //发送我要断开连接的消息
                this.SendRoot<int>(0x06, "OneClientDisConnected", 0, 0);


                // receives_thread1.Abort();
                //checkToken_UpdateList_thread2.Abort();

                tcpClient.Close();
                AbortThread();

            }
            catch
            {
                Debug.Log("CloseConnect失败，发生异常");
            }


        }


        void AbortThread()
        {
           //EZThreading插件有 自带Destory方法自动检测UNITY程序关闭事件触发，这里什么都不用写

        }


        /// <summary>
        /// 接收到服务器发来的数据的处理方法
        /// </summary>
        /// <param name="obj"></param>
        void ReceiveData(object obj)
        {
            TempPakeage str = obj as TempPakeage;
            mytemppakeList.Add(str);
            if (ReceiveMessageEvent != null)
                ReceiveMessageEvent(str.command, str.date);

        }



        /// <summary>
        /// 线程启动的方法，初始化连接后要接收服务器发来的token,并更新
        /// </summary>
        void CheckToken_UpdateListDataThread()
        {
            while (isok)
            {
                 System.Threading.Thread.Sleep(10);
              
                try
                {
                    int count = ListData.Count;
                    if (count > 0)
                    {
                        int bytesRead = ListData[0] != null ? ListData[0].Length : 0;
                        if (bytesRead == 0)
                            continue;
                        //如果到这里的continue内部，那么下面的代码不执行，重新到 --》 System.Threading.Thread.Sleep(10);开始


                        byte[] tempbtye = new byte[bytesRead];
                        //解析消息体ListData，，
                        Array.Copy(ListData[0], tempbtye, tempbtye.Length);
                        // 检查tempbtye(理论上里面应该没有0x99,有方法已经处理掉了，但是可能会有)检测还有没有0x99开头的心跳包
                        _0x99:
                        if (tempbtye[0] == 0x99)
                        {
                            if (bytesRead > 1)
                            {
                                byte[] b = new byte[bytesRead - 1];
                                byte[] t = tempbtye;
                                //把心跳包0x99去掉
                                Array.Copy(t, 1, b, 0, b.Length);
                                ListData[0] = b;
                                tempbtye = b;
                                goto _0x99;
                            }
                            else
                            {   //说明只有1个字节，心跳包包头，无任何意思，那么直接删除
                                ListData.RemoveAt(0);
                                continue;
                            }
                        }

                        //ListData[0]第一个元素的长度 大于2
                        if (bytesRead > 2)
                        {
                            //第二段是固定一个字节，最高255，代表了第三段的长度
                            //这样第三段最高可以255*255位，这样表示数据内容的长度基本可以无限大了
                            // int a = tempbtye[1];
                            int part3_Length = tempbtye[1];
                            //tempbtye既是ListData[0]数据，
                            //第一位为command,指令
                            //第二位的数据是第三段的长度
                            //第三段的数据是第四段的长度
                            //第四段是内容数据
                            if (bytesRead > 2 + part3_Length)
                            {   //如果收到数据这段数据，大于
                                // int len = 0;
                                int part4_Length = 0;
                                if (s_datatype == SocketDataType.Bytes)
                                {
                                    byte[] bb = new byte[part3_Length];
                                    byte[] part4_LengthBitArray = new byte[part3_Length];

                                    //将tempbtye从第三位开始，复制数据到part4_LengthBitArray
                                    Array.Copy(tempbtye, 2, part4_LengthBitArray, 0, part3_Length);
                                    //len = ConvertToInt(bb);
                                    //获得实际数据的长度，也就是第四部分数据的长度
                                    part4_Length = MyGameClientHelper.ConvertToInt(part4_LengthBitArray);
                                }
                                else
                                {   //如果DataType不是DataType.bytes类型，，， 是Json类型
                                    //从某个Data中第三位开始截取数据，，获取第四段数据长度的字符串string
                                    String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, part3_Length);
                                    String part4_Lengthstr = System.Text.Encoding.UTF8.GetString(tempbtye, 2, part3_Length);

                                    part4_Length = 0;
                                    //len = 0;
                                    //int part4_Lengthstrlength = 0;
                                    try
                                    {
                                        // len = int.Parse(temp);
                                        part4_Length = int.Parse(part4_Lengthstr);
                                        if (part4_Length == 0)  //len
                                        {   //如果第四段数据的长度为0 ，，，说明发的空消息，，没有第四段数据
                                            //如果第二位没有数据，，说明发的是空消息
                                            ListData.RemoveAt(0);
                                            continue;
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                try
                                {
                                    //如果计算出来的（2位数据位+第三段长度+第四度长度） 比当前ListData[0]长度还大...
                                    if ((part4_Length + 2 + part3_Length) > tempbtye.Length)
                                    {
                                        if (ListData.Count > 1)
                                        {
                                            //将第一个数据包删除
                                            ListData.RemoveAt(0);
                                            //重新读取第一个数据包（第二个数据包变为第一个）内容
                                            byte[] temps = new byte[ListData[0].Length];
                                            //将 数据表内容 拷贝到 temps中
                                            Array.Copy(ListData[0], temps, temps.Length);
                                            //新byte数组长度扩充，原数据长度 + （第二个）元素长度
                                            byte[] temps2 = new byte[tempbtye.Length + temps.Length];

                                            Array.Copy(tempbtye, 0, temps2, 0, tempbtye.Length);
                                            //将第一个元素ListData[0]完全从第一个地址开始 ，完全拷贝到 temps2数组中
                                            Array.Copy(temps, 0, temps2, tempbtye.Length, temps.Length);
                                            //将第二个元素拼接到temps2中，从刚复制数据的最后一位 +1 开始
                                            ListData[0] = temps2;
                                            //最后将更新数据包里面的 第一个元素为新元素
                                        }
                                        else
                                        {
                                              System.Threading.Thread.Sleep(20);
                                           
                                        }
                                        continue;
                                    }
                                    else if (tempbtye.Length > (part4_Length + 2 + part3_Length))
                                    {
                                        //如果 数据包长度 比  计算出来的（2位数据位+第三段长度+第四度长度）  还大 
                                        //考虑大出的部分
                                        int currentAddcount = (part4_Length + 2 + part3_Length);
                                        int offset_length = tempbtye.Length - currentAddcount;
                                        byte[] temps = new byte[offset_length];


                                        //Array.Copy(tempbtye, (part4_Length + 2 + part3_Length), temps, 0, temps.Length);
                                        Array.Copy(tempbtye, currentAddcount, temps, 0, temps.Length);
                                        //把当前ListData[0]中 后面的数据，复制到 temps数组中...

                                        ListData[0] = temps;
                                    }
                                    else if (tempbtye.Length == (part4_Length + 2 + part3_Length))
                                    {   //长度刚好匹配
                                        ListData.RemoveAt(0);
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (ErrorMessageEvent != null)
                                        ErrorMessageEvent(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + part3_Length + "---len" + part4_Length + "--tempbtye" + tempbtye.Length);
                                }

                                try
                                {
                                    if (s_datatype == SocketDataType.Json)
                                    {
                                        //读取出第四部分数据内容，，
                                        string temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + part3_Length, part4_Length);
                                        TempPakeage str = new TempPakeage();
                                        str.command = tempbtye[0];
                                        //命令等于第一位 
                                        str.date = temp;
                                        //服务器发来执行是0xff，说明发送的是token指令
                                        if (tempbtye[0] == 0xff)
                                        {
                                            if (temp.IndexOf("token") >= 0)
                                                tokan = temp.Split('|')[1];

                                            //用单个字符来分隔字符串，并获取第二个元素，，
                                            //这里因为服务端发来的token后面跟了一个|字符
                                            else if (temp.IndexOf("jump") >= 0)
                                            {
                                                //0xff就是指服务器满了
                                                tokan = "连接数量满";
                                                if (JumpServerEvent != null)
                                                    JumpServerEvent(temp.Split('|')[1]);
                                            }
                                            else
                                            {  // 当上面条件都不为真时执行 ，如果虽然指令是0xff，但是不包含token或jump
                                                ReceiveData(str);

                                            }
                                        }
                                        else if (ReceiveMessageEvent != null)
                                        {
                                            //如果tempbtye[0] == 0xff 表示token,不等的情况
                                            ReceiveData(str);

                                        }
                                    }
                                    //if (DT == DataType.bytes)
                                    //{

                                    //    byte[] bs = new byte[len - 2 + a];
                                    //    Array.Copy(tempbtye, bs, bs.Length);
                                    //    temppake str = new temppake();
                                    //    str.command = tempbtye[0];
                                    //    str.datebit = bs;
                                    //    rec(str);

                                    //}
                                    continue;
                                }
                                catch (Exception e)
                                {
                                    if (ErrorMessageEvent != null)
                                        ErrorMessageEvent(3, e.StackTrace + "unup122:" + e.Message);
                                }
                            }
                        }
                        else
                        {   // //ListData[0]第一个元素的Length 不大于2 ，，  
                            if (tempbtye[0] == 0)
                                ListData.RemoveAt(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (ErrorMessageEvent != null)
                        ErrorMessageEvent(3, "unup:" + e.Message + "---" + e.StackTrace);
                    try
                    {
                        ListData.RemoveAt(0);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 接收数据到缓冲区的方法
        /// </summary>
        /// <param name="obj"></param>
        void ReceivesThread(  )
        {
            while (isok)
            {
                System.Threading.Thread.Sleep(50);
             
                try
                {
                    //可以用TcpClient的Available属性判断接收缓冲区是否有数据，来决定是否调用Read方法
                    int bytesRead = tcpClient.Client.Available;
                    if (bytesRead > 0)
                    {
                        //缓冲区
                        byte[] tempbtye = new byte[bytesRead];
                        try
                        {
                            timeout = DateTime.Now;
                            //从绑定接收数据 Socket 到接收缓冲区中
                            tcpClient.Client.Receive(tempbtye);
                            _0x99:
                            if (tempbtye[0] == 0x99)
                            {   //如果缓冲区第一个字符是 0x99心跳包指令
                                timeout = DateTime.Now;
                                //记录现在的时间
                                if (tempbtye.Length > 1)
                                {
                                    //去掉第一个字节，长度总体减去1个，
                                    byte[] b = new byte[bytesRead - 1];
                                    try
                                    {
                                        //复制 Array 中的一系列元素（从指定的源索引开始），
                                        //并将它们粘贴到另一 Array 中（从指定的目标索引开始）
                                        //原始 Array 为tempbtye,原始Array的初始位置
                                        //另外一个 目标 Array , 开始位置为0，长度为b.Length
                                        Array.Copy(tempbtye, 1, b, 0, b.Length);
                                        //那么b中的到的就是去掉心跳包指令0x99以后的后面的数据
                                    }
                                    catch { }
                                    tempbtye = b;
                                    //反复执行去掉，心跳包,,,
                                    goto _0x99;
                                }
                                else
                                    continue;

                                //后面的不执行了，回调到 上面的while循环开始 重新执行...
                            }


                        }
                        catch (Exception ee)
                        {
                            if (ErrorMessageEvent != null)
                                ErrorMessageEvent(22, ee.Message);
                        }
                        //lock (this)

                        //{
                        //将接收到的 非心跳包数据加入到 ListData中
                        ListData.Add(tempbtye);
                        // }

                    }

                    //线程每隔指定的过期时间秒，判定，如果没有数据发送过来，，，tcpc.Client.Available=0 情况下
                    else
                    {
                        try
                        {
                            TimeSpan ts = DateTime.Now - timeout;
                            if (ts.TotalSeconds > mytimeout)
                            {  //判断时间过期
                                IsOnline = false;
                                CloseConnect();
                                //isreceives = false;

                                if (TimeOutEvent != null)
                                    TimeOutEvent();

                                if (ErrorMessageEvent != null)
                                    ErrorMessageEvent(2, "连接超时，未收到服务器指令");
                                continue;
                            }
                        }
                        catch (Exception ee)
                        {
                            if (ErrorMessageEvent != null)
                                ErrorMessageEvent(21, ee.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (ErrorMessageEvent != null)
                        ErrorMessageEvent(2, e.Message);
                }
            }
        }
    }
}
