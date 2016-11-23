using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Interace.Idx
{
    public class Index
    {
        public LastUpdateIdx[] LastUpdate { get; set; }

        public Index Update(Index o)
        {
            var q = from a in o.LastUpdate
                    join b in LastUpdate on new { a.Code, a.Period } equals new { b.Code, b.Period }
                    select new { N = b, O = b };
            foreach(var p in q)
            {
                p.O.Date = p.N.Date;
            }

            return this;
        }

        public Index()
        {
            LastUpdate = new LastUpdateIdx[0];
        }
    }
}
