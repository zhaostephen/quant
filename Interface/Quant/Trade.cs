using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class Trade
    {
        public string portflio { get; set; }
        public string code { get; set; }
        public DateTime date { get; set; }
        public string dir { get; set; }
        public double quantity { get; set; }
        public string comments { get; set; }
        public DateTime ts { get; set; }

        public Trade()
        {
            ts = DateTime.Now;
        }

        public Trade(string portflio, string stock, DateTime date, string dir, double quantity, string comments)
        {
            this.portflio = portflio;
            code = stock;
            this.date = date;
            this.dir = dir;
            this.quantity = quantity;
            this.comments = comments;
            ts = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2:yyyy-MM-dd HH:mm:ss},{3},{4},{5}", portflio, code, date, dir, quantity, comments);
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
