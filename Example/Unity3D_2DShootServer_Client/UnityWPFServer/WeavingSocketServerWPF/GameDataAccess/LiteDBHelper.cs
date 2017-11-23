using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameDataAccess
{
    public static class LiteDBHelper
    {

        public static string GetConString()
        {
            //获取Configuration对象
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            //根据Key读取<add>元素的Value
            string dbstr = config.AppSettings.Settings["LiteDataBasePath"].Value;

            string conStr = dbstr; 
            //ConfigurationManager.AppSettings["LiteDBConstring"].ToString();
           // string conStr = @"F:\MyProjectWeb\YiLeWeb\Web\App_Data\YiLeWebDb.db";
            return conStr;
        }


       

    }
}
