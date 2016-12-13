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
using Trade.Indicator;

namespace Web.Controllers.Api
{
    public class analyticController : ApiController
    {
        [Route("api/analytic/{id}/{ktype}")]
        public analytic Get(string id, string ktype)
        {
            var result = new analytic();

            var k = new Trade.Db.db().kdata(id, ktype);
            var basic = new Trade.Db.db().basics(id);

            result.code = basic.code;
            result.name = basic.name;

            if(k != null && k.Any())
            {
                var cur = k.Last();
                var prev = k.Count > 1 ? k[k.Count - 2] : null;

                result.date = cur.date.ToString("yyyy-MM-dd");
                result.high = cur.high;
                result.low = cur.close;
                result.open = cur.open;
                result.close = cur.close;
                result.change = prev == null ? (double?)null : ((cur.close - prev.close) / prev.close) * 100;

                var q = (quotation)new QUOTATION(k);
                if (q != null)
                {
                    result.state = q.state.ToString();
                    result.position = q.position;
                    result.strategy = q.strategy;
                }

                var deviation = (deviation)new DEVIATION(k);
                if(deviation != null && deviation.d2.Date == cur.date.Date)
                {
                    if(ktype == "5" || ktype == "15" || ktype == "30" || ktype == "60")
                        result.deviation = string.Format("{0:MM/dd HH:mm} ~ {1:MM/dd HH:mm}({2})", deviation.d1, deviation.d2, deviation.cross);
                    else
                        result.deviation = string.Format("{0:MM/dd} ~ {1:MM/dd}({2})", deviation.d1, deviation.d2, deviation.cross);
                }

                var ma = new List<string>();
                var close = k.close();
                var ma5 = (double?)new MA(close, 5);
                var ma30 = (double?)new MA(close, 30);
                var ma55 = (double?)new MA(close, 55);
                var ma120 = (double?)new MA(close, 120);
                if (ma5 >= ma30 && ma30 >= ma55 && ma55 >= ma120)
                    ma.Add("多头");
                if (cur.close < ma5)
                    ma.Add("破5日线");
                if (cur.close < ma30)
                    ma.Add("破30日生命线");
                if (cur.close < ma55)
                    ma.Add("破55日生命线");
                if (cur.close < ma120)
                    ma.Add("破半年线");
                if (ma.Any())
                    result.ma = string.Join(",",ma);
            }

            var mainindex = basic.mainindex();
            if (!string.IsNullOrEmpty(mainindex))
            {
                var dindex = new Trade.Db.db().kdata(mainindex, ktype);
                result.beta = new BETA(
                    new kdata(id, k.Where(p => p.date >= Trade.Cfg.Configuration.data.bearcrossbull).ToArray()),
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
        public double? change { get; set; }
        public string deviation { get; set; }
        public string ma { get; set; }
    }
}