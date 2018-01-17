using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public abstract partial class ILinAlg
	{
        public Tuple<Matrix, Vector> FnEigSymm(Matrix A)
        {
            if(HDebug.Selftest())
            {
                Matrix tA = new double[,] { { 1, 2 }, { 2, 1 } };
                var tVD = FnEigSymm(tA);
                Matrix tV = new double[,] { { -0.7071, 0.7071 }, { 0.7071, 0.7071 } };
                Vector      tD = new double[] { -1, 3 };
                HDebug.AssertToleranceMatrix(0.0001, tV-tVD.Item1);
                HDebug.AssertToleranceVector(0     , tD-tVD.Item2);
            }
            var AA = ToILMat(A);
            var VVDD = EigSymm(AA);
            return new Tuple<Matrix, Vector>(VVDD.Item1.ToArray(), VVDD.Item2);
        }
        public Matrix FnMul(Matrix A, Matrix B, Matrix C)
        {
            if(HDebug.Selftest())
            {
                Matrix tA = new double[,] { { 1, 2 } };
                Matrix tB = new double[,] { { 2, 3 }, { 4, 5 } };
                Matrix tC = new double[,] { { 6 }, { 7 } };
                Matrix tABC0 = new double[,] { { 151 } };
                Matrix tABC1 = FnMul(tA, tB, tC);
                HDebug.AssertToleranceMatrix(0, tABC0-tABC1);
            }
            if(C == null)
                return FnMul(A, B);
            var AA=ToILMat(A);
            var BB=ToILMat(B);
            var CC=ToILMat(C);
            var AABBCC = AA*BB*CC;
            AA.Dispose();
            BB.Dispose();
            CC.Dispose();
            Matrix ABC = AABBCC.ToArray();
            AABBCC.Dispose();
            return ABC;
        }
        public Matrix FnMul(Matrix A, Matrix B)
        {
            if(HDebug.Selftest())
            {
                Matrix tA = new double[,] { { 1, 2 } };
                Matrix tB = new double[,] { { 2, 3 }, { 4, 5 } };
                Matrix tAB0 = new double[,] { { 10, 13 } };
                Matrix tAB1 = FnMul(tA, tB);
                HDebug.AssertToleranceMatrix(0, tAB0-tAB1);
            }

            var AA=ToILMat(A);
            var BB=ToILMat(B);
            var AABB = AA*BB;
            AA.Dispose();
            BB.Dispose();
            Matrix AB = AABB.ToArray();
            AABB.Dispose();
            return AB;
        }
    }
}
