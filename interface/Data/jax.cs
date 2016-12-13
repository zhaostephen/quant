using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.Data
{
    public class jax
    {
        public DateTime Date { get; set; }
        public double JAX { get; set; }
        public double J { get; set; }
        public double A { get; set; }
        public double X { get; set; }

        public bool upward { get { return A > JAX && J == 0d && X == 0d; } }
    }
}
