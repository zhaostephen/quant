using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class Trade
    {
        public string Portflio { get; set; }
        public string Stock { get; set; }
        public DateTime Date { get; set; }
        public string Dir { get; set; }
        public double Quantity { get; set; }

        public Trade()
        {

        }

        public Trade(string portflio, string stock, DateTime date, string dir, double quantity)
        {
            Portflio = portflio;
            Stock = stock;
            Date = date;
            Dir = dir;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2:yyyy-MM-dd},{3},{4}", Portflio, Stock, Date, Dir, Quantity);
        }

        public static Trade Buy(string portflio, string stock, double quantity, DateTime? date = null)
        {
            return new Trade(portflio, stock, date??DateTime.Today, TradeDir.buy, quantity);
        }

        public static Trade Sell(string portflio, string stock, double quantity, DateTime? date = null)
        {
            return new Trade(portflio, stock, date ?? DateTime.Today, TradeDir.sell, quantity);
        }
    }

    public static class TradeDir
    {
        public const string buy = "buy";
        public const string sell = "sell";
    }
}
