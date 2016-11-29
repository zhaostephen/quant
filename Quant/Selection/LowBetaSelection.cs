﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interace.Strategies;
using log4net;
using Trade.Selections.Utility;
using Trade.Factors;

namespace Trade.Selections
{
    public class LowBetaSelection : Selection
    {
        static ILog log = typeof(LowBetaSelection).Log();

        double benchmark = 20;
        double beta = 0.5;
        bool junxianduotou = false;

        public LowBetaSelection(bool junxianduotou = false, double beta = 0.5)
        {
            this.junxianduotou = junxianduotou;
            this.beta = beta;
        }

        public override StockPool Pass(IEnumerable<string> stocks)
        {
            var client = new MktDataClient();

            log.Info("query market data");
            var data = stocks
                .AsParallel()
                .Select(code => client.Query(code, Cfg.PeriodEnum.Daily))
                .Where(p => p != null)
                .ToArray();
            log.InfoFormat("total {0}", data.Count());

            log.Info("query fundamentals");
            var basics = client.QueryFundamentals(data.Select(p => p.Code).Distinct().ToArray());

            var stat = data
                            .Select(series => new factorset(series.Code)
                            {
                                收阳百分比 = new close_up_percent(series, TimeSpan.FromDays(180)).value,
                                均线多头 = new jun_xian_dou_tout(series).value,
                                低点反弹高度 = new low_to_historical_lowest(series, new DateTime(2015, 5, 1)).value
                            })
                            .ToArray();

            //if (junxianduotou)
            //    stat = stat.Where(p => p.均线多头).ToArray();

            //stat = stat
            //    .Where(p => p.低点反弹高度 < benchmark * beta)
            //    .OrderBy(p => p.低点反弹高度)
            //    .ToArray();

            var q = from s in stat
                    join b in basics on s.代码 equals b.代码
                    join d in data on s.代码 equals d.Code
                    select new Stock(s.代码,
                    new
                    {
                        s.代码,
                        b.名称,
                        s.收阳百分比,
                        s.低点反弹高度,
                        s.均线多头,
                        b.市盈率,
                        b.总市值,
                        b.所属行业,
                        b.流通市值,
                        b.营业总收入同比,
                        b.销售毛利率,
                        b.净利润同比
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
