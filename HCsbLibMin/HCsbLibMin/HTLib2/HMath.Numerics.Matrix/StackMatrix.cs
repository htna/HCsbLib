using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	public struct ValueMatrix3x3 : IMatrix<double>
	{
		double v00, v01, v02;
        double v10, v11, v12;
        double v20, v21, v22;

        public int ColSize { get { return 3; } }    //public int NumRows { get { return ColSize; } }
        public int RowSize { get { return 3; } }    //public int NumCols { get { return RowSize; } }
        public double this[int  c, int  r] { get { return GetAt(c, r); } set { SetAt(c, r, value); } }
        public double this[long c, long r] { get { return GetAt(c, r); } set { SetAt(c, r, value); } }
        public double[,] ToArray()
        {
            return new double[3, 3]
            {
                { v00, v01, v02 },
                { v10, v11, v12 },
                { v20, v21, v22 },
            };
        }
        public double GetAt(long c, long r)
        {
            if(c == 0)
            {
                if(r == 0) return v00;
                if(r == 1) return v01;
                if(r == 2) return v02;
                throw new IndexOutOfRangeException();
            }
            if(c == 1)
            {
                if(r == 0) return v10;
                if(r == 1) return v11;
                if(r == 2) return v12;
                throw new IndexOutOfRangeException();
            }
            if(c == 2)
            {
                if(r == 0) return v20;
                if(r == 1) return v21;
                if(r == 2) return v22;
                throw new IndexOutOfRangeException();
            }
            throw new IndexOutOfRangeException();
        }
        public void SetAt(long c, long r, double value)
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
