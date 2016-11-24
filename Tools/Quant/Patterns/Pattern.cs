using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Patterns
{
    public class Pattern
    {
        public PatternResult Make(StkDataSeries data)
        {
            PatternResult result;
            if (IdentifyGetIn(data, out result))
                return result;

            throw new NotImplementedException();
        }

        private bool IdentifyGetIn(StkDataSeries data, out PatternResult result)
        {
            throw new NotImplementedException();
        }
    }
}
