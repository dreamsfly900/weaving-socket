using System.Security.Cryptography.X509Certificates;
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

        public X509Certificate2 Certificate { get; set; } = null;
    }
}
