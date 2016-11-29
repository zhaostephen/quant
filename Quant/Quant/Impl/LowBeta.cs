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
            var client = new MktDataClient();

            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var daily = client.Query(stock.Code, Cfg.PeriodEnum.Daily);
                if(daily != null && daily.Any())
                {
                    var f = new factorset(daily.Code) { 低点反弹高度 = new low_to_historical_lowest(daily, new DateTime(2015, 5, 1)).value };
                    if (f.低点反弹高度 >= benchmark * beta)
                    {
                        log.InfoFormat("sell {0}", stock.Code);
                        Sell(account);
                    }
                    else
                    {
                        var min60 = client.Query(stock.Code, Cfg.PeriodEnum.Min60);
                        var min15 = client.Query(stock.Code, Cfg.PeriodEnum.Min15);
                        if (min60.Any() && min15.Any())
                        {
                            if (min60.Last().MACD >= 0 && min15.Last().MACD >= 0)
                            {
                                log.InfoFormat("buy {0}", stock.Code);
                                Buy(account);
                            }
                        }
                    }
                }
            }
        }
    }
}
