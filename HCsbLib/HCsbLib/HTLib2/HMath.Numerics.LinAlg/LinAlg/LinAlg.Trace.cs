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
        public static double Trace(this Matrix mat)
        {
            HDebug.Assert(mat.ColSize == mat.RowSize);
            double tr = 0;
            for(int i=0; i<mat.ColSize; i++)
                tr += mat[i, i];
            return tr;
        }
    }
}
