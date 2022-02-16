using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class Alglib
    {
        public static double[] NonPositiveLeastSquares
            ( double[,] A
            , double[]  b
            )
        {
            ///   argmin || A x - b ||_2^2              subjto x <= 0
            /// 
            ///   argmin || A (- xx) - b ||_2^2         subjto -xx <= 0     where x = -xx
            /// = argmin (-A xx - b)' (-A xx - b)       subjto  xx >= 0
            /// = argmin -(A xx + b)' -(A xx + b)       subjto  xx >= 0
            /// = argmin  (A xx - -b)' (A xx - -b)      subjto  xx >= 0
            /// = argmin  (A xx - bb)' (A xx - bb)      subjto  xx >= 0     where bb = -b
            /// = argmin || A xx - bb ||_2^2            subjto  xx >= 0

            double[] bb = new double[b.Length];
            for(int i=0; i<bb.Length; i++)
                bb[i] = -1 * b[i];

            double[] xx = NonNegativeLeastSquares(A, bb);
            if(xx == null)
                return null;

            double[] x = new double[xx.Length];
            for(int i=0; i<xx.Length; i++)
                x[i] = -1 * xx[i];

            return x;
        }
        public static double[] NonPositiveLeastSquares_AA_Ab
            ( double[,] AA
            , double[]  Ab
            )
        {
            /// argmin || A x - b  ||_2^2                subjto x <= 0
            /// 
            /// argmin || A xx - bb ||_2^2               subjto x >= 0      where bb = -b
            /// AA = A' A  =  Q
            /// Ab = A' bb = A' -b = - A' b = -c
            /// Q = A' A = AA
            /// c = A' b = Ab

            int n = AA.GetLength(0);
            HDebug.Assert(n == AA.GetLength(1));
            HDebug.Assert(n == Ab.Length      );

            double[,] Q = AA;
            double[]  c = Ab;

            double[] x0     = new double[n];
            double[] x_bndl = new double[n];
            double[] x_bndu = new double[n];
            for(int i=0; i<x0.Length; i++)
            {
                x0    [i] = 1;
                x_bndl[i] = 0;
                x_bndu[i] = double.PositiveInfinity;
            }

            double[] x = ConstrainedQuadraticProgramming(Q, c, x0, x_bndl, x_bndu, null);
            HDebug.Assert(x != null);

            return x;
        }
    }
}
