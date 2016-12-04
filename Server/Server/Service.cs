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
        readonly AttributionImpl _attribution;
        readonly CancellationTokenSource _cancel;

        public Service()
        {
            _mktdata = new MktDataImpl();
            _attribution = new AttributionImpl();
            _cancel = new CancellationTokenSource();
        }

        internal void Start(string command)
        {
            log.Info("**********START**********");

            switch (command.ToLower())
            {
                case "fundamental":
                    {
                        log.Info("Make basics");
                        var fundamentals = _mktdata.MakeFundametals();
                        log.InfoFormat("GOT, total {0}", fundamentals.Count());
                    }
                    break;
                case "keyprice":
                    {
                        log.Info("Query codes");
                        var codes = GetCodes();
                        log.InfoFormat("GOT, total {0}", codes.Count());

                        log.Info("Make key price");
                        _attribution.MakeKeyPrice(codes, Configuration.bearcrossbull);
                    }
                    break;
                case "days":
                    {
                        log.Info("Query codes");
                        var codes = GetCodes();
                        log.InfoFormat("GOT, total {0}", codes.Count());

                        log.Info("Make days");
                        _mktdata.MakeDays(codes);
                    }
                    break;
                case "minutes":
                    {
                        log.Info("Query codes");
                        var codes = GetCodes();
                        log.InfoFormat("GOT, total {0}", codes.Count());

                        log.Info("Make minutes");
                        _mktdata.MakeMinutes(codes);
                    }
                    break;
                case "mktdata":
                    {
                        log.Info("Query codes");
                        var codes = GetCodes();
                        log.InfoFormat("GOT, total {0}", codes.Count());

                        log.Info("Make mkt data");
                        var tasks = _mktdata.MakeAsync(codes);

                        try
                        {
                            Task.WaitAll(tasks, _cancel.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            log.Warn("Cancelled");
                        }
                    }
                    break;
            }

            log.Info("**********DONE**********");
        }

        internal void Start(Tuple<int?, int?> range = null)
        {
            log.Info("**********START**********");

            log.Info("Make fundamental");
            var fundamentals = _mktdata.QueryFundametals();
            log.InfoFormat("GOT, total {0}", fundamentals.Count());

            log.Info("Query codes");
            var codes = GetCodes(range, fundamentals);
            log.InfoFormat("GOT, total {0}", codes.Count());

            log.Info("Make mkt data");
            var tasks = _mktdata.MakeAsync(codes);

            try
            {
                Task.WaitAll(tasks, _cancel.Token);
            }
            catch (OperationCanceledException)
            {
                log.Warn("Cancelled");
            }

            log.Info("**********DONE**********");
        }

        private string[] GetCodes(Tuple<int?, int?> range = null, IEnumerable<Fundamental> fundamentals =null)
        {
            fundamentals = fundamentals ?? _mktdata.QueryFundametals();
            if (range != null && range.Item1.HasValue && range.Item2.HasValue)
            {
                fundamentals = fundamentals.Skip(range.Item1.Value).Take(range.Item2.Value - range.Item1.Value).ToArray();
                log.WarnFormat("********Range {0}-{1}********", range.Item1, range.Item2);
            }
            log.InfoFormat("GOT, total {0}", fundamentals.Count());

            var codes = fundamentals.Select(p => p.代码).Distinct().ToArray();
            
            return codes;
        }

        internal void Stop()
        {
            _cancel.Cancel(false);
        }
    }
}
