using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interace.Quant;
using log4net;

namespace Quant.strategies
{
    public class MA120 : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(MA120));

        public override void Run(Account account)
        {
            var client = new Trade.Db.db();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var daily = client.kdata(stock.Code, "D");
                if (daily != null && daily.Any())
                {
                    var current = daily.Last();
                    var macd = current.macd >= 0;

                    var backma120 = false;
                    if (current.ma120.HasValue)
                    {
                        var distance = Math.Abs((current.ma120.Value - current.low) / current.low * 100);
                        if (current.close >= current.ma120 &&
                            (distance <= 1 || current.low < current.ma120))
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
