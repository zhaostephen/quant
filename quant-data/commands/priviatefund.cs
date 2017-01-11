using Cli;
using HtmlAgilityPack;
using Interface.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trade.commands
{
    [command("privatefund")]
    class priviatefundcmd : command<object>
    {
        static ILog log = typeof(priviatefundcmd).Log();

        public priviatefundcmd(string[] args)
            : base(args)
        {

        }
        public override void exec()
        {
            log.Info("**********START**********");

            var db = new Db.db();
            var funds = private_fund_position.funds;
            for (var i = 0; i < funds.Length; ++i)
            {
                var fund = funds.ElementAt(i);
                try
                {
                    log.Info($"{i+1}/{funds.Length} get {fund}");
                    Thread.Sleep(10 * 1000);
                    var s = private_fund_position.get(fund);
                    log.Info($"{i + 1}/{funds.Length} got {fund}, total {s.Length}");
                    db.save(s);
                }
                catch (Exception e)
                {
                    log.Warn("ex @ get " + fund, e);
                }
            }

            log.Info("**********DONE**********");
        }
    }
    class private_fund_position
    {
        static Lazy<Dictionary<string, string>> fund_code_map_url = new Lazy<Dictionary<string, string>>(fund_position_code_map_url);

        public static string[] funds { get { return fund_code_map_url.Value.Keys.Distinct().ToArray(); } }

        public static privatefund[] get(string code, bool fund = true)
        {
            code = fund ? (fund_code_map_url.Value.ContainsKey(code) ? fund_code_map_url.Value[code] : string.Empty) : code;
            if (string.IsNullOrEmpty(code)) return new privatefund[0];

            var c = new HtmlWeb(); c.OverrideEncoding = Encoding.GetEncoding("GB2312");
            var url = (fund ? "http://cwzx.shdjt.com/" : "http://cwzx.shdjt.com/gpdmgd.asp?gpdm=") + code;
            var html = c.Load(url, "GET");
            if (html == null || html.DocumentNode == null) return new privatefund[0];
            var table = html.DocumentNode.Descendants("table").Where(p => p.GetAttributeValue("class", string.Empty) == "tb0td1").FirstOrDefault();
            if (table == null) return new privatefund[0];

            return table.Descendants("tr").Select(tr =>
            {
                var tds = tr.Elements("td").ToArray();
                var d = new privatefund();
                if (tds[0].InnerText.Trim() == "序")
                    return null;

                //d.index = tds[0].InnerText.Trim();
                d.stock_code = tds[1].InnerText.Trim();
                //d.name = tds[2].InnerText.Replace("资金", "").Replace(Environment.NewLine, "").Replace("点评", "").Replace("新闻", "").Replace("股东", "").Replace("F10", "").Replace("评", "").Trim();
                //d.price = tds[3].InnerText.Trim();
                //d.close = tds[4].InnerText.Trim();
                //d.chg = tds[5].InnerText.Trim();
                d.holdtype = tds[6].InnerText.Trim();
                d.updatedate = DateTime.Parse(tds[7].InnerText.Trim());
                d.fund_name = tds[8].InnerText.Trim();
                var amt = 0d; double.TryParse(tds[9].InnerText.Trim(), out amt);
                d.amount = amt;
                //d.typea = tds[10].InnerText.Trim();
                var pct = 0d; double.TryParse(tds[11].InnerText.Trim(), out pct);
                d.percentage = pct;
                d.type = tds[12].InnerText.Trim();
                d.changetype = tds[13].InnerText.Trim();
                double.TryParse(tds[14].InnerText.Trim(), out amt);
                d.changeamount = amt;
                return d;
            })
            .Where(p => p != null)
            .ToArray();
        }

        static Dictionary<string, string> fund_position_code_map_url()
        {
            var c = new HtmlWeb(); c.OverrideEncoding = Encoding.GetEncoding("GB2312");
            var html = c.Load("http://cwzx.shdjt.com/", "GET");
            if (html == null || html.DocumentNode == null) return new Dictionary<string, string>();
            var table = html.DocumentNode.Descendants("table").Where(p => p.GetAttributeValue("class", string.Empty) == "tb2td1").FirstOrDefault();
            if (table == null) return new Dictionary<string, string>();

            return table.Descendants("a")
                .Select(a => new { code = a.InnerText, href = a.GetAttributeValue("href", string.Empty) })
                .Where(p => !string.IsNullOrEmpty(p.code) && !string.IsNullOrEmpty(p.href))
                .GroupBy(p => p.code)
                .ToDictionary(p => p.Key, p => p.First().href);
        }
    }
}
