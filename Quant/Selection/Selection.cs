using Interace.Quant;
using System.Collections.Generic;

namespace Trade.Selections
{
    public abstract class Selection
    {
        public abstract StockPool Pass(IEnumerable<string> stocks);
    }
}
