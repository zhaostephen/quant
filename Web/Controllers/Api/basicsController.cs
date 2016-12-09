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
    public class basicsController : ApiController
    {
        [Route("api/basics/names/{id}")]
        public basicname[] Get(string id)
        {
            if (id == "all")
                return new client().basicnames().ToArray();

            return new client()
                .basicnames()
                .Where(p => p.nameabbr.StartsWith(id, StringComparison.InvariantCultureIgnoreCase) ||
                          p.code.StartsWith(id))
                 .OrderBy(p=>p.nameabbr)
                 .Take(10)
                .ToArray();
        }
    }
}