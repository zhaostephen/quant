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
using Trade.Mixin;

namespace Quant.Strategies
{
    class LowPEStrategy : Strategy
    {
        static ILog log = typeof(SectorStrategy).Log();

        public override string Name { get { return "LowPE"; } }

        protected override IEnumerable<object> InternalRun(IEnumerable<StkDataSeries> data, IEnumerable<Fundamental> basics, StrategyIn parameters)
        {
            var result = data
                .Where(p => p.Any() && new jun_xian_dou_tout(p).value)
                .ToArray();

            var q = from s in result
                    join b in basics on s.Code equals b.代码
                    where b.市盈率.DoubleNull()<20 && b.市盈率.DoubleNull() > 0
                    let c =s.Last()
                    select new
                    {
                        b.代码,
                        b.名称,
                        b.市盈率,
                        c.Date
                    };

            return q.OrderBy(p=>p.市盈率).ToArray();
        }
    }
}
