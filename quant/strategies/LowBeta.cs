using System;
using Interace.Quant;
using System.Linq;
using Trade.Factors;
using log4net;
using Trade.Indicator;
using Trade.Data;

namespace Quant.strategies
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
            var client = new Trade.Db.db();

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
                        var macd15 = (macd)new MACD(min15.close());
                        var macd60 = (macd)new MACD(min60.close());
                        if (macd15 != null && macd60 != null)
                        {
                            if (macd15.MACD >= 0 && macd60.MACD >= 0)
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
