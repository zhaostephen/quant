using CommandLine;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Data;
using Trade.Utility;

namespace Cli
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                help();
                return;
            }

            commandfactory.exec(args[0], args.Skip(1).ToArray());
        }

        static void help()
        {
            log.Warn(Assembly.GetExecutingAssembly().GetName().Name.Replace(".exe", "") + " <command> <param>");
        }
    }
}
