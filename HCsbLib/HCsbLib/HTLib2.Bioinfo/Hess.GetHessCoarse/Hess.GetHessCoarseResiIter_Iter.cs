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
            public enum IterOption
            { ILinAlg_20150329
            , ILinAlg
            , Matlab
            , Matlab_experimental
            , Matlab_IterLowerTri
            , LinAlg_IterLowerTri
            };

            public static HessInfoCoarseResiIter GetHessCoarseResiIter
                ( Hess.HessInfo hessinfo
                , Vector[] coords
                , FuncGetIdxKeepListRemv GetIdxKeepListRemv
                , ILinAlg ila
                , double thres_zeroblk=0.001
                , IterOption iteropt = IterOption.Matlab_experimental
                , string[] options=null
                )
            {
                bool rediag=true;

                HessMatrix H = null;
                List<int>[] lstNewIdxRemv = null;
                int numca  = 0;
                double[] reMass   = null;
                object[] reAtoms  = null;
                Vector[] reCoords = null;
                Tuple<int[],int[][]> idxKeepRemv = null;
                //System.Console.WriteLine("begin re-indexing hess");
                {
                    object[] atoms = hessinfo.atoms;
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

                    H = hessinfo.hess.ReshapeByAtom(idxs);
                    numca    = idxKeep.Length;
                    reMass   = hessinfo.mass .ToArray().HSelectByIndex(idxs);
                    reAtoms  = hessinfo.atoms.ToArray().HSelectByIndex(idxs);
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
                GC.Collect(0);
                HDebug.Assert(numca == H.ColBlockSize - lstNewIdxRemv.HListCount().Sum());

                //if(bool.Parse("false"))
                {
                    if(bool.Parse("false"))
                        #region
                    {
                        int[]   idxKeep  = idxKeepRemv.Item1;
                        int[][] idxsRemv = idxKeepRemv.Item2;
                        Pdb.Atom[] pdbatoms = hessinfo.atomsAsUniverseAtom.ListPdbAtoms();
                        Pdb.ToFile(@"C:\temp\coarse-keeps.pdb", pdbatoms.HSelectByIndex(idxKeep), false);
                        if(HFile.Exists(@"C:\temp\coarse-graining.pdb"))
                            HFile.Delete(@"C:\temp\coarse-graining.pdb");
                        foreach(int[] idxremv in idxsRemv.Reverse())
                        {
                            List<Pdb.Element> delatoms = new List<Pdb.Element>();
                            foreach(int idx in idxremv)
                            {
                                if(pdbatoms[idx] == null)
                                    continue;
                                string line = pdbatoms[idx].GetUpdatedLine(coords[idx]);
                                Pdb.Atom delatom = Pdb.Atom.FromString(line);
                                delatoms.Add(delatom);
                            }
                            Pdb.ToFile(@"C:\temp\coarse-graining.pdb", delatoms.ToArray(), true);
                        }
                    }
                        #endregion

                    if(bool.Parse("false"))
                        #region
                    {
                        // export matrix to matlab, so the matrix can be checked in there.
                        int[] idxca  = HEnum.HEnumCount(numca).ToArray();
                        int[] idxoth = HEnum.HEnumFromTo(numca, coords.Length-1).ToArray();
                        Matlab.Register(@"C:\temp\");
                        Matlab.PutSparseMatrix("H", H.GetMatrixSparse(), 3, 3);
                        Matlab.Execute("figure; spy(H)");
                        Matlab.Clear();
                    }
                        #endregion

                    if(bool.Parse("false"))
                        #region
                    {
                        HDirectory.CreateDirectory(@"K:\temp\$coarse-graining\");
                        {   // export original hessian matrix
                            List<int> cs = new List<int>();
                            List<int> rs = new List<int>();
                            foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in hessinfo.hess.EnumBlocks())
                            {
                                cs.Add(bc_br_bval.Item1);
                                rs.Add(bc_br_bval.Item2);
                            }
                            Matlab.Clear();
                            Matlab.PutVector("cs", cs.ToArray());
                            Matlab.PutVector("rs", rs.ToArray());
                            Matlab.Execute("hess = sparse(cs+1, rs+1, ones(size(cs)));");
                            Matlab.Execute("hess = float(hess);");
                            Matlab.Execute("figure; spy(hess)");
                            Matlab.Execute("cs = int32(cs+1);");
                            Matlab.Execute("rs = int32(rs+1);");
                            Matlab.Execute(@"save('K:\temp\$coarse-graining\hess-original.mat', 'cs', 'rs', '-v6');");
                            Matlab.Clear();
                        }
                        {   // export reshuffled hessian matrix
                            List<int> cs = new List<int>();
                            List<int> rs = new List<int>();
                            foreach(ValueTuple<int, int, MatrixByArr> bc_br_bval in H.EnumBlocks())
                            {
                                cs.Add(bc_br_bval.Item1);
                                rs.Add(bc_br_bval.Item2);
                            }
                            Matlab.Clear();
                            Matlab.PutVector("cs", cs.ToArray());
                            Matlab.PutVector("rs", rs.ToArray());
                            Matlab.Execute("H = sparse(cs+1, rs+1, ones(size(cs)));");
                            Matlab.Execute("H = float(H);");
                            Matlab.Execute("figure; spy(H)");
                            Matlab.Execute("cs = int32(cs+1);");
                            Matlab.Execute("rs = int32(rs+1);");
                            Matlab.Execute(@"save('K:\temp\$coarse-graining\hess-reshuffled.mat', 'cs', 'rs', '-v6');");
                            Matlab.Clear();
                        }
                    }
                        #endregion

                    if(bool.Parse("false"))
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
                }

                List<HessCoarseResiIterInfo> iterinfos = null;
                {
                    object[] atoms = reAtoms; // reAtoms.HToType(null as Universe.Atom[]);
                    CGetHessCoarseResiIterImpl info = null;
                    switch(iteropt)
                    {
                        case IterOption.ILinAlg_20150329   : info = GetHessCoarseResiIterImpl_ILinAlg_20150329(H, lstNewIdxRemv, thres_zeroblk, ila, false);                    break;
                        case IterOption.ILinAlg            : info = GetHessCoarseResiIterImpl_ILinAlg(H, lstNewIdxRemv, thres_zeroblk, ila, false);                             break;
                        case IterOption.Matlab             : info = GetHessCoarseResiIterImpl_Matlab(atoms, H, lstNewIdxRemv, thres_zeroblk, ila, false, options);              break;
                        case IterOption.Matlab_experimental: info = GetHessCoarseResiIterImpl_Matlab_experimental(atoms, H, lstNewIdxRemv, thres_zeroblk, ila, false, options); break;
                        case IterOption.Matlab_IterLowerTri: info = GetHessCoarseResiIterImpl_Matlab_IterLowerTri(atoms, H, lstNewIdxRemv, thres_zeroblk, ila, false, options); break;
                        case IterOption.LinAlg_IterLowerTri: info = GetHessCoarseResiIterImpl_LinAlg_IterLowerTri.Do(atoms, H, lstNewIdxRemv, thres_zeroblk, ila, false, options); break;
                    };
                    H = info.H;
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
                        bool equal = object.ReferenceEquals(hessinfo.atoms[idx], reAtoms[nidx]);
                        if(equal == false)
                            HDebug.Assert(false);
                        HDebug.Assert(equal);
                        nidx++;
                    }
                }

                if(rediag)
                    H = H.CorrectHessDiag();
                //System.Console.WriteLine("finish fixing diag");

                return new HessInfoCoarseResiIter
                {
                    hess          = H,
                    mass          = reMass.HSelectCount(numca),
                    atoms         = reAtoms.HSelectCount(numca),
                    coords        = reCoords.HSelectCount(numca),
                    numZeroEigval = 6,
                    iterinfos     = iterinfos,
                };
            }
            private static HessMatrix Get_BInvDC
                ( HessMatrix A
                , HessMatrix C
                , HessMatrix D
                , bool process_disp_console
                , string[] options
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

                HessMatrix CC = HessMatrixSparse.ZerosSparse(C.ColSize, Cbr_CCbr.Count*3);
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

                    HessMatrixSparse BB_invDD_CC;
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
                        Matrix BBinvDDCC = Matlab.GetMatrix("BinvDC", true);                                    if(process_disp_console) System.Console.Write("Y");
                                            //Matlab.Execute("[i,j,s] = find(sparse(BinvDC));");
                                            //int[] listi = Matlab.GetVectorInt("i");
                                            //int[] listj = Matlab.GetVectorInt("j");
                                            //double[] lists = Matlab.GetVector("s");
                                            //int colsize = Matlab.GetValueInt("size(BinvDC,1)");
                                            //int rowsize = Matlab.GetValueInt("size(BinvDC,2)");
                                            //Matrix BBinvDDCC = Matrix.Zeros(colsize, rowsize);
                                            //for(int i=0; i<listi.Length; i++)
                                            //    BBinvDDCC[listi[i], listj[i]] = lists[i];
                        //GC.Collect(0);
                        BB_invDD_CC = HessMatrixSparse.FromMatrix(BBinvDDCC, parallel);                         if(process_disp_console) System.Console.Write("Z), ");
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
                        Action<ValueTuple<int, int, MatrixByArr>> func = delegate(ValueTuple<int, int, MatrixByArr> bcc_brr_bval)
                        {
                            int bcc = bcc_brr_bval.Item1;
                            int brr = bcc_brr_bval.Item2;
                            var bval= bcc_brr_bval.Item3;

                            int bc = CCbr_Cbr[bcc];
                            int br = CCbr_Cbr[brr];
                            lock(B_invD_C)
                                B_invD_C.SetBlock(bc, br, bval);
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
