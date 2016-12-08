using Cli;
using Interace.Quant;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Selections.Impl;

namespace Quant.commands
{
    [command("kdj15min")]
    class kdj15mincmd : command<object>
    {
        static ILog log = LogManager.GetLogger(typeof(lowbetacmd));

        public kdj15mincmd(string[] args) : base(args) { }

        public override void exec()
        {
            var pool = new StockPool(new[] { "002233" });
            var account = new Account("kdj15min", pool);

            log.Info("run strategy");
            new strategies.KDJ15minStrategy().Run(account);
        }
    }
}
