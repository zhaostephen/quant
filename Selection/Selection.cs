using Interace.Quant;
using Interface.Quant;
using System.Collections.Generic;

namespace Trade.Selections
{
    public abstract class selection
    {
        public abstract universe Pass(IEnumerable<string> stocks);
    }
}
