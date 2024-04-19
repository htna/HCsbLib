using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T[] HToArray1D<T>(this T[,] values)
        {
            T[] arr = new T[values.Length];
            int i=0;
            for(int i0=0; i0<values.GetLength(0); i0++)
                for(int i1=0; i1<values.GetLength(1); i1++)
                    arr[i++] = values[i0, i1];
            return arr;
        }

        public static T[] HToArray<T>(this Tuple<T> values)          { return new T[] { values.Item1                                                         }; }
        public static T[] HToArray<T>(this Tuple<T,T> values)        { return new T[] { values.Item1, values.Item2                                           }; }
        public static T[] HToArray<T>(this Tuple<T,T,T> values)      { return new T[] { values.Item1, values.Item2, values.Item3                             }; }
        public static T[] HToArray<T>(this Tuple<T,T,T,T> values)    { return new T[] { values.Item1, values.Item2, values.Item3, values.Item4               }; }
        public static T[] HToArray<T>(this Tuple<T,T,T,T,T> values)  { return new T[] { values.Item1, values.Item2, values.Item3, values.Item4, values.Item5 }; }

        public static T[] HToArray<T>(this ValueTuple<T> values)          { return new T[] { values.Item1                                                         }; }
        public static T[] HToArray<T>(this ValueTuple<T,T> values)        { return new T[] { values.Item1, values.Item2                                           }; }
        public static T[] HToArray<T>(this ValueTuple<T,T,T> values)      { return new T[] { values.Item1, values.Item2, values.Item3                             }; }
        public static T[] HToArray<T>(this ValueTuple<T,T,T,T> values)    { return new T[] { values.Item1, values.Item2, values.Item3, values.Item4               }; }
        public static T[] HToArray<T>(this ValueTuple<T,T,T,T,T> values)  { return new T[] { values.Item1, values.Item2, values.Item3, values.Item4, values.Item5 }; }

        public static List<T> HToList<T>(this Tuple<T> values)          { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this Tuple<T,T> values)        { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this Tuple<T,T,T> values)      { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this Tuple<T,T,T,T> values)    { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this Tuple<T,T,T,T,T> values)  { return new List<T>(values.HToArray()); }

        public static List<T> HToList<T>(this ValueTuple<T> values)          { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this ValueTuple<T,T> values)        { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this ValueTuple<T,T,T> values)      { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this ValueTuple<T,T,T,T> values)    { return new List<T>(values.HToArray()); }
        public static List<T> HToList<T>(this ValueTuple<T,T,T,T,T> values)  { return new List<T>(values.HToArray()); }

        public static T[,] HToArray<T>(this IList<Tuple<T        >> values)  { int size=1; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<Tuple<T,T      >> values)  { int size=2; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<Tuple<T,T,T    >> values)  { int size=3; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<Tuple<T,T,T,T  >> values)  { int size=4; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<Tuple<T,T,T,T,T>> values)  { int size=5; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }

        public static T[,] HToArray<T>(this IList<ValueTuple<T        >> values)  { int size=1; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<ValueTuple<T,T      >> values)  { int size=2; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<ValueTuple<T,T,T    >> values)  { int size=3; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<ValueTuple<T,T,T,T  >> values)  { int size=4; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }
        public static T[,] HToArray<T>(this IList<ValueTuple<T,T,T,T,T>> values)  { int size=5; int count=values.Count; T[,] mat = new T[count, size]; for(int i=0; i<count; i++) for(int j=0; j<size; j++) mat[i, j] = values[i].HGetItem(j); return mat; }

        public static char[] HToArray(this string arr)
        {
            return arr.ToArray();
        }

        public static T[][] HToArrayArray<T>(this IList<IList<T>> valuess) { T[][] retss=new T[valuess.Count][]; for(int i=0; i<valuess.Count; i++) retss[i]=valuess[i].ToArray(); return retss; }
        public static T[][] HToArrayArray<T>(this IList< List<T>> valuess) { T[][] retss=new T[valuess.Count][]; for(int i=0; i<valuess.Count; i++) retss[i]=valuess[i].ToArray(); return retss; }
    }
}
