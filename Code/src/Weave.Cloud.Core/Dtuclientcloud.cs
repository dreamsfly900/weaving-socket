using System;
using Weave.TCPClient;

namespace Weave.Cloud
{
    /// <summary>
    /// 对TCPClient ,DTUclient的二次封装，有Tcpdut(DTUclient),Token两个属性,,原来有个小写的名称dtuclient,我给改成了Dtuclientcloud
    /// </summary>
    public class Dtuclientcloud
    {
        DTUclient tcpdtu;
        public DTUclient Tcpdtu
        {
            get
            {
                return tcpdtu;
            }
            set
            {
                tcpdtu = value;
            }
        }
        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }
        String token;
    }
}
