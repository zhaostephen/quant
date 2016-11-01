using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Utility
{
    public static class ArrayMixin
    {
        public static double[] sub(this double[] x, double[] y)
        {
            return op(x, y, (v1, v2) => v1 - v2);
        }

        public static double[] add(this double[] x, double[] y)
        {
            return op(x, y, (v1, v2) => v1 + v2);
        }

        public static double[] multiply(this double[] x, double[] y)
        {
            return op(x, y, (v1, v2) => v1 * v2);
        }

        public static double[] div(this double[] x, double[] y)
        {
            return op(x, y, (v1, v2) => v1 * v2);
        }

        public static double SumOfEquality(this double[] x)
        {
            if (x.Length == 1) return x[0];
            double q = x[1] / x[0];
            int n = x.Length;
            return x[0] * (1 - Math.Pow(q, n)) / (1 - q);
        }

        public static double SumOfEquality(double q, int n)
        {
            return SumOfEquality(EqualityRange(q, n));
        }

        public static double[] EqualityRange(double q, int n)
        {
            return Enumerable.Range(1, n).Select(p => 1 / Math.Pow(q, p)).ToArray();
        }

        public static T[] op<T>(T[] x, T[] y, Func<T, T, T> eval) where T : struct
        {
            int length = Math.Min(x.Length, y.Length);
            T[] xy = new T[length];
            for (int i = 0; i < length; ++i)
                xy[i] = eval(x[i], y[i]);

            return xy;
        }
    }
}
