using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public abstract class HessInfoBase
        {
            public Vector   mass          = null;
            public object[] atoms         = null;
            public Vector[] coords        = null;
            public int?     numZeroEigval = null;

            public Universe.Atom[] atomsAsUniverseAtom { get { return atoms.HToType(null as Universe.Atom[]); } }

            public int[] ListIdxCa()
            {
                List<int> idxca = new List<int>();
                for(int idx=0; idx<atoms.Length; idx++)
                {
                    if(atoms[idx] is Universe.Atom)
                    {
                        var atom = atoms[idx] as Universe.Atom;
                        if(atom.AtomName.Trim().ToUpper() == "CA")
                            idxca.Add(idx);
                        continue;
                    }
                    throw new Exception();
                }
                return idxca.ToArray();
            }
        }
        public partial class HessInfo : HessInfoBase
        {
            public HessMatrix  hess = null;

            public HessInfo GetSubHessInfoByBlockHess(IList<int> idxs, ILinAlg ila)
            {
                // GetSubHessInfoByBlockHess(idxSele, ila, "pinv")
                Vector     idxmass   = mass.ToArray().HSelectByIndex(idxs);
                object[]   idxatoms  = atoms         .HSelectByIndex(idxs);
                Vector[]   idxcoords = coords        .HSelectByIndex(idxs);
                HessMatrix idxhess   = null;
                if(ila != null) idxhess = Hess.GetHessCoarseBlkmat(hess, idxs, ila);
                else            idxhess = Hess.GetHessCoarseBlkmat(hess, idxs);

                return new HessInfo
                {
                    mass   = idxmass,
                    atoms  = idxatoms,
                    coords = idxcoords,
                    hess   = idxhess,
                    numZeroEigval = numZeroEigval,
                };
            }
            public HessInfo GetSubHessInfoByBlockHess(IList<int> idxs, string invopt)
            {
                // GetSubHessInfoByBlockHess(idxSele, ila, "pinv")
                Vector     idxmass   = mass.ToArray().HSelectByIndex(idxs);
                object[]   idxatoms  = atoms         .HSelectByIndex(idxs);
                Vector[]   idxcoords = coords        .HSelectByIndex(idxs);
                HessMatrix idxhess   = Hess.GetHessCoarseBlkmat(hess, idxs, invopt);

                return new HessInfo
                {
                    mass   = idxmass,
                    atoms  = idxatoms,
                    coords = idxcoords,
                    hess   = idxhess,
                    numZeroEigval = numZeroEigval,
                };
            }
            public HessInfo GetSubHessInfo(IList<int> idxSele)
            {
                Dictionary<int, int> whole2sele = idxSele.HToDictionaryAsValueIndex();

                Vector   submass   = (mass   == null) ? null : mass.ToArray().HSelectByIndex(idxSele);
                object[] subatoms  = (atoms  == null) ? null : atoms .HSelectByIndex(idxSele);
                Vector[] subcoords = (coords == null) ? null : coords.HSelectByIndex(idxSele);
                int subsize = idxSele.Count;
                HessMatrix subhess = hess.Zeros(subsize*3, subsize*3);
                foreach(var bc_br_bval in hess.EnumBlocks_dep())
                {
                    int bc   = bc_br_bval.Item1; if(whole2sele.ContainsKey(bc) == false) continue; int nbc = whole2sele[bc];
                    int br   = bc_br_bval.Item2; if(whole2sele.ContainsKey(br) == false) continue; int nbr = whole2sele[br];
                    var bval = bc_br_bval.Item3;
                    subhess.SetBlock(nbc, nbr, bval.CloneT());
                }
                return new HessInfo
                {
                    mass   = submass,
                    atoms  = subatoms,
                    coords = subcoords,
                    hess   = subhess,
                    numZeroEigval = null,
                };
            }

            public HessMatrix GetHessMassWeighted()
            {
                bool delhess = false;
                return GetHessMassWeighted(delhess);
            }
            public HessMatrix GetHessMassWeighted(bool delhess)
            {
                HessMatrix mwhess;
                if(delhess)
                {
                    mwhess = hess;
                    hess = null;
                }
                else
                {
                    mwhess = hess.CloneHess();
                }

                double[] mass3 = new double[mass.Size*3];
                for(int i=0; i<mass3.Length; i++) mass3[i] = mass[i/3];

                Hess.UpdateMassWeightedHess(mwhess, mass3);
                return mwhess;
            }

            public Mode[] GetModesMassWeighted(ILinAlg la)
            {
                bool delhess = false;
                return GetModesMassWeighted(delhess, la);
            }
            public Mode[] GetModesMassWeighted(bool delhess, ILinAlg la)
            {
                Matrix mwhess = GetHessMassWeighted(delhess);
                Mode[] mwmodes = Hess.GetModesFromHess(mwhess, la);
                return mwmodes;
            }

            public Mode[] GetModesMassReduced(bool delhess, ILinAlg la)
            {
                Mode[] modes;
                modes = GetModesMassWeighted(delhess, la);  // mass-weighted modes
                modes.UpdateMassReduced(mass.ToArray());    // mass-reduced modes
                return modes;
            }

            public Mode[] GetModesMassReduced()
            {
                bool delhess = false;
                int? numModeReturn = null; // return all
                return GetModesMassReduced(delhess, numModeReturn);
            }
            public Mode[] GetModesMassReduced(bool delhess)
            {
                int? numModeReturn = null; // return all
                return GetModesMassReduced(delhess, numModeReturn);
            }
            public Mode[] GetModesMassReduced(bool delhess, int? numModeReturn)
            {
                return GetModesMassReduced(delhess, numModeReturn, null);
            }
            public Mode[] GetModesMassReduced(bool delhess, int? numModeReturn, Dictionary<string,object> secs)
            {
                HessMatrix mwhess_ = GetHessMassWeighted(delhess);
                IMatrix<double> mwhess = mwhess_;
                bool bsparse = (mwhess_ is HessMatrixSparse);

                Mode[] modes;
                using(new Matlab.NamedLock(""))
                {
                    string msg = "";
                    {
                        if(bsparse) Matlab.PutSparseMatrix("V", mwhess_.GetMatrixSparse(), 3, 3);
                        else        Matlab.PutMatrix      ("V", ref mwhess, true, true);
                    }
                    msg += Matlab.Execute("tic;");
                    msg += Matlab.Execute("V = (V+V')/2;                   "); // make symmetric
                    {   // eigen-decomposition
                        if(bsparse)
                        {
                            if(numModeReturn != null)
                            {
                                int numeig = numModeReturn.Value;
                                string cmd = "eigs(V,"+numeig+",'sm')";
                                msg += Matlab.Execute("[V,D] = "+cmd+";        ");
                            }
                            else
                            {
                                msg += Matlab.Execute("[V,D] = eig(full(V));         ");
                            }
                        }
                        else
                        {
                            msg += Matlab.Execute("[V,D] = eig(V);         ");
                        }
                    }
                    msg += Matlab.Execute("tm=toc;                         ");
                    if(secs != null)
                    {
                        int numcore = Matlab.Environment.NumCores;
                        double tm = Matlab.GetValue("tm");
                        secs.Clear();
                        secs.Add("num cores", numcore);
                        secs.Add("secs multi-threaded", tm);
                        secs.Add("secs estimated single-threaded", tm*Math.Sqrt(numcore));
                        /// x=[]; for i=1:20; tic; H=rand(100*i); [V,D]=eig(H+H'); xx=toc; x=[x;i,xx]; fprintf('%d, %f\n',i,xx); end; x
                        /// 
                        /// http://www.mathworks.com/help/matlab/ref/matlabwindows.html
                        ///     run matlab in single-thread: matlab -nodesktop -singleCompThread
                        ///                    multi-thread: matlab -nodesktop
                        /// 
                        /// my computer, single thread: cst1={0.0038,0.0106,0.0277,0.0606,0.1062,0.1600,0.2448,0.3483,0.4963,0.6740,0.9399,1.1530,1.4568,1.7902,2.1794,2.6387,3.0510,3.6241,4.2203,4.8914};
                        ///                    2 cores: cst2={0.0045,0.0098,0.0252,0.0435,0.0784,0.1203,0.1734,0.2382,0.3316,0.4381,0.5544,0.6969,1.0170,1.1677,1.4386,1.7165,2.0246,2.4121,2.8124,3.2775};
                        ///                      scale: (cst1.cst2)/(cst1.cst1)              = 0.663824
                        ///                     approx: (cst1.cst2)/(cst1.cst1)*Sqrt[2.2222] = 0.989566
                        /// my computer, single thread: cst1={0.0073,0.0158,0.0287,0.0573,0.0998,0.1580,0.2377,0.3439,0.4811,0.6612,0.8738,1.0974,1.4033,1.7649,2.1764,2.6505,3.1142,3.5791,4.1910,4.8849};
                        ///                    2 cores: cst2={0.0085,0.0114,0.0250,0.0475,0.0719,0.1191,0.1702,0.2395,0.3179,0.4319,0.5638,0.7582,0.9454,1.1526,1.4428,1.7518,2.0291,2.4517,2.8200,3.3090};
                        ///                      scale: (cst1.cst2)/(cst1.cst1)              = 0.671237
                        ///                     approx: (cst1.cst2)/(cst1.cst1)*Sqrt[2.2222] = 1.00062
                        /// ts4-stat   , singhe thread: cst1={0.0048,0.0213,0.0641,0.1111,0.1560,0.2013,0.3307,0.3860,0.4213,0.8433,1.0184,1.3060,1.9358,2.2699,2.1718,3.0149,3.1081,4.3594,5.0356,5.5260};
                        ///                   12 cores: cst2={0.2368,0.0614,0.0235,0.1321,0.0574,0.0829,0.1078,0.1558,0.1949,0.3229,0.4507,0.3883,0.4685,0.6249,0.6835,0.8998,0.9674,1.1851,1.3415,1.6266};
                        ///                      scale: (cst1.cst2)/(cst1.cst1)                 = 0.286778
                        ///                             (cst1.cst2)/(cst1.cst1)*Sqrt[12*1.1111] = 1.04716
                        /// ts4-stat   , singhe thread: cst1={0.0138,0.0215,0.0522,0.0930,0.1783,0.2240,0.2583,0.4054,0.4603,0.9036,0.9239,1.5220,1.9443,2.1042,2.3583,3.0208,3.5507,3.8810,3.6943,6.2085};
                        ///                   12 cores: cst2={0.1648,0.1429,0.1647,0.0358,0.0561,0.0837,0.1101,0.1525,0.2084,0.2680,0.3359,0.4525,0.4775,0.7065,0.6691,0.9564,1.0898,1.2259,1.2926,1.5879};
                        ///                      scale: (cst1.cst2)/(cst1.cst1)          = 0.294706
                        ///                             (cst1.cst2)/(cst1.cst1)*Sqrt[12] = 1.02089
                        /// ts4-stat   , singhe thread: cst1={0.0126,0.0183,0.0476,0.0890,0.1353,0.1821,0.2265,0.3079,0.4551,0.5703,1.0009,1.2175,1.5922,1.8805,2.1991,2.3096,3.7680,3.7538,3.9216,5.2899,5.6737,7.0783,8.8045,9.0091,9.9658,11.6888,12.8311,14.4933,17.2462,17.5660};
                        ///                   12 cores: cst2={0.0690,0.0117,0.0275,0.0523,0.0819,0.1071,0.1684,0.1984,0.1974,0.2659,0.3305,0.4080,0.4951,0.7089,0.9068,0.7936,1.2632,1.0708,1.3187,1.6106,1.7216,2.1114,2.8249,2.7840,2.8259,3.3394,4.3092,4.2708,5.3358,5.7479};
                        ///                      scale: (cst1.cst2)/(cst1.cst1)          = 0.311008
                        ///                             (cst1.cst2)/(cst1.cst1)*Sqrt[12]  = 1.07736
                        /// Therefore, the speedup using multi-core could be sqrt(#core)
                    }
                    msg += Matlab.Execute("D = diag(D);                    ");

                    if(msg.Trim() != "")
                    {
                        System.Console.WriteLine();
                        bool domanual = HConsole.ReadValue<bool>("possibly failed. Will you do ((('V = (V+V')/2;[V,D] = eig(V);D = diag(D);))) manually ?", false, null, false, true);
                        if(domanual)
                        {
                            Matlab.Clear();
                            Matlab.PutMatrix("V", ref mwhess, true, true);
                            System.Console.WriteLine("cleaning working-space and copying V in matlab are done.");
                            System.Console.WriteLine("do V = (V+V')/2; [V,D]=eig(V); D=diag(D);");
                            while(HConsole.ReadValue<bool>("V and D are ready to use in matlab?", false, null, false, true) == false) ;
                            //string path_V = HConsole.ReadValue<string>("path V.mat", @"C:\temp\V.mat", null, false, true);
                            //Matlab.Execute("clear;");
                            //Matlab.PutMatrix("V", ref mwhess, true, true);
                            //Matlab.Execute(string.Format("save('{0}', '-V7.3');", path_V));
                            //while(HConsole.ReadValue<bool>("ready for VD.mat containing V and D?", false, null, false, true) == false) ;
                            //string path_VD = HConsole.ReadValue<string>("path VD.mat", @"C:\temp\VD.mat", null, false, true);
                            //Matlab.Execute(string.Format("load '{0}';", path_V));
                        }
                    }

                    if(numModeReturn != null)
                    {
                        Matlab.PutValue("nmode", numModeReturn.Value);
                        Matlab.Execute("V = V(:,1:nmode);");
                        Matlab.Execute("D = D(1:nmode);");
                    }
                    MatrixByRowCol V = Matlab.GetMatrix("V", MatrixByRowCol.Zeros, true, true);
                    Vector         D = Matlab.GetVector("D");
                    HDebug.Assert(V.RowSize == D.Size);
                    modes = new Mode[D.Size];
                    for(int i=0; i<D.Size; i++)
                    {
                        Vector eigvec = V.GetColVector(i);
                        double eigval = D[i];
                        modes[i] = new Mode
                        {
                            th     = i,
                            eigval = eigval,
                            eigvec = eigvec,
                        };
                    }
                    V = null;
                }
                System.GC.Collect();

                modes.UpdateMassReduced(mass.ToArray());

                return modes;
            }
        }
        //public class SparseHessInfo : HessInfoBase
        //{
        //    public MatrixSparse<MatrixByArr> hess = null;
        //
        //    public MatrixSparse<MatrixByArr> _mwhess = null;
        //    public MatrixSparse<MatrixByArr> GetHessMassWeighted()
        //    {
        //        if(_mwhess == null)
        //            _mwhess = Hess.GetMassWeightedHess(hess, mass);
        //        return _mwhess;
        //    }
        //
        //    public Mode[] GetModesMassWeighted(ILinAlg la)
        //    {
        //        MatrixByArr mwhess  = MatrixByArr.FromMatrixArray(GetHessMassWeighted().ToArray());
        //        Mode[] mwmodes = Hess.GetModesFromHess(mwhess, la);
        //        return mwmodes;
        //    }
        //
        //    public Mode[] _mrmodes = null;
        //    public Mode[] GetModesMassReduced(ILinAlg la)
        //    {
        //        if(_mrmodes == null)
        //        {
        //            Mode[] mwmodes = GetModesMassWeighted(la);
        //            _mrmodes = mwmodes.GetMassReduced(mass.ToArray()).ToArray();
        //        }
        //        return _mrmodes;
        //    }
        //
        //    public Tuple<SparseHessInfo,int[]> GetHessInfoRemoveNull()
        //    {
        //        List<int> select = new List<int>();
        //        for(int i=0; i<coords.Length; i++)
        //            if(coords[i] != null)
        //                select.Add(i);
        //
        //        SparseHessInfo cmpk = GetHessInfoSelect(select);
        //
        //        return new Tuple<SparseHessInfo, int[]>(cmpk, select.ToArray());
        //    }
        //    public SparseHessInfo GetHessInfoSelect(IList<int> select)
        //    {
        //        int size = select.Count;
        //        SparseHessInfo sele = new SparseHessInfo();
        //        sele.mass          = new double[size];
        //        sele.atoms         = new object[size];
        //        sele.coords        = new Vector[size];
        //        sele.numZeroEigval = numZeroEigval;
        //        sele.hess          = new MatrixSparse<MatrixByArr>(size, size, hess.GetDefault);
        //
        //        for(int idxsele=0; idxsele<size; idxsele++)
        //        {
        //            int idxall = select[idxsele];
        //            sele.mass  [idxsele] = mass  [idxall];
        //            sele.atoms [idxsele] = atoms [idxall];
        //            sele.coords[idxsele] = coords[idxall];
        //        }
        //        Dictionary<int, int> all2sele = select.HToDictionaryAsValueIndex();
        //        foreach(var c_r_val in hess.EnumElements())
        //        {
        //            int all_c = c_r_val.Item1; int sele_c = all2sele[all_c];
        //            int all_r = c_r_val.Item2; int sele_r = all2sele[all_r];
        //            var val    = c_r_val.Item3;
        //            sele.hess[sele_c, sele_r] = val;
        //        }
        //        HDebug.Assert(hess.NumElements == sele.hess.NumElements);
        //
        //        return sele;
        //    }
        //}
    }
}
