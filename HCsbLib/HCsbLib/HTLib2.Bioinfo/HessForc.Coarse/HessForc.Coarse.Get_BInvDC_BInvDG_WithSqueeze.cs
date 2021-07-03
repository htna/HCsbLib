using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
    public partial class HessForc
    {
        public static partial class Coarse
        {
            private static ValueTuple<HessMatrix, Vector> Get_BInvDC_BInvDG_WithSqueeze
                ( HessMatrix C
                , HessMatrix D
                , Vector     G
                , bool process_disp_console
                , string[] options
                , double? thld_BinvDC   // =null
                , bool parallel         // =false
                )
            {
                if(options == null)
                    options = new string[0];

                HessMatrix B_invD_C;
                Vector     B_invD_G;
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

                HessMatrix CC = HessMatrix.ZerosHessMatrix(C.ColSize, Cbr_CCbr.Count*3);
                {
                    Action<ValueTuple<int, int, MatrixByArr>, HessMatrix, Dictionary<int, int>> func =
                        delegate(ValueTuple<int, int, MatrixByArr> bc_br_bval, HessMatrix _CC, Dictionary<int, int> _Cbr_CCbr)
                    {
                        int Cbc  = bc_br_bval.Item1; int CCbc = Cbc;
                        int Cbr  = bc_br_bval.Item2; int CCbr = _Cbr_CCbr[Cbr];
                        var bval = bc_br_bval.Item3;
                        lock(_CC)
                                _CC.SetBlock(CCbc, CCbr, bval);
                    };

                    if(parallel)   HParallel.ForEach(         C.EnumBlocks(), func           , CC, Cbr_CCbr);
                    else            foreach(var bc_br_bval in C.EnumBlocks()) func(bc_br_bval, CC, Cbr_CCbr);
                }
                if(process_disp_console)
                {
                    System.Console.Write("squeezeC({0,6}->{1,6} blk), ", C.RowBlockSize, CC.RowBlockSize);
                }

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
                    Vector     BB_invDD_GG;
                    {
                        var BBInvDDCC_BBInvDDGG = Get_BInvDC_BInvDG(CC, D, G, process_disp_console, options, thld_BinvDC, parallel);
                        BB_invDD_CC = BBInvDDCC_BBInvDDGG.Item1;
                        BB_invDD_GG = BBInvDDCC_BBInvDDGG.Item2;
                    }

                    B_invD_C = HessMatrix.ZerosHessMatrix(C.RowSize, C.RowSize);
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
                        Action<ValueTuple<int, int, MatrixByArr>, HessMatrix, List<int>> func =
                            delegate(ValueTuple<int, int, MatrixByArr> bcc_brr_bval, HessMatrix _B_invD_C, List<int> _CCbr_Cbr)
                        {
                            int bcc = bcc_brr_bval.Item1;
                            int brr = bcc_brr_bval.Item2;
                            var bval= bcc_brr_bval.Item3;
                    
                            int bc = _CCbr_Cbr[bcc];
                            int br = _CCbr_Cbr[brr];
                            //lock(B_invD_C)
                                _B_invD_C.SetBlockLock(bc, br, bval);
                        };
                    
                        if(parallel)   HParallel.ForEach(           BB_invDD_CC.EnumBlocks(), func             , B_invD_C, CCbr_Cbr);
                        else            foreach(var bcc_brr_bval in BB_invDD_CC.EnumBlocks()) func(bcc_brr_bval, B_invD_C, CCbr_Cbr);
                    }
                    B_invD_G = new double[C.RowSize];
                    {
                        HDebug.Assert(BB_invDD_GG.Size % 3 == 0);
                        /////////////////////////////////////////////////////////////////////////////////////
                        // Double Check Later
                        if(CCbr_Cbr.Count == 0)
                        {
                            // this is the case that a molecule is isolated.
                            //HDebug.Assert(B_invD_C.NumUsedBlocks == 0);
                            //HDebug.Assert(B_invD_G.Dist2 == 0);
                        }

                        for(int bii=0; bii<CCbr_Cbr.Count; bii++)
                        //for(int bii=0; bii<BB_invDD_GG.Size/3; bii++)
                        /////////////////////////////////////////////////////////////////////////////////////
                        {
                            int bi = CCbr_Cbr[bii];
                            B_invD_G[bi * 3 + 0] = BB_invDD_GG[bii * 3 + 0];
                            B_invD_G[bi * 3 + 1] = BB_invDD_GG[bii * 3 + 1];
                            B_invD_G[bi * 3 + 2] = BB_invDD_GG[bii * 3 + 2];
                        }
                    }
                }
                GC.Collect(0);
                return new ValueTuple<HessMatrix, Vector>
                    ( B_invD_C
                    , B_invD_G
                    );
            }
        }
    }
}
