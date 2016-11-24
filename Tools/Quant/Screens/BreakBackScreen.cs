using Trade.Data;
using Trade.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Screens
{
    public class BreakBackScreen : Screen
    {
        public override ScreenResult Do(string stockName, DataSeries data)
        {
            if (!data.Any()) return ScreenResult.Error(stockName, "No stock data");

            var maxDate = data.Max(p => p.Date);

            var jax = new JAX(data, 20, 3);
            var ind = new BREAK_BACK(jax.A());

            var errors = new List<string>();
            if (!ind.Any(p => p.Date == maxDate))
                errors.Add("突破前高后回踩不成功");
          
            if (errors.Count > 0)
                return ScreenResult.Error(stockName, string.Join(" | ", errors));

            return ScreenResult.Ok(stockName);
        }
    }
}
