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

                var upserts = universe
                    .codes
                    .Select(p => string.Format("INSERT INTO universe (code,name) VALUES ('{0}','{1}') ON DUPLICATE KEY UPDATE ts=CURRENT_TIMESTAMP", p, universe.name))
                    .ToArray();
                var sql = string.Join(Environment.NewLine + ";", upserts);

                conn.Execute(sql);
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
            var file = Configuration.data.kdata.file(ktype + "/" + code + ".csv");
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
            var file = Configuration.data.kdata.file(ktype + "/" + data.Code + ".csv");
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
            var file = Configuration.data.basics.file("stock_basics.csv");
            return file.ReadCsv<basics>(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> area_classified()
        {
            var file = Configuration.data.basics.file("area_classified.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> concept_classified()
        {
            var file = Configuration.data.basics.file("concept_classified.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> gem_classified()
        {
            var file = Configuration.data.basics.file("gem_classified.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> hs300s()
        {
            var file = Configuration.data.basics.file("hs300s.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> industry_classified()
        {
            var file = Configuration.data.basics.file("industry_classified.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> sme_classified()
        {
            var file = Configuration.data.basics.file("sme_classified.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> st_classified()
        {
            var file = Configuration.data.basics.file("st_classified.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> suspended()
        {
            var file = Configuration.data.basics.file("suspended.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> sz50s()
        {
            var file = Configuration.data.basics.file("sz50s.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> terminated()
        {
            var file = Configuration.data.basics.file("terminated.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
        }

        public IEnumerable<dynamic> zz500s()
        {
            var file = Configuration.data.basics.file("zz500s.csv");
            return file.ReadCsv(Configuration.encoding.gbk);
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
