using Interace.Strategies;
using System;
using System.Collections.Generic;

namespace Trade.Strategies
{
    public abstract class Selection
    {
        public abstract StockPool Pass(IEnumerable<string> stocks);
    }
}
