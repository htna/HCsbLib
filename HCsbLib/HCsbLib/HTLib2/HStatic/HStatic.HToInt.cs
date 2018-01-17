using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static int[,] HToInt(this double[,] mat)
        {
            int ColSize = mat.GetLength(0);
            int RowSize = mat.GetLength(1);
            int[,] arr = new int[ColSize, RowSize];
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    arr[c, r] = (int)mat[c, r];
            return arr;
        }
    }
}
