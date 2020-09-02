using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        public static Tuple<MatrixByArr,Vector> Eig(MatrixByArr A)
        {
            if(HDebug.Selftest())
            {
                MatrixByArr tA = new double[,]{{1,2,3},
                                          {2,9,5},
                                          {3,5,6}};
                MatrixByArr tV = new double[,]{{ -0.8879,  0.3782,  0.2618},
                                          { -0.0539, -0.6508,  0.7573},
                                          {  0.4568,  0.6583,  0.5983}};
                Vector tD = new double[] { -0.4219, 2.7803, 13.6416 };

                Tuple<MatrixByArr,Vector> tVD = Eig(tA);
                Vector tV0 = tVD.Item1.GetColVector(0); double tD0 = tVD.Item2[0];
                Vector tV1 = tVD.Item1.GetColVector(1); double tD1 = tVD.Item2[1];
                Vector tV2 = tVD.Item1.GetColVector(2); double tD2 = tVD.Item2[2];

                HDebug.AssertTolerance(0.00000001, 1-LinAlg.VtV(tV0, tV0));
                HDebug.AssertTolerance(0.00000001, 1-LinAlg.VtV(tV1, tV1));
                HDebug.AssertTolerance(0.00000001, 1-LinAlg.VtV(tV2, tV2));
                MatrixByArr tAA = tVD.Item1*LinAlg.Diag(tVD.Item2)*tVD.Item1.Tr();
                HDebug.AssertTolerance(0.00000001, tA-tAA);

                //HDebug.AssertTolerance(0.0001, VD.Item1-tV);
                HDebug.AssertTolerance(0.0001, tVD.Item2-tD);
            }

            HDebug.Assert(A.ColSize == A.RowSize);
            double[] eigval;
            double[,] eigvec;

            #region bool alglib.smatrixevd(double[,] a, int n, int zneeded, bool isupper, out double[] d, out double[,] z)
            /// Finding the eigenvalues and eigenvectors of a symmetric matrix
            /// 
            /// The algorithm finds eigen pairs of a symmetric matrix by reducing it to
            /// tridiagonal form and using the QL/QR algorithm.
            /// 
            /// Input parameters:
            ///     A       -   symmetric matrix which is given by its upper or lower
            ///                 triangular part.
            ///                 Array whose indexes range within [0..N-1, 0..N-1].
            ///     N       -   size of matrix A.
            ///     ZNeeded -   flag controlling whether the eigenvectors are needed or not.
            ///                 If ZNeeded is equal to:
            ///                  * 0, the eigenvectors are not returned;
            ///                  * 1, the eigenvectors are returned.
            ///     IsUpper -   storage format.
            /// 
            /// Output parameters:
            ///     D       -   eigenvalues in ascending order.
            ///                 Array whose index ranges within [0..N-1].
            ///     Z       -   if ZNeeded is equal to:
            ///                  * 0, Z hasn’t changed;
            ///                  * 1, Z contains the eigenvectors.
            ///                 Array whose indexes range within [0..N-1, 0..N-1].
            ///                 The eigenvectors are stored in the matrix columns.
            /// 
            /// Result:
            ///     True, if the algorithm has converged.
            ///     False, if the algorithm hasn't converged (rare case).
            /// 
            ///   -- ALGLIB --
            ///      Copyright 2005-2008 by Bochkanov Sergey
            /// 
            /// public static bool alglib.smatrixevd(
            ///     double[,] a,
            ///     int n,
            ///     int zneeded,
            ///     bool isupper,
            ///     out double[] d,
            ///     out double[,] z)
            #endregion
            bool success = alglib.smatrixevd(A, A.ColSize, 1, false, out eigval, out eigvec);

            if(success == false)
            {
                HDebug.Assert(false);
                return null;
            }
            return new Tuple<MatrixByArr, Vector>(eigvec, eigval);
        }
    }
}
