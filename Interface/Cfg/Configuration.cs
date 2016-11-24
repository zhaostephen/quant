using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Cfg
{
    public class Configuration
    {
        public class Raw
        {
            const string PATH = @"D:\trade\data\raw";

            public const string min_5 = PATH + @"\5mins";
            public const string min_15 = PATH + @"\15mins";
            public const string min_30 = PATH + @"\30mins";
            public const string min_60 = PATH + @"\60mins";
            public const string daily = PATH + @"\daily";
            public const string month = PATH + @"\month";
            public const string week = PATH + @"\week";
            public const string fundamental = PATH + @"\fundamental";
            public const string index = PATH + @"\index";
        }

        public class level1
        {
            const string PATH = @"D:\trade\data\level1";

            public const string min_5 = PATH + @"\5mins";
            public const string min_15 = PATH + @"\15mins";
            public const string min_30 = PATH + @"\30mins";
            public const string min_60 = PATH + @"\60mins";
            public const string daily = PATH + @"\daily";
            public const string month = PATH + @"\month";
            public const string week = PATH + @"\week";

            public const string fundamental = PATH + @"\fundamental\fundamental.csv";
            public const string index = PATH + @"\index";
        }

        public class strategy
        {
            const string PATH = @"D:\trade\data\strategy";

            public const string selection = PATH + @"\selection";
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
        Level1
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
                LevelEnum.Level1,
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
