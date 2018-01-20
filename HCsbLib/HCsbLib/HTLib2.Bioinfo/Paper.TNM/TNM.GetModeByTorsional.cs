using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;
    using RotableInfo = Universe.RotableInfo;
public static partial class Paper
{
    public partial class TNM
    {
        public static Mode[] GetModeByTorsional(Universe univ
            , Matrix hessian
            , List<Universe.RotableInfo> univ_rotinfos=null
            , Matrix J = null
            , Vector[] coords=null
            , HPack<Matrix> optoutJMJ=null // J' M J
            , HPack<Matrix> optoutJM =null // J' M
            , Func<Matrix, Tuple<Matrix, Vector>> fnEigSymm = null
            , Func<Matrix, Matrix, Matrix, Matrix> fnMul = null
            )
        {
            return GetModeByTorsional(univ
                , new HessMatrixDense { hess=hessian }
                , univ_rotinfos, J
                , coords, optoutJMJ, optoutJM
                , fnEigSymm, fnMul
                );
        }
        public static Mode[] GetModeByTorsional(Universe univ
            , HessMatrix hessian
            , List<Universe.RotableInfo> univ_rotinfos=null
            , Matrix J = null
            , Vector[] coords=null
            , HPack<Matrix> optoutJMJ=null // J' M J
            , HPack<Matrix> optoutJM =null // J' M
            , Func<Matrix, Tuple<Matrix, Vector>> fnEigSymm = null
            , Func<Matrix, Matrix, Matrix, Matrix> fnMul = null
            )
        {
            if(univ_rotinfos == null)
            {
                Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = univ.BuildFlexibilityGraph(null as IList<Bond>);
                if(univ_flexgraph.FindLoops().Count > 0)
                {
                    // loop should not exist in the flexibility-graph; no-global loop in backbone
                    return null;
                }
                univ_rotinfos = univ.GetRotableInfo(univ_flexgraph);
            }
            if(coords == null)
                coords = univ.GetCoords();
            if(J == null)
                J = TNM.GetJ(univ, coords, univ_rotinfos);
            Vector masses = univ.GetMasses();
            Mode[] modes = GetModeByTorsional(hessian, masses, J
                                            , optoutJMJ: optoutJMJ, optoutJM: optoutJM
                                            , fnEigSymm: fnEigSymm, fnMul: fnMul
                                            );
            return modes;
        }
        public static Mode[] GetModeByTorsional(HessMatrix hessian, Vector masses, Matrix J
                                               , HPack<Matrix> optoutJMJ=null // J' M J
                                               , HPack<Matrix> optoutJM =null // J' M
                                               , Func<Matrix, Tuple<Matrix, Vector>> fnEigSymm = null
                                               , Func<Matrix, Matrix, Matrix, Matrix> fnMul = null
                                               )
        {
            
            string opt;
            opt = "eig(JMJ^-1/2 * JHJ * JMJ^-1/2)";
            //opt = "mwhess->tor->eig(H)->cart->mrmode";
            if((fnEigSymm != null) && (fnMul != null))
                opt = "fn-"+opt;
            switch(opt)
            {
                case "mwhess->tor->eig(H)->cart->mrmode":
                    /// http://www.lct.jussieu.fr/manuels/Gaussian03/g_whitepap/vib.htm
                    /// http://www.lct.jussieu.fr/manuels/Gaussian03/g_whitepap/vib/vib.pdf
                    /// does not work properly.
                    HDebug.Assert(false);
                    using(new Matlab.NamedLock("GetModeByTor"))
                    {
                        int n = J.ColSize;
                        int m = J.RowSize;

                        //Matrix M = massmat; // univ.GetMassMatrix(3);
                        Vector[] toreigvecs = new Vector[m];
                        Vector[] tormodes   = new Vector[m];
                        double[] toreigvals = new double[m];
                        Mode[] modes      = new Mode[m];
                        {
                            Matlab.Clear    ("GetModeByTor");
                            Matlab.PutMatrix("GetModeByTor.H", hessian);
                            Matlab.PutMatrix("GetModeByTor.J", J);
                            //Matlab.PutMatrix("GetModeByTor.M", M);
                            Matlab.PutVector("GetModeByTor.m", masses);                         // ex: m = [1,2,...,n]
                            Matlab.Execute  ("GetModeByTor.m3 = kron(GetModeByTor.m,[1;1;1]);");  // ex: m3 = [1,1,1,2,2,2,...,n,n,n]
                            Matlab.Execute  ("GetModeByTor.M = diag(GetModeByTor.m3);");
                            Matlab.Execute  ("GetModeByTor.m = diag(1 ./ sqrt(diag(GetModeByTor.M)));");
                            Matlab.Execute  ("GetModeByTor.mHm = GetModeByTor.m * GetModeByTor.H * GetModeByTor.m;");
                            Matlab.Execute  ("GetModeByTor.JmHmJ = GetModeByTor.J' * GetModeByTor.mHm * GetModeByTor.J;");
                            Matlab.Execute  ("[GetModeByTor.V, GetModeByTor.D] = eig(GetModeByTor.JmHmJ);");
                            Matlab.Execute  ("GetModeByTor.JV = GetModeByTor.m * GetModeByTor.J * GetModeByTor.V;");
                            Matrix V = Matlab.GetMatrix("GetModeByTor.V");
                            Vector D = Matlab.GetVector("diag(GetModeByTor.D)");
                            Matrix JV = Matlab.GetMatrix("GetModeByTor.JV");
                            Matlab.Clear("GetModeByTor");
                            for(int i=0; i<m; i++)
                            {
                                toreigvecs[i] = V.GetColVector(i);
                                toreigvals[i] = D[i];
                                tormodes[i] = JV.GetColVector(i);
                                modes[i] = new Mode();
                                modes[i].eigval = toreigvals[i];
                                modes[i].eigvec = tormodes[i];
                                modes[i].th     = i;
                            }
                        }
                        return modes;
                    }
                case "eig(JMJ^-1/2 * JHJ * JMJ^-1/2)":
                    /// Solve the problem of using eng(H,M).
                    /// 
                    /// eig(H,M) => H.v = M.v.l
                    ///             H.(M^-1/2 . M^1/2).v = (M^1/2 . M^1/2).v.l
                    ///             M^-1/2 . H.(M^-1/2 . M^1/2).v = M^1/2 .v.l
                    ///             (M^-1/2 . H . M^-1/2) . (M^1/2.v) = (M^1/2.v).l
                    ///             (M^-1/2 . H . M^-1/2) . w = w.l
                    ///       where (M^1/2.v) = w
                    ///             v = M^-1/2 . w
                    ///       where M = V . D . V'
                    ///             M^-1/2 = V . (1/sqrt(D)) . V'
                    ///             M^-1/2 . M^-1/2 . M = (V . (1/sqrt(D)) . V') . (V . (1/sqrt(D)) . V') . (V . D . V')
                    ///                                 = V . (1/sqrt(D)) . (1/sqrt(D)) . D . V'
                    ///                                 = V . I . V'
                    ///                                 = I
                    using(new Matlab.NamedLock("GetModeByTor"))
                    {
                        int n = J.ColSize;
                        int m = J.RowSize;
                        
                        //Matrix M = massmat; // univ.GetMassMatrix(3);
                        Vector[] toreigvecs = new Vector[m];
                        Vector[] tormodes   = new Vector[m];
                        double[] toreigvals = new double[m];
                        Mode[] modes      = new Mode[m];
                        {
                            Matlab.Clear("GetModeByTor");
                            Matlab.PutMatrix("GetModeByTor.J", J.ToArray()      , true);
                            //Matlab.PutMatrix("GetModeByTor.M", M      , true);
                            //Matlab.PutMatrix("GetModeByTor.H", hessian, true);
                            Matlab.PutSparseMatrix("GetModeByTor.H", hessian.GetMatrixSparse(), 3, 3);
                            if(HDebug.IsDebuggerAttached && hessian.ColSize < 10000)
                            {
                                Matlab.PutMatrix("GetModeByTor.Htest", hessian.ToArray(), true);
                                double dHessErr = Matlab.GetValue("max(max(abs(GetModeByTor.H - GetModeByTor.Htest)))");
                                Matlab.Execute("clear GetModeByTor.Htest");
                                HDebug.Assert(dHessErr == 0);
                            }
                            Matlab.PutVector("GetModeByTor.m", masses);                         // ex: m = [1,2,...,n]
                            Matlab.Execute("GetModeByTor.m3 = kron(GetModeByTor.m,[1;1;1]);");  // ex: m3 = [1,1,1,2,2,2,...,n,n,n]
                            Matlab.Execute("GetModeByTor.M = diag(GetModeByTor.m3);");

                            Matlab.Execute("GetModeByTor.JMJ = GetModeByTor.J' * GetModeByTor.M * GetModeByTor.J;");
                            Matlab.Execute("GetModeByTor.JHJ = GetModeByTor.J' * GetModeByTor.H * GetModeByTor.J;");
                            Matlab.Execute("[GetModeByTor.V, GetModeByTor.D] = eig(GetModeByTor.JMJ);");
                            Matlab.Execute("GetModeByTor.jmj = GetModeByTor.V * diag(1 ./ sqrt(diag(GetModeByTor.D))) * GetModeByTor.V';"); // jmj = sqrt(JMJ)
                          //Matlab.Execute("max(max(abs(JMJ*jmj*jmj - eye(size(JMJ)))));"); // for checking
                          //Matlab.Execute("max(max(abs(jmj*JMJ*jmj - eye(size(JMJ)))));"); // for checking
                          //Matlab.Execute("max(max(abs(jmj*jmj*JMJ - eye(size(JMJ)))));"); // for checking

                            Matlab.Execute("[GetModeByTor.V, GetModeByTor.D] = eig(GetModeByTor.jmj * GetModeByTor.JHJ * GetModeByTor.jmj);");
                            Matlab.Execute("GetModeByTor.D = diag(GetModeByTor.D);");
                            Matlab.Execute("GetModeByTor.V = GetModeByTor.jmj * GetModeByTor.V;");
                            Matlab.Execute("GetModeByTor.JV = GetModeByTor.J * GetModeByTor.V;");
                            Matrix V  = Matlab.GetMatrix("GetModeByTor.V" , true);
                            Vector D  = Matlab.GetVector("GetModeByTor.D" );
                            Matrix JV = Matlab.GetMatrix("GetModeByTor.JV", true);
                            if(optoutJMJ != null)
                            {
                                optoutJMJ.value = Matlab.GetMatrix("GetModeByTor.JMJ", true);
                            }
                            if(optoutJM != null)
                            {
                                optoutJM.value = Matlab.GetMatrix("GetModeByTor.J' * GetModeByTor.M", true);
                            }
                            Matlab.Clear("GetModeByTor");
                            for(int i=0; i<m; i++)
                            {
                                toreigvecs[i] = V.GetColVector(i);
                                toreigvals[i] = D[i];
                                tormodes[i] = JV.GetColVector(i);
                                modes[i] = new Mode();
                                modes[i].eigval = toreigvals[i];
                                modes[i].eigvec = tormodes[i];
                                modes[i].th     = i;
                            }
                        }
                        return modes;
                    }
                case "fn-eig(JMJ^-1/2 * JHJ * JMJ^-1/2)":
                    /// Solve the problem of using eng(H,M).
                    /// 
                    /// eig(H,M) => H.v = M.v.l
                    ///             H.(M^-1/2 . M^1/2).v = (M^1/2 . M^1/2).v.l
                    ///             M^-1/2 . H.(M^-1/2 . M^1/2).v = M^1/2 .v.l
                    ///             (M^-1/2 . H . M^-1/2) . (M^1/2.v) = (M^1/2.v).l
                    ///             (M^-1/2 . H . M^-1/2) . w = w.l
                    ///       where (M^1/2.v) = w
                    ///             v = M^-1/2 . w
                    ///       where M = V . D . V'
                    ///             M^-1/2 = V . (1/sqrt(D)) . V'
                    ///             M^-1/2 . M^-1/2 . M = (V . (1/sqrt(D)) . V') . (V . (1/sqrt(D)) . V') . (V . D . V')
                    ///                                 = V . (1/sqrt(D)) . (1/sqrt(D)) . D . V'
                    ///                                 = V . I . V'
                    ///                                 = I
                    {
                        int n = J.ColSize;
                        int m = J.RowSize;
                        
                        //Matrix M = massmat; // univ.GetMassMatrix(3);
                        Vector[] toreigvecs = new Vector[m];
                        Vector[] tormodes   = new Vector[m];
                        double[] toreigvals = new double[m];
                        Mode[] modes      = new Mode[m];
                        {
                            Matrix H = hessian; HDebug.Assert(hessian.ColSize == hessian.RowSize);
                            Matrix M = Matrix.Zeros(hessian.ColSize, hessian.RowSize); HDebug.Assert(3*masses.Size == M.ColSize, M.ColSize == M.RowSize);
                            for(int i=0; i<M.ColSize; i++) M[i, i] = masses[i/3];
                            Matrix Jt = J.Tr();

                            Matrix JMJ = fnMul(Jt, M, J);   // JMJ = J' * M * J
                            Matrix JHJ = fnMul(Jt, H, J);   // JHJ = J' * H * J
                            Matrix V; Vector D; {           // [V, D] = eig(JMJ)
                                var VD = fnEigSymm(JMJ);
                                V = VD.Item1;
                                D = VD.Item2;
                            }
                            Matrix jmj; {                   // jmj = sqrt(JMJ)
                                Vector isD = new double[D.Size]; 
                                for(int i=0; i<isD.Size; i++)
                                    isD[i] = 1 / Math.Sqrt(D[i]);
                                jmj = fnMul(V, LinAlg.Diag(isD), V.Tr());
                            }
                            {                               // [V, D] = eig(jmj * JHJ * jmj)
                                Matrix jmj_JHJ_jmj = fnMul(jmj, JHJ, jmj);
                                var VD = fnEigSymm(jmj_JHJ_jmj);
                                V = VD.Item1;
                                D = VD.Item2;
                            }
                            V = fnMul(jmj, V, null);        // V = jmj * V
                            Matrix JV = fnMul(J, V, null);  // JV = J * V
                            if(optoutJMJ != null)
                            {
                                optoutJMJ.value = JMJ;
                            }
                            if(optoutJM != null)
                            {
                                optoutJM.value = fnMul(Jt, M, null); // J' * M
                            }
                            for(int i=0; i<m; i++)
                            {
                                toreigvecs[i] = V.GetColVector(i);
                                toreigvals[i] = D[i];
                                tormodes[i] = JV.GetColVector(i);
                                modes[i] = new Mode();
                                modes[i].eigval = toreigvals[i];
                                modes[i].eigvec = tormodes[i];
                                modes[i].th     = i;
                            }
                        }
                        //if(Debug.IsDebuggerAttached)
                        //{
                        //    Mode[] tmodes = GetModeByTorsional(hessian, masses, J);
                        //    Debug.Assert(modes.Length ==  tmodes.Length);
                        //    for(int i=0; i<modes.Length; i++)
                        //    {
                        //        Debug.AssertTolerance(0.00001, modes[i].eigval - tmodes[i].eigval);
                        //        Debug.AssertTolerance(0.00001, modes[i].eigvec - tmodes[i].eigvec);
                        //    }
                        //}
                        return modes;
                    }
                case "eig(JHJ,JMJ)":
                    /// Generalized eigendecomposition does not guarantee that the eigenvalue be normalized.
                    /// This becomes a problem when a B-factor (determined using eig(H,M)) is compared with another B-factor (determined using eig(M^-1/2 H M^-1/2)).
                    /// This problem is being solved using case "eig(JMJ^-1/2 * JHJ * JMJ^-1/2)"
                    using(new Matlab.NamedLock("GetModeByTor"))
                    {
                        int n = J.ColSize;
                        int m = J.RowSize;
                        
                        //Matrix M = massmat; // univ.GetMassMatrix(3);
                        Matrix JMJ;
                        {
                            Matlab.PutMatrix("GetModeByTor.J", J);
                            //Matlab.PutMatrix("GetModeByTor.M", M);
                            Matlab.PutVector("GetModeByTor.m", masses);                         // ex: m = [1,2,...,n]
                            Matlab.Execute  ("GetModeByTor.m3 = kron(GetModeByTor.m,[1;1;1]);");  // ex: m3 = [1,1,1,2,2,2,...,n,n,n]
                            Matlab.Execute  ("GetModeByTor.M = diag(GetModeByTor.m3);");
                            Matlab.Execute("GetModeByTor.JMJ = GetModeByTor.J' * GetModeByTor.M * GetModeByTor.J;");
                            JMJ = Matlab.GetMatrix("GetModeByTor.JMJ");
                            Matlab.Clear("GetModeByTor");
                        }
                        Matrix JHJ;
                        {
                            Matlab.PutMatrix("GetModeByTor.J", J);
                            Matlab.PutMatrix("GetModeByTor.H", hessian);
                            Matlab.Execute("GetModeByTor.JHJ = GetModeByTor.J' * GetModeByTor.H * GetModeByTor.J;");
                            JHJ = Matlab.GetMatrix("GetModeByTor.JHJ");
                            Matlab.Clear("GetModeByTor");
                        }
                        Vector[] toreigvecs = new Vector[m];
                        Vector[] tormodes   = new Vector[m];
                        double[] toreigvals = new double[m];
                        Mode[]   modes      = new Mode[m];
                        {
                            Matlab.PutMatrix("GetModeByTor.JHJ", JHJ);
                            Matlab.PutMatrix("GetModeByTor.JMJ", JMJ);
                            Matlab.PutMatrix("GetModeByTor.J", J);
                            Matlab.Execute("[GetModeByTor.V, GetModeByTor.D] = eig(GetModeByTor.JHJ, GetModeByTor.JMJ);");
                            Matlab.Execute("GetModeByTor.D = diag(GetModeByTor.D);");
                            Matlab.Execute("GetModeByTor.JV = GetModeByTor.J * GetModeByTor.V;");
                            Matrix V = Matlab.GetMatrix("GetModeByTor.V");
                            Vector D = Matlab.GetVector("GetModeByTor.D");
                            Matrix JV = Matlab.GetMatrix("GetModeByTor.JV");
                            Matlab.Clear("GetModeByTor");
                            for(int i=0; i<m; i++)
                            {
                                toreigvecs[i] = V.GetColVector(i);
                                toreigvals[i] = D[i];
                                tormodes[i] = JV.GetColVector(i);
                                modes[i] = new Mode();
                                modes[i].eigval = toreigvals[i];
                                modes[i].eigvec = tormodes[i];
                                modes[i].th     = i;
                            }
                        }
                        return modes;
                    }
            }
            return null;
        }
        public class TorEigen
        {
            public int    th;
            public double eigval;
            public Vector eigvec;
        }
        public static TorEigen[] GetEigenTorsional(HessMatrix hessian, Vector masses, Matrix J)
        {
            int n = J.ColSize;
            int m = J.RowSize;

            //Matrix M = massmat; // univ.GetMassMatrix(3);
            Matrix JMJ;
            using(new Matlab.NamedLock("GetModeByTor"))
            {
                Matlab.PutMatrix("GetModeByTor.J", J);
                //Matlab.PutMatrix("GetModeByTor.M", M);
                Matlab.PutVector("GetModeByTor.m", masses);                         // ex: m = [1,2,...,n]
                Matlab.Execute  ("GetModeByTor.m3 = kron(GetModeByTor.m,[1;1;1]);");  // ex: m3 = [1,1,1,2,2,2,...,n,n,n]
                Matlab.Execute  ("GetModeByTor.M = diag(GetModeByTor.m3);");
                Matlab.Execute("GetModeByTor.JMJ = GetModeByTor.J' * GetModeByTor.M * GetModeByTor.J;");
                JMJ = Matlab.GetMatrix("GetModeByTor.JMJ");
                Matlab.Clear("GetModeByTor");
            }
            Matrix JHJ;
            using(new Matlab.NamedLock("GetModeByTor"))
            {
                Matlab.PutMatrix("GetModeByTor.J", J);
                Matlab.PutMatrix("GetModeByTor.H", hessian);
                Matlab.Execute("GetModeByTor.JHJ = GetModeByTor.J' * GetModeByTor.H * GetModeByTor.J;");
                JHJ = Matlab.GetMatrix("GetModeByTor.JHJ");
                Matlab.Clear("GetModeByTor");
            }
            TorEigen[] toreigens = new TorEigen[m];
            using(new Matlab.NamedLock("GetModeByTor"))
            {
                Matlab.PutMatrix("GetModeByTor.JHJ", JHJ);
                Matlab.PutMatrix("GetModeByTor.JMJ", JMJ);
                Matlab.PutMatrix("GetModeByTor.J", J);
                Matlab.Execute("[GetModeByTor.V, GetModeByTor.D] = eig(GetModeByTor.JHJ, GetModeByTor.JMJ);");
                Matlab.Execute("GetModeByTor.D = diag(GetModeByTor.D);");
                Matrix V = Matlab.GetMatrix("GetModeByTor.V");
                Vector D = Matlab.GetVector("GetModeByTor.D");
                Matlab.Clear("GetModeByTor");
                for(int i=0; i<m; i++)
                {
                    toreigens[i] = new TorEigen();
                    toreigens[i].th     = i;
                    toreigens[i].eigval = D[i];
                    toreigens[i].eigvec = V.GetColVector(i);
                }
            }
            if(HDebug.IsDebuggerAttached)
            {
                Mode[] modes0 = GetModeByTorsional(hessian, masses, J);
                Mode[] modes1 = GetModeByTorsional(toreigens, J);
                HDebug.Assert(modes0.Length == modes1.Length);
                for(int i=0; i<modes1.Length; i++)
                {
                    HDebug.Assert(modes0[i].th == modes1[i].th);
                    HDebug.AssertTolerance(0.000000001, modes0[i].eigval - modes1[i].eigval);
                    HDebug.AssertTolerance(0.000000001, modes0[i].eigvec - modes1[i].eigvec);
                }
            }
            return toreigens;
        }
        public static Mode[] GetModeByTorsional(TorEigen[] toreigens, Matrix J)
        {
            ///using(new Matlab.NamedLock("GetModeByTor"))
            ///{
            ///    Matlab.PutMatrix("GetModeByTor.JHJ", JHJ);
            ///    Matlab.PutMatrix("GetModeByTor.JMJ", JMJ);
            ///    Matlab.PutMatrix("GetModeByTor.J", J);
            ///    Matlab.Execute("[GetModeByTor.V, GetModeByTor.D] = eig(GetModeByTor.JHJ, GetModeByTor.JMJ);");
            ///    Matlab.Execute("GetModeByTor.D = diag(GetModeByTor.D);");
            ///    Matlab.Execute("GetModeByTor.JV = GetModeByTor.J * GetModeByTor.V;");
            ///    Matrix V = Matlab.GetMatrix("GetModeByTor.V");
            ///    Vector D = Matlab.GetVector("GetModeByTor.D");
            ///    Matrix JV = Matlab.GetMatrix("GetModeByTor.JV");
            ///    Matlab.Clear("GetModeByTor");
            ///    for(int i=0; i<m; i++)
            ///    {
            ///        toreigvecs[i] = V.GetColVector(i);
            ///        toreigvals[i] = D[i];
            ///        tormodes[i] = JV.GetColVector(i);
            ///        modes[i] = new Mode();
            ///        modes[i].eigval = toreigvals[i];
            ///        modes[i].eigvec = tormodes[i];
            ///        modes[i].th     = i;
            ///    }
            ///}
            Mode[] modes = new Mode[toreigens.Length];
            for(int i=0; i<toreigens.Length; i++)
            {
                modes[i] = new Mode();
                modes[i].th     = toreigens[i].th;
                modes[i].eigval = toreigens[i].eigval;
                modes[i].eigvec = LinAlg.MV(J, toreigens[i].eigvec);
            }
            return modes;
        }
    }
}
}
