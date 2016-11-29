using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class Trade
    {
        public DateTime Date { get; set; }
        public string Dir { get; set; }
        public double Quantity { get; set; }
        public string Portflio { get; set; }

        public Trade()
        {

        }

        public Trade(string portflio, DateTime date, string dir, double quantity)
        {
            Portflio = portflio;
            Date = date;
            Dir = dir;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return string.Format("{0},{1:yyyy-MM-dd},{2},{3}", Portflio, Date, Dir, Quantity);
        }

        public static Trade Buy(string portflio, double quantity)
        {
            return new Trade(portflio,DateTime.Today, TradeDir.buy, quantity);
        }

        public static Trade Sell(string portflio, double quantity)
        {
            return new Trade(portflio,DateTime.Today, TradeDir.sell, quantity);
        }
    }

    public static class TradeDir
    {
        public const string buy = "buy";
        public const string sell = "sell";
    }
}
