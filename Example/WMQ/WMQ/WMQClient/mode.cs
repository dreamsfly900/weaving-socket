using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public string form;
        //毫秒
        public int Validityperiod;
    }
    public class RegData
    {
        public string to;
        public String type;
        

    }
}
