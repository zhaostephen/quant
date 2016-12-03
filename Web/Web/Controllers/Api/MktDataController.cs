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
    public class MktDataController : ApiController
    {
        public chart Get(string id, string period)
        {
            var per = (PeriodEnum)Enum.Parse(typeof(PeriodEnum), period, true);
            var d = new MktDataClient().Query(id, per);
            var basic = new MktDataClient().QueryFundamental(id);
            //var since = d.Max(p => p.Date).AddDays(-250);
            var since = Trade.Cfg.Configuration.bearcrossbull;

            var data = d
                .Where(p => p.Date >= since)
                .Select(p => new object[] { p.Date, p.Open, p.High, p.Low, p.Close })
                .ToArray();

            var keyprices = new AttributionClient().QueryKeyPrices(id).ToArray();

            return new chart { data = data, code = d.Code, name= basic.名称, keyprices = keyprices };
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