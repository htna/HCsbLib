using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static partial class HessCoarseResiIter
        {
            public static CGetHessCoarseResiIterImpl GetHessCoarseResiIterImpl_ILinAlg(HessMatrix H, List<int>[] lstNewIdxRemv, double thres_zeroblk, ILinAlg ila, bool cloneH)
            {
                if(cloneH)
                    H = H.CloneHess();

                bool       process_disp_console = true;
                DateTime[] process_time         = new DateTime[7];

                //System.Console.WriteLine("begin coarse-graining");
                List<HessCoarseResiIterInfo> iterinfos = new List<HessCoarseResiIterInfo>();
                for(int iter=lstNewIdxRemv.Length-1; iter>=0; iter--)
                {
                                                                                                                            if(process_disp_console) { process_time[0] = DateTime.UtcNow; System.Console.Write(" - {0:000} : ", iter); }

                    //int[] ikeep = lstNewIdxRemv[iter].Item1;
                    int[] iremv = lstNewIdxRemv[iter].ToArray();
                    HDebug.Assert(H.ColBlockSize == H.RowBlockSize);
                    int   blksize = H.ColBlockSize;
                    //HDebug.Assert(ikeep.Max() < blksize);
                    //HDebug.Assert(iremv.Max() < blksize);
                    //HDebug.Assert(iremv.Max()+1 == blksize);
                    //HDebug.Assert(iremv.Max() - iremv.Min() + 1 == iremv.Length);

                    int[] idxkeep = HEnum.HEnumFromTo(          0, iremv.Min()-1).ToArray();
                    int[] idxremv = HEnum.HEnumFromTo(iremv.Min(), iremv.Max()  ).ToArray();
                    //HDebug.Assert(idxkeep.HUnionWith(idxremv).Length == blksize);

                    HessCoarseResiIterInfo iterinfo = new HessCoarseResiIterInfo();
                    iterinfo.sizeHessBlkMat  = idxremv.Max()+1; // H.ColBlockSize;
                    iterinfo.numAtomsRemoved = idxremv.Length;
                    iterinfo.time0 = DateTime.UtcNow;
                    double C_density0 = double.NaN;
                    double C_density1 = double.NaN;
                    {
                        //HessMatrix    A = H.SubMatrixByAtoms(false, idxkeep, idxkeep);
                        HessMatrix    A = H;
                        //HessMatrix    B = H.SubMatrixByAtoms(false, idxkeep, idxremv);
                        HessMatrix    C = H.SubMatrixByAtoms(false, idxremv, idxkeep);                                      if(process_disp_console) { process_time[1] = DateTime.UtcNow; System.Console.Write("C({0:00.00} min), ", (process_time[1]-process_time[0]).TotalMinutes); }
                        HessMatrix    D = H.SubMatrixByAtoms(false, idxremv, idxremv);                                      if(process_disp_console) { process_time[2] = DateTime.UtcNow; System.Console.Write("D({0:00.00} min), ", (process_time[2]-process_time[1]).TotalMinutes); }
                        HessMatrix invD = new HessMatrixDense { hess=ila.InvSymm(D) };                                      if(process_disp_console) { process_time[3] = DateTime.UtcNow; System.Console.Write("invD({0:00.00} min), ", (process_time[3]-process_time[2]).TotalMinutes); }

                        // make B,C sparse
                        //int B_cntzero = B.MakeNearZeroBlockAsZero(thres_zeroblk);
                        C_density0 = C.RatioUsedBlocks;
                        iterinfo.numSetZeroBlock = C.MakeNearZeroBlockAsZero(thres_zeroblk);                                if(process_disp_console) { process_time[4] = DateTime.UtcNow; System.Console.Write("sparseC({0:00.00} min), ", (process_time[4]-process_time[3]).TotalMinutes); }
                        //int B_nzeros = B.NumUsedBlocks; double B_nzeros_ = Math.Sqrt(B_nzeros);
                        iterinfo.numNonZeroBlock = C.NumUsedBlocks;
                        C_density1 = C.RatioUsedBlocks;

                        HessMatrix B = C.Tr();
                        HessMatrix B_invD_C = HessMatrix.GetMul(ila, B, invD, C); /* B * invD * C;*/                        if(process_disp_console) { process_time[5] = DateTime.UtcNow; System.Console.Write("B.invD.C({0:00.00} min), ", (process_time[5]-process_time[4]).TotalMinutes); }
                        iterinfo.numAddIgnrBlock = A.UpdateAdd(B_invD_C, -1, null, thres_zeroblk/lstNewIdxRemv.Length);     if(process_disp_console) { process_time[6] = DateTime.UtcNow; System.Console.Write("A+BinvDC({0:00.00} min), ", (process_time[6]-process_time[5]).TotalMinutes); }
                        //HessMatrix nH = A - B_invD_C;
                        //nH = ((nH + nH.Tr())/2).ToHessMatrix();
                        H = A;
                    }
                    iterinfo.usedMemoryByte = GC.GetTotalMemory(false);
                    iterinfo.time1 = DateTime.UtcNow;
                    iterinfos.Add(iterinfo);

                    if(process_disp_console)
                        System.Console.WriteLine("summary(makezero {0,5}, nonzero {1,5}, numIgnMul {2,7}, numRemvAtoms {3,3}, {4,5:0.00} sec, {5} mb, {6}x{6}, nzeroBlk/Atom {7:0.00})"
                                            , iterinfo.numSetZeroBlock
                                            , iterinfo.numNonZeroBlock
                                            , iterinfo.numAddIgnrBlock
                                            , iterinfo.numAtomsRemoved
                                            , iterinfo.compSec
                                            , iterinfo.usedMemoryByte/(1024*1024)
                                            , (idxkeep.Length*3)
                                            , ((double)iterinfo.numNonZeroBlock / idxremv.Length)
                                            );

                    GC.Collect(0);
                }

                int numca = H.ColBlockSize - lstNewIdxRemv.HListCount().Sum();
                //System.Console.WriteLine("finish coarse-graining");
                {
                    int[] idxkeep = HEnum.HEnumCount(numca).ToArray();
                    H = H.SubMatrixByAtoms(false, idxkeep, idxkeep, false);
                }
                {
                    H.MakeNearZeroBlockAsZero(thres_zeroblk);
                }
                GC.Collect(0);
                //System.Console.WriteLine("finish resizing");

                return new CGetHessCoarseResiIterImpl
                {
                    iterinfos = iterinfos,
                    H = H,
                };
            }
            public static CGetHessCoarseResiIterImpl GetHessCoarseResiIterImpl_ILinAlg_20150329(HessMatrix H, List<int>[] lstNewIdxRemv, double thres_zeroblk, ILinAlg ila, bool cloneH)
            {
                if(cloneH)
                    H = H.CloneHess();

                //System.Console.WriteLine("begin coarse-graining");
                List<HessCoarseResiIterInfo> iterinfos = new List<HessCoarseResiIterInfo>();
                for(int iter=lstNewIdxRemv.Length-1; iter>=0; iter--)
                {
                    //int[] ikeep = lstNewIdxRemv[iter].Item1;
                    int[] iremv = lstNewIdxRemv[iter].ToArray();
                    HDebug.Assert(H.ColBlockSize == H.RowBlockSize);
                    int   blksize = H.ColBlockSize;
                    //HDebug.Assert(ikeep.Max() < blksize);
                    //HDebug.Assert(iremv.Max() < blksize);
                    //HDebug.Assert(iremv.Max()+1 == blksize);
                    //HDebug.Assert(iremv.Max() - iremv.Min() + 1 == iremv.Length);

                    int[] idxkeep = HEnum.HEnumFromTo(          0, iremv.Min()-1).ToArray();
                    int[] idxremv = HEnum.HEnumFromTo(iremv.Min(), iremv.Max()  ).ToArray();
                    //HDebug.Assert(idxkeep.HUnionWith(idxremv).Length == blksize);

                    HessCoarseResiIterInfo iterinfo = new HessCoarseResiIterInfo();
                    iterinfo.sizeHessBlkMat  = idxremv.Max()+1; // H.ColBlockSize;
                    iterinfo.numAtomsRemoved = idxremv.Length;
                    iterinfo.time0 = DateTime.UtcNow;
                    {
                        //HessMatrix    A = H.SubMatrixByAtoms(false, idxkeep, idxkeep);
                        HessMatrix    A = H;
                        //HessMatrix    B = H.SubMatrixByAtoms(false, idxkeep, idxremv);
                        HessMatrix    C = H.SubMatrixByAtoms(false, idxremv, idxkeep);
                        HessMatrix    D = H.SubMatrixByAtoms(false, idxremv, idxremv);
                        HessMatrix invD = new HessMatrixDense { hess=ila.InvSymm(D) };

                        // make B,C sparse
                        //int B_cntzero = B.MakeNearZeroBlockAsZero(thres_zeroblk);
                        iterinfo.numSetZeroBlock = C.MakeNearZeroBlockAsZero(thres_zeroblk);
                        //int B_nzeros = B.NumUsedBlocks; double B_nzeros_ = Math.Sqrt(B_nzeros);
                        iterinfo.numNonZeroBlock = C.NumUsedBlocks;

                        HessMatrix B = C.Tr();
                        HessMatrix B_invD_C = B * invD * C;
                        iterinfo.numAddIgnrBlock = A.UpdateAdd(B_invD_C, -1, null, thres_zeroblk/lstNewIdxRemv.Length);
                        //HessMatrix nH = A - B_invD_C;
                        //nH = ((nH + nH.Tr())/2).ToHessMatrix();
                        H = A;
                    }
                    iterinfo.usedMemoryByte = GC.GetTotalMemory(false);
                    iterinfo.time1 = DateTime.UtcNow;
                    iterinfos.Add(iterinfo);

                    //System.Console.WriteLine(" - {0:000} : makezero {1,5}, nonzero {2,5}, numIgnMul {3,7}, numRemvAtoms {4,3}, {5,5:0.00} sec, {6} mb, {7}x{7}"
                    //                        , iter
                    //                        , iterinfo.numSetZeroBlock
                    //                        , iterinfo.numNonZeroBlock
                    //                        , iterinfo.numAddIgnrBlock
                    //                        , iterinfo.numAtomsRemoved
                    //                        , iterinfo.compSec
                    //                        , iterinfo.usedMemoryByte/(1024*1024)
                    //                        , (idxkeep.Length*3)
                    //                        );

                    GC.Collect();
                }

                int numca = H.ColBlockSize - lstNewIdxRemv.HListCount().Sum();
                //System.Console.WriteLine("finish coarse-graining");
                {
                    int[] idxkeep = HEnum.HEnumCount(numca).ToArray();
                    H = H.SubMatrixByAtoms(false, idxkeep, idxkeep, false);
                }
                {
                    H.MakeNearZeroBlockAsZero(thres_zeroblk);
                }
                GC.Collect();
                //System.Console.WriteLine("finish resizing");

                return new CGetHessCoarseResiIterImpl
                {
                    iterinfos = iterinfos,
                    H = H,
                };
            }
        }
    }
}
