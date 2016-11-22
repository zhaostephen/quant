using log4net;
using Trade.Cfg;
using Trade.Data;
using Trade.Utility;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Trade.Factors;

namespace Trade
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var client = new MktDataClient();

            log.Info("query market data");
            var data = client.QueryAll(PeriodEnum.Daily, Sector.any);
            log.InfoFormat("total {0}", data.Count());

            log.Info("build stat");
            var stat = data
                .Select(series => new factorset(series.Code)
                {
                    收阳百分比 = new close_up_percent(series, TimeSpan.FromDays(180)).value,
                    均线多头 = new jun_xian_dou_tout(series).value,
                    低点反弹高度 =  new low_to_historical_lowest(series, new DateTime(2015, 5, 1)).value
                })
                .ToArray();

            //log.Info("save stat");
            //File.WriteAllText("__stat__.csv", stat.ToArray().ToCsv(), Encoding.UTF8);

            log.Info("screen");
            var benchmark = 20;
            var safebenchmark = benchmark * 1;

            stat = stat
                .Where(p => p.均线多头)
                .Where(p=>p.低点反弹高度 < safebenchmark)
                .OrderBy(p=>p.低点反弹高度)
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
                        s.均线多头,
                        买入卖出 = BuyOrSell(s.代码) ? "买入" : "卖出",
                        b.市盈率,
                        b.总市值,
                        b.所属行业,
                        b.流通市值,
                        b.营业总收入同比,
                        b.销售毛利率,
                        b.净利润同比
                    };

            File.WriteAllText("__screen__.csv", q.ToArray().ToCsv(), Encoding.UTF8);
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
