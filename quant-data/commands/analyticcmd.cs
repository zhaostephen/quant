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
using Trade.Indicator;

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
                        var kdata = new Db.db().kdata(code, ktype);
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
                                close = kdata.Last().close,
                                open = kdata.Last().open,
                                high = kdata.Last().high,
                                low = kdata.Last().low,
                                volume = kdata.Last().volume,
                                chg = chg.Last().Value,
                                ma5 = ma5.Last().Value,
                                ma10 = ma10.Last().Value,
                                ma20 = ma20.Last().Value,
                                ma30 = ma30.Last().Value,
                                ma60 = ma60.Last().Value,
                                ma120 = ma120.Last().Value,
                                dea = macd.Last().DEA,
                                macd = macd.Last().MACD,
                                dif = macd.Last().DIF,
                                deavol = macdvol.Last().DEA,
                                macdvol = macdvol.Last().MACD,
                                difvol = macdvol.Last().DIF
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
