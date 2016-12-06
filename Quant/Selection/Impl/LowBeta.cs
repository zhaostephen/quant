using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Trade.Factors;
using Interace.Quant;

namespace Trade.Selections.Impl
{
    public class LowBeta : Selection
    {
        static ILog log = LogManager.GetLogger(typeof(LowBeta));

        double benchmark = 20;
        double beta = 0.5;
        bool junxianduotou = false;

        public LowBeta(bool junxianduotou = true, double beta = 0.5)
        {
            this.junxianduotou = junxianduotou;
            this.beta = beta;
        }

        public override StockPool Pass(IEnumerable<string> stocks)
        {
            var client = new client();

            log.Info("query market data");
            var data = stocks
                .AsParallel()
                .Select(code => client.kdata(code, "D"))
                .Where(p => p != null)
                .ToArray();
            log.InfoFormat("total {0}", data.Count());

            log.Info("query fundamentals");
            var basics = client.basics(data.Select(p => p.Code).Distinct().ToArray());

            var stat = data
                            .Select(series => new factorset(series.Code)
                            {
                                收阳百分比 = new close_up_percent(series, TimeSpan.FromDays(180)).value,
                                均线多头 = new jun_xian_dou_tout(series).value,
                                低点反弹高度 = new low_to_historical_lowest(series, new DateTime(2015, 5, 1)).value
                            })
                            .ToArray();

            if (junxianduotou)
                stat = stat.Where(p => p.均线多头).ToArray();

            stat = stat
                .Where(p => p.低点反弹高度 < benchmark * beta)
                .OrderBy(p => p.低点反弹高度)
                .ToArray();

            var q = from s in stat
                    join b in basics on s.代码 equals b.code
                    join d in data on s.代码 equals d.Code
                    select new Stock(s.代码,
                    new
                    {
                        s.代码,
                        b.name,
                        s.收阳百分比,
                        s.低点反弹高度,
                        s.均线多头,
                        b.pe,
                        b.totalAssets,
                        b.industry,
                        b.liquidAssets
                    });
            //var result = q
            //      .Where(p => p.Data.Any() &&
            //          //new jun_xian_dou_tout(p).value &&
            //          p.Data.Last().MA120.HasValue &&
            //          (p.Data.Last().Low / p.Data.Last().MA120.Value - 1) * 100 >= 0 &&
            //          (p.Data.Last().Low / p.Data.Last().MA120.Value - 1) * 100 < 1)
            //      .ToArray();

            //var q = from s in result
            //        join b in basics on s.Code equals b.代码
            //        let c = s.Last()
            //        select new
            //        {
            //            b.代码,
            //            b.名称,
            //            b.市盈率,
            //            c.Date,
            //            c.Low,
            //            c.MA120,
            //            MaxLossPct = (c.Low / c.MA120.Value - 1) * 100
            //        };

            return new StockPool(q.ToArray());
        }
    }
}
