using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    [Serializable]
    public partial class MatrixByRowCol : Matrix
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // member variables

        double[][]   arr;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix

        public override int ColSize { get { return arr[0].Length; } }
        public override int RowSize { get { return arr.Length; } }

        public override double this[int c, int r]
        {
            get { return arr[r][c]; }
            set { arr[r][c] = value; }
        }
        public override double this[long c, long r]
        {
            get { return arr[r][c]; }
            set { arr[r][c] = value; }
        }

        public override Matrix Clone()
        {
            return CloneT();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // others

        public MatrixByRowCol(double[][] arr)
        {
            this.arr = arr;

            int rowsize = arr.Length;
            int colsize = arr[0].Length;
            for(int r=0; r<rowsize; r++)
                if(arr[r].Length != colsize)
                    throw new HException();
        }
        public MatrixByRowCol CloneT()
        {
            double[][] narr = new double[arr.GetLength(0)][];

            for(int i=0; i<arr.GetLength(0); i++)
                narr[i] = arr[i].HClone();

            return new MatrixByRowCol(narr);
        }

        public new static MatrixByRowCol Zeros(int colsize, int rowsize)
        {
            double[][] arr = new double[rowsize][];
            for(int r=0; r<rowsize; r++)
                arr[r] = new double[colsize];
            return new MatrixByRowCol(arr);
        }

        public override Vector GetColVector(int row)
        {
            return arr[row];
        }
    }
}
