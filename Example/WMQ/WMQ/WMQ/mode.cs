using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WMQ
{
    public class WMQData
    {
        [JsonIgnore]
        String id;
        public String to;
        public String message;
        [JsonIgnore]
        public DateTime ctime;
        //毫秒
        public int Validityperiod;
        public string form;
    }
    public class RegData
    {
        public string to;
        public String type;
        [JsonIgnore]
        public Socket soc;

    }
}
