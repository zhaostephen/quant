using log4net;
using Screen.Data;
using Screen.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Screen
{
    class Program
    {
        static ILog log = typeof(Program).Log();

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var client = new MktDataClient();
            var d = client.Query("000001");
        }

        private static IEnumerable<object> stat(IEnumerable<StkDataSeries> d, DateTime since)
        {
            var results = d.Select(p =>
            {
                var current = p.Last();
                var s = p.Where(p1 => p1.Date >= since);
                if (!s.Any()) return null;

                var lowest = s.Min(p1 => p1.Low);

                return new
                {
                    Name = p.Code,
                    Date = current.Date,
                    Open = current.Open,
                    Close = current.Close,
                    High = current.High,
                    Low = current.Low,
                    CloseEnum = current.CloseEnum,

                    Lowest = lowest,
                    Low2Low = Math.Truncate((current.Low / lowest - 1) * 100)
                };
            })
            .Where(p => p != null);

            var today = results.Max(p => p.Date);

            results = results
                .Where(p => p.Date == today)//exclude suspending
                .OrderBy(p => p.Low2Low);

            return results.ToArray();
        }
    }
}
