using Cli;
using Interace.Attribution;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Indicator;

namespace Trade.commands
{
    [command("keyprice")]
    class keypricecmd : command<object>
    {
        static ILog log = typeof(sectorcmd).Log();

        public keypricecmd(string[] args)
            :base(args)
        {

        }

        public override void exec()
        {
            log.Info("**********START**********");

            var cache = new Dictionary<string, kdata>();
            var db = new Db.db();
            var codes = db.codes();
            var i = 0;
            var count = codes.Count();
            foreach (var code in codes)
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} calc {2}", i, count, code);

                trycalc(code, "5");
                trycalc(code, "15");
                trycalc(code, "30");
                trycalc(code, "60");
                trycalc(code, "D");
                trycalc(code, "W");
                trycalc(code, "M");
            }

            log.Info("**********DONE**********");
        }

        public void trycalc(string id, string ktype)
        {
            try
            {
                var db = new Db.db();
                var d = db.kdata(id, ktype);
                var peak_h = new PEAK(d, PEAK_TYPE.high);
                var peak_l = new PEAK(d, PEAK_TYPE.low);

                var keyprices = d.Select(p =>
                {
                    var h = peak_h[p.date];
                    var l = peak_l[p.date];
                    if (h > 0) return KeyPrice.high(id, p.date, h, true);
                    else if (l > 0) return KeyPrice.low(id, p.date, l, true);
                    return null;
                })
                .Where(p => p != null)
                .ToArray();

                db.save(keyprices);
            }
            catch(Exception e)
            {
                log.Warn("ex @ calc "+ id + " for "+ ktype, e);
            }
        }
    }
}
