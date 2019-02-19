using System.Net.Sockets;

namespace Weave.Base.Interface
{
    /// <summary>
    /// 主要TCP服务器接口，有4个事件，两个发送方法（byte数组和string 数据），还有Start方法的
    /// </summary>
    public interface IWeaveTcpBase
    {
        /// <summary>
        /// 端口号
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// 启动服务器的方法
        /// </summary>
        /// <param name="port">要监听的端口号</param>
        void Start(int port);

        /// <summary>
        /// 获取在线客户端的数量
        /// </summary>
        /// <returns>在线客户端数量</returns>
        int GetNetworkItemCount();

        bool Send(Socket soc, byte command, string text);

        bool Send(Socket soc, byte command, byte[] data);

        /// <summary>
        /// 接收到Socket数据事件
        /// </summary>
        event WaveReceiveEventEvent waveReceiveEvent;

        /// <summary>
        /// 接收到Socket发来的Bit数据的事件
        /// </summary>
        event WeaveReceiveBitEvent weaveReceiveBitEvent;

        /// <summary>
        /// 接收到Socket第一次连接到服务器的事件
        /// </summary>
        event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;

        /// <summary>
        /// 接收到Socket断开连接的事件
        /// </summary>
        event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
    }
}
