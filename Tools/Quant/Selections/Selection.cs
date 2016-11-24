using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quant.Selections
{
    public abstract class Selection
    {
        public abstract SelectionResult Do(string stockName, DataSeries data);
    }

    public class SelectionResult
    {
        public string StockName { get; set; }
        public bool Good { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get { return !string.IsNullOrEmpty(ErrorMessage); } }

        public static SelectionResult Ok(string stockName)
        {
            return new SelectionResult { StockName = stockName, Good = true };
        }
        public static SelectionResult Error(string stockName, string errorMessage)
        {
            return new SelectionResult { StockName = stockName, ErrorMessage = errorMessage };
        }
    }
}
