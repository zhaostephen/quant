using Interace.Attribution;
using Interace.Mixin;
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
        public kdata kdata(string code, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "/" + code + ".csv");
            var p = file.ReadCsv<kdatapoint>(Configuration.encoding.gbk);
            return new kdata(code, p);
        }

        public void save(kdata data, string ktype)
        {
            var file = Configuration.data.kdata.file(ktype + "/" + data.Code + ".csv");
            var p = data.ToArray().ToCsv();
            File.WriteAllText(file, p, Configuration.encoding.gbk);
        }

        public void save(Basics data)
        {
            var file = Configuration.data.basics.file("basics.csv");
            var p = data.ToArray().ToCsv();
            File.WriteAllText(file, p, Configuration.encoding.gbk);
        }

        public IEnumerable<Basic> basics()
        {
            var file = Configuration.data.basics.file("stock_basics.csv");
            return file.ReadCsv<Basic>(Configuration.encoding.gbk);
        }

        public IEnumerable<Basic> stock_basics()
        {
            var file = Configuration.data.basics.file("stock_basics.csv");
            return file.ReadCsv<Basic>(Configuration.encoding.gbk);
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

        public IEnumerable<string> codes()
        {
            return basics().Select(p => p.code).Distinct().ToArray();
        }
    }
}
