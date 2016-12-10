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
    [command("kdata")]
    class kdatacmd : command<kdatacmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(kdatacmd));

        public kdatacmd(string[] args) : base(args) { }

        public override void exec()
        {
            var client = new Trade.Db.db();
            var query = 
                client.kdata(param.code, param.ktype)
                        .Where(p => p.date == param.date || !param.date.HasValue);
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
            [Option('k', "ktype", Required = true)]
            public string ktype { get; set; }
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
