using Interace.Strategies;
using System;
using System.Collections.Generic;

namespace Trade.Strategies
{
    public abstract class Strategy
    {
        protected Strategy() { }

        public void Run(Account account)
        {
            foreach(var stock in account.StockPool)
            {
                Buy(account,stock);
                Sell(account, stock);
            }
        }

        protected abstract void Sell(Account account, string stock);
        protected abstract void Buy(Account account, string stock);
    }
}
