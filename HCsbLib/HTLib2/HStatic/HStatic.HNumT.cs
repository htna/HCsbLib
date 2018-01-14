using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static int HNumZeros(this double[,] arr)
        {
            int ColSize = arr.GetLength(0);
            int RowSize = arr.GetLength(1);
            int count = 0;
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    if(arr[c, r] == 0)
                        count++;
            return count;
        }
        public static int HNumZeros(this IMatrix<double> mat)
        {
            int count = 0;
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    if(mat[c, r] == 0)
                        count++;
            return count;
        }
    }
}
