using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Cfg
{
    public class Configuration
    {
        public static readonly string env = ConfigurationManager.AppSettings["env"];

        public static string quantdb = env == "dev"
            ? @"Server=584a482f41204.gz.cdb.myqcloud.com;Port=17020;Database=quant;Uid=quant;Pwd=Woaiquant123"
            : @"Server=10.66.111.191;Port=3306 ;Database=quant;Uid=quant;Pwd=Woaiquant123";
        public static string basicsdb = env == "dev"
            ? @"Server=584a482f41204.gz.cdb.myqcloud.com;Port=17020;Database=basics;Uid=quant;Pwd=Woaiquant123"
            : @"Server=10.66.111.191;Port=3306 ;Database=basics;Uid=quant;Pwd=Woaiquant123";
        public static string analyticdb = env == "dev"
            ? @"Server=584a482f41204.gz.cdb.myqcloud.com;Port=17020;Database=analytic;Uid=quant;Pwd=Woaiquant123"
            : @"Server=10.66.111.191;Port=3306 ;Database=analytic;Uid=quant;Pwd=Woaiquant123";
        public static string kdatadb = env == "dev"
            ? @"Server=584a482f41204.gz.cdb.myqcloud.com;Port=17020;Database=kdata;Uid=quant;Pwd=Woaiquant123"
            : @"Server=10.66.111.191;Port=3306 ;Database=kdata;Uid=quant;Pwd=Woaiquant123";

        public class data
        {
            public static readonly string PATH = ConfigurationManager.AppSettings["quant"];
            public static readonly DateTime bearcrossbull = new DateTime(2015, 5, 12);
            public static readonly string kdata = PATH + @"\kdata";
            public static readonly string trade = PATH + @"\trade";
        }

        public class encoding
        {
            public static readonly Encoding gbk = Encoding.GetEncoding("gbk");
        }
    }
}
