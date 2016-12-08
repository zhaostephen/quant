using Cli;
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

namespace Quant.flex
{
    //public class sms2
    //{
    //    private const String host = "http://sms.market.alicloudapi.com";
    //    private const String path = "/singleSendSms";
    //    private const String method = "GET";
    //    private const String appcode = "f665f176e7194eb181e28d2bff799a72";

    //    public void order(Interace.Quant.Trade trade)
    //    {
    //        var message = HttpUtility.UrlEncode(string.Format("{0:yyyy-MM-dd HH:mm:ss} | {1} {2}({3})", trade.Date, trade.Dir, trade.Stock, trade.Portflio));
    //        String querys = "ParamString=" +"&RecNum=15801427695";
    //        String bodys = "";
    //        String url = host + path;
    //        HttpWebRequest httpRequest = null;
    //        HttpWebResponse httpResponse = null;

    //        if (0 < querys.Length)
    //        {
    //            url = url + "?" + querys;
    //        }

    //        if (host.Contains("https://"))
    //        {
    //            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
    //            httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
    //        }
    //        else
    //        {
    //            httpRequest = (HttpWebRequest)WebRequest.Create(url);
    //        }
    //        httpRequest.Method = method;
    //        httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
    //        if (0 < bodys.Length)
    //        {
    //            byte[] data = Encoding.UTF8.GetBytes(bodys);
    //            using (Stream stream = httpRequest.GetRequestStream())
    //            {
    //                stream.Write(data, 0, data.Length);
    //            }
    //        }
    //        try
    //        {
    //            httpResponse = (HttpWebResponse)httpRequest.GetResponse();
    //        }
    //        catch (WebException ex)
    //        {
    //            httpResponse = (HttpWebResponse)ex.Response;
    //        }

    //        Console.WriteLine(httpResponse.StatusCode);
    //        Console.WriteLine(httpResponse.Method);
    //        Console.WriteLine(httpResponse.Headers);
    //        Stream st = httpResponse.GetResponseStream();
    //        StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
    //        Console.WriteLine(reader.ReadToEnd());
    //        Console.WriteLine("\n");

    //    }

    //    public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    //    {
    //        return true;
    //    }

    //}
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
