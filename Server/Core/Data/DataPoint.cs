using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Data
{
    public class DataPoint
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double PreClose { get; set; }

        public double NetChange { get; set; }
        public double PctChange { get; set; }
        public double HighNetChange { get; set; }
        public double HighPctChange { get; set; }
        public double LowNetChange { get; set; }
        public double LowPctChange { get; set; }
        public double OpenNetChange { get; set; }
        public double OpenPctChange { get; set; }

        public double? MACD { get; set; }
        public double? DIF { get; set; }
        public double? DEA { get; set; }
        public double? MA5 { get; set; }
        public double? MA10 { get; set; }
        public double? MA20 { get; set; }
        public double? MA30 { get; set; }
        public double? MA55 { get; set; }
        public double? MA60 { get; set; }
        public double? MA120 { get; set; }
        public double? MA250 { get; set; }

        public double HighLowNetChange { get; set; }
        public double HighLowPctChange { get; set; }
        public double AbsNetChange { get { return Math.Abs(NetChange); } }
        public double AbsPctChange { get { return Math.Abs(PctChange); } }
        public double AbsHighLowNetChange { get { return Math.Abs(HighLowNetChange); } }
        public double AbsHighLowPctChange { get { return Math.Abs(HighLowPctChange); } }
        public CloseEnum CloseEnum { get { return NetChange == 0d ? CloseEnum.收平 : (NetChange > 0d ? CloseEnum.收阳 : Data.CloseEnum.收阴); } }
    }

    public static class DataPointMixin
    {
        public static DataPoint[] NetPctChange(this DataPoint[] data)
        {
            for (var i = 1; i < data.Length; ++i)
            {
                var preClose = data[i - 1].Close;
                data[i].PreClose = preClose;

                data[i].NetChange = data[i].Close - preClose;
                data[i].PctChange = Math.Round(((data[i].Close - preClose) / preClose) * 100, 2);

                data[i].LowNetChange = data[i].Low - preClose;
                data[i].LowPctChange = Math.Round(((data[i].Low - preClose) / preClose) * 100, 2);

                data[i].HighNetChange = data[i].High - preClose;
                data[i].HighPctChange = Math.Round(((data[i].High - preClose) / preClose) * 100, 2);

                data[i].OpenNetChange = data[i].Open - preClose;
                data[i].OpenPctChange = Math.Round(((data[i].Open - preClose) / preClose) * 100, 2);

                data[i].HighLowNetChange = data[i].High - data[i].Low;
                data[i].HighLowPctChange = Math.Round(((data[i].High - data[i].Low) / preClose) * 100, 2);
            }

            return data;
        }
    }
}
