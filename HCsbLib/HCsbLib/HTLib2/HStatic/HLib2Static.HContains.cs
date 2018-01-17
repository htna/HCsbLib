using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static bool HContainsNan(this IList<double> values)
        {
            foreach(double value in values)
                if(double.IsNaN(value))
                    return true;
            return false;
        }

        public static bool HContains<T>(this Tuple<T        > values, T value) { return values.HToArray().Contains(value); }
        public static bool HContains<T>(this Tuple<T,T      > values, T value) { return values.HToArray().Contains(value); }
        public static bool HContains<T>(this Tuple<T,T,T    > values, T value) { return values.HToArray().Contains(value); }
        public static bool HContains<T>(this Tuple<T,T,T,T  > values, T value) { return values.HToArray().Contains(value); }
        public static bool HContains<T>(this Tuple<T,T,T,T,T> values, T value) { return values.HToArray().Contains(value); }
    }
}
