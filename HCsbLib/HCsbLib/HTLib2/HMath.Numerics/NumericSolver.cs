using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public abstract partial class NumericSolver
    {
        static Stack<NumericSolver> _solvers = new Stack<NumericSolver>();
        static NumericSolver solver { get { if(_solvers.Count==0) return null; return _solvers.Peek(); } }
        protected static void Register(NumericSolver solver)
        {
            NumericSolver._solvers.Push(solver);
        }
        public static NumericSolver GetRegistered()
        {
            return NumericSolver.solver;
        }
        protected static void Unregister(NumericSolver solver)
        {
            NumericSolver solver_ = NumericSolver._solvers.Pop();
            HDebug.Assert(solver == solver_);
        }


        //public static Matrix GetPInv(Matrix mat)
        //{
        //    return _solver.GetPInvImpl(mat);
        //}
        //protected virtual Matrix GetPInvImpl(Matrix mat)
        //{
        //    Debug.Assert(false);
        //    return null;
        //}
        protected abstract bool InvImpl(Matrix mat, out Matrix inv, InfoPack extra=null);
        public    static   bool Inv    (Matrix mat, out Matrix inv, InfoPack extra=null)
             { bool succ=solver.InvImpl(       mat, out        inv,          extra     ); HDebug.Assert(succ); return succ; }
        public    static Matrix Inv    (Matrix mat                , InfoPack extra=null) { Matrix inv; HDebug.Verify(solver.InvImpl(mat, out inv, extra)); return inv; }

        protected abstract bool RankImpl(Matrix mat, out int rank);
        public    static   bool Rank    (Matrix mat, out int rank)
             { bool succ=solver.RankImpl(       mat, out     rank); HDebug.Assert(succ); return succ; }
        public    static    int Rank    (Matrix mat              ) { int rank; HDebug.Verify(solver.RankImpl(mat, out rank)); return rank; }

        protected abstract bool PinvImpl(Matrix mat, out Matrix pinv, InfoPack extra=null);
        public    static   bool Pinv    (Matrix mat, out Matrix pinv, InfoPack extra=null)
             { bool succ=solver.PinvImpl(       mat, out        pinv,          extra     ); HDebug.Assert(succ); return succ; }
        public    static Matrix Pinv    (Matrix mat                 , InfoPack extra=null) { Matrix pinv; HDebug.Verify(solver.PinvImpl(mat, out pinv, extra)); return pinv; }

        protected abstract bool CorrImpl(Vector vec1, Vector vec2, out double corr);
        public    static   bool Corr    (Vector vec1, Vector vec2, out double corr)
             { bool succ=solver.CorrImpl(       vec1,        vec2, out        corr); HDebug.Assert(succ); return succ; }
        public    static double Corr    (Vector vec1, Vector vec2                 ) { double corr; HDebug.Verify(solver.CorrImpl(vec1, vec2, out corr)); return corr; }

        // eige-decomposition
        protected abstract bool EigImpl(Matrix mat, out Matrix   eigvec, out Vector   eigval);
        public    static   bool Eig    (Matrix mat, out Matrix   eigvec, out Vector   eigval)
             { bool succ=solver.EigImpl(       mat, out          eigvec, out          eigval); HDebug.Assert(succ); return succ; }
        public    static   bool Eig    (Matrix mat, out Vector[] eigvec, out double[] eigval)
        {
            Matrix _eigvec;
            Vector _eigval;
            bool succ=solver.EigImpl(       mat, out         _eigvec, out         _eigval);
            int size = _eigval.Size;
            eigval = _eigval.ToArray();
            eigvec = new Vector[size];
            for(int r=0; r<size; r++)
            {
                eigvec[r] = new double[size];
                for(int c=0; c<size; c++)
                    eigvec[r][c] = _eigvec[c, r];
            }
            //if(Debug.IsDebuggerAttachedWithProb(0.1))
            //{
            //    for(int i=0; i<size; i++)
            //        for(int j=0; j<size; j++)
            //        {
            //            if(Math.Abs(eigval[i]) < 0.00000001 && Math.Abs(eigval[j]) < 0.00000001)
            //                continue;
            //            double dot = Vector.VtV(eigvec[i], eigvec[j]);
            //            Debug.AssertIf(i==j, Math.Abs(dot-1) < 0.00000001);
            //            Debug.AssertIf(i!=j, Math.Abs(dot) < 0.00000001);
            //        }
            //}
            return succ;
        }

        // generalized eige-decomposition
        protected abstract bool EigImpl(Matrix A, Matrix B, out Matrix eigvec, out Vector eigval);
        public    static   bool Eig    (Matrix A, Matrix B, out Matrix eigvec, out Vector eigval)
             { bool succ=solver.EigImpl(       A,        B, out        eigvec, out        eigval); HDebug.Assert(succ); return succ; }

        // inverse matrix using eigen-decomposition
        protected abstract bool InvEigImpl(Matrix mat, double? thresEigval, int? numZeroEigval, out Matrix inv, InfoPack extra=null);
        public    static   bool InvEig    (Matrix mat, double? thresEigval, int? numZeroEigval, out Matrix inv, InfoPack extra=null)
             { bool succ=solver.InvEigImpl(       mat,         thresEigval,      numZeroEigval, out        inv,          extra     ); HDebug.Assert(succ); return succ; }
        public    static Matrix InvEig    (Matrix mat, double? thresEigval, int? numZeroEigval,                 InfoPack extra=null) { Matrix inv;
            HDebug.Verify(solver.InvEigImpl(       mat,         thresEigval,      numZeroEigval,        out inv,          extra)); return inv; }

        //// inverse matrix using generalized eigen-decomposition
        //protected abstract bool InvEigImpl(Matrix A, Matrix B, double thresEigval, out Matrix inv, InfoPack extra=null);
        //public    static   bool InvEig    (Matrix A, Matrix B, double thresEigval, out Matrix inv, InfoPack extra=null)
        //    { bool succ=_solver.InvEigImpl(       A,        B,        thresEigval, out        inv,          extra     ); Debug.Assert(succ); return succ; }
        //public    static Matrix InvEig    (Matrix A, Matrix B, double thresEigval,                 InfoPack extra=null) { Matrix inv;
        //   Debug.Verify(_solver.InvEigImpl(       A,        B,        thresEigval, out        inv,          extra     )); return inv; }

        protected abstract bool SvdImpl(Matrix X, out Matrix U, out Vector S, out Matrix V);
        public    static   bool Svd    (Matrix X, out Matrix U, out Vector S, out Matrix V)
             { bool succ=solver.SvdImpl(       X, out        U, out        S, out        V); HDebug.Assert(succ); return succ; }

        protected abstract bool LeastSquareConstrainedImpl(out Vector x, Matrix C, Vector d, Matrix A=null, Vector b=null, Matrix Aeq=null, Vector beq=null, Vector lb=null, Vector ub=null, Vector x0=null, string options=null);
        public    static   bool LeastSquareConstrained    (out Vector x, Matrix C, Vector d, Matrix A=null, Vector b=null, Matrix Aeq=null, Vector beq=null, Vector lb=null, Vector ub=null, Vector x0=null, string options=null)
             { bool succ=solver.LeastSquareConstrainedImpl(out        x,        C,        d,        A,             b,             Aeq,             beq,             lb,             ub,             x0,             options     ); HDebug.Assert(succ); return succ; }
        
        protected abstract bool QuadraticProgrammingConstrainedImpl(out Vector x, Matrix H, Vector f, Matrix A=null, Vector b=null, Matrix Aeq=null, Vector beq=null, Vector lb=null, Vector ub=null, Vector x0=null, string options=null);
        public    static   bool QuadraticProgrammingConstrained    (out Vector x, Matrix H, Vector f, Matrix A=null, Vector b=null, Matrix Aeq=null, Vector beq=null, Vector lb=null, Vector ub=null, Vector x0=null, string options=null)
             { bool succ=solver.QuadraticProgrammingConstrainedImpl(out        x,        H,        f,        A,             b,             Aeq,             beq,             lb,             ub,             x0,             options     ); HDebug.Assert(succ); return succ; }
    }
}
