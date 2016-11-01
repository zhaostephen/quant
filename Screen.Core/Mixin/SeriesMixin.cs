using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Mixin
{
    public static class SeriesMixin
    {
        public static void Dump(this DataSeries @this)
        {
            const string format = "{0,-15:MM/dd/yyyy}{1,-10}{2,-10}{3,-10}{4,-10}";
            Console.WriteLine(format, "日期", "开盘", "最高", "最低", "收盘");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.Date, i.Open, i.High, i.Low, i.Close);
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count);
            Console.WriteLine("-----------------------------------------------------------------");
        }

        public static void Dump(this IEnumerable<Security> @this)
        {
            const string format = "{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}";
            Console.WriteLine(format, "代码", "名称", "流通股本", "细分行业", "地区");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.代码, i.名称, i.流通股本, i.细分行业, i.地区);
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count());
            Console.WriteLine("-----------------------------------------------------------------");
        }

        public static void Dump(this TimeSeries<double> @this)
        {
            const string format = "{0,-15:MM/dd/yyyy}{1,-10}";
            Console.WriteLine(format, "日期", "值");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.Date, i.Value);
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count);
            Console.WriteLine("-----------------------------------------------------------------");
        }

        public static void Dump(this JAXSeries @this)
        {
            const string format = "{0,-15:MM/dd/yyyy}{1,-10}{2,-10}{3,-10}{4,-10}{5,-10}";
            Console.WriteLine(format, "日期", "济安线", "J", "A", "X","upward");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.Date, i.JAX, i.J, i.A, i.X, i.upward ? "Y" : "N");
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count);
            Console.WriteLine("-----------------------------------------------------------------");
        }

        public static void Dump(this MACDSeries @this)
        {
            const string format = "{0,-15:MM/dd/yyyy}{1,-10}{2,-10}{3,-10}";
            Console.WriteLine(format, "日期", "DIF", "DEA", "MACD");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.Date, i.DIF, i.DEA, i.MACD);
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count);
            Console.WriteLine("-----------------------------------------------------------------");
        }

        public static void Dump(this BREAKSeries @this)
        {
            const string format = "{0,-15:MM/dd/yyyy}{1,-10}{2,-15:MM/dd/yyyy}{3,-10}";
            Console.WriteLine(format, "日期", "值", "前高日", "前高值");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.Date, i.Value, i.Peak.Date, i.Peak.Value);
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count);
            Console.WriteLine("-----------------------------------------------------------------");
        }

        public static void Dump(this BREAKBACKSeries @this)
        {
            const string format = "{0,-15:MM/dd/yyyy}{1,-10}{2,-15:MM/dd/yyyy}{3,-10}{4,-15:MM/dd/yyyy}{5,-10}{6,-15:MM/dd/yyyy}{7,-10}";
            Console.WriteLine(format, "日期", "值", "突破日", "突破值", "前高日", "前高值", "前低日", "前低值");
            foreach (var i in @this)
            {
                Console.WriteLine(format, i.Date, i.Value, i.Break.Date, i.Break.Value, i.Break.Peak.Date, i.Break.Peak.Value, i.Bottom.Date, i.Bottom.Value);
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("total {0}", @this.Count);
            Console.WriteLine("-----------------------------------------------------------------");
        }
    }
}
