using Trade.Cfg;
using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Mixin
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
    }
}
