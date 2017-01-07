using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Data
{
    public class kanalytic
    {
        public string code { get; set; }
        public DateTime date { get; set; }
        public string ktype { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double volume { get; set; }
        public double ma5 { get; set; }
        public double ma10 { get; set; }
        public double ma20 { get; set; }
        public double ma30 { get; set; }
        public double ma60 { get; set; }
        public double ma120 { get; set; }
        public double macd { get; set; }
        public double dif { get; set; }
        public double dea { get; set; }
        public double macdvol { get; set; }
        public double difvol { get; set; }
        public double deavol { get; set; }
        public DateTime ts { get; set; }
    }
}
