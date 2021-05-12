>  
版权所有-河南创志威科技有限公司

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


- 新版本更新：
- 2015-8-8
- 增加U3D游戏客户的通讯项目支持，并提供示例内容。
- 2017-5-3更新新版本。老版本在多协议公用业务逻辑方面使用了协议中转网关，将协议进行兼容转换，并做到了分布式部署。
- 目前大量的项目中，大多数不需要使用分布式的连接部署，新版本更新后，可实现单机多协议多接口共享业务逻辑的方式，也就是业务逻辑只用写一次，通过不同的端口监听不同的协议内容，即可达到不同设备不同协议的互联互通。

- 2018-3-16 新增加websocket 对于ssl的支持

- 2018-7-30 新增WeaveSocketDemoForUnity项目资料及视频教程下载地址
- 链接：https://pan.baidu.com/s/1Qu0zmPUAd3oLdves8fpyBQ 密码：jpyu




### 视频教程架构：
** 
教程1

[http://v.youku.com/v_show/id_XMTYxNTg4ODU2MA==.html](http://v.youku.com/v_show/id_XMTYxNTg4ODU2MA==.html)

教程2

[http://v.youku.com/v_show/id_XMTYxNTg4OTYyMA==.html](http://v.youku.com/v_show/id_XMTYxNTg4OTYyMA==.html)

进阶教程，网关的使用

[http://v.youku.com/v_show/id_XMTczOTAzMjAyOA==.html?from=y1.7-2](http://v.youku.com/v_show/id_XMTczOTAzMjAyOA==.html?from=y1.7-2)

高级教程：物联网开发：硬件数据到客户端的项目讲解

[http://v.youku.com/v_show/id_XMTc0MDEzNTkyMA==.html](http://v.youku.com/v_show/id_XMTc0MDEzNTkyMA==.html)


### 架构简述：

通用数据通讯构建,设计基于TCP通信的交互框架。是编写物联网，消息队列，websocket应用，移动通信应用，IM等完美的选择。可规范先后台交互处理，可支持，B/C,C/S,手机移动标准化的通信方式
。达到后台业务一次编写，前台展示全线支持的目的。还可根据网络及负载情况分布式部署网管与服务。先已支持win10 IOT 设备与架构的数据传输支持。


QQ交流群17375149 联系QQ：20573886
现已支持：


- 1.安卓客户端，WP8.1客户端，websocket客户端，C/S C#桌面程序客户端，UWP通用程序客户端。
- 2.socket负载网关，websocket中转网关,dtu中转网关，http中转网关。
- 3。socket服务端架构
架构好处：


1. 1.开源方式，更容易的自行维护与编写试用范围。
1. 2.可自建通信平台，稳定方便，免费。
1. 3.支持多种类型网络结构，项目案例，从底层直通用户。高效简洁。
1. 4，学习速度快，编码迅速，只需关心业务逻辑。
1. 5，通过一次逻辑编码，搭配不同的网关，可达到支持不同的网络协议而不需要重构代码。只需要打开网关即可。避免了大量学习SOCKET,WEBSOCKET,DTU,HTTP等相关通信与协议内容。并且可担负负载均衡与单点满载推荐等特点。
架构用途：

1

1. .企业级，通用级C/s系统。相对于直接连接数据库，此架构更稳定安全，相对于基于http通讯的c/s项目，具有更高的执行效率，数据通信更小更安全性。可以后端持久运行逻辑与数据。
1. 2.手机推送项目，相对于第三方手机消息推送此架构，拥有更高的自由特性，更便于对于信息异常的追踪处理，根据项目的特性可以拥有更高的即时通讯。
1. 3.及时通讯项目，更便于开发出c/s，b/s同步的混合项目，例如：开发c/s，b/s的聊天项目，后端逻辑只需编写一次，不需要分别为b/s，c/s单独编写逻辑。只需打开路由即可代理不同协议 。
1. 4.对于复杂网络的项目，一些项目需要从公网发生数据到不同的内网平台，此架构可以统一对外数据接收端口，分发到不同的对应网络。从不同的内网平台的数据可以通过统一端口分发至不同的公网地址。简单的表述就是外网多端数据统一路由分发到对应端，或是内网单一端数据分发外网多端。安全，稳定，快速，健壮。
1. 5.物联网项目。通过usb，com，udt，等接口物联网统一转换为socket接口。 b/s，web项目，对于服务端执行大量消耗等待的功能可使用web socket，使浏览器提升等待体验和避免服务端的阻塞
1. 6。 新增DTU网关，可实现传感器等DTU数值中转至服务器端处理逻辑。
1. 7。新增uwp socket 客户端示例，可支持wp系统与win10 iot底层设备数据直链服务器端。帮助您更简单的实现物联网云平台。
1. 8.新增HTTP协议网关，可使用ajax方式，获取与传输数据，兼容http简单熟悉的编码方式，又可得到socket的高效传输处理属性。
 


> 架构通讯使用的数据封包协议示意图
> 
![架构通讯使用的数据封包协议示意图](https://images.gitee.com/uploads/images/2018/1130/132516_ece8ae62_598831.png "架构数据包协议.png")
### **简单示例** 


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