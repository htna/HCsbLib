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
        public interface IMinAlign
        {
            //public static Trans3 GetTrans(IList<Vector> C1
            //                            , IList<Vector> C2
            //                            //, Pack<List<Vector>> C2new = null
            //                            )
            //{
            //    Trans3 trans = ICP3.OptimalTransform(C2, C1);
            //    if(Debug.IsDebuggerAttached)
            //    {
            //        Vector[] C2updated = trans.GetTransformed(C2);
            //        double RMSD0 = 0;
            //        double RMSD1 = 0;
            //        for(int i=0; i<C1.Count; i++)
            //        {
            //            RMSD0 += (C1[i]-C2[i]).Dist2;
            //            RMSD1 += (C1[i]-C2updated[i]).Dist2;
            //        }
            //        Debug.Assert(RMSD1 <= RMSD0);
            //    }
            //    return trans;
            //}
            //
            //public static void GetEnergies(AlignData data1, IList<Vector> coords2
            //                             , out double geo_rmsd, out double geo_enrg, out double geo_enrg_full, out double geo_enrg_anisou
            //                             , Pdb pdb2, string pdb2outpath
            //                             )
            //{
            //    Trans3 trans = GetTrans(data1.coords, coords2);
            //    List<Vector> coords2trans = new List<Vector>(trans.GetTransformed(coords2));
            //    if(pdb2 != null && pdb2outpath != null)
            //        pdb2.ToFile(pdb2outpath, coords2);
            //
            //    geo_rmsd = data1.GetRmsdFrom(coords2trans);
            //    geo_enrg = data1.GetEnergyFromDiag(coords2trans);
            //    geo_enrg_full = data1.GetEnergyFromFull(coords2trans);
            //    geo_enrg_anisou = data1.GetEnergyFromAnisou(coords2trans);
            //}
        }
    }
}
