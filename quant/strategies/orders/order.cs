using Interace.Quant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quant.strategies.orders
{
    public interface IOrder
    {
        void order(Account account, Interace.Quant.Trade trade);
    }
}
