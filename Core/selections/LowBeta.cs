using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Trade.Factors;
using Interace.Quant;
using Interface.Quant;
using Trade.Db;

namespace Trade.selections
{
    public class lowbeta : selection
    {
        static ILog log = LogManager.GetLogger(typeof(lowbeta));

        double benchmark = 20;
        double beta = 0.5;
        bool junxianduotou = false;

        public lowbeta(bool junxianduotou = true, double beta = 0.5)
        {
            this.junxianduotou = junxianduotou;
            this.beta = beta;
        }

        public override universe Pass(IEnumerable<string> stocks)
        {
            var client = new kdatadb();

            log.Info("query market data");
            var data = stocks
                .AsParallel()
                .Select(code => client.kdata(code, "D"))
                .Where(p => p != null)
                .ToArray();
            log.InfoFormat("total {0}", data.Count());

            log.Info("query fundamentals");
            var basics = new db().basics(data.Select(p => p.Code).Distinct().ToArray());

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

            return new universe("lowbeta", q.Select(p => p.Code).Distinct().ToArray());
        }
    }
}
