using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserLogin
{
    public class users
    {
        string name;
        string pwd;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Pwd
        {
            get
            {
                return pwd;
            }

            set
            {
                pwd = value;
            }
        }
    }
}
