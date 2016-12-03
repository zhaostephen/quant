using Cli;
using CommandLine;
using log4net;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Trade.Data;

namespace Trade.Cli.commands
{
    [command("updateconfig")]
    class updateconfigcmd : command<updateconfigcmd.Parameters>
    {
        static ILog log = LogManager.GetLogger(typeof(updateconfigcmd));

        public updateconfigcmd(string[] args) : base(args) { }

        public override void exec()
        {
            foreach(var file in Directory.GetFiles(param.path,"*.config"))
            {
                log.InfoFormat("update " + file);

                var e = XElement.Load(file);
                var q = e.XPathSelectElement("//appSettings/add[@key='quant']");
                if(q==null)
                {
                    log.WarnFormat("ignore " + file);
                    continue;
                }
                q.SetAttributeValue("value", param.quant);
                e.Save(file);
            }            
        }

        public class Parameters
        {
            [Option('p', "path", Required = true)]
            public string path { get; set; }
            [Option('q', "quant", Required = true)]
            public string quant { get; set; }
        }
    }
}
