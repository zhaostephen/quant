using Interace.Mixin;
using Interace.Quant;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade;
using Trade.Cfg;
using Trade.Selections.Impl;

namespace Quant
{
    public class QuantService
    {
        static ILog log = LogManager.GetLogger(typeof(QuantService));

        public void LowBeta(string sector)
        {
            log.Info("run selection");
            var pool = new LowBeta(junxianduotou: false).Pass(codes(sector));
            log.WarnFormat("{0} selections", pool.Count);
            var account = new Account("lowbeta", pool);

            log.Info("run strategy");
            new Trade.Strategies.Impl.LowBeta().Run(account);

            save(account);
        }

        public void ma120(string sector)
        {
            var pool = new StockPool(codes(sector));
            var account = new Account("ma120", pool);

            log.Info("run strategy");
            new Trade.Strategies.Impl.MA120().Run(account);

            save(account);
        }

        private IEnumerable<string> codes(string sector)
        {
            log.InfoFormat("query codes from sector {0}", string.IsNullOrEmpty(sector) ? "any" : sector);
            var client = new MktDataClient();
            return client.Codes(sector ?? string.Empty);
        }

        private void save(Account account)
        {
            log.WarnFormat("{0} trades", account.Trades.Count);
            if (account.Trades.Any())
            {
                log.Info("save down trades");
                var path = Configuration.oms.trade.EnsurePathCreated();

                File.WriteAllText(Path.Combine(path, DateTime.Today.ToString("yyyy-MM-dd") + ".csv"), account.Trades.ToCsv(), Encoding.UTF8);
            }
        }
    }
}
