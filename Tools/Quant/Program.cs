using Interace.Mixin;
using log4net;
using Quant.Strategy;
using ServiceStack;
using System.IO;
using System.Linq;
using System.Text;
using Trade;
using Trade.Cfg;
using Trade.Data;

namespace Quant
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            log.Warn("***********************START***********************");

            log.Info("run strategy");
            var s = new SectorStrategy();
            var obj = s.Run();

            var path = Path.Combine(Configuration.strategy.selection.EnsurePathCreated(), "板块.csv");
            log.Info("save to path "+path);
            File.WriteAllText(
                path,
                obj.ToCsv(),
                Encoding.UTF8);

            log.Warn("***********************DONE***********************");
        }
    }
}
