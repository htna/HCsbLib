using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static Tuple<T1, T2> HItem12<T1, T2, T3>(this Tuple<T1, T2, T3> tuple) { return new Tuple<T1, T2>(tuple.Item1, tuple.Item2); }
        public static Tuple<T1, T3> HItem13<T1, T2, T3>(this Tuple<T1, T2, T3> tuple) { return new Tuple<T1, T3>(tuple.Item1, tuple.Item3); }
        public static Tuple<T2, T3> HItem23<T1, T2, T3>(this Tuple<T1, T2, T3> tuple) { return new Tuple<T2, T3>(tuple.Item2, tuple.Item3); }

        public static Tuple<T1, T2, T3> HItem123<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T2, T3>(tuple.Item1, tuple.Item2, tuple.Item3); }
        public static Tuple<T1, T2, T4> HItem124<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T2, T4>(tuple.Item1, tuple.Item2, tuple.Item4); }
        public static Tuple<T1, T3, T4> HItem134<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T3, T4>(tuple.Item1, tuple.Item3, tuple.Item4); }
        public static Tuple<T2, T3, T4> HItem234<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T2, T3, T4>(tuple.Item2, tuple.Item3, tuple.Item4); }

        public static Tuple<T1, T2> HItem12<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T2>(tuple.Item1, tuple.Item2); }
        public static Tuple<T1, T3> HItem13<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T3>(tuple.Item1, tuple.Item3); }
        public static Tuple<T1, T4> HItem14<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T4>(tuple.Item1, tuple.Item4); }
        public static Tuple<T2, T3> HItem23<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T2, T3>(tuple.Item2, tuple.Item3); }
        public static Tuple<T2, T4> HItem24<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T2, T4>(tuple.Item2, tuple.Item4); }
        public static Tuple<T3, T4> HItem34<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T3, T4>(tuple.Item3, tuple.Item4); }

        public static Tuple<T1, T2, T3> HItem123<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T2, T3>(tuple.Item1, tuple.Item2, tuple.Item3); }
        public static Tuple<T1, T2, T4> HItem124<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T2, T4>(tuple.Item1, tuple.Item2, tuple.Item4); }
        public static Tuple<T1, T2, T5> HItem125<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T2, T5>(tuple.Item1, tuple.Item2, tuple.Item5); }
        public static Tuple<T1, T3, T4> HItem134<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T3, T4>(tuple.Item1, tuple.Item3, tuple.Item4); }
        public static Tuple<T1, T3, T5> HItem135<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T3, T5>(tuple.Item1, tuple.Item3, tuple.Item5); }
        public static Tuple<T1, T4, T5> HItem145<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T4, T5>(tuple.Item1, tuple.Item4, tuple.Item5); }
        public static Tuple<T2, T3, T4> HItem234<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T2, T3, T4>(tuple.Item2, tuple.Item3, tuple.Item4); }
        public static Tuple<T2, T3, T5> HItem235<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T2, T3, T5>(tuple.Item2, tuple.Item3, tuple.Item5); }
        public static Tuple<T2, T4, T5> HItem245<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T2, T4, T5>(tuple.Item2, tuple.Item4, tuple.Item5); }
        public static Tuple<T3, T4, T5> HItem345<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T3, T4, T5>(tuple.Item3, tuple.Item4, tuple.Item5); }

        public static Tuple<T1, T2> HItem12<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T2>(tuple.Item1, tuple.Item2); }
        public static Tuple<T1, T3> HItem13<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T3>(tuple.Item1, tuple.Item3); }
        public static Tuple<T1, T4> HItem14<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T4>(tuple.Item1, tuple.Item4); }
        public static Tuple<T1, T5> HItem15<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T1, T5>(tuple.Item1, tuple.Item5); }
        public static Tuple<T2, T3> HItem23<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T2, T3>(tuple.Item2, tuple.Item3); }
        public static Tuple<T2, T4> HItem24<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T2, T4>(tuple.Item2, tuple.Item4); }
        public static Tuple<T2, T5> HItem25<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T2, T5>(tuple.Item2, tuple.Item5); }
        public static Tuple<T3, T4> HItem34<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T3, T4>(tuple.Item3, tuple.Item4); }
        public static Tuple<T3, T5> HItem35<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T3, T5>(tuple.Item3, tuple.Item5); }
        public static Tuple<T4, T5> HItem45<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) { return new Tuple<T4, T5>(tuple.Item4, tuple.Item5); }
    }
}
