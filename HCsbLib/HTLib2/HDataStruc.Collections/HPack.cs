using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public class HPack<T>
    {
        public T value;

        public static implicit operator T(HPack<T> pack)
        {
            return pack.value;
        }
        public override string ToString()
        {
            return "Pack: " + value.ToString();
        }
    }
}
