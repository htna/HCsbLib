using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<Tuple<T,T>> HEnumCombination2<T>(this IList<T> values)
        {
            int count = values.Count;
            for(int i=0; i<count-1; i++)
                for(int j=i+1; j<count; j++)
                {
                    yield return new Tuple<T, T>(values[i], values[j]);
                }
        }
        public static IEnumerable<Tuple<T,T,T>> HEnumCombination3<T>(this IList<T> values)
        {
            int count = values.Count;
            for(int i=0; i<count-2; i++)
                for(int j=i+1; j<count-1; j++)
                    for(int k=j+1; k<count-0; k++)
                        {
                            yield return new Tuple<T,T,T>(values[i], values[j], values[k]);
                        }
        }
        public static IEnumerable<T[]> HEnumCombination23<T>(this IList<T> values)
        {
            foreach(var comb in HEnumCombination2(values))
                yield return comb.HToArray();
            foreach(var comb in HEnumCombination3(values))
                yield return comb.HToArray();
        }
        public static IEnumerable<Tuple<T1,T2>> HEnumCombination2<T1,T2>(IList<T1> values1, IList<T2> values2)
        {
            foreach(T1 val1 in values1)
                foreach(T2 val2 in values2)
                    yield return new Tuple<T1,T2>(val1,val2);
        }
    }
}
