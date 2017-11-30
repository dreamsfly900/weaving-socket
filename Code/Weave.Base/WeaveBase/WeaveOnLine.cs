﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
namespace Weave.Base
{
    /// <summary>
    /// 连接到服务器的Socket封装类，含有Token,Name,Obj和原始Socket 
    /// </summary>
    public class WeaveOnLine
    {
        public string Token
        {
            get; set;
        }
        [JsonIgnore]
        public Socket Socket
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        [JsonIgnore]
        public object Obj
        {
            get; set;
        }
    }
}
