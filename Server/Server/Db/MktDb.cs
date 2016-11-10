using Screen.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Screen.Mixin;
using Screen.Utility;
using log4net;
using ServiceStack;

namespace Screen.Db
{
    public class MktDb
    {
        static ILog log = typeof(MktDb).Log();

        public IEnumerable<StkDataSeries> QueryMany(string path)
        {
            log.Info("Search files...");
            var fileArray = Directory.GetFiles(path, "*.txt");

            log.Info("Read data...");
            return fileArray
                .AsParallel()
                .Select(file => Query(file))
                .Where(p => p != null)
                .ToArray();
        }

        public StkDataSeries Query(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path).Replace("SH", "").Replace("SZ", "").Replace("#", "");
            var lines = File.ReadAllLines(path);

            var data = lines
                .Select(p =>
                {
                    var splits = p.Split(new[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var isDate = Regex.IsMatch(splits[0], @"\d\d\d\d/\d\d/\d\d");
                    if (!isDate) return null;

                    return new DataPoint
                    {
                        Date = splits[0].Date(),
                        Open = splits[1].Double(),
                        High = splits[2].Double(),
                        Low = splits[3].Double(),
                        Close = splits[4].Double()
                    };
                })
                .Where(p => p != null && p.Open > 0d && p.Close > 0d)
                .OrderBy(p => p.Date)
                .ToArray();

            if (!data.Any())
                return null;

            return new StkDataSeries(name, new DataSeries(data.NetPctChange()));
        }

        public void Save(IEnumerable<object> data, string path)
        {
            while (true)
            {
                var dir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
                    break;
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, data.ToCsv(), Encoding.UTF8);
        }

        public void SaveMany(IEnumerable<StkDataSeries> dataset, string path)
        {
            foreach(var d in dataset)
            {
                Save(d, Path.Combine(path, d.Code + ".csv"));
            }
        }
    }
}
