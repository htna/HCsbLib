using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<Tuple<int,int,double>> HEnumNonZeros(this IMatrix<double> mat)
        {
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c, r];
                    if(val == 0)
                        continue;
                    yield return new Tuple<int, int, double>(c, r, val);
                }
        }
    }
}
