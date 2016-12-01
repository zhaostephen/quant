using Interace.Attribution;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Db;
using Trade.Factors;

namespace Trade.Impl
{
    public interface IAttributionImpl
    {
        void MakeKeyPrice(IEnumerable<string> codes);
    }

    public class AttributionImpl : IAttributionImpl
    {
        static ILog log = LogManager.GetLogger(typeof(AttributionImpl));

        readonly MktDb _mktdb;
        readonly AttributionDb _attrdb;

        public AttributionImpl()
        {
            _mktdb = new MktDb();
            _attrdb = new AttributionDb();
        }

        public void MakeKeyPrice(IEnumerable<string> codes)
        {
            var total = codes.Count();
            log.InfoFormat("total {0}", total);

            var i = 0;
            var keyprices = new ConcurrentStack<KeyPrice>();
            foreach(var code in codes.AsParallel())
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} {2}", i, total, code);

                var daily = _mktdb.Query(code, Cfg.PeriodEnum.Daily);
                if (daily != null)
                {
                    var f = new historical_low(daily, new DateTime(2015, 5, 1));
                    if(f.value != null)
                    {
                        var keyprice = KeyPrice.Lower(code, f.value.Date, f.value.Low);

                        keyprices.Push(keyprice);

                        log.Info(keyprice);
                    }
                }
            }

            log.InfoFormat("save key price, total {0}", keyprices.Count);
            _attrdb.SaveKeyPrices(keyprices);
        }
    }
}
