using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<T, U>[] HToTupleArray<T, U>(this Tuple<T[], U[]> ts_us)
        {
            IList<T> ts = ts_us.Item1;
            IList<U> us = ts_us.Item2;
            return HToTupleArray(ts, us);
        }
        public static Tuple<T, U>[] HToTupleArray<T, U>(this Tuple<List<T>, List<U>> ts_us)
        {
            IList<T> ts = ts_us.Item1;
            IList<U> us = ts_us.Item2;
            return HToTupleArray(ts, us);
        }
        public static Tuple<T, U>[] HToTupleArray<T, U>(this Tuple<IList<T>, IList<U>> ts_us)
        {
            IList<T> ts = ts_us.Item1;
            IList<U> us = ts_us.Item2;
            return HToTupleArray(ts, us);
        }
        public static Tuple<T, U>[] HToTupleArray<T, U>(IList<T> ts, IList<U> us)
        {
            if(ts.Count != us.Count)
                throw new Exception("ts.count != us.count");
            int leng = ts.Count;
            Tuple<T, U>[] tus = new Tuple<T, U>[leng];
            for(int i=0; i<leng; i++)
                tus[i] = new Tuple<T, U>(ts[i], us[i]);
            return tus;
        }
    }
}
