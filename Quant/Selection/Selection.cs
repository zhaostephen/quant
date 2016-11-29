using Interace.Strategies;
using System;
using System.Collections.Generic;

namespace Trade.Selections
{
    public abstract class Selection
    {
        public abstract StockPool Pass(IEnumerable<string> stocks);
    }
}
