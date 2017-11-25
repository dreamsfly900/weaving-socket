using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WeaveBase;

namespace MyTCPCloud
{

    public delegate void WeaveServerReceiveSocketMessageCallBack(byte command, string data, WeaveOnLine _socket);


    public delegate void WeaveServerDeleteSocketCallBack(WeaveOnLine _socket);


    public delegate void WeaveServerUpdateSocketCallBack(WeaveOnLine _socket);


    public delegate void WeaveServerReceiveOnLineUnityPlayerMessageCallBack(byte command, string data, UnityPlayerOnClient gamer);


    public delegate void WeaveServerGetUnityPlayerOnLineCallBack(UnityPlayerOnClient gamer);

    public delegate void WeaveServerGetUnityPlayerOffLineCallBack(UnityPlayerOnClient gamer);



}

