using Trade.Data;
using Trade.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quant.Selections
{
    public class BreakBack : Selection
    {
        public override SelectionResult Do(string stockName, DataSeries data)
        {
            if (!data.Any()) return SelectionResult.Error(stockName, "No stock data");

            var maxDate = data.Max(p => p.Date);

            var jax = new JAX(data, 20, 3);
            var ind = new BREAK_BACK(jax.A());

            var errors = new List<string>();
            if (!ind.Any(p => p.Date == maxDate))
                errors.Add("突破前高后回踩不成功");
          
            if (errors.Count > 0)
                return SelectionResult.Error(stockName, string.Join(" | ", errors));

            return SelectionResult.Ok(stockName);
        }
    }
}
