using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public class DataPoint
    {
        public DateTime Date { get; set; }
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
}
