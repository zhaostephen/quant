using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Mixin
{
    static class DictMixin
    {
        public static string select(this Dictionary<string, Dictionary<string, string>> @this, string k1, params string[] k2s)
        {
            if (!@this.ContainsKey(k1)) return string.Empty;
            foreach (var k2 in k2s)
            {
                if (@this[k1].ContainsKey(k2))
                    return @this[k1][k2];
            }
            return string.Empty;
        }

        public static Dictionary<string, string> Merge(this Dictionary<string, string> @this, Dictionary<string, string> other)
        {
            if (other == null) return @this;
            foreach (var p in other)
            {
                @this[p.Key] = p.Value;
            }

            return @this;
        }
        public static Dictionary<string, Dictionary<string, string>> Merge(this Dictionary<string, Dictionary<string, string>> @this,
            params Dictionary<string, Dictionary<string, string>>[] others)
        {
            foreach (var other in others)
            {
                foreach (var p in other)
                {
                    if (@this.ContainsKey(p.Key))
                        Merge(@this[p.Key], p.Value);
                    else
                        @this[p.Key] = new Dictionary<string, string>();
                }
            }
            return @this;
        }
    }
}
