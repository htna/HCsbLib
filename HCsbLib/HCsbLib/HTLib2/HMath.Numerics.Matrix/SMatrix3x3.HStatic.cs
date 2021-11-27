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

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsComputable      (this SMatrix3x3 mat              ) { return ((mat.IsNaN() == false) && (mat.IsInfinity() == false)); }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNaN             (this SMatrix3x3 mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsNaN             (mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsInfinity        (this SMatrix3x3 mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsInfinity        (mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPositiveInfinity(this SMatrix3x3 mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsPositiveInfinity(mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNegativeInfinity(this SMatrix3x3 mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsNegativeInfinity(mat[c, r])) return true; return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZeros           (this SMatrix3x3 mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(mat[c, r] != 0    ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOnes            (this SMatrix3x3 mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(mat[c, r] != 1    ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValues          (this SMatrix3x3 mat, double value) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(mat[c, r] != value) return false; return true; }
    }
}
