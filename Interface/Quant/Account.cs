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

        public Account(string portflio, StockPool universe)
        {
            this.Portflio = portflio;
            this.universe = universe;
            Trades = new List<Trade>();
        }
    }
}
