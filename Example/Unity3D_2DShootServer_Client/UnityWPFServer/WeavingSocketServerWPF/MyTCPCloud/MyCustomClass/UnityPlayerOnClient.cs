using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaveBase;

namespace MyTCPCloud
{
    public class UnityPlayerOnClient  : WeaveOnLine 
    {
        public bool isLogin { get; set; }

        public string UserName { get; set; }


    }
}
