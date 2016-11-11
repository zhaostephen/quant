using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screen.Mixin;

namespace Screen.Utility
{
    public static class MathUtil
    {
        /// <summary>
        /// y = α + β1 * x1 + β2 * x2
        /// </summary>B
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Func<double, double, double> LinearRegression(double[] x1, double[] x2, double[] y)
        {
            double beta1 = (D(x1, y) * D(x2) - D(x2, y) * D(x1, x2)) / (D(x1) * D(x2) - D(x1, x2));
            double beta2 = (D(x2, y) * D(x1) - D(x1, y) * D(x1, x2)) / (D(x1) * D(x2) - D(x1, x2));
            double alpha = E(y) - beta1 * E(x1) - beta2 * E(x2);

            return new Func<double, double, double>((v1, v2) => alpha + beta1 * v1 + beta2 * v2);
        }
        /// <summary>
        /// LinearRegression_A
        /// y = α + β * x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Func<double, double> LinearRegression(double[] x, double[] y)
        {
            double beta = D(x, y) / D(x);
            double alpha = E(y) - beta * E(x);

            return new Func<double, double>(v => alpha + beta * v);
        }
        public static double beta(double[] x, double[] y)
        {
            double beta = D(x, y) / D(x);
            return beta;
        }
        public static double alpha(double[] x, double[] y)
        {
            double beta = D(x, y) / D(x);
            double alpha = E(y) - beta * E(x);

            return alpha;
        }
        /// <summary>
        /// D(x,y)/sqrt(D(x)*D(y))
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Correlation(double[] x, double[] y)
        {
            return D(x, y) / Math.Sqrt(D(x) * D(y));
        }
        /// <summary>
        /// E(x*y)-E(x)*E(y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Covariance(double[] x, double[] y)
        {
            return E(x.multiply(y)) - E(x) * E(y);
        }
        /// <summary>
        /// sqrt(D(x))
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Stdev(double[] x)
        {
            return Math.Sqrt(D(x));
        }
        /// <summary>
        /// E(x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double D(double[] x)
        {
            double result = 0.0d;
            for (int i = 0; i < x.Length; ++i)
                result += (x[i] - E(x)) * (x[i] - E(x));
            return result / (x.Length - 1);
        }
        public static double D(double[] x, double[] y)
        {
            double result = 0.0d;
            for (int i = 0; i < x.Length; ++i)
                result += (x[i] - E(x)) * (y[i] - E(y));
            return result / (x.Length - 1);
        }
        public static double E(double[] x)
        {
            return x.Average();
        }
        /// <summary>
        /// normal distribution
        /// </summary>
        /// <returns>propbility</returns>
        public static double N(double x)
        {
            double result = 0d;
            for (double t = x - 100; t <= x; t += 0.01)
            {
                result += Math.Exp(-t * t / 2) / Math.Sqrt(2 * Math.PI) * 0.01;
            }

            return result;
        }
        /// <summary>
        /// normal distribution的一阶导
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double N_d1(double x)
        {
            double result = 0d;
            for (double t = double.MinValue; t <= x; t += 0.01)
            {
                result += Math.Exp(-t / 2) * 0.01;
            }

            return result;
        }
        public static double T(double days)
        {
            return days / 365d;
        }
    }
}
