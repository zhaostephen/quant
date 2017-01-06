using Cli;
using log4net;
using Pinyin4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Mixin;

namespace Trade.commands
{
    [command("basics")]
    class basicscmd : command<object>
    {
        static ILog log = typeof(basicscmd).Log();

        public basicscmd(string[] args)
            :base(args)
        {

        }
        public override void exec()
        {
            log.Info("**********START**********");

            new datasvc().basics();

            log.Info("**********DONE**********");
        }
    }
}
