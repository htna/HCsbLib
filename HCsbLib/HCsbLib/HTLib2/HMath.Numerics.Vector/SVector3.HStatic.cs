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
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsComputable      (this SVector3 vec    ) { return ((vec.IsNaN() == false) && (vec.IsInfinity() == false)); }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNaN             (this SVector3 vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsNaN             (vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsInfinity        (this SVector3 vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsInfinity        (vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPositiveInfinity(this SVector3 vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsPositiveInfinity(vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNegativeInfinity(this SVector3 vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsNegativeInfinity(vec[i])) return true; return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZeros (this SVector3 vec              ) { for(int i=0; i<vec.Size; i++) if(vec[i] != 0                      ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOnes  (this SVector3 vec              ) { for(int i=0; i<vec.Size; i++) if(vec[i] != 1                      ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValues(this SVector3 vec, double value) { for(int i=0; i<vec.Size; i++) if(vec[i] != value                  ) return false; return true; }
    }
}
