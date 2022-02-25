using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HCsbLibImport
{
    public static partial class Alglib
    {
        public static double[] ConstrainedQuadraticProgramming
            ( double[,] A       // argmin ( 0.5 x' A x   +   b' x )
            , double[]  b       // subjto x_bndl < x < x_bndu
            , double[]  x0      // 
            , double[]  x_bndl  //
            , double[]  x_bndu  //
            , double[]  s = null//
            , string    option = null
            )
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            /// This example demonstrates minimization of F(x0,x1) = x0^2 + x1^2 -6*x0 - 4*x1
            /// subject to bound constraints 0<=x0<=2.5, 0<=x1<=2.5
            ///
            /// Exact solution is [x0,x1] = [2.5,2]
            ///
            /// We provide algorithm with starting point. With such small problem good starting
            /// point is not really necessary, but with high-dimensional problem it can save us
            /// a lot of time.
            ///
            /// IMPORTANT: this solver minimizes  following  function:
            ///     f(x) = 0.5*x'*A*x + b'*x.
            /// Note that quadratic term has 0.5 before it. So if you want to minimize
            /// quadratic function, you should rewrite it in such way that quadratic term
            /// is multiplied by 0.5 too.
            /// For example, our function is f(x)=x0^2+x1^2+..., but we rewrite it as 
            ///     f(x) = 0.5*(2*x0^2 + 2*x1^2) + (-6*x0 + -4*x1)
            ///          = 0.5 [x0 x1] [2 0] [x0] + [-6 -4] [x0]
            ///                        [0 2] [x1]           [x1]
            ///          = 0.5  x'  A  x          +      b'  x
            /// and pass diag(2,2) as quadratic term - NOT diag(1,1)!
            #region ...
            ///
            /// double[,] a   = new double[,]{{2,0},{0,2}};
            /// double[] b    = new double[]{-6,-4};
            /// double[] x0   = new double[]{0,1};
            /// double[] s    = new double[]{1,1};
            /// double[] bndl = new double[]{0.0,0.0};
            /// double[] bndu = new double[]{3.0,3.5};
            /// double[] x;
            /// alglib.minqpstate state;
            /// alglib.minqpreport rep;
            /// 
            /// // create solver, set quadratic/linear terms
            /// alglib.minqpcreate(2, out state);
            /// alglib.minqpsetquadraticterm(state, a);
            /// alglib.minqpsetlinearterm(state, b);
            /// alglib.minqpsetstartingpoint(state, x0);
            /// alglib.minqpsetbc(state, bndl, bndu);
            /// 
            /// // Set scale of the parameters.
            /// // It is strongly recommended that you set scale of your variables.
            /// // Knowing their scales is essential for evaluation of stopping criteria
            /// // and for preconditioning of the algorithm steps.
            /// // You can find more information on scaling at http://www.alglib.net/optimization/scaling.php
            /// alglib.minqpsetscale(state, s);
            /// 
            /// // solve problem with Cholesky-based QP solver
            /// alglib.minqpsetalgocholesky(state);
            /// alglib.minqpoptimize(state);
            /// alglib.minqpresults(state, out x, out rep);
            /// System.Console.WriteLine("{0}", rep.terminationtype); // EXPECTED: 4
            /// System.Console.WriteLine("{0}", alglib.ap.format(x,2)); // EXPECTED: [2.5,2]
            /// 
            /// // solve problem with BLEIC-based QP solver
            /// // default stopping criteria are used.
            /// alglib.minqpsetalgobleic(state, 0.0, 0.0, 0.0, 0);
            /// alglib.minqpoptimize(state);
            /// alglib.minqpresults(state, out x, out rep);
            /// System.Console.WriteLine("{0}", alglib.ap.format(x,2)); // EXPECTED: [2.5,2]
            /// System.Console.ReadLine();
            #endregion
            ///////////////////////////////////////////////////////////////////////////////////////

            if(s == null)
            {
                s = new double[x0.Length];
                for(int i=0; i<s.Length; i++)
                    s[i] = 1;
            }

            double[] x;
            alglib.minqpstate state;
            alglib.minqpreport rep;

            // create solver, set quadratic/linear terms
            alglib.minqpcreate(4, out state);
            alglib.minqpsetquadraticterm(state, A);
            alglib.minqpsetlinearterm(state, b);
            alglib.minqpsetstartingpoint(state, x0);
            alglib.minqpsetbc(state, x_bndl, x_bndu);

            // Set scale of the parameters.
            // It is strongly recommended that you set scale of your variables.
            // Knowing their scales is essential for evaluation of stopping criteria
            // and for preconditioning of the algorithm steps.
            // You can find more information on scaling at http://www.alglib.net/optimization/scaling.php
            alglib.minqpsetscale(state, s);

            switch(option)
            {
                case "Cholesky-based QP solver":
                    // solve problem with Cholesky-based QP solver
                    //alglib.minqpsetalgocholesky(state);
                    alglib.minqpoptimize(state);
                    alglib.minqpresults(state, out x, out rep);
                    if(rep.terminationtype == 4)
                        return x;
                    //System.Console.WriteLine("{0}", rep.terminationtype); // EXPECTED: 4
                    //System.Console.WriteLine("{0}", alglib.ap.format(x,2)); // EXPECTED: [2.5,2]
                    break;
                case null:
                case "BLEIC-based QP solver":
                    // solve problem with BLEIC-based QP solver
                    // default stopping criteria are used.
                    alglib.minqpsetalgobleic(state, 0.0, 0.0, 0.0, 0);
                    alglib.minqpoptimize(state);
                    alglib.minqpresults(state, out x, out rep);
                    //System.Console.WriteLine("{0}", alglib.ap.format(x,2)); // EXPECTED: [2.5,2]
                    if(rep.terminationtype > 0)
                        return x;
                    break;
            }
            return null;
        }
    }
}
