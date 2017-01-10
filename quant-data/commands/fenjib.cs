using Cli;
using HtmlAgilityPack;
using Interface.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.commands
{
    [command("fenjib")]
    class fenjibcmd : command<object>
    {
        static ILog log = typeof(fenjibcmd).Log();

        public fenjibcmd(string[] args)
            : base(args)
        {

        }
        public override void exec()
        {
            log.Info("**********START**********");

            var db = new Db.db();
            var codes = db.fenjib();
            var validcodes = db.basicnames().GroupBy(p => p.code).ToDictionary(p => p.Key, p => p.First().code);
            for(var i  = 0; i < codes.Count(); ++i)
            {
                var code = codes.ElementAt(i);
                try
                {
                    log.Info($"{i}/{codes.Count()} get {code}");
                    var s = fenjib.get(code).Where(p=> validcodes.ContainsKey(p.stock_code)).ToArray();
                    db.save(s);
                }
                catch(Exception e)
                {
                    log.Warn("ex @ get "+code, e);
                }
            }

            log.Info("**********DONE**********");
        }
    }
    class fenjib
    {
        public static fenjibdata[] get(string code)
        {
            var c = new HtmlWeb(); c.OverrideEncoding = Encoding.GetEncoding("GB2312");
            var url = $"http://finance.sina.com.cn/fund/quotes/{code}/bc.shtml";
            var html = c.Load(url, "GET");
            if (html == null) return new fenjibdata[0];
            var table = html.GetElementbyId("table-investment-association");
            if (table == null) return new fenjibdata[0];

            var name = html.GetElementbyId("box-fund-hq").Element("div").Element("h1").InnerText.Trim();
            var trs = table.Element("tbody").Elements("tr");
            if (trs.Count() < 2) return new fenjibdata[0];
            var tdc = trs.ElementAt(1).Elements("td").ElementAt(0).Element("td");
            var date = DateTime.Parse(tdc.Element("p").InnerText.Replace("数据日期：", ""));//数据日期：2016-09-30

            return tdc.Element("table").Element("tbody").Elements("tr").Select(tr =>
            {
                var tds = tr.Elements("td").ToArray();
                var d = new fenjibdata();
                d.fund_code = code;
                d.fund_name = name;
                d.update_date = date;
                d.stock_name = tds[0].InnerText.Trim();
                d.stock_code = tds[0].Element("a").GetAttributeValue("href",string.Empty)
                                .Replace("http://finance.sina.com.cn/realstock/company/","")
                                .Replace("/nc.shtml","")
                                .Replace("sh","")
                                .Replace("sz", "")
                                .Trim();
                //d.price = tds[1].InnerText.Trim();
                //d.pricechg = tds[2].InnerText.Trim();
                d.weight = double.Parse(tds[3].InnerText.Trim().Replace("%", "").Trim());
                //d.holdfunds = tds[4].InnerText.Trim();
                return d;
            })
            .ToArray();
        }
    }
}
