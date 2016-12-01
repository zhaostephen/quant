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
using Trade.Impl;

namespace Trade
{
    class Service
    {
        static ILog log = typeof(Service).Log();

        readonly MktDataImpl _mktdata;
        readonly CancellationTokenSource _cancel;

        public Service()
        {
            _mktdata = new MktDataImpl();
            _cancel = new CancellationTokenSource();
        }

        internal void Start(Tuple<int?, int?> range)
        {
            log.Info("**********START**********");

            var fundamentals = _mktdata.MakeFundametals();
            log.InfoFormat("GOT, total {0}", fundamentals.Count());

            var tasks = _mktdata.MakeAsync(range);

            try
            {
                Task.WaitAll(tasks, _cancel.Token);
            }
            catch(OperationCanceledException)
            {
                log.Warn("Cancelled");
            }

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
            _cancel.Cancel(false);
        }
    }
}
