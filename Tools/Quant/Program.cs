using Interace.Mixin;
using log4net;
using Quant.Strategies;
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
            var s = new BackMA120Strategy();
            s.Run(StrategyIn.Default);
            //var s = new LowBetaStrategy(junxianduotou:false, beta:0.5);
            //s.Run(StrategyIn.Default);
            //var s = new LowPEStrategy();
            //s.Run(StrategyIn.Default);

            log.Warn("***********************DONE***********************");
        }
    }
}
