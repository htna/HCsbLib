using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HAdd<T>(this IList<T> values, T add)
        {
            List<T> list = new List<T>(values);
            list.Add(add);
            return list.ToArray();
        }

        public static Tuple<T1,T2      > HAdd<T1,T2      >(this Tuple<T1      > tuple, T2 item2) { return new Tuple<T1,T2      >(tuple.Item1,       item2                    ); }
        public static Tuple<T1,T2,T3   > HAdd<T1,T2,T3   >(this Tuple<T1,T2   > tuple, T3 item3) { return new Tuple<T1,T2,T3   >(tuple.Item1, tuple.Item2, item3             ); }
        public static Tuple<T1,T2,T3,T4> HAdd<T1,T2,T3,T4>(this Tuple<T1,T2,T3> tuple, T4 item4) { return new Tuple<T1,T2,T3,T4>(tuple.Item1, tuple.Item2, tuple.Item3, item4); }

        public static T[] HAddRange<T>(this IList<T> values, params T[] adds)
        {
            List<T> list = new List<T>(values);
            list.AddRange(adds);
            return list.ToArray();
        }

        static int HAddToRangeImpl<T>(this HashSet<T> set, IEnumerable<T> adds)
        {
            int count = 0;
            foreach(T add in adds)
            {
                bool isadded = set.Add(add);
                if(isadded)
                    count++;
            }
            return count;
        }
        public static int HAddToRange<T>(this HashSet<T> set, IEnumerable<T> adds)
        {
            return set.HAddToRangeImpl(adds);
        }
        public static int HAddToRange<T>(this HashSet<T> set, params T[] adds)
        {
            return set.HAddToRangeImpl(adds);
        }
        public static HashSet<T> HAddRange<T>(this HashSet<T> set, IEnumerable<T> adds)
        {
            HashSet<T> newset = new HashSet<T>(set);
            newset.HAddToRangeImpl(adds);
            return newset;
        }
        public static HashSet<T> HAddRange<T>(this HashSet<T> set, params T[] adds)
        {
            HashSet<T> newset = new HashSet<T>(set);
            newset.HAddToRangeImpl(adds);
            return newset;
        }

        public static Tuple<T1,T2>[] HAdd<T1,T2>(this IList<Tuple<T1,T2>> values, T1 add1, T2 add2)
        {
            return values.HAdd(new Tuple<T1, T2>(add1, add2));
        }

        public static Tuple<T1> HAdd<T1>(this Tuple<T1> val1, Tuple<T1> val2)
        {
            return new Tuple<T1>
            (
                ((dynamic)val1.Item1) + val2.Item1
            );
        }
        public static Tuple<T1, T2> HAdd<T1, T2>(this Tuple<T1, T2> val1, Tuple<T1, T2> val2)
        {
            return new Tuple<T1, T2>
            (
                ((dynamic)val1.Item1) + val2.Item1,
                ((dynamic)val1.Item2) + val2.Item2
            );
        }
        public static Tuple<T1, T2, T3> HAdd<T1, T2, T3>(this Tuple<T1, T2, T3> val1, Tuple<T1, T2, T3> val2)
        {
            return new Tuple<T1, T2, T3>
            (
                ((dynamic)val1.Item1) + val2.Item1,
                ((dynamic)val1.Item2) + val2.Item2,
                ((dynamic)val1.Item3) + val2.Item3
            );
        }
        public static Tuple<T1, T2, T3, T4> HAdd<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> val1, Tuple<T1, T2, T3, T4> val2)
        {
            return new Tuple<T1, T2, T3, T4>
            (
                ((dynamic)val1.Item1) + val2.Item1,
                ((dynamic)val1.Item2) + val2.Item2,
                ((dynamic)val1.Item3) + val2.Item3,
                ((dynamic)val1.Item4) + val2.Item4
            );
        }
        public static Tuple<T1, T2, T3, T4, T5> HAdd<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> val1, Tuple<T1, T2, T3, T4, T5> val2)
        {
            return new Tuple<T1, T2, T3, T4, T5>
            (
                ((dynamic)val1.Item1) + val2.Item1,
                ((dynamic)val1.Item2) + val2.Item2,
                ((dynamic)val1.Item3) + val2.Item3,
                ((dynamic)val1.Item4) + val2.Item4,
                ((dynamic)val1.Item5) + val2.Item5
            );
        }
    }
}
