using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public abstract partial class ILinAlg
	{
        public abstract class ILinAlgMat : IDisposable
        {
            public abstract ILinAlg la { get; }
            public abstract int ColSize { get; }
            public abstract int RowSize { get; }
            public abstract double this[int c, int r] { get; set; }
            public abstract ILinAlgMat Tr { get; }
            public abstract double[,] ToArray();
            public abstract void Dispose();

            public abstract ILinAlgMat op_Add(/*ILinAlgMat mat1, */ ILinAlgMat mat2);
            public abstract ILinAlgMat op_Sub(/*ILinAlgMat mat1, */ ILinAlgMat mat2);
            public abstract ILinAlgMat op_Mul(/*ILinAlgMat mat1, */ ILinAlgMat mat2);
            public abstract ILinAlgMat op_Mul(/*ILinAlgMat mat1, */ double     val2);
            public abstract ILinAlgMat op_Div(/*ILinAlgMat mat1, */ double     val2);

            public static ILinAlgMat operator+(ILinAlgMat mat1, ILinAlgMat mat2) { return mat1.op_Add(mat2); }
            public static ILinAlgMat operator-(ILinAlgMat mat1, ILinAlgMat mat2) { return mat1.op_Sub(mat2); }
            public static ILinAlgMat operator*(ILinAlgMat mat1, ILinAlgMat mat2) { return mat1.op_Mul(mat2); }
            public static ILinAlgMat operator*(ILinAlgMat mat1, double     val2) { return mat1.op_Mul(val2); }
            public static ILinAlgMat operator*(double     val1, ILinAlgMat mat2) { return mat2.op_Mul(val1); }
            public static ILinAlgMat operator/(ILinAlgMat mat1, double     val2) { return mat1.op_Div(val2); }

            public ILinAlgMat Diag() { return la.Diag(this); }
            public double     Det()  { return la.Det(this); }
        }

        ///////////////////////////////////////////////////////////////
        /// Usage:
        ///     ILinAlg ila = ...;
        /// 
        ///     using(var v = new ila.Disposables())
        ///     {
        ///         var mat = ila.ToILMat(new double[2,2]).AddDisposables();
        ///         ...
        ///         ...
        ///     }   ⇐ when v.Dispose() is called
        ///                 its all matrices calling AddDisposables() will be disposed all together
        ///
        public static Stack<Disposables> StackDisposables = new Stack<Disposables>();
        public Disposables NewDisposables() { return new Disposables(); }
        public static void AddDisposable<T>(T disposable)
            where T : IDisposable
        {
            HDebug.Assert(StackDisposables.Count == 1);
            if(StackDisposables.Count > 0)
                StackDisposables.Peek().AddDisposable(disposable);
        }
        public class Disposables : IDisposable
        {
            public HashSet<IDisposable> disposables = new HashSet<IDisposable>();
            public Disposables()
            {
                StackDisposables.Push(this);
            }
            public void AddDisposable(IDisposable disposable)
            {
                disposables.Add(disposable);
            }
            public void Dispose()
            {
                var disps = StackDisposables.Pop();
                Debug.Assert(object.ReferenceEquals(this, disps));
                foreach(var disposable in disposables)
                    disposable.Dispose();
            }
        }

        public abstract ILinAlgMat _ToILMat(Matrix mat);
        public ILinAlgMat ToILMat(Matrix mat)
        {
            Func<Matrix,ILinAlgMat> func = _ToILMat;
            if(HDebug.Selftest(func))
            {
            }
            return _ToILMat(mat);
        }
        public abstract ILinAlgMat _ToILMat(double[] mat);
        public ILinAlgMat ToILMat(double[] mat)
        {
            Func<double[],ILinAlgMat> func = _ToILMat;
            if(HDebug.Selftest(func))
            {
            }
            return _ToILMat(mat);
        }

        public abstract double _Det(ILinAlgMat mat);
        public double Det(ILinAlgMat mat)
        {
            Func<ILinAlgMat,double> func = _Det;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2, 3 }, { 2, 4, 5 }, { 3, 5, 6 } });
                double   tdet = Det(tA);
                if(Math.Abs(tdet - (-1.0)) > 0.00000001) Exit("Det(A) is wrong");
            }
            return func(mat);
        }

        public abstract ILinAlgMat _Diag(ILinAlgMat mat);
        public ILinAlgMat Diag(ILinAlgMat mat)
        {
            Func<ILinAlgMat,ILinAlgMat> func = _Diag;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2, 3 }, { 2, 4, 5 }, { 3, 5, 6 } });
                ILinAlgMat tADiag0 = Diag(tA);
                ILinAlgMat tADiag1 = ToILMat(new double[] { 1, 4, 6 });
                double tAerr = (tADiag1 - tADiag0).ToArray().HAbs().HMax();
                if(tAerr > 0.00000001) Exit("Diag(A) is wrong");

                ILinAlgMat tB = ToILMat(new double[] { 1, 2, 3 });
                ILinAlgMat tBDiag0 = Diag(tB);
                ILinAlgMat tBDiag1 = ToILMat(new double[,] { { 1, 0, 0 }, { 0, 2, 0 }, { 0, 0, 3 } });
                double tBerr = (tADiag1 - tADiag0).ToArray().HAbs().HMax();
                if(tBerr > 0.00000001) Exit("Diag(B) is wrong");
            }
            return func(mat);
        }

        public Func<double[,],Tuple<double[,],double[]>> FuncEigSymm
        {
            get
            {
                Func<double[,],Tuple<double[,],double[]>> fnEigSymm = delegate(double[,] A)
                {
                    ILinAlgMat AA = ToILMat(A);
                    var VVDD = EigSymm(AA);
                    ILinAlgMat VV = VVDD.Item1;
                    double[]    D = VVDD.Item2;
                    double[,]   V = VV.ToArray();
                    AA.Dispose();
                    VV.Dispose();
                    return new Tuple<double[,], double[]>(V, D);
                };
                return fnEigSymm;
            }
        }
        public abstract Tuple<ILinAlgMat, double[]> _EigSymm(ILinAlgMat A);
        public Tuple<ILinAlgMat, double[]> EigSymm(ILinAlgMat A)
        {
            Func<ILinAlgMat,Tuple<ILinAlgMat, double[]>> func = _EigSymm;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2, 3 }, { 2, 4, 5 }, { 3, 5, 6 } });
                var tVD = EigSymm(tA);
                ILinAlgMat tV = tVD.Item1;
                double[] tD  = tVD.Item2;
                ILinAlgMat tDD = Diag(ToILMat(tD));

                ILinAlgMat tAA = Mul(tV, tDD, tV.Tr);
                double err = (tAA - tA).ToArray().HAbs().HMax();
                if(err > 0.00000001) Exit("EigSymm, A != V D V'");
                ILinAlgMat tV_Vt = Mul(tV, tV.Tr);
                ILinAlgMat tI = Eye(3);
                err = (tV_Vt - tI).ToArray().HAbs().HMax();
                if(err > 0.00000001) Exit("EigSymm, I != V V'");
            }
            return func(A);
        }

        public abstract ILinAlgMat _Mul(params ILinAlgMat[] mats);
        public ILinAlgMat Mul(params ILinAlgMat[] mats)
        {
            Func<ILinAlgMat[],ILinAlgMat> func = _Mul;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } });
                ILinAlgMat tB = ToILMat(new double[,] { { 7, 8 }, { 9, 1 } });
                ILinAlgMat tC = ToILMat(new double[,] { { 2, 3, 4 }, { 5, 6, 7 } });
                ILinAlgMat tR = Mul(tA, tB, tC);
                ILinAlgMat tRR = ToILMat(new double[,] { { 100, 135, 170 }, { 254, 339, 424 }, { 408, 543, 678 } });
                double terr = (tR - tRR).ToArray().HAbs().HMax();
                if(terr > 0.00000001) Exit("Mul, (A * B * C) is wrong");
            }
            return func(mats);
        }

        public abstract ILinAlgMat _PInv(ILinAlgMat A);
        public ILinAlgMat PInv(ILinAlgMat A)
        {
            Func<ILinAlgMat,ILinAlgMat> func = _PInv;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA   = ToILMat(new double[,] { { 1, 2, 3 }, { 2, 3, 5 }, { 3, 4, 5 } });
                ILinAlgMat tiA  = PInv(tA);
                ILinAlgMat tiAA = ToILMat(new double[,] { { -2.5, 1.0, 0.5 }, { 2.5, -2.0, 0.5 }, { -0.5, 1.0, -0.5 } });
                double err0 = (tiA - tiAA).ToArray().HAbs().HMax();
                if(err0 > 0.00000001) Exit("Pinv, is incorrect");
                ILinAlgMat tI = Eye(3);
                double err1 = (tA * tiA - tI).ToArray().HAbs().HMax();
                if(err1 > 0.00000001) Exit("Pinv, A * invA != I");
                double err2 = (tiA * tA - tI).ToArray().HAbs().HMax();
                if(err2 > 0.00000001) Exit("Pinv, invA * A != I");
            }
            return func(A);
        }

        public abstract ILinAlgMat _LinSolve(ILinAlgMat A, ILinAlgMat B);
        public ILinAlgMat LinSolve(ILinAlgMat A, ILinAlgMat B)
        {
            Func<ILinAlgMat,ILinAlgMat,ILinAlgMat> func = _LinSolve;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
                ILinAlgMat tB = ToILMat(new double[,] { { 3, 4, 5 }, { 6, 7, 8 }, { 9,10,11 } });
                ILinAlgMat tX = _LinSolve(tA, tB);
                double err = (tA*tX - tB).ToArray().HAbs().HMax();
                if(err > 0.00000001) Exit("LinSolve, is incorrect");
            }
            return func(A, B);
        }

        public abstract ILinAlgMat _Inv(ILinAlgMat A);
        public ILinAlgMat Inv(ILinAlgMat A)
        {
            Func<ILinAlgMat,ILinAlgMat> func = _Inv;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2, 3 }, { 4, 9, 6 }, { 7, 8, 9 } });
                ILinAlgMat tI = ToILMat(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
                ILinAlgMat tInvA = func(tA);
                double err = (tInvA*tA - tI).ToArray().HAbs().HMax();
                if(err > 0.00000001) Exit("LinSolve, is incorrect");
            }
            return func(A);
        }

        public abstract ILinAlgMat _InvSymm(ILinAlgMat A);
        public ILinAlgMat InvSymm(ILinAlgMat A)
        {
            Func<ILinAlgMat,ILinAlgMat> func = _InvSymm;
            if(HDebug.Selftest(func))
            {
                ILinAlgMat tA = ToILMat(new double[,] { { 1, 2, 3 }, { 2, 4, 5 }, { 3, 5, 6 } });
                ILinAlgMat tI = ToILMat(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
                ILinAlgMat tInvA = func(tA);
                double err = (tInvA*tA - tI).ToArray().HAbs().HMax();
                if(err > 0.00000001) Exit("LinSolve, is incorrect");
            }
            return func(A);
        }

        public abstract ILinAlgMat _Eye(int size);
        public ILinAlgMat Eye(int size)
        {
            Func<int,ILinAlgMat> func = _Eye;
            if(HDebug.Selftest(func))
            {
            }
            return func(size);
        }

        public abstract ILinAlgMat _Zeros(int colsize, int rowsize);
        public ILinAlgMat Zeros(int colsize, int rowsize)
        {
            Func<int,int,ILinAlgMat> func = _Zeros;
            if(HDebug.Selftest(func))
            {
            }
            return func(colsize, rowsize);
        }

        public abstract ILinAlgMat _Ones(int colsize, int rowsize);
        public ILinAlgMat Ones(int colsize, int rowsize)
        {
            Func<int,int,ILinAlgMat> func = _Ones;
            if(HDebug.Selftest(func))
            {
            }
            return func(colsize, rowsize);
        }

        public static void Exit(string message)
        {
            System.Console.Error.WriteLine("SelfTest Error: "+message);
            throw new Exception();
            // System.Environment.Exit(-1);
        }


        //public ILinAlgMat[] ToILMat(params double[][,] mats)
        //{
        //    ILinAlgMat[] lmats = new ILinAlgMat[mats.Length];
        //    for(int i=0; i<mats.Length; i++)
        //        lmats[i] = ToILMat(mats[i]);
        //    return lmats;
        //}
        //public void Dispose(params ILinAlgMat[] mats)
        //{
        //    foreach(ILinAlgMat mat in mats)
        //        mat.Dispose();
        //}
        //public ILinAlgMat[] ToILMat(params Matrix[] mats)
        //{
        //    ILinAlgMat[] lmats = new ILinAlgMat[mats.Length];
        //    for(int i=0; i<mats.Length; i++)
        //        lmats[i] = ToILMat(mats[i].ToArray());
        //    return lmats;
        //}
        //public Matrix Mul(params Matrix[] mats)
        //{
        //    ILinAlgMat[] lmats = ToILMat(mats);
        //    ILinAlgMat mat = Mul(lmats);
        //    Matrix     mul = mat.ToArray();
        //    Dispose(lmats);
        //    Dispose(mat);
        //    return mul;
        //}
    }
}
