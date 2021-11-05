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

        public abstract int ColSize { get; } //public int NumRows { get { return ColSize; } }
        public abstract int RowSize { get; } //public int NumCols { get { return RowSize; } }
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

        public IEnumerable<double> EnumValues()
        {
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                {
                    double val = this[c, r];
                    yield return val;
                }
        }
        public IEnumerable<double> EnumNonZeroValues()
        {
            for(int c=0; c<ColSize; c++)
                for(int r=0; r<RowSize; r++)
                {
                    double val = this[c, r];
                    if(val != 0)
                        yield return val;
                }
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
        // IBinarySerializable
        public void BinarySerialize(HBinaryWriter writer)
        {
            switch(this)
            {
                case MatrixByColRow mbcr:
                    writer.Write("MatrixByColRow");
                    mbcr.BinarySerialize(writer);
                    return;
                case MatrixByArr mba:
                default:
                    throw new NotImplementedException();
            }
            //writer.Write(value);
        }
        public static Matrix BinaryDeserialize(HBinaryReader reader)
        {
            string type; reader.Read(out type);
            switch(type)
            {
                case "MatrixByColRow":
                    return MatrixByColRow.BinaryDeserialize(reader);
                case "MatrixByArr":
                default:
                    throw new NotImplementedException();
            }
        }
        // IBinarySerializable
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // ToString()
        public override string ToString()
		{
            //return "no display...";
            StringBuilder sb = new StringBuilder();
			sb.Append("Matrix ["+ColSize+","+RowSize+"] ");
            sb.Append(sb);
            //str.Append(HToString("0.00000", null, "{{", "}}", ", ", "}, {", 100));
            MatrixStatic.HToString
                ( this, sb
                , "0.00000", null, "{{", "}}", ", ", "}, {", 100
                );
            return sb.ToString();
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
		public string HToString
            ( string format = "0.00000"
            , IFormatProvider formatProvider = null
            , string begindelim = "{{"
            , string enddelim   = "}}"
            , string rowdelim   = ", "
            , string coldelim   = "}, {"
            , int? maxcount     = null
            )
		{
			StringBuilder sb = new StringBuilder();
            MatrixStatic.HToString
            ( this
            , sb
            , format
            , formatProvider
            , begindelim
            , enddelim
            , rowdelim
            , coldelim
            , maxcount
            );
            return sb.ToString();
		}
    }
}
