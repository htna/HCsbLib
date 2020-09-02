using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<U> HEnumCall<T, U>(this IEnumerable<T> list, Func<T,U> Func)
        {
            foreach(var item in list)
                yield return Func(item);
        }
        public static IEnumerable<U> HEnumCall<T, U, P1>(this IEnumerable<T> list, Func<T, P1, U> Func, P1 p1)
        {
            foreach(var item in list)
                yield return Func(item, p1);
        }
        public static IEnumerable<U> HEnumCall<T, U, P1, P2>(this IEnumerable<T> list, Func<T, P1, P2, U> Func, P1 p1, P2 p2)
        {
            foreach(var item in list)
                yield return Func(item, p1, p2);
        }
        public static IEnumerable<U> HEnumCall<T, U, P1, P2, P3>(this IEnumerable<T> list, Func<T, P1, P2, P3, U> Func, P1 p1, P2 p2, P3 p3)
        {
            foreach(var item in list)
                yield return Func(item, p1, p2, p3);
        }
    }
}
