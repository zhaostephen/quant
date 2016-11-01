using Screen.Data;
using Screen.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Screens
{
    public class BreakJAXScreen : Screen
    {
        public override ScreenResult Do(string stockName, DataSeries data)
        {
            if (!data.Any()) return ScreenResult.Error(stockName, "No stock data");

            var maxDate = data.Max(p => p.Date);
            var close = data.CloseTimeSeries();

            var jax = new JAX(data, 20, 3);
            var intrend = jax.WHICH(maxDate);
            var macd = new MACD(data);
            var macdtrend = macd.WHICH(maxDate);
            var ind = new BREAK(jax.A());

            var ma5 = new MA(close, 5).WHICH(maxDate);
            var ma10 = new MA(close, 10).WHICH(maxDate);
            var ma20 = new MA(close, 20).WHICH(maxDate);
            var ma60 = new MA(close, 60).WHICH(maxDate);

            var errors = new List<string>();
            if (!ind.Any(p => p.Date == maxDate))
                errors.Add("没有突破前期高点");
            if(intrend == null && !intrend.upward)
                errors.Add("JAX趋势向下");
            if( macdtrend == null && macdtrend.MACD < 0d)
                errors.Add("MACD趋势向下");
            if (!(ma5 != 0d && ma10 != 0d && ma20 != 0d && ma60 != 0d
                && ma60 <= ma20 && ma20 <= ma10 && ma10 <= ma5))
                errors.Add("不是多头向上");
            if (errors.Count > 0)
                return ScreenResult.Error(stockName, string.Join(" | ", errors));

            return ScreenResult.Ok(stockName);
        }
    }
}
