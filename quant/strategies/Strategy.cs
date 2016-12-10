using Cli;
using Interace.Quant;
using log4net;
using Quant.strategies.orders;
using System;
using Trade.Db;

namespace Quant.strategies
{
    public abstract class Strategy
    {
        static ILog log = typeof(Strategy).Log();
        readonly IOrder[] orders;

        protected Strategy()
        {
            orders = new IOrder[] { new dbOrder(), new smsOrder() };
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
            account.Trades.Add(trade);

            foreach (var order in orders)
            {
                log.WarnFormat("{0} order trade | {1}", order, trade);
                try
                {
                    order.order(account, trade);
                }
                catch (Exception ex)
                {
                    log.Error("ex @ "+ order, ex);
                }
            }
        }
    }
}
