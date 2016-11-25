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

namespace Quant.Strategies
{
    class SectorStrategy
    {
        static ILog log = typeof(SectorStrategy).Log();

        public SectorStrategy()
        {

        }

        public IEnumerable<object> Run()
        {
            var client = new MktDataClient();

            log.Info("query market data");
            var data = client.QueryAll(PeriodEnum.Daily, Sector.板块指数);
            log.InfoFormat("total {0}", data.Count());

            log.Info("build stat");
            var stat = data
                .Select(series => new factorset(series.Code)
                {
                    收阳百分比 = new close_up_percent(series, TimeSpan.FromDays(180)).value,
                    均线多头 = new jun_xian_dou_tout(series).value,
                    低点反弹高度 = new low_to_historical_lowest(series, new DateTime(2015, 5, 1)).value
                })
                .ToArray();

            log.Info("run selection");
            stat = stat
                .OrderBy(p => p.低点反弹高度)
                .ToArray();

            var basics = client.QueryFundamentals(stat.Select(p => p.代码).Distinct().ToArray());

            var q = from s in stat
                    join b in basics on s.代码 equals b.代码
                    select new
                    {
                        s.代码,
                        b.名称,
                        s.收阳百分比,
                        s.低点反弹高度,
                        短线买入 = BuyOrSell(s.代码) ? "买入" : "卖出",
                        b.市盈率
                    };

            var result = q.ToArray();

            var path = Path.Combine(Configuration.oms.selection.EnsurePathCreated(), "板块.csv");
            log.Info("save to path " + path);
            File.WriteAllText(
                path,
                result.ToCsv(),
                Encoding.UTF8);

            return result;
        }

        static bool BuyOrSell(string code)
        {
            var client = new MktDataClient();

            var Min15 = client.Query(code, PeriodEnum.Min15);
            var Min30 = client.Query(code, PeriodEnum.Min30);
            var Min60 = client.Query(code, PeriodEnum.Min60);

            return Min15.Last().MACD >= 0 &&
                Min30.Last().MACD >= 0 &&
                Min60.Last().MACD >= 0;
        }
    }
}
