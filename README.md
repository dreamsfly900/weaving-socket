 2018-3-16 新增加websocket 对于ssl的支持

图文版教程:https://my.oschina.net/u/2476624/blog

 QQ交流群17375149
新版本更新：
2015-8-8
增加U3D游戏客户的通讯项目支持，并提供示例内容。
2017-5-3更新新版本。老版本在多协议公用业务逻辑方面使用了协议中转网关，将协议进行兼容转换，并做到了分布式部署。
目前大量的项目中，大多数不需要使用分布式的连接部署，新版本更新后，可实现单机多协议多接口共享业务逻辑的方式，也就是业务逻辑只用写一次，通过不同的端口监听不同的协议内容，即可达到不同设备不同协议的互联互通。

![输入图片说明](https://git.oschina.net/uploads/images/2017/0503/172653_618507d1_598831.png "在这里输入图片标题")

**新增：
使用该架构制作的聊天室示例程序：
[IM聊天室示例展示](http://dreamsfly900.oschina.io/universal-data-communication-system-for-windows/IM/chat.html)

** 
[WIN10IOT树莓派（物联网）展示示例，如果数值不变动，说明我把树莓派关闭了](http://dreamsfly900.oschina.io/universal-data-communication-system-for-windows/WebApplication1/)
 **
[图片：----IOT运行照片](http://git.oschina.net/uploads/images/2016/1201/135739_2baae981_598831.jpeg "IOT运行照片")


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
最新版本已支持WIN10 IOT （物联网）设备与架构的数据传输支持,linux 的.net core网关版本正在制作中，感谢QQ100538511的贡献者 
QQ交流群17375149 联系QQ：20573886
现已支持：
1.安卓客户端，WP8.1客户端，websocket客户端，C/S C#桌面程序客户端，UWP通用程序客户端。
2.socket负载网关，websocket中转网关,dtu中转网关，http中转网关。
3。socket服务端架构
架构好处：
1.开源方式，更容易的自行维护与编写试用范围。
2.可自建通信平台，稳定方便，免费。
3.支持多种类型网络结构，项目案例，从底层直通用户。高效简洁。
4，学习速度快，编码迅速，只需关心业务逻辑。
5，通过一次逻辑编码，搭配不同的网关，可达到支持不同的网络协议而不需要重构代码。只需要打开网关即可。避免了大量学习SOCKET,WEBSOCKET,DTU,HTTP等相关通信与协议内容。并且可担负负载均衡与单点满载推荐等特点。
架构用途：

1.企业级，通用级C/s系统。相对于直接连接数据库，此架构更稳定安全，相对于基于http通讯的c/s项目，具有更高的执行效率，数据通信更小更安全性。可以后端持久运行逻辑与数据。
2.手机推送项目，相对于第三方手机消息推送此架构，拥有更高的自由特性，更便于对于信息异常的追踪处理，根据项目的特性可以拥有更高的即时通讯。
3.及时通讯项目，更便于开发出c/s，b/s同步的混合项目，例如：开发c/s，b/s的聊天项目，后端逻辑只需编写一次，不需要分别为b/s，c/s单独编写逻辑。只需打开路由即可代理不同协议 。
4.对于复杂网络的项目，一些项目需要从公网发生数据到不同的内网平台，此架构可以统一对外数据接收端口，分发到不同的对应网络。从不同的内网平台的数据可以通过统一端口分发至不同的公网地址。简单的表述就是外网多端数据统一路由分发到对应端，或是内网单一端数据分发外网多端。安全，稳定，快速，健壮。
5.物联网项目。通过usb，com，udt，等接口物联网统一转换为socket接口。 b/s，web项目，对于服务端执行大量消耗等待的功能可使用web socket，使浏览器提升等待体验和避免服务端的阻塞
6。 新增DTU网关，可实现传感器等DTU数值中转至服务器端处理逻辑。
7。新增uwp socket 客户端示例，可支持wp系统与win10 iot底层设备数据直链服务器端。帮助您更简单的实现物联网云平台。
8.新增HTTP协议网关，可使用ajax方式，获取与传输数据，兼容http简单熟悉的编码方式，又可得到socket的高效传输处理属性。

### 架构版本修改特性：
2016-12-01 版本修改：
1.新增并完善UWP（IOT类型项目）的示例程序。
2.修改UWP客户端测试中的BUG与异常内容。
3.UWP(IOT)示例运行在树莓派中并长期执行，可通过示例连接查看运行状态
2016-11-29版本修改：
新增方法可直接获取通过网关加入的上线设备的对象
  online [] onlieuser= GetOnline();
新增
SetonlineByToken 方法，根据TOKEN 设置 name与OBJ属性；
新增
GetonlineByToken 方法，根据TOKEN 获取online对象
新增重写的方法，Tokenout与Tokenin方法，
1.上线设备后激活Tokenin方法，
2. 设备下线后激活Tokenout方法。
用法
 public override void Tokenout(string Token, Socket soc)
   { }

public override void Tokenin(string Token, Socket soc)
 { }

2016年10月10日
新增UWP程序，UWP通用程序客户端。可用于WIN10 IOT设备以及WP10手机等应用程序。

2016年9月29日
路由功能增强
功能1，预设单个路由链接上限，默认30000人。当链接数量达到30000人时则，回发oxff内部指令，推荐其他关联路由地址与端口号。


功能2.增加路由转发效率，使用固定连接数的寻址算法。增加转发效率!