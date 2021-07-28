using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    [Serializable]
    public partial class MatrixByColRow : Matrix
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // member variables

        double[][]   arr;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix

        public override int ColSize { get { return arr.Length; } }
        public override int RowSize { get { return arr[0].Length; } }

        public override double this[int c, int r]
        {
            get { return arr[c][r]; }
            set { arr[c][r] = value; }
        }
        public override double this[long c, long r]
        {
            get { return arr[c][r]; }
            set { arr[c][r] = value; }
        }

        public override Matrix Clone()
        {
            return CloneT();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // others

        public MatrixByColRow(double[][] arr)
        {
            this.arr = arr;

            int colsize = arr.Length;
            int rowsize = arr[0].Length;
            for(int c=0; c<colsize; c++)
                if(arr[c].Length != rowsize)
                    throw new HException();
        }
        public MatrixByColRow CloneT()
        {
            double[][] narr = new double[arr.GetLength(0)][];

            for(int i=0; i<arr.GetLength(0); i++)
                narr[i] = arr[i].HClone();

            return new MatrixByColRow(narr);
        }

        public new static MatrixByColRow Zeros(int colsize, int rowsize)
        {
            double[][] arr = new double[colsize][];
            for(int c=0; c<colsize; c++)
                arr[c] = new double[rowsize];
            return new MatrixByColRow(arr);
        }

        public Vector GetRowVector(int col)
        {
            return arr[col];
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // IBinarySerializable
        public new void BinarySerialize(HBinaryWriter writer)
        {
            int leng = arr.Length;
            writer.Write(leng);
            for(int i=0; i<leng; i++)
                writer.Write(arr[i]);
        }
        public static new MatrixByColRow BinaryDeserialize(HBinaryReader reader)
        {
            int leng; reader.Read(out leng);
            double[][] arr = new double[leng][];
            for(int i=0; i<leng; i++)
                reader.Read(out arr[i]);

            MatrixByColRow mat = new MatrixByColRow(arr);
            return mat;
        }
        // IBinarySerializable
        //////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
