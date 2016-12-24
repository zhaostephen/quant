using Interace.Quant;
using log4net;
using Trade.Db;
using Trade.Utility;

namespace Quant.strategies.orders
{
    public class dbOrder : IOrder
    {
        static ILog log = typeof(dbOrder).Log();

        public void order(Interace.Quant.Trade trade)
        {
            new db().save(trade.Portflio, new[] { trade });
        }
    }
}
