using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Trade.Factors;
using Interace.Quant;
using Interface.Quant;
using Trade.Indicator;
using Trade.Data;
using Interace.Attribution;
using Trade.Db;

namespace Trade.selections
{
    public class macd60 : selection
    {
        static ILog log = LogManager.GetLogger(typeof(macd60));

        public override universe Pass(IEnumerable<string> stocks)
        {
            var client = new kdatadb();

            log.Info("query k60");
            var k60 = stocks
                .AsParallel()
                .Select(code => client.kdata(code, "60"))
                .Where(p => p != null && p.Any())
                .ToArray();
            log.InfoFormat("k60 total {0}", k60.Count());

            var codes = k60
                .Where(p =>
                {
                    var macd = (macd)new MACD(p.close());
                    return macd != null
                    && macd.MACD > 0 && macd.DIF <= 0.01
                    && macd.Date.Date == DateTime.Today;
                })
                .Select(p => p.Code)
                .Distinct()
                .ToArray();

            if (codes.Any())
            {
                log.Info("query k15");
                var k15 = codes
                    .AsParallel()
                    .Select(code => client.kdata(code, "15"))
                    .Where(p => p != null && p.Any())
                    .ToArray();
                log.InfoFormat("k15 total {0}", k15.Count());
                codes = k15
                    .Where(p =>
                    {
                        var close = p.close();
                        var macd = (macd)new MACD(close);
                        var deviation = (deviation)new DEVIATION(close, deviationtype.底背离);
                        return macd != null
                        && macd.MACD > 0
                        && macd.Date.Date == DateTime.Today
                        && deviation != null
                        && deviation.d2.Date == DateTime.Today;
                    })
                    .Select(p => p.Code)
                    .Distinct()
                    .ToArray();
            }

            log.InfoFormat("selected {0}", codes.Count());

            return new universe("macd60", codes);
        }
    }
}
