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
        public class MinPackingDensity : IMinAlign
        {
            public static void GetEnergies(AlignData data1, IList<Vector> coords2
                                              , out double pot_rmsd, out double pot_enrg, out double pot_enrg_full, out double pot_enrg_anisou
                                              , Pdb pdb2 = null
                                              , string pdb2outpath = null
                                              )
            {
                HDebug.Assert(false);
                //Bioinfo.Anisou[] anisous1;// = data1.GetAnisous();
                //double[] bfactor1;
                //{
                //    List<Bioinfo.Mode> modes = new List<Bioinfo.Mode>(data1.GetModes());
                //    for(int i=0; i<6; i++)
                //        modes.RemoveAt(0);
                //    bfactor1 = Bioinfo.GetBFactor(modes.ToArray(), data1.masses);
                //    Debug.Assert(data1.size == bfactor1.Length);
                //    anisous1 = Bioinfo.GetAnisou(bfactor1, scale: 10000*1000);
                //}
                ////Trans3 trans = MinAnisou.GetTrans(data1.coords, anisous1, coords2);
                //Trans3 trans = GetTrans(data1.coords, bfactor1, coords2);
                //List<Vector> coords2trans = new List<Vector>(trans.GetTransformed(coords2));
                //if(pdb2 != null && pdb2outpath != null)
                //    data1.pdb.ToFile(pdb2outpath, coords2trans, anisous: anisous1.GetUs());
                //
                //pot_rmsd = data1.GetRmsdFrom(coords2trans);
                //pot_enrg = data1.GetEnergyFromDiag(coords2trans);
                //pot_enrg_full = data1.GetEnergyFromFull(coords2trans);
                //pot_enrg_anisou = data1.GetEnergyFromAnisou(coords2trans);
                pot_rmsd        = double.NaN;
                pot_enrg        = double.NaN;
                pot_enrg_full   = double.NaN;
                pot_enrg_anisou = double.NaN;
            }

            public static Trans3 GetTrans(IList<Vector> C1, IList<Vector> C2)
            {
                HDebug.Assert(C1.Count == C2.Count);
                int size = C1.Count;

                double[] pakdens1 = new double[size];
                for(int i=0; i<size-1; i++)
                    for(int j=i+1; j<size; j++)
                    {
                        double dist2 = (C1[i] - C1[j]).Dist2;
                        pakdens1[i] += 1/dist2;
                        pakdens1[j] += 1/dist2;
                    }

                Trans3 trans = Geometry.AlignPointPoint.MinRMSD.GetTrans(C2, C1, pakdens1);
                return trans;
            }
        }
    }
}
