using CommandLine;
using log4net;
using ServiceStack;
using System;
using System.Linq;
using System.Reflection;

namespace Cli
{
    interface ICommand
    {
        void exec();
    }

    abstract class command<Tparam> : ICommand where Tparam : new()
    {
        static ILog log = LogManager.GetLogger(typeof(ICommand));

        protected Tparam param;

        public command()
        {

        }
        public command(string[] args)
        {
            param = new Tparam();
            if (args.Any())
            {
                if (!Parser.Default.ParseArguments(args, param))
                {
                    help();
                    return;
                }
            }
        }

        protected static void help()
        {
            log.Warn(Assembly.GetExecutingAssembly().GetName().Name.Replace(".exe", "") + " <command> <param>");
        }

        public abstract void exec();
    }

    class commandfactory
    {
        static ILog log = LogManager.GetLogger(typeof(commandfactory));
        static Tuple<string[], Type>[] _commands = new Tuple<string[], Type>[0];

        static commandfactory()
        {
            var assem = typeof(commandfactory).Assembly;
            _commands = assem.GetTypes()
                .Where(p => Attribute.GetCustomAttribute(p, typeof(commandAttribute)) != null)
                .Select(p => Tuple.Create((Attribute.GetCustomAttribute(p,typeof(commandAttribute)) as commandAttribute).Commands, p))
                .ToArray();
        }

        public static void exec(string command, string[] args)
        {
            command = command.ToLower();

            log.Info("exec " + command);

            var o = _commands.FirstOrDefault(p=>p.Item1.Contains(command));
            if (o == null)
            {
                help();
                return;
            }

            var cmd = Activator.CreateInstance(o.Item2,new object[] { args }) as ICommand;
            cmd.exec();
        }

        static void help()
        {
            log.Warn(Assembly.GetExecutingAssembly().GetName().Name.Replace(".exe", "") + " <command> <param>");
        }
    }

    class commandAttribute : Attribute
    {
        public string[] Commands { get; set; }

        public commandAttribute(params string[] commands)
        {
            Commands = commands.Select(p=>p.ToLower()).ToArray();
        }
    }
}
