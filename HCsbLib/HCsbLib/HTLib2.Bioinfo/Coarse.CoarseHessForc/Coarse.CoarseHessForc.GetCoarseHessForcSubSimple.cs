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
            public static CGetHessCoarseResiIterImpl GetCoarseHessForcSubSimple
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
                if(cloneH)
                    H = H.CloneHess();

                bool process_disp_console = false;
                bool parallel = true;

                for(int iter=lstNewIdxRemv.Length-1; iter>=0; iter--)
                {
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
                    HessMatrix    A = H.SubMatrixByAtoms(false, idxkeep, idxkeep);
                    HessMatrix    B = H.SubMatrixByAtoms(false, idxkeep, idxremv);
                    HessMatrix    C = H.SubMatrixByAtoms(false, idxremv, idxkeep);
                    HessMatrix    D = H.SubMatrixByAtoms(false, idxremv, idxremv);
                    Vector        nF;
                    Vector        nG;
                    {
                        nF = new double[idxkeep.Length * 3];
                        nG = new double[idxremv.Length * 3];
                        for(int i = 0; i < idxkeep.Length * 3; i++) nF[i] = F[i];
                        for(int i = 0; i < idxremv.Length * 3; i++) nG[i] = F[i + nF.Size];
                    }

                    Matlab.PutMatrix("A", A, true);
                    Matlab.PutMatrix("B", B, true);
                    Matlab.PutMatrix("C", C, true);
                    Matlab.PutMatrix("D", D, true);
                    Matlab.PutVector("F", nF);
                    Matlab.PutVector("G", nG);

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Get B.inv(D).C
                    //
                    // var BInvDC_BInvDG = Get_BInvDC_BInvDG_WithSqueeze(C, D, nG, process_disp_console
                    //     , options
                    //     , thld_BinvDC: thres_zeroblk/lstNewIdxRemv.Length
                    //     , parallel: parallel
                    //     );
                    // HessMatrix B_invD_C = BInvDC_BInvDG.Item1;
                    // Vector     B_invD_G = BInvDC_BInvDG.Item2;
                    // GC.Collect(0);
                    Matlab.Execute("BinvD = B * inv(D);");
                    Matlab.Execute("clear B, D;");
                    Matlab.Execute("BinvDC = BinvD * C;");
                    Matlab.Execute("BinvDG = BinvD * G;");

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Get A - B.inv(D).C
                    //     F - B.inv(D).G
                    Matlab.Execute("HH = A - BinvDC;");
                    Matlab.Execute("FF = F - BinvDG;");

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Replace A -> H
                    H = Matlab.GetMatrix("HH", H.Zeros, true);
                    F = Matlab.GetVector("FF");
                    Matlab.Execute("clear;");

                    GC.Collect();
                }

                HDebug.Assert(H.ColSize == H.RowSize);
                HDebug.Assert(H.ColSize == F.Size);
                return new CGetHessCoarseResiIterImpl
                {
                    iterinfos = null,
                    H = H,
                    F = F,
                };
            }
        }
    }
}
