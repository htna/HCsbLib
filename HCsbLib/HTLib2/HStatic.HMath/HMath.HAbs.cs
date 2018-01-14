using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HMath
    {
        public static double MaxAbs(this double[] values)
        {
            double max = 0;
            for(int i = 0; i < values.Length; i++)
                max = Math.Max(max, Math.Abs(values[i]));
            return max;
        }
        public static double MaxAbs(this double[,] values)
        {
            double max = 0;
            for(int i = 0; i < values.GetLength(0); i++)
                for(int j = 0; j < values.GetLength(1); j++)
                    max = Math.Max(max, Math.Abs(values[i, j]));
            return max;
        }
        public static double HAbsMaxDiffWith(this IMatrix<double> mat, IMatrix<double> diff)
        {
            HDebug.Assert(mat.ColSize == diff.ColSize);
            HDebug.Assert(mat.RowSize == diff.RowSize);
            double absmax = -1;
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    absmax = Math.Max(absmax, Math.Abs(mat[c, r] - diff[c, r]));
            HDebug.Assert(absmax != -1);
            return absmax;
        }
        public static double HAbsMin(this IMatrix<double> mat)
        {
            double absmin = double.MaxValue;
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    absmin = Math.Min(absmin, Math.Abs(mat[c, r]));
            HDebug.Assert(absmin >= 0);
            return absmin;
        }
        public static double HAbsMax(this IMatrix<double> mat)
        {
            double absmax = -1;
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    absmax = Math.Max(absmax, Math.Abs(mat[c, r]));
            HDebug.Assert(absmax != -1);
            return absmax;
        }
        public static double[] HAbs(this IList<double> values)
        {
            double[] absvalues = new double[values.Count];
            for(int i=0; i<values.Count; i++)
                absvalues[i] = Math.Abs(values[i]);
            return absvalues;
        }
        public static int[] HAbs(this IList<int> values)
        {
            int[] absvalues = new int[values.Count];
            for(int i=0; i<values.Count; i++)
                absvalues[i] = Math.Abs(values[i]);
            return absvalues;
        }
        public static double[,] HAbs(this double[,] values)
        {
            double[,] absvalues = new double[values.GetLength(0), values.GetLength(1)];
            for(int i=0; i<values.GetLength(0); i++)
                for(int j=0; j<values.GetLength(1); j++)
                    absvalues[i,j] = Math.Abs(values[i, j]);
            return absvalues;
        }
    }
}
