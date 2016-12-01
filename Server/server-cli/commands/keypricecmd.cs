using Cli;
using CommandLine;
using Interace.Attribution;
using Interace.Mixin;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Data;
using Trade.Mixin;

namespace Trade.Cli.commands
{
    [command("keyprice")]
    class keypricecmd : command<keypricecmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(fundamentalcmd));
        readonly subcommandenum subcommand;

        public keypricecmd(string[] args)
        {
            switch (args[0].ToLower())
            {
                case "add":
                    {
                        subcommand = subcommandenum.add;
                        param = new Parameters();
                        if (!Parser.Default.ParseArguments(args.Skip(1).ToArray(), param))
                        {
                            help();
                            return;
                        }

                        break;
                    }
                default:
                    {
                        param = new Parameters();
                        if (!Parser.Default.ParseArguments(args, param))
                        {
                            help();
                            return;
                        }

                        break;
                    }
            }
        }

        public override void exec()
        {
            switch(subcommand)
            {
                case subcommandenum.add:
                    {
                        var splits = param.price.Split(new[] {',' }, StringSplitOptions.RemoveEmptyEntries);
                        var keyprice = new KeyPrice()
                        {
                            Code = param.code,
                            Auto = false,
                            Date = splits[0].Date(),
                            Price = splits[1].Double(),
                            Flag = splits[2]
                        };
                        SaveKeyPrices(new[] { keyprice });

                        break;
                    }
                default:
                    {
                        var client = new AttributionClient();
                        var f = client.QueryKeyPrices(param.code).Where(p => p != null).ToArray();
                        log.Info(f.ToCsv());
                        break;
                    }
            }
        }

        public void SaveKeyPrices(IEnumerable<KeyPrice> keyPrices)
        {
            var prices = new AttributionClient().QueryKeyPrices();

            foreach (var price in keyPrices)
            {
                var found = prices.SingleOrDefault(p => p.Code == price.Code && p.Date == price.Date);
                if (found == null)
                    prices = prices.Concat(new[] { price }).ToArray();
                else
                {
                    found.Price = price.Price;
                    found.Auto = price.Auto;
                    found.Flag = price.Flag;
                }
            }

            File.WriteAllText(Path(), prices.ToCsv(), Encoding.UTF8);
        }

        private string Path()
        {
            return System.IO.Path.Combine(Configuration.attribution.root.EnsurePathCreated(), "keyprice.csv");
        }

        enum subcommandenum
        {
            none,
            add
        }
        public class Parameters
        {
            [Option('c', "code", Required = true)]
            public string code { get; set; }
            [Option('p', "price", Required = false)]
            public string price { get; set; }
        }
    }
}
