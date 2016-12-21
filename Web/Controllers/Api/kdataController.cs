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

namespace Web.Controllers.Api
{
    public class kdataController : ApiController
    {
        [Route("api/kdata/{id}")]
        public chart Get(string id, string ktype)
        {
            var d = new Trade.Db.db().kdata(id, ktype);
            var basic = new Trade.Db.db().basics(id);
            var since = Trade.Cfg.Configuration.data.bearcrossbull;
            var q = d.Where(p => p.date >= since).ToArray();
            var data = q.Select(p => new object[] { p.date, p.open, p.high, p.low, p.close })
                .ToArray();
            var keyprices = Trade.analytic.keyprice(id, ktype);
            if (q.Any())
            {
                var cur = new[]
                {
                KeyPrice.low(id, q.Last().date, q.Last().low, true),
                KeyPrice.high(id, q.Last().date, q.Last().high, true)
            };
                keyprices = keyprices.Concat(cur).ToArray();
            }

            return new chart { data = data, code = d.Code, name = basic.name, keyprices = keyprices };
        }
    }

    public class chart
    {
        public KeyPrice[] keyprices { get; set; }
        public object[][] data { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}