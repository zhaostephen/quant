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

namespace Web.Controllers.Api
{
    public class kdataController : ApiController
    {
        public chart Get(string id, string ktype)
        {
            var d = new client().kdata(id, ktype);
            var basic = new client().basics(id);
            var since = Trade.Cfg.Configuration.data.bearcrossbull;

            var q = d.Where(p => p.date >= since).ToArray();
            var data = q.Select(p => new object[] { p.date, p.open, p.high, p.low, p.close })
                .ToArray();

            var keyprices = q.Select(p =>
            {
                if (p.peak_h.HasValue) return KeyPrice.high(id, p.date, p.peak_h.Value, true);
                else if (p.peak_l.HasValue) return KeyPrice.low(id, p.date, p.peak_l.Value, true);
                return null;
            })
            .Where(p => p != null)
            .ToArray();

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