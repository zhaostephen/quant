using Cli;
using Interace.Mixin;
using Interace.Quant;
using log4net;
using Quant.flex;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Trade.Cfg;
using Trade.Db;

namespace Quant.strategies
{
    public abstract class Strategy
    {
        static ILog log = typeof(Strategy).Log();

        readonly db db;
        readonly sms sms;

        protected Strategy()
        {
            db = new db();
            sms = new sms();
        }

        public abstract void Run(Account account);

        protected void Buy(Account account, string stock, DateTime date, double quantity = 0)
        {
            PostTrade(account, Interace.Quant.Trade.Buy(account.Portflio, stock, quantity, date.Date));
        }

        protected void Sell(Account account, string stock, DateTime date, double quantity = 0)
        {
            PostTrade(account, Interace.Quant.Trade.Sell(account.Portflio, stock, quantity, date.Date));
        }

        protected void PostTrade(Account account, Interace.Quant.Trade trade)
        {
            log.WarnFormat("trade | {0}", trade);
            account.Trades.Add(trade);

            log.Info("save down");
            db.save(account.Portflio, new[] { trade });

            log.Info("sms order");
            sms.order(trade);
        }
    }
}
