using Cli;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Quant.flex
{
    public class sms
    {
        static ILog log = typeof(sms).Log();

        static readonly string url = "http://utf8.sms.webchinese.cn/?";
        static readonly string Uid = "Uid=soudog";
        static readonly string key = "&key=38ee090621c5a6eb2ba9";
        static readonly string smsMob = "&smsMob=15801427695";
        static readonly string smsText = "&smsText=";

        public sms()
        {

        }

        string smsUrl(string content)
        {
            content = content;
            return url + Uid + key + smsMob + smsText + HttpUtility.UrlEncode(content);
        }

        public void order(Interace.Quant.Trade trade)
        {
            try
            {
                var message = string.Format("{0:yyyy-MM-dd HH:mm:ss} | {1} {2}({3})", trade.Date, trade.Dir, trade.Stock, trade.Portflio);
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
            catch (Exception ex)
            {
                log.WarnFormat("ex @ sms", ex);
            }
        }
    }
}
