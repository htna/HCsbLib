using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<T,T> ToTupleSorted<T>(T v1, T v2)
            where T : IComparable<T>
        {
            if(v1.CompareTo(v2) <= 0)
                return new Tuple<T, T>(v1, v2);
            return new Tuple<T, T>(v2, v1);
        }
    }
}
