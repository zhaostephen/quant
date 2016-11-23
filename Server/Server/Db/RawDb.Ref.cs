using System;
using System.Collections.Generic;
using System.Linq;
using Trade.Data;
using System.IO;
using System.Text.RegularExpressions;
using Trade.Mixin;
using Trade.Utility;
using log4net;
using ServiceStack;
using Trade.Cfg;
using Excel;
using System.Data;

namespace Trade.Db
{
    partial class RawDb
    {
        public IEnumerable<Fundamental> QueryFundamentals()
        {
            var caiwushuju = QueryFundamentals(Path.Combine(Configuration.Raw.fundamental, "caiwushuju.xls"));
            var hangqing = QueryFundamentals(Path.Combine(Configuration.Raw.fundamental, "hangqing.xls"));
            var sector = QueryFundamentals(Path.Combine(Configuration.Raw.fundamental, "sector.xls"));

            var result = caiwushuju.Merge(hangqing,sector);

            return result.Select(p=>new Fundamental()
            {
                代码 = result.select(p.Key,"代码"),
                名称 = result.select(p.Key,"名称"),
                更新日期 = result.select(p.Key,"更新日期"),
                总股本 = result.select(p.Key,"总股本"),
                流通A股 = result.select(p.Key,"流通A股"),
                人均持股数 = result.select(p.Key,"人均持股数"),
                每股收益 = result.select(p.Key,"每股收益"),
                每股净资产 = result.select(p.Key,"每股净资产"),
                加权净资产收益率 = result.select(p.Key,"加权净资产收益率"),
                营业总收入 = result.select(p.Key,"营业总收入"),
                营业总收入同比 = result.select(p.Key,"营业总收入同比"),
                营业利润 = result.select(p.Key,"营业利润"),
                投资收益 = result.select(p.Key,"投资收益"),
                利润总额 = result.select(p.Key,"利润总额"),
                净利润 = result.select(p.Key,"净利润"),
                净利润同比 = result.select(p.Key,"净利润同比"),
                未分配利润 = result.select(p.Key,"未分配利润"),
                每股未分配利润 = result.select(p.Key,"每股未分配利润"),
                销售毛利率 = result.select(p.Key,"销售毛利率"),
                总资产 = result.select(p.Key,"总资产"),
                流动资产 = result.select(p.Key,"流动资产"),
                固定资产 = result.select(p.Key,"固定资产"),
                无形资产 = result.select(p.Key,"无形资产"),
                总负债 = result.select(p.Key,"总负债"),
                流动负债 = result.select(p.Key,"流动负债"),
                长期负债 = result.select(p.Key,"长期负债"),
                资产负债比率 = result.select(p.Key,"资产负债比率"),
                股东权益 = result.select(p.Key,"股东权益"),
                股东权益比 = result.select(p.Key,"股东权益比"),
                公积金 = result.select(p.Key,"公积金"),
                每股公积金 = result.select(p.Key,"每股公积金"),
                流通B股 = result.select(p.Key,"流通B股"),
                H股 = result.select(p.Key,"H股"),
                上市日期 = result.select(p.Key,"上市日期"),
                市盈率 = result.select(p.Key,"市盈率", "市盈(动)"),
                所属行业 = result.select(p.Key,"所属行业"),
                市净率 = result.select(p.Key,"市净率"),
                总市值 = result.select(p.Key,"总市值"),
                流通股本 = result.select(p.Key,"流通股本"),
                流通市值 = result.select(p.Key,"流通市值"),
            })
            .ToArray();
        }

        private Dictionary<string, Dictionary<string, string>> QueryFundamentals(string file, string key= "代码")
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
            using (var excelReader = ExcelReaderFactory.CreateBinaryReader(stream))
            {
                excelReader.IsFirstRowAsColumnNames = true;

                var result = excelReader.AsDataSet();
                if (result.Tables.Count == 0)
                    return new Dictionary<string, Dictionary<string, string>>();

                var set = new Dictionary<string, Dictionary<string, string>>();
                var table = result.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    var code = row[key].ToString();
                    var f = new Dictionary<string, string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        f[column.ColumnName] = row[column].ToString();
                    }
                    set[code] = f;
                }

                return set;
            }
        }
    }
}
