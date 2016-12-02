using Trade.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Trade.Mixin;
using Trade.Utility;
using log4net;
using ServiceStack;
using Trade.Cfg;
using Interace.Mixin;

namespace Trade.Db
{
    public partial class MktDb
    {
        static ILog log = typeof(MktDb).Log();

        LevelEnum level;

        public MktDb(LevelEnum level = LevelEnum.Analytic)
        {
            this.level = level;
        }

        public StkDataSeries Query(string code, PeriodEnum period = PeriodEnum.Daily)
        {
            var file = Path.Combine(period.Path(level), code + ".csv");
            var p = file.ReadCsv<DataPoint>();
            return new StkDataSeries(code, p);
        }

        public void Save(IEnumerable<StkDataSeries> dataset, PeriodEnum period = PeriodEnum.Daily)
        {
            var path = period.Path(level).EnsurePathCreated();
            foreach (var d in dataset)
            {
                File.WriteAllText(Path.Combine(path, d.Code + ".csv"), d.ToCsv(), Encoding.UTF8);
            }
        }
    }
}
