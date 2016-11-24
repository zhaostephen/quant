using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Interace.Mixin
{
    public static class PathMixin
    {
        public static string EnsurePathCreated(this string path)
        {
            var s = new Stack<string>();
            var dir = path;
            while (true)
            {
                if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
                    break;

                s.Push(dir);

                dir = Path.GetDirectoryName(dir);
            }

            while (s.Count > 0)
            {
                Directory.CreateDirectory(s.Pop());
            }

            return path;
        }
    }
}
