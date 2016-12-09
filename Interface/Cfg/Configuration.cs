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

        public class data
        {
            public static readonly string PATH = ConfigurationManager.AppSettings["quant"];
            public static readonly DateTime bearcrossbull = new DateTime(2015, 5, 12);
            public static readonly string kdata = PATH + @"\kdata";
            public static readonly string basics = PATH + @"\basics";
            public static readonly string selection = PATH + @"\selection";
            public static readonly string trade = PATH + @"\trade";
        }

        public class encoding
        {
            public static readonly Encoding gbk = Encoding.GetEncoding("gbk");
        }
    }
}
