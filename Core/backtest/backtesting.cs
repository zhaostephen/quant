using Interace.Quant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Data;

namespace Trade.backtest
{
    public class backtesting
    {
        public pnl pnl { get; private set; }

        public backtesting(string code, Series<double> prices,IEnumerable<Interace.Quant.Trade> trades)
        {
            var q = (from p in prices
                    join t in trades on p.Date equals t.Date
                    select new { date = p.Date, quantity = 1000, price = p.Value, Dir = t.Dir }).ToArray();

            var market = 0d;
            var available = 1000;
            var valuePnl = 0d;
            var capital = 0d;
            foreach(var p in q)
            {
                if(p.Dir == TradeDir.buy && available > 0)
                {
                    available = 0;
                    market = p.quantity * p.price;
                    if (capital == 0)
                        capital = market;
                }
                else if(p.Dir == TradeDir.sell && available == 0)
                {
                    valuePnl += (p.quantity * p.price - market);
                    available = 1000;
                }
            }

            pnl = new pnl {capital = capital, value = valuePnl, ratio = valuePnl / capital * 100, code = code, date = prices.Last().Date };
        }
    }
}
