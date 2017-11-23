namespace WeaveBase
{
    public class WeaveTcpToken
    {
        public WeavePortTypeEnum PortType { get; set; }
        public IWeaveTcpBase P2Server;
        public bool IsToken
        {
            get; set;
        }
        public IDataparsing BytesDataparsing { get; set; }
       public WeavePortTypeEnum WPTE { get; set; }
    }
}
