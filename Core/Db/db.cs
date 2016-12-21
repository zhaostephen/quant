using Dapper;
using Interace.Attribution;
using Interace.Mixin;
using Interface.Quant;
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

namespace Trade.Db
{
    public class db
    {
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
            return basics()
                .FirstOrDefault(p => string.Equals(p.code, code, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<basics> basics(IEnumerable<string> codes)
        {
            var set = basics();

            var q = from f in set
                    join c in codes on f.code equals c
                    select f;

            return q.ToArray();
        }

        public IEnumerable<basicname> basicnames()
        {
            return basics()
                .Select(p => new basicname { name = p.name, code = p.code, assettype = p.assettype, nameabbr = p.nameabbr })
                .ToArray();
        }

        public kdata kdata(string code, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "\\" + code + ".csv");
            var p = file.ReadCsv<kdatapoint>(Configuration.encoding.gbk);
            return new kdata(code, p);
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
            var file = Configuration.data.basics.file("basics.csv");
            var p = data.ToArray().ToCsv();
            File.WriteAllText(file, p, Configuration.encoding.gbk);
        }

        public void save(IEnumerable<basicname> data)
        {
            var file = Configuration.data.basics.file("basicnames.csv");
            var p = data.ToArray().ToCsv();
            File.WriteAllText(file, p, Configuration.encoding.gbk);
        }

        public IEnumerable<basics> basics()
        {
            var file = Configuration.data.basics.file("basics.csv");
            return file.ReadCsv<basics>(Configuration.encoding.gbk);
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
