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
        public class MinWRMSD : IMinAlign
        {
            public static double GetWRMSD(List<Vector> C1, List<Vector> C2, IList<double> weight)
            {
                double rmsd = 0;
                HDebug.Assert(C1.Count == C2.Count);
                for(int i=0; i<C1.Count; i++)
                    rmsd += (C1[i] - C2[i]).Dist2 * weight[i];
                rmsd /= weight.Sum();
                rmsd = Math.Sqrt(rmsd);
                return rmsd;
            }
            public static Vector[] Align(IList<Vector> C1
                                        , Vector[] C2
                                        , IList<double> weight
                                        )
            {
                List<Vector> lC2 = C2.HCloneDeep().ToList();
                Align(C1, ref lC2, weight);
                return lC2.ToArray();
            }
            public static void Align(IList<Vector> C1
                                    , ref Vector[] C2
                                    , IList<double> weight
                                    )
            {
                List<Vector> lC2 = new List<Vector>(C2);
                Align(C1, ref lC2, weight);
                C2 = lC2.ToArray();
            }
            public static void Align(IList<Vector> C1
                                    , ref List<Vector> C2
                                    , IList<double> weight
                                    , HPack<Trans3> outTrans = null
                                    )
            {
                Trans3 trans = GetTrans(C1, C2, weight);
                Vector[] nC2 = trans.GetTransformed(C2).ToArray();
                C2 = new List<Vector>(nC2);
                if(outTrans != null)
                    outTrans.value = trans.Clone();
            }
            public static Trans3 GetTrans(IList<Vector> C1
                                        , IList<Vector> C2
                                        , IList<double> weight
                //, Pack<List<Vector>> C2new = null
                                        )
            {
                if(HDebug.Selftest())
                {
                    double[] tweight = new double[weight.Count];
                    for(int i=0; i<tweight.Length; i++)
                        tweight[i] = 0.2;
                    Trans3 ttrans0 = GetTrans(C1, C2, tweight);
                    Trans3 ttrans1 = MinRMSD.GetTrans(C1, C2);
                    HDebug.AssertTolerance(0.0001, ttrans0.ds - ttrans1.ds);
                    HDebug.AssertTolerance(0.0001, ttrans0.dt - ttrans1.dt);
                    HDebug.AssertTolerance(0.0001, new Vector(ttrans0.dr.ToArray()) - ttrans1.dr.ToArray());
                    HDebug.AssertTolerance(0.0001, (ttrans0.TransformMatrix - ttrans1.TransformMatrix));
                }
                HDebug.Assert(C1.Count == C2.Count);
                HDebug.Assert(C1.Count == weight.Count);
                //Trans3 trans = ICP3.OptimalTransform(C2, C1);
                Trans3 trans = ICP3.OptimalTransformWeighted(C2, C1, weight);
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
                    //Debug.AssertTolerance(0.00000001, Math.Abs(RMSD1 - RMSD0));
                }
                return trans;
            }
        }
    }
}
