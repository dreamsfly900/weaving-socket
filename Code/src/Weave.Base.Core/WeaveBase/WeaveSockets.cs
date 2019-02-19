using System.Net.Sockets;

namespace Weave.Base
{
    public class WeaveSockets
    {
        public Socket Sck
        {
            get; set;
        }

        public WeaveScheduling Sch
        {
            get; set;
        }
    }
}
