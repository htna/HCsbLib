using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForc
        {
            public static HessForcInfo GetCoarseHessForc
                ( HessForcInfo hessforc
                , Vector[] coords
                , FuncGetIdxKeepListRemv GetIdxKeepListRemv
                , ILinAlg ila
                , double thres_zeroblk=0.001
                , string[] options=null
                )
            {
                if(options == null)
                    options = new string[0];

                bool rediag=true;

                HessMatrix H = null;
                Vector[]   F = null;
                List<int>[] lstNewIdxRemv = null;
                int numca  = 0;
                double[] reMass   = null;
                object[] reAtoms  = null;
                Vector[] reCoords = null;
                Tuple<int[],int[][]> idxKeepRemv = null;
                //System.Console.WriteLine("begin re-indexing hess");
                {
                    object[] atoms = hessforc.atoms;
                    idxKeepRemv = GetIdxKeepListRemv(atoms, coords);
                    int[]   idxKeep  = idxKeepRemv.Item1;
                    int[][] idxsRemv = idxKeepRemv.Item2;

                    // reshuffle iterations
                    string opt = null;
                    if(options.Contains("OneIter")) opt = "OneIter";
                    if(options.Contains("AllIter")) opt = "AllIter";
                    switch(opt)
                    {
                        case null:
                            break;
                        case "OneIter":
                            {
                                List<int> nIdxsRemv = new List<int>();
                                foreach(var iidxsRemv in idxsRemv)
                                    nIdxsRemv.AddRange(iidxsRemv);
                                idxsRemv = new int[1][];
                                idxsRemv[0] = nIdxsRemv.ToArray();
                            }
                            break;
                        case "AllIter":
                            {
                                List<int> nIdxsRemv = new List<int>();
                                foreach(var iidxsRemv in idxsRemv)
                                    nIdxsRemv.AddRange(iidxsRemv);
                                //nIdxsRemv.Remove(2623);
                                //nIdxsRemv.Add(2623);

                                idxsRemv = new int[nIdxsRemv.Count][];
                                for(int i = 0; i < nIdxsRemv.Count; i++)
                                    idxsRemv[i] = new int[] { nIdxsRemv[i] };
                            }
                            break;
                        default:
                            HDebug.Assert();
                            break;
                    }

                    {
                        List<int> check = new List<int>();
                        check.AddRange(idxKeep);
                        foreach(int[] idxRemv in idxsRemv)
                            check.AddRange(idxRemv);
                        check = check.HToHashSet().ToList();
                        if(check.Count != coords.Length)
                            throw new Exception("the re-index contains the duplicated atoms or the missing atoms");
                    }
                    List<int> idxs = new List<int>();
                    idxs.AddRange(idxKeep);
                    foreach(int[] idxRemv in idxsRemv)
                        idxs.AddRange(idxRemv);
                    HDebug.Assert(idxs.Count == idxs.HToHashSet().Count);

                    H = hessforc.hess.ReshapeByAtom(idxs);
                    numca    = idxKeep.Length;
                    F        = hessforc.forc .ToArray().HSelectByIndex(idxs);
                    reMass   = hessforc.mass .ToArray().HSelectByIndex(idxs);
                    reAtoms  = hessforc.atoms.ToArray().HSelectByIndex(idxs);
                    reCoords = coords                  .HSelectByIndex(idxs);

                    int nidx = idxKeep.Length;
                    lstNewIdxRemv = new List<int>[idxsRemv.Length];
                    for(int i=0; i<idxsRemv.Length; i++)
                    {
                        lstNewIdxRemv[i] = new List<int>();
                        foreach(var idx in idxsRemv[i])
                        {
                            lstNewIdxRemv[i].Add(nidx);
                            nidx++;
                        }
                    }
                    if(HDebug.IsDebuggerAttached)
                    {
                        if(lstNewIdxRemv.Length != 0)
                            HDebug.Assert(nidx == lstNewIdxRemv.Last().Last()+1);
                        HDebug.Assert(nidx == idxs.Count);
                    }
                }
                GC.Collect();
                HDebug.Assert(numca == H.ColBlockSize - lstNewIdxRemv.HListCount().Sum());

                if(HDebug.False)
                    #region
                {
                    int[] idxca  = HEnum.HEnumCount(numca).ToArray();
                    int[] idxoth = HEnum.HEnumFromTo(numca, coords.Length-1).ToArray();

                    HessMatrix A = H.SubMatrixByAtoms(false, idxca , idxca );
                    HessMatrix B = H.SubMatrixByAtoms(false, idxca , idxoth);
                    HessMatrix C = H.SubMatrixByAtoms(false, idxoth, idxca );
                    HessMatrix D = H.SubMatrixByAtoms(false, idxoth, idxoth);
                    Matlab.Clear();
                    Matlab.PutSparseMatrix("A", A.GetMatrixSparse(), 3, 3);
                    Matlab.PutSparseMatrix("B", B.GetMatrixSparse(), 3, 3);
                    Matlab.PutSparseMatrix("C", C.GetMatrixSparse(), 3, 3);
                    Matlab.PutSparseMatrix("D", D.GetMatrixSparse(), 3, 3);
                    Matlab.Clear();
                }
                #endregion

                if(HDebug.IsDebuggerAttached)
                {
                    int nidx = 0;
                    int[] ikeep = idxKeepRemv.Item1;
                    foreach(int idx in ikeep)
                    {
                        bool equal = object.ReferenceEquals(hessforc.atoms[idx], reAtoms[nidx]);
                        if(equal == false)
                            HDebug.Assert(false);
                        HDebug.Assert(equal);
                        nidx++;
                    }
                }

                HessForcInfo hessforcinfo;

                string iteropt = null;
                if(options.Contains("SubSimple"          )) { HDebug.Assert(iteropt == null); iteropt = "SubSimple"; }
                //if(options.Contains("OneIter")) iteropt = "OneIter";
                if(options.Contains("MatlabOneiter"      )) { HDebug.Assert(iteropt == null); iteropt = "MatlabOneiter"; }
                if(options.Contains("MatlabOneiterSparse")) { HDebug.Assert(iteropt == null); iteropt = "MatlabOneiterSparse"; }
                //if(options.Contains("AllIter")) iteropt = "AllIter";

                switch(iteropt)
                {
                    case null:
                        hessforcinfo = GetCoarseHessForcSubIter(reAtoms, H, F, lstNewIdxRemv, thres_zeroblk, ila, false, options);
                        break;
                    case "SubSimple":
                        hessforcinfo = GetCoarseHessForcSubSimple(reAtoms, H, F, lstNewIdxRemv, thres_zeroblk, ila, false, options);
                        break;
                    //case "OneIter":
                    //    {
                    //        int totalcount = lstNewIdxRemv.HListCount().Sum();
                    //        for(int i=1; i<lstNewIdxRemv.Length; i++)
                    //        {
                    //            lstNewIdxRemv[0].AddRange(lstNewIdxRemv[i]);
                    //            lstNewIdxRemv[i] = null;
                    //        }
                    //        lstNewIdxRemv = new List<int>[] { lstNewIdxRemv[0] };
                    //        HDebug.Assert(lstNewIdxRemv.Length == 1);
                    //        HDebug.Assert(totalcount == lstNewIdxRemv[0].Count);
                    //        HDebug.Assert(totalcount == lstNewIdxRemv[0].HToHashSet().Count);
                    //        hessforcinfo = GetCoarseHessForcSubIter(reAtoms, H, F, lstNewIdxRemv, thres_zeroblk, ila, false, options);
                    //    }
                    //    break;
                    //case "AllIter":
                    //    {
                    //        List<int> lstNewIdxRemv_HMerge = lstNewIdxRemv.HMerge();
                    //        int totalcount = lstNewIdxRemv.HListCount().Sum();
                    //        HDebug.Assert(totalcount == lstNewIdxRemv_HMerge.Count);
                    //        lstNewIdxRemv = new List<int>[lstNewIdxRemv_HMerge.Count];
                    //        for(int i=0; i<lstNewIdxRemv_HMerge.Count; i++)
                    //        {
                    //            lstNewIdxRemv[i] = new List<int>();
                    //            lstNewIdxRemv[i].Add(lstNewIdxRemv_HMerge[i]);
                    //        }
                    //        hessforcinfo = GetCoarseHessForcSubIter(reAtoms, H, F, lstNewIdxRemv, thres_zeroblk, ila, false, options);
                    //    }
                    //    break;
                    case "MatlabOneiter":
                        {
                            // collect indexes to remove
                            HashSet<int> idxRemv = new HashSet<int>();
                            foreach(var lst in lstNewIdxRemv)
                                foreach(var idx in lst)
                                    idxRemv.Add(idx);
                            // collect indexes to keep
                            HashSet<int> idxKeep = new HashSet<int>();
                            for(int idx = 0; idx < coords.Length; idx++)
                                if(idxRemv.Contains(idx) == false)
                                    idxKeep.Add(idx);
                            // check
                            HDebug.Assert(idxKeep.Min() == 0);
                            HDebug.Assert(idxKeep.Max() + 1 == idxRemv.Min());
                            HDebug.Assert(idxRemv.Max() == coords.Length - 1);

                            HessMatrix HHH;
                            Vector     FFF;
                            lock(new Matlab.NamedLock(""))
                            {
                                // get A,B,C,D, F,G
                                double assert;
                                Matlab.Execute("clear");
                                Matlab.PutMatrix("HH", H, true);
                                Matlab.PutVector("FF", F.ToVector());
                                Matlab.PutValue("n", idxKeep.Count*3);
                                Matlab.PutValue("N", coords.Length*3);
                                Matlab.Execute("A = HH(1:n  ,1:n  );");
                                Matlab.Execute("B = HH(1:n  ,n+1:N);");
                                Matlab.Execute("C = HH(n+1:N,1:n  );");
                                Matlab.Execute("D = HH(n+1:N,n+1:N);");
                                Matlab.Execute("F = FF(1:n  );");
                                Matlab.Execute("G = FF(n+1:N);");
                                assert = Matlab.GetValue("max(max(abs(B-C')))");    HDebug.Assert(assert == 0);
                                // compute coarse hess/forc
                                Matlab.Execute("HHH = A - B * inv(D) * C;");
                                Matlab.Execute("FFF = F - B * inv(D) * G;");
                                // return
                                HHH = Matlab.GetMatrix("HHH", true);
                                FFF = Matlab.GetVector("FFF");
                                Matlab.Execute("clear");
                            }

                            hessforcinfo = new HessForcInfo
                            {
                                hess = HHH,
                                forc = FFF.ToVectors(3),
                            };
                        }
                        break;
                    case "MatlabOneiterSparse":
                        {
                            // collect indexes to remove
                            HashSet<int> idxRemv = new HashSet<int>();
                            foreach(var lst in lstNewIdxRemv)
                                foreach(var idx in lst)
                                    idxRemv.Add(idx);
                            // collect indexes to keep
                            HashSet<int> idxKeep = new HashSet<int>();
                            for(int idx = 0; idx < coords.Length; idx++)
                                if(idxRemv.Contains(idx) == false)
                                    idxKeep.Add(idx);
                            // check
                            HDebug.Assert(idxKeep.Min() == 0);
                            HDebug.Assert(idxKeep.Max() + 1 == idxRemv.Min());
                            HDebug.Assert(idxRemv.Max() == coords.Length - 1);

                            HessMatrix HHH;
                            Vector     FFF;
                            lock(new Matlab.NamedLock(""))
                            {
                                // get A,B,C,D, F,G
                                Matlab.Execute("clear");
                                Matlab.PutSparseMatrix("HH", H.GetMatrixSparse(), 3, 3, "use file");
                                Matlab.PutVector("FF", F.ToVector());
                                Matlab.PutValue("n", idxKeep.Count*3);
                                Matlab.PutValue("N", coords.Length*3);

                                string pathbase = null;
                                if(options.Contains("MatlabOneiterSparse-SaveMatrix"))
                                {
                                    pathbase = @"K:\Temp\test_sparsehess2\";
                                    Matlab.Execute("cd '"+pathbase+"';");
                                    // save matrix to file
                                    Matlab.Execute("save('HH.mat'    ,'HH','-v7.3');    ");
                                    Matlab.Execute("save('FF.mat'    ,'FF','-v7.3');    ");
                                    Matlab.Execute("save('cntkep.mat','n' ,'-v7.3');    ");
                                    Matlab.Execute("save('cntall.mat','N' ,'-v7.3');    ");
                                    // compute
                                    //Matlab.Execute("load('HH.mat'    ,'HH');            ");
                                    //Matlab.Execute("load('FF.mat'    ,'FF');            ");
                                    //Matlab.Execute("load('cntkep.mat','n' );            ");
                                    //Matlab.Execute("load('cntall.mat','N' );            ");
                                }

                                Matlab.Execute("A = full(HH(1:n  ,1:n  ));                  ");
                                Matlab.Execute("B = full(HH(1:n  ,n+1:N));                  ");
                                Matlab.Execute("C = full(HH(n+1:N,1:n  ));                  ");
                                Matlab.Execute("D = full(HH(n+1:N,n+1:N));                  ");
                                Matlab.Execute("F = FF(1:n  );                              ");
                                Matlab.Execute("G = FF(n+1:N);                              ");
                                Matlab.Execute("assert ( max(max(abs(B-C'))) < 0.00000001 );");
                                Matlab.Execute("BinvD = inv(D);                             ");
                                Matlab.Execute("BinvD = B * BinvD;                          ");
                                Matlab.Execute("HHH = A - BinvD * C;                        ");
                                Matlab.Execute("FFF = F - BinvD * G;                        ");
                                // load matrix

                                if(options.Contains("MatlabOneiterSparse-SaveMatrix"))
                                {
                                    Matlab.Execute("save('HHH.mat'   ,'HHH','-v7.3');   ");
                                    Matlab.Execute("save('FFF.mat'   ,'FFF','-v7.3');   ");
                                  //Matlab.Execute("load('HHH.mat'   ,'HHH');           ");
                                  //Matlab.Execute("load('HHH.mat'   ,'FFF');           ");
                                }

                                HHH = Matlab.GetMatrix("HHH", true);
                                FFF = Matlab.GetVector("FFF");
                                Matlab.Execute("clear");
                            }

                            hessforcinfo = new HessForcInfo
                            {
                                hess = HHH,
                                forc = FFF.ToVectors(3),
                            };
                        }
                        break;
                    default:
                        goto case null;
                }

                HDebug.Assert(hessforcinfo.hess.ColBlockSize == hessforcinfo.hess.RowBlockSize);
                HDebug.Assert(hessforcinfo.hess.ColBlockSize == hessforcinfo.forc.Length);
                //{
                //    var info = GetHessCoarseResiIterImpl_Matlab(H, lstNewIdxRemv, thres_zeroblk);
                //    H = info.H;
                //}
                GC.Collect(0);


                if(rediag)
                    hessforcinfo.hess = hessforcinfo.hess.CorrectHessDiag();
                //System.Console.WriteLine("finish fixing diag");

                hessforcinfo.mass   = reMass  .HSelectCount(numca);
                hessforcinfo.atoms  = reAtoms .HSelectCount(numca);
                hessforcinfo.coords = reCoords.HSelectCount(numca);

                return hessforcinfo;
            }
        }
    }
}
