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
            public static CGetHessCoarseResiIterImpl GetCoarseHessImpl
                ( object[] atoms
                , HessMatrix H
                , Vector     F
                , List<int>[] lstNewIdxRemv
                , double thres_zeroblk
                , ILinAlg ila
                , bool cloneH
                , string[] options // { "pinv(D)" }
                )
            {
                ila = null;
                if(cloneH)
                    H = H.CloneHess();

                bool       process_disp_console = true;
                if(options != null && options.Contains("print process"))
                    process_disp_console = true;

                bool parallel = true;

                /// keep only lower triangle of H (lower block triangles)
                {
                    HashSet<Tuple<int, int, MatrixByArr>> lstUppTrig = new HashSet<Tuple<int, int, MatrixByArr>>();
                    foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in H.EnumBlocks())
                    {
                        int bc = bc_br_bval.Item1;
                        int br = bc_br_bval.Item2;
                        if(bc < br)
                        {
                            lstUppTrig.Add(bc_br_bval.ToTuple());
                        }
                    }
                    foreach(Tuple<int, int, MatrixByArr> bc_br_bval in lstUppTrig)
                    {
                        int bc = bc_br_bval.Item1;
                        int br = bc_br_bval.Item2;
                        HDebug.Assert(bc < br);
                        H.SetBlock(bc, br, null);
                    }
                }
                GC.Collect();

                List<DateTime> process_time = new List<DateTime>();

                //System.Console.WriteLine("begin coarse-graining");
                List<IterInfo> iterinfos = new List<IterInfo>();
                for(int iter=lstNewIdxRemv.Length-1; iter>=0; iter--)
                {
                    process_time.Clear();
                    if(process_disp_console)
                    {
                        process_time.Add(DateTime.UtcNow);
                        System.Console.Write(" - {0:000} : ", iter);
                    }

                    //int[] ikeep = lstNewIdxRemv[iter].Item1;
                    int[] iremv = lstNewIdxRemv[iter].ToArray();
                    int iremv_min = iremv.Min();
                    int iremv_max = iremv.Max();

                    HDebug.Assert(H.ColBlockSize == H.RowBlockSize);
                    int   blksize = H.ColBlockSize;
                    //HDebug.Assert(ikeep.Max() < blksize);
                    //HDebug.Assert(iremv.Max() < blksize);
                    //HDebug.Assert(iremv.Max()+1 == blksize);
                    //HDebug.Assert(iremv.Max() - iremv.Min() + 1 == iremv.Length);

                    int[] idxkeep = HEnum.HEnumFromTo(          0, iremv_min-1).ToArray();
                    int[] idxremv = HEnum.HEnumFromTo(iremv_min, iremv_max  ).ToArray();
                    //HDebug.Assert(idxkeep.HUnionWith(idxremv).Length == blksize);

                    IterInfo iterinfo = new IterInfo();
                    iterinfo.sizeHessBlkMat  = idxremv.Max()+1; // H.ColBlockSize;
                    iterinfo.numAtomsRemoved = idxremv.Length;
                    iterinfo.time0 = DateTime.UtcNow;

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // make C sparse
                    double C_density0;
                    double C_density1;
                    {
                        double thres_absmax = thres_zeroblk;

                        C_density0 = 0;
                        List<Tuple<int, int>> lstIdxToMakeZero = new List<Tuple<int, int>>();
                        foreach(var bc_br_bval in H.EnumBlocksInCols(idxremv))
                        {
                            int bc   = bc_br_bval.Item1;
                            int br   = bc_br_bval.Item2;
                            var bval = bc_br_bval.Item3;
                            if(br >= iremv_min)
                                // bc_br is in D, not in C
                                continue;

                            C_density0++;
                            double absmax_bval = bval.HAbsMax();
                            if(absmax_bval < thres_absmax)
                            {
                                lstIdxToMakeZero.Add(new Tuple<int, int>(bc, br));
                            }
                        }
                        C_density1 = C_density0 - lstIdxToMakeZero.Count;
                        foreach(var bc_br in lstIdxToMakeZero)
                        {
                            int bc   = bc_br.Item1;
                            int br   = bc_br.Item2;
                            HDebug.Assert(bc > br);
                            var Cval = H.GetBlock(bc, br);
                            var Dval = H.GetBlock(bc, bc);
                            var Aval = H.GetBlock(br, br);
                            var Bval = Cval.Tr();
                            H.SetBlock(bc, br, null);           // nCval = Cval    -Cval
                            H.SetBlock(bc, bc, Dval + Cval);    // nDval = Dval - (-Cval) = Dval + Cval
                                                                // nBval = Bval    -Bval
                            H.SetBlock(br, br, Aval + Bval);    // nAval = Aval - (-Bval) = Aval + Bval
                        }
                        iterinfo.numSetZeroBlock = lstIdxToMakeZero.Count;
                        iterinfo.numNonZeroBlock = (int)C_density1;
                        C_density0 /= (idxkeep.Length * idxremv.Length);
                        C_density1 /= (idxkeep.Length * idxremv.Length);
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // get A, B, C, D
                    HessMatrix    A = H;    // HessMatrix    A = H.SubMatrixByAtoms(false, idxkeep, idxkeep);
                                            // HessMatrix    B = H.SubMatrixByAtoms(false, idxkeep, idxremv);
                    HessMatrix    C;        // HessMatrix    C = H.SubMatrixByAtoms(false, idxremv, idxkeep, parallel:parallel);
                    HessMatrix    D;        // HessMatrix    D = H.SubMatrixByAtoms(false, idxremv, idxremv, parallel:parallel);
                    {
                        C = H.Zeros(idxremv.Length*3, idxkeep.Length*3);
                        D = H.Zeros(idxremv.Length*3, idxremv.Length*3);

                        //List<Tuple<int, int, MatrixByArr>> lst_bc_br_bval = H.EnumBlocksInCols(idxremv).ToList();
                        //foreach(var bc_br_bval in lst_bc_br_bval)
                        foreach(var bc_br_bval in H.EnumBlocksInCols(idxremv))
                        {
                            int bc   = bc_br_bval.Item1;
                            int br   = bc_br_bval.Item2;
                            var bval = bc_br_bval.Item3;

                            H.SetBlock(bc, br, null);
                            if(bc > iremv_max) { HDebug.Assert(false); continue; }
                            if(br > iremv_max) { HDebug.Assert(false); continue; }
                            if(br < iremv_min)
                            {
                                int nc = bc - iremv_min;
                                int nr = br;
                                HDebug.Assert(C.HasBlock(nc, nr) == false);
                                C.SetBlock(nc, nr, bval.CloneT());
                            }
                            else
                            {
                                int nc = bc - iremv_min;
                                int nr = br - iremv_min;
                                HDebug.Assert(D.HasBlock(nc, nr) == false);
                                D.SetBlock(nc, nr, bval);
                                if(nc != nr)
                                {
                                    HDebug.Assert(D.HasBlock(nr, nc) == false);
                                    D.SetBlock(nr, nc, bval.Tr());
                                }
                            }
                        }
                        HDebug.Assert(H.EnumBlocksInCols(idxremv).Count() == 0);
                    }
                    if(process_disp_console)
                    {
                        process_time.Add(DateTime.UtcNow);
                        int ptc = process_time.Count;
                        System.Console.Write("CD({0:00.00} min), ", (process_time[ptc - 1] - process_time[ptc - 2]).TotalMinutes);
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Get B.inv(D).C
                    HessMatrix B_invD_C;
                    {
                        {
                            var BInvDC_BInvDG = Get_BInvDC_BInvDG_WithSqueeze(A, C, D, process_disp_console
                                , options
                                , thld_BinvDC: thres_zeroblk/lstNewIdxRemv.Length
                                , parallel:parallel
                                );
                            B_invD_C = BInvDC_BInvDG.Item1;
                        }
                        if(process_disp_console)
                        {
                            process_time.Add(DateTime.UtcNow);
                            int ptc = process_time.Count;
                            System.Console.Write("B.invD.C({0:00.00} min), ", (process_time[ptc-1] - process_time[ptc-2]).TotalMinutes);
                        }
                        GC.Collect(0);
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Get A - B.inv(D).C
                    /// iterinfo.numAddIgnrBlock = A.UpdateAdd(B_invD_C, -1, null, thres_zeroblk/lstNewIdxRemv.Length, parallel:parallel);
                    {
                        HessMatrix __this = A;
                        HessMatrix other = B_invD_C;
                        double _thres_NearZeroBlock = thres_zeroblk/lstNewIdxRemv.Length;

                        int[] _count         = new int[1];
                        int[] _count_ignored = new int[1];

                        //foreach(var bc_br_bval in other.EnumBlocks())
                        Action<ValueTuple<int, int, MatrixByArr>> func = delegate(ValueTuple<int, int, MatrixByArr> bc_br_bval)
                        {
                            _count[0]++;
                            int               bc   = bc_br_bval.Item1;
                            int               br   = bc_br_bval.Item2;
                            MatrixByArr other_bmat = bc_br_bval.Item3;
                            if(bc < br)
                                return; // continue;
                            if(other_bmat.HAbsMax() <= _thres_NearZeroBlock)
                            {
                                /// other_bmat = other_bmat    -other_bmat;
                                /// other_diag = other_diag - (-other_bmat) = other_diag + other_bmat;
                                ///  this_diag =  this_diat - B_invD_C
                                ///            =  this_diat - other_diag
                                ///            =  this_diat - (other_diag + other_bmat)
                                ///            =  this_diat - other_diag  - other_bmat
                                ///            = (this_diat - other_bmat) - other_diag
                                ///            = (this_diat - other_bmat) - (processed later)
                                ///            = (this_diat - other_bmat)
                                MatrixByArr  this_diag = __this.GetBlock(bc, bc);
                                MatrixByArr   new_diag = this_diag - other_bmat;
                                __this.SetBlockLock(bc, bc, new_diag);
                                other_bmat = null;
                                lock(_count_ignored)
                                        _count_ignored[0]++;
                            }
                            if(other_bmat != null)
                            {
                                MatrixByArr  this_bmat = __this.GetBlock(bc, br);
                                if(this_bmat == null)
                                    this_bmat = new double[3, 3];
                                MatrixByArr   new_bmat = this_bmat - other_bmat;
                                __this.SetBlockLock(bc, br, new_bmat);
                            }
                        };
                        if(parallel)    HParallel.ForEach(other.EnumBlocks(), func);
                        else            foreach(var bc_br_bval in other.EnumBlocks()) func(bc_br_bval);
                            
                        iterinfo.numAddIgnrBlock = _count_ignored[0];
                    }
                    if(process_disp_console)
                    {
                        process_time.Add(DateTime.UtcNow);
                        int ptc = process_time.Count;
                        System.Console.Write("A-BinvDC({0:00.00} min), ", (process_time[ptc - 1] - process_time[ptc - 2]).TotalMinutes);
                    }
                    //HessMatrix nH = A - B_invD_C;
                    //nH = ((nH + nH.Tr())/2).ToHessMatrix();
                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Replace A -> H
                    H = A;

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // print iteration log
                    iterinfo.usedMemoryByte = GC.GetTotalMemory(false);
                    iterinfo.time1 = DateTime.UtcNow;
                    iterinfos.Add(iterinfo);

                    if(process_disp_console)
                        System.Console.Write("summary(makezero {0,5}, nonzero {1,5}, numIgnMul {2,7}, numRemvAtoms {3,3}, {4,5:0.00} min, {5} mb, {6}x{6}, nzeroBlk/Atom {7:0.00}), GC("
                                            , iterinfo.numSetZeroBlock
                                            , iterinfo.numNonZeroBlock
                                            , iterinfo.numAddIgnrBlock
                                            , iterinfo.numAtomsRemoved
                                            , iterinfo.compTime.TotalMinutes
                                            , iterinfo.usedMemoryByte/(1024*1024)
                                            , (idxkeep.Length*3)
                                            , ((double)iterinfo.numNonZeroBlock / idxremv.Length)
                                            );
                    GC.Collect();
                    if(process_disp_console)
                        System.Console.WriteLine(")");
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
