using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class HStatic
    {
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNaN             (this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsNaN             (arr[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsInfinity        (this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsInfinity        (arr[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPositiveInfinity(this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsPositiveInfinity(arr[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNegativeInfinity(this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsNegativeInfinity(arr[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsComputable      (this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); return ((arr.IsNaN() == false) && (arr.IsInfinity() == false)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZeros (this double[,] arr              ) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(arr[c, r] != 0    ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOnes  (this double[,] arr              ) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(arr[c, r] != 1    ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValues(this double[,] arr, double value) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(arr[c, r] != value) return false; return true; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNaN             (this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsNaN             (mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsInfinity        (this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsInfinity        (mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPositiveInfinity(this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsPositiveInfinity(mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNegativeInfinity(this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsNegativeInfinity(mat[c, r])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsComputable      (this IMatrix<double> mat) { return ((mat.IsNaN() == false) && (mat.IsInfinity() == false)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZeros (this IMatrix<double> mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(mat[c, r] != 0    ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOnes  (this IMatrix<double> mat              ) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(mat[c, r] != 1    ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValues(this IMatrix<double> mat, double value) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(mat[c, r] != value) return false; return true; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsComputable      (this IVector<double> vec    ) { return ((vec.IsNaN() == false) && (vec.IsInfinity() == false)); }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNaN             (this IVector<double> vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsNaN             (vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsInfinity        (this IVector<double> vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsInfinity        (vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPositiveInfinity(this IVector<double> vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsPositiveInfinity(vec[i])) return true; return false; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNegativeInfinity(this IVector<double> vec    ) { for(int i=0; i<vec.Size; i++) if(double.IsNegativeInfinity(vec[i])) return true; return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsZeros (this IVector<double> vec              ) { for(int i=0; i<vec.Size; i++) if(vec[i] != 0                      ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOnes  (this IVector<double> vec              ) { for(int i=0; i<vec.Size; i++) if(vec[i] != 1                      ) return false; return true; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValues(this IVector<double> vec, double value) { for(int i=0; i<vec.Size; i++) if(vec[i] != value                  ) return false; return true; }
	}
}
