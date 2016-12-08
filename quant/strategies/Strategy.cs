using Cli;
using Interace.Mixin;
using Interace.Quant;
using log4net;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Trade.Cfg;

namespace Quant.strategies
{
    public abstract class Strategy
    {
        static ILog log = typeof(Strategy).Log();

        protected Strategy() { }

        public abstract void Run(Account account);

        protected void Buy(Account account, string stock, DateTime date, double quantity = 0)
        {
            PostTrade(account, Interace.Quant.Trade.Buy(account.Portflio, stock, quantity, date));
        }

        protected void Sell(Account account, string stock, DateTime date, double quantity = 0)
        {
            PostTrade(account, Interace.Quant.Trade.Sell(account.Portflio, stock, quantity, date));
        }

        protected void PostTrade(Account account, Interace.Quant.Trade trade)
        {
            log.WarnFormat("trade | {0}", trade);
            account.Trades.Add(trade);

            var path = Configuration.data.trade.EnsurePathCreated();
            var file = Path.Combine(path, DateTime.Today.ToString("yyyy-MM-dd") + ".csv");
            log.Info("save down trades | " + file);

            var csv = new[] { trade }.ToCsv();
            if (File.Exists(file))
            {
                csv = string.Join(Environment.NewLine,
                    csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToArray());
            }
            File.AppendAllText(
                file,
                csv,
                Encoding.UTF8);
        }
    }
}
