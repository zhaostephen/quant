using Dapper;
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
using Interface.Data;

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

        public void save(IEnumerable<privatefund> s)
        {
            if (s == null || !s.Any()) return;

            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();

                var upserts = s
                    .Select(p => "INSERT IGNORE INTO private_fund_classified(fund_name,stock_code,holdtype,updatedate,amount,percentage,`type`,changetype,changeamount) VALUES " +
                                $"('{p.fund_name}','{p.stock_code}','{p.holdtype}','{p.updatedate:yyyy-MM-dd}',{p.amount},{p.percentage},'{p.type}','{p.changetype}',{p.changeamount})")
                    .ToArray();

                conn.Execute(string.Join(";", upserts));
            }
        }

        public void save(IEnumerable<fenjibdata> s)
        {
            if (s == null || !s.Any()) return;

            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();

                var upserts = s
                    .Select(p => "INSERT IGNORE INTO fenji_b_classified(fund_code,fund_name,stock_code,update_date,weight) VALUES " + 
                                $"('{p.fund_code}','{p.fund_name}','{p.stock_code}','{p.update_date:yyyy-MM-dd HH:mm:ss}',{p.weight})")
                    .ToArray();

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

        public void save(IEnumerable<kanalytic> o)
        {
            if (!o.Any()) return;

            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();

                var upserts = o
                    .Select(p => @"INSERT IGNORE INTO k (code,date,ktype,close,open,high,low,volume,chg,ma5,ma10,ma20,ma30,ma60,ma120,dea,macd,dif,deavol,macdvol,difvol) "+
                                $" VALUES ('{p.code}','{p.date:yyyy-MM-dd HH:mm:ss}','{p.ktype}',{p.close},{p.open},{p.high},{p.low},{p.volume},{p.chg},{p.ma5},{p.ma10},{p.ma20},{p.ma30},{p.ma60},{p.ma120},{p.dea},{p.macd},{p.dif},{p.deavol},{p.macdvol},{p.difvol})")
                    .ToArray();

                if (upserts.Any())
                    conn.Execute(string.Join(";", upserts));
            }
        }

        public kanalytic kanalytic(string code, string ktype)
        {
            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();
                return conn
                    .Query<kanalytic>("select * from k where code=@code and ktype=@ktype", new { code = code, ktype = ktype })
                    .OrderByDescending(p => p.ts)
                    .FirstOrDefault();
            }
        }

        public IEnumerable<kanalytic> kanalytics(string ktype)
        {
            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();
                return conn
                    .Query<kanalytic>("select * from k where ktype=@ktype", new { ktype = ktype })
                    .GroupBy(p => p.code)
                    .Select(p => p.OrderByDescending(p1 => p1.ts).FirstOrDefault())
                    .Where(p => p != null)
                    .ToArray();
            }
        }

        public IEnumerable<kanalytic> kanalytic(IEnumerable<string> codes, string ktype)
        {
            if (codes == null || !codes.Any()) return new kanalytic[0];

            using (var conn = new MySqlConnection(Configuration.analyticdb))
            {
                conn.Open();
                return conn
                    .Query<kanalytic>("select * from k where ktype=@ktype and code IN @codes", new { ktype = ktype, codes = codes })
                    .GroupBy(p => p.code)
                    .Select(p => p.OrderByDescending(p1 => p1.ts).FirstOrDefault())
                    .Where(p => p != null)
                    .ToArray();
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

        public string[] fenjib_fund_codes()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<string>("select code from fenji_b")
                    .Distinct()
                    .ToArray();
            }
        }

        public fenjibdata[] fenjib_classified()
        {
            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<fenjibdata>("select * from fenji_b_classified")
                    .ToArray();
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
            if (codes == null || !codes.Any()) return new basics[0];

            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basics>("select * from basics where code IN @codes", new { codes = codes });
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

        public IEnumerable<basicname> basicnames(IEnumerable<string> codes)
        {
            if (codes == null || !codes.Any()) return new basicname[0];

            using (var conn = new MySqlConnection(Configuration.basicsdb))
            {
                conn.Open();
                return conn
                    .Query<basicname>("select code,name,nameabbr,assettype from basics where code IN @codes", new { codes = codes });
            }
        }

        public IEnumerable<basicname> basicnamesinsector(string sector)
        {
            var basics = this.basics().AsEnumerable();
            return basics
                .Where(p => string.IsNullOrEmpty(sector)
                            || p.belongtoindex(sector)
                            || p.belongtosector(sector))
                .Select(p=>new basicname { code = p.code, name =p.name, assettype = p.assettype ,nameabbr = p.nameabbr })
                .ToArray();
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

        public string[] sectors()
        {
            return basics()
                .Select(p => p.sectors)
                .Where(p => !string.IsNullOrEmpty(p))
                .SelectMany(p => p.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToArray();
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

        public void save(string portflio, Interace.Quant.Trade trade)
        {
            using (var conn = new MySqlConnection(Configuration.quantdb))
            {
                conn.Open();

                var upsert = "INSERT IGNORE orders (portflio,code,date,dir,quantity,comments) " +
                       $"VALUES ('{trade.portflio}','{trade.code}','{trade.date:yyyy-MM-dd HH:mm:ss}','{trade.dir}',{trade.quantity},'{trade.comments ?? "-"}')";

                conn.Execute(upsert);
            }
        }

        public bool tradeexists(string portflio, Interace.Quant.Trade trade)
        {
            using (var conn = new MySqlConnection(Configuration.quantdb))
            {
                conn.Open();

                return conn
                    .Query<Interace.Quant.Trade>(
                        "select * from orders where portflio=@portflio and code=@code and date=@date",
                        new { portflio= portflio, code= trade.code, date= trade.date.ToString("yyyy-MM-dd HH:mm:ss") })
                    .Any();
            }
        }

        public Interace.Quant.Trade[] trades(string portflio)
        {
            using (var conn = new MySqlConnection(Configuration.quantdb))
            {
                conn.Open();

                return conn
                    .Query<Interace.Quant.Trade>(
                        "select * from orders where portflio=@portflio",
                        new { portflio = portflio })
                    .ToArray();
            }
        }
    }
}
