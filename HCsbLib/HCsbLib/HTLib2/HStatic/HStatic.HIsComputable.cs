using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
		public static bool IsNaN             (this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsNaN             (arr[c, r])) return true; return false; }
		public static bool IsInfinity        (this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsInfinity        (arr[c, r])) return true; return false; }
		public static bool IsPositiveInfinity(this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsPositiveInfinity(arr[c, r])) return true; return false; }
		public static bool IsNegativeInfinity(this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsNegativeInfinity(arr[c, r])) return true; return false; }
		public static bool IsComputable      (this double[,] arr) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); return ((arr.IsNaN() == false) && (arr.IsInfinity() == false)); }

		public static bool IsNaN             (this Matrix mat) { return mat.ToArray().IsNaN             (); }
		public static bool IsInfinity        (this Matrix mat) { return mat.ToArray().IsInfinity        (); }
		public static bool IsPositiveInfinity(this Matrix mat) { return mat.ToArray().IsPositiveInfinity(); }
		public static bool IsNegativeInfinity(this Matrix mat) { return mat.ToArray().IsNegativeInfinity(); }
		public static bool IsComputable      (this Matrix mat) { return mat.ToArray().IsComputable      (); }
    }
}
