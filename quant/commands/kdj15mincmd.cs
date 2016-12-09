using Cli;
using CommandLine;
using Interace.Quant;
using Interface.Quant;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade;
using Trade.Selections.Impl;

namespace Quant.commands
{
    [command("kdj15min")]
    class kdj15mincmd : command<kdj15mincmd.parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(kdj15mincmd));

        public kdj15mincmd(string[] args) : base(args) { }

        public override void exec()
        {
            var universe = getUniverse(param.universe);
            var pool = new StockPool(universe.codes);
            var account = new Account("kdj15min", pool);

            log.Info("run strategy");
            new strategies.KDJ15minStrategy().Run(account);
        }

        private universe getUniverse(string name)
        {
            return new client().universe(name);
        }

        public class parameters
        {
            [Option('u', "universe", DefaultValue = "default" , Required = false)]
            public string universe { get; set; }
        }
    }
}
