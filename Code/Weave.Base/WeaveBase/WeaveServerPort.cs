﻿using System.Security.Cryptography.X509Certificates;
using Weave.Base.Interface;

namespace Weave.Base
{
    public class WeaveServerPort
    {
        public WeavePortTypeEnum PortType { get; set; }
        public int Port { get; set; }
        public bool IsToken
        {
            get; set;
        }
        X509Certificate2 _Certificate=null;
        public IDataparsing BytesDataparsing { get; set; }

        public X509Certificate2 Certificate
        {
            get
            {
                return _Certificate;
            }

            set
            {
                _Certificate = value;
            }
        }
        // public X509Certificate2 Certificate { get => _Certificate; set => _Certificate = value; }
    }
}
