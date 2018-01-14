using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class MatrixByArr
    {
		public static Vector operator*(Vector lvec, MatrixByArr rmat)
		{
			return LinAlg.VtM(lvec, rmat);
		}
		public static Vector operator*(MatrixByArr lmat, Vector rvec)
		{
			return LinAlg.MV(lmat, rvec);
		}
        public static implicit operator double[,](MatrixByArr mat)
        {
            return mat._data;
        }
        public static implicit operator MatrixByArr(double[,] mat)
        {
            return new MatrixByArr(mat);
        }
        static bool selftest_operator_equal = true;
        public static bool operator==(MatrixByArr lmat, MatrixByArr rmat)
        {
            if(HDebug.IsDebuggerAttached && selftest_operator_equal)
            #region self test
            {
                selftest_operator_equal = false;
                MatrixByArr A0 = new MatrixByArr(new double[,] { { 1, 2 }, { 3, 4 } });
                MatrixByArr A1 = new MatrixByArr(new double[,] { { 1, 2 }, { 3, 4 } });
                MatrixByArr B0 = new MatrixByArr(new double[,] { { 1, 2, 3, 4 } });
                MatrixByArr B1 = new MatrixByArr(new double[,] { { 0, 2 }, { 3, 4 } });
                HDebug.Assert(A0 == A1);
                HDebug.Assert(A0 != B0);
                HDebug.Assert(A0 != B1);
            }
            #endregion
            if(object.ReferenceEquals(lmat, rmat)) return true;
            if(object.ReferenceEquals(lmat,null) || object.ReferenceEquals(rmat,null)) return false;
            if(lmat._data == rmat._data) return true;
            if(lmat._data==null ||rmat._data==null) return false;
            if(lmat.ColSize != rmat.ColSize) return false;
            if(lmat.RowSize != rmat.RowSize) return false;
            for(int c=0; c<lmat.ColSize; c++)
                for(int r=0; r<lmat.RowSize; r++)
                    if(lmat[c, r] != rmat[c, r])
                        return false;
            return true;
        }
        public static bool operator!=(MatrixByArr lmat, MatrixByArr rmat)
        {
            return !(lmat == rmat);
        }
        public static MatrixByArr operator -(MatrixByArr mat)
        {
            MatrixByArr result = mat.CloneT();
            int colsize = mat.ColSize;
            int rowsize = mat.RowSize;
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    result[c, r] = -result[c, r];
            return result;
        }
        public static MatrixByArr operator+(MatrixByArr lmat, MatrixByArr rmat)
        {
            HDebug.Assert(lmat.ColSize == rmat.ColSize);
            HDebug.Assert(lmat.RowSize == rmat.RowSize);
            int colsize = lmat.ColSize;
            int rowsize = lmat.RowSize;
            MatrixByArr result = new MatrixByArr(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    result[c, r] = lmat[c, r] + rmat[c, r];
            return result;
        }
        public static MatrixByArr operator+(MatrixByArr lmat, double val)
        {
            int colsize = lmat.ColSize;
            int rowsize = lmat.RowSize;
            MatrixByArr result = new MatrixByArr(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    result[c, r] = lmat[c, r] + val;
            return result;
        }
        public static MatrixByArr operator-(MatrixByArr lmat, MatrixByArr rmat)
        {
            HDebug.Assert(lmat.ColSize == rmat.ColSize);
            HDebug.Assert(lmat.RowSize == rmat.RowSize);
            int colsize = lmat.ColSize;
            int rowsize = lmat.RowSize;
            MatrixByArr result = new MatrixByArr(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    result[c, r] = lmat[c, r] - rmat[c, r];
            return result;
        }
        public static MatrixByArr operator*(MatrixByArr lmat, MatrixByArr rmat)
        {
            HDebug.Assert(lmat.RowSize == rmat.ColSize);
            int colsize = lmat.ColSize;
            int rowsize = rmat.RowSize;
            MatrixByArr result = new MatrixByArr(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    for(int i=0; i<lmat.RowSize; i++)
                        result[c, r] += lmat[c, i] * rmat[i, r];
            return result;
        }
        public static MatrixByArr operator*(double lmat, MatrixByArr rmat)
        {
            int colsize = rmat.ColSize;
            int rowsize = rmat.RowSize;
            MatrixByArr result = new MatrixByArr(colsize, rowsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                    result[c, r] = lmat * rmat[c, r];
            return result;
        }
        public static MatrixByArr operator*(MatrixByArr lmat, double rmat)
        {
            return rmat * lmat;
        }
        public static MatrixByArr operator/(MatrixByArr lmat, double rmat)
        {
            return (1/rmat) * lmat;
        }
    }
}
