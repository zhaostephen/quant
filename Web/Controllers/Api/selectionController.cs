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
        [Route("api/selections")]
        public object[] Get()
        {
            var codenames = new client().basicnames().GroupBy(p=>p.code).ToDictionary(p => p.Key, p => p.First().name);
            var universe = new client().universe("macd60");
            return universe.codes
                .Select(p => new { universe.name, code = p, stockname = codenames[p] })
                .ToArray();
        }
    }
}