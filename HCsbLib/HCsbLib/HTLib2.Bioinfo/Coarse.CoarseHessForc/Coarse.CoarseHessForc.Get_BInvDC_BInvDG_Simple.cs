using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForc
        {
                private static ValueTuple<HessMatrix, Vector> Get_BInvDC_BInvDG_Simple
                    (HessMatrix CC
                    , HessMatrix DD
                    , Vector GG
                    , bool process_disp_console
                    , double? thld_BinvDC = null
                    , bool parallel = false
                    )
                {
                    HessMatrix BB_invDD_CC;
                    Vector     BB_invDD_GG;
                    using(new Matlab.NamedLock(""))
                    {
                        Matlab.Execute("clear;"); if(process_disp_console) System.Console.Write("matlab(");
                        Matlab.PutMatrix("C", CC); if(process_disp_console) System.Console.Write("C"); //Matlab.PutSparseMatrix("C", C.GetMatrixSparse(), 3, 3);
                        Matlab.PutMatrix("D", DD); if(process_disp_console) System.Console.Write("D");
                        Matlab.PutVector("G", GG); if(process_disp_console) System.Console.Write("G");
                        // Matlab.Execute("BinvDC = C' * inv(D) * C;");
                        {
                            Matlab.Execute("BinvDC = C' * inv(D);");
                            Matlab.Execute("BinvD_G = BinvDC * G;");
                            Matlab.Execute("BinvDC  = BinvDC * C;");
                        }

                        BB_invDD_GG = Matlab.GetVector("BinvD_G");

                        //Matrix BBinvDDCC = Matlab.GetMatrix("BinvDC", true);                                    
                        if(thld_BinvDC != null)
                        {
                            Matlab.Execute("BinvDC(find(BinvDC < " + thld_BinvDC.ToString() + ")) = 0;");
                        }

                        Func<int, int, HessMatrix> Zeros = delegate(int colsize, int rowsize)
                        {
                            return HessMatrixDense.ZerosDense(colsize, rowsize);
                        };
                        BB_invDD_CC = Matlab.GetMatrix("BinvDC", Zeros, true);
                        if(process_disp_console) System.Console.Write("Y), ");

                        Matlab.Execute("clear;");
                    }
                    //GC.Collect(0);

                    return new ValueTuple<HessMatrix, Vector>
                        ( BB_invDD_CC
                        , BB_invDD_GG
                        );
                }
        }
    }
}
