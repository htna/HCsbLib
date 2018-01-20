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
        public class MinBFactor : IMinAlign
        {
            public static void GetEnergies(AlignData data1, IList<Vector> coords2
                                              , out double pot_rmsd, out double pot_enrg, out double pot_enrg_full, out double pot_enrg_anisou
                                              , Pdb pdb2 = null
                                              , string pdb2outpath = null
                                              )
            {
                Anisou[] anisous1;// = data1.GetAnisous();
                double[] bfactor1;
                {
                    List<Mode> modes = new List<Mode>(data1.GetModes());
                    for(int i=0; i<6; i++)
                        modes.RemoveAt(0);
                    bfactor1 = HBioinfo.GetBFactor(modes.ToArray(), data1.masses);
                    HDebug.Assert(data1.size == bfactor1.Length);
                    anisous1 = Anisou.FromBFactor(bfactor1, scale: 10000*1000);
                }
                //Trans3 trans = MinAnisou.GetTrans(data1.coords, anisous1, coords2);
                Trans3 trans = GetTrans(data1.coords, bfactor1, coords2);
                List<Vector> coords2trans = new List<Vector>(trans.GetTransformed(coords2));
                if(pdb2 != null && pdb2outpath != null)
                    data1.pdb.ToFile(pdb2outpath, coords2trans, anisous: anisous1.GetUs());

                pot_rmsd = data1.GetRmsdFrom(coords2trans);
                pot_enrg = data1.GetEnergyFromDiag(coords2trans);
                pot_enrg_full = data1.GetEnergyFromFull(coords2trans);
                pot_enrg_anisou = data1.GetEnergyFromAnisou(coords2trans);
            }

            public static Anisou[] BFactor2Anisou(IList<double> bfactor, double scale=10000*1000)
            {
                return Anisou.FromBFactor(bfactor.ToArray(), scale: scale);
            }

            public static void Align(IList<Vector> C1, IList<double> bfactor, ref List<Vector> C2)
            {
                Trans3 trans = GetTrans(C1, bfactor, C2);
                for(int i=0; i<C2.Count; i++)
                    C2[i] = trans.DoTransform(C2[i]);
            }
            public static Trans3 GetTrans(IList<Vector> C1, IList<double> bfactor, IList<Vector> C2)
            {
                Trans3 trans;

                double[] weight = new double[bfactor.Count];
                {
                    for(int i=0; i<weight.Length; i++)
                        weight[i] = 1.0 / bfactor[i];
                }
                {
                    int size = C1.Count;

                    Vector MassCenter1 = new double[3];
                    Vector MassCenter2 = new double[3];
                    {
                        double weight_sum = 0;
                        for(int i=0; i<size; i++)
                        {
                            weight_sum += weight[i];
                            MassCenter1 += weight[i] * C1[i];
                            MassCenter2 += weight[i] * C2[i];
                        }
                        MassCenter1 = MassCenter1 / weight_sum;
                        MassCenter2 = MassCenter2 / weight_sum;
                    }
                    Vector[] nc1 = new Vector[size];
                    Vector[] nc2 = new Vector[size];
                    {
                        for(int i=0; i<size; i++)
                        {
                            nc1[i] = C1[i] - MassCenter1;
                            nc2[i] = C2[i] - MassCenter2;
                        }
                    }
                    Quaternion rot = ICP3.OptimalRotationWeighted(nc2, nc1, weight);
                    Quaternion urot = Quaternion.UnitRotation;

                    Trans3 trans1 = new Trans3(-MassCenter2, 1, urot);
                    Trans3 trans2 = new Trans3(new double[3], 1, rot);
                    Trans3 trans3 = new Trans3(MassCenter1, 1, urot);

                    trans = trans1;
                    trans = Trans3.AppendTrans(Trans3.AppendTrans(trans1, trans2), trans3);

                    HDebug.AssertTolerance(0.00000001, trans.DoTransform(MassCenter2)-MassCenter1);
                    return trans;
                }

                {
                    trans = ICP3.OptimalTransformWeighted(C2, C1, weight);
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
                        HDebug.Assert(RMSD1 <= RMSD0);
                    }
                    return trans;
                }
            }
        }
    }
}
