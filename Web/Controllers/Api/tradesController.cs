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
    public class trades : ApiController
    {
        public Interace.Quant.Trade[] Get(string id)
        {
            return new client().trades(id);
        }
    }
}