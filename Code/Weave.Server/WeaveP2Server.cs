using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Weave.Base;

namespace Weave.Server
{

    /// <summary>
    /// 继承自Weava.Base里面的WeaveBaseServer类的方法，它原始继承自IWeaveTcpBase接口
    /// </summary>
    public class WeaveP2Server : WeaveBaseServer
    {
        /// <summary>
        /// 没有传递参数，那么默认是new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// </summary>
        public WeaveP2Server()
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 传递了loaclip参数，new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// </summary>
        /// <param name="_loaclip">不知道干嘛用的，好像没有用到，估计是预留字段</param>
        public WeaveP2Server(string _loaclip):base(_loaclip)
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.loaclip = _loaclip;
        }

        /// <summary>
        /// 传递了weaveDataType枚举类型，那么是new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);定义了基类的weaveDataType类型
        /// </summary>
        /// <param name="weaveDataType"></param>
        public WeaveP2Server(WeaveDataTypeEnum weaveDataType):base(weaveDataType)
        {
            socketLisener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.weaveDataType = weaveDataType;
        }
        
    }
  
}
