using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HCsbLibImport
{
    public static partial class Alglib
    {
        public static double[] ConstrainedLeastSquares
            ( double[,] A // argmin || A x - b ||_2^2
            , double[]  b // subjto x >= 0
            , double[]  x_bndl
            , double[]  x_bndu
            , double[]  x0 = null
            , string    option = null
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
            HDebug.Assert(n == b.Length);
            HDebug.Assert(m == x_bndl.Length);
            HDebug.Assert(m == x_bndu.Length);

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

            if(x0 != null)
            {
                HDebug.Assert(m == x0.Length);
            }
            else
            {
                x0 = new double[m];
                for(int i=0; i<x0.Length; i++)
                {
                    double xi0 = x_bndl[i]; HDebug.Assert(double.IsNaN(xi0) == false);
                    double xi1 = x_bndu[i]; HDebug.Assert(double.IsNaN(xi1) == false);
                    bool xi0inf = double.IsNegativeInfinity(xi0);
                    bool xi1inf = double.IsPositiveInfinity(xi1);
                    if(xi0inf== true  && xi1inf == true ) { x0[i] = 1;             continue; }
                    if(xi0inf== true  && xi1inf == false) { x0[i] = xi1 - 1;       continue; }
                    if(xi0inf== false && xi1inf == true ) { x0[i] = xi0 + 1;       continue; }
                    if(xi0inf== false && xi1inf == false) { x0[i] = (xi0 + xi1)/2; continue; }
                }
            }

            double[] x = ConstrainedQuadraticProgramming(Q, c, x0, x_bndl, x_bndu, null, option);
            HDebug.Assert(x != null);

            return x;
        }
        public static double[] ConstrainedLeastSquares_AA_Ab
            ( double[,] AA // argmin || A x - b ||_2^2 == argmin (x' A' A x - 2 b' A x + b' b) == argmin 2 (0.5 x' Q x  =>  AA = A' A = Q
            , double[]  Ab // subjto x >= 0               subjto x >= 0                              Ab = A' b = -c
            , double[]  x_bndl
            , double[]  x_bndu
            , double[]  x0 = null
            , string    option = null
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
            HDebug.Assert(n == AA.GetLength(0));
            HDebug.Assert(n == AA.GetLength(1));
            HDebug.Assert(n == Ab    .Length);
            HDebug.Assert(n == x_bndl.Length);
            HDebug.Assert(n == x_bndu.Length);

            double[,] Q = AA;
            double[]  c = new double[n];    for(int i=0; i<n; i++) c[i] = -1 * Ab[i];

            if(x0 != null)
            {
                HDebug.Assert(n == x0.Length);
            }
            else
            {
                x0 = new double[n];
                for(int i=0; i<x0.Length; i++)
                {
                    double xi0 = x_bndl[i]; HDebug.Assert(double.IsNaN(xi0) == false);
                    double xi1 = x_bndu[i]; HDebug.Assert(double.IsNaN(xi1) == false);
                    bool xi0inf = double.IsNegativeInfinity(xi0);
                    bool xi1inf = double.IsPositiveInfinity(xi1);
                    if(xi0inf== true  && xi1inf == true ) { x0[i] = 1;             continue; }
                    if(xi0inf== true  && xi1inf == false) { x0[i] = xi1 - 1;       continue; }
                    if(xi0inf== false && xi1inf == true ) { x0[i] = xi0 + 1;       continue; }
                    if(xi0inf== false && xi1inf == false) { x0[i] = (xi0 + xi1)/2; continue; }
                }
            }

            // argmin || A x - b ||_2^2  =>  argmin (x' A' A x - 2 b' A x + b' b)  =>  argmin 2 (0.5 x' (A' A) x + (- A' b)' x + 0.5 b' b)
            // subjto x >= 0                 subjto x >= 0                             subjto x >= 0
            double[] x = ConstrainedQuadraticProgramming(Q, c, x0, x_bndl, x_bndu, null, option);
            // argmin ( 0.5 x' Q x   +   c' x )
            // subjto x_bndl < x < x_bndu
            HDebug.Assert(x != null);

            return x;
        }
    }
}
