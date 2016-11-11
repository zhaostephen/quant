using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen
{
    public static class MktDataClientMixin
    {
        public static IEnumerable<StkDataSeries> QueryAll(this MktDataClient client, PeriodEnum period = PeriodEnum.Daily, string sector = null)
        {
            var codes = client.Codes().Where(code => InSector(code, sector)).ToArray();
            var results = codes
                .AsParallel()
                .Select(code => client.Query(code, period))
                .Where(p => p != null)
                .ToArray();
            return results;
        }

        public static IEnumerable<string> CodesInSector(string sector)
        {
            var client = new MktDataClient();
            return client.Codes().Where(code => InSector(code, sector)).ToArray();
        }

        static bool InSector(string code, string sector)
        {
            if (string.IsNullOrEmpty(sector)) return true;

            switch (sector)
            {
                case Sector.shang_hai: return code.StartsWith("60");
                case Sector.sheng_zheng: return code.StartsWith("30") || code.StartsWith("00");
                case Sector.chuang_ye_ban: return code.StartsWith("30");
                case Sector.zhong_xiao_ban: return code.StartsWith("00");
                default:
                    throw new NotSupportedException(sector);
            }
        }
    }
}
