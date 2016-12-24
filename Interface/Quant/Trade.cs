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
        public string Comments { get; set; }
        public DateTime Ts { get; set; }

        public Trade()
        {
            Ts = DateTime.Now;
        }

        public Trade(string portflio, string stock, DateTime date, string dir, double quantity, string comments)
        {
            Portflio = portflio;
            Stock = stock;
            Date = date;
            Dir = dir;
            Quantity = quantity;
            Comments = comments;
            Ts = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2:yyyy-MM-dd HH:mm:ss},{3},{4},{5}", Portflio, Stock, Date, Dir, Quantity, Comments);
        }

        public static Trade Buy(string portflio, string stock, double quantity, string comments = null, DateTime? date = null)
        {
            return new Trade(portflio, stock, date??DateTime.Today, TradeDir.buy, quantity, comments);
        }

        public static Trade Sell(string portflio, string stock, double quantity, string comments = null, DateTime? date = null)
        {
            return new Trade(portflio, stock, date ?? DateTime.Today, TradeDir.sell, quantity, comments);
        }
    }

    public static class TradeDir
    {
        public const string buy = "buy";
        public const string sell = "sell";
    }
}
