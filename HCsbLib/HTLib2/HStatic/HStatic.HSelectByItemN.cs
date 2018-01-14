using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<T1         >[] HSelectByItem1<T1         >(this IList<Tuple<T1         >> list, T1 key) { var sele = new List<Tuple<T1         >>(); foreach(dynamic item in list) if(item.Item1 == key) sele.Add(item); return sele.ToArray(); }
        public static Tuple<T1,T2      >[] HSelectByItem1<T1,T2      >(this IList<Tuple<T1,T2      >> list, T1 key) { var sele = new List<Tuple<T1,T2      >>(); foreach(dynamic item in list) if(item.Item1 == key) sele.Add(item); return sele.ToArray(); }
        public static Tuple<T1,T2,T3   >[] HSelectByItem1<T1,T2,T3   >(this IList<Tuple<T1,T2,T3   >> list, T1 key) { var sele = new List<Tuple<T1,T2,T3   >>(); foreach(dynamic item in list) if(item.Item1 == key) sele.Add(item); return sele.ToArray(); }
        public static Tuple<T1,T2,T3,T4>[] HSelectByItem1<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list, T1 key) { var sele = new List<Tuple<T1,T2,T3,T4>>(); foreach(dynamic item in list) if(item.Item1 == key) sele.Add(item); return sele.ToArray(); }

        public static Tuple<T1,T2      >[] HSelectByItem2<T1,T2      >(this IList<Tuple<T1,T2      >> list, T2 key) { var sele = new List<Tuple<T1,T2      >>(); foreach(dynamic item in list) if(item.Item2 == key) sele.Add(item); return sele.ToArray(); }
        public static Tuple<T1,T2,T3   >[] HSelectByItem2<T1,T2,T3   >(this IList<Tuple<T1,T2,T3   >> list, T2 key) { var sele = new List<Tuple<T1,T2,T3   >>(); foreach(dynamic item in list) if(item.Item2 == key) sele.Add(item); return sele.ToArray(); }
        public static Tuple<T1,T2,T3,T4>[] HSelectByItem2<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list, T2 key) { var sele = new List<Tuple<T1,T2,T3,T4>>(); foreach(dynamic item in list) if(item.Item2 == key) sele.Add(item); return sele.ToArray(); }

        public static Tuple<T1,T2,T3   >[] HSelectByItem3<T1,T2,T3   >(this IList<Tuple<T1,T2,T3   >> list, T3 key) { var sele = new List<Tuple<T1,T2,T3   >>(); foreach(dynamic item in list) if(item.Item3 == key) sele.Add(item); return sele.ToArray(); }
        public static Tuple<T1,T2,T3,T4>[] HSelectByItem3<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list, T3 key) { var sele = new List<Tuple<T1,T2,T3,T4>>(); foreach(dynamic item in list) if(item.Item3 == key) sele.Add(item); return sele.ToArray(); }

        public static Tuple<T1,T2,T3,T4>[] HSelectByItem4<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list, T4 key) { var sele = new List<Tuple<T1,T2,T3,T4>>(); foreach(dynamic item in list) if(item.Item4 == key) sele.Add(item); return sele.ToArray(); }
    }
}
