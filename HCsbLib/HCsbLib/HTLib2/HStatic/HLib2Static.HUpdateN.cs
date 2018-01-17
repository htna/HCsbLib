using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static Tuple<T1         > HUpdateItem1<T1         >(this Tuple<T1         > tuple, T1 item1)  { return new Tuple<T1         >(item1); }
        public static Tuple<T1,T2      > HUpdateItem1<T1,T2      >(this Tuple<T1,T2      > tuple, T1 item1)  { return new Tuple<T1,T2      >(item1, tuple.Item2); }
        public static Tuple<T1,T2,T3   > HUpdateItem1<T1,T2,T3   >(this Tuple<T1,T2,T3   > tuple, T1 item1)  { return new Tuple<T1,T2,T3   >(item1, tuple.Item2, tuple.Item3); }
        public static Tuple<T1,T2,T3,T4> HUpdateItem1<T1,T2,T3,T4>(this Tuple<T1,T2,T3,T4> tuple, T1 item1)  { return new Tuple<T1,T2,T3,T4>(item1, tuple.Item2, tuple.Item3, tuple.Item4); }

        public static Tuple<T1,T2      > HUpdateItem2<T1,T2      >(this Tuple<T1,T2      > tuple, T2 item2)  { return new Tuple<T1,T2      >(tuple.Item1, item2); }
        public static Tuple<T1,T2,T3   > HUpdateItem2<T1,T2,T3   >(this Tuple<T1,T2,T3   > tuple, T2 item2)  { return new Tuple<T1,T2,T3   >(tuple.Item1, item2, tuple.Item3); }
        public static Tuple<T1,T2,T3,T4> HUpdateItem2<T1,T2,T3,T4>(this Tuple<T1,T2,T3,T4> tuple, T2 item2)  { return new Tuple<T1,T2,T3,T4>(tuple.Item1, item2, tuple.Item3, tuple.Item4); }

        public static Tuple<T1,T2,T3   > HUpdateItem3<T1,T2,T3   >(this Tuple<T1,T2,T3   > tuple, T3 item3)  { return new Tuple<T1,T2,T3   >(tuple.Item1, tuple.Item2, item3); }
        public static Tuple<T1,T2,T3,T4> HUpdateItem3<T1,T2,T3,T4>(this Tuple<T1,T2,T3,T4> tuple, T3 item3)  { return new Tuple<T1,T2,T3,T4>(tuple.Item1, tuple.Item2, item3, tuple.Item4); }

        public static Tuple<T1,T2,T3,T4> HUpdateItem4<T1,T2,T3,T4>(this Tuple<T1,T2,T3,T4> tuple, T4 item4)  { return new Tuple<T1,T2,T3,T4>(tuple.Item1, tuple.Item2, tuple.Item3, item4); }
    }
}
