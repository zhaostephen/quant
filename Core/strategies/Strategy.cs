using Interace.Quant;
using log4net;
using System;
using Trade.Utility;

namespace Quant.strategies
{
    public abstract class Strategy
    {
        static ILog log = typeof(Strategy).Log();

        protected Strategy() { }

        public abstract void Run(Account account);

        protected void Buy(Account account, string stock, DateTime date, double quantity = 0, string comments=null)
        {
            PostTrade(account, Interace.Quant.Trade.Buy(account.Portflio, stock, quantity, comments, date));
        }

        protected void Sell(Account account, string stock, DateTime date, double quantity = 0, string comments = null)
        {
            PostTrade(account, Interace.Quant.Trade.Sell(account.Portflio, stock, quantity, comments, date));
        }

        protected void PostTrade(Account account, Interace.Quant.Trade trade)
        {
            account.Trades.Add(trade);

            foreach (var order in account.orders)
            {
                log.WarnFormat("{0} order trade | {1}", order, trade);
                try
                {
                    order.order(trade);
                }
                catch (Exception ex)
                {
                    log.Error("ex @ "+ order, ex);
                }
            }
        }
    }
}
