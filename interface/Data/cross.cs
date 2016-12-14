using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Data
{
    public class cross<T>
    {
        public crosstype type { get; set; }
        public T value { get; set; }

        public cross()
        {

        }
        public cross(T value, crosstype type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public enum crosstype
    {
        none,
        gold,
        dead
    }
}
