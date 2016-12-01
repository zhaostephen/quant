using Interace.Attribution;
using Interace.Mixin;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;

namespace Trade.Db
{
    public class AttributionDb
    {
        public void SaveKeyPrices(IEnumerable<KeyPrice> keyPrices)
        {
            var prices = QueryKeyPrices();

            foreach(var price in keyPrices)
            {
                var found = prices.SingleOrDefault(p => p.Code == price.Code && p.Date == price.Date);
                if (found == null)
                    prices = prices.Concat(new[] { price }).ToArray();
                else
                {
                    found.Price = price.Price;
                    found.Auto = price.Auto;
                    found.Flag = price.Flag;
                }
            }

            File.WriteAllText(Path(), prices.ToCsv(), Encoding.UTF8);
        }

        private IEnumerable<KeyPrice> QueryKeyPrices()
        {
            return Path().ReadCsv<KeyPrice>();
        }

        private string Path()
        {
            return System.IO.Path.Combine(Configuration.attribution.root.EnsurePathCreated(), "keyprice.csv");
        }
    }
}
