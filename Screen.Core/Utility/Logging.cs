using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Utility
{
    public class Log
    {
        public void Info(string f, params object[] objs)
        {
            Console.Write("{0:yyyy-MM-dd HH:mm:ss.fff}||{1}||{2}", DateTime.Now, Environment.MachineName, Environment.UserName);
            if (objs != null && objs.Any())
                Console.Write("INFO ||" + f, objs);
            else
                Console.Write("INFO ||" + f);
            Console.WriteLine();
        }
    }

    public static class LogMixin
    {
        public static Log Log(this object obj)
        {
            return new Log();
        }
    }
}
