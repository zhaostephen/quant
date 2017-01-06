using Interace.Attribution;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Indicator;

namespace Trade
{
    public static class analytic
    {
        public static KeyPrice[] keyprice(string code, string ktype)
        {
            var db = new Db.db();
            var d = db.kdata(code, ktype);
            var peak_h = new PEAK(d, PEAK_TYPE.high);
            var peak_l = new PEAK(d, PEAK_TYPE.low);
            //var peak_h_volume = new PEAK(d, k => k.volume, PEAK_TYPE.high);
            //var peak_l_volume = new PEAK(d, k => k.volume, PEAK_TYPE.low);

            var keyprices = d.Select(p =>
            {
                var h = peak_h[p.date];
                var l = peak_l[p.date];
                //var h_volume = peak_h_volume[p.date];
                //var l_volume = peak_l_volume[p.date];
                //if (h > 0 && h_volume > 0) return KeyPrice.high(code, p.date, h, true);
                //else if (l > 0 && l_volume > 0) return KeyPrice.low(code, p.date, l, true);
                if (h > 0) return KeyPrice.high(code, p.date, h, true);
                else if (l > 0) return KeyPrice.low(code, p.date, l, true);
                return null;
            })
            .Where(p => p != null)
            .ToArray();

            return keyprices;
        }

        public static dynamic[] hitkeyprices()
        {
            var db = new Db.db();

            var keyprices = db.keyprices("D");
            var todayquotes = db.ktoday();

            keyprices = keyprices
                .OrderBy(p => p.Code)
                .ThenByDescending(p => p.Date)
                .Where(p => p.Flag == KeyPrice.Flags.lower)
                .GroupBy(p => p.Code)
                .Select(p => p.First())
                .ToArray();

            var q = (from k in keyprices
                     join t in todayquotes on k.Code equals t.code
                     where k.Flag == KeyPrice.Flags.lower 
                        && Math.Abs((t.low / k.Price - 1) * 100d) <= 1.0
                        && t.changepercent>=0.01 && t.changepercent <= 0.5
                        && Math.Abs((k.Date - DateTime.Today).TotalDays) >= 21
                     select new { k, t }
                    )
                    .ToArray()
                    .Where(p=>
                    {
                        var code = (string)p.t.code;
                        var k = db.kdata(code, "D");
                        if (k == null) return false;

                        var low = (double)p.t.low;
                        var N = 8;
                        for (var i = k.Count - 1; i > k.Count - N; ++i)
                        {
                            if (k[i].low < low)
                                return false;
                        }

                        return true;
                    })
                    .Select(_ =>
                    {
                        var k = _.k;
                        var t = _.t;

                        dynamic d = new ExpandoObject();
                        d.date = k.Date;
                        d.cross = Math.Abs((k.Date - DateTime.Today).TotalDays);
                        d.price = k.Price;
                        d.distpercent = (t.low / k.Price - 1) * 100;
                        d.pe = t.pe;
                        d.code = t.code;
                        d.name = t.name;
                        d.trade = t.trade;
                        d.high = t.high;
                        d.low = t.low;
                        d.open = t.open;
                        d.close = t.close;
                        d.volume = t.volume;
                        d.changepercent = t.changepercent;
                        d.turnoverratio = t.turnoverratio;
                        d.pb = t.pb;
                        d.amount = t.amount;
                        d.mktcap = t.mktcap;
                        d.ts = t.ts;

                        return d;
                    });

            var r = q
                .OrderBy(p => p.code)
                .ThenByDescending(p => p.date)
                .GroupBy(p => new { p.code })
                .Select(p => p.First())
                .OrderBy(p=>p.cross)
                .ThenByDescending(p => p.changepercent)
                .ToArray();

            return r;
        }
    }
}
