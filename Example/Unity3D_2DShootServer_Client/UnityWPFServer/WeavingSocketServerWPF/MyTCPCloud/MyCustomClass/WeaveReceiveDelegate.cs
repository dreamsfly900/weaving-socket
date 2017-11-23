using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTCPCloud
{

    public delegate void WeaveServerReceiveDelegate(byte command, string data, UnityPlayerOnClient gamer);


    public delegate void WeaveServerDeleteSocketHander(UnityPlayerOnClient gamer);


    public delegate void WeaveServerUpdateSocketHander(UnityPlayerOnClient gamer);

    public delegate void WeaveServerUpdateUnityPlayerSetOnLineHander(UnityPlayerOnClient gamer);



}

