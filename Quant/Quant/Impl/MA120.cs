using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interace.Quant;
using log4net;

namespace Trade.Strategies.Impl
{
    public class MA120 : Strategy
    {
        static ILog log = LogManager.GetLogger(typeof(MA120));

        public override void Run(Account account)
        {
            var client = new MktDataClient();

            log.InfoFormat("total {0}", account.universe.Count);
            foreach (var stock in account.universe.AsParallel())
            {
                log.InfoFormat("run {0}", stock.Code);
                var daily = client.Query(stock.Code, Cfg.PeriodEnum.Daily);
                if (daily != null && daily.Any())
                {
                    var current = daily.Last();
                    var macd = current.MACD >= 0;

                    var backma120 = false;
                    if (current.MA120.HasValue)
                    {
                        var distance = Math.Abs((current.MA120.Value - current.Low) / current.Low * 100);
                        if (current.Close >= current.MA120 &&
                            (distance <= 1 || current.Low < current.MA120))
                        {
                            backma120 = true;
                        }
                    }

                    if(macd && backma120)
                        Buy(account, stock.Code, daily.Last().Date);
                }
            }
        }
    }
}
