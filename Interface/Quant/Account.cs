using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class Account
    {
        public StockPool StockPool { get; set; }
        public Money Money { get; set; }

        public Account(StockPool stocks, Money money)
        {
            StockPool = stocks;
            Money = money;
        }
    }
}
