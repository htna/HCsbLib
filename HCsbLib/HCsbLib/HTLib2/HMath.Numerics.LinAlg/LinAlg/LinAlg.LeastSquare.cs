using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class LinAlg
    {
        public static double[] LeastSquare
            ( double[] As, double[] bs
            , double[] mean_square_err=null
            )
        {
            /// => A x = b
            /// 
            /// => [A1, 1] * [ x ] = [b1]
            ///    [A2, 1]   [ t ]   [b2]      
            ///    [A3, 1]           [b3]
            ///    [...  ]           [..]
            /// 
            /// => At A xt = At b
            /// 
            /// => [A1 A2 A3] * [A1, 1] * [ x ] = [A1 A2 A3] * [b1]
            ///    [ 1  1  1]   [A2, 1]   [ t ]   [ 1  1  1]   [b2]
            ///                 [A3, 1]                        [b3]
            ///                 [...  ]                        [..]
            /// 
            /// => [A1^2 + A2^2 + A3^2 + ...,  A1+A2+A3+...] * [ x ] = [A1*b1 + A2*b2 + A3*b3 + ...]
            ///    [A1+A2+A3+...            ,  1+1+1+...   ]   [ t ] = [b1+b2+b3+...               ]
            ///
            /// => [sumA2, sumA ] * [ x ] = [sumAb]
            ///    [sumA , sum1 ]   [ t ] = [sumb ]
            ///
            /// => AA * xt = Ab
            /// => xt = inv(AA) * Ab
            double[,] AA = new double[2,2];
            double[]  Ab = new double[2];
            int n = As.Length;
            HDebug.Assert(n == As.Length);
            HDebug.Assert(n == bs.Length);
            for(int i=0; i<n; i++)
            {
                double ai = As[i];
                double bi = bs[i];
                double Ai2 = ai * ai;
                AA[0,0] += Ai2;
                AA[0,1] += ai;
                AA[1,0] += ai;
                AA[1,1] += 1;
                Ab[0] += ai * bi;
            }
            MatrixByArr invA = LinAlg.Inv2x2(AA);
            Vector xt        = LinAlg.MV(invA, Ab);

            if(mean_square_err != null)
            {
                HDebug.Assert(mean_square_err.Length == 1);
                double err2 = 0;
                double x = xt[0];
                double t = xt[1];
                for(int i=0; i<n; i++)
                {
                    double nbi = As[i] * x + t;
                    double erri = (nbi - bs[i]);
                    err2 += erri*erri;
                }
                mean_square_err[0] = err2/n;
            }

            return xt;
        }
    }
}
