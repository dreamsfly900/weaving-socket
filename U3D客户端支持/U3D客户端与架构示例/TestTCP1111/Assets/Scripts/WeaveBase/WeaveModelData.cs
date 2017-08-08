using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace WeaveBase
{
    public  class WeaveModelData
    {
        public string Request
        {
            get;set;
        }
        public string type
        {
            get; set;
        }
        public bool dtu
        {
            get; set;
        }
        public WeaveRequestDataDelegate rd
        {
            get; set;
        }
        public WeaveRequestDataDtuDelegate rd2
        {
            get; set;
        }
    }
}
