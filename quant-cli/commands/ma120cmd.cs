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
    class ma120cmd : command<Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(lowbetacmd));

        public ma120cmd(string[] args) : base(args) { }

        public override void exec()
        {
            var pool = new StockPool(codes(param.sector));
            var account = new Account("ma120", pool);

            log.Info("run strategy");
            new Trade.Strategies.Impl.MA120().Run(account);

            save(account);
        }
    }
}
