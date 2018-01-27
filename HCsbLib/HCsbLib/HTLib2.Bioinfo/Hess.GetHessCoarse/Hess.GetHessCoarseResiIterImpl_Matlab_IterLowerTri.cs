using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static partial class HessCoarseResiIter
        {
            public static CGetHessCoarseResiIterImpl GetHessCoarseResiIterImpl_Matlab_IterLowerTri
                ( object[] atoms
                , HessMatrix H
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
                List<HessCoarseResiIterInfo> iterinfos = new List<HessCoarseResiIterInfo>();
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
                    double C_density0;
                    double C_density1;
                    // make C sparse
                    {
                        double thres_absmax = thres_zeroblk;

                        C_density0 = 0;
                        List<Tuple<int, int>> lstIdxToMakeZero = new List<Tuple<int, int>>();
                        foreach(var bc_br_bval in H.EnumBlocksInCols(idxremv))
                        {
                            C_density0++;
                            int bc   = bc_br_bval.Item1;
                            int br   = bc_br_bval.Item2;
                            var bval = bc_br_bval.Item3;
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
                            System.Console.Write("CD({0:00.00} min), ", (process_time[ptc-1] -process_time[ptc-2]).TotalMinutes);
                        }

                        //  // make B,C sparse
                        //  //int B_cntzero = B.MakeNearZeroBlockAsZero(thres_zeroblk);
                        //  C_density0 = C.RatioUsedBlocks;
                        //  /// iterinfo.numSetZeroBlock = C.MakeNearZeroBlockAsZero(thres_zeroblk);
                        //  {
                        //      double thres_absmax = thres_zeroblk;
                        //  
                        //      List<Tuple<int, int>> lstIdxToMakeZero = new List<Tuple<int, int>>();
                        //      foreach(var bc_br_bval in C.EnumBlocks())
                        //      {
                        //          int bc   = bc_br_bval.Item1;
                        //          int br   = bc_br_bval.Item2;
                        //          var bval = bc_br_bval.Item3;
                        //          double absmax_bval = bval.HAbsMax();
                        //          if(absmax_bval < thres_absmax)
                        //          {
                        //              lstIdxToMakeZero.Add(new Tuple<int, int>(bc, br));
                        //          }
                        //      }
                        //      foreach(var bc_br in lstIdxToMakeZero)
                        //      {
                        //          int cc   = bc_br.Item1;
                        //          int cr   = bc_br.Item2;
                        //          var Cval = C.GetBlock(cc, cr);
                        //                      C.SetBlock(cc, cr, null);
                        //          var Dval = D.GetBlock(cc, cc);              // nCval = Cval    -Cval
                        //                      D.SetBlock(cc, cc, Dval+Cval);   // nDval = Dval - (-Cval) = Dval + Cval
                        //          int bc   = cr;
                        //          int br   = cc;
                        //          var Bval = Cval.Tr();
                        //          var Aval = A.GetBlock(bc, bc);              // nBval = Bval    -Bval
                        //                      A.SetBlock(bc, bc, Aval+Bval);   // nAval = Aval - (-Bval) = Aval + Bval
                        //      }
                        //      iterinfo.numSetZeroBlock = lstIdxToMakeZero.Count;
                        //  }
                        //  //int B_nzeros = B.NumUsedBlocks; double B_nzeros_ = Math.Sqrt(B_nzeros);
                        //  iterinfo.numNonZeroBlock = C.NumUsedBlocks;
                        //  C_density1 = C.RatioUsedBlocks;


                        HessMatrix B_invD_C = GetHessCoarseResiIterImpl_Matlab_IterLowerTri_Get_BInvDC(A, C, D, process_disp_console
                            , options
                            , thld_BinvDC: thres_zeroblk/lstNewIdxRemv.Length
                            , parallel:parallel
                            );
                        if(process_disp_console)
                        {
                            process_time.Add(DateTime.UtcNow);
                            int ptc = process_time.Count;
                            System.Console.Write("B.invD.C({0:00.00} min), ", (process_time[ptc-1] - process_time[ptc-2]).TotalMinutes);
                        }
                        GC.Collect(0);

                        /// iterinfo.numAddIgnrBlock = A.UpdateAdd(B_invD_C, -1, null, thres_zeroblk/lstNewIdxRemv.Length, parallel:parallel);
                        {
                            HessMatrix _this = A;
                            HessMatrix other = B_invD_C;
                            double thres_NearZeroBlock = thres_zeroblk/lstNewIdxRemv.Length;

                            //foreach(var bc_br_bval in other.EnumBlocks())
                            Action<ValueTuple<int, int, MatrixByArr>, object> func = delegate(ValueTuple<int, int, MatrixByArr> bc_br_bval, object _param)
                            {
                                Tuple<HessMatrix, double, int[], int[]> __param = _param as Tuple<HessMatrix, double, int[], int[]>;
                                HessMatrix  __this               =      __param.Item1;
                                double      _thres_NearZeroBlock =      __param.Item2;
                                int[]       _count               =      __param.Item3;
                                int[]       _count_ignored       =      __param.Item4;

                                _count[0]++;
                                int               bc   = bc_br_bval.Item1;
                                int               br   = bc_br_bval.Item2;
                                MatrixByArr other_bmat = bc_br_bval.Item3;
                                if(bc < br)
                                    return; // continue;
                                if(other_bmat.HAbsMax() <= _thres_NearZeroBlock)
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
                            Tuple<HessMatrix, double, int[], int[]> param = new Tuple<HessMatrix, double, int[], int[]>
                                ( _this
                                , thres_NearZeroBlock
                                , new int[1] // count              
                                , new int[1] // count_ignored      
                                );
                            if(parallel)    HParallel.ForEach(other.EnumBlocks(), param, func);
                            else            foreach(var bc_br_bval in other.EnumBlocks()) func(bc_br_bval, param);
                            

                            int count         = param.Item3[0];
                            int count_ignored = param.Item4[0];

                            iterinfo.numAddIgnrBlock = count_ignored;
                        }
                        if(process_disp_console)
                        {
                            process_time.Add(DateTime.UtcNow);
                            int ptc = process_time.Count;
                            System.Console.Write("A-BinvDC({0:00.00} min), ", (process_time[ptc - 1] - process_time[ptc - 2]).TotalMinutes);
                        }
                        //HessMatrix nH = A - B_invD_C;
                        //nH = ((nH + nH.Tr())/2).ToHessMatrix();
                        H = A;
                    }
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
            private static HessMatrix GetHessCoarseResiIterImpl_Matlab_IterLowerTri_Get_BInvDC
                ( HessMatrix A
                , HessMatrix C
                , HessMatrix D
                , bool process_disp_console
                , string[] options
                , double? thld_BinvDC=null
                , bool parallel=false
                )
            {
                HessMatrix B_invD_C;
                Dictionary<int, int> Cbr_CCbr = new Dictionary<int, int>();
                List<int>            CCbr_Cbr = new List<int>();
                foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in C.EnumBlocks())
                {
                    int Cbr = bc_br_bval.Item2;
                    if(Cbr_CCbr.ContainsKey(Cbr) == false)
                    {
                        HDebug.Assert(Cbr_CCbr.Count == CCbr_Cbr.Count);
                        int CCbr = Cbr_CCbr.Count;
                        Cbr_CCbr.Add(Cbr, CCbr);
                        CCbr_Cbr.Add(Cbr);
                        HDebug.Assert(CCbr_Cbr[CCbr] == Cbr);
                    }
                }

                HessMatrix CC = C.Zeros(C.ColSize, Cbr_CCbr.Count*3);
                {
                    Action<ValueTuple<int, int, MatrixByArr>> func = delegate(ValueTuple<int, int, MatrixByArr> bc_br_bval)
                    {
                        int Cbc  = bc_br_bval.Item1; int CCbc = Cbc;
                        int Cbr  = bc_br_bval.Item2; int CCbr = Cbr_CCbr[Cbr];
                        var bval = bc_br_bval.Item3;
                        lock(CC)
                             CC.SetBlock(CCbc, CCbr, bval);
                    };

                    if(parallel)    Parallel.ForEach(         C.EnumBlocks(), func);
                    else            foreach(var bc_br_bval in C.EnumBlocks()) func(bc_br_bval);
                }
                                                                                                                if(process_disp_console) { System.Console.Write("squeezeC({0,6}->{1,6} blk), ", C.RowBlockSize, CC.RowBlockSize); }
                {
                    /// If a diagonal element of D is null, that row and column should be empty.
                    /// This assume that the atom is removed. In this case, the removed diagonal block
                    /// is replace as the 3x3 identity matrix.
                    /// 
                    ///  [B1  0] [ A 0 ]^-1 [C1 C2 C3] = [B1  0] [ A^-1  0    ] [C1 C2 C3]
                    ///  [B2  0] [ 0 I ]    [ 0  0  0]   [B2  0] [ 0     I^-1 ] [ 0  0  0]
                    ///  [B3  0]                         [B3  0]
                    ///                                = [B1.invA  0] [C1 C2 C3]
                    ///                                  [B2.invA  0] [ 0  0  0]
                    ///                                  [B3.invA  0]
                    ///                                = [B1.invA.C1  B1.invA.C2  B1.invA.C3]
                    ///                                  [B2.invA.C1  B2.invA.C2  B2.invA.C3]
                    ///                                  [B3.invA.C1  B3.invA.C2  B3.invA.C3]
                    ///
                    {
                        //HDebug.Exception(D.ColBlockSize == D.RowBlockSize);
                        for(int bi=0; bi<D.ColBlockSize; bi++)
                        {
                            if(D.HasBlock(bi, bi) == true)
                                continue;
                            //for(int bc=0; bc< D.ColBlockSize; bc++) HDebug.Exception( D.HasBlock(bc, bi) == false);
                            //for(int br=0; br< D.RowBlockSize; br++) HDebug.Exception( D.HasBlock(bi, br) == false);
                            //for(int br=0; br<CC.RowBlockSize; br++) HDebug.Exception(CC.HasBlock(bi, br) == false);
                            D.SetBlock(bi, bi, new double[3, 3] {{1,0,0}, {0,1,0}, {0,0,1}});
                        }
                    }

                    HessMatrix BB_invDD_CC;
                    using(new Matlab.NamedLock(""))
                    {
                        Matlab.Execute("clear;");                                                               if(process_disp_console) System.Console.Write("matlab(");
                        Matlab.PutMatrix("C", CC);                                                              if(process_disp_console) System.Console.Write("C"); //Matlab.PutSparseMatrix("C", CC.GetMatrixSparse(), 3, 3); 
                        Matlab.PutMatrix("D", D);                                                               if(process_disp_console) System.Console.Write("D");
                        {   // Matlab.Execute("BinvDC = (C' / D) * C;");
                            if(options != null && options.Contains("pinv(D)"))
                            {
                                string msg =  Matlab.Execute("BinvDC = (C' / D) * C;",true);
                                if(msg != "") Matlab.Execute("BinvDC = C' * pinv(D) * C;");
                            }
                            else
                            {
                                Matlab.Execute("BinvDC = (C' / D) * C;");
                            }
                        }                                                                                       if(process_disp_console) System.Console.Write("X");
                        /// » whos
                        ///   Name         Size                 Bytes  Class     Attributes
                        ///                                                                                 //   before compressing C matrix
                        ///   C         1359x507              5512104  double                               //   C         1359x1545            16797240  double              
                        ///   CC        1359x507               198464  double    sparse                     //   CC        1359x1545              206768  double    sparse    
                        ///   D         1359x1359            14775048  double                               //   D         1359x1359            14775048  double              
                        ///   DD        1359x1359              979280  double    sparse                     //   DD        1359x1359              979280  double    sparse    
                        ///   ans          1x1                      8  double              
                        /// 
                        /// » tic; for i=1:30; A=(C' / D) * C; end; toc         dense  * dense  * dense  => 8.839463 seconds. (win)
                        /// Elapsed time is 8.839463 seconds.
                        /// » tic; for i=1:30; AA=(CC' / DD) * CC; end; toc     sparse * sparse * sparse => 27.945534 seconds.
                        /// Elapsed time is 27.945534 seconds.
                        /// » tic; for i=1:30; AAA=(C' / DD) * C; end; toc      sparse * dense  * sparse => 29.136144 seconds.
                        /// Elapsed time is 29.136144 seconds.
                        /// » 
                        /// » tic; for i=1:30; A=(C' / D) * C; end; toc         dense  * dense  * dense  => 8.469071 seconds. (win)
                        /// Elapsed time is 8.469071 seconds.
                        /// » tic; for i=1:30; AA=(CC' / DD) * CC; end; toc     sparse * sparse * sparse => 28.309953 seconds.
                        /// Elapsed time is 28.309953 seconds.
                        /// » tic; for i=1:30; AAA=(C' / DD) * C; end; toc      sparse * dense  * sparse => 28.586375 seconds.
                        /// Elapsed time is 28.586375 seconds.
                        //Matrix BBinvDDCC = Matlab.GetMatrix("BinvDC", true);                                    
                        if(thld_BinvDC != null)
                        {
                            Matlab.Execute("BinvDC(find(BinvDC < "+ thld_BinvDC.ToString() + ")) = 0;");
                        }
                        if(Matlab.GetValue("nnz(BinvDC)/numel(BinvDC)") > 0.5 || HDebug.True)
                        {
                            Func<int, int, HessMatrix> Zeros = delegate(int colsize, int rowsize)
                            {
                                return HessMatrixDense.ZerosDense(colsize, rowsize);
                            };
                            BB_invDD_CC = Matlab.GetMatrix("BinvDC", Zeros, true);
                            if(process_disp_console) System.Console.Write("Y), ");
                        }
                        else
                        {
                            Matlab.Execute("[i,j,s] = find(sparse(BinvDC));");
                            TVector<int>    listi = Matlab.GetVectorLargeInt("i", true);
                            TVector<int>    listj = Matlab.GetVectorLargeInt("j", true);
                            TVector<double> lists = Matlab.GetVectorLarge("s", true);
                            int colsize = Matlab.GetValueInt("size(BinvDC,1)");
                            int rowsize = Matlab.GetValueInt("size(BinvDC,2)");

                            Dictionary<ValueTuple<int, int>, MatrixByArr> lst_bc_br_bval = new Dictionary<ValueTuple<int, int>, MatrixByArr>();
                            for(long i=0; i<listi.SizeLong; i++)
                            {
                                int c = listi[i]-1; int bc = c/3; int ic = c%3;
                                int r = listj[i]-1; int br = r/3; int ir = r%3;
                                double v = lists[i];
                                ValueTuple<int, int> bc_br = new ValueTuple<int, int>(bc, br);
                                if(lst_bc_br_bval.ContainsKey(bc_br) == false)
                                    lst_bc_br_bval.Add(bc_br, new double[3, 3]);
                                lst_bc_br_bval[bc_br][ic, ir] = v;
                            }

                            //  Matrix BBinvDDCC = Matrix.Zeros(colsize, rowsize);
                            //  for(int i=0; i<listi.Length; i++)
                            //      BBinvDDCC[listi[i]-1, listj[i]-1] = lists[i];
                            //  //GC.Collect(0);
                            BB_invDD_CC = D.Zeros(colsize, rowsize);
                            foreach(var bc_br_bval in lst_bc_br_bval)
                            {
                                int bc = bc_br_bval.Key.Item1;
                                int br = bc_br_bval.Key.Item2;
                                var bval = bc_br_bval.Value;
                                BB_invDD_CC.SetBlock(bc, br, bval);
                            }
                            if(process_disp_console) System.Console.Write("Z), ");

                            if(HDebug.IsDebuggerAttached)
                            {
                                for(int i=0; i<listi.Size; i++)
                                {
                                    int c = listi[i]-1;
                                    int r = listj[i]-1;  
                                    double v = lists[i];
                                    HDebug.Assert(BB_invDD_CC[c, r] == v);
                                }
                            }
                        }
                        Matlab.Execute("clear;");
                    }
                    //GC.Collect(0);

                    B_invD_C = A.Zeros(C.RowSize, C.RowSize);
                    {
                        //  for(int bcc=0; bcc<CCbr_Cbr.Count; bcc++)
                        //  {
                        //      int bc = CCbr_Cbr[bcc];
                        //      for(int brr=0; brr<CCbr_Cbr.Count; brr++)
                        //      {
                        //          int br   = CCbr_Cbr[brr];
                        //          HDebug.Assert(B_invD_C.HasBlock(bc, br) == false);
                        //          if(BB_invDD_CC.HasBlock(bcc, brr) == false)
                        //              continue;
                        //          var bval = BB_invDD_CC.GetBlock(bcc, brr);
                        //          B_invD_C.SetBlock(bc, br, bval);
                        //          HDebug.Exception(A.HasBlock(bc, bc));
                        //          HDebug.Exception(A.HasBlock(br, br));
                        //      }
                        //  }
                        Action<ValueTuple<int, int, MatrixByArr>> func = delegate(ValueTuple<int, int, MatrixByArr> bcc_brr_bval)
                        {
                            int bcc = bcc_brr_bval.Item1;
                            int brr = bcc_brr_bval.Item2;
                            var bval= bcc_brr_bval.Item3;
                    
                            int bc = CCbr_Cbr[bcc];
                            int br = CCbr_Cbr[brr];
                            //lock(B_invD_C)
                                B_invD_C.SetBlockLock(bc, br, bval);
                        };
                    
                        if(parallel)    Parallel.ForEach(           BB_invDD_CC.EnumBlocks(), func);
                        else            foreach(var bcc_brr_bval in BB_invDD_CC.EnumBlocks()) func(bcc_brr_bval);
                    }
                }
                GC.Collect(0);
                return B_invD_C;
            }
        }
    }
}
