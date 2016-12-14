using Interace.Quant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interace.Quant
{
    public interface IOrder
    {
        void order(Trade trade);
    }
}
