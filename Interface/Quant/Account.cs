using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class Account
    {
        public string Portflio { get; set; }
        public StockPool universe { get; set; }
        public List<Trade> Trades { get; set; }
        public IOrder[] orders { get; set; }
        public bool backtest { get; set; }

        public Account(string portflio, StockPool universe, IOrder[] orders, bool backtest)
        {
            Portflio = portflio;
            this.universe = universe;
            this.orders = orders ?? new IOrder[0];
            Trades = new List<Trade>();
            this.backtest = backtest;
        }
    }
}
