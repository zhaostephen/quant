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
using Trade.Indicator;
using Trade.Data;

namespace Web.Controllers.Api
{
    public class kdataController : ApiController
    {
        [Route("api/kdata/{id}")]
        public chart Get(string id, string ktype)
        {
            var kdata = new Trade.Db.db().kdata(id, ktype);
            var basic = new Trade.Db.db().basics(id);
            var since = Trade.Cfg.Configuration.data.bearcrossbull;
            var k = kdata.Where(p => p.date >= since).ToArray();
            var macd = new MACD(kdata.close()).ToDictionary(p => p.Date, p => p.MACD);
            return new chart {
                data = k.Select(p => new object[] { p.date, p.open, p.high, p.low, p.close }).ToArray(),
                volume = k.Select(p => new object[] { p.date, p.volume }).ToArray(),
                code = kdata.Code,
                name = basic.name,
                keyprices = keyprice(k, id, ktype)
            };
        }

        KeyPrice[] keyprice(IEnumerable<kdatapoint> k, string id, string ktype)
        {
            var keyprices = Trade.analytic.keyprice(id, ktype);
            if (k.Any())
            {
                var cur = new[]
                {
                    KeyPrice.low(id, k.Last().date, k.Last().low, true),
                    KeyPrice.high(id, k.Last().date, k.Last().high, true)
                };
                keyprices = keyprices.Concat(cur).ToArray();
            }
            return keyprices;
        }
    }

    public class chart
    {
        public KeyPrice[] keyprices { get; set; }
        public object[][] data { get; set; }
        public object[][] volume { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}