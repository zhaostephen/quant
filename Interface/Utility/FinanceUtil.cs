using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Utility
{
    public static class FinanceUtil
    {
        /// <summary>
        /// payment
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <param name="pv"></param>
        /// <param name="fv"></param>
        /// <returns></returns>
        public static double PMT(double r, int n, double pv, double fv = 0d)
        {
            pv = pv + PV(r, n, 0, fv);
            return pv / ArrayMixin.SumOfEquality(1 + r, n);
        }
        /// <summary>
        /// present value for perpetual annuity formula
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <param name="pmt"></param>
        /// <param name="fv"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public static double PVAF(double r, double pmt)
        {
            return pmt / r;
        }
        /// <summary>
        /// present value
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <param name="pmt"></param>
        /// <param name="fv"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public static double PV(double r, int n, double pmt, double fv = 0d)
        {
            return fv / Math.Pow(1 + r, n) + CD(r, n, pmt, false);
        }
        /// <summary>
        /// future value
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <param name="pmt"></param>
        /// <param name="pv"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public static double FV(double r, int n, double pmt, double pv = 0d)
        {
            return pv * Math.Pow(1 + r, n) + CI(r, n, pmt, true);
        }
        /// <summary>
        /// compound interest
        /// </summary>
        /// <param name="r"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double CI(double r, int n, double pmt, bool immediate = false)
        {
            double result = 0d;
            double from = immediate ? 0 : 1;
            double to = immediate ? (n - 1) : n;
            for (double i = from; i <= to; ++i)
                result += pmt * Math.Pow(1 + r, i);

            return result;
        }
        /// <summary>
        /// compound discount
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <param name="pmt"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        public static double CD(double r, int n, double pmt, bool immediate = false)
        {
            double result = 0d;
            double from = immediate ? 0 : 1;
            double to = immediate ? (n - 1) : n;
            for (double i = from; i <= to; ++i)
                result += pmt / Math.Pow(1 + r, i);

            return result;
        }
        /// <summary>
        /// continuous interest
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double ContI(double r, int n, double pmt, bool immediate = false)
        {
            double result = 0d;
            double from = immediate ? 0 : 1;
            double to = immediate ? (n - 1) : n;
            for (double i = from; i <= to; ++i)
                result += pmt * Math.Pow(Math.E, r * i);

            return result;
        }
        /// <summary>
        /// continuous discount
        /// </summary>
        /// <param name="r"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double ContD(double r, int n, double pmt, bool immediate = false)
        {
            double result = 0d;
            double from = immediate ? 0 : 1;
            double to = immediate ? (n - 1) : n;
            for (double i = from; i <= to; ++i)
                result += pmt / Math.Pow(Math.E, r * i);

            return result;
        }
    }
}
