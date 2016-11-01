using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class StockData
    {
        public string Name { get; set; }
        public DataSeries Data { get; set; }

        public StockData(string name, DataSeries data)
        {
            Name = name;
            Data = data;
        }
    }
}
