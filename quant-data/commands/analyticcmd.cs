using Cli;
using Interace.Attribution;
using Interface.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Db;
using Trade.Indicator;
using Trade.Mixin;

namespace Trade.commands
{
    [command("analytic")]
    class analyticcmd : command<object>
    {
        static ILog log = typeof(analyticcmd).Log();

        public analyticcmd(string[] args)
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
            var keypricedates = db.keypricedates();
            var ktypes = new[] { "5", "15", "30", "60", "D", "W", "M" };
            foreach (var code in codes.AsParallel())
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} calc {2}", i, count, code);

                foreach (var ktype in ktypes)
                {
                    try
                    {
                        var kdata = new kdatadb().kdata(code, ktype);
                        if (kdata != null && kdata.Any())
                        {
                            var date = kdata.Last().date;
                            var close = kdata.close();

                            var macd = new MACD(close);
                            var macdvol = new MACD(kdata.volume(100));
                            var ma5 = new MA(close, 5);
                            var ma10 = new MA(close, 10);
                            var ma20 = new MA(close, 20);
                            var ma30 = new MA(close, 30);
                            var ma60 = new MA(close, 60);
                            var ma120 = new MA(close, 120);
                            var chg = new CHG(close);

                            var ka = new kanalytic()
                            {
                                code = code,
                                date = date,
                                ktype = ktype,
                                close = kdata.Last().close.ZeroNaN(),
                                open = kdata.Last().open.ZeroNaN(),
                                high = kdata.Last().high.ZeroNaN(),
                                low = kdata.Last().low.ZeroNaN(),
                                volume = kdata.Last().volume.ZeroNaN(),
                                chg = chg.Last().Value.ZeroNaN(),
                                ma5 = ma5.Last().Value.ZeroNaN(),
                                ma10 = ma10.Last().Value.ZeroNaN(),
                                ma20 = ma20.Last().Value.ZeroNaN(),
                                ma30 = ma30.Last().Value.ZeroNaN(),
                                ma60 = ma60.Last().Value.ZeroNaN(),
                                ma120 = ma120.Last().Value.ZeroNaN(),
                                dea = macd.Last().DEA.ZeroNaN(),
                                macd = macd.Last().MACD.ZeroNaN(),
                                dif = macd.Last().DIF.ZeroNaN(),
                                deavol = macdvol.Last().DEA.ZeroNaN(),
                                macdvol = macdvol.Last().MACD.ZeroNaN(),
                                difvol = macdvol.Last().DIF.ZeroNaN()
                            };

                            db.save(new[] { ka });
                        }
                    }
                    catch (Exception e)
                    {
                        log.Warn("ex @ calc " + code + " for " + ktype, e);
                    }
                }
            }

            log.Info("**********DONE**********");
        }
    }
}
