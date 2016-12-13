using Cli;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Data;

namespace Trade.commands
{
    [command("sector")]
    class sectorcmd : command<object>
    {
        static ILog log = typeof(sectorcmd).Log();

        public sectorcmd(string[] args)
            :base(args)
        {

        }

        public override void exec()
        {
            log.Info("**********START**********");

            var cache = new Dictionary<string, kdata>();
            var db = new Db.db();
            var sectors = db.sectors();
            var i = 0;
            var count = sectors.Count();
            foreach (var sector in sectors)
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} calc sector", i, count);

                var codes = db.codes(sector);
                if (!codes.Any())
                {
                    log.WarnFormat("empty data set for sector {0}", sector);
                    continue;
                }

                db.save(sum(sector, kdata(db, cache, codes, "5")), "5");
                db.save(sum(sector, kdata(db, cache, codes, "15")), "15");
                db.save(sum(sector, kdata(db, cache, codes, "30")), "30");
                db.save(sum(sector, kdata(db, cache, codes, "60")), "60");
                db.save(sum(sector, kdata(db, cache, codes, "D")), "D");
                db.save(sum(sector, kdata(db, cache, codes, "W")), "W");
                db.save(sum(sector, kdata(db, cache, codes, "M")), "M");
            }

            log.Info("**********DONE**********");
        }

        IEnumerable<kdata> kdata(Db.db db, Dictionary<string, kdata> cache, IEnumerable<string> codes, string ktype)
        {
            var list = new List<kdata>();
            var notincache = new List<string>();
            foreach(var code in codes)
            {
                var key = code + ktype;
                if (cache.ContainsKey(key))
                    list.Add(cache[key]);
                else
                    notincache.Add(code);
            }

            var data = db.kdata(notincache, ktype);
            foreach (var d in data)
            {
                var key = d.Code + ktype;
                cache[key] = d;
            }

            return list.Concat(data).ToArray();
        }

        kdata sum(string code, IEnumerable<kdata> d)
        {
            if (!d.Any())
            {
                log.WarnFormat("empty data set for {0}", code);
                return new kdata(code);
            }

            var dict = new Dictionary<DateTime, kdatapoint>();

            foreach (var kdata in d)
            {
                foreach(var p in kdata)
                {
                    if (!dict.ContainsKey(p.date))
                        dict[p.date] = new kdatapoint() { date = p.date };
                    dict[p.date].open += p.open;
                    dict[p.date].close += p.close;
                    dict[p.date].high += p.high;
                    dict[p.date].low += p.low;
                }
            }

            return new kdata(code, dict.Values);
        }
    }
}
