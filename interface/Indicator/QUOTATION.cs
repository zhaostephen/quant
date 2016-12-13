using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Indicator;

namespace Interface.Indicator
{
    public class QUOTATION : List<quotation>
    {
        public QUOTATION(kdata data)
        {
            var macd = new MACD(data);
            var ma120 = new MA(data.close(), 120);
            var q = (from a in data
                     join m in macd on a.date equals m.Date
                     select new { date = a.date, price = a, macd = m, ma120 = ma120[m.Date]}).ToArray();

            for (var i = 0; i < q.Length; ++i)
            {
                var item = q[i];
                var state = quotationstate.None;

                if (item.macd.MACD < 0)
                {
                    state = quotationstate.调整;
                    if (item.price.close >= item.price.open)
                        state = quotationstate.反弹;
                }
                else
                {
                    state = quotationstate.上升;
                    if (i - 1 > 0)
                    {
                        if (this[i - 1].state == quotationstate.回调 ||
                           this[i - 1].state == quotationstate.调整)
                            state = quotationstate.回升;
                    }
                    if (item.price.close < item.price.open)
                        state = quotationstate.回调;
                }

                Add(new quotation
                {
                    date = item.date,
                    state = state,
                    position = judgePosition(state, item.price, item.ma120),
                    strategy = judgeStrategy(state, item.price, item.ma120)
                });
            }
        }

        private string judgeStrategy(quotationstate state, kdatapoint price, double ma120)
        {
            switch (state)
            {
                case quotationstate.调整:
                    return price.close > ma120 ? "减仓" : "清仓";
                case quotationstate.反弹:
                    return "高抛低吸";
                case quotationstate.上升:
                    return "持股不动";
                case quotationstate.回升:
                    return "持股不动";
                case quotationstate.回调:
                    return "高抛低吸";
                default:
                    return "";
            }
        }

        private double judgePosition(quotationstate state, kdatapoint price, double ma120)
        {
            switch (state)
            {
                case quotationstate.调整:
                    return price.close > ma120 ? 25 :0;
                case quotationstate.反弹:
                    return 50;
                case quotationstate.上升:
                    return 100;
                case quotationstate.回升:
                    return 100;
                case quotationstate.回调:
                    return 100;
                default:
                    return 0d;
            }
        }

        public static implicit operator quotation(QUOTATION o)
        {
            return o != null && o.Any() ? o.Last() : null;
        }
    }

    public enum quotationstate
    {
        None,
        调整,
        反弹,
        上升,
        回升,
        回调
    }

    public class quotation
    {
        public DateTime date { get; set; }
        public quotationstate state { get; set; }
        public double position { get; set; }
        public string strategy { get; set; }
    }
}
