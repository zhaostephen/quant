using Cli;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trade.commands
{
    [command("calckdata")]
    class calckdatacmd : command<object>
    {
        static ILog log = typeof(calckdatacmd).Log();

        public calckdatacmd(string[] args)
            :base(args)
        {

        }

        public override void exec()
        {
            log.Info("**********START**********");

            var db = new Db.db();
            var codes = db.codes();
            var i = 0;
            var count = codes.Count();
            foreach (var code in codes.AsParallel())
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} complete code", i, count);

                db.save(db.kdata(code, "5").complete(), "5");
                db.save(db.kdata(code, "15").complete(), "15");
                db.save(db.kdata(code, "30").complete(), "30");
                db.save(db.kdata(code, "60").complete(), "60");
                db.save(db.kdata(code, "D").complete(), "D");
                db.save(db.kdata(code, "W").complete(), "W");
                db.save(db.kdata(code, "M").complete(), "M");
            }

            log.Info("**********DONE**********");
        }
    }
}
