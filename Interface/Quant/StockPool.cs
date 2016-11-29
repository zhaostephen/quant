using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interace.Quant
{
    public class StockPool : List<Stock>
    {
        public StockPool()
        {

        }

        public StockPool(IEnumerable<Stock> stocks)
            :base(stocks)
        {

        }

        public IEnumerable<string> Codes { get { return this.Select(p => p.Code).Distinct().ToArray(); } }
        public IEnumerable<object> Attributes { get { return this.Select(p => p.Attribute).Distinct().ToArray(); } }
    }
}
