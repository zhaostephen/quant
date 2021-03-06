﻿using Interace.Attribution;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Db;
using Trade.Indicator;

namespace Trade
{
    public static class analytic
    {
        public static KeyPrice[] keyprice(string code, string ktype)
        {
            var d = new kdatadb().kdata(code, ktype);
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
            var todayquotes = new kdatadb().ktoday();

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
                        && Math.Abs((t.low / k.Price - 1) * 100d) <= 0.8
                        && t.changepercent >= 0.01 && t.changepercent <= 1.0
                        && Math.Abs((k.Date - DateTime.Today).TotalDays) >= 8
                     select new { k, t }
                    )
                    .ToArray()
                    .Where(p =>
                    {
                        var code = (string)p.t.code;
                        var k = new kdatadb().kdata(code, "D");
                        if (k == null) return false;

                        var low = (double)p.t.low;
                        var N = 8;
                        for (var i = k.Count - 1; i > k.Count - N; --i)
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
                .OrderBy(p => p.cross)
                .ThenByDescending(p => p.changepercent)
                .ToArray();

            return r;
        }

        public static dynamic[] sectorstocks(string sector)
        {
            var db = new db();
            var s = db.basics(sector);
            if (s == null || s.assettype != assettypes.sector)
                return new dynamic[0];

            var codes = db.codes(sector);
            if(!codes.Any()) return new dynamic[0];

            var basics = db.basics(codes);
            var ka = db.kanalytic(codes, "D");

            var q = (from k in ka
                     join t in basics on k.code equals t.code
                     where t.assettype == assettypes.stock && !t.terminated && !t.suspended && !t.st
                     select new { k, t }
                    )
                    .Select(_ =>
                    {
                        var k = _.k;
                        var t = _.t;

                        dynamic d = new ExpandoObject();
                        d.date = k.date;
                        d.pe = t.pe;
                        d.code = t.code;
                        d.name = t.name;
                        d.high = k.high;
                        d.low = k.low;
                        d.open = k.open;
                        d.close = k.close;
                        d.volume = k.volume;
                        d.chg = k.chg;
                        d.pb = t.pb;
                        d.pe = t.pe;
                        return d;
                    });

            var r = q
                .OrderBy(p => p.code)
                .ThenByDescending(p => p.date)
                .GroupBy(p => new { p.code })
                .Select(p => p.First())
                .OrderByDescending(p=>p.chg)
                .ToArray();

            return r;
        }

        public static dynamic[] macd60()
        {
            var kas = new db().kanalytics("60")
                .Where(p => { return p.macd > 0 && p.dif <= 0.01; })
                .ToArray();
            if (!kas.Any()) return new dynamic[0];
            kas = new db().kanalytic(kas.Select(p => p.code).Distinct().ToArray(), "15")
                .Where(p => { return p.macd > 0; })
                .ToArray();
            if (!kas.Any()) return new dynamic[0];

            kas = kas
                .AsParallel()
                .Where(p =>
                {
                    var k = new kdatadb().kdata(p.code, "15");
                    if (k == null || !k.Any()) return false;

                    var deviation = (deviation)new DEVIATION(k.close(), deviationtype.底背离);
                    return deviation != null && deviation.d2.Date.Date >= p.date.Date;
                })
                .ToArray();

            if (!kas.Any()) return new dynamic[0];

            var codes = kas.Select(p => p.code).Distinct().ToArray();
            var basics = new db().basics(codes);
            var ka = new db().kanalytic(codes, "D");

            var q = (from k in ka
                     join t in basics on k.code equals t.code
                     where t.assettype == assettypes.stock && !t.terminated && !t.suspended && !t.st
                     select new { k, t }
                    )
                    .Select(_ =>
                    {
                        var k = _.k;
                        var t = _.t;

                        dynamic d = new ExpandoObject();
                        d.date = k.date;
                        d.pe = t.pe;
                        d.code = t.code;
                        d.name = t.name;
                        d.high = k.high;
                        d.low = k.low;
                        d.open = k.open;
                        d.close = k.close;
                        d.volume = k.volume;
                        d.chg = k.chg;
                        d.pb = t.pb;
                        d.pe = t.pe;
                        return d;
                    });

            var r = q
                .OrderBy(p => p.code)
                .ThenByDescending(p => p.date)
                .GroupBy(p => new { p.code })
                .Select(p => p.First())
                .ToArray();

            return r;
        }
    }
}
