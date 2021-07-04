using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class MatrixStatic
    {
        public static Matrix ToMatrix(this double[,] arr)
        {
            return new MatrixByArr(arr);
        }
        public static void UpdateAbs(this Matrix mat)
        {
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c,r];
                    if(val < 0) mat[c,r] = Math.Abs(val);
                }
        }
        public static Matrix GetAbs(this Matrix mat)
        {
            Matrix absmat = mat.Clone();
            absmat.UpdateAbs();
            return absmat;
        }
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
        public static void UpdateMul(this IMatrix<double> _this, double other)
        {
            for(int c=0; c<_this.ColSize; c++)
                for(int r=0; r<_this.RowSize; r++)
                    _this[c, r] *= other;
        }

        public static Vector GetColVector(this IMatrix<double> _this, int row)
        {
            HDebug.AssertAnd(0<=row, row<_this.RowSize);
            double[] vec = new double[_this.ColSize];
            for(int col=0; col<_this.ColSize; col++)
                vec[col] = _this[col, row];
            return vec;
        }
        public static Vector GetRowVector(this IMatrix<double> _this, int col)
        {
            HDebug.AssertAnd(0<=col, col<_this.ColSize);
            double[] vec = new double[_this.RowSize];
            for(int row=0; row<_this.RowSize; row++)
                vec[row] = _this[col, row];
            return vec;
        }

        public static void SetValue(this IMatrix<double> _this, double value)
        {
            for(int c=0; c<_this.ColSize; c++)
                for(int r=0; r<_this.RowSize; r++)
                    _this[c, r] = value;
        }
    }
}
