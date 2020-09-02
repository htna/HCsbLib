using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Matrix ToMatrix(this double[,] arr)
        {
            return new MatrixByArr(arr);
        }
        public static void UpdateAbs(this Matrix mat)
        {
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c,r];
                    if(val < 0) mat[c,r] = Math.Abs(val);
                }
        }
        public static Matrix GetAbs(this Matrix mat)
        {
            Matrix absmat = mat.Clone();
            absmat.UpdateAbs();
            return absmat;
        }
    }
}
