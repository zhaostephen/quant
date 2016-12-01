using Cli;
using CommandLine;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Data;

namespace Trade.Cli.commands
{
    [command("fundamental","basic")]
    class fundamentalcmd : command<fundamentalcmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(fundamentalcmd));

        public fundamentalcmd(string[] args) : base(args) { }

        public override void exec()
        {
            var client = new MktDataClient();
            var f = new[] { client.QueryFundamental(param.code) }.Where(p => p != null).ToArray();
            log.Info(f.ToCsv());
        }

        public class Parameters
        {
            [Option('c', "code", Required = true)]
            public string code { get; set; }
        }
    }
}
