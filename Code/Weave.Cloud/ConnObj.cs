using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Weave.Cloud
{
    [Serializable]
    public class ConnObj
    {
        bool _Validation = false;
        int id = 0;
        byte[] makes;
        public byte[] Makes
        {
            get
            {
                return makes;
            }
            set
            {
                makes = value;
            }
        }
        string _Token;
        Socket soc;
        public Socket Soc
        {
            get
            {
                return soc;
            }
            set
            {
                soc = value;
            }
        }
        public string Token
        {
            get
            {
                return _Token;
            }
            set
            {
                _Token = value;
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public bool Validation
        {
            get
            {
                return _Validation;
            }
            set
            {
                _Validation = value;
            }
        }
    }
}
