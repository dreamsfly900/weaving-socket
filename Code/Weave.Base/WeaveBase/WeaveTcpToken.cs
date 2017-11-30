using Weave.Base.Interface;

namespace Weave.Base
{
    /// <summary>
    /// 含有 IWeaveTcpBase，IDataparsing两个接口，两个 WeavePortTypeEnum 枚举对象，分别为PortType，WPTE
    /// </summary>
    public class WeaveTcpToken
    {
        /// <summary>
        /// 端口类型
        /// </summary>
        public WeavePortTypeEnum PortType { get; set; }
        /// <summary>
        /// 实现接口的服务器对象
        /// </summary>
        public IWeaveTcpBase P2Server;
        public bool IsToken
        {
            get; set;
        }
        /// <summary>
        /// 数据转换接口的实现类，用于数据处理，转换
        /// </summary>
        public IDataparsing BytesDataparsing { get; set; }
        /// <summary>
        /// 枚举类型，看是哪一种socket协议
        /// </summary>
        public WeavePortTypeEnum WPTE { get; set; }
    }
}
