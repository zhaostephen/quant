using Dapper;
using Interace.Mixin;
using log4net;
using MySql.Data.MySqlClient;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Data;
using Trade.Mixin;

namespace Trade.Db
{
    public class kdatadb
    {
        static ILog log = LogManager.GetLogger(typeof(kdatadb));

        public IEnumerable<kdata> kdata(IEnumerable<string> codes, string ktype)
        {
            return codes.AsParallel().Select(code => kdata(code, ktype)).ToArray();
        }

        public void save(kdata data, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "\\" + data.Code + ".csv");
            var p = data.ToArray().ToCsv();
            File.WriteAllText(file, p, Configuration.encoding.gbk);
        }

        public kdata kdata(string code, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "\\" + code + ".csv");
            var p = file.ReadCsv<kdatapoint>(Configuration.encoding.gbk);

            try
            {
                p = kupdate(p, code, ktype);
            }
            catch (Exception e)
            {
                log.Error("kupdate " + code + " | " + ktype, e);
            }

            return new kdata(code, p);
        }

        IEnumerable<kdatapoint> kupdate(IEnumerable<kdatapoint> p, string code, string ktype)
        {
            if (p.Any())
            {
                switch (ktype)
                {
                    case "5":
                    case "15":
                    case "30":
                    case "60":
                        {
                            var perunit = int.Parse(ktype);
                            var kcurrent = p.Last().date;
                            var kt = ktoday(code);
                            var eodtoday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 15, 30, 0);
                            if (kcurrent < eodtoday)
                            {
                                if (kt.Any())
                                {
                                    var s = kt
                                        .Where(p1 => p1.ts > kcurrent)
                                        .OrderBy(p1 => (DateTime)p1.ts)
                                        .Select(t =>
                                        {
                                            var ts = ((DateTime)t.ts);

                                            var afeernoon = new DateTime(ts.Year, ts.Month, ts.Day, 13, 30, 0);
                                            var eod = new DateTime(ts.Year, ts.Month, ts.Day, 15, 30, 0);

                                            var sod = new DateTime(ts.Year, ts.Month, ts.Day, 9, 30, 0);
                                            var noon = new DateTime(ts.Year, ts.Month, ts.Day, 11, 30, 0);

                                            var datetime = ts < sod
                                                            ? ts.NearestKMinutes(sod, perunit, noon)
                                                            : ts.NearestKMinutes(afeernoon, perunit, eod);

                                            return new kdatapoint()
                                            {
                                                date = datetime,
                                                open = (double)t.open,
                                                close = (double)t.trade,
                                                high = (double)t.high,
                                                low = (double)t.low,
                                                volume = (double)t.volume / 100d
                                            };
                                        })
                                        .ToArray();

                                    s = s.Concat(p).ToArray();
                                    var dict = new SortedDictionary<DateTime, kdatapoint>();
                                    foreach (var i in s)
                                    {
                                        dict[i.date] = i;
                                    }

                                    p = dict.Values;
                                }
                            }
                        }
                        break;
                    case "D":
                    case "W":
                    case "M":
                        {
                            var current = p.Last();
                            var kcurrent = current.date;
                            if (kcurrent < DateTime.Today)
                            {
                                var kt = ktoday(code);
                                if (kt.Any())
                                {
                                    var t = kt.OrderByDescending(p1 => p1.ts).First();
                                    if (((DateTime)t.ts) > kcurrent)
                                    {
                                        var dt = (DateTime)t.ts;
                                        var kp = new kdatapoint()
                                        {
                                            date = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0),
                                            open = (double)t.open,
                                            close = (double)t.trade,
                                            high = (double)t.high,
                                            low = (double)t.low,
                                            volume = (double)t.volume / 100d
                                        };
                                        p = p.Concat(new[] { kp }).ToArray();

                                        var dict = new SortedDictionary<DateTime, kdatapoint>();
                                        foreach (var i in p)
                                        {
                                            dict[i.date] = i;
                                        }

                                        p = dict.Values;
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            return p;
        }

        public dynamic[] ktoday()
        {
            using (var conn = new MySqlConnection(Configuration.kdatadb))
            {
                conn.Open();

                return conn
                    .Query(@"SELECT * FROM ktoday WHERE ts in (SELECT max(ts) FROM ktoday)")
                    .ToArray();
            }
        }

        public dynamic[] ktoday(string code)
        {
            using (var conn = new MySqlConnection(Configuration.kdatadb))
            {
                conn.Open();

                return conn
                    .Query(@"SELECT * FROM ktoday WHERE code=@code", new { code = code })
                    .ToArray();
            }
        }
    }
}
