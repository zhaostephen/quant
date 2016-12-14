using System;
using Interace.Quant;
using log4net;
using System.Linq;
using Trade.Indicator;

namespace Quant.strategies
{
    public class MACD15minStrategy : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(MACD15minStrategy));

        public MACD15minStrategy()
        {
        }

        public override void Run(Account account)
        {
            var client = new Trade.Db.db();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var k = client.kdata(stock.Code, "15");
                var kdj = new MACD(k.close());
                var crossup = kdj.cross_up();
                var crossdown = kdj.cross_down();

                if (crossup.Any())
                {
                    var s = crossup.Last();
                    if (s.Date == k.Last().date && s.Date.Date == DateTime.Today)
                    {
                        Buy(account, stock.Code, s.Date);
                    }
                }
                else if(crossdown.Any())
                {
                    var s = crossdown.Last();
                    if (s.Date == k.Last().date && s.Date.Date == DateTime.Today)
                    {
                        Sell(account, stock.Code, s.Date);
                    }
                }
                else
                {
                    log.InfoFormat("no signal {0}", stock.Code);
                }
            }
        }
    }
}