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
        public struct CLeastSquare
        {
            public double[] x                 ;
            public double? opt_mean_square_err;
            public double? opt_estimation_corr;
            public Vector  opt_estimation     ;
        }
        public static CLeastSquare LeastSquare
            ( double[,] As, double[] bs
            , bool opt_get_stat = false
            , object opt_inv = null
            )
        {
            if(HDebug.Selftest())
            {
                /// >> A = [ 1,3,2, 1 ; 4,5,6, 1 ; 7,9,9, 1 ; 11,11,12, 1 ; 13,16,15, 1 ]
                /// >> b = [1, 4, 6, 9, 12]'
                /// >> x = inv(A' * A) * (A' * b)
                ///     0.2171
                ///     0.2125
                ///     0.4205
                ///    -0.7339
                /// >> esti = A * x
                ///     0.9619
                ///     3.7203
                ///     6.4832
                ///     9.0381
                ///    11.7965
                /// >> corr(esti,b)
                ///     0.9976            
                /// >> mean( (b-esti).^2 )
                ///     0.0712
                double[,] _A = new double[5,4] { { 1,3,2,1 },{ 4,5,6,1 },{ 7,9,9,1 },{ 11,11,12,1 },{ 13,16,15,1 } };
                double[]  _b = new double[5] { 1, 4, 6, 9, 12 };
                dynamic _out = LeastSquare(_A, _b, true);

                double   _matlab_corr = 0.9976;
                double   _matlab_mse  = 0.0712;
                double[] _matlab_x    = new double[] { 0.2171, 0.2125, 0.4205, -0.7339 };
                double[] _matlab_esti = new double[] { 0.9619, 3.7203, 6.4832, 9.0381, 11.7965 };

                double err1 = Math.Abs(_matlab_corr  - _out.opt_estimation_corr);
                double err2 = Math.Abs(_matlab_mse   - _out.opt_mean_square_err);
                double err3 = (_matlab_x    - (Vector)_out.x             ).ToArray().MaxAbs();
                double err4 = (_matlab_esti - (Vector)_out.opt_estimation).ToArray().MaxAbs();

                HDebug.Assert(err1 < 0.0001);
                HDebug.Assert(err2 < 0.0001);
                HDebug.Assert(err3 < 0.0001);
                HDebug.Assert(err4 < 0.0001);
            }
            /// => A x = b
            /// 
            /// => At A x = At b
            /// 
            /// => AA * x = Ab
            /// => x = inv(AA) * Ab
            HDebug.Assert(As.GetLength(0) == bs.Length);
            int n = As.GetLength(0);
            int k = As.GetLength(1);
            
            Matrix AA = LinAlg.MtM(As, As);
            Vector Ab = LinAlg.MtV(As, bs);

            double[] x;
            LeastSquare(AA.ToArray(), Ab, out x, opt_inv);

            double? opt_mean_square_err = null;
            double? opt_estimation_corr = null;
            Vector  opt_estimation      = null;
            if(opt_get_stat)
            {
                opt_estimation = new double[n];
                double avg_err2 = 0;
                for(int i=0; i<n; i++)
                {
                    double esti = 0;
                    for(int j=0; j<k; j++)
                        esti += As[i,j] * x[j];

                    opt_estimation[i] = esti;
                    avg_err2 += (esti - bs[i])*(esti - bs[i]);
                }
                avg_err2 /= n;

                opt_mean_square_err = avg_err2;
                opt_estimation_corr = HMath.HCorr(opt_estimation, bs);
            }
            
            return new CLeastSquare
            {
                x = x,
                /// optional outputs
                opt_mean_square_err = opt_mean_square_err,
                opt_estimation_corr = opt_estimation_corr,
                opt_estimation      = opt_estimation     ,
            };
        }
        public static void LeastSquare
            ( double[,] At_A, double[] At_b
            , out double[] x
            , object opt_inv = null
            )
        {
            int k = At_A.GetLength(0);
            if(k != At_A.GetLength(0)) throw new ArgumentException("k != At_A.GetLength(0)");
            if(k != At_A.GetLength(1)) throw new ArgumentException("k != At_A.GetLength(1)");
            if(k != At_b.GetLength(0)) throw new ArgumentException("k != At_b.GetLength(0)");

            switch(k)
            {
                case 2: { Matrix invAA = LinAlg.Inv2x2(At_A); x = LinAlg.MV(invAA, At_b); } return;
                case 3: { Matrix invAA = LinAlg.Inv3x3(At_A); x = LinAlg.MV(invAA, At_b); } return;
                case 4: { Matrix invAA = LinAlg.Inv4x4(At_A); x = LinAlg.MV(invAA, At_b); } return;
                default:
                    switch(opt_inv)
                    {
                        case null:
                        case "matlab":
                            Matlab.PutMatrix("LinAlg_LeastSquare.AA", At_A);
                            Matlab.PutVector("LinAlg_LeastSquare.Ab", At_b);
                            Matlab.Execute("LinAlg_LeastSquare.AA = inv(LinAlg_LeastSquare.AA);");
                            Matlab.Execute("LinAlg_LeastSquare.x = LinAlg_LeastSquare.AA * LinAlg_LeastSquare.Ab;");
                            x = Matlab.GetVector("LinAlg_LeastSquare.x");
                            Matlab.Execute("clear LinAlg_LeastSquare;");
                            return;
                        case "matlab-pinv":
                            Matlab.PutMatrix("LinAlg_LeastSquare.AA", At_A);
                            Matlab.PutVector("LinAlg_LeastSquare.Ab", At_b);
                            Matlab.Execute("LinAlg_LeastSquare.AA = pinv(LinAlg_LeastSquare.AA);");
                            Matlab.Execute("LinAlg_LeastSquare.x = LinAlg_LeastSquare.AA * LinAlg_LeastSquare.Ab;");
                            x = Matlab.GetVector("LinAlg_LeastSquare.x");
                            Matlab.Execute("clear LinAlg_LeastSquare;");
                            return;
                        default:
                            if(opt_inv is Func<double[,], double[,]>)
                            {
                                Func<double[,], double[,]> inv = opt_inv as Func<double[,], double[,]>;
                                Matrix invAA = inv(At_A);
                                x = LinAlg.MV(invAA, At_b);
                                return;
                            }
                            throw new NotImplementedException();
                    }
            }
        }
        public static CLeastSquare LeastSquare
            ( IList<double>[] As, IList<double> bs
            , bool opt_get_stat = false
            , object opt_inv = null
            )
        {
            /// [ As[0][0], As[1][0], ... , As[k][0] ]   [x_0]   [ bs[0] ]
            /// [ As[0][1], As[1][1], ... , As[k][1] ] * [x_1] = [ bs[1] ]
            /// [ As[0][2], As[1][2], ... , As[k][2] ]   [...]   [ bs[2] ]
            /// [    ...  ,    ...  ,     ,    ...   ]   [x_k]   [  ...  ]
            /// [ As[0][n], As[1][n], ... , As[k][n] ]           [ bs[n] ]
            int k = As.Length;
            int n = As[0].Count;

            double[,] At_A = new double[k,k];
            double[ ] At_b = new double[k  ];

            if(bs.Count != As[0].Count)
                throw new ArgumentException("bs.Count != As.Length");
            for(int i=0; i<n; i++)
            {
                for(int a=0; a<k; a++)
                {
                    double Ai_a = As[a][i];// if(Ai_a == 0) Ai_a = 0.000000001;
                    double bi   = bs[i];
                    for(int b=0; b<k; b++)
                    {
                        double Ai_b = As[b][i];// if(Ai_b == 0) Ai_b = 0.000000001;
                        At_A[a,b] += Ai_a * Ai_b;
                    }
                    At_b[a] += Ai_a * bi;
                }
            }

            double[] x;
            LeastSquare(At_A, At_b, out x, opt_inv);

            double? opt_mean_square_err = null;
            double? opt_estimation_corr = null;
            Vector  opt_estimation      = null;
            if(opt_get_stat)
            {
                opt_estimation = new double[n];
                double avg_err2 = 0;
                for(int i=0; i<n; i++)
                {
                    double esti = 0;
                    for(int j=0; j<k; j++)
                        esti += As[j][i] * x[j];

                    opt_estimation[i] = esti;
                    avg_err2 += (esti - bs[i])*(esti - bs[i]);
                }
                avg_err2 /= n;

                opt_mean_square_err = avg_err2;
                opt_estimation_corr = HMath.HCorr(opt_estimation, bs.ToArray());
            }

            return new CLeastSquare
            {
                x = x,
                /// optional outputs
                opt_mean_square_err = opt_mean_square_err,
                opt_estimation_corr = opt_estimation_corr,
                opt_estimation      = opt_estimation     ,
            };
        }
    }
}
