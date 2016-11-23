using Interace.Idx;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Db;
using Trade.Utility;

namespace Trade.Services
{
    public class IdxService
    {
        static readonly ILog log = typeof(IdxService).Log();

        private readonly RawDb _rawdb;
        private readonly MktDb _mktdb;

        internal IdxService(RawDb rawDb, MktDb mktDb)
        {
            _rawdb = rawDb;
            _mktdb = mktDb;
        }

        public void Build(IEnumerable<string> codes)
        {
            log.Info("build raw index");
            var rawIndx = new Interace.Idx.Index();
            rawIndx.LastUpdate = BuildLastUpdateIndex(codes,
                new[] { PeriodEnum.Daily, PeriodEnum.Min5 },
                (code, period) => _rawdb.LastUpdate(code, period));
            _rawdb.SaveIdx(rawIndx);

            log.Info("build mkt index");
            var mktIndx = new Interace.Idx.Index();
            mktIndx.LastUpdate = BuildLastUpdateIndex(codes,
                new[] { PeriodEnum.Daily, PeriodEnum.Weekly, PeriodEnum.Monthly, PeriodEnum.Min5, PeriodEnum.Min15, PeriodEnum.Min30, PeriodEnum.Min60 },
                (code, period) => _mktdb.LastUpdate(code, period));
            _mktdb.SaveIdx(mktIndx);
        }

        private static LastUpdateIdx[] BuildLastUpdateIndex(IEnumerable<string> codes, PeriodEnum[] periods, Func<string, PeriodEnum, DateTime?> get)
        {
            return codes.AsParallel()
                .SelectMany(code =>
                    periods.AsParallel().Select(period =>
                    {
                        var dt = get(code, period);
                        if (dt == null) return null;
                        return new LastUpdateIdx()
                        {
                            Code = code,
                            Period = period.ToString(),
                            Date = dt.Value.ToString("yyyy-MM-dd")
                        };
                    })
                        .ToArray())
                .Where(p => p != null)
                .ToArray();
        }
    }
}
