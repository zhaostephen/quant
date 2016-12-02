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
        public class Raw
        {
            public static readonly string PATH = ConfigurationManager.AppSettings["quant"] + @"\raw";

            public static readonly string min_5 = PATH + @"\5mins";
            public static readonly string min_15 = PATH + @"\15mins";
            public static readonly string min_30 = PATH + @"\30mins";
            public static readonly string min_60 = PATH + @"\60mins";
            public static readonly string daily = PATH + @"\daily";
            public static readonly string month = PATH + @"\month";
            public static readonly string week = PATH + @"\week";
            public static readonly string fundamental = PATH + @"\fundamental";
            public static readonly string index = PATH + @"\index";
        }

        public class level1
        {
            public static readonly string PATH = ConfigurationManager.AppSettings["quant"] + @"\analytic";

            public static readonly string min_5 = PATH + @"\5mins";
            public static readonly string min_15 = PATH + @"\15mins";
            public static readonly string min_30 = PATH + @"\30mins";
            public static readonly string min_60 = PATH + @"\60mins";
            public static readonly string daily = PATH + @"\daily";
            public static readonly string month = PATH + @"\month";
            public static readonly string week = PATH + @"\week";

            public static readonly string fundamental = PATH + @"\fundamental\fundamental.csv";
            public static readonly string index = PATH + @"\index";
        }

        public class attribution
        {
            public static readonly string PATH = ConfigurationManager.AppSettings["quant"] + @"\attribution";

            public static readonly string root = PATH;
        }

        public class oms
        {
            public static readonly string PATH = ConfigurationManager.AppSettings["quant"];

            public static readonly string selection = PATH + @"\selection";
            public static readonly string trade = PATH + @"\trade";
        }
    }

    public enum PeriodEnum
    {
        Min5,
        Min15,
        Min30,
        Min60,
        Daily,
        Weekly,
        Monthly
    }

    public enum LevelEnum
    {
        Raw,
        Analytic
    }

    public static class PeriodMixin
    {
        static Dictionary<LevelEnum, Dictionary<PeriodEnum, string>> PATH = new Dictionary<LevelEnum, Dictionary<PeriodEnum, string>>
        {
            {
                LevelEnum.Raw,
                new Dictionary<PeriodEnum, string>()
                {
                    { PeriodEnum.Min5, Configuration.Raw.min_5 },
                    { PeriodEnum.Min15, Configuration.Raw.min_15 },
                    { PeriodEnum.Min30, Configuration.Raw.min_30 },
                    { PeriodEnum.Min60, Configuration.Raw.min_60 },
                    { PeriodEnum.Daily, Configuration.Raw.daily },
                    { PeriodEnum.Monthly, Configuration.Raw.month },
                    { PeriodEnum.Weekly, Configuration.Raw.week }
                }
            },
            {
                LevelEnum.Analytic,
                new Dictionary<PeriodEnum, string>()
                {
                    { PeriodEnum.Min5, Configuration.level1.min_5 },
                    { PeriodEnum.Min15, Configuration.level1.min_15 },
                    { PeriodEnum.Min30, Configuration.level1.min_30 },
                    { PeriodEnum.Min60, Configuration.level1.min_60 },
                    { PeriodEnum.Daily, Configuration.level1.daily },
                    { PeriodEnum.Monthly, Configuration.level1.month },
                    { PeriodEnum.Weekly, Configuration.level1.week }
                }
            }
        };

        public static string Path(this PeriodEnum period, LevelEnum level)
        {
            return PATH[level][period];
        }

        public static bool Daybase(this PeriodEnum period)
        {
            return period >= PeriodEnum.Daily;
        }

        public static bool Minbase(this PeriodEnum period)
        {
            return period < PeriodEnum.Daily;
        }

        public static string Path(this LevelEnum level, PeriodEnum period)
        {
            return PATH[level][period];
        }
    }
}
