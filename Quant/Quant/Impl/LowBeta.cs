using System;
using Interace.Quant;
using System.Linq;
using Trade.Factors;
using log4net;

namespace Trade.Strategies.Impl
{
    public class LowBeta : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(LowBeta));

        private double benchmark;
        private double beta;

        public LowBeta(double benchmark = 20, double beta = 1)
        {
            this.benchmark = benchmark;
            this.beta = beta;
        }

        public override void Run(Account account)
        {
            var client = new client();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var daily = client.kdata(stock.Code, "D");
                if(daily != null && daily.Any())
                {
                    var f = new factorset(daily.Code) { 低点反弹高度 = new low_to_historical_lowest(daily, new DateTime(2015, 5, 1)).value };
                    if (f.低点反弹高度 >= benchmark * beta)
                    {
                        log.InfoFormat("sell {0}", stock.Code);
                        Sell(account, stock.Code, daily.Last().date);
                    }
                    else
                    {
                        var min60 = client.kdata(stock.Code, "60");
                        var min15 = client.kdata(stock.Code, "15");
                        if (min60.Any() && min15.Any())
                        {
                            if (min60.Last().macd >= 0 && min15.Last().macd >= 0)
                            {
                                log.InfoFormat("buy {0}", stock.Code);
                                Buy(account, stock.Code, daily.Last().date);
                            }
                        }
                    }
                }
            }
        }
    }
}
