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
            public static class GetHessCoarseResiIterImpl_LinAlg_IterLowerTri
            {
                public static CGetHessCoarseResiIterImpl Do
                    ( object[] atoms
                    , HessMatrix H
                    , List<int>[] lstNewIdxRemv
                    , double thres_zeroblk
                    , ILinAlg ila
                    , bool cloneH
                    , string[] options
                    )
                {
                    //HDebug.ToDo();

                    ila = null;
                    cloneH = true;
                    if(cloneH)
                        H = H.CloneHess();

                    bool       process_disp_console = true;
                    if(options != null && options.Contains("print process"))
                        process_disp_console = true;

                    bool parallel = false;

                    /// keep only lower triangle of H (lower block triangles)
                    {
                        HashSet<Tuple<int, int, MatrixByArr>> lstUppTrig = new HashSet<Tuple<int, int, MatrixByArr>>();
                        foreach(Tuple<int, int, MatrixByArr> bc_br_bval in H.EnumBlocks_dep())
                        {
                            int bc = bc_br_bval.Item1;
                            int br = bc_br_bval.Item2;
                            if(bc < br)
                            {
                                lstUppTrig.Add(bc_br_bval);
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

                    DateTime[] process_time         = new DateTime[6];

                    //System.Console.WriteLine("begin coarse-graining");
                    List<HessCoarseResiIterInfo> iterinfos = new List<HessCoarseResiIterInfo>();
                    for(int iter=lstNewIdxRemv.Length-1; iter>=0; iter--)
                    {
                        bool lprocess_disp_console = (process_disp_console && iter%5==0);
                        lprocess_disp_console = true;
                        if(lprocess_disp_console)
                        {
                            process_time[0] = DateTime.UtcNow;
                            System.Console.Write(" - {0:000} : ", iter);
                        }

                        //int[] ikeep = lstNewIdxRemv[iter].Item1;

                        HessCoarseResiIterInfo iterinfo = new HessCoarseResiIterInfo();
                        iterinfo.sizeHessBlkMat = 1;
                        iterinfo.numAtomsRemoved = 1;
                        iterinfo.time0 = DateTime.UtcNow;
                        int _count_update = 0;
                        {
                            HDebug.Assert(lstNewIdxRemv[iter].Count == 1);
                            int iremv = lstNewIdxRemv[iter][0];

                            HessMatrix        A = H;
                            MatrixByArr       D = null;
                            double            D_absmin  = 0;
                            List<int>         C_lstbr   = new List<int>();
                            List<MatrixByArr> C_lstbval = new List<MatrixByArr>();
                            {
                                // get C and D
                                foreach(var (bc, br, bval) in H.EnumBlocks(new int[] { iremv }))
                                {
                                    HDebug.Assert(iremv == bc);
                                    if(bc == br)
                                    {
                                        HDebug.Assert(D == null);
                                        D        = bval;
                                        D_absmin = D.HAbsMin();
                                    }
                                    else
                                    {
                                        if(bval.HAbsMin() >= thres_zeroblk)
                                        {
                                            C_lstbr.Add(br);
                                            C_lstbval.Add(bval);
                                        }
                                    }
                                }
                                // remove C and D
                                {
                                    int bc = iremv;
                                    H.SetBlock(bc, bc, null);
                                    foreach(int br in C_lstbr)
                                        H.SetBlock(bc, br, null);
                                }
                            }
                            if(lprocess_disp_console)
                            {
                                process_time[1] = process_time[2] = DateTime.UtcNow;
                                System.Console.Write("CD({0:00.00} min), ", (process_time[2] - process_time[0]).TotalMinutes);
                            }

                            double threshold = thres_zeroblk / lstNewIdxRemv.Length;

                            //  DD ={ { d00,d01,d02},{ d10,d11,d12},{ d20,d21,d22} }; MatrixForm[DD]
                            //  BB ={ { b00,b01,b02},{ b10,b11,b12},{ b20,b21,b22} }; MatrixForm[BB]
                            //  CC ={ { c00,c01,c02},{ c10,c11,c12},{ c20,c21,c22} }; MatrixForm[CC]
                            //  object[]    dbginfo = null; // new object[] { iremv, atoms, H, D, C_lstbr, C_lstbval };

                            Action<ValueTuple<int, int, MatrixByArr, MatrixByArr>> Update_A_B_invD_C = delegate(ValueTuple<int, int, MatrixByArr, MatrixByArr> info)
                            {
                                int         bc = info.Item1; // bc      
                                int         br = info.Item2; // br      
                                MatrixByArr DC = info.Item3; // invDD_CC  // XX ={ { x00,x01,x02},{ x10,x11,x12},{ x20,x21,x22} }; MatrixForm[CC]
                                MatrixByArr B  = info.Item4; // BB        // BB ={ { b00,b01,b02},{ b10,b11,b12},{ b20,b21,b22} }; MatrixForm[BB]
                                //var _iremv                      = (int)dbginfo[0];
                                //var _atoms                      = dbginfo[1] as Universe.Atom[]  ;
                                //var _H                          = dbginfo[2] as HessMatrix       ;
                                //var _D                          = dbginfo[3] as MatrixByArr      ;
                                //var _C_lstbr                    = dbginfo[4] as List<int>        ;
                                //var _C_lstbval                  = dbginfo[5] as List<MatrixByArr>;

                                //MatrixByArr BB_invDD_CC = BB * invDD_CC;
                                double _BDC00 = 0 - B[0,0] * DC[0,0] - B[0,1] * DC[1,0] - B[0,2] * DC[2,0]; // { { b00 x00 +b01 x10 + b02 x20
                                double _BDC01 = 0 - B[0,0] * DC[0,1] - B[0,1] * DC[1,1] - B[0,2] * DC[2,1]; //   , b00 x01 +b01 x11 + b02 x21
                                double _BDC02 = 0 - B[0,0] * DC[0,2] - B[0,1] * DC[1,2] - B[0,2] * DC[2,2]; //   , b00 x02 +b01 x12 + b02 x22
                                double _BDC10 = 0 - B[1,0] * DC[0,0] - B[1,1] * DC[1,0] - B[1,2] * DC[2,0]; // },{ b10 x00 +b11 x10 + b12 x20
                                double _BDC11 = 0 - B[1,0] * DC[0,1] - B[1,1] * DC[1,1] - B[1,2] * DC[2,1]; //   , b10 x01 +b11 x11 + b12 x21
                                double _BDC12 = 0 - B[1,0] * DC[0,2] - B[1,1] * DC[1,2] - B[1,2] * DC[2,2]; //   , b10 x02 +b11 x12 + b12 x22
                                double _BDC20 = 0 - B[2,0] * DC[0,0] - B[2,1] * DC[1,0] - B[2,2] * DC[2,0]; // },{ b20 x00 +b21 x10 + b22 x20
                                double _BDC21 = 0 - B[2,0] * DC[0,1] - B[2,1] * DC[1,1] - B[2,2] * DC[2,1]; //   , b20 x01 +b21 x11 + b22 x21
                                double _BDC22 = 0 - B[2,0] * DC[0,2] - B[2,1] * DC[1,2] - B[2,2] * DC[2,2]; //   , b20 x02 +b21 x12 + b22 x22 }}

                                if(A.HasBlockLock(bc, br))
                                {
                                    MatrixByArr A_bc_br = A.GetBlockLock(bc, br);
                                    A_bc_br[0,0] += _BDC00; // A = A + (-B.invD.C)
                                    A_bc_br[0,1] += _BDC01; // A = A + (-B.invD.C)
                                    A_bc_br[0,2] += _BDC02; // A = A + (-B.invD.C)
                                    A_bc_br[1,0] += _BDC10; // A = A + (-B.invD.C)
                                    A_bc_br[1,1] += _BDC11; // A = A + (-B.invD.C)
                                    A_bc_br[1,2] += _BDC12; // A = A + (-B.invD.C)
                                    A_bc_br[2,0] += _BDC20; // A = A + (-B.invD.C)
                                    A_bc_br[2,1] += _BDC21; // A = A + (-B.invD.C)
                                    A_bc_br[2,2] += _BDC22; // A = A + (-B.invD.C)
                                    // (small && small && small) == !(large || large || large)
                                    bool toosmall = !( Math.Abs(A_bc_br[0,0]) > threshold || Math.Abs(A_bc_br[0,1]) > threshold || Math.Abs(A_bc_br[0,2]) > threshold ||
                                                       Math.Abs(A_bc_br[1,0]) > threshold || Math.Abs(A_bc_br[1,1]) > threshold || Math.Abs(A_bc_br[1,2]) > threshold ||
                                                       Math.Abs(A_bc_br[2,0]) > threshold || Math.Abs(A_bc_br[2,1]) > threshold || Math.Abs(A_bc_br[2,2]) > threshold );
                                    if(toosmall)
                                    {
                                        HDebug.Assert(bc != br);
                                        A.SetBlockLock(bc, br, null);
                                    }
                                }
                                else
                                {
                                    // (small && small && small) == !(large || large || large)
                                    bool toosmall = !( Math.Abs(_BDC00) > threshold || Math.Abs(_BDC01) > threshold || Math.Abs(_BDC02) > threshold ||
                                                       Math.Abs(_BDC10) > threshold || Math.Abs(_BDC11) > threshold || Math.Abs(_BDC12) > threshold ||
                                                       Math.Abs(_BDC20) > threshold || Math.Abs(_BDC21) > threshold || Math.Abs(_BDC22) > threshold );
                                    if(!toosmall)
                                    {
                                        MatrixByArr A_bc_br = new double[3,3]
                                        { { _BDC00, _BDC01, _BDC02 } // A = 0 + (-B.invD.C)
                                         ,{ _BDC10, _BDC11, _BDC12 } // A = 0 + (-B.invD.C)
                                         ,{ _BDC20, _BDC21, _BDC22 } // A = 0 + (-B.invD.C)
                                        };
                                        A.SetBlockLock(bc, br, A_bc_br);
                                    }
                                }

                                System.Threading.Interlocked.Increment(ref _count_update);
                            };

                            var lstCompInfo = EnumComput(D, C_lstbr, C_lstbval, threshold);
                            if(parallel) Parallel.ForEach(lstCompInfo, Update_A_B_invD_C);
                            else         foreach(var info in lstCompInfo) Update_A_B_invD_C(info);

                            //GC.Collect(0);

                            if(lprocess_disp_console)
                            {
                                process_time[4] = DateTime.UtcNow;
                                System.Console.Write("B.invD.C({0:00.00} min), ", (process_time[4] - process_time[3]).TotalMinutes);
                            }

                            H = A;
                        }
                        iterinfo.usedMemoryByte = GC.GetTotalMemory(false);
                        iterinfo.time1 = DateTime.UtcNow;
                        iterinfos.Add(iterinfo);

                        if(lprocess_disp_console)
                            System.Console.WriteLine("summary(makezero {0,5}, nonzero {1,5}, numIgnMul {2,7}, numRemvAtoms {3,3}, {4,5:0.00} sec, {5} mb, {6}x{6}, nzeroBlk/Atom {7:0.00}, cntUpdateBlk({8}))"
                                                , iterinfo.numSetZeroBlock
                                                , iterinfo.numNonZeroBlock
                                                , iterinfo.numAddIgnrBlock
                                                , iterinfo.numAtomsRemoved
                                                , iterinfo.compSec
                                                , iterinfo.usedMemoryByte / (1024 * 1024)
                                                , (0 * 3)
                                                , 0//((double)iterinfo.numNonZeroBlock / idxremv.Length)
                                                , _count_update
                                                );

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
                private static IEnumerable<ValueTuple<int, int, MatrixByArr, MatrixByArr>> EnumComput
                    ( MatrixByArr       D
                    , List<int>         C_lstbr
                    , List<MatrixByArr> C_lstbval
                    , double            threshold
                    )
                {
                    MatrixByArr       invDD = LinAlg.Inv3x3(D);
                    HDebug.Assert(invDD.IsComputable());
                    HDebug.Assert(invDD != null);

                    List<MatrixByArr> C_lstbval_tr = new List<MatrixByArr>();
                    List<double> C_lstbval_absmax = new List<double>();
                    for(int i=0; i< C_lstbval.Count; i++)
                    {
                        C_lstbval_tr.Add(C_lstbval[i].Tr());
                        C_lstbval_absmax.Add(C_lstbval[i].HAbsMax());
                    }

                    for(int c = 0; c < C_lstbr.Count; c++)
                    {
                        int bc = C_lstbr[c];
                        MatrixByArr CC = C_lstbval[c];
                        MatrixByArr invDD_CC = invDD * CC;
                        double      invDD_CC_absmax = invDD_CC.HAbsMax();

                        for(int r=0; r<C_lstbr.Count; r++)
                        {
                            int br = C_lstbr[r];
                            if(bc < br)
                                // upper-triangle
                                continue;

                            //double BB_absmax = C_lstbval_absmax[r];
                            //if(BB_absmax*invDD_CC_absmax < threshold)
                            //    continue;

                            MatrixByArr BB = C_lstbval_tr[r];

                            yield return
                                new ValueTuple<int, int, MatrixByArr, MatrixByArr>
                                ( bc
                                , br
                                , invDD_CC
                                , BB
                                );
                        }
                    }
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
                    foreach(Tuple<int, int, MatrixByArr> bc_br_bval in C.EnumBlocks_dep())
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

                    HessMatrix CC = HessMatrixSparse.ZerosSparse(C.ColSize, Cbr_CCbr.Count*3);
                    {
                        Action<Tuple<int, int, MatrixByArr>> func = delegate(Tuple<int, int, MatrixByArr> bc_br_bval)
                        {
                            int Cbc  = bc_br_bval.Item1; int CCbc = Cbc;
                            int Cbr  = bc_br_bval.Item2; int CCbr = Cbr_CCbr[Cbr];
                            var bval = bc_br_bval.Item3;
                            lock(CC)
                                 CC.SetBlock(CCbc, CCbr, bval);
                        };

                        if(parallel)    Parallel.ForEach(        C.EnumBlocks_dep(), func);
                        else            foreach(var bc_br_bval in C.EnumBlocks_dep()) func(bc_br_bval);
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
                            if(Matlab.GetValue("nnz(BinvDC)/numel(BinvDC)") > 0.5)
                            {
                                double[,] arr = Matlab.GetMatrix("BinvDC", true);
                                BB_invDD_CC = HessMatrixDense.FromMatrix(arr);
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
                                BB_invDD_CC = HessMatrixSparse.ZerosSparse(colsize, rowsize);
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

                        B_invD_C = HessMatrixSparse.ZerosSparse(C.RowSize, C.RowSize);
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
                            Action<Tuple<int, int, MatrixByArr>> func = delegate(Tuple<int, int, MatrixByArr> bcc_brr_bval)
                            {
                                int bcc = bcc_brr_bval.Item1;
                                int brr = bcc_brr_bval.Item2;
                                var bval= bcc_brr_bval.Item3;
                    
                                int bc = CCbr_Cbr[bcc];
                                int br = CCbr_Cbr[brr];
                                lock(B_invD_C)
                                    B_invD_C.SetBlock(bc, br, bval);
                            };
                    
                            if(parallel)    Parallel.ForEach(          BB_invDD_CC.EnumBlocks_dep(), func);
                            else            foreach(var bcc_brr_bval in BB_invDD_CC.EnumBlocks_dep()) func(bcc_brr_bval);
                        }
                    }
                    GC.Collect(0);
                    return B_invD_C;
                }
            }
        }
    }
}
