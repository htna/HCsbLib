using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HDebug
	{
		[System.Diagnostics.Conditional("DEBUG")]
//		[System.Diagnostics.DebuggerHiddenAttribute()]
		public static void SetEpsilon(IMatrix<double> mat)
		{
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    mat[c, r] = double.Epsilon;
		}

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertToleranceMatrix(double tolerance, IMatrix<double> values)
        {
            bool assert = CheckToleranceMatrix(tolerance, values);
            System.Diagnostics.Debug.Assert(assert);
        }
        public static bool CheckToleranceMatrix(double tolerance, IMatrix<double> values)
        {
            for(int c=0; c<values.ColSize; c++)
                for(int r=0; r<values.RowSize; r++)
                {
                    double value = values[c, r];
                    if(Math.Abs(value) > tolerance)
                        return false;
                }
            return true;
        }
    }
}
