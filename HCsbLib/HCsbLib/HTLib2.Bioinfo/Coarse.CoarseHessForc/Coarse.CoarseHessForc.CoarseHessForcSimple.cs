using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForc
        {
            public static partial class CoarseHessForcSimple
            {
/*
                private static ValueTuple<HessMatrix, Vector> Get_BInvDC_BInvDG
                    (HessMatrix CC
                    , HessMatrix DD
                    , Vector GG
                    , bool process_disp_console
                    , string[] options
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
                            Matlab.Execute("BinvD_G = BinvDC * G;");
                            Matlab.Execute("BinvDC  = BinvDC * C;");
                        }
                        if(process_disp_console) System.Console.Write("X");


                        BB_invDD_GG = Matlab.GetVector("BinvD_G");


                        //Matrix BBinvDDCC = Matlab.GetMatrix("BinvDC", true);                                    
                        if(thld_BinvDC != null)
                        {
                            Matlab.Execute("BinvDC(find(BinvDC < " + thld_BinvDC.ToString() + ")) = 0;");
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
                            for(long i = 0; i < listi.SizeLong; i++)
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
                            BB_invDD_CC = DD.Zeros(colsize, rowsize);
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
                                for(int i = 0; i < listi.Size; i++)
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

                    return new ValueTuple<HessMatrix, Vector>
                        (BB_invDD_CC
                        , BB_invDD_GG
                        );
                }
*/
            }
        }
    }
}
