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
        public void SaveKeyPrices(IEnumerable<KeyPrice> keyPrices, bool overwrite=true)
        {
            var prices = QueryKeyPrices();
            if (overwrite)
                prices = prices.Where(p=>!p.Auto).ToArray();

            var updates = (from o in prices
                          join n in keyPrices on new { o.Code, o.Date.Date } equals new { n.Code, n.Date.Date }
                          select new {key = o.Code+o.Date.Date.ToString("yyyyMMdd"), o, n }).ToArray();
            foreach(var update in updates)
            {
                update.o.Price = update.n.Price;
                update.o.Auto = update.n.Auto;
                update.o.Flag = update.n.Flag;
            }

            var updateKeys = updates.Select(p => p.key).ToArray();
            var inserts = (from o in keyPrices
                           let key = o.Code + o.Date.Date.ToString("yyyyMMdd")
                           where !updateKeys.Contains(key)
                           select o).ToArray();

            prices = prices.Concat(inserts).OrderBy(p=>p.Code).ThenByDescending(p=>p.Date).ToArray();

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
