using Weave.Base.Interface;

namespace Weave.Base
{
    public class WeaveServerPort
    {
        public WeavePortTypeEnum PortType { get; set; }
        public int Port { get; set; }
        public bool IsToken
        {
            get; set;
        }
        public IDataparsing BytesDataparsing { get; set; }
    }
}
