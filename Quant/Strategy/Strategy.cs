using Interace.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trade.Strategies
{
    public abstract class Strategy
    {
        protected Strategy() { }

        public void Run(Account account)
        {
            foreach(var stock in account.StockPool.AsParallel())
            {
                Buy(account, stock);
                Sell(account, stock);
            }
        }

        protected abstract void Sell(Account account, Stock stock);
        protected abstract void Buy(Account account, Stock stock);
    }
}
