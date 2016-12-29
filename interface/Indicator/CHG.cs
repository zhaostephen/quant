using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Indicator
{
    public class CHG : Series<double>
    {
        public CHG(Series<double> data, int precision=2)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (i == 0) Add(data[i].Date, 0d);
                else Add(data[i].Date, Math.Round((data[i].Value - data[i - 1].Value) / data[i - 1].Value * 100d, precision));
            }
        }
    }
}
