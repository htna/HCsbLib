using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class IMatrixStatic
    {
        public static IEnumerable<T> HEnumIntByString<T>(this IMatrix<T> mat)
        {
            for(int c = 0; c < mat.ColSize; c++)
                for(int r = 0; r < mat.RowSize; r++)
                    yield return mat[c, r];
        }
    }
}
