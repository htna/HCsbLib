using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib3
{
    public class HPack<T>
    {
        public T Value;

        public static implicit operator T(HPack<T> pack)
        {
            return pack.Value;
        }
        public override string ToString()
        {
            return "Pack: " + Value.ToString();
        }
    }
}
