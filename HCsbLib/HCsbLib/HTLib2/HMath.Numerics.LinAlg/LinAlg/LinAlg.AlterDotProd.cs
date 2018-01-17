using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        public static MatrixByArr AlterDotProd(Vector v1, Vector v2)
        {
            HDebug.ToDo("depreciated. Call VVt(v1,v2)");
            if(HDebug.IsDebuggerAttached && HDebug.Selftest())
            {
                MatrixByArr M0 = AlterDotProd(v1, v2);
                MatrixByArr M1 = VVt(v1, v2);
                HDebug.AssertTolerance(0, M0-M1);
            }

            MatrixByArr mat = new MatrixByArr(v1.Size, v2.Size);
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    mat[c, r] = v1[c] * v2[r];
            return mat;
        }
    }
}
