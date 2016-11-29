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

namespace Quant
{
    class Program
    {
        static ILog log = LogManager.GetLogger(typeof(Program));

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
                        log.InfoFormat("query data from sector {0}", string.IsNullOrEmpty(parameters.sector) ? "any" : parameters.sector);
                        var client = new MktDataClient();
                        var codes = client.Codes(parameters.sector ?? string.Empty);

                        log.Info("run selection");
                        var pool = new Trade.Selections.Impl.LowBeta().Pass(codes);
                        log.WarnFormat("{0} selections", pool.Count);

                        var quant = new LowBeta();
                        var account = new Interace.Quant.Account("lowbeta",pool);

                        log.Info("run strategy");
                        quant.Run(account);

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
        [Option('s', "sector", DefaultValue = "", Required = true)]
        public string sector { get; set; }
    }
}
