using WeaveBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
namespace WeaveBase
{
    public delegate void WeaveErrorMessageDelegate(Socket soc, WeaveBase.WeaveSession _0x01, string message);
    public delegate void WeaveRequestDataDelegate(Socket soc, WeaveBase.WeaveSession _0x01);
    public delegate void WeaveRequestDataDtuDelegate(Socket soc, byte[] _0x01, string ip, int prot);
    public delegate void WeaveLogDelegate(string type, string log);
    public delegate void WaveReceiveEventEvent(byte command, String data, Socket soc);
    public delegate void WeaveReceiveDtuEvent(byte[] data, Socket soc);
    public delegate void WeaveReceiveBitEvent(byte command, byte[] data, Socket soc);
    public delegate void NATthrough(byte command, String data, EndPoint ep);
    public delegate void WeaveUpdateSocketListEvent(Socket soc);
    public delegate void WeaveDeleteSocketListEvent(Socket soc);
    public delegate void deleteSoc(Socket soc, string token);
   public  delegate void SendHeadDelegate(Socket handler, byte[] tempbtye);
    public delegate void WeaveUpdateudpListEvent(EndPoint ep);
    public delegate void WeaveDeleteudpListEvent(EndPoint ep);
    public delegate void WaveReceivedupEvent(byte command, String data, EndPoint ep);
    public delegate void WeaveReceiveBitdupEvent(byte command, byte[] data, EndPoint ep);

}
