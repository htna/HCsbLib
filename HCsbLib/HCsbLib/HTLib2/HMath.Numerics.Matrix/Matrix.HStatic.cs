using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class MatrixStatic
    {
        public static Matrix ToMatrix(this double[,] arr)
        {
            return new MatrixByArr(arr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateAbs(this Matrix mat)
        {
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c,r];
                    if(val < 0) mat[c,r] = Math.Abs(val);
                }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetAbs(this Matrix mat)
        {
            Matrix absmat = mat.Clone();
            absmat.UpdateAbs();
            return absmat;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateAdd(this IMatrix<double> _this, IMatrix<double> other, double other_mul)
        {
            HDebug.Exception(_this.ColSize == other.ColSize);
            HDebug.Exception(_this.RowSize == other.RowSize);
            if(other_mul == 1)
            {
                for(int c=0; c<_this.ColSize; c++)
                    for(int r=0; r<_this.RowSize; r++)
                        _this[c, r] += other[c, r];
            }
            else if(other_mul == -1)
            {
                for(int c=0; c<_this.ColSize; c++)
                    for(int r=0; r<_this.RowSize; r++)
                        _this[c, r] -= other[c, r];
            }
            else
            {
                for(int c=0; c<_this.ColSize; c++)
                    for(int r=0; r<_this.RowSize; r++)
                        _this[c, r] += (other[c, r] * other_mul);
            }
        }
        //public virtual void UpdateAdd(IMatrix other) 
        //{
        //    HDebug.Exception(ColSize == other.ColSize);
        //    HDebug.Exception(RowSize == other.RowSize);
        //    for(int c=0; c<ColSize; c++)
        //        for(int r=0; r<RowSize; r++)
        //            this[c, r] += other[c, r];
        //}
        //public virtual void UpdateSub(IMatrix other)
        //{
        //    HDebug.Exception(ColSize == other.ColSize);
        //    HDebug.Exception(RowSize == other.RowSize);
        //    for(int c=0; c<ColSize; c++)
        //        for(int r=0; r<RowSize; r++)
        //            this[c, r] -= other[c, r];
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetMul(this IMatrix<double> left, IMatrix<double> right)
        {
            int RowSize = left.RowSize;
            int ColSize = left.ColSize;
            HDebug.Exception(RowSize == right.ColSize);
            Matrix mul = Matrix.Zeros(ColSize, right.RowSize);
            int colsize = mul.ColSize;
            int midsize = RowSize;
            int rowsize = mul.RowSize;
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    for(int i=0; i<midsize; i++)
                        mul[c, r] += left[c, i] * right[i, r];
            return mul;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateMul(this IMatrix<double> _this, double other)
        {
            for(int c=0; c<_this.ColSize; c++)
                for(int r=0; r<_this.RowSize; r++)
                    _this[c, r] *= other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector GetColVector(this IMatrix<double> _this, int row)
        {
            HDebug.AssertAnd(0<=row, row<_this.RowSize);
            double[] vec = new double[_this.ColSize];
            for(int col=0; col<_this.ColSize; col++)
                vec[col] = _this[col, row];
            return vec;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector GetRowVector(this IMatrix<double> _this, int col)
        {
            HDebug.AssertAnd(0<=col, col<_this.ColSize);
            double[] vec = new double[_this.RowSize];
            for(int row=0; row<_this.RowSize; row++)
                vec[row] = _this[col, row];
            return vec;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(this IMatrix<double> _this, double value)
        {
            for(int c=0; c<_this.ColSize; c++)
                for(int r=0; r<_this.RowSize; r++)
                    _this[c, r] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualContents(this Matrix a, Matrix b)
        {
            if(a.ColSize != b.ColSize) return false;
            if(a.RowSize != b.RowSize) return false;
            for(int c=0; c<a.ColSize; c++)
            {
                for(int r=0; r<a.RowSize; r++)
                {
                    double a_val = a[c,r];
                    double b_val = b[c,r];
                    if(a_val != b_val) return false;
                }
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEqualContents(this Matrix a, Matrix b, double tol)
        {
            if(a.ColSize != b.ColSize) return false;
            if(a.RowSize != b.RowSize) return false;
            for(int c=0; c<a.ColSize; c++)
            {
                for(int r=0; r<a.RowSize; r++)
                {
                    double a_val = a[c,r];
                    double b_val = b[c,r];
                    if(Math.Abs(a_val - b_val) > tol) return false;
                }
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HIsSymmetric(this Matrix mat)
        {
            if(mat.ColSize != mat.RowSize) return false;
            int size = mat.ColSize;
            for(int c=0; c<size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double cr_val = mat[c,r];
                    double rc_val = mat[r,c];
                    if(cr_val != rc_val)
                        return false;
                }
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HIsSymmetric(this Matrix mat, double tol)
        {
            if(mat.ColSize != mat.RowSize) return false;
            int size = mat.ColSize;
            for(int c=0; c<size; c++)
            {
                for(int r=0; r<=c; r++)
                {
                    double cr_val = mat[c,r];
                    double rc_val = mat[r,c];
                    if(Math.Abs(cr_val - rc_val) > tol)
                        return false;
                }
            }
            return true;
        }

		public static void HToString
            ( this IMatrix<double> mat
            , StringBuilder sb
            , string format = "0.00000"
            , IFormatProvider formatProvider = null
            , string begindelim = "{{"
            , string enddelim   = "}}"
            , string rowdelim   = ", "
            , string coldelim   = "}, {"
            , int? maxcount     = null
            )
		{
			sb.Append(begindelim);

            int count = 0;

			for(int c = 0; c < mat.ColSize; c++) {
				if(c != 0) sb.Append(coldelim);

				for(int r = 0; r < mat.RowSize; r++) {
					if(r != 0) sb.Append(rowdelim);
					// str += this[c, r].ToString(format, formatProvider);
                    if(maxcount != null && count > maxcount.Value)
                        break;
					sb.Append(mat[c, r].ToString(format));
                    count++;
				}
			}

			sb.Append(enddelim);
		}
    }
}
