using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaveBase;

namespace MyTCPCloud
{
    public class WeaveTCPCommandItem
    {

        public byte CmdName
        {
            get; set;
        }
        public WeaveTCPCommand WeaveTcpCmd
        {
            get; set;
        }


    }
}
