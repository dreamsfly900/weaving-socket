using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace P2P
{
    public class NETcollection
    {
        Socket soc;

        public Socket Soc
        {
            get { return soc; }
            set { soc = value; }
        }
        private int _BufferSize = 20480;

        public int BufferSize
        {
            get { return _BufferSize; }
            set { _BufferSize = value; }
        }
        // Receive buffer.
        private byte[] buffer =new byte[20480];

        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    public class NETcollectionUdp
    {
        int port;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        System.Net.IPEndPoint iep;

        public System.Net.IPEndPoint Iep
        {
            get { return iep; }
            set { iep = value; }
        }
        System.Net.IPEndPoint localiep;

        public System.Net.IPEndPoint Localiep
        {
            get { return localiep; }
            set { localiep = value; }
        }
        Socket soc;

        public Socket Soc
        {
            get { return soc; }
            set { soc = value; }
        }
        DateTime timeout;

        public DateTime Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }
    }
}
