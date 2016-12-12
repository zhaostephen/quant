using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Trade;
using Interace.Attribution;
using Trade.Cfg;
using Interface.Indicator;
using Trade.Data;

namespace Web.Controllers.Api
{
    public class analyticController : ApiController
    {
        [Route("api/analytic/{id}/{ktype}")]
        public analytic Get(string id, string ktype)
        {
            var result = new analytic();

            var d = new Trade.Db.db().kdata(id, ktype);
            var basic = new Trade.Db.db().basics(id);

            result.code = basic.code;
            result.name = basic.name;

            if(d != null && d.Any())
            {
                var cur = d.Last();
                result.date = cur.date.ToString("yyyy-MM-dd");
                result.high = cur.high;
                result.low = cur.close;
                result.open = cur.open;
                result.close = cur.close;
                result.state = cur.state;
                result.position = cur.position;
                result.strategy = cur.strategy;
                if (string.IsNullOrEmpty(cur.state))
                {
                    var i = new QUOTATION(d);
                    if (i.Any())
                    {
                        result.state = i.Last().state.ToString();
                        result.position = i.Last().position;
                        result.strategy = i.Last().strategy;
                    }
                }
            }

            var mainindex = basic.mainindex();
            if (!string.IsNullOrEmpty(mainindex))
            {
                var dindex = new Trade.Db.db().kdata(mainindex, ktype);
                result.beta = new BETA(
                    new kdata(id, d.Where(p => p.date >= Trade.Cfg.Configuration.data.bearcrossbull).ToArray()),
                    new kdata(id, dindex.Where(p => p.date >= Trade.Cfg.Configuration.data.bearcrossbull).ToArray())).beta;
            }

            return result;
        }
    }

    public class analytic
    {
        public string code { get; set; }
        public string name { get; set; }
        public double? beta { get; set; }
        public string date { get; set; }
        public double? open { get; set; }
        public double? high { get; set; }
        public double? low { get; set; }
        public double? close { get; set; }
        public string state { get; set; }
        public double? position { get; set; }
        public string strategy { get; set; }
        public double? change { get { return ((close - open) / open) * 100; } }
    }
}