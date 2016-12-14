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

namespace Trade.selections
{
    public class volume : selection
    {
        static ILog log = LogManager.GetLogger(typeof(volume));

        public override universe Pass(IEnumerable<string> stocks)
        {
            var client = new Db.db();

            log.Info("query D");
            var D = stocks
                .AsParallel()
                .Select(code => client.kdata(code, "D"))
                .Where(p => p != null && p.Any())
                .ToArray();
            log.InfoFormat("D total {0}", D.Count());

            var codes = D
                .Where(p =>
                {
                    var macd = (macd)new MACD(p.volume());
                    return macd != null
                    && macd.MACD > 0 && macd.DIF <= 0
                    && macd.Date.Date == DateTime.Today;
                })
                .Select(p => p.Code)
                .Distinct()
                .ToArray();

            //if (codes.Any())
            //{
            //    log.Info("query k15");
            //    var k15 = codes
            //        .AsParallel()
            //        .Select(code => client.kdata(code, "15"))
            //        .Where(p => p != null && p.Any())
            //        .ToArray();
            //    log.InfoFormat("k15 total {0}", k15.Count());
            //    codes = k15
            //        .Where(p =>
            //        {
            //            var close = p.close();
            //            var macd = (macd)new MACD(close);
            //            var deviation = (deviation)new DEVIATION(close, deviationtype.底背离);
            //            return macd != null
            //            && macd.MACD > 0
            //            && macd.Date.Date == DateTime.Today
            //            && deviation != null
            //            && deviation.d2.Date == DateTime.Today;
            //        })
            //        .Select(p => p.Code)
            //        .Distinct()
            //        .ToArray();
            //}

            log.InfoFormat("selected {0}", codes.Count());

            return new universe("volume", codes);
        }
    }
}
