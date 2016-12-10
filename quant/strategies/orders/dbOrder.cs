using Cli;
using Interace.Quant;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Trade.Db;

namespace Quant.strategies.orders
{
    public class dbOrder : IOrder
    {
        static ILog log = typeof(dbOrder).Log();

        public void order(Account account, Interace.Quant.Trade trade)
        {
            new db().save(account.Portflio, new[] { trade });
        }
    }
}
