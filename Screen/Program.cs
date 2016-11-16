using log4net;
using Screen.Cfg;
using Screen.Data;
using Screen.Stat;
using Screen.Utility;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Screen
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
                .Select(series => new statistic(series.Code)
                {
                    close_up_percent = new close_up_percent(series, TimeSpan.FromDays(180)).value,
                    jun_xian_dou_tout = new jun_xian_dou_tout(series).value,
                    low_to_historical_lowest =  new low_to_historical_lowest(series, new DateTime(2015, 5, 1)).value
                })
                .ToArray();

            log.Info("save stat");
            File.WriteAllText("__stat__.csv", stat.ToArray().ToCsv(), Encoding.UTF8);

            log.Info("screen");
            var benchmark = 20;
            var safebenchmark = benchmark * 0.5;
            var screen = stat
                .Where(p => p.jun_xian_dou_tout)
                .Where(p=>p.low_to_historical_lowest < safebenchmark)
                .OrderBy(p=>p.low_to_historical_lowest)
                .ToArray();
            File.WriteAllText("__screen__.csv", screen.ToArray().ToCsv(), Encoding.UTF8);
        }
    }
}
