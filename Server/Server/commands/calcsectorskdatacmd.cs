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
    [command("calcsectorskdata")]
    class calcsectorskdatacmd : command<object>
    {
        static ILog log = typeof(calckdatacmd).Log();

        public calcsectorskdatacmd(string[] args)
            :base(args)
        {

        }

        public override void exec()
        {
            log.Info("**********START**********");

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

                db.save(sum(sector, db.kdata(codes, "5")).complete(), "5");
                db.save(sum(sector, db.kdata(codes, "15")).complete(), "15");
                db.save(sum(sector, db.kdata(codes, "30")).complete(), "30");
                db.save(sum(sector, db.kdata(codes, "60")).complete(), "60");
                db.save(sum(sector, db.kdata(codes, "D")).complete(), "D");
                db.save(sum(sector, db.kdata(codes, "W")).complete(), "W");
                db.save(sum(sector, db.kdata(codes, "M")).complete(), "M");
            }

            log.Info("**********DONE**********");
        }

        kdata sum(string code, IEnumerable<kdata> d)
        {
            if (!d.Any()) return new kdata(code);

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
