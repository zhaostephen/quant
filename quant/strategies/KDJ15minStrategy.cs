using System;
using Interace.Quant;
using log4net;
using System.Linq;
using Trade.Indicator;

namespace Quant.strategies
{
    public class KDJ15minStrategy : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(KDJ15minStrategy));

        public KDJ15minStrategy()
        {
        }

        public override void Run(Account account)
        {
            var client = new Trade.client();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var k = client.kdata(stock.Code, "15");
                var kdj = new KDJ(k);
                var crossup = kdj.cross_up();
                var crossdown = kdj.cross_down();

                if (crossup.Any())
                {
                    var s = crossup.Last();
                    if ((DateTime.Now - s.Date).TotalMinutes < 30)
                    {
                        Buy(account, stock.Code, s.Date);
                    }
                }
                else if(crossdown.Any())
                {
                    var s = crossdown.Last();
                    if ((DateTime.Now - s.Date).TotalMinutes < 30)
                    {
                        Sell(account, stock.Code, s.Date);
                    }
                }
                else
                {

                }
            }
        }
    }
}