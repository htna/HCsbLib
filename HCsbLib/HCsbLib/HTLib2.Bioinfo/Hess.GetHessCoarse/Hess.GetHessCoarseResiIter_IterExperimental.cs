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
            public static CGetHessCoarseResiIterImpl GetHessCoarseResiIterImpl_Matlab_experimental
                ( object[] atoms
                , HessMatrix H
                , List<int>[] lstNewIdxRemv
                , double thres_zeroblk
                , ILinAlg ila
                , bool cloneH
                , string[] options
                )
            {
                ila = null;
                if(cloneH)
                    H = H.CloneHess();

                bool       process_disp_console = true;
                if(options != null && options.Contains("print process"))
                    process_disp_console = true;

                bool parallel = true;

                GC.Collect(0);

                DateTime[] process_time         = new DateTime[6];

                //System.Console.WriteLine("begin coarse-graining");
                List<HessCoarseResiIterInfo> iterinfos = new List<HessCoarseResiIterInfo>();
                for(int iter=lstNewIdxRemv.Length-1; iter>=0; iter--)
                {
                    if(process_disp_console)
                    {
                        process_time[0] = DateTime.UtcNow;
                        System.Console.Write(" - {0:000} : ", iter);
                    }

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

                        HessMatrix    C, D;
                        /// HessMatrix    C = H.SubMatrixByAtoms(false, idxremv, idxkeep, parallel:parallel);
                        /// HessMatrix    D = H.SubMatrixByAtoms(false, idxremv, idxremv, parallel:parallel);
                        {
                            C = H.Zeros(idxremv.Length*3, idxkeep.Length*3);
                            D = H.Zeros(idxremv.Length*3, idxremv.Length*3);
                            int iremv_min = iremv.Min();
                            int iremv_max = iremv.Max();

                            foreach(var bc_br_bval in H.EnumBlocks_dep(idxremv))
                            {
                                int bc   = bc_br_bval.Item1;
                                int br   = bc_br_bval.Item2;
                                var bval = bc_br_bval.Item3;

                                if(bc > iremv_max) continue;
                                if(br > iremv_max) continue;
                                if(br < iremv_min)
                                {
                                    int nc = bc - iremv_min;
                                    int nr = br;
                                    C.SetBlock(nc, nr, bval.CloneT());
                                }
                                else
                                {
                                    int nc = bc - iremv_min;
                                    int nr = br - iremv_min;
                                    D.SetBlock(nc, nr, bval.CloneT());
                                }
                            }
                        }
                        if(process_disp_console)
                        {
                            process_time[1] = process_time[2] = DateTime.UtcNow;
                            System.Console.Write("CD({0:00.00} min), ", (process_time[2]-process_time[0]).TotalMinutes);
                        }

                        // make B,C sparse
                        //int B_cntzero = B.MakeNearZeroBlockAsZero(thres_zeroblk);
                        C_density0 = C.RatioUsedBlocks;
                        /// iterinfo.numSetZeroBlock = C.MakeNearZeroBlockAsZero(thres_zeroblk);
                        {
                            double thres_absmax = thres_zeroblk;

                            List<Tuple<int, int>> lstIdxToMakeZero = new List<Tuple<int, int>>();
                            foreach(var bc_br_bval in C.EnumBlocks_dep())
                            {
                                int bc   = bc_br_bval.Item1;
                                int br   = bc_br_bval.Item2;
                                var bval = bc_br_bval.Item3;
                                double absmax_bval = bval.HAbsMax();
                                if(absmax_bval < thres_absmax)
                                {
                                    lstIdxToMakeZero.Add(new Tuple<int, int>(bc, br));
                                }
                            }
                            foreach(var bc_br in lstIdxToMakeZero)
                            {
                                int cc   = bc_br.Item1;
                                int cr   = bc_br.Item2;
                                var Cval = C.GetBlock(cc, cr);
                                            C.SetBlock(cc, cr, null);
                                var Dval = D.GetBlock(cc, cc);              // nCval = Cval    -Cval
                                            D.SetBlock(cc, cc, Dval+Cval);   // nDval = Dval - (-Cval) = Dval + Cval
                                int bc   = cr;
                                int br   = cc;
                                var Bval = Cval.Tr();
                                var Aval = A.GetBlock(bc, bc);              // nBval = Bval    -Bval
                                            A.SetBlock(bc, bc, Aval+Bval);   // nAval = Aval - (-Bval) = Aval + Bval
                            }
                            iterinfo.numSetZeroBlock = lstIdxToMakeZero.Count;
                        }
                        //int B_nzeros = B.NumUsedBlocks; double B_nzeros_ = Math.Sqrt(B_nzeros);
                        iterinfo.numNonZeroBlock = C.NumUsedBlocks;
                        C_density1 = C.RatioUsedBlocks;

                        HessMatrix B_invD_C = Get_BInvDC(A, C, D, process_disp_console, options, parallel:parallel);
                        if(process_disp_console)
                        {
                            process_time[4] = DateTime.UtcNow;
                            System.Console.Write("B.invD.C({0:00.00} min), ", (process_time[4]-process_time[3]).TotalMinutes);
                        }

                        /// iterinfo.numAddIgnrBlock = A.UpdateAdd(B_invD_C, -1, null, thres_zeroblk/lstNewIdxRemv.Length, parallel:parallel);
                        {
                            HessMatrix _this = A;
                            HessMatrix other = B_invD_C;
                            double thres_NearZeroBlock = thres_zeroblk/lstNewIdxRemv.Length;

                            int count = 0;
                            int count_ignored = 0;
                            foreach(var bc_br_bval in other.EnumBlocks_dep())
                            {
                                count++;
                                int               bc   = bc_br_bval.Item1;
                                int               br   = bc_br_bval.Item2;
                                MatrixByArr other_bmat = bc_br_bval.Item3;
                                if(other_bmat.HAbsMax() <= thres_NearZeroBlock)
                                {
                                    // other_bmat = other_bmat    -other_bmat;
                                    // other_diag = other_diag - (-other_bmat) = other_diag + other_bmat;
                                    //  this_diag =  this_diat - B_invD_C
                                    //            =  this_diat - other_diag
                                    //            =  this_diat - (other_diag + other_bmat)
                                    //            =  this_diat - other_diag  - other_bmat
                                    //            = (this_diat - other_bmat) - other_diag
                                    //            = (this_diat - other_bmat) - (processed later)
                                    //            = (this_diat - other_bmat)
                                    MatrixByArr  this_diag = _this.GetBlock(bc, bc);
                                    MatrixByArr   new_diag = this_diag - other_bmat;
                                    _this.SetBlock(bc, bc, new_diag);
                                    other_bmat = null;
                                    count_ignored++;
                                }
                                if(other_bmat != null)
                                {
                                    MatrixByArr  this_bmat = _this.GetBlock(bc, br);
                                    MatrixByArr   new_bmat = this_bmat - other_bmat;
                                    _this.SetBlock(bc, br, new_bmat);
                                }
                            }
                            iterinfo.numAddIgnrBlock = count_ignored;
                        }
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
        }
    }
}
