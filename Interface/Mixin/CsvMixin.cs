using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Trade.Mixin;

namespace Interace.Mixin
{
    public static class CsvMixin
    {
        public static IEnumerable<T> ReadCsv<T>(this string path) where T:new()
        {
            if (!File.Exists(path))
                return Enumerable.Empty<T>();

            var lines = File.ReadAllLines(path);
            if (!lines.Any())
                return Enumerable.Empty<T>();

            var columns = lines[0].Split(new[] { ',' });
            return lines
                .Skip(1)
                .Select(p =>
                {
                    var splits = p.Split(new[] { ',' });
                    var f = new T();
                    for (var i = 0; i < columns.Length; ++i)
                    {
                        var column = columns[i];
                        f.SetPropertyValue(column, splits[i]);
                    }
                    return f;
                })
                .ToArray();
        }
    }
}
