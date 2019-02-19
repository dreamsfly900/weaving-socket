using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Weave.Base
{
    public class WeaveEvent
    {
        public byte Command { get; set; }

        public string Data { get; set; }

        public Socket Soc { get; set; }

        public byte[] Masks { get; set; }

        public byte[] Databit { get; set; }

        public EndPoint Ep { get; set; }

        public SslStream Ssl { get; set; }
    }
}
