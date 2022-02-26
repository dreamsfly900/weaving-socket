>  
版权所有:河南知真信息技术有限公司

 **与其他架构区别，除了同意数据接收外，架构自带内置协议，保证数据完整** 

### nuget可搜索包Weave.TCPClient与Weave.Server


### 相关延申项目介绍



> WeaveMicro 微服务架构
> 支持.net core 2.x-5.x，正常使用

> https://gitee.com/UDCS/weave-micro

>
> Weave微服务架构 主要目的，尽量简化和减少开发复杂度和难度，尽量双击可使用。 尽量不集成操作数据库等内容，由开发习惯自己选择。只负责最核心内容。 尽量简化调用方法和启动的方式方法



> WeavingDB是一个轻量级的便捷的内存数据库，缓存库。
> 基于 weaving-socket ，欢迎大家学习使用
> 
> https://gitee.com/UDCS/WeavingDB
> 
> 基于 weaving-socket 通讯架构制作的内存数据库，缓存库。



> 
> WsocketAutoUpPrj是一个几个weaving-socket的软件版本自动升级更新程序示例。
> 
> https://gitee.com/UDCS/WsocketAutoUpPrj


### 其他说明
  由于版本变化，一些视频内容与版本不服，仅供参考
- https://gitee.com/dreamsfly900/universal-Data-Communication-System-for-windows/wikis

- 图文版教程:https://my.oschina.net/u/2476624/
- 
- QQ交流群17375149



### 架构简述：

通用数据通讯构建,设计基于TCP通信的交互框架。是编写物联网，消息队列，websocket应用，移动通信应用，IM等完美的选择。可规范先后台交互处理，可支持，B/C,C/S,手机移动标准化的通信方式
。达到后台业务一次编写，前台展示全线支持的目的。


QQ交流群17375149 联系QQ：20573886





服务端：

创建一个控制台程序，引用类库 
using Weave.Base;
using Weave.Server;

然后编写代码
```
static void Main(string[] args)
        {
            WeaveP2Server server = new WeaveP2Server();//初始化类库
            server.receiveevent += Server_receiveevent;//注册接收事件
//当然还有很多其他的事件可以注册，比如新增连接事件，连接断开事件
            server.start(8989);//启动监听8989端口
             
           
            Console.WriteLine("8989listen:");
            Console.ReadKey();
        }

        private static void Server_receiveevent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            Console.WriteLine(data);//输出客户端发来的信息
        }
```
客户端：

然后创建一个控制台程序，引用类库
using Weave.TCPClient;
using Weave.Base;

然后编写代码
```
   P2Pclient client = new P2Pclient(false);//初始化类库
static void  Main(string[] args)
        {
           
            client.timeoutevent += Client_timeoutevent;//注册连接超时事件
            client.receiveServerEvent += Client_receiveServerEvent;//注册接收事件
              client.start("127.0.0.1", 8989, false);//启动连接127.0.0.1服务器的8989端口。不需要服务器TOKEN
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("server link OK:");
            client.send(0x1, "test2017-5-5");//给服务器发送信息，参数1，0x01指令，指令可以设置0-254，其中0x9c与0xff，是保留指令不能使用。参数2：发送string类型的数据。
            Console.WriteLine("send:test2017-5-5");
            Console.ReadKey();
        }

        private static void Client_receiveServerEvent(byte command, string text)
        {
          //command是从服务器发来的指令
          //text是从服务器发来的数据
        }

        private static void Client_timeoutevent()
        {
         //连接超时或断线会启动此事件
            client。Restart(false);//重新连接
        }
 
```
最后：先运行服务器端，在运行客户端，就能在服务器端看到 test2017-5-5 的输出内容。