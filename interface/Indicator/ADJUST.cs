using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Indicator
{
    public class ADJUST
    {
        public double high { get; set; }
        public double low { get; set; }
        public double move { get; set; }
        public double t1 { get; set; }
        public double t2 { get; set; }
        public double t3 { get; set; }

        public ADJUST()
        {

        }
        public ADJUST(double h, double l)
        {
            high = h;
            low = l;
            move = h - l;
            t1 = h - 0.33 * move;
            t2 = h - 0.5 * move;
            t3 = h - 0.66 * move;
        }
    }
}
