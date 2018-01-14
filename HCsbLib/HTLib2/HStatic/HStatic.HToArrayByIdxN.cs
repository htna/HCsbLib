using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HToArrayByIdx1<T>(this IList<Tuple<int,T>> values, int size) { T[] arr = new T[size]; foreach(var value in values) { HDebug.Assert(arr[value.Item1] == null); arr[value.Item1] = value.Item2; } return arr; }
        public static T[] HToArrayByIdx2<T>(this IList<Tuple<T,int>> values, int size) { T[] arr = new T[size]; foreach(var value in values) { HDebug.Assert(arr[value.Item2] == null); arr[value.Item2] = value.Item1; } return arr; }
    }
}
