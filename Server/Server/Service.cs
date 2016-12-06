using System.Collections.Generic;
using System.Linq;
using Trade.Data;
using Trade.Utility;
using log4net;
using System.Threading;
using Trade.Cfg;
using System.Threading.Tasks;
using System;
using Trade.Impl;

namespace Trade
{
    class Service
    {
        static ILog log = typeof(Service).Log();

        readonly Impl.kdatas1_make _mktdata;
        readonly keyprice_make _attribution;

        public Service()
        {
            _mktdata = new Impl.kdatas1_make();
            _attribution = new keyprice_make();
        }

        internal void Start(string command)
        {
            log.Info("**********START**********");

            switch (command.ToLower())
            {
                case "keyprice":
                    {
                        log.Info("Make key price");
                        new keyprice_make().@do(Configuration.bearcrossbull);
                    }
                    break;
                case "mktdata":
                    {
                        log.Info("Make mkt data");
                        new kdatas1_make().@do();
                    }
                    break;
            }

            log.Info("**********DONE**********");
        }
    }
}
