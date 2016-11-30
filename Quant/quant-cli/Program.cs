using CommandLine;
using Interace.Mixin;
using Interace.Quant;
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
            var s = new QuantService();
            switch (command.ToLower())
            {
                case "ma120":
                    {
                        s.ma120(parameters.sector);
                        break;
                    }
                case "lowbeta":
                    {
                        s.LowBeta(parameters.sector);
                        break;
                    }
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
