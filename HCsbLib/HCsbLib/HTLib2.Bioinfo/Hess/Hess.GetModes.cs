using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        static bool GetModesSelftest_DoTest = true;
        public static bool GetModesSelftest()
        {
            if(GetModesSelftest_DoTest == false)
                return true;
            GetModesSelftest_DoTest = false;

            string pdbpath = @"C:\Users\htna\htnasvn_htna\VisualStudioSolutions\Library2\HTLib2.Bioinfo\Bioinfo.Data\pdb\1MJC.pdb";
            if(HFile.Exists(pdbpath) == false)
                return false;

            Pdb pdb = Pdb.FromFile(pdbpath);
            for(int i=0; i<pdb.atoms.Length; i++)
            {
                HDebug.Assert(pdb.atoms[0].altLoc  == pdb.atoms[i].altLoc );
                HDebug.Assert(pdb.atoms[0].chainID == pdb.atoms[i].chainID);
            }
            List<Vector> coords = pdb.atoms.ListCoord();
            double cutoff = 13;
            Matrix hess = Hess.GetHessAnm(coords.ToArray(), cutoff).ToArray();

            Matrix modes;
            Vector freqs;
            GetModes(hess, out modes, out freqs);

            return true;
        }
        public static void GetModes(Matrix hess, out Matrix modes, out Vector freqs)
        {
            HDebug.Depreciated("use others");
            HDebug.Assert(GetModesSelftest());

            HDebug.Assert(hess.RowSize == hess.ColSize);
            Matrix eigvec;
            Vector eigval;
            NumericSolver.Eig(hess.ToArray(), out eigvec, out eigval);
            int n = hess.RowSize;

            modes = new double[n, n-6];
            freqs = new double[   n-6];
            for(int r=0; r<6; r++)
                HDebug.AssertTolerance(0.00000001, eigval[r]);
            for(int r=6; r<n; r++)
            {
                int rr = r-6;
                freqs[rr] = eigval[r];
                for(int c=0; c<n; c++)
                    modes[c, rr] = eigvec[c, r];
            }
        }
        public static Mode[] GetModesFromHessGnm(Matrix hess, ILinAlg la)
        {
            return GetModesFromHess(hess, la);
        }
        //public static Mode[] GetModes(Matrix hess)
        //{
        //    return GetModesFromHess(hess);
        //}
        public static Mode[] GetModesFromHess(Matrix hess)
        {
            //string cachepath = null;
            HDebug.Depreciated("use Mode[] GetModesFromHess(Matrix hess, ILinAlg la)");

            Vector[] eigvec;
            double[] eigval;
            //if(cachepath != null && HFile.Exists(cachepath))
            //{
            //    HSerialize.Deserialize(cachepath, null, out eigval, out eigvec);
            //}
            //else
            //{
                HDebug.Verify(NumericSolver.Eig(hess.ToArray(), out eigvec, out eigval));
            //    if(cachepath != null)
            //        HSerialize.SerializeDepreciated(cachepath, null, eigval, eigvec);
            //}

            List<Mode> modes;
            {   // sort by eigenvalues
                int[] idx = eigval.HAbs().HIdxSorted();
                modes = new List<Mode>(idx.Length);
                for(int i=0; i<eigval.Length; i++)
                {
                    Mode mode = new Mode
                    {
                        eigval = eigval[idx[i]],
                        eigvec = eigvec[idx[i]]
                    };
                    modes.Add(mode);
                }
            }

            return modes.ToArray();
        }

        public static Mode[] GetModesFromHess(Matrix hess, ILinAlg la)
        {
            List<Mode> modes;
            {
                Matrix V;
                Vector D;
                switch(la)
                {
                    case null:
                        using(new Matlab.NamedLock(""))
                        {
                            Matlab.PutMatrix("H", hess, true);
                            Matlab.Execute("H = (H + H')/2;");
                            Matlab.Execute("[V,D] = eig(H);");
                            Matlab.Execute("D = diag(D);");
                            V = Matlab.GetMatrix("V", true);
                            D = Matlab.GetVector("D", true);
                        }
                        break;
                    default:
                        {
                            var H = la.ToILMat(hess);
                            H = (H + H.Tr)/2;
                            var VD = la.EigSymm(H);
                            V = VD.Item1.ToMatrix();
                            D = VD.Item2;
                            H.Dispose();
                            VD.Item1.Dispose();
                        }
                        break;
                }

                int[] idxs = D.ToArray().HAbs().HIdxSorted();
                modes = new List<Mode>(idxs.Length);
                //foreach(int idx in idxs)
                for(int th=0; th<idxs.Length; th++)
                {
                    int idx = idxs[th];
                    Mode mode = new Mode
                    {
                        th     = (th+1),
                        eigval = D[idx],
                        eigvec = V.GetColVector(idx)
                    };
                    modes.Add(mode);
                }
            }
            System.GC.Collect();
            return modes.ToArray();
        }
    }
}
