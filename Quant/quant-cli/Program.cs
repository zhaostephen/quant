using log4net;
using Quant.commands;
using ServiceStack;
using System.Linq;

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

            commandfactory.exec(args[0], args.Skip(1).ToArray());
        }
    }
}
