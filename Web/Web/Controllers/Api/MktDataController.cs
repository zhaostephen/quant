using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Trade;

namespace Web.Controllers.Api
{
    public class MktDataController : ApiController
    {
        public chart Get(string id)
        {
            var c = new MktDataClient();
            var d = c.Query(id, Trade.Cfg.PeriodEnum.Daily);
            var basic = c.QueryFundamental(id);
            var since = d.Max(p => p.Date).AddDays(-220);

            var data = d
                .Where(p => p.Date >= since)
                .Select(p => new object[] { p.Date, p.Open, p.High, p.Low, p.Close })
                .ToArray();

            return new chart { data = data, code = d.Code, name= basic.名称 };
        }
    }

    public class chart
    {
        public object[][] data { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}