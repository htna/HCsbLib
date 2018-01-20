using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Matrix : Matrix<double>
    {
        public Matrix(int ColSize, int RowSize)
            : base(ColSize, RowSize)
        {
        }
        protected Matrix(double[] data, int ColSize, int RowSize)
            : base(data, ColSize, RowSize)
        {
        }
        public Matrix(double[,] mat)
            : base(mat)
        {
        }
        public static implicit operator double[,](Matrix mat)
        {
            double[,] arr = new double[mat.ColSize, mat.RowSize];
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    arr[c, r] = mat[c, r];
            return arr;
        }
        public static implicit operator Matrix(double[,] arr)
        {
            return new Matrix(arr);
        }
        public static implicit operator HTLib2.Matrix(Matrix mat)
        {
            double[,] arr = new double[mat.ColSize, mat.RowSize];
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    arr[c, r] = mat[c, r];
            return arr;
        }
        public override string ToString()
        {
            string format = "0.0000";
            return ToString(format);
        }
        public string ToString(string format)
        {
            Func<double, string> tostring = delegate(double val) { return val.ToString(format); };
            return ToString(null, tostring);
        }
    }
}
