using System;

namespace Weave.Base
{
    public class WeaveScheduling
    {
        public string From
        {
            get; set;
        }

        public string Sgin
        {
            get; set;
        }

        public string To
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        public string Lat
        {
            get; set;
        }

        public string Lng
        {
            get; set;
        }

        public string Phone
        {
            get; set;
        }

        public string Context
        {
            get; set;
        }

        public string Dt
        {
            get; set;
        }

        public string Dts { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public int Err
        {
            get; set;
        }

        public bool Islock
        {
            get; set;
        }
    }
}
