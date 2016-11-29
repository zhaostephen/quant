using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Strategies
{
    public class Money
    {
        public double Total { get; set; }

        public Money(double total)
        {
            Total = total;
        }
    }
}
