using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        /// Lei Zhou and Steven A. Siegelbaum,
        /// Effects of Surface Water on Protein Dynamics Studied by a Novel Coarse-Grained Normal Mode Approach
        /// Biophysical Journal, Volume 94, May 2008
        /// http://www.ncbi.nlm.nih.gov/pmc/articles/PMC2292380/pdf/3461.pdf
        /// 
        /// Divide Hessian matrix into heavy atoms and others:
        ///   Hess = [ HH HL ]
        ///          [ LH LL ],
        /// where H and L imply heavy and light atoms, respectively.
        /// 
        /// Using block inverse matrix
        ///   [A B]-1 = [ (A-B D^-1 C)^-1  ... ]
        ///   [C D]     [ ...              ... ],
        /// find the HH coarse-grain block of Hessian matrix:
        ///   Hess_HH = HH - HL * LL^-1 * LH

        public static HessMatrix GetHessCoarseBlkmat(Matrix hess, IList<int> idx_heavy, ILinAlg ila, double? chkDiagToler, string invtype, params object[] invopt)
        {
            List<int> idxhess = new List<int>();
            foreach(int idx in idx_heavy)
            {
                idxhess.Add(idx*3+0);
                idxhess.Add(idx*3+1);
                idxhess.Add(idx*3+2);
            }

            HessMatrix hess_HH = new HessMatrixDense{ hess=hess.InvOfSubMatrixOfInv(idxhess, ila, invtype, invopt) };
            if(chkDiagToler == null)
                chkDiagToler = 0.000001;
            HDebug.Assert(Hess.CheckHessDiag(hess_HH, chkDiagToler.Value));
            return hess_HH;
        }
        public static HessMatrix GetHessCoarseBlkmat(Matrix hess, IList<int> idx_heavy, ILinAlg ila)
        {
            return GetHessCoarseBlkmat(hess, idx_heavy, ila, null, "inv");
        }
        public static Mode[] GetModeCoarseBlkmat(Matrix hess, IList<int> idx_heavy, ILinAlg ila)
        {
            HDebug.Depreciated("use GetHessCoarseBlkmat() and GetModesFromHess() separately");
            Matrix hess_HH = GetHessCoarseBlkmat(hess, idx_heavy, ila);
            //{
            //    Matlab.PutMatrix("H", hess);
            //    Matlab.Execute("H = (H + H')/2;");
            //
            //    Matlab.PutVector("idx0", idx_heavy.ToArray());
            //    Matlab.Execute("idx0 = sort([idx0*3+1; idx0*3+2; idx0*3+3]);");
            //    Matlab.PutValue("idx1", hess.ColSize);
            //    Matlab.Execute("idx1 = setdiff(1:idx1, idx0)';");
            //    HDebug.Assert(Matlab.GetValueInt("length(union(idx0,idx1))") == hess.ColSize*3);
            //
            //    Matlab.Execute("A = full(H(idx0,idx0));");
            //    Matlab.Execute("B =      H(idx0,idx1) ;");
            //    Matlab.Execute("C =      H(idx1,idx0) ;");
            //    Matlab.Execute("D = full(H(idx1,idx1));");
            //    Matlab.Execute("clear H;");
            //
            //    Matlab.Execute("bhess = A - B * inv(D) * C;");
            //    Matlab.Execute("bhess = (bhess + bhess')/2;");
            //}
            Mode[] modes = GetModesFromHess(hess_HH, ila);
            return modes;
        }
        public static Mode[] GetModeCoarseBlkmat(HessMatrix hess, IList<int> idx_heavy, ILinAlg ila)
        {
            HDebug.Depreciated("use GetHessCoarseBlkmat() and GetModesFromHess() separately");
            Matrix hess_HH = GetHessCoarseBlkmat(hess, idx_heavy);
            Mode[] modes = GetModesFromHess(hess_HH, ila);
            return modes;
        }
        public static HessMatrixDense GetHessCoarseBlkmat(HessMatrix hess, IList<int> idx_heavy, string invopt = "inv")
        {
            /// Hess = [ HH HL ] = [ A B ]
            ///        [ LH LL ]   [ C D ]
            /// 
            /// Hess_HH = HH - HL * LL^-1 * LH
            ///         = A  - B  *  D^-1 * C

            Matrix hess_HH;
            using(new Matlab.NamedLock(""))
            {
                Matlab.Clear();
                if(hess is HessMatrixSparse) Matlab.PutSparseMatrix("H", hess.GetMatrixSparse(), 3, 3);
                else                         Matlab.PutMatrix("H", hess, true);

                Matlab.Execute("H = (H + H')/2;");

                int[] idx0 = new int[idx_heavy.Count*3];
                for(int i=0; i<idx_heavy.Count; i++)
                {
                    idx0[i*3+0] = idx_heavy[i]*3+0;
                    idx0[i*3+1] = idx_heavy[i]*3+1;
                    idx0[i*3+2] = idx_heavy[i]*3+2;
                }
                Matlab.PutVector("idx0", idx0);
                Matlab.Execute("idx0 = idx0+1;");
                Matlab.PutValue("idx1", hess.ColSize);
                Matlab.Execute("idx1 = setdiff(1:idx1, idx0)';");
                HDebug.Assert(Matlab.GetValueInt("length(union(idx0,idx1))") == hess.ColSize);

                Matlab.Execute("A = full(H(idx0,idx0));");
                Matlab.Execute("B =      H(idx0,idx1) ;");
                Matlab.Execute("C =      H(idx1,idx0) ;");
                Matlab.Execute("D = full(H(idx1,idx1));");
                Matlab.Execute("clear H;");

                object linvopt = null;
                switch(invopt)
                {
                    case  "B/D":
                        Matlab.Execute("bhess = A -(B / D)* C;");
                        break;
                    case  "inv":
                        Matlab.Execute("D =  inv(D);");
                        Matlab.Execute("bhess = A - B * D * C;");
                        break;
                    case "pinv":
                        Matlab.Execute("D = pinv(D);");
                        Matlab.Execute("bhess = A - B * D * C;");
                        break;
                    case "_eig" :
                        Matlab.Execute("[D,DD] = eig(D);");
                        Matlab.Execute("DD(abs(DD)<"+linvopt+") = 0;");
                        Matlab.Execute("DD = pinv(DD);");
                        Matlab.Execute("D = D * DD * D';");
                        Matlab.Execute("clear DD;");
                        Matlab.Execute("bhess = A - B * D * C;");
                        break;
                    default:
                        {
                            if(invopt.StartsWith("eig(threshold:") && invopt.EndsWith(")"))
                            {
                                // ex: "eig(threshold:0.000000001)"
                                linvopt = invopt.Replace("eig(threshold:","").Replace(")","");
                                linvopt = double.Parse(linvopt as string);
                                goto case "_eig";
                            }
                        }
                        throw new HException();
                }

                Matlab.Execute("clear A; clear B; clear C; clear D;");
                Matlab.Execute("bhess = (bhess + bhess')/2;");
                hess_HH = Matlab.GetMatrix("bhess", Matrix.Zeros, true);

                Matlab.Clear();
            }
            return new HessMatrixDense { hess=hess_HH };
        }
        //public static MatrixByArr GetHessCoarseBlkmat_ilu(MatrixSparse<MatrixByArr> hess, IList<int> idx_heavy)
        //{
        //    HDebug.Depreciated("Do not use, because this does compute $inv(D)*C$ incorrectly!");
        //    throw new NotImplementedException();
        //    /// Hess = [ HH HL ]
        //    ///        [ LH LL ]
        //    /// 
        //    /// Hess_HH = HH - HL * LL^-1 * LH
        //    /// 
        //    /// [L,U,P] = LUDecompose(A)
        //    /// P A = L U
        //    /// A x = b => P A x = P b
        //    ///            L U x = P b
        //    ///            L y   = P b : use linear solver
        //    ///              U x = y   : use linear solver
        //    /// 
        //
        //    MatrixByArr hess_HH;
        //    using(new Matlab.NamedLock(""))
        //    {
        //        /// http://www.mathworks.com/help/matlab/ref/sparse.html
        //        /// S = sparse(i,j,s,m,n)
        //        /// * create m-by-n sparse matrix
        //        /// * where S(i(k),j(k)) = s(k)
        //        /// * Vectors i, j, and s are all the same length.
        //        /// * Any elements of s that are zero are ignored.
        //        /// * Any elementsof s that have duplicate values of i and j are added together. 
        //        Matlab.PutSparseMatrix("H", hess, 3, 3);
        //        Matlab.Execute("H = (H + H')/2;");
        //
        //        Matlab.PutVector("idx0", idx_heavy.ToArray());
        //        Matlab.Execute("idx0 = sort([idx0*3+1; idx0*3+2; idx0*3+3]);");
        //        Matlab.PutValue("idx1", hess.ColSize*3);
        //        Matlab.Execute("idx1 = setdiff(1:idx1, idx0)';");
        //        HDebug.Assert(Matlab.GetValueInt("length(union(idx0,idx1))") == hess.ColSize*3);
        //
        //        Matlab.Execute("A = H(idx0,idx0);");
        //        Matlab.Execute("B = H(idx0,idx1);");
        //        Matlab.Execute("C = H(idx1,idx0);");
        //        Matlab.Execute("D = H(idx1,idx1);");
        //
        //        /// http://www.mathworks.com/help/matlab/ref/ilu.html
        //        /// ilu(A,setup)
        //        /// [L,U] = ilu(A,setup)
        //        /// [L,U,P] = ilu(A,setup)
        //        Matlab.Execute("setup.type = 'ilutp';");
        //        Matlab.Execute("setup.milu = 'row';  ");
        //        Matlab.Execute("setup.droptol = 0;   ");
        //        Matlab.Execute("[DL,DU] = ilu(D,setup);");
        //        if(HDebug.IsDebuggerAttached && "true"=="false")
        //        {
        //            Matlab.Execute("chk.e    = rand(length(D),1);");
        //            Matlab.Execute("chk.norm = norm(D*chk.e - DL*DU*chk.e);");
        //            HDebug.AssertTolerance(0.00000001, Matlab.GetValue("chk.norm"));
        //        }
        //
        //        /// http://en.wikipedia.org/wiki/LU_decomposition#Solving_linear_equations
        //        /// [L,U] = LUDecompose(A)
        //        /// P A = L U
        //        /// A x = b => P A x = b
        //        ///            L U x = b
        //        ///            L y   = b : use linear solver
        //        ///              U x = y   : use linear solver
        //        /// ======================================================================
        //        /// x = D^-1 C
        //        /// D x = C
        //        /// DL DU x = C
        //        /// DL y    = C : use linear solver : y = linsolv(DL, C)
        //        ///    DU x = y : use linear solver : x = linsolv(DU, y)
        //        ///       x = D^-1 C
        //        /// ======================================================================
        //        /// http://www.mathworks.com/help/matlab/ref/lsqr.html
        //        /// x = lsqr(A,b)
        //        /// * attempts to solve the system of linear equations A*x=b for x if A is consistent.
        //        /// * otherwise it attempts to solve the least squares solution x that minimizes norm(b-A*x).
        //        Matlab.Execute("C = full(C);");
        //        Matlab.Execute("invDC = zeros(size(C));");
        //        Matlab.Execute("for i=1:size(C,2); invDC(:,i) =lsqr(DL,     C(:,i)); end;");
        //        Matlab.Execute("for i=1:size(C,2); invDC(:,i) =lsqr(DU, invDC(:,i)); end;");
        //        if(HDebug.IsDebuggerAttached && "true"=="false")
        //        {
        //            Matlab.Execute("chk.errInvDC = norm(inv(D)*C - invDC);");
        //            Matlab.Execute("chk.errInvDC2 = norm(inv(D)*C - D\\C);");
        //            System.Console.WriteLine("===========================================================");
        //        }
        //
        //        /// Hess_HH = HH - HL * LL^-1 * LH
        //        ///         =  A -  B *  D^-1 * C
        //        ///         =  A -  B * invDC
        //        Matlab.Execute("bhess = A - B * invDC;");
        //        Matlab.Execute("bhess = full(bhess);");
        //        Matlab.Execute("bhess = (bhess + bhess')/2;"); /// make it symmetric, because (B * invDC) is not symmetric.
        //        hess_HH = Matlab.GetMatrix("bhess", true);
        //
        //        Matlab.Clear();
        //    }
        //    return hess_HH;
        //}
    }
}
