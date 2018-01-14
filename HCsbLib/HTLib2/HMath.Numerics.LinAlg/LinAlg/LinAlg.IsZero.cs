using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class LinAlg
    {
        public static bool IsZero_selftest = HDebug.IsDebuggerAttached;
        public static bool IsZero(this Matrix mat)
        {
            if(IsZero_selftest)
            {
                IsZero_selftest = false;
                HDebug.Assert(((MatrixByArr)(new double[2, 2] { { 0, 0 }, { 0, 0 } })).IsZero() == true );
                HDebug.Assert(((MatrixByArr)(new double[2, 2] { { 0, 0 }, { 0, 1 } })).IsZero() == false);
            }

            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    if(mat[c, r] != 0)
                        return false;
            return true;
        }
    }
}
