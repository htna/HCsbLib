using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static double HSum(this double[,] arr)
        {
            double sum = 0;
            int ColSize = arr.GetLength(0);
            int RowSize = arr.GetLength(1);
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    sum += arr[c, r];
            return sum;
        }
        public static double SumSquared(this double[,] arr)
        {
            double sum = 0;
            int ColSize = arr.GetLength(0);
            int RowSize = arr.GetLength(1);
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    sum += (arr[c, r] * arr[c, r]);
            return sum;
        }
    }
}
