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
            log4net.Config.XmlConfigurator.Configure();

            return LogManager.GetLogger(obj.ToString());
        }
    }
}
