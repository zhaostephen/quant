using System.Collections.Generic;
using System.Linq;
using Screen.Data;
using Screen.Utility;
using log4net;
using Topshelf.FileSystemWatcher;
using Screen.Db;
using Screen.Mixin;
using System.IO;
using System.Threading;
using Screen.Cfg;
using System.Threading.Tasks;

namespace Screen
{
    class ScreenService
    {
        static ILog log = typeof(ScreenService).Log();

        readonly MktDb _mktdb;
        readonly RawDb _rawdb;

        public ScreenService()
        {
            _mktdb = new MktDb();
            _rawdb = new RawDb();
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

            log.Info("Query codes");
            var codes = _rawdb.Codes();
            log.InfoFormat("GOT, total {0}", codes.Count());

            log.Info("Make days");
            Task.Factory.StartNew(() => MakeDays(codes));

            log.Info("Make minutes");
            Task.Factory.StartNew(() => MakeMinutes(codes)); 

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
                Make(code, PeriodEnum.Min_5, new[] { PeriodEnum.Min_5, PeriodEnum.Min_15, PeriodEnum.Min_30, PeriodEnum.Min_60 });
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
