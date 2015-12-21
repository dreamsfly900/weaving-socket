using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C
{
    public class Ccontext
    {

        string content;
        string sendusername;
        string recusername;

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
            }
        }

        public string Sendusername
        {
            get
            {
                return sendusername;
            }

            set
            {
                sendusername = value;
            }
        }

        public string Recusername
        {
            get
            {
                return recusername;
            }

            set
            {
                recusername = value;
            }
        }
    }
}
