using System.Net.Sockets;

namespace Weave.Server
{
    /// <summary>
    /// DtuModel模型，定义了 一个byte【】数组和一个Socket
    /// </summary>
    public class DtuModel
    {
        public byte[] Data { get; set; }

        public Socket Soc { get; set; }
    }
}
