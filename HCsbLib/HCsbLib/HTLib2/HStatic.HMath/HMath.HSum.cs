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
        public static double HSumPow2Diff<MAT1, MAT2>(this (MAT1, MAT2) mat12)
            where MAT1 : IMatrix<double>
            where MAT2 : IMatrix<double>
        {
            IMatrix<double> mat1 = mat12.Item1;
            IMatrix<double> mat2 = mat12.Item2;
            HDebug.Exception(mat1.ColSize == mat2.ColSize);
            HDebug.Exception(mat1.RowSize == mat2.RowSize);
            double sum = 0;
            for(int c=0; c<mat1.ColSize; c++)
                for(int r=0; r<mat1.RowSize; r++)
                {
                    double v = (mat1[c, r] - mat2[c, r]);
                    sum += v * v;
                }
            return sum;
        }
    }
}
