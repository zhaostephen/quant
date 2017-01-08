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
    public class volume : selection
    {
        static ILog log = LogManager.GetLogger(typeof(volume));

        public override universe Pass(IEnumerable<string> stocks)
        {
            var client = new kdatadb();

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
                    var cross = new MACD(p.volume()).cross();

                    return cross.Any() 
                    && cross.Last().type == Interface.Data.crosstype.gold
                    && cross.Last().value.Date.Date == DateTime.Today;
                })
                .Select(p => p.Code)
                .Distinct()
                .ToArray();

            log.InfoFormat("selected {0}", codes.Count());

            return new universe("volume", codes);
        }
    }
}
