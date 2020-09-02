using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static MatrixByArr ToMatrixByArr(this Matrix mat)
        {
            return new MatrixByArr(mat.ToArray());
        }
    }

    [Serializable]
    public partial class MatrixByArr : Matrix
	{
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // member variables
        
        double[,] _data;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix

        public override int ColSize { get { return _data.GetLength(0); } }
        public override int RowSize { get { return _data.GetLength(1); } }

        public override double this[int c, int r]
        {
            get { return _data[c, r]; }
            set { _data[c, r] = value; }
        }
        public override double this[long c, long r]
        {
            get { return _data[c, r]; }
            set { _data[c, r] = value; }
        }

        public override Matrix Clone()
        {
            return CloneT();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // others

        public MatrixByArr(double[,] data)
        {
            this._data = data;
        }

        public MatrixByArr CloneT()
        {
            return new MatrixByArr((double[,])_data.Clone());
        }

        public new static MatrixByArr Zeros(int colsize, int rowsize)
        {
            return new MatrixByArr(new double[colsize, rowsize]);
        }

        public override double[,] ToArray()
        {
            return _data;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // ...

        public MatrixByArr Tr()
        {
            MatrixByArr tr = new double[RowSize, ColSize];
            for(int c=0; c<tr.ColSize; c++)
                for(int r=0; r<tr.RowSize; r++)
                    tr[c, r] = this[r, c];
            return tr;
        }

        public MatrixByArr(int colsize, int rowsize)
		{
			HDebug.Assert(colsize >= 0);
			HDebug.Assert(rowsize >= 0);
			this._data = new double[colsize, rowsize];
		}
		public MatrixByArr(MatrixByArr mat)
		{
			if(mat._data == null)
			{
				_data = null;
			}
			this._data = new double[mat.ColSize, mat.RowSize];
			for(int c=0; c<ColSize; c++)
				for(int r=0; r<RowSize; r++)
					_data[c, r] = mat._data[c, r];
		}
    }
}
