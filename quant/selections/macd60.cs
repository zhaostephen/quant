using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Trade.Factors;
using Interace.Quant;
using Interface.Quant;

namespace Trade.selections
{
    public class macd60 : selection
    {
        static ILog log = LogManager.GetLogger(typeof(macd60));

        public override universe Pass(IEnumerable<string> stocks)
        {
            var client = new client();

            log.Info("query market data");
            var data = stocks
                .AsParallel()
                .Select(code => client.kdata(code, "60"))
                .Where(p => p != null && p.Any())
                .ToArray();
            log.InfoFormat("total {0}", data.Count());

            var codes = data
                .Where(p => p.Last().macd > 0 && p.Last().dif <= 0.01)
                .Select(p => p.Code)
                .Distinct()
                .ToArray();

            return new universe("macd60", codes);
        }
    }
}
