using CommandLine;
using Interace.Mixin;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Trade;
using Trade.Cfg;
using Trade.Data;
using Trade.Selections;
using Trade.Selections.Impl;

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
                log.Warn("selection-cli <command> <param>");
                return;
            }

            var command = args[0];

            var parameters = new Parameters();
            if (!Parser.Default.ParseArguments(args.Skip(1).ToArray(), parameters))
            {
                log.Warn("selection-cli <command> <param>");
                return;
            }

            switch (command.ToLower())
            {
                case "lbv":
                case "lowbeta":
                case "lowbetavalue":
                    {
                        log.InfoFormat("query data from sector {0}", string.IsNullOrEmpty(parameters.sector) ? "any" : parameters.sector);
                        var client = new MktDataClient();
                        var codes = client.Codes(parameters.sector??string.Empty);

                        log.Info("run selection");
                        var o = new LowBeta().Pass(codes);
                        log.WarnFormat("{0} selections", o.Count);
                        if (o.Any())
                        {
                            log.Info("save down selections");
                            var path = Path.Combine(parameters.output ?? Configuration.oms.selection, DateTime.Today.ToString("yyyy-MM-dd")).EnsurePathCreated();

                            File.WriteAllLines(Path.Combine(path, "lowbetavalue"), o.Codes, Encoding.UTF8);
                            File.WriteAllText(Path.Combine(path, "lowbetavalue+a.csv"), o.Attributes.ToCsv(), Encoding.UTF8);
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
        [Option('s', "sector", DefaultValue = "", Required = false)]
        public string sector { get; set; }
        [Option('o', "output", DefaultValue = null, Required = false)]
        public string output { get; set; }
    }
}
