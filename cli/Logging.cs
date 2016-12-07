using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cli
{
    static class LogMixin
    {
        static bool configured = false;
        public static ILog Log(this object obj)
        {
            if (!configured)
            {
                log4net.Config.XmlConfigurator.Configure();
                configured = true;
            }

            return LogManager.GetLogger(obj.ToString());
        }
    }
}
