using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	public partial class Matlab
	{
        public class NumericSolver : HTLib2.NumericSolver
        {
            static NumericSolver _singletol = null;
            public static void Register()
            {
                if(_singletol != null)
                    return;
                _singletol = new NumericSolver();
                NumericSolver.Register(_singletol);
            }
            public static void Unregister()
            {
                if(_singletol == null)
                    return;
                NumericSolver.Unregister(_singletol);
                _singletol = null;
            }
            protected override bool InvImpl(Matrix mat, out Matrix inv, InfoPack extra)
            {
                NamedLock.FuncO<Matrix,bool> func = delegate(out Matrix linv)
                {
                    Matlab.Clear("HTLib2_Matlab_InvImpl");
                    Matlab.PutMatrix("HTLib2_Matlab_InvImpl", mat.ToArray());
                    if(extra != null)
                        extra.SetValue("rank", Matlab.GetValueInt("rank(HTLib2_Matlab_InvImpl)"));
                    Matlab.Execute("HTLib2_Matlab_InvImpl = inv(HTLib2_Matlab_InvImpl);");
                    linv = Matlab.GetMatrix("HTLib2_Matlab_InvImpl");
                    Matlab.Clear("HTLib2_Matlab_InvImpl");
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.InvImpl(Matrix , out Matrix , InfoPack)", func, out inv);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_InvImpl"), func, out inv);
            }
            protected override bool RankImpl(Matrix mat, out int rank)
            {
                NamedLock.FuncO<int,bool> func = delegate(out int lrank)
                {
                    string varname = "HTLib2_Matlab_RankImpl";
                    Matlab.Clear(varname);
                    Matlab.PutMatrix(varname, mat.ToArray());
                    lrank = Matlab.GetValueInt("rank("+varname+")");
                    Matlab.Clear(varname);
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.RankImpl(Matrix, out int)", func, out rank);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_RankImpl"), func, out rank);
            }
            protected override bool PinvImpl(Matrix mat, out Matrix pinv, InfoPack extra)
            {
                NamedLock.FuncO<Matrix,bool> func = delegate(out Matrix lpinv)
                {
                    Matlab.Clear("HTLib2_Matlab_PinvImpl");
                    Matlab.PutMatrix("HTLib2_Matlab_PinvImpl", mat.ToArray());
                    if(extra != null)
                        extra.SetValue("rank", Matlab.GetValueInt("rank(HTLib2_Matlab_PinvImpl)"));
                    Matlab.Execute("HTLib2_Matlab_PinvImpl = pinv(HTLib2_Matlab_PinvImpl);");
                    lpinv = Matlab.GetMatrix("HTLib2_Matlab_PinvImpl");
                    Matlab.Clear("HTLib2_Matlab_PinvImpl");
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.PinvImpl(Matrix, out Matrix, InfoPack)", func, out pinv);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_PinvImpl"), func, out pinv);
            }
            protected override bool CorrImpl(Vector vec1, Vector vec2, out double corr)
            {
                NamedLock.FuncO<double,bool> func = delegate(out double lcorr)
                {
                    Matlab.Clear("HTLib2_Matlab_CorrImpl");
                    Matlab.PutVector("HTLib2_Matlab_CorrImpl.vec1", vec1.ToArray());
                    Matlab.PutVector("HTLib2_Matlab_CorrImpl.vec2", vec2.ToArray());
                    Matlab.Execute("HTLib2_Matlab_CorrImpl.corr = corr(HTLib2_Matlab_CorrImpl.vec1, HTLib2_Matlab_CorrImpl.vec2);");
                    lcorr = Matlab.GetValue("HTLib2_Matlab_CorrImpl.corr");
                    Matlab.Clear("HTLib2_Matlab_CorrImpl");
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.CorrImpl(Vector, Vector, out double)", func, out corr);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_CorrImpl"), func, out corr);
            }
            protected override bool EigImpl(Matrix mat, out Matrix eigvec, out Vector eigval)
            {
                NamedLock.FuncOO<Matrix,Vector,bool> func = delegate(out Matrix leigvec, out Vector leigval)
                {
                    bool bUseFile = (mat.ColSize*mat.ColSize > 1000*1000);
                    Matlab.Clear("HTLib2_Matlab_EigImpl");
                    Matlab.PutMatrix("HTLib2_Matlab_EigImpl.A", mat.ToArray(), bUseFile);
                    Matlab.Execute("[HTLib2_Matlab_EigImpl.V, HTLib2_Matlab_EigImpl.D] = eig(HTLib2_Matlab_EigImpl.A);");
                    Matlab.Execute("HTLib2_Matlab_EigImpl.D = diag(HTLib2_Matlab_EigImpl.D);");
                    leigvec = Matlab.GetMatrix("HTLib2_Matlab_EigImpl.V", bUseFile);
                    leigval = Matlab.GetVector("HTLib2_Matlab_EigImpl.D");
                    Matlab.Clear("HTLib2_Matlab_EigImpl");
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.EigImpl(Matrix, out Matrix, out Vector)", func, out eigvec, out eigval);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_EigImpl"), func, out eigvec, out eigval);
            }
            protected override bool EigImpl(Matrix A, Matrix B, out Matrix eigvec, out Vector eigval)
            {
                NamedLock.FuncOO<Matrix,Vector,bool> func = delegate(out Matrix leigvec, out Vector leigval)
                {
                    Matlab.Clear("HTLib2_Matlab_EigImpl");
                    Matlab.PutMatrix("HTLib2_Matlab_EigImpl.A", A.ToArray());
                    Matlab.PutMatrix("HTLib2_Matlab_EigImpl.B", B.ToArray());
                    Matlab.Execute("[HTLib2_Matlab_EigImpl.V, HTLib2_Matlab_EigImpl.D] = eig(HTLib2_Matlab_EigImpl.A, HTLib2_Matlab_EigImpl.B);");
                    Matlab.Execute("HTLib2_Matlab_EigImpl.D = diag(HTLib2_Matlab_EigImpl.D);");
                    leigvec = Matlab.GetMatrix("HTLib2_Matlab_EigImpl.V");
                    leigval = Matlab.GetVector("HTLib2_Matlab_EigImpl.D");
                    Matlab.Clear("HTLib2_Matlab_EigImpl");
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.EigImpl(Matrix, Matrix, out Matrix, out Vector)", func, out eigvec, out eigval);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_EigImpl"), func, out eigvec, out eigval);
            }
            protected override bool InvEigImpl(Matrix mat, double? thresEigval, int? numZeroEigval, out Matrix inv, InfoPack extra)
            {
                NamedLock.FuncO<Matrix,bool> func = delegate(out Matrix linv)
                {
                    Matlab.Clear("HTLib2_Matlab_InvEigImpl");
                    Matlab.PutMatrix("HTLib2_Matlab_InvEigImpl.A", mat.ToArray());
                    Matlab.PutValue("HTLib2_Matlab_InvEigImpl.ze", numZeroEigval.GetValueOrDefault(0));
                    Matlab.PutValue("HTLib2_Matlab_InvEigImpl.th", Math.Abs(thresEigval.GetValueOrDefault(0)));
                    Matlab.Execute("[HTLib2_Matlab_InvEigImpl.V, HTLib2_Matlab_InvEigImpl.D] = eig(HTLib2_Matlab_InvEigImpl.A);");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.D = diag(HTLib2_Matlab_InvEigImpl.D);");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.sortAbsD = sort(abs(HTLib2_Matlab_InvEigImpl.D));");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.sortAbsD0 = [0; HTLib2_Matlab_InvEigImpl.sortAbsD];"); // add zero to the first list, for the case ze=0 (null)
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.ze  = HTLib2_Matlab_InvEigImpl.sortAbsD0(HTLib2_Matlab_InvEigImpl.ze+1);");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.th  = max(HTLib2_Matlab_InvEigImpl.th, HTLib2_Matlab_InvEigImpl.ze);");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.idx = abs(HTLib2_Matlab_InvEigImpl.D) <= HTLib2_Matlab_InvEigImpl.th;");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.invD = ones(size(HTLib2_Matlab_InvEigImpl.D)) ./ HTLib2_Matlab_InvEigImpl.D;");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.invD(HTLib2_Matlab_InvEigImpl.idx) = 0;");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.invD = diag(HTLib2_Matlab_InvEigImpl.invD);");
                    Matlab.Execute("HTLib2_Matlab_InvEigImpl.invA = HTLib2_Matlab_InvEigImpl.V * HTLib2_Matlab_InvEigImpl.invD * inv(HTLib2_Matlab_InvEigImpl.V);");
                    linv = Matlab.GetMatrix("HTLib2_Matlab_InvEigImpl.invA");
                    if(extra != null)
                    {
                        int num_zero_eigvals = Matlab.GetValueInt("sum(HTLib2_Matlab_InvEigImpl.idx)");
                        HDebug.AssertIf(numZeroEigval != null, numZeroEigval.GetValueOrDefault() <= num_zero_eigvals);
                        extra["num_zero_eigvals"] = num_zero_eigvals;
                        extra["eigenvalues"     ] = Matlab.GetVector("HTLib2_Matlab_InvEigImpl.D");
                    }
                    Matlab.Clear("HTLib2_Matlab_InvEigImpl");
 
                    return true;
                };
                //return NamedLock.LockedCall("bool HTLib2.Matlab.NumericSolver.InvEigImpl(Matrix, double?, int?, out Matrix, InfoPack)", func, out inv);
                return NamedLock.LockedCall(Matlab.NamedLock.GetName("HTLib2_Matlab_InvEigImpl"), func, out inv);
            }
            protected override bool SvdImpl(Matrix X, out Matrix U, out Vector S, out Matrix V)
            {
                using(new Matlab.NamedLock("NUMSLV"))
                {
                    Matlab.Clear("NUMSLV");
                    Matlab.PutMatrix("NUMSLV.X", X.ToArray());
                    Matlab.Execute("[NUMSLV.U, NUMSLV.S, NUMSLV.V] = svd(NUMSLV.X);");
                    Matlab.Execute("NUMSLV.S = diag(NUMSLV.S);");
                    U = Matlab.GetMatrix("NUMSLV.U");
                    S = Matlab.GetVector("NUMSLV.S");
                    V = Matlab.GetMatrix("NUMSLV.V");
                    Matlab.Clear("NUMSLV");
                }
                return true;
            }
            protected override bool LeastSquareConstrainedImpl( out Vector x
                                                              , Matrix C, Vector d
                                                              , Matrix A=null, Vector b=null
                                                              , Matrix Aeq=null, Vector beq=null
                                                              , Vector lb=null, Vector ub=null
                                                              , Vector x0=null
                                                              , string options=null
                                                              )
            {
                using(new Matlab.NamedLock("NUMSLV"))
                {
                    Matlab.Clear("NUMSLV");
                    Matlab.PutMatrix("NUMSLV.C", C.ToArray());
                    Matlab.PutVector("NUMSLV.d", d.ToArray());
                    Matlab.Execute("NUMSLV.A   = [];"); if(A   != null) Matlab.PutMatrix("NUMSLV.A"  , A  .ToArray());
                    Matlab.Execute("NUMSLV.b   = [];"); if(b   != null) Matlab.PutVector("NUMSLV.b"  , b  .ToArray());
                    Matlab.Execute("NUMSLV.Aeq = [];"); if(Aeq != null) Matlab.PutMatrix("NUMSLV.Aeq", Aeq.ToArray());
                    Matlab.Execute("NUMSLV.beq = [];"); if(beq != null) Matlab.PutVector("NUMSLV.beq", beq.ToArray());
                    Matlab.Execute("NUMSLV.lb  = [];"); if(lb  != null) Matlab.PutVector("NUMSLV.lb" , lb .ToArray());
                    Matlab.Execute("NUMSLV.ub  = [];"); if(ub  != null) Matlab.PutVector("NUMSLV.ub" , ub .ToArray());
                    Matlab.Execute("NUMSLV.x0  = [];"); if(x0  != null) Matlab.PutVector("NUMSLV.x0" , x0 .ToArray());
                    if(options == null) options = "[]";
                    Matlab.Execute("NUMSLV.x = lsqlin(NUMSLV.C, NUMSLV.d, NUMSLV.A, NUMSLV.b, NUMSLV.Aeq, NUMSLV.beq, NUMSLV.lb, NUMSLV.ub, NUMSLV.x0, "+options+");", true);
                    x = Matlab.GetVector("NUMSLV.x");
                    Matlab.Clear("NUMSLV");
                }
                return true;
            }
            protected override bool QuadraticProgrammingConstrainedImpl( out Vector x
                                                                       , Matrix H, Vector f
                                                                       , Matrix A=null, Vector b=null
                                                                       , Matrix Aeq=null, Vector beq=null
                                                                       , Vector lb=null, Vector ub=null
                                                                       , Vector x0=null
                                                                       , string options=null
                                                                       )
            {
                using(new Matlab.NamedLock("NUMSLV"))
                {
                    Matlab.Clear("NUMSLV");
                    Matlab.PutMatrix("NUMSLV.H", H.ToArray());
                    Matlab.PutVector("NUMSLV.f", f.ToArray());
                    Matlab.Execute("NUMSLV.A   = [];"); if(A   != null) Matlab.PutMatrix("NUMSLV.A"  , A  .ToArray());
                    Matlab.Execute("NUMSLV.b   = [];"); if(b   != null) Matlab.PutVector("NUMSLV.b"  , b  .ToArray());
                    Matlab.Execute("NUMSLV.Aeq = [];"); if(Aeq != null) Matlab.PutMatrix("NUMSLV.Aeq", Aeq.ToArray());
                    Matlab.Execute("NUMSLV.beq = [];"); if(beq != null) Matlab.PutVector("NUMSLV.beq", beq.ToArray());
                    Matlab.Execute("NUMSLV.lb  = [];"); if(lb  != null) Matlab.PutVector("NUMSLV.lb" , lb .ToArray());
                    Matlab.Execute("NUMSLV.ub  = [];"); if(ub  != null) Matlab.PutVector("NUMSLV.ub" , ub .ToArray());
                    Matlab.Execute("NUMSLV.x0  = [];"); if(x0  != null) Matlab.PutVector("NUMSLV.x0" , x0 .ToArray());
                    if(options == null) options = "[]";
                    Matlab.Execute("NUMSLV.x = quadprog(NUMSLV.H, NUMSLV.f, NUMSLV.A, NUMSLV.b, NUMSLV.Aeq, NUMSLV.beq, NUMSLV.lb, NUMSLV.ub, NUMSLV.x0, "+options+");", true);
                    x = Matlab.GetVector("NUMSLV.x");
                    Matlab.Clear("NUMSLV");
                }
                return true;
            }
        }
    }
}
