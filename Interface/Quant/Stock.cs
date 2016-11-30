using System;
using System.Collections.Generic;
using System.Text;

namespace Interace.Quant
{
    public class Stock
    {
        public object Attribute { get; set; }
        public string Code { get; set; }

        public Stock()
        {

        }
        public Stock(string code, object details = null)
        {
            Code = code;
            Attribute = details;
        }
    }
}
