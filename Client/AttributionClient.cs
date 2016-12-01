using Interace.Attribution;
using Interace.Mixin;
using System.Collections.Generic;
using System.Linq;
using Trade.Cfg;

namespace Trade
{
    public class AttributionClient
    {
        public IEnumerable<KeyPrice> QueryKeyPrices()
        {
            return Path().ReadCsv<KeyPrice>();
        }

        public IEnumerable<KeyPrice> QueryKeyPrices(string code)
        {
            return Path().ReadCsv<KeyPrice>().Where(p=>p.Code == code).ToArray();
        }

        private string Path()
        {
            return System.IO.Path.Combine(Configuration.attribution.root.EnsurePathCreated(), "keyprice.csv");
        }
    }
}
