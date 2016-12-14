using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interace.Quant;
using log4net;
using Trade.Indicator;
using Trade.Data;
using Interface.Data;

namespace Quant.strategies
{
    public class volumestrategy : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(volumestrategy));

        public override void Run(Account account)
        {
            var client = new Trade.Db.db();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var k = client.kdata(stock.Code, "D");
                if (k == null && !k.Any())
                {
                    log.WarnFormat("empty data set for {0}", stock.Code);
                    continue;
                }
                var macd = new MACD(k.volume());
                foreach(var p in macd.cross())
                {
                    switch(p.type)
                    {
                        case crosstype.gold:
                            Buy(account, stock.Code, p.value.Date, 0);
                            break;
                        case crosstype.dead:
                            Sell(account, stock.Code, p.value.Date, 0);
                            break;
                    }
                }
            }
        }
    }
}
