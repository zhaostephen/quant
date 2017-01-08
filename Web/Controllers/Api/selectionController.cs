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
using Trade.Data;

namespace Web.Controllers.Api
{
    public class selectionController : ApiController
    {
        [Route("api/selections/macd60")]
        public object macd60(string id)
        {
            var r = Trade.analytic.macd60();
            var rows = pagination(r);
            return new { total = r.Count(), rows = rows };
        }

        [Route("api/selections/hitkeyprice")]
        [HttpGet]
        public object hitkeyprice(string order = null, string sort = null, int? limit = null, int? offset = null)
        {
            var r = Trade.analytic.hitkeyprices();
            var rows = pagination(r);
            return new { total = r.Count(), rows = rows };
        }

        dynamic[] pagination(dynamic[] r, string order = null, string sort = null, int? limit = null, int? offset = null)
        {
            if (r == null || !r.Any())
                return new dynamic[0];

            if (limit.HasValue && offset.HasValue)
                r = r.Skip(offset.Value).Take(limit.Value).ToArray();

            order = order ?? "cross";
            sort = sort ?? "desc";

            if (sort == "asc")
                r = r.OrderBy(p => orderdynamic(p, order)).ToArray();
            else
                r = r.OrderByDescending(p => orderdynamic(p, order)).ToArray();

            return r;
        }

        static object orderdynamic(dynamic d, string orderby)
        {
            var dict = (IDictionary<string, object>)d;
            if (!dict.ContainsKey(orderby)) return default(object);
            return dict[orderby];
        }
    }
}