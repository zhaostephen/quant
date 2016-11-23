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
using Interace.Mixin;
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

            log.Info("Build index");
            _idxService.Build(codes);

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
        }
    }
}
