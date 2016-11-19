using Trade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Mixin;

namespace Trade.Indicator
{
    public class STATE : TimeSeries<StateEnum>
    {
        public STATE(DataSeries data)
        {
            foreach (var d in data)
            {
                var state = ParseState(d);
                this.Add(new TimePoint<StateEnum>(d.Date, state));
            }
        }

        private StateEnum ParseState(DataPoint d)
        {
            //回调幅度超过6个点
            if (d.CloseEnum == CloseEnum.收阴 &&
                d.AbsPctChange >= 6)
                return StateEnum.异常回调;

            //冲高回落6个点
            if (d.AbsHighLowPctChange >= 6 && d.Close < d.High)
                return StateEnum.危险信号;

            //涨停打开之后，尾盘没有封住涨停
            if (Math.Round(d.HighPctChange,0) >= 10 && d.Close < d.High)
                return StateEnum.危险信号;

            //6个点以内的回调
            if (d.CloseEnum == CloseEnum.收平 ||
                (d.CloseEnum == CloseEnum.收阴 && d.AbsPctChange.Between(0, 6)))
                return StateEnum.自然回调;

            if (d.CloseEnum == CloseEnum.收平 || d.CloseEnum == CloseEnum.收阳)
                return StateEnum.自然回升;

            return StateEnum.不确定;
        }
    }
}
