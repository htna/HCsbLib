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
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsComputable      (this SVector3 vec          ) { return ((vec.IsNaN() == false) && (vec.IsInfinity() == false)); }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNaN             (this SVector3 vec          ) { for(int i=0; i<vec.Size; i++) if(double.IsNaN             (vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsInfinity        (this SVector3 vec          ) { for(int i=0; i<vec.Size; i++) if(double.IsInfinity        (vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPositiveInfinity(this SVector3 vec          ) { for(int i=0; i<vec.Size; i++) if(double.IsPositiveInfinity(vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNegativeInfinity(this SVector3 vec          ) { for(int i=0; i<vec.Size; i++) if(double.IsNegativeInfinity(vec[i])) return true; return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZeros           (this SVector3 vec          ) { return ((vec.v0 == 0) && (vec.v1 == 0) && (vec.v2 == 0)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOnes            (this SVector3 vec          ) { return ((vec.v0 == 1) && (vec.v1 == 1) && (vec.v2 == 1)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValues          (this SVector3 vec, double v) { return ((vec.v0 == v) && (vec.v1 == v) && (vec.v2 == v)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IEnumerable<double> HEnumV0(this IEnumerable<SVector3> vecs) { foreach(var vec in vecs) yield return vec.v0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IEnumerable<double> HEnumV1(this IEnumerable<SVector3> vecs) { foreach(var vec in vecs) yield return vec.v1; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static IEnumerable<double> HEnumV2(this IEnumerable<SVector3> vecs) { foreach(var vec in vecs) yield return vec.v2; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MakeUnitVector(this SVector3 vec)
        {
            double dist = vec.Dist;
            vec.v0 = vec.v0 / dist;
            vec.v1 = vec.v1 / dist;
            vec.v2 = vec.v2 / dist;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SVector3 UnitVector(this SVector3 vec)
        {
            double dist = vec.Dist;
            return new SVector3
            {
                v0 = vec.v0 / dist,
                v1 = vec.v1 / dist,
                v2 = vec.v2 / dist,
            };
        }
    }
}
