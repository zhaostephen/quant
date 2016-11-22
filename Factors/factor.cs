using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Factors
{
    public class factorset
    {
        public string 代码{ get; set; }
        public double? 低点反弹高度 { get; set; }
        public double? 收阳百分比 { get; set; }
        public bool 均线多头 { get; set; }

        public factorset()
        {

        }
        public factorset(string code)
        {
            this.代码 = code;
        }
    }

    public class factor<T>
    {
        public T value { get; set; }

        public factor(StkDataSeries series)
        {
        }

        public override string ToString()
        {
            return value + "";
        }
    }
}
