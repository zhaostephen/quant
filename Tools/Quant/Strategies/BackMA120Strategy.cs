using Interace.Mixin;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade;
using Trade.Cfg;
using Trade.Data;
using Trade.Factors;
using Trade.Indicator;

namespace Quant.Strategies
{
    class BackMA120Strategy : Strategy
    {
        static ILog log = typeof(SectorStrategy).Log();

        public override string Name { get { return "回踩120日均线"; } }

        protected override IEnumerable<object> InternalRun(IEnumerable<StkDataSeries> data, IEnumerable<Fundamental> basics, StrategyIn parameters)
        {
            var result = data
                .Where(p => p.Any() &&
                    //new jun_xian_dou_tout(p).value &&
                    p.Last().MA120.HasValue &&
                    (p.Last().Low / p.Last().MA120.Value - 1) * 100 >= 0 &&
                    (p.Last().Low / p.Last().MA120.Value - 1) * 100 < 1)
                .ToArray();

            var q = from s in result
                    join b in basics on s.Code equals b.代码
                    let c =s.Last()
                    select new
                    {
                        b.代码,
                        b.名称,
                        b.市盈率,
                        c.Date,
                        c.Low,
                        c.MA120,
                        MaxLossPct = (c.Low /c.MA120.Value - 1) * 100
                    };

            return q.OrderBy(p=>p.MaxLossPct).ThenBy(p=>p.市盈率).ToArray();
        }
    }
}
