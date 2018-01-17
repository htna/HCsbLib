using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    [Serializable]
    public abstract partial class Matrix : IMatrix<double>
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // IMatrix

        public abstract int ColSize { get; }
        public abstract int RowSize { get; }
        public abstract double this[int c, int r] { get; set; }
        public abstract double this[long c, long r] { get; set; }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // abstract

        public abstract Matrix Clone();

        public static bool Zeros_Selftest = HDebug.IsDebuggerAttached;
        public static Matrix Zeros(int colsize, int rowsize)
        {
            if(Zeros_Selftest)
            {
                Zeros_Selftest = false;
                HDebug.Assert(Matrix.Zeros(  3,   2).IsZero());
                HDebug.Assert(Matrix.Zeros(100, 100).IsZero());
            }
            if((colsize < 10) && (rowsize < 10))
                return MatrixByArr.Zeros(colsize, rowsize);
            return MatrixByColRow.Zeros(colsize, rowsize);
        }
        public static Matrix Ones(int colsize, int rowsize)
        {
            return Ones(colsize, rowsize, 1);
        }
        public static Matrix Ones(int colsize, int rowsize, double mul)
        {
            if(Zeros_Selftest)
            {
                Zeros_Selftest = false;
                Matrix tm0 = new double[2, 2] { { 1, 1 }, { 1, 1 } };
                Matrix tm1 = tm0 * 1.1;
                HDebug.Assert((tm0          - Ones(2, 2     )).IsZero());
                HDebug.Assert((tm1          - Ones(2, 2, 1.1)).IsZero());
                HDebug.Assert((Zeros(10,10) - Ones(10, 10, 0)).IsZero());
            }
            Matrix mat = null;
            if((colsize < 10) && (rowsize < 10))    mat = MatrixByArr   .Zeros(colsize, rowsize);
            else                                    mat = MatrixByColRow.Zeros(colsize, rowsize);
            if(mul == 0)
                return mat;

            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    mat[c, r] = mul;
            return mat;
        }

        public virtual double[,] ToArray()
        {
            double[,] arr = new double[ColSize, RowSize];
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    arr[c, r] = this[c, r];
            return arr;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // operators

        public static implicit operator Matrix(double[,] mat)
        {
            return new MatrixByArr(mat);
        }

        public static Matrix operator-(Matrix left               ) { Matrix mat = left.Clone(); mat.UpdateMul(-1     ); return mat; }
        public static Matrix operator+(Matrix left, IMatrix<double> right) { Matrix mat = left.Clone(); mat.UpdateAdd(right, 1); return mat; }
        public static Matrix operator-(Matrix left, IMatrix<double> right) { Matrix mat = left.Clone(); mat.UpdateAdd(right,-1); return mat; }
        public static Matrix operator*(Matrix left, IMatrix<double> right) { Matrix mat = left.GetMul(right);                   return mat; }
        public static Matrix operator*(Matrix left, double  right) { Matrix mat = left.Clone(); mat.UpdateMul(right  ); return mat; }
        public static Matrix operator*(double left, Matrix  right) { Matrix mat = right.Clone(); mat.UpdateMul(left  ); return mat; }
        public static Matrix operator/(Matrix left, double  right) { Matrix mat = left.Clone(); mat.UpdateMul(1/right); return mat; }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // ToString()
        public override string ToString()
		{
            //return "no display...";
            StringBuilder str = new StringBuilder();
			str.Append("Matrix ["+ColSize+","+RowSize+"] ");
            str.Append(HToString("0.00000", null, "{{", "}}", ", ", "}, {", 100));
            return str.ToString();
		}
		//public string ToString(string format)
		//{
        //    return ToString(format, null, "{{", "}}", ", ", "}, {");
		//}
		//public string ToString(string coldelim, string rowdelim)
		//{
        //    return ToString(null, null, "{{", "}}", coldelim, rowdelim);
		//}
		//public string ToString(string format, string coldelim, string rowdelim)
		//{
        //    return ToString(format, null, "{{", "}}", coldelim, rowdelim);
		//}
		public string HToString( string format = "0.00000"
                               , IFormatProvider formatProvider = null
                               , string begindelim = "{{"
                               , string enddelim   = "}}"
                               , string rowdelim   = ", "
                               , string coldelim   = "}, {"
                               , int? maxcount     = null
                               )
		{
			StringBuilder str = new StringBuilder();
			str.Append(begindelim);

            int count = 0;

			for(int c = 0; c < ColSize; c++) {
				if(c != 0) str.Append(coldelim);

				for(int r = 0; r < RowSize; r++) {
					if(r != 0) str.Append(rowdelim);
					// str += this[c, r].ToString(format, formatProvider);
                    if(maxcount != null && count > maxcount.Value)
                        break;
					str.Append(this[c, r].ToString(format));
                    count++;
				}
			}

			str.Append(enddelim);
			return str.ToString();
		}

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // others

        public virtual void UpdateAdd(IMatrix<double> other, double other_mul)
        {
            HDebug.Exception(ColSize == other.ColSize);
            HDebug.Exception(RowSize == other.RowSize);
            if(other_mul == 1)
            {
                for(int c=0; c<ColSize; c++)
                    for(int r=0; r<RowSize; r++)
                        this[c, r] += other[c, r];
            }
            else if(other_mul == -1)
            {
                for(int c=0; c<ColSize; c++)
                    for(int r=0; r<RowSize; r++)
                        this[c, r] -= other[c, r];
            }
            else
            {
                for(int c=0; c<ColSize; c++)
                    for(int r=0; r<RowSize; r++)
                        this[c, r] += (other[c, r] * other_mul);
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
        public virtual Matrix GetMul(IMatrix<double> right)
        {
            return Matrix.GetMul(this, right);
        }
        public static Matrix GetMul(Matrix left, IMatrix<double> right)
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
        public virtual void UpdateMul(double other)
        {
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    this[c, r] *= other;
        }

        public virtual Vector GetColVector(int row)
        {
            HDebug.AssertAnd(0<=row, row<RowSize);
            double[] vec = new double[ColSize];
            for(int col=0; col<ColSize; col++)
                vec[col] = this[col, row];
            return vec;
        }
        public virtual Vector GetRowVector(int col)
        {
            HDebug.AssertAnd(0<=col, col<ColSize);
            double[] vec = new double[RowSize];
            for(int row=0; row<RowSize; row++)
                vec[row] = this[col, row];
            return vec;
        }

        public void SetValue(double value)
        {
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                    this[c, r] = value;
        }

    }
}
