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
    class lowbetacmd : command<Parameters>
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

            save(account);
        }
    }
}
