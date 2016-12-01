using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Attribution
{
    public class KeyPrice
    {
        public static class Flags
        {
            public const string upper = "upper";
            public const string lower = "lower";
        }

        public string Code { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string Flag { get; set; }
        public bool Auto { get; set; }

        public static KeyPrice Upper(string code, DateTime date, double price, bool auto = true)
        {
            return new KeyPrice { Code = code, Date = date, Price = price, Auto = auto, Flag= Flags.upper };
        }
        public static KeyPrice Lower(string code, DateTime date, double price, bool auto = true)
        {
            return new KeyPrice { Code = code, Date = date, Price = price, Auto = auto, Flag = Flags.lower };
        }
    }
}
