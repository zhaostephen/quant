using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Utility
{
    public static class LogMixin
    {
        public static ILog Log(this object obj)
        {
            return LogManager.GetLogger(obj.ToString());
        }
    }
}
