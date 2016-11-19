using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public enum PhaseEnum
    {
        不确定,
        建仓,
        洗盘,
        拉升,
        出货
    }

    public enum StateEnum
    {
        不确定,
        自然回升,
        自然回调,
        轻度危险信号,
        危险信号,
        异常回调
    }

    public enum TrendEnum
    {
        不确定,
        趋势上升,
        趋势下降
    }

    public enum CloseEnum
    {
        收平,
        收阴,
        收阳
    }
}
