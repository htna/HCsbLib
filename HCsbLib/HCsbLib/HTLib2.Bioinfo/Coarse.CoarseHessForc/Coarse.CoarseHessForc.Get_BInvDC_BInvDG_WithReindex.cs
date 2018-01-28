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
            private static ValueTuple<HessMatrix, Vector> Get_BInvDC_BInvDG_WithReindex
                ( HessMatrix A
                , HessMatrix C
                , HessMatrix D
                , bool process_disp_console
                , string[] options
                , double? thld_BinvDC=null
                , bool parallel=false
                )
            {
                if(options == null)
                    options = new string[0];

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
                        Matlab.Execute("clear;");   if(process_disp_console) System.Console.Write("matlab(");
                        Matlab.PutMatrix("C", CC);  if(process_disp_console) System.Console.Write("C"); //Matlab.PutSparseMatrix("C", CC.GetMatrixSparse(), 3, 3);
                        Matlab.PutMatrix("D", D);   if(process_disp_console) System.Console.Write("D");

                        // Matlab.Execute("BinvDC = C' * inv(D) * C;");
                        {
                            // Matlab.Execute("BinvDC = C' * inv(D);");
                            // Matlab.Execute("BinvD_G = BinvDC * G;");
                            // Matlab.Execute("BinvDC  = BinvDC * C;");

                            {
                                if(options.Contains("/D"))
                                {
                                    Matlab.Execute("BinvDC = C' / D;");
                                }
                                else if(options.Contains("pinv(D)"))
                                {
                                    Matlab.Execute("BinvDC = C' * pinv(D);");
                                }
                                else if(options.Contains("/D -> pinv(D)"))
                                {
                                    string msg =  Matlab.Execute("BinvDC = C' / D;", true);
                                    if(msg != "") Matlab.Execute("BinvDC = C' * pinv(D);");
                                }
                                else
                                {
                                    Matlab.Execute("BinvDC = C' * inv(D);");
                                }
                            }
                            Matlab.Execute("BinvDC  = BinvDC * C;");
                        }
                        if(process_disp_console) System.Console.Write("X");

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
                return new ValueTuple<HessMatrix, Vector>
                    ( B_invD_C
                    , null
                    );
            }
        }
    }
}
