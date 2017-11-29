using System;
namespace Weave.Base
{
    public class WeaveScheduling
    {
        private string dts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        private string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
        public String Phone
        {
            get; set;
        }
        public String Context
        {
            get; set;
        }
        public String Dt
        {
            get; set;
        }
        public String Dts
        {
            get { return dts; }
            set { dts = value; }
        }
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
