﻿using Cli;
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
    [command("lowbeta")]
    class lowbetacmd : command<lowbetacmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(lowbetacmd));

        public lowbetacmd(string[] args) : base(args) { }

        public override void exec()
        {
            log.Info("run selection");
            var pool = new LowBeta(junxianduotou: false).Pass(codes(param.sector));
            log.WarnFormat("{0} selections", pool.Count);
            var account = new Account("lowbeta", pool);

            log.Info("run strategy");
            new strategies.LowBeta().Run(account);
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
