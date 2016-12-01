using DHDVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cpanel
{
   
    static class Program
    {
       static CameraData cd = new CameraData();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] str)
        {
            cd.IP = "g1e5527595.imwork.net";
           
            IPHostEntry hostinfo = Dns.GetHostEntry(cd.IP);
            IPAddress[] aryIP = hostinfo.AddressList;
            cd.IP = aryIP[0].ToString();
            cd.Port = 47355;
            cd.UserName = "admin";
            cd.Pwd = "admin123";
            cd.Code = "1314";
            if (str.Length > 1)
            {
                cd.IP = str[0].Split('|')[0];
              
                cd.Port = Convert.ToInt32(str[0].Split('|')[1]);
                cd.UserName = str[0].Split('|')[2];
                cd.Pwd = str[0].Split('|')[3];
                cd.Code = str[0].Split('|')[4];
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(cd));
        }
    }
}
