using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class Stock
    {
        public string 代码 { get; set; }
        public string 名称 { get; set; }
        public double 流通股本 { get; set; }
        public string 细分行业 { get; set; }
        public string 地区 { get; set; }
    }

    public class StockB
    {
        public string 代码 { get; set; }
        public string 名称 { get; set; }
        public double 持有数量 { get; set; }
        public double 占流通股比例 { get; set; }
        public double 占净值比 { get; set; }
    }

    public class FundB
    {
        public string 代码 { get; set; }
        public string 名称 { get; set; }
        public StockB[] SecurityBs { get; set; }
    }
}
