using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        static bool selftest_KroneckerProduct = HDebug.IsDebuggerAttached;
        public static Matrix KroneckerProduct(Matrix A, Matrix B)
        {
            #region self-test
            if(HDebug.IsDebuggerAttached && selftest_KroneckerProduct)
            {
                selftest_KroneckerProduct = false;
                MatrixByArr _A = new MatrixByArr(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
                MatrixByArr _B = new MatrixByArr(new double[,] { { 7, 8 }, { 9, 10 } });
                MatrixByArr _mat = new MatrixByArr(new double[,] {{ 7,  8, 14, 16, 21, 24},
														{ 9, 10, 18, 20, 27, 30},
														{28, 32, 35, 40, 42, 48},
														{36, 40, 45, 50, 54, 60}});
                MatrixByArr _AB = KroneckerProduct(_A, _B).ToArray();
                HDebug.Assert(_mat  == _AB);
            }
            #endregion
            Matrix mat = Matrix.Zeros(A.ColSize*B.ColSize, A.RowSize*B.RowSize);
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    int ac = c / B.ColSize;
                    int ar = r / B.RowSize;
                    int bc = c % B.ColSize;
                    int br = r % B.RowSize;
                    mat[c, r] = A[ac, ar] * B[bc, br];
                }
            return mat;
        }
    }
}
