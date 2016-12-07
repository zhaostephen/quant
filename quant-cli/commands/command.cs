using CommandLine;
using Interace.Mixin;
using Interace.Quant;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade;
using Trade.Cfg;

namespace Quant.commands
{
    interface ICommand
    {
        void exec();
    }

    abstract class command<Tparam> : ICommand where Tparam : new()
    {
        static ILog log = LogManager.GetLogger(typeof(lowbetacmd));

        protected Tparam param;

        public command(string[] args)
        {
            param = new Tparam();
            if (!Parser.Default.ParseArguments(args, param))
            {
                log.Warn("quant-cli <command> <param>");
                return;
            }
        }

        public abstract void exec();

        protected IEnumerable<string> codes(string sector)
        {
            log.InfoFormat("query codes from sector {0}", string.IsNullOrEmpty(sector) ? "any" : sector);
            var client = new client();
            return client.codes(sector ?? string.Empty);
        }

        protected void save(Account account)
        {
            log.WarnFormat("{0} trades", account.Trades.Count);
            if (account.Trades.Any())
            {
                log.Info("save down trades");
                var path = Configuration.data.trade.EnsurePathCreated();

                File.WriteAllText(Path.Combine(path, DateTime.Today.ToString("yyyy-MM-dd") + ".csv"), account.Trades.ToCsv(), Encoding.UTF8);
            }
        }
    }

    class commandfactory
    {
        static ILog log = LogManager.GetLogger(typeof(commandfactory));

        public static void exec(string command, string[] args)
        {
            log.Info("exec " + command);

            switch (command.ToLower())
            {
                case "ma120":
                    {
                        new ma120cmd(args).exec();
                        break;
                    }
                case "lowbeta":
                    {
                        new lowbetacmd(args).exec();
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
