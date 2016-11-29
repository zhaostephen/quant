using Interace.Quant;
using System;
using System.Linq;

namespace Trade.Strategies
{
    public abstract class Strategy
    {
        protected Strategy() { }

        public abstract void Run(Account account);

        protected void Buy(Account account, double quantity = 0)
        {
            PostTrade(account, Interace.Quant.Trade.Buy(account.Portflio, quantity));
        }

        protected void Sell(Account account, double quantity = 0)
        {
            PostTrade(account, Interace.Quant.Trade.Sell(account.Portflio, quantity));
        }

        protected void PostTrade(Account account, Interace.Quant.Trade trade)
        {
            account.Trades.Add(trade);
        }
    }
}
