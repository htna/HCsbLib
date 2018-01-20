using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Align
    {
        public class MinRMSD : IMinAlign
        {
            public static double GetRMSD(IList<Vector> C1, IList<Vector> C2)
            {
                double rmsd = 0;
                HDebug.Assert(C1.Count == C2.Count);
                for(int i=0; i<C1.Count; i++)
                    rmsd += (C1[i] - C2[i]).Dist2;
                rmsd /= C1.Count;
                rmsd = Math.Sqrt(rmsd);
                return rmsd;
            }
            public static Vector[] Align( IList<Vector> C1
                                        , IList<Vector> C2
                                        )
            {
                List<Vector> lC2 = C2.HCloneVectors().ToList();
                Align(C1, ref lC2);
                return lC2.ToArray();
            }
            public static void Align(IList<Vector> C1
                                    , ref Vector[] C2
                                    )
            {
                List<Vector> lC2 = new List<Vector>(C2);
                Align(C1, ref lC2);
                C2 = lC2.ToArray();
            }
            public static void Align(IList<Vector> C1
                                    , ref List<Vector> C2
                                    , HPack<List<Vector>> optMoveC2 = null
                                    , HPack<Trans3> outTrans = null
                                    , HPack<double> optRmsd = null
                                    )
            {
                Trans3 trans = GetTrans(C1, C2);
                Vector[] nC2 = trans.GetTransformed(C2).ToArray();
                if(optMoveC2 != null)
                {
                    optMoveC2.value = new List<Vector>(nC2.Length);
                    for(int i=0; i<nC2.Length; i++)
                        optMoveC2.value[i] = nC2[i] - C2[i];
                }
                if(optRmsd != null)
                {
                    optRmsd.value = 0;
                    for(int i=0; i<nC2.Length; i++)
                        optRmsd.value += (nC2[i] - C1[i]).Dist2;
                    optRmsd.value /= nC2.Length;
                }
                C2 = new List<Vector>(nC2);
                if(outTrans != null)
                    outTrans.value = trans.Clone();
            }
            public static Trans3 GetTrans(IList<Vector> C1
                                        , IList<Vector> C2 // ref 
                //, Pack<List<Vector>> C2new = null
                                        )
            {
                HDebug.Assert(C1.Count == C2.Count);
                Trans3 trans = ICP3.OptimalTransform(C2, C1);
                if(HDebug.IsDebuggerAttached)
                {
                    Vector[] C2updated = trans.GetTransformed(C2).ToArray();
                    double RMSD0 = 0;
                    double RMSD1 = 0;
                    for(int i=0; i<C1.Count; i++)
                    {
                        RMSD0 += (C1[i]-C2[i]).Dist2;
                        RMSD1 += (C1[i]-C2updated[i]).Dist2;
                    }
                    RMSD0 /= C1.Count;
                    RMSD1 /= C1.Count;
                    //HDebug.AssertTolerance(0.00000001, Math.Abs(RMSD1 - RMSD0));
                }
                return trans;
            }

            public static void GetEnergies(AlignData data1, IList<Vector> coords2
                                         , out double geo_rmsd, out double geo_enrg, out double geo_enrg_full, out double geo_enrg_anisou
                                         , Pdb pdb2, string pdb2outpath
                                         )
            {
                Trans3 trans = GetTrans(data1.coords, coords2);
                List<Vector> coords2trans = new List<Vector>(trans.GetTransformed(coords2).ToArray());
                if(pdb2 != null && pdb2outpath != null)
                    pdb2.ToFile(pdb2outpath, coords2);

                geo_rmsd = data1.GetRmsdFrom(coords2trans);
                geo_enrg = data1.GetEnergyFromDiag(coords2trans);
                geo_enrg_full = data1.GetEnergyFromFull(coords2trans);
                geo_enrg_anisou = data1.GetEnergyFromAnisou(coords2trans);
            }
        }
    }
}
