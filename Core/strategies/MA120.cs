using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interace.Quant;
using log4net;
using Trade.Indicator;
using Trade.Data;
using Trade.Db;

namespace Quant.strategies
{
    public class MA120 : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(MA120));

        public override void Run(Account account)
        {
            var client = new kdatadb();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var daily = client.kdata(stock.Code, "D");
                if (daily != null && daily.Any())
                {
                    var close = daily.close();
                    var macdDaily = (macd)new MACD(close);
                    var ma120 = (double?)new MA(close, 120);
                    var current = daily.Last();
                    var macd = macdDaily != null && macdDaily.MACD >= 0;

                    var backma120 = false;
                    if (ma120.HasValue)
                    {
                        var distance = Math.Abs((ma120.Value - current.low) / current.low * 100);
                        if (current.close >= ma120 &&
                            (distance <= 1 || current.low < ma120))
                        {
                            backma120 = true;
                        }
                    }

                    if(macd && backma120)
                        Buy(account, stock.Code, daily.Last().date);
                }
            }
        }
    }
}
