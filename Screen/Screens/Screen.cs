using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen
{
    public abstract class Screen
    {
        public abstract ScreenResult Do(string stockName, DataSeries data);
    }

    public class ScreenResult
    {
        public string StockName { get; set; }
        public bool Good { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get { return !string.IsNullOrEmpty(ErrorMessage); } }

        public static ScreenResult Ok(string stockName)
        {
            return new ScreenResult { StockName = stockName, Good = true };
        }
        public static ScreenResult Error(string stockName, string errorMessage)
        {
            return new ScreenResult { StockName = stockName, ErrorMessage = errorMessage };
        }
    }
}
