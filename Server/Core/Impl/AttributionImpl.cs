using Interace.Attribution;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Db;
using Trade.Factors;

namespace Trade.Impl
{
    public interface IAttributionImpl
    {
        void MakeKeyPrice(IEnumerable<string> codes, DateTime since, bool overwrite=true);
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

        public void MakeKeyPrice(IEnumerable<string> codes, DateTime since, bool overwrite = true)
        {
            var total = codes.Count();
            log.InfoFormat("total {0}", total);

            var i = 0;
            var stopwatch = Stopwatch.StartNew();
             var keyprices = new ConcurrentStack<KeyPrice>();
            foreach(var code in codes.AsParallel().WithDegreeOfParallelism(10))
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} {2}, total cost {3:N0}s", i, total, code, stopwatch.Elapsed.TotalSeconds);

                var d = _mktdb.Query(code, Cfg.PeriodEnum.Daily);
                if (d == null) continue;
                var daily = d.Where(p=>p.Date >= since).ToArray();

                var lowpeaks = peak(daily, (a, prev, next)=>a.Low <= prev.Low && a.Low <= next.Low);
                var highpeaks = peak(daily, (a, prev, next) => a.High >= prev.High && a.High >= next.High);

                if(lowpeaks.Any())
                    keyprices.PushRange(lowpeaks.Select(p=> KeyPrice.Lower(code, p.Date, p.Low)).ToArray());
                if(highpeaks.Any())
                    keyprices.PushRange(highpeaks.Select(p => KeyPrice.Upper(code, p.Date, p.High)).ToArray());
            }

            log.InfoFormat("save key price, total {0}", keyprices.Count);
            _attrdb.SaveKeyPrices(keyprices, overwrite);
        }

        static IEnumerable<DataPoint> peak(DataPoint[] points, Func<DataPoint, DataPoint, DataPoint, bool> cmp, int distance=5)
        {
            var count = points.Length;
            for (var i = distance + 1; i < count - distance; ++i)
            {
                var j = 1; 
                for(; j <= distance;++j)
                {
                    if (!cmp(points[i], points[i - j], points[i + j]))
                        break;
                }
                if (j > distance)
                    yield return points[i];
            }
        }
    }
}
