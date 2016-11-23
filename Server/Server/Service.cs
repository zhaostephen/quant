using System.Collections.Generic;
using System.Linq;
using Trade.Data;
using Trade.Utility;
using log4net;
using Topshelf.FileSystemWatcher;
using Trade.Db;
using Trade.Mixin;
using System.IO;
using System.Threading;
using Trade.Cfg;
using System.Threading.Tasks;
using System;
using Interace.Idx;
using Trade.Services;

namespace Trade
{
    class Service
    {
        static ILog log = typeof(Service).Log();

        readonly MktDb _mktdb;
        readonly RawDb _rawdb;
        readonly IdxService _idxService;

        public Service()
        {
            _mktdb = new MktDb();
            _rawdb = new RawDb();
            _idxService = new IdxService(_rawdb, _mktdb);
        }

        internal void FileChange(TopshelfFileSystemEventArgs e)
        {
            //var code = _rawdb.Code(e.FullPath);
            //log.InfoFormat("Make {0}", code);
            //Make(code);
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

            log.Info("Make days");
            var t1 = Task.Factory.StartNew(() => MakeDays(codes));

            log.Info("Make minutes");
            var t2 = Task.Factory.StartNew(() => MakeMinutes(codes));

            Task.WaitAll(new[] { t1, t2 });

            log.Info("Build index");
            _idxService.Build(codes);

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
        }

        private void MakeDays(IEnumerable<string> codes)
        {
            var i = 0;
            var total = codes.Count();
            foreach (var code in codes.AsParallel())
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} - make days - {2}",i, total , code);
                Make(code, PeriodEnum.Daily, new[] { PeriodEnum.Daily, PeriodEnum.Weekly, PeriodEnum.Monthly });
            }
        }

        private void MakeMinutes(IEnumerable<string> codes)
        {
            var i = 0;
            var total = codes.Count();
            foreach (var code in codes.AsParallel())
            {
                Interlocked.Increment(ref i);
                log.InfoFormat("{0}/{1} - make minutes - {2}", i, total, code);
                Make(code, PeriodEnum.Min5, new[] { PeriodEnum.Min5, PeriodEnum.Min15, PeriodEnum.Min30, PeriodEnum.Min60 });
            }
        }

        private void Make(string code, PeriodEnum rawPeriod, PeriodEnum[] followings)
        {
            var rawUpdate = _rawdb.LastUpdate(code, rawPeriod);
            var followingUpdates = followings.Select(p => _mktdb.LastUpdate(code, p)).ToArray();

            if (rawUpdate.HasValue && followingUpdates.All(p => p.HasValue && p.Value >= rawUpdate.Value))
            {
                log.WarnFormat("Ignore {0}, already updated", code);
                return;
            }

            var dataset = 
                new[] { _rawdb.Query(code, rawPeriod) }
                .Where(p => p != null)
                .ToArray();

            if (!dataset.Any()) return;

            foreach (var following in followings.AsParallel())
            {
                log.Info(following);
                var another = dataset.Make(rawPeriod, following);
                _mktdb.Save(another, following);
            }
        }
    }
}
