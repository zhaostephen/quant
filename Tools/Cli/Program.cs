using CommandLine;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.Cfg;
using Trade.Data;
using Trade.Utility;

namespace Trade.Cli
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                log.Warn("trade-cli <command> <param>");
                return;
            }

            var command = args[0];

            var parameters = new Parameters();
            if(!Parser.Default.ParseArguments(args.Skip(1).ToArray(), parameters))
            {
                log.Warn("trade-cli <command> <param>");
                return;
            }

            var client = new MktDataClient();
            IEnumerable<DataPoint> query = null;
            switch (command.ToLower())
            {
                case "min5":
                    query = client.Query(parameters.code, PeriodEnum.Min5)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break;
                case "min15":
                    query = client.Query(parameters.code, PeriodEnum.Min15)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break; 
                case "min30":
                    query = client.Query(parameters.code, PeriodEnum.Min30)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break;
                case "min60":
                    query = client.Query(parameters.code, PeriodEnum.Min60)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break;
                case "daily":
                    query = client.Query(parameters.code, PeriodEnum.Daily)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break;
                case "weekly":
                    query = client.Query(parameters.code, PeriodEnum.Weekly)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break;
                case "monthly":
                    query = client.Query(parameters.code, PeriodEnum.Monthly)
                        .Where(p => p.Date == parameters.date || !parameters.date.HasValue);
                    break;
            }
            if (query != null && !string.IsNullOrEmpty(parameters.field))
            {
                var r = query.Select(p =>
                {
                    object v;
                    if (GetPropertyByName(p, parameters.field, out v))
                        return v;
                    return null;
                })
                .Where(p => p != null)
                .ToArray();

                log.Info(r.ToCsv());
            }
            else
                log.Info(query.ToArray().ToCsv());
        }

        static bool GetPropertyByName(object obj, string property, out object value)
        {
            value = null;

            var type = obj.GetType();
            var prop = type.Properties().FirstOrDefault(p => string.Equals(p.Name, property, StringComparison.InvariantCultureIgnoreCase));
            if(prop != null)
            {
                value = prop.GetValue(obj);
                return true;
            }
            return false;
        }
    }

    class Parameters
    {
        [Option('c', "code", Required = true)]
        public string code { get; set; }
        [Option('d', "date", Required = false)]
        public string _date { get; set; }
        public DateTime? date { get { return string.IsNullOrEmpty(_date) ? (DateTime?)null : DateTime.Parse(_date); } }
        [Option('f', "field", Required = false)]
        public string field { get; set; }
    }
}
