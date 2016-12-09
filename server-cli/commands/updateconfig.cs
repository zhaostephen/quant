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
            foreach (var file in Directory.GetFiles(param.path, "*.config"))
            {
                log.InfoFormat("update " + file);

                var e = XElement.Load(file);

                setValue(e, "quant", param.quant);
                setValue(e, "nodes", param.nodes);
                setValue(e, "env", "prod");

                e.Save(file);
            }

            var py = Path.Combine(param.path, "storage.py");
            log.InfoFormat("update " + py);
            File.WriteAllText(py, File.ReadAllText(py).Replace("D:/quant/data/", param.tushre));
        }

        static bool setValue(XElement e, string key, string value)
        {
            var q = e.XPathSelectElement("//appSettings/add[@key='" + key + "']");
            if (q == null)
            {
                log.WarnFormat("ignore " + key);
                return false;
            }
            q.SetAttributeValue("value", value);

            return true;
        }

        public class Parameters
        {
            [Option('p', "path", Required = true)]
            public string path { get; set; }
            [Option('q', "quant", Required = true)]
            public string quant { get; set; }
            [Option('n', "nodes", Required = true)]
            public string nodes { get; set; }
            [Option('t', "tushre", Required = true)]
            public string tushre { get; set; }
        }
    }
}
