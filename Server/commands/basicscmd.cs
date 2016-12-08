﻿using Cli;
using log4net;
using Pinyin4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trade.Data;
using Trade.Mixin;

namespace Trade.commands
{
    [command("basics")]
    class basicscmd : command<object>
    {
        static ILog log = typeof(basicscmd).Log();

        public basicscmd(string[] args)
            :base(args)
        {

        }
        public override void exec()
        {
            log.Info("**********START**********");

            var db = new Db.db();
            var stock_basics = db.stock_basics().ToList();
            var suspended = code_names(() => db.suspended());
            var terminated = code_names(() => db.terminated());
            var st_classified = code_names(() => db.st_classified());
            var hs300s = code_names(() => db.hs300s());
            var sz50s = code_names(() => db.sz50s());
            var sme_classified = code_names(() => db.sme_classified());
            var gem_classified = code_names(() => db.gem_classified());
            var concept_classified = code_cnames(() => db.concept_classified());
            var industry_classified = code_cnames(() => db.industry_classified());

            var basiscs = new Basics(stock_basics);
            foreach (var stockbasic in stock_basics)
            {
                stockbasic.assettype = assettypes.stock;
                stockbasic.nameabbr = pinyin(stockbasic.name);

                if (suspended.ContainsKey(stockbasic.code))
                    stockbasic.suspended = true;
                if (terminated.ContainsKey(stockbasic.code))
                    stockbasic.terminated = true;
                if (st_classified.ContainsKey(stockbasic.code))
                    stockbasic.st = true;

                if(stockbasic.code.StartsWith("00") || stockbasic.code.StartsWith("30"))
                    stockbasic.addindex(index.sz);
                if (stockbasic.code.StartsWith("60"))
                    stockbasic.addindex(index.sh);
                if (hs300s.ContainsKey(stockbasic.code))
                    stockbasic.addindex(index.hs300);
                if (sz50s.ContainsKey(stockbasic.code))
                    stockbasic.addindex(index.sz50);
                if (sme_classified.ContainsKey(stockbasic.code))
                    stockbasic.addindex(index.zxb);
                if (gem_classified.ContainsKey(stockbasic.code))
                    stockbasic.addindex(index.cyb);

                if (concept_classified.ContainsKey(stockbasic.code))
                    stockbasic.addsector(concept_classified[stockbasic.code]);
                if (industry_classified.ContainsKey(stockbasic.code))
                    stockbasic.addsector(industry_classified[stockbasic.code]);
            }

            basiscs.Add(new Basic {
                code = index.hs300,
                name = "沪深300",
                nameabbr = "hs300",
                assettype = assettypes.index
            });
            basiscs.Add(new Basic
            {
                code = index.sz50,
                name = "上证50",
                nameabbr = "sz50",
                assettype = assettypes.index
            });
            basiscs.Add(new Basic
            {
                code = index.sz,
                name = "深圳成指",
                nameabbr = "sz",
                assettype = assettypes.index
            });
            basiscs.Add(new Basic
            {
                code = index.sh,
                name = "上证综指",
                nameabbr = "sh",
                assettype = assettypes.index
            });
            basiscs.Add(new Basic
            {
                code = index.cyb,
                name = "创业板指",
                nameabbr="cyb",
                assettype = assettypes.index
            });
            basiscs.Add(new Basic
            {
                code = index.zxb,
                name = "中小板指",
                nameabbr = "zxb",
                assettype = assettypes.index
            });

            var concepts = concept_classified
                .SelectMany(p => p.Value)
                .Where(p=>!string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();
            foreach(var i in concepts)
            {
                basiscs.Add(new Basic
                {
                    code = i,
                    name = i,
                    nameabbr = pinyin(i),
                    assettype = assettypes.sector
                });
            }

            var industries = industry_classified
                .SelectMany(p => p.Value)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();
            foreach (var i in industries)
            {
                basiscs.Add(new Basic
                {
                    code = i,
                    name = i,
                    nameabbr = pinyin(i),
                    assettype = assettypes.sector
                });
            }

            var indexdict = basiscs
                .Where(p => p.assettype == assettypes.index)
                .ToDictionary(p => p, p => basiscs
                                            .Where(p1=> p1.assettype == assettypes.stock)
                                            .Where(p1 => p1.belongtoindex(p.code))
                                            .ToArray());
            calc(indexdict);

            var sectordict = basiscs
                .Where(p => p.assettype == assettypes.sector)
                .ToDictionary(p => p, p => basiscs
                                            .Where(p1 => p1.assettype == assettypes.stock)
                                            .Where(p1 => p1.belongtosector(p.code))
                                            .ToArray());
            calc(sectordict);

            db.save(basiscs);

            db.save(basiscs.Select(p => new basicname { name = p.name, code = p.code, nameabbr = p.nameabbr }).ToArray());

            log.Info("**********DONE**********");
        }

        private string pinyin(string chinese)
        {
            var a = chinese.ToCharArray()
                .Select(p =>
                {
                    var v = PinyinHelper.ToHanyuPinyinStringArray(p);
                    if (v == null || !v.Any())
                        return p;

                    return v.First().First();
                })
                .ToArray();

            return string.Join("", a);
        }

        private void calc(Dictionary<Basic, Basic[]> dict)
        {   
            foreach(var basicP in dict)
            {
                var s = basicP.Key;
                var count = basicP.Value.Count();
                if (count == 0)
                    continue;

                s.pe = (basicP.Value
                    .Where(p => !(p.suspended && p.terminated && p.st))
                    .Select(p => p.pe.DoubleNull())
                    .Where(p => p.HasValue && p.Value > 0)
                    .Sum(p=>p.Value)/ count)
                    .ToString();

                s.pb = (basicP.Value
                    .Where(p => !(p.suspended && p.terminated && p.st))
                    .Select(p => p.pb.DoubleNull())
                    .Where(p => p.HasValue && p.Value > 0)
                    .Sum(p => p.Value)/ count)
                    .ToString();

                s.totalAssets = basicP.Value
                    .Select(p => p.totalAssets.DoubleNull())
                    .Where(p => p.HasValue)
                    .Sum(p => p.Value)
                    .ToString();

                s.liquidAssets = basicP.Value
                    .Select(p => p.liquidAssets.DoubleNull())
                    .Where(p => p.HasValue)
                    .Sum(p => p.Value)
                    .ToString();

                s.fixedAssets = basicP.Value
                    .Select(p => p.fixedAssets.DoubleNull())
                    .Where(p => p.HasValue)
                    .Sum(p => p.Value)
                    .ToString();
            }
        }

        Dictionary<string, string[]> code_names(Func<IEnumerable<dynamic>> get)
        {
            return get()
                .GroupBy(p => (string)p.code)
                .ToDictionary(p => p.Key, p => p.Select(p1 => (string)p1.name).ToArray());
        }
        Dictionary<string, string[]> code_cnames(Func<IEnumerable<dynamic>> get)
        {
            return get()
                .GroupBy(p => (string)p.code)
                .ToDictionary(p => p.Key, p => p.Select(p1 => (string)p1.c_name).ToArray());
        }
    }
}