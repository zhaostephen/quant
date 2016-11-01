using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screen.Mixin;

namespace Screen
{
    class Arguments
    {
        [Option("date", Required = false, DefaultValue = null, HelpText = "untill stock date")]
        public DateTime? Date { get; set; }

        public static Arguments Parse(string[] args)
        {
            var arguments = new Arguments();
            if (!Parser.Default.ParseArguments(args, arguments))
            {
                var help = CommandLine.Text.HelpText.AutoBuild(arguments);
                Console.WriteLine(help.ToString());
                return null;
            }
            arguments.Date = arguments.Date ?? DateTime.Today.TradingDay(0);
            return arguments;
        }
    }
}
