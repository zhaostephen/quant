using System.Collections.Generic;
using System.Linq;
using Trade.Utility;
using log4net;
using System;
using System.Diagnostics;
using System.Configuration;
using System.Threading.Tasks;

namespace Cluster
{
    class Service
    {
        static ILog log = typeof(Service).Log();
        readonly NodeList _nodes;

        public Service()
        {
            _nodes = new NodeList();
        }

        internal void Start()
        {
            log.Info("**********START**********");

            var tasks = new List<Task>();
            foreach (var node in _nodes)
            {
                log.InfoFormat("start node {0}", node);

                var t = Task.Run(() =>
                {
                    node.Process = Process.Start("server.exe", string.Format("console {0} {1}", node.From, node.To));
                    node.Process.WaitForExit((int)TimeSpan.FromMinutes(45).TotalMilliseconds);
                });
                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray(), (int)TimeSpan.FromHours(2).TotalMilliseconds);

            log.Info("**********DONE**********");
        }

        internal void Stop()
        {
            foreach (var node in _nodes)
            {
                log.InfoFormat("stop node {0}", node);
                try
                {
                    if (node.Process != null)
                        node.Process.Kill();
                }
                catch (Exception e)
                {
                    log.WarnFormat("ex @ stop node {0}| {1}", node, e.Message);
                }
            }
        }

        class Node
        {
            public int From { get; set; }
            public int To { get; set; }
            public Process Process { get; set; }

            public Node(int from, int to)
            {
                From = from;
                To = to;
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}", From, To);
            }
        }

        class NodeList : List<Node>
        {
            public NodeList()
            {
                var nodes = ConfigurationManager.AppSettings["nodes"].Split(',')
                    .Select(p => new Node(int.Parse(p.Split('-')[0]), int.Parse(p.Split('-')[1])))
                    .ToArray();
                AddRange(nodes);
            }
        }
    }
}
