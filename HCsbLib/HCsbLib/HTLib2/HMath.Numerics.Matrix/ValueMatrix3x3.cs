using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace HTLib2
{
	public struct ValueMatrix3x3 : IMatrix<double>
	{
		public double v00, v01, v02;
        public double v10, v11, v12;
        public double v20, v21, v22;

        public int ColSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return 3; } }    //public int NumRows { get { return ColSize; } }
        public int RowSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return 3; } }    //public int NumCols { get { return RowSize; } }
        public double this[int  c, int  r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(c, r); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(c, r, value); } }
        public double this[long c, long r] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return GetAt(c, r); } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { SetAt(c, r, value); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double[,] ToArray()
        {
            return new double[3, 3]
            {
                { v00, v01, v02 },
                { v10, v11, v12 },
                { v20, v21, v22 },
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double GetAt(long c, long r)
        {
            if(c == 0)
            {
                if     (r == 0) return v00;
                else if(r == 1) return v01;
                else if(r == 2) return v02;
                else throw new IndexOutOfRangeException();
            }
            else if(c == 1)
            {
                if     (r == 0) return v10;
                else if(r == 1) return v11;
                else if(r == 2) return v12;
                else throw new IndexOutOfRangeException();
            }
            else if(c == 2)
            {
                if     (r == 0) return v20;
                else if(r == 1) return v21;
                else if(r == 2) return v22;
                else throw new IndexOutOfRangeException();
            }
            throw new IndexOutOfRangeException();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void SetAt(long c, long r, double value)
        {
            if(c == 0)
            {
                if(r == 0) { v00 = value; return; }
                if(r == 1) { v01 = value; return; }
                if(r == 2) { v02 = value; return; }
                throw new IndexOutOfRangeException();
            }
            if(c == 1)
            {
                if(r == 0) { v10 = value; return; }
                if(r == 1) { v11 = value; return; }
                if(r == 2) { v12 = value; return; }
                throw new IndexOutOfRangeException();
            }
            if(c == 2)
            {
                if(r == 0) { v20 = value; return; }
                if(r == 1) { v21 = value; return; }
                if(r == 2) { v22 = value; return; }
                throw new IndexOutOfRangeException();
            }
            throw new IndexOutOfRangeException();
        }
        public static implicit operator double[,] (ValueMatrix3x3 mat)
        {
            return mat.ToArray();
            //  return new double[3, 3]
            //  {
            //      { v00, v01, v02 },
            //      { v10, v11, v12 },
            //      { v20, v21, v22 },
            //  };
        }
        public static implicit operator ValueMatrix3x3 (double[,] mat)
        {
            return new ValueMatrix3x3
            {
                v00 = mat[0,0],
                v01 = mat[0,1],
                v02 = mat[0,2],
                v10 = mat[1,0],
                v11 = mat[1,1],
                v12 = mat[1,2],
                v20 = mat[2,0],
                v21 = mat[2,1],
                v22 = mat[2,2],
            };
        }
		////////////////////////////////////////////////////////////////////////////////////
		// Functions
        //public Matrix Transpose()
        //{
        //    Matrix tran = new Matrix(RowSize, ColSize);
        //    for(int c=0; c<ColSize; c++)
        //        for(int r=0; r<RowSize; r++)
        //            tran[r, c] = this[c, r];
        //    return tran;
        //}
        //public double Trace()
        //{
        //    HDebug.Assert(ColSize == RowSize);
        //    double tr = 0;
        //    for(int i=0; i<ColSize; i++)
        //        tr += this[i, i];
        //    return tr;
        //}
        //public bool IsDiagonal()
        //{
        //    return IsDiagonal(0.00000001);
        //}
        //public bool IsDiagonal(double tolerance)
        //{
        //    if(RowSize != ColSize)
        //        return false;
        //    for(int r=0; r<RowSize; r++)
        //        for(int c=0; c<ColSize; c++)
        //            if((r != c) && (this[c, r] >= tolerance))
        //                return false;
        //    return true;
        //}
        //public Vector DiagonalVector()
        //{
        //    HDebug.Assert(RowSize == ColSize);
        //    Vector diagonal = new Vector(ColSize);
        //    for(int i=0; i<ColSize; i++)
        //        diagonal[i] = this[i, i];
        //    return diagonal;
        //}
        //
        //public double[,] ToArray()
        //{
        //    return _data;
        //}
        //public int[,] ToArrayInt()
        //{
        //    int[,] arr = new int[ColSize, RowSize];
        //    for(int c=0; c<ColSize; c++)
        //        for(int r=0; r<RowSize; r++)
        //            arr[c, r] = (int)this[c, r];
        //    return arr;
        //}
        //
		//public bool IsNaN              { get { for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsNaN             (this[c, r])) return true; return false; } }
		//public bool IsInfinity         { get { for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsInfinity        (this[c, r])) return true; return false; } }
		//public bool IsPositiveInfinity { get { for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsPositiveInfinity(this[c, r])) return true; return false; } }
		//public bool IsNegativeInfinity { get { for(int c=0; c<ColSize; c++) for(int r=0; r<RowSize; r++) if(double.IsNegativeInfinity(this[c, r])) return true; return false; } }
		//public bool IsComputable       { get { return ((IsNaN == false) && (IsInfinity == false)); } }

		////////////////////////////////////////////////////////////////////////////////////
		// ToString
		public override string ToString()
		{
			return ToString("0.00000", null, "{{", "}}", ", ", "}, {");
		}
		public string ToString(string format)
		{
            return ToString(format, null, "{{", "}}", ", ", "}, {");
		}
		public string ToString(string coldelim, string rowdelim)
		{
            return ToString(null, null, "{{", "}}", coldelim, rowdelim);
		}
		public string ToString(string format, string coldelim, string rowdelim)
		{
            return ToString(format, null, "{{", "}}", coldelim, rowdelim);
		}
		public string ToString(string format, IFormatProvider formatProvider, string begindelim, string enddelim, string rowdelim, string coldelim)
		{
			StringBuilder str = new StringBuilder();
			str.Append("Matrix ["+ColSize+","+RowSize+"] ");
			str.Append(begindelim);

            int count = 0;

			for(int c = 0; c < ColSize; c++) {
				if(c != 0) str.Append(coldelim);

				for(int r = 0; r < RowSize; r++) {
					if(r != 0) str.Append(rowdelim);
					// str += this[c, r].ToString(format, formatProvider);
                    if(count > 1000)
                        break;
					str.Append(this[c, r].ToString(format));
                    count++;
				}
			}

			str.Append(enddelim);
			return str.ToString();
		}
	}
}
