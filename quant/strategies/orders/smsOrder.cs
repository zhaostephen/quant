using Cli;
using Interace.Quant;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Quant.strategies.orders
{
    public class smsOrder : IOrder
    {
        static ILog log = typeof(smsOrder).Log();

        static readonly string url = "http://utf8.sms.webchinese.cn/?";
        static readonly string Uid = "Uid=soudog";
        static readonly string key = "&key=38ee090621c5a6eb2ba9";
        static readonly string smsMob = "&smsMob=15801427695";
        static readonly string smsText = "&smsText=";

        public smsOrder()
        {

        }

        string smsUrl(string content)
        {
            return url + Uid + key + smsMob + smsText + HttpUtility.UrlEncode(content);
        }

        public void order(Interace.Quant.Trade trade)
        {
            var message = string.Format("{0:yyyy-MM-dd HH:mm:ss} | {1} {2}({3})",
                trade.Date,
                trade.Dir,
                trade.Stock,
                trade.Portflio);
            var smsurl = smsUrl(message);
            var hr = (HttpWebRequest)WebRequest.Create(smsurl);
            hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            hr.Method = "GET";
            hr.Timeout = 30 * 60 * 1000;
            using (var hs = hr.GetResponse())
            using (var sr = hs.GetResponseStream())
            using (var ser = new StreamReader(sr, Encoding.Default))
            {
                var ret = ser.ReadToEnd();
                log.Info("sms return | " + ret);
            }
        }
    }
}
