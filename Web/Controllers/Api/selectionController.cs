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
            var codenames = new Trade.Db.db().basics().GroupBy(p => p.code).ToDictionary(p => p.Key, p => p.First());
            var universe = new Trade.Db.db().universe("macd60");
            return universe.codes
                .Select(p => new
                {
                    universe.name,
                    code = p,
                    stockname = codenames[p].name,
                    pe = codenames[p].pe.ToDouble()
                })
                //.Where(p => p.pe > 0 && p.pe < 30)
                .OrderBy(p => p.pe)
                .ToArray();
        }
    }
}