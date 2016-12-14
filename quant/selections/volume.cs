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
                    if (!p.Any()) return false;

                    var close = p.close();
                    var macd = (macd)new MACD(p.volume());
                    //var ma5 = new MA(close, 5);
                    //var ma20 = new MA(close, 20);
                    //var ma60 = new MA(close, 60);
                    var ma120 = new MA(close, 120);

                    return macd != null && macd.MACD > 0 && macd.DIF <= 0 && macd.Date.Date == DateTime.Today
                    && (close.Last().Value>= ma120);
                })
                .Select(p => p.Code)
                .Distinct()
                .ToArray();

            log.InfoFormat("selected {0}", codes.Count());

            return new universe("volume", codes);
        }
    }
}
