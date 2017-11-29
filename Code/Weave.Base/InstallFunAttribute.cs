using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Weave.Base
{
    public class InstallFunAttribute : System.Attribute
    {
        private string type;
        bool dtu = false;
        /// <summary>
        /// 标识这个方法是执行一次即卸载，还是长期执行
        /// </summary>
        /// <param name="type">forever,或noce</param>
        public InstallFunAttribute(string type)
        {
            Type = type;
        }
        public InstallFunAttribute(string type, bool _Dtu)
        {
            Type = type;
            Dtu = _Dtu;
        }
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public bool Dtu
        {
            get
            {
                return dtu;
            }
            set
            {
                dtu = value;
            }
        }
    }
}
