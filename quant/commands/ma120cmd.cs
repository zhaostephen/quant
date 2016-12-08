using Cli;
using CommandLine;
using Interace.Mixin;
using Interace.Quant;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Selections.Impl;

namespace Quant.commands
{
    [command("ma120")]
    class ma120cmd : command<ma120cmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(lowbetacmd));

        public ma120cmd(string[] args) : base(args) { }

        public override void exec()
        {
            var pool = new StockPool(codes(param.sector));
            var account = new Account("ma120", pool);

            log.Info("run strategy");
            new strategies.MA120().Run(account);
        }

        protected IEnumerable<string> codes(string sector)
        {
            log.InfoFormat("query codes from sector {0}", string.IsNullOrEmpty(sector) ? "any" : sector);
            var client = new Trade.client();
            return client.codes(sector ?? string.Empty);
        }

        public class Parameters
        {
            [Option('s', "sector", DefaultValue = "", Required = true)]
            public string sector { get; set; }
        }
    }
}
