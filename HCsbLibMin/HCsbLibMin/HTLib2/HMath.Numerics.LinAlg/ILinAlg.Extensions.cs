using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    using ILinAlgMat = ILinAlg.ILinAlgMat;
    public static class ILinAlgExtension
    {
        public static ILinAlg.ILinAlgMat AddDisposable(this ILinAlg.ILinAlgMat mat)
        {
            ILinAlg.AddDisposable(mat);
            return mat;
        }
        public static Tuple<ILinAlg.ILinAlgMat,double[]> AddDisposable(this Tuple<ILinAlg.ILinAlgMat,double[]> MV)
        {
            MV.Item1.AddDisposable();
            return MV;
        }
        public static Matrix ToMatrix(this ILinAlg.ILinAlgMat AA)
        {
            Matrix A = Matrix.Zeros(AA.ColSize, AA.RowSize);
            for(int c=0; c<A.ColSize; c++)
                for(int r=0; r<A.RowSize; r++)
                    A[c, r] = AA[c, r];
            return A;
        }
        public static Matrix HInv(this ILinAlg ila, Matrix A, string invtype, params object[] invopt)
        {
            Matrix invA;
            {
                ILinAlgMat AA    = ila.ToILMat(A);
                ILinAlgMat invAA = ila.HInv(AA, invtype, invopt);
                invA = invAA.ToMatrix();
                AA.Dispose();
                invAA.Dispose();
            }
            GC.Collect();
            return invA;
        }
        public static ILinAlgMat HInv(this ILinAlg ila, ILinAlgMat AA, string invtype, params object[] invopt)
        {
            if(invtype == null)
                invtype = "inv";
            object optparam = null;

            switch(invtype)
            {
                case "inv":
                    return ila.Inv(AA);
                case "pinv":
                    return ila.PInv(AA);
                case "eig-zerothres":
                    {
                        var VVDD = ila.EigSymm(AA);

                        double threshold = (double)invopt[0];
                        for(int i=0; i<VVDD.Item2.Length; i++)
                            if(Math.Abs(VVDD.Item2[i]) < threshold)
                                VVDD.Item2[i] = 0;

                        optparam = VVDD;
                    }
                    goto case "eig-VVDD";
                case "eig-zerocount":
                    {
                        var VVDD = ila.EigSymm(AA);

                        int zerocount = (int)invopt[0];
                        for(int i=0; i<zerocount; i++)
                            VVDD.Item2[i] = 0;

                        optparam = VVDD;
                    }
                    goto case "eig-VVDD";
                case "eig-VVDD":
                    {
                        Tuple<ILinAlgMat, double[]> VVDD = optparam as Tuple<ILinAlgMat, double[]>;
                        for(int i=0; i<VVDD.Item2.Length; i++)
                        {
                            if(VVDD.Item2[i] != 0)
                                VVDD.Item2[i] = 1 / VVDD.Item2[i];
                        }
                        var invAA = ila.Mul(VVDD.Item1, ila.ToILMat(VVDD.Item2).Diag(), VVDD.Item1.Tr);
                        if(HDebug.IsDebuggerAttached)
                        {
                            var check = AA * invAA;
                            check.Dispose();
                        }
                        VVDD.Item1.Dispose();
                        return invAA;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
        public static Matrix PInv(this ILinAlg ila, Matrix A)
        {
            var AA      = ila.ToILMat(A);
            var invAA   = ila.PInv(AA);
            Matrix invA = ToMatrix(invAA);
            AA.Dispose();
            invAA.Dispose();
            GC.Collect();
            return invA;
        }
        public static Matrix Inv(this ILinAlg ila, Matrix A)
        {
            var AA      = ila.ToILMat(A);
            var invAA   = ila.Inv(AA);
            Matrix invA = ToMatrix(invAA);
            AA.Dispose();
            invAA.Dispose();
            GC.Collect();
            return invA;
        }
        public static Matrix InvSymm(this ILinAlg ila, Matrix A)
        {
            var AA      = ila.ToILMat(A);
            var invAA   = ila.InvSymm(AA);
            Matrix invA = ToMatrix(invAA);
            AA.Dispose();
            invAA.Dispose();
            GC.Collect();
            return invA;
        }
        public static Matrix Mul(this ILinAlg ila, params Matrix[] mats)
        {
            ILinAlgMat[] MATS = new ILinAlgMat[mats.Length];
            for(int i=0; i<mats.Length; i++)
                MATS[i] = ila.ToILMat(mats[i]);
            ILinAlgMat MUL = ila.Mul(MATS);
            Matrix mul = ToMatrix(MUL);
            MUL.Dispose();
            foreach(var MAT in MATS) MAT.Dispose();
            return mul;
        }
        public static Tuple<Matrix, Vector> EigSymm(this ILinAlg ila, Matrix A)
        {
            var AA     = ila.ToILMat(A);
            var VVDD   = ila.EigSymm(AA);
            Matrix V = VVDD.Item1.ToMatrix();
            Vector D = VVDD.Item2;
            AA.Dispose();
            VVDD.Item1.Dispose();
            GC.Collect();
            return new Tuple<Matrix, Vector>(V, D);
        }
        public static Matrix LinSolve(this ILinAlg ila, Matrix A, Matrix B)
        {
            var AA = ila.ToILMat(A);
            var BB = ila.ToILMat(B);
            var XX = ila.LinSolve(AA, BB);
            Matrix X = XX.ToMatrix();
            AA.Dispose();
            BB.Dispose();
            XX.Dispose();
            GC.Collect();
            return X;
        }
    }
}
