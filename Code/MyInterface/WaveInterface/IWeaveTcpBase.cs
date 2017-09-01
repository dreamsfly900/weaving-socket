using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
namespace WeaveBase
{
    public interface IWeaveTcpBase
    {
        int Port { get; set; }
        void Start(int port);
        int GetNetworkItemCount();
      
        bool Send(Socket soc, byte command, string text);
        bool Send(Socket soc, byte command, byte[] data);
        event WaveReceiveEventEvent waveReceiveEvent;
        event WeaveReceiveBitEvent weaveReceiveBitEvent;
       
        event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
    }
}
