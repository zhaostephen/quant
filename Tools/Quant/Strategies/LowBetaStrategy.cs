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
    class LowBetaStrategy : Strategy
    {
        static ILog log = typeof(LowBetaStrategy).Log();

        double benchmark = 20;
        double beta = 0.5;
        bool junxianduotou = false;

        public override string Name { get { return "LowBeta"; } }

        public LowBetaStrategy(bool junxianduotou = false, double beta=0.5)
        {
            this.junxianduotou = junxianduotou;
            this.beta = beta;
        }

        protected override IEnumerable<object> InternalRun(IEnumerable<StkDataSeries> data, IEnumerable<Fundamental> basics, StrategyIn parameters)
        {
            var stat = data
                .Select(series => new factorset(series.Code)
                {
                    收阳百分比 = new close_up_percent(series, TimeSpan.FromDays(180)).value,
                    均线多头 = new jun_xian_dou_tout(series).value,
                    低点反弹高度 = new low_to_historical_lowest(series, new DateTime(2015, 5, 1)).value
                })
                .ToArray();

            if (junxianduotou)
                stat = stat.Where(p => p.均线多头).ToArray();

            stat = stat
                .Where(p => p.低点反弹高度 < benchmark * beta)
                .OrderBy(p => p.低点反弹高度)
                .ToArray();

            var q = from s in stat
                    join b in basics on s.代码 equals b.代码
                    select new
                    {
                        s.代码,
                        b.名称,
                        s.收阳百分比,
                        s.低点反弹高度,
                        s.均线多头,
                        //短线买入 = BuyOrSell(s.代码) ? "买入" : "卖出",
                        b.市盈率,
                        b.总市值,
                        b.所属行业,
                        b.流通市值,
                        b.营业总收入同比,
                        b.销售毛利率,
                        b.净利润同比
                    };

            return q.ToArray();
        }
    }
}
