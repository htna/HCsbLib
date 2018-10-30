using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T> HEnumByIndex<T>(this IMatrix<T> mat, IEnumerable<ValueTuple<int, int>> indices)
        {
            foreach(var index in indices)
            {
                int c = index.Item1;
                int r = index.Item2;
                HDebug.Assert(0 <= c, c < mat.ColSize);
                HDebug.Assert(0 <= r, r < mat.ColSize);
                yield return mat[c, r];
            }
        }
        public static IEnumerable<T> HEnumByIndex<T>(this IMatrix<T> mat, IEnumerable<Tuple<int, int>> indices)
        {
            foreach(var index in indices)
            {
                int c = index.Item1;
                int r = index.Item2;
                HDebug.Assert(0 <= c, c < mat.ColSize);
                HDebug.Assert(0 <= r, r < mat.ColSize);
                yield return mat[c, r];
            }
        }
    }
}
