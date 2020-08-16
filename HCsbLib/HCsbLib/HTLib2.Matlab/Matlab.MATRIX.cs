using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public partial class Matlab
    {
        class MATRIX : IMatrix<double>
	    {
            public double[,] _data;
            public int ColSize                                      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _data.GetLength(0); } }
            public int RowSize                                      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _data.GetLength(1); } }
            public double this[ int c,  int r]                      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _data[c, r]; } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { _data[c, r] = value; } }
            public double this[long c, long r]                      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _data[c, r]; } [MethodImpl(MethodImplOptions.AggressiveInlining)] set { _data[c, r] = value; } }
            public static MATRIX Zeros(int colsize, int rowsize)    { return new MATRIX{ _data = new double[colsize, rowsize] }; }
            public double[,] ToArray()                              { return _data; }
        }
    }
}
