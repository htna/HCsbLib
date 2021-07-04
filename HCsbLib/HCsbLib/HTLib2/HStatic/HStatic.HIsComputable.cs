﻿using System;
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

        public static bool IsZeros (this double[,] arr              ) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(arr[c, r] != 0    ) return false; return true; }
        public static bool IsOnes  (this double[,] arr              ) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(arr[c, r] != 1    ) return false; return true; }
        public static bool IsValues(this double[,] arr, double value) { int ColSize=arr.GetLength(0); int RowSize=arr.GetLength(1); for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(arr[c, r] != value) return false; return true; }

		public static bool IsNaN             (this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsNaN             (mat[c, r])) return true; return false; }
		public static bool IsInfinity        (this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsInfinity        (mat[c, r])) return true; return false; }
		public static bool IsPositiveInfinity(this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsPositiveInfinity(mat[c, r])) return true; return false; }
		public static bool IsNegativeInfinity(this IMatrix<double> mat) { for(int c=0; c<mat.ColSize; c++) for(int r=0; r<mat.RowSize; r++) if(double.IsNegativeInfinity(mat[c, r])) return true; return false; }
		public static bool IsComputable      (this IMatrix<double> mat) { return ((mat.IsNaN() == false) && (mat.IsInfinity() == false)); }
    }
}
