using Cli;
using CommandLine;
using Interace.Quant;
using Interface.Quant;
using log4net;
using System.Linq;
using Trade;

namespace Quant.commands
{
    [command("strategy")]
    class strategycmd : command<strategycmd.parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(strategycmd));

        public strategycmd(string[] args) : base(args) { }

        public override void exec()
        {
            log.Info("run strategy " + param.name.ToLower());

            log.Info("get codes");
            var universe = param.asector 
                ? new universe(param.universe, new Trade.Db.db().codes(param.universe).ToArray())
                : getUniverse(param.universe);
            log.Info("total " + universe.codes.Length);

            log.Info("get run");
            var pool = new StockPool(universe.codes);
            var account = new Account(param.name.ToLower(), pool);
            switch (param.name.ToLower())
            {
                case "kdj15min":
                    new strategies.KDJ15minStrategy().Run(account);
                    break;
            }

            log.Info("**********DONE**********");
        }

        private universe getUniverse(string name)
        {
            return new Trade.Db.db().universe(name);
        }

        public class parameters
        {
            [Option('n', "name", Required = true)]
            public string name { get; set; }
            [Option('u', "universe", DefaultValue = "default", Required = false)]
            public string universe { get; set; }
            [Option('a', "asector", DefaultValue = false, Required = false)]
            public bool asector { get; set; }
        }
    }
}
