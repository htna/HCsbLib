using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<Tuple<T1,T2>> HToListTupleByItem12<T1,T2,T3>(this IList<Tuple<T1,T2,T3>> list) { Tuple<T1,T2>[] items = new Tuple<T1,T2>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T2>(list[i].Item1, list[i].Item2); return new List<Tuple<T1,T2>>(items); }
        public static List<Tuple<T1,T3>> HToListTupleByItem13<T1,T2,T3>(this IList<Tuple<T1,T2,T3>> list) { Tuple<T1,T3>[] items = new Tuple<T1,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T3>(list[i].Item1, list[i].Item3); return new List<Tuple<T1,T3>>(items); }
        public static List<Tuple<T2,T3>> HToListTupleByItem23<T1,T2,T3>(this IList<Tuple<T1,T2,T3>> list) { Tuple<T2,T3>[] items = new Tuple<T2,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2,T3>(list[i].Item2, list[i].Item3); return new List<Tuple<T2,T3>>(items); }
    }
}
