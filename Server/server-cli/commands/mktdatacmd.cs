using Cli;
using CommandLine;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using Trade.Cfg;
using Trade.Data;

namespace Trade.Cli.commands
{
    [command("mktdata", "mkt", "hangqing")]
    class mktdatacmd : command<mktdatacmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(mktdatacmd));

        public mktdatacmd(string[] args) : base(args) { }

        public override void exec()
        {
            var client = new MktDataClient();
            IEnumerable<kdatapoint> query = null;
            switch (param.period)
            {
                case "min5":
                    query = client.Query(param.code, PeriodEnum.Min5)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                case "min15":
                    query = client.Query(param.code, PeriodEnum.Min15)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                case "min30":
                    query = client.Query(param.code, PeriodEnum.Min30)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                case "min60":
                    query = client.Query(param.code, PeriodEnum.Min60)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                case "daily":
                    query = client.Query(param.code, PeriodEnum.Daily)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                case "weekly":
                    query = client.Query(param.code, PeriodEnum.Weekly)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                case "monthly":
                    query = client.Query(param.code, PeriodEnum.Monthly)
                        .Where(p => p.Date == param.date || !param.date.HasValue);
                    break;
                default:
                    throw new Exception("Unknown period "+param.period);
            }
            if (query != null && !string.IsNullOrEmpty(param.field))
            {
                var r = query.Select(p =>
                {
                    object v;
                    if (GetPropertyByName(p, param.field, out v))
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
            if (prop != null)
            {
                value = prop.GetValue(obj);
                return true;
            }
            return false;
        }

        public class Parameters
        {
            [Option('p', "period", Required = true)]
            public string period { get; set; }
            [Option('c', "code", Required = true)]
            public string code { get; set; }
            [Option('d', "date", Required = false)]
            public string _date { get; set; }
            public DateTime? date { get { return string.IsNullOrEmpty(_date) ? (DateTime?)null : DateTime.Parse(_date); } }
            [Option('f', "field", Required = false)]
            public string field { get; set; }
        }
    }
}
