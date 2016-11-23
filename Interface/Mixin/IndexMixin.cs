using Interace.Idx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trade.Cfg;
using Trade.Mixin;

namespace Interace.Mixin
{
    public static class IndexMixin
    {
        public static DateTime? LastUpdate(this Index index, string code, PeriodEnum period)
        {
            var pe = period.ToString();
            var o = index.LastUpdate.FirstOrDefault(p=>p.Code == code && p.Period == pe);
            return o == null ? (DateTime?)null : o.Date.Date();
        }
    }
}
