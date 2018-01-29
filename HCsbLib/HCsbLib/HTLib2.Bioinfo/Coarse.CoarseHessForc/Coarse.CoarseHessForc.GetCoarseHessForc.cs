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
        public static partial class CoarseHessForcIter
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
                    HDebug.Assert(nidx == lstNewIdxRemv.Last().Last()+1);
                    HDebug.Assert(nidx == idxs.Count);
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

                List<IterInfo> iterinfos = null;
                {
                    object[] atoms = reAtoms; // reAtoms.HToType(null as Universe.Atom[]);
                    CGetHessCoarseResiIterImpl info = null;

                    Vector f = F.ToVector();
                    info = GetCoarseHessImpl(atoms, H, f, lstNewIdxRemv, thres_zeroblk, ila, false, options);

                    H = info.H;
                    F = info.F.ToVectors(3);
                    HDebug.Assert(H.ColBlockSize == H.RowBlockSize);
                    HDebug.Assert(H.ColBlockSize == F.Length);
                    iterinfos = info.iterinfos;
                }
                //{
                //    var info = GetHessCoarseResiIterImpl_Matlab(H, lstNewIdxRemv, thres_zeroblk);
                //    H = info.H;
                //}
                GC.Collect(0);

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

                if(rediag)
                    H = H.CorrectHessDiag();
                //System.Console.WriteLine("finish fixing diag");

                return new HessForcInfo
                {
                    hess          = H,
                    forc          = F,
                    mass          = reMass.HSelectCount(numca),
                    atoms         = reAtoms.HSelectCount(numca),
                    coords        = reCoords.HSelectCount(numca),
                    iterinfos     = iterinfos,
                };
            }
        }
        }
    }
}
