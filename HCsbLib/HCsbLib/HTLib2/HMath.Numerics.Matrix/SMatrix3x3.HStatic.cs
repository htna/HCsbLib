using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class HStatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SMatrix3x3 SMatrix3x3(this Matrix mat)
        {
            HDebug.Assert(mat.ColSize == 3);
            HDebug.Assert(mat.RowSize == 3);
            return new SMatrix3x3
                ( mat[0,0], mat[0,1], mat[0,2]
                , mat[1,0], mat[1,1], mat[1,2]
                , mat[2,0], mat[2,1], mat[2,2]
                );
        }
    }
}
