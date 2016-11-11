using System;
using Topshelf.FileSystemWatcher;
using Screen.Utility;
using Topshelf;
using log4net;
using Topshelf.HostConfigurators;
using System.IO;

namespace Screen
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static string SERVICE_NAME = "ScreenService";
        static string SERVICE_DESC = "Screen Service";
        static string cmdLine = string.Empty;
        static ScreenService svc;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            string cmdLine = string.Empty;
            if (args != null && args.Length > 0)
                cmdLine = string.Join(" ", args);

            svc = new ScreenService();

            var code = HostFactory.Run(Go);
            if (code != TopshelfExitCode.Ok)
            {
                throw new Exception(string.Format("Topshelf error code name: {0}, value: {1}", code.ToString(), (int)code));
            }
        }

        static void Go(HostConfigurator svchost)
        {
            svchost.Service<ScreenService>(
                (s) =>
                {
                    s.ConstructUsing(c => svc);
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());

                    s.WhenFileSystemChanged((cfg) =>
                    {
                        cfg.AddDirectory((dir) =>
                        {
                            dir.Path = @"D:\screen\Data";
                            dir.IncludeSubDirectories = false;
                            dir.FileFilter = "*.txt";
                            dir.NotifyFilters = NotifyFilters.LastWrite;
                        });
                    }, e => svc.FileChange(e));
                });
            svchost.SetStartTimeout(TimeSpan.FromSeconds(60));
            svchost.SetStopTimeout(TimeSpan.FromSeconds(300));
            svchost.StartManually();
            if (!string.IsNullOrEmpty(cmdLine))
            {
                if (cmdLine.Contains("install") || cmdLine.Contains("uninstall") || cmdLine.Contains("run"))
                {
                    log.InfoFormat("Using command line: {0}", cmdLine);
                    svchost.ApplyCommandLine(cmdLine);
                }
            }
            svchost.SetDescription(string.Format("{0}({1})", SERVICE_DESC, SERVICE_NAME));
            svchost.SetServiceName(SERVICE_NAME);
            svchost.UseLog4Net();
        }
    }
}
