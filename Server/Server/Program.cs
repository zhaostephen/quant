using System;
using Topshelf.FileSystemWatcher;
using Screen.Utility;
using Topshelf;
using log4net;
using Topshelf.HostConfigurators;

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

        static void Go(HostConfigurator x)
        {
            x.Service<ScreenService>(
                (s) =>
                {
                    svc.Initialize();

                    s.ConstructUsing(c => svc);
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());

                    s.WhenFileSystemChanged(cfg => { }, e => svc.FileChange(e));
                    s.WhenFileSystemCreated(cfg => { }, e => svc.FileChange(e));
                    s.WhenFileSystemDeleted(cfg => { }, e => { });
                });
            x.SetStartTimeout(TimeSpan.FromSeconds(60));
            x.SetStopTimeout(TimeSpan.FromSeconds(300));
            x.StartManually();
            if (!string.IsNullOrEmpty(cmdLine))
            {
                if (cmdLine.Contains("install") || cmdLine.Contains("uninstall") || cmdLine.Contains("run"))
                {
                    log.InfoFormat("Using command line: {0}", cmdLine);
                    x.ApplyCommandLine(cmdLine);
                }
            }
            x.SetDescription(string.Format("{0}({1})", SERVICE_DESC, SERVICE_NAME));
            x.SetServiceName(SERVICE_NAME);
            x.UseLog4Net();
        }
    }
}
