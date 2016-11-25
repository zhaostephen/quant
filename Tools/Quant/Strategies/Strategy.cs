using Interace.Mixin;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade;
using Trade.Cfg;
using Trade.Data;
using Trade.Factors;

namespace Quant.Strategies
{
    class StrategyIn
    {
        public static StrategyIn Default = new StrategyIn();

        public string sector { get; set; }
        public PeriodEnum period { get; set; }

        public StrategyIn()
        {
            sector = Sector.板块指数除外;
            period = PeriodEnum.Daily;
        }
    }

    abstract class Strategy
    {
        static ILog log = typeof(Strategy).Log();

        public abstract string Name { get; }
        protected abstract IEnumerable<object> InternalRun(IEnumerable<StkDataSeries> data, IEnumerable<Fundamental> basics, StrategyIn parameters);

        protected MktDataClient client;

        public Strategy()
        {
            client = new MktDataClient();
        }

        public void Run(StrategyIn parameters)
        {
            log.Info("query market data");
            var data = client.QueryAll(parameters.period, parameters.sector);
            log.InfoFormat("total {0}", data.Count());

            log.Info("query fundamentals");
            var basics = client.QueryFundamentals(data.Select(p => p.Code).Distinct().ToArray());

            log.Info("do strategy");
            var result = InternalRun(data, basics, parameters);

            log.Info("save results");
            var path = Path.Combine(Configuration.oms.strategy, DateTime.Today.ToString("yyyy-MM-dd"));
            path = Path.Combine(path.EnsurePathCreated(), Name + ".csv");
            log.Info("save to path " + path);
            File.WriteAllText(
                path,
                result.ToCsv(),
                Encoding.UTF8);
        }

        protected bool BuyOrSell(string code)
        {
            var Min15 = client.Query(code, PeriodEnum.Min15);
            var Min30 = client.Query(code, PeriodEnum.Min30);
            var Min60 = client.Query(code, PeriodEnum.Min60);

            return Min15.Last().MACD >= 0 &&
                Min30.Last().MACD >= 0 &&
                Min60.Last().MACD >= 0;

        }
    }
}
