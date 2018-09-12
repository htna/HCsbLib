using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        public static double HSumPow2(this IMatrix<double> mat)
        {
            double sum = 0;
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double v = mat[c, r];
                    sum += v * v;
                }
            return sum;
        }
    }
}
