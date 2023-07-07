using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HCsbLibImport
{
    public static partial class Alglib
    {
        public static double[] NonNegativeLeastSquares
            ( double[,] A // argmin || A x - b ||_2^2
            , double[]  b // subjto x >= 0
            )
        {
            /// https://en.wikipedia.org/wiki/Non-negative_least_squares
            /// 
            /// argmin_x {A x - y}_2^2 subject to x>=0
            /// 
            /// argmin (1/2 * x' Q x  +  c' x)
            /// subjto (x >= 0)
            /// where Q = A' A
            ///       c = -A' y
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            HDebug.Assert(b.Length == n);

            double[,] Q = new double[m, m];
            double[]  c = new double[m   ];
            for(int i=0; i<n; i++)
            {
                for(int j0=0; j0<m; j0++)
                {
                    c[j0] += -1 * A[i,j0] * b[i];
                    for(int j1=0; j1<m; j1++)
                        Q[j0,j1] += A[i,j0] * A[i,j1];
                }
            }

            double[] x0     = new double[m];
            double[] x_bndl = new double[m];
            double[] x_bndu = new double[m];
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
        public static double[] NonNegativeLeastSquares_AA_Ab
            ( double[,] AA // argmin || A x - b ||_2^2 == argmin (x' A' A x - 2 b' A x + b' b) == argmin 2 (0.5 x' Q x  =>  AA = A' A = Q
            , double[]  Ab // subjto x >= 0               subjto x >= 0                              Ab = A' b = -c
            , double[]  x0 = null
            )
        {
            // argmin || A x - b ||_2^2  =>  argmin (x' A' A x - 2 b' A x + b' b)  =>  argmin 2 (0.5 x' (A' A) x + (- A' b)' x + 0.5 b' b)
            // subjto x >= 0                 subjto x >= 0                             subjto x >= 0
            //
            // argmin ( 0.5 x' Q x   +   c' x )  <=  Q =   A' A
            // subjto x_bndl < x < x_bndu            c = - A' b
            //                                       x_bndl = 0
            //                                       x_bndu = double.PositiveInfinity

            /// https://en.wikipedia.org/wiki/Non-negative_least_squares
            int n = AA.GetLength(0);
            HDebug.Assert(n == AA.GetLength(1));
            HDebug.Assert(n == Ab.Length      );

            double[,] Q = AA;
            double[]  c = new double[n];    for(int i=0; i<n; i++) c[i] = -1 * Ab[i];

            if(x0 != null) {
                HDebug.Assert(n == x0.Length);
            } else {
                x0 = new double[n];
                for(int i=0; i<x0.Length; i++)
                    x0[i] = 1;
            }
            double[] x_bndl = new double[n];
            double[] x_bndu = new double[n];
            for(int i=0; i<x0.Length; i++)
            {
                x0    [i] = 1;
                x_bndl[i] = 0;
                x_bndu[i] = double.PositiveInfinity;
            }

            // argmin || A x - b ||_2^2  =>  argmin (x' A' A x - 2 b' A x + b' b)  =>  argmin 2 (0.5 x' (A' A) x + (- A' b)' x + 0.5 b' b)
            // subjto x >= 0                 subjto x >= 0                             subjto x >= 0
            double[] x = ConstrainedQuadraticProgramming(Q, c, x0, x_bndl, x_bndu, null);
            // argmin ( 0.5 x' Q x   +   c' x )
            // subjto x_bndl < x < x_bndu
            HDebug.Assert(x != null);

            return x;
        }
    }
}
