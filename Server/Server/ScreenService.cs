using System.Collections.Generic;
using System.Linq;
using Screen.Data;
using Screen.Utility;
using log4net;
using Topshelf.FileSystemWatcher;
using Screen.Db;
using Screen.Mixin;

namespace Screen
{
    class ScreenService
    {
        static ILog log = typeof(ScreenService).Log();

        readonly FileSystemWatcherConfigurator _fs;
        readonly MktDb _mktdb;

        public ScreenService()
        {
            _fs = new FileSystemWatcherConfigurator();
            _mktdb = new MktDb();
        }

        internal void Initialize()
        {
            _fs.AddDirectory((cfg) => { cfg.Path = @"D:\screen\Data"; cfg.FileFilter = "*.txt"; cfg.IncludeSubDirectories = false; });
        }

        internal void FileChange(TopshelfFileSystemEventArgs e)
        {
            log.Info("make  | " + e.FullPath);

            var data = _mktdb.Query(e.FullPath);
            Make(new[] { data });

            log.Info("make complete | " + e.FullPath);
        }

        internal void Start()
        {
            log.Info("**********START**********");

            log.Info("raw");
            var dataset = _mktdb.QueryMany(@"D:\screen\Data");

            log.Info("roll");
            Make(dataset);

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
        }

        private void Make(IEnumerable<StkDataSeries> dataset)
        {
            dataset = dataset.Where(p => p != null).ToArray();

            log.Info("daily");
            var daily = dataset;
            _mktdb.SaveMany(daily, @"D:\screen\Data\daily");

            log.Info("weekly");
            var weekly = dataset.MakeWeek();
            _mktdb.SaveMany(weekly, @"D:\screen\Data\week");

            log.Info("monthly");
            var monthly = dataset.MakeMonth();
            _mktdb.SaveMany(monthly, @"D:\screen\Data\month");
        }
    }
}
