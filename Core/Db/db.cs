﻿using Dapper;
using Interace.Attribution;
using Interace.Mixin;
using Interface.Quant;
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
    public class db
    {
        static ILog log = LogManager.GetLogger(typeof(db));

        public void save(universe universe)
        {
            using (var conn = new MySqlConnection(Configuration.quantdb))
            {
                conn.Open();

                conn.Execute("DELETE FROM universe WHERE name=@name", new { name = universe.name });

                var upserts = universe
                    .codes
                    .Select(p => string.Format("INSERT INTO universe (code,name) VALUES ('{0}','{1}')", p, universe.name))
                    .ToArray();

                if (upserts.Any())
                    conn.Execute(string.Join(";", upserts));
            }
        }

        public Dictionary<string, DateTime> keypricedates()
        {
            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();

                return conn
                    .Query(@"SELECT DISTINCT code,ktype, max(date) date FROM keyprice GROUP by code,ktype")
                    .GroupBy(p=>new { code = (string)p.code, ktype = (string)p.ktype })
                    .ToDictionary(p => p.Key.code + p.Key.ktype, p => p.Max(p1=>(DateTime)p1.date));
            }
        }

        public KeyPrice[] keyprices(string ktype)
        {
            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();

                return conn
                    .Query<KeyPrice>(@"SELECT * FROM keyprice WHERE ktype=@ktype",new { ktype = ktype })
                    .ToArray();
            }
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
                    .Query(@"SELECT * FROM ktoday WHERE code=@code",new { code = code})
                    .ToArray();
            }
        }

        public void save(string ktype, KeyPrice[] o)
        {
            if (!o.Any()) return;

            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();

                var upserts = o
                    .Select(p => string.Format(@"INSERT IGNORE INTO keyprice (code,date,price,flag,ktype,auto) 
                                                VALUES ('{0}','{1:yyyy-MM-dd HH:mm:ss}',{2},'{3}','{4}',{5})",
                                p.Code, p.Date, p.Price, p.Flag, ktype, p.Auto ? "true" : "false"))
                    .ToArray();

                if (upserts.Any())
                    conn.Execute(string.Join(";", upserts));
            }
        }

        public universe universe(string name)
        {
            using (var conn = new MySqlConnection(Configuration.quantdb))
            {
                conn.Open();
                var codes = conn
                    .Query<string>("select code from universe where name=@name", new { name = name })
                    .Distinct()
                    .ToArray();

                return new universe(name, codes);
            }
        }

        public basics basics(string code)
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basics>("select * from basics where code=@code",new { code = code })
                    .FirstOrDefault();
            }
        }

        public IEnumerable<basics> basics(IEnumerable<string> codes)
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basics>("select * from basics where code IN '@codes'", new { codes = codes });
            }
        }

        public IEnumerable<basicname> basicnames()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basicname>("select code,name,nameabbr,assettype from basics");
            }
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
                            if(kcurrent < eodtoday)
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
                                            volume = (double)t.volume/100d
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

        public IEnumerable<kdata> kdataall(string ktype, string secorOrIndex = null)
        {
            var codes = this.codes(secorOrIndex).ToArray();
            var results = codes
                .AsParallel()
                .Select(code => kdata(code, ktype))
                .Where(p => p != null)
                .ToArray();
            return results;
        }

        public IEnumerable<string> codes(string secorOrIndex = null)
        {
            var basics = this.basics().AsEnumerable();
            if (!string.IsNullOrEmpty(secorOrIndex))
            {
                switch (secorOrIndex)
                {
                    case assettypes.index:
                        basics = basics.Where(p => p.assettype == assettypes.index).ToArray();
                        break;
                    case assettypes.stock:
                        basics = basics.Where(p => p.assettype == assettypes.stock).ToArray();
                        break;
                    case assettypes.sector:
                        basics = basics.Where(p => p.assettype == assettypes.sector).ToArray();
                        break;
                    default:
                        basics = basics
                            .Where(p => string.IsNullOrEmpty(secorOrIndex)
                                        || p.belongtoindex(secorOrIndex)
                                        || p.belongtosector(secorOrIndex))
                            .ToArray();
                        break;
                }
            }
            return basics.Select(p => p.code).Distinct().ToArray();
        }

        public Interace.Quant.Trade[] trades(string porflio)
        {
            var path = Configuration.data.trade.EnsurePathCreated();
            var file = Path.Combine(path, porflio + ".csv");
            return file.ReadCsv<Interace.Quant.Trade>().ToArray();
        }

        public string[] sectors()
        {
            return basics()
                .Select(p => p.sectors)
                .Where(p => !string.IsNullOrEmpty(p))
                .SelectMany(p => p.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToArray();
        }

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

        public void save(IEnumerable<basics> data)
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();

                var upserts = data
                    .GroupBy(p=>p.code)
                    .Select(p=>p.First())
                    .SelectMany(d => new[]
                {
                    "DELETE FROM basics WHERE code='" +d.code+"'",

                    "INSERT INTO basics (code,name,nameabbr,industry,area,pe,outstanding,totals,totalAssets,liquidAssets,fixedAssets,reserved,reservedPerShare,esp,bvps,pb,timeToMarket,undp,perundp,rev,profit,gpr,npr,holders,st,suspended,`terminated`,assettype,`indexes`,sectors) " +
                    $"VALUES ('{d.code}','{d.name}','{d.nameabbr}','{d.industry}','{d.area}',{d.pe},{d.outstanding},{d.totals},{d.totalAssets},{d.liquidAssets},{d.fixedAssets},{d.reserved},{d.reservedPerShare},{d.esp},{d.bvps},{d.pb},{d.timeToMarket},{d.undp},{d.perundp},{d.rev},{d.profit},{d.gpr},{d.npr},{d.holders},'{d.st}','{d.suspended}','{d.terminated}','{d.assettype}','{d.indexes}','{d.sectors}') "
                })
                .ToArray();

                if (upserts.Any())
                    conn.Execute(string.Join(";", upserts));
            }
        }

        public IEnumerable<basics> basics()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basics>("select * from basics")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<basics> stock_basics()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basics>("select * from stock_basics")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> area_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from area_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> custom_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from custom_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> concept_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from concept_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> gem_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from gem_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> hs300s()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from hs300s")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> industry_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from industry_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> sme_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from sme_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> st_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from st_classified")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> suspended()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from suspended")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> sz50s()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from sz50s")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> terminated()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("SELECT * FROM `terminated`")
                    .Distinct()
                    .ToArray();
            }
        }

        public IEnumerable<dynamic> zz500s()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<dynamic>("select * from zz500s")
                    .Distinct()
                    .ToArray();
            }
        }

        public void save(string porflio, IEnumerable<Interace.Quant.Trade> trades)
        {
            if (!trades.Any()) return;

            var path = Configuration.data.trade.EnsurePathCreated();
            var file = Path.Combine(path, porflio + ".csv");
            var csv = trades.ToCsv();
            if (File.Exists(file))
            {
                csv = string.Join(Environment.NewLine,
                    csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToArray());
            }
            File.AppendAllText(
                file,
                csv,
                Encoding.UTF8);
        }
    }
}
