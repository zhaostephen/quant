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
            var code = _rawdb.Code(e.FullPath);
            log.InfoFormat("Make {0}", code);
            Make(code);
        }

        internal void Start()
        {
            log.Info("**********START**********");

            log.Info("Query codes");
            var codes = _rawdb.Codes();
            log.InfoFormat("GOT, total {0}", codes.Count());

            foreach (var code in codes.AsParallel())
            {
                log.InfoFormat("Make {0}", code);
                Make(code);
            }

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
        }

        private void Make(string code)
        {
            var dailyUpdate = _mktdb.LastUpdate(code, PeriodEnum.Daily);
            var monthlyUpdate = _mktdb.LastUpdate(code, PeriodEnum.Monthly);
            var weeklyUpdate = _mktdb.LastUpdate(code, PeriodEnum.Weekly);
            var rawUpdate = _rawdb.LastUpdate(code);

            if (rawUpdate.HasValue
                && (dailyUpdate.HasValue && dailyUpdate.Value >= rawUpdate.Value)
                && (monthlyUpdate.HasValue && monthlyUpdate.Value >= rawUpdate.Value)
                && (weeklyUpdate.HasValue && weeklyUpdate.Value >= rawUpdate.Value))
            {
                log.WarnFormat("Ignore {0}, already updated", code);
                return;
            }

            var dataset = 
                new[] { _rawdb.Query(code, PeriodEnum.Daily) }
                .Where(p => p != null)
                .ToArray();

            if (!dataset.Any()) return;

            log.Info("daily");
            var daily = dataset;
            _mktdb.Save(daily, PeriodEnum.Daily);

            log.Info("weekly");
            var weekly = dataset.MakeWeek();
            _mktdb.Save(weekly, PeriodEnum.Weekly);

            log.Info("monthly");
            var monthly = dataset.MakeMonth();
            _mktdb.Save(monthly, PeriodEnum.Monthly);
        }
    }
}
