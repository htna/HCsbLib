using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class ModeStatic
    {
        public static Mode[] GetTransposed(this IList<Mode> modes, Trans3 trans)
        {
            if((trans.ds != 1) || (trans.dt.Dist2 != 0))        // if trans is rot+sca+mov,
                trans = new Trans3(new double[3], 1, trans.dr); // then redetermine trans as rot only

            Mode[] nmodes = new Mode[modes.Count];
            for(int i=0; i<modes.Count; i++)
                nmodes[i] = modes[i].GetTransposed(trans);

            return nmodes;
        }
        public static Mode[] GetSubModesIndexed(this IList<Mode> modes, params int[] atomidxs)
        {
            Mode[] idxmodes = new Mode[modes.Count];
            for(int i=0; i<modes.Count; i++)
            {
                idxmodes[i] = new Mode
                {
                    th     = modes[i].th,
                    eigval = modes[i].eigval,
                    eigvec = modes[i].GetEigvecsOfAtoms().HSelectByIndex(atomidxs).ToVector(),
                };
            }
            return idxmodes;
        }

        public static Mode[] SelectPositiveEigval(this Mode[] modes) { return SelectPositiveEigval(new List<Mode>(modes)).ToArray(); }
        public static List<Mode> SelectPositiveEigval(this List<Mode> modes)
        {
            modes = modes.HClone<Mode>();
            for(int i=0; i<modes.Count; i++)
                if(modes[i].eigval <= 0)
                    modes[i] = null;
            modes = modes.HRemoveAllNull().ToList();
            return modes;
        }
        public static IList<double> ListEigval(this IList<Mode> modes) { return new List<double>(ListEigval(modes.ToArray())); }
        public static double[] ListEigval(this Mode[] modes)
        {
            double[] lstEigval = new double[modes.Length];
            for(int i=0; i<modes.Length; i++)
                lstEigval[i] = modes[i].eigval;
            return lstEigval;
        }
        public static IList<Vector> ListEigvec(this IList<Mode> modes)
        {
            Vector[] lstEigvec = new Vector[modes.Count];
            for(int i=0; i < modes.Count; i++)
                lstEigvec[i] = modes[i].eigvec.Clone();
            return lstEigvec;
        }
        public static double[] ListFreq(this IList<Mode> modes)
        {
            double[] lstFreq = new double[modes.Count];
            for(int i=0; i<modes.Count; i++)
                lstFreq[i] = modes[i].freq;
            return lstFreq;
        }
        public static IList<Vector> ListEigvecNormalized(this IList<Mode> modes)
        {
            Vector[] lstEigvecNormalized = new Vector[modes.Count];
            for(int i=0; i < modes.Count; i++)
                lstEigvecNormalized[i] = modes[i].eigvec.UnitVector();
            return lstEigvecNormalized;
        }
        public static double[,] ListEigvecAsMatrix(this Mode[] modes)
        {
            int eigvecsize = modes.First().eigvec.Size;
            double[,] mat = new double[eigvecsize, modes.Length];
            for(int i=0; i<modes.Length; i++)
            {
                HDebug.Assert(eigvecsize == modes[i].eigvec.Size);
                for(int j=0; j<eigvecsize; j++)
                    mat[j,i] = modes[i].eigvec[j];
            }
            return mat;
        }
        public static Tuple<Mode[], Mode[]> SeparateTolerantsByCountMagnitude(this Mode[] modes, int zerocount)
        {
            double[] eigvals = modes.ListEigval().ToArray();
            double[] abs_eigvals = eigvals.HAbs();
            double[] srt_abs_eigvals = eigvals.HAbs().HSort().ToArray();
            int   [] idxsrt = abs_eigvals.HIdxSorted();

            int[] idxZero    = idxsrt.HSelectFromCount(0        , zerocount             ).ToArray();
            int[] idxNonzero = idxsrt.HSelectFromCount(zerocount, modes.Length-zerocount).ToArray();
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(modes.Length == idxZero.Length+idxNonzero.Length);
                HashSet<int> setIdx = new HashSet<int>();
                foreach(int idx in idxZero   ) setIdx.Add(idx);
                foreach(int idx in idxNonzero) setIdx.Add(idx);
                HDebug.Assert(modes.Length == setIdx.Count);
            }

            Mode[] modesNonzero = modes.HSelectByIndex(idxNonzero).ToArray();
            Mode[] modesZero    = modes.HSelectByIndex(idxZero   ).ToArray();

            return new Tuple<Mode[], Mode[]>(modesNonzero, modesZero);
        }
        public static Tuple<Mode[], Mode[]> SeparateTolerantsByCountSigned(this Mode[] modes, int zerocount)
        {
            double[] eigvals = modes.ListEigval().ToArray();
            double[] srt_eigvals = eigvals.HSort().ToArray();
            int   [] idxsrt      = eigvals.HIdxSorted();

            int[] idxZero    = idxsrt.HSelectFromCount(0        , zerocount             ).ToArray();
            int[] idxNonzero = idxsrt.HSelectFromCount(zerocount, modes.Length-zerocount).ToArray();
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(modes.Length == idxZero.Length+idxNonzero.Length);
                HashSet<int> setIdx = new HashSet<int>();
                foreach(int idx in idxZero   ) setIdx.Add(idx);
                foreach(int idx in idxNonzero) setIdx.Add(idx);
                HDebug.Assert(modes.Length == setIdx.Count);
            }

            Mode[] modesNonzero = modes.HSelectByIndex(idxNonzero).ToArray();
            Mode[] modesZero    = modes.HSelectByIndex(idxZero   ).ToArray();

            return new Tuple<Mode[], Mode[]>(modesNonzero, modesZero);
        }
        public static Tuple<Mode[], Mode[]> SeparateTolerantsByThreshold(this Mode[] modes, double threshold)
        {
            List<Mode> modesNonzero = new List<Mode>();
            List<Mode> modesZero    = new List<Mode>();

            foreach(Mode mode in modes)
            {
                if(Math.Abs(mode.eigval) < threshold)   modesZero.Add(mode);
                else                                    modesNonzero.Add(mode);
            }

            return new Tuple<Mode[], Mode[]>(modesNonzero.ToArray(), modesZero.ToArray());
        }
        public static Tuple<Mode[],Mode[]>         SeparateTolerants(this Mode[] modes
                                                                    , double thres_idxmax_ratio_avgsrt=1000
                                                                    , double thres_idx_idnsrt_nonzero=0.0000000001
                                                                    )
        {
            Tuple<List<Mode>, List<Mode>> nzmodes_zmodes = SeparateTolerants(modes.ToList()
                                                                            , thres_idxmax_ratio_avgsrt:thres_idxmax_ratio_avgsrt
                                                                            , thres_idx_idnsrt_nonzero:thres_idx_idnsrt_nonzero
                                                                            );
            if(nzmodes_zmodes == null)
            {
                HDebug.Assert(false);
                return null;
            }

            Mode[] modesNonzero = nzmodes_zmodes.Item1.ToArray();
            Mode[] modesZero    = nzmodes_zmodes.Item2.ToArray();

            return new Tuple<Mode[], Mode[]>(modesNonzero, modesZero);
        }
        public static IList<Mode> SortByEigval(this IList<Mode> modes)
        {
            double[] eigvals = modes.ToArray().ListEigval().ToArray();
            int   [] idxsrt = eigvals.HIdxSorted();
            IList<Mode> srtmodes = modes.HSelectByIndex(idxsrt);
            return srtmodes;
        }
        public static IList<Mode> SortByEigvalAbs(this IList<Mode> modes)
        {
            double[] eigvals = modes.ToArray().ListEigval().ToArray();
            double[] abs_eigvals = eigvals.HAbs();
            int   [] idxsrt = abs_eigvals.HIdxSorted();
            IList<Mode> srtmodes = modes.HSelectByIndex(idxsrt);
            return srtmodes;
        }
        public static Tuple<List<Mode>,List<Mode>> SeparateTolerants(this List<Mode> modes
                                                                    , double thres_idxmax_ratio_avgsrt=1000
                                                                    , double thres_idx_idnsrt_nonzero=0.0000000001
                                                                    )
        {
            if(HDebug.Selftest())
            {
                IList<Mode> tmodes;
                Tuple<List<Mode>,List<Mode>> tmdNonzeroZero;

                tmodes = new Mode[] { new Mode { eigval = 0 }, new Mode { eigval = 0 }, new Mode { eigval = 0 }, new Mode { eigval = 0.00001 } };
                tmdNonzeroZero = SeparateTolerants(tmodes.ToList());
                HDebug.Assert(tmdNonzeroZero.Item1.Count == 1, tmdNonzeroZero.Item2.Count == 3);

                tmodes = new Mode[] { new Mode { eigval = -0.2 }, new Mode { eigval = 0.1 }, new Mode { eigval = 0 }, new Mode { eigval = -10 } };
                tmdNonzeroZero = SeparateTolerants(tmodes.ToList());
                HDebug.Assert(tmdNonzeroZero.Item1.Count == 3, tmdNonzeroZero.Item2.Count == 1);

                tmodes = new Mode[] { new Mode { eigval = 0.000000000002 }, new Mode { eigval = 0.1 }, new Mode { eigval = -0.000000001 }, new Mode { eigval = -10 } };
                tmdNonzeroZero = SeparateTolerants(tmodes.ToList());
                HDebug.Assert(tmdNonzeroZero.Item1.Count == 2);
                HDebug.Assert(tmdNonzeroZero.Item2.Count == 2);
            }

            modes = new List<Mode>(modes);
            Func<double, double> sqrt = Math.Sqrt;
            double[] eigvals = modes.ListEigval().ToArray();
            double[] abs_eigvals = eigvals.HAbs();
            double[] srt_abs_eigvals = eigvals.HAbs().HSort().ToArray();
            int   [] idxsrt = abs_eigvals.HIdxSorted();

            double[] sumsrt_1_n = new double[abs_eigvals.Length]; sumsrt_1_n[0] = abs_eigvals[idxsrt[0]];
            double[] avgsrt_1_n = new double[abs_eigvals.Length]; avgsrt_1_n[0] = abs_eigvals[idxsrt[0]];
            double[] ratio_avgsrt_1_n = new double[abs_eigvals.Length]; ratio_avgsrt_1_n[0] = 0;
            List<Tuple<int,double>> idxmax_ratio_avgsrt = new List<Tuple<int, double>>();
            for(int i=1; i<abs_eigvals.Length; i++)
            {
                sumsrt_1_n[i] += sumsrt_1_n[i-1] + abs_eigvals[idxsrt[i]];
                avgsrt_1_n[i]  = sumsrt_1_n[i] / i;
                ratio_avgsrt_1_n[i] = avgsrt_1_n[i] / (avgsrt_1_n[i-1]==0 ? double.Epsilon : avgsrt_1_n[i-1]);
                if(ratio_avgsrt_1_n[i] > thres_idxmax_ratio_avgsrt)
                    idxmax_ratio_avgsrt.Add(new Tuple<int, double>(i, ratio_avgsrt_1_n[i]));
                //if(ratio_avgsrt_1_n[i] > ratio_avgsrt_1_n[idxmax_ratio_avgsrt])
                //{
                //    if(double.IsInfinity(ratio_avgsrt_1_n[i]) == false)
                //        idxmax_ratio_avgsrt = i;
                //    else
                //    {
                //        bool bAllZero = true;
                //        for(int j=0; j<i; j++)
                //            if(ratio_avgsrt_1_n[j]!=0)
                //                bAllZero = false;
                //        if(bAllZero)
                //            idxmax_ratio_avgsrt = i;
                //    }
                //}
            }

            int? idx_idnsrt_nonzero = null;
            foreach(var idxmax in idxmax_ratio_avgsrt)
            {
                if(idx_idnsrt_nonzero != null)
                    continue;
                double abs_eigvals_i = abs_eigvals[idxmax.Item1];
                if(abs_eigvals_i > thres_idx_idnsrt_nonzero)
                    idx_idnsrt_nonzero = idxmax.Item1;
            }

            if(idx_idnsrt_nonzero == null)
            {
                HDebug.Assert(false);
                System.Console.Error.WriteLine("cannot find separation index regarding zero modes");
                return null;
            }

            //int idx_idnsrt_nonzero = idxmax_ratio_avgsrt;
            //int idx_idnsrt_nonzero = 0;
            List<Mode> modesNonzero = new List<Mode>();
            List<Mode> modesZero    = new List<Mode>();
            for(int i=0; i<modes.Count; i++)
            {
                if(i <  idx_idnsrt_nonzero.Value) { modesZero   .Add(modes[idxsrt[i]]); modes[idxsrt[i]]=null; }
                if(i >= idx_idnsrt_nonzero.Value) { modesNonzero.Add(modes[idxsrt[i]]); modes[idxsrt[i]]=null; }
            }
            modes = modes.HRemoveAllNull(false).ToList();
            HDebug.Assert(modes.Count == 0);

            return new Tuple<List<Mode>, List<Mode>>(modesNonzero, modesZero);
        }
        public static Mode[] SelectExceptSmallSix(this Mode[] modes) { return (new List<Mode>(modes)).SelectExceptSmallSix().ToArray(); }
        public static List<Mode> SelectExceptSmallSix(this List<Mode> modes)
        {
            HDebug.ToDo("use SeparateTolerants(...) instead");
            /// example:
            ///     Tuple<Mode[], Mode[]> nzmodes_zeromodes = modes.SeparateTolerants();
            ///     Mode[] modesNonzero = nzmodes_zeromodes.Item1;
            ///     Mode[] modesZero    = nzmodes_zeromodes.Item2;
            ///     {
            ///         Debug.Assert(modesZero.Length == 6);
            ///         if(modesNonzero.ListEigval().SelectLess(0).Length > 0)
            ///         {
            ///             message = "existance of the negative eigenvalues for "+hessopt;
            ///         }
            ///     }
            double[] eigvals = modes.ListEigval().ToArray();
            double[] abs_eigvals = eigvals.HAbs();
            int[] idx_sort_abs_eigvals = abs_eigvals.HIdxSorted();
            int[] idx_zeroeigval = idx_sort_abs_eigvals.HSelectCount(6).ToArray();
            modes = modes.HRemoveIndexe(idx_zeroeigval);
            return modes;
        }
        public static IList<Mode> GetMassReducedGnm(this IList<Mode> modes, IList<double> masses)
        {
            List<Mode> newmodes = new List<Mode>();
            for(int i=0; i<modes.Count; i++)
                newmodes.Add(modes[i].GetMassReducedGnm(masses));
            return newmodes;
        }
        public static IList<Mode> GetMassReduced(this IList<Mode> modes, IList<double> masses, HOptions options=null)
        {
            Mode[] mrmodes = modes.ToArray().HClone();
            mrmodes.UpdateMassReduced(masses, options);
            return mrmodes;
        }
        public static void UpdateMassReduced(this IList<Mode> modes, IList<double> masses, HOptions options=null)
        {
            if(options == null)
                options = "";
            if(options.Contains("parallel"))
            {
                throw new NotImplementedException("check");
                int[] iter = new int[1] { 0 };
                System.Threading.Tasks.Parallel.For(0, modes.Count, delegate(int i)
                {
                    Mode modei = modes[i];
                    modes[i] = null;
                    modes[i] = modei.GetMassReduced(masses);
                    lock(iter)
                    {
                        iter[0] ++;
                        if(iter[0] % 1000 == 0)
                            System.GC.Collect(0);
                    }
                });
                System.GC.Collect();
            }
            else
            {
                for(int i=0; i<modes.Count; i++)
                {
                    Mode modei = modes[i];
                    modes[i] = null;
                    modes[i] = modei.GetMassReduced(masses);
                    if(i % 1000 == 0)
                        System.GC.Collect(0);
                }
                System.GC.Collect();
            }
        }
        public static Vector GetDCoords(this IList<Mode> modes, IList<double> contrib)
        {
            HDebug.Assert(modes.Count == contrib.Count);
            Vector motion = new double[modes[0].eigvec.Size];
            for(int im=0; im<modes.Count; im++)
                motion += modes[im].eigvec * contrib[im];
            return motion;
        }
        public static List<double> GetBFactor(this IList<Mode> modes)
        {
            int size = modes[0].size;
            List<double> bfactor = new List<double>(new double[size]);
            foreach(Mode mode in modes)
            {
                if(mode.eigval == 0)
                    continue;
                mode.GetBFactor(bfactor);
            }
            return bfactor;
        }
        public static List<double> GetBFactor(this IList<Mode> modes, double T)
        {
            int size = modes[0].size;
            List<double> bfactor = new List<double>(new double[size]);
            foreach(Mode mode in modes)
            {
                if(mode.eigval == 0)
                    continue;
                mode.GetBFactor(bfactor, T);
            }
            return bfactor;
        }
        public static List<double> GetBFactorGnm(this IList<Mode> modes)
        {
            int size = modes[0].eigvec.Size;
            List<double> bfactor = new List<double>(new double[size]);
            foreach(Mode mode in modes)
                mode.GetBFactorGnm(bfactor);
            return bfactor;
        }
        public static List<MatrixByArr> GetAnisou(this IList<Mode> modes)
        {
            int size = modes[0].size;
            List<MatrixByArr> anisou = new List<MatrixByArr>(new MatrixByArr[size]);
            for(int i=0; i<size; i++)
                anisou[i] = new double[3, 3];
            foreach(Mode mode in modes)
                mode.GetAnisou(anisou);
            return anisou;
        }
        public static MatrixByArr GetHessian(this IEnumerable<Mode> modes)
        {
            int size = modes.First().eigvec.Size;
            MatrixByArr hess = new double[size, size];
            foreach(Mode mode in modes)
                mode.GetHessian(hess);
            return hess;
        }
        public static MatrixByArr GetHessian(this IList<Mode> modes, ILinAlg la)
        {
            Vector[] eigvecs = modes.ListEigvec().ToArray();
            Vector   eigvals = modes.ListEigval().ToArray();
            MatrixByArr   hess;
            {
                var V = la.ToILMat(eigvecs.ToMatrix());
                var D = la.ToILMat(eigvals).Diag();
                var H = la.Mul(V, D, V.Tr);
                hess = H.ToArray();
                V.Dispose();
                D.Dispose();
                H.Dispose();
            }
            return hess;
        }
        public static MatrixByArr GetHessian(this IList<Mode> modes, IList<double> masses, ILinAlg la)
        {
            /// mode.eigval =             mweigval
            /// mode.eigvec = mass^-0.5 * mweigvec
            /// 
            /// W = [mode(1).eigvec, mode(2).eigvec, ... ]
            /// M = masses
            /// D = diag([mode(1).eigval, mode(2).eigval, ...])
            /// 
            /// mwH = M^-0.5 H M^-0.5
            /// [V,D] = eig(mwH)
            /// mode.eigval == D
            /// mode.eigvec == M^-0.5 V = W
            /// 
            /// W D W' = M^-0.5       V D V       M^-0.5
            ///        = M^-0.5        mwH        M^-0.5
            ///        = M^-0.5 (M^-0.5 H M^-0.5) M^-0.5
            ///        = M^-1           H         M^-1
            /// H = M (M^-1 H M^-1) M
            ///   = M (W    D   W') M
            ///   = (M W)   D   (M W)'
            HDebug.Assert(modes.CheckModeMassReduced(masses.ToArray(), la, 0.000001));

            Vector[] mws = modes.ListEigvec().ToArray();
            for(int iv=0; iv<mws.Length; iv++)
            {
                for(int i=0; i<mws[iv].Size; i++)
                    mws[iv][i] = mws[iv][i] * masses[i/3];
            }
            Vector ds = modes.ListEigval().ToArray();
            MatrixByArr hess;
            {
                var MW = la.ToILMat(mws.ToMatrix());
                var D  = la.ToILMat(ds).Diag();
                var H = la.Mul(MW, D, MW.Tr);
                hess = H.ToArray();
                MW.Dispose();
                D.Dispose();
                H.Dispose();
            }
            return hess;
        }
        public static Mode.FloatMode[] ToFloatModes(this Mode[] modes)
        {
            Mode.FloatMode[] fmodes = new Mode.FloatMode[modes.Length];
            for(int i=0; i<modes.Length; i++)
                fmodes[i] = modes[i].ToFloatMode();
            return fmodes;
        }
        public static Mode[] FromFloatModes(Mode.FloatMode[] fmodes)
        {
            Mode[] modes = new Mode[fmodes.Length];
            for(int i=0; i<modes.Length; i++)
                modes[i] = Mode.FromFloatMode(fmodes[i]);
            return modes;
        }
        public static Mode[] ToModes(this Mode.FloatMode[] fmodes)
        {
            Mode[] modes = new Mode[fmodes.Length];
            for(int i=0; i<modes.Length; i++)
                modes[i] = Mode.FromFloatMode(fmodes[i]);
            return modes;
        }
        public static Matrix ToModeMatrix(this IList<Mode> modes)
        {
            int vecsize = modes.First().eigvec.Size;
            Matrix mat = Matrix.Zeros(vecsize, modes.Count);
            for(int c=0; c<vecsize; c++)
                for(int r=0; r<modes.Count; r++)
                    mat[c, r] = modes[r].eigvec[c];
            return mat;
        }
        public static Mode[] GetNormalized(this IList<Mode> modes)
        {
            Mode[] nmodes = new Mode[modes.Count];
            for(int im=0; im<modes.Count; im++)
                nmodes[im] = modes[im].GetNormalized();
            return nmodes;
        }
        public static Mode[] ToModesUnnormalized(this IList<Mode> modes, double[] masses)
        {
            Mode[] unmodes = new Mode[modes.Count];
            for(int im=0; im<modes.Count; im++)
                unmodes[im] = modes[im].ToModesUnnormalized(masses);
            return unmodes;
        }
        public static Mode[] ToModeEigvecResized(this IList<Mode> modes, int numAtom, IList<int> idxAtom)
        {
            // reindex eigvecs by idxAtom
            // (idxAtom[i == -1) will be ignored
            // new eigvec size is (numAtom * 3)
            Mode[] modeIdxed = new Mode[modes.Count];
            for(int im=0; im<modes.Count; im++)
            {
                Vector eigvec      = modes[im].eigvec;
                Vector eigvecIdxed = new double[numAtom*3];
                eigvecIdxed.SetValue(double.NaN);
                for(int i=0; i<idxAtom.Count; i++)
                {
                    if(idxAtom[i] == -1)
                        continue;
                    eigvecIdxed[idxAtom[i]*3+0] = eigvec[i*3+0];
                    eigvecIdxed[idxAtom[i]*3+1] = eigvec[i*3+1];
                    eigvecIdxed[idxAtom[i]*3+2] = eigvec[i*3+2];
                }
                modeIdxed[im] = new Mode
                {
                    th     = modes[im].th,
                    eigval = modes[im].eigval,
                    eigvec = eigvecIdxed,
                };
            }
            return modeIdxed;
        }
        public static Mode[] GetModesBySelectingAtomIndex(this IList<Mode> modes, params int[] atomidxs)
        {
            Mode[] nmodes = new Mode[modes.Count];
            for(int i=0; i<modes.Count; i++)
                nmodes[i] = new Mode
                {
                    th     = modes[i].th,
                    eigval = modes[i].eigval,
                    eigvec = modes[i].GetEigvecsOfAtoms().HSelectByIndex(atomidxs).ToVector()
                };
            return nmodes;
        }
    }
}
