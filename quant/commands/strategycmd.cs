using Cli;
using CommandLine;
using Interace.Quant;
using Interface.Quant;
using log4net;
using Quant.strategies.orders;
using System;
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
            universe universe;
            if (param.astock)
            {
                var codes =
                    param.universe.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                universe = new universe(param.universe, codes);
            }
            else if (param.asector)
                universe = new universe(param.universe, new Trade.Db.db().codes(param.universe).ToArray());
            else
                universe = getUniverse(param.universe);

            log.Info("total " + universe.codes.Length);

            log.Info("get run");
            var pool = new StockPool(universe.codes);
            var orders = !param.backtest 
                ? new IOrder[] { new dbOrder(), new smsOrder() }
                : new IOrder[] { new dbOrder()};
            var portflio = (param.portflio ?? param.name.ToLower()) +  (param.backtest ? "-backtest" : "");
            var account = new Account(portflio, pool, orders, param.backtest);
            switch (param.name.ToLower())
            {
                case "kdj15min":
                    new strategies.kdj15minstrategy().Run(account);
                    break;
                case "macd15min":
                    new strategies.macd15minstrategy().Run(account);
                    break;
                case "volume":
                    new strategies.volumestrategy().Run(account);
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
            [Option('p', "portflio", DefaultValue = null, Required = false)]
            public string portflio { get; set; }
            [Option('u', "universe", DefaultValue = "default", Required = false)]
            public string universe { get; set; }
            [Option('a', "asector", DefaultValue = false, Required = false)]
            public bool asector { get; set; }
            [Option('s', "astock", DefaultValue = false, Required = false)]
            public bool astock { get; set; }
            [Option('b', "backtest", DefaultValue = false, Required = false)]
            public bool backtest { get; set; }

        }
    }
}
