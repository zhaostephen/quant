using CommandLine;
using Interace.Mixin;
using log4net;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Trade;
using Trade.Cfg;
using Trade.Strategies.Impl;
using Trade.Strategies.Utility;

namespace Quant
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            if (args.Length < 2)
            {
                log.Warn("quant-cli <command> <param>");
                return;
            }

            var command = args[0];

            var parameters = new Parameters();
            if (!Parser.Default.ParseArguments(args.Skip(1).ToArray(), parameters))
            {
                log.Warn("quant-cli <command> <param>");
                return;
            }

            log.Info(command);
            switch (command.ToLower())
            {
                case "lowbeta":
                    {
                        var s = new LowBeta();
                        var pool = new Interace.Quant.StockPool();
                        var account = new Interace.Quant.Account("lowbeta",pool);

                        log.Info("run strategy");
                        s.Run(account);

                        log.WarnFormat("{0} trades", account.Trades.Count);
                        if (account.Trades.Any())
                        {
                            log.Info("save down selections");
                            var path = Configuration.oms.trade.EnsurePathCreated();

                            File.WriteAllText(Path.Combine(path, DateTime.Today.ToString("yyyy-MM-dd")+".csv"), account.Trades.ToCsv(), Encoding.UTF8);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    class Parameters
    {
        [Option('p', "pool", DefaultValue = "", Required = true)]
        public string pool { get; set; }
    }
}
