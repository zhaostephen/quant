using System.Collections.Generic;
using System.Linq;
using Trade.Utility;
using log4net;
using Trade.Db;
using Trade.Cfg;
using System;
using Interace.Idx;

namespace Trade
{
    class Service
    {
        static ILog log = typeof(Service).Log();

        readonly MktDb _mktdb;
        readonly RawDb _rawdb;

        public Service()
        {
            _mktdb = new MktDb();
            _rawdb = new RawDb();
        }

        internal void Start()
        {
            log.Info("**********START**********");

            log.Info("Make fundamental");
            var fundamentals = _rawdb.QueryFundamentals();
            _mktdb.Save(fundamentals);
            log.InfoFormat("GOT, total {0}", fundamentals.Count());

            log.Info("Query codes");
            var codes = _rawdb.Codes();
            log.InfoFormat("GOT, total {0}", codes.Count());

            log.Info("Build index");
            Build(codes);

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
        }

        private void Build(IEnumerable<string> codes)
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
