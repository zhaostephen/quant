using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screen.Mixin;

namespace Screen.Indicator
{
    public class MACD : MACDSeries
    {
        public MACD(DataSeries data, int MID = 9, int SHORT = 12, int LONG = 26)
        {
            var EMA_SHORT = new EMA(data.CloseTimeSeries(), SHORT);
            var EMA_MID = new EMA(data.CloseTimeSeries(), MID);
            var EMA_LONG = new EMA(data.CloseTimeSeries(), LONG);

            var DIF = (from s in EMA_SHORT
                      join l in EMA_LONG on s.Date equals l.Date
                      select new TimePoint<double>{ Date = s.Date, Value = s.Value - l.Value }).ToArray();
            var DEA = new EMA(new TimeSeries<double>(DIF.ToArray()), MID);
            var MACD = (from dif in DIF
                       join dea in DEA on dif.Date equals dea.Date
                       select new TimePoint<double> { Date = dif.Date, Value = (dif.Value - dea.Value) * 2 }).ToArray();

            //DIF:EMA(CLOSE,SHORT)-EMA(CLOSE,LONG);
            //DEA:EMA(DIF,MID);
            //MACD:(DIF-DEA)*2,COLORSTICK;
            var result = (from dif in DIF
                          join dea in DEA on dif.Date equals dea.Date
                          join macd in MACD on dif.Date equals macd.Date
                          select new MACDPoint { Date = dif.Date, DEA = Math.Round(dea.Value, 2), MACD = Math.Round(macd.Value, 2), DIF = Math.Round(dif.Value, 2) }).ToArray();

            this.AddRange(result);
        }
    }
}
