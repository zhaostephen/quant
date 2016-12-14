using System;
using Cli;
using CommandLine;
using Interface.Quant;
using log4net;
using Trade;
using Trade.selections;
using Trade.Data;

namespace Quant.commands
{
    [command("selection")]
    class selectioncmd : command<selectioncmd.parameters>
    {
        static ILog log = typeof(selectioncmd).Log();

        public selectioncmd(string[] args)
            :base(args)
        {
        }

        public override void exec()
        {
            log.Info("**********START**********");

            switch(param.name.ToLower())
            {
                case "macd60":
                    {
                        var client = new Trade.Db.db();
                        var universe = new macd60().Pass(client.codes(param.sector));
                        client.save(universe);
                    }
                    break;
                case "volume":
                    {
                        var client = new Trade.Db.db();
                        var universe = new volume().Pass(client.codes(param.sector));
                        client.save(universe);
                    }
                    break;
            }

            log.Info("**********DONE**********");
        }

        public class parameters
        {
            [Option('n', "name", Required = true)]
            public string name { get; set; }
            [Option('s', "sector", DefaultValue =assettypes.stock, Required = false)]
            public string sector { get; set; }
        }
    }
}
