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
        public static SVector3 ToSVector3(this Vector vec)
        {
            HDebug.Assert(vec.Size == 3);
            return new SVector3(vec[0], vec[1], vec[2]);
        }
    }
}
