using System;
using Topshelf.FileSystemWatcher;
using Trade.Utility;
using Topshelf;
using log4net;
using Topshelf.HostConfigurators;
using System.IO;

namespace Trade
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static string cmdLine = string.Empty;
        static Service svc;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            if (args != null && args.Length > 0)
                cmdLine = string.Join(" ", args);

            svc = new Service();
            svc.Start(args[0]);
        }
    }
}
