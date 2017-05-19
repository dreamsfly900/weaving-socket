using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserLogin
{
  public  class ViewData
    {
        string name;
        string school;
        string age;

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

        public string School
        {
            get
            {
                return school;
            }

            set
            {
                school = value;
            }
        }

        public string Age
        {
            get
            {
                return age;
            }

            set
            {
                age = value;
            }
        }
    }
}
