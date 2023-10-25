# 简介
版权所有：河南知真信息技术有限公司
weaving-socket，已.net core 为基础，设计基于TCP通信的交互框架。是编写物联网，消息队列，websocket应用，移动通信应用，IM等完美的选择。
可规范先后台交互处理，可支持，B/C,C/S,手机移动标准化的通信方式。
- 支持 Json, Bytes, custom 多种方式，分别代表，内置json协议，内置二进制协议，自定义协议（原始数据）
- 支持 socket(TCP),websocket,udp

**与其他架构区别，除了同意数据接收外，架构自带内置协议，保证数据完整** 

# 开发准备
## 安装下载
- nuget可搜索包
- Weave.TCPClient 客户端异步请求包，Weave.TcpSynClient 客户端同步请求包
- Weave.Server 服务端开发包
- U3D开发包 nuget Weave.TCPClient
- [luat客户端代码示例](https://gitee.com/dotnetchina/weaving-socket/tree/New/luat%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)
- [python客户端代码示例](https://gitee.com/dotnetchina/weaving-socket/tree/New/python%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)
- [安卓/java 代码示例](https://gitee.com/dotnetchina/weaving-socket/tree/New/android%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)
##  运行步骤
- 先编写好客户端，与服务器端代码，设置好IP,PROT。然后先运行服务端，在运行客户端。
## 类说明
### 服务端类 Weave.Server 包
 - WeaveP2Server socket服务端类库
 - WeaveWebServer wbesocket服务端类库
 - HttpServer HTTP协议类库
 - WeaveUDPServer  UDP服务端类库
 - 服务类库中事件包括
 weaveReceiveBitEvent - Bytes, custom 类型接收事件
 waveReceiveEvent  Json  类型接收事件
 WeaveReceiveSslEvent  ssl 证书加密接收事件
 weaveDeleteSocketListEvent  客户端断链事件
 weaveUpdateSocketListEvent  客户端连接上线 事件
- Send(soc,command, data); 服务端发送数据方法
### 异步客户端类 Weave.TCPClient 包
 - P2Pclient socket客户端端类库 
 - WeaveUDPclient  UDP客户端端类库
- [luat客户端代码示例](https://gitee.com/dotnetchina/weaving-socket/tree/New/luat%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)
- [python客户端代码示例](https://gitee.com/dotnetchina/weaving-socket/tree/New/python%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)
- [安卓/java 代码示例](https://gitee.com/dotnetchina/weaving-socket/tree/New/android%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)
- [websocket 客户端JS](https://gitee.com/dotnetchina/weaving-socket/blob/New/Code/test/WEB%20JS%E5%BA%93/websocket.js)

- 客户类库中事件包括
 ReceiveServerEventbit - Bytes, custom 类型接收事件
 ReceiveServerEvent  Json  类型接收事件
 Timeoutevent  客户端断链事件
 - Send()客户端发送数据方法
### 同步客户端类 Weave.TcpSynClient 包
- TcpSynClient 同步客户端类库，只有收到服务端返回响应才算完成请求。

## 服务端代码示例

### 服务端：

创建一个控制台程序，引用类库 
using Weave.Base;
using Weave.Server;

然后编写代码
```
static void Main(string[] args)
        {
            WeaveP2Server server = new WeaveP2Server(WeaveDataTypeEnum.Bytes);//初始化类库
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
### WEBSOCKT服务端代码示例
 -  将服务端 代码中 WeaveP2Server 类 替换成 WeaveWebServer类

### UDP服务端代码示例
 -  将服务端 代码中 WeaveP2Server 类 替换成 WeaveUDPServer类


## 客户代码示例


  
  ### 异步请求客户端：

然后创建一个控制台程序，引用类库
using Weave.TCPClient;
using Weave.Base;

然后编写代码
```
   P2Pclient client = new P2Pclient(DataType.bytes);//初始化类库
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
### 同步请求客户端：
- nuegt  Weave.TcpSynClient

```
  Weave.Client.TcpSynClient tcpSynClient = new TcpSynClient(Weave.Client.DataType.bytes, "127.0.0.1", 9903);
            tcpSynClient.Start();
            tcpSynClient.Send(0x01, "asdasd");//发送请求
                 while(true)
                var commdata =  tcpSynClient.Receives(null);//等待回执
```
### websocket客户端：
-  html 中引用 [websocket 客户端JS](https://gitee.com/dotnetchina/weaving-socket/blob/New/Code/test/WEB%20JS%E5%BA%93/websocket.js)

```    var socket;
          socket = new UDCsocket({
                //115.28.26.204
                ip: 'ws://127.0.0.1', port: 11001, conn: function () {
                  //  socket.settakon("123123");
                    alert("连接成功");
                    //socket.SendData(1, "login", "123123ssdfsdf", "");
                }
                , recData: function (text) {
                    //$('#test').html("");
                    $('#test').append("收到:" + text + '<br/>  ')//这个意思你们都懂了把
                }
                , close: function () { alert("连接关闭"); }
                , error: function (msg) { alert("连接错误" + msg); }
                , jump: function (ip) { alert("服务器超过最大连接，请连接其他服务器：" + ip); }
            });
  socket.SendData(0x02, "GetLISTimei", '', ""); //发送内容
```
## Python 客户端示例
 [Python 客户端 使用说明](https://gitee.com/dotnetchina/weaving-socket/tree/New/python%E5%AE%A2%E6%88%B7%E7%AB%AF%E4%BB%A3%E7%A0%81)

# 相关延申项目介绍

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


 


# 联系我们

QQ交流群17375149 联系QQ/微信：20573886