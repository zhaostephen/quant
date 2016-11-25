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
            var s = new LowBetaStrategy();
            s.Run(Sector.券商信托);

            log.Warn("***********************DONE***********************");
        }
    }
}
