using Trade.Data;
using Trade.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Screens
{
    public class BreakScreen : Screen
    {
        public override ScreenResult Do(string stockName, DataSeries data)
        {
            if (!data.Any()) return ScreenResult.Error(stockName, "No stock data");

            var maxDate = data.Max(p => p.Date);
            var close = data.CloseTimeSeries();

            var jax = new JAX(data, 20, 3);
            var ind = new BREAK(jax.A());

            var errors = new List<string>();
            if (!ind.Any(p => p.Date == maxDate))
                errors.Add("没有突破前期高点");
          
            if (errors.Count > 0)
                return ScreenResult.Error(stockName, string.Join(" | ", errors));

            return ScreenResult.Ok(stockName);
        }
    }
}
