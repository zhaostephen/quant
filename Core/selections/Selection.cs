using Interace.Quant;
using Interface.Quant;
using System.Collections.Generic;

namespace Trade.selections
{
    public abstract class selection
    {
        public abstract universe Pass(IEnumerable<string> stocks);
    }
}
