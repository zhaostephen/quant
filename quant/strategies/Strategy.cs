using Interace.Quant;
using System;
using System.Linq;

namespace Quant.strategies
{
    public abstract class Strategy
    {
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
            account.Trades.Add(trade);
        }
    }
}
