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
    [command("basics")]
    class basicscmd : command<basicscmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(basicscmd));

        public basicscmd(string[] args) : base(args) { }

        public override void exec()
        {
            var client = new Trade.Db.db();
            var f = new[] { client.basics(param.code) }.Where(p => p != null).ToArray();
            log.Info(f.ToCsv());
        }

        public class Parameters
        {
            [Option('c', "code", Required = true)]
            public string code { get; set; }
        }
    }
}
