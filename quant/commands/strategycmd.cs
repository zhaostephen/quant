using Cli;
using CommandLine;
using Interace.Quant;
using Interface.Quant;
using log4net;
using Quant.strategies.orders;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using Trade;
using Trade.backtest;

namespace Quant.commands
{
    [command("strategy")]
    class strategycmd : command<strategycmd.parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(strategycmd));

        public strategycmd(string[] args) : base(args) { }

        public override void exec()
        {
            var a = analytic.hitkeyprices();
            log.Info(a.ToCsv());
            return;

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
                case "macd":
                    new strategies.macdstrategy().Run(account);
                    break;
            }

            if(param.backtest)
            {
                log.Info("run back test");
                var client = new Trade.Db.db();
                log.InfoFormat("total {0}", account.universe.Count);
                var pnls = new List<pnl>();
                foreach (var stock in account.universe.AsParallel())
                {
                    log.InfoFormat("run {0}", stock.Code);
                    var k = client.kdata(stock.Code, "D");
                    if (k == null && !k.Any())
                    {
                        log.WarnFormat("empty data set for {0}", stock.Code);
                        continue;
                    }

                    var trades = account.Trades
                        .Where(p => p.Stock == stock.Code)
                        //.Where(p=>p.Date >= new DateTime(2016,9,1))
                        .OrderBy(p=>p.Date)
                        .ToArray();

                    var backtest = new backtesting(stock.Code, k.close(), trades);

                    if(backtest.pnl != null)
                        pnls.Add(backtest.pnl);
                }

                var format = "{0,-15}{1,-20}{2,10:N0}{3,10:N0}{4,10:N1}";
                log.InfoFormat(format, "code", "date", "value", "capital", "ratio%");
                foreach (var pnl in pnls)
                {
                    log.InfoFormat(format, pnl.code, pnl.date, pnl.value, pnl.capital, pnl.ratio);
                }
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
