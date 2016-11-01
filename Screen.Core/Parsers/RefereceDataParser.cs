using Screen.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Screen.Mixin;

namespace Screen.Parsers
{
    public class RefereceDataParser
    {
        public static FundB[] ParseFundB(string file)
        {
            var text = File.ReadAllText(file, Encoding.GetEncoding("GB2312"));
            var matches = Regex.Matches(text, "(?<content>☆持股明细☆.*?)☆持股明细☆", RegexOptions.Singleline);
            var result = new Dictionary<string, FundB>();
            foreach(var match in matches.Cast<Match>())
            {
                var content = match.Groups["content"].Value;
                var m = Regex.Match(content, @"☆持股明细☆\s*◇(?<code>\d+)\s*(?<name>.*)\s*更新日期", RegexOptions.Singleline);
                var code = m.Groups["code"].Value;
                var name = m.Groups["name"].Value;
                var lines = Regex.Matches(content, @"(?<代码>\d+)\s+(?<股票名称>.*)\s+(?<持有数量>\d+\.\d+)\s+(?<市值>\d+\.\d+)\s+(?<占流通股比例>\d+\.\d+)\s+(?<占净值比>\d+\.\d+)");

                var tmp = new List<SecurityB>();
                foreach(var line in lines.Cast<Match>())
                {
                    var 代码 = line.Groups["代码"].Value.Trim();
                    var 股票名称 = line.Groups["股票名称"].Value.Trim();
                    var 持有数量 = line.Groups["持有数量"].Value.Double()*1e4;
                    var 市值 = line.Groups["市值"].Value.Double() * 1e4;
                    var 占流通股比例 = line.Groups["占流通股比例"].Value.Double();
                    var 占净值比 = line.Groups["占净值比"].Value.Double();

                    tmp.Add(new SecurityB { 代码 = 代码, 占净值比 = 占净值比, 占流通股比例 = 占流通股比例, 名称 = 股票名称, 持有数量 = 持有数量 });
                }

                result[name] = new FundB { 代码 = code, 名称 = name, SecurityBs = tmp.ToArray() };
            }

            return result.Values.ToArray();
        }
        public static Security[] ParseSecurites(string file)
        {
            var lines = File.ReadAllLines(file, Encoding.GetEncoding("GB2312"));
            var headerSplits = lines[0].Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var 代码 = Array.IndexOf(headerSplits, "代码");
            var 名称 = Array.IndexOf(headerSplits, "名称");
            var 流通股本 = Array.IndexOf(headerSplits, "流通股本(万)");
            var 细分行业 = Array.IndexOf(headerSplits, "细分行业");
            var 地区 = Array.IndexOf(headerSplits, "地区");

            var securities = lines
                .Select(p =>
                {
                    var splits = p.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var iscode = Regex.IsMatch(splits[0], @"\d\d\d\d\d\d");
                    if (!iscode) return null;
                    if (splits.Length <= 地区) return null;
                    return new Security
                    {
                        代码 = splits[代码],
                        名称 = splits[名称],
                        流通股本 = splits[流通股本].Double() * 1e4,
                        细分行业 = splits[细分行业],
                        地区 = splits[地区]
                    };
                })
                .Where(p => p != null)
                .ToArray();

            if (!securities.Any())
                return null;

            return securities;
        }
    }
}
