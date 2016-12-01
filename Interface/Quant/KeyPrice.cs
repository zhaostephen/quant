using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class KeyPrice
    {
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string Flag { get; set; }
        public bool Auto { get; set; }
    }
}
