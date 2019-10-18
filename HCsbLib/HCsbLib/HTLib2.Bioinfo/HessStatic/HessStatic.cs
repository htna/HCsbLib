using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public static partial class HessStatic
    {
        public static double HessGetSumSpringBwAtoms(this Matrix hess, int atomi, int atomj)
        {
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.RowSize % 3 == 0);
            HDebug.Assert(hess.ColSize / 3 < atomi);
            HDebug.Assert(hess.ColSize / 3 < atomj);

            double bibj_trace = 0;
            {
                bibj_trace += hess[atomi*3+0, atomj*3+0];
                bibj_trace += hess[atomi*3+1, atomj*3+1];
                bibj_trace += hess[atomi*3+2, atomj*3+2];
            }
            double bjbi_trace = 0;
            {
                bjbi_trace += hess[atomj*3+0, atomi*3+0];
                bjbi_trace += hess[atomj*3+1, atomi*3+1];
                bjbi_trace += hess[atomj*3+2, atomi*3+2];
            }
            double threshold = 0.00001;
            HDebug.Assert(Math.Abs(bibj_trace - bjbi_trace) < threshold);
            return (bibj_trace + bjbi_trace)/2;
        }
        public static double HessGetSumSpringBwAtoms(this Matrix hess, IList<Vector> coords, int atomi, int atomj)
        {
            // https://en.wikipedia.org/wiki/Anisotropic_Network_Model
            // 
            // H_ij = - spring_constant / S_ij^2 * [xj - xi]
            //                                     [yj - yi] [xj - xi,  yj - yi,  zj - zi]
            //                                     [zj - zi]
            //      = - spring_constant * (vj - vi).UnitVector * (vj - vi).UnitVector.Tr

            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.RowSize % 3 == 0);
            HDebug.Assert(hess.ColSize / 3 < atomi);
            HDebug.Assert(hess.ColSize / 3 < atomj);
            HDebug.Assert(atomi < coords.Count);
            HDebug.Assert(atomj < coords.Count);

            int i3 = atomi * 3;
            int j3 = atomj * 3;

            double[,] hess_ij = new double[3,3]
            {
                { hess[i3+0, j3+0],  hess[i3+0, j3+1],  hess[i3+0, j3+2] },
                { hess[i3+1, j3+0],  hess[i3+1, j3+1],  hess[i3+1, j3+2] },
                { hess[i3+2, j3+0],  hess[i3+2, j3+1],  hess[i3+2, j3+2] },
            };
            double[,] hess_ji = new double[3,3]
            {
                { hess[j3+0, i3+0],  hess[j3+0, i3+1],  hess[j3+0, i3+2] },
                { hess[j3+1, i3+0],  hess[j3+1, i3+1],  hess[j3+1, i3+2] },
                { hess[j3+2, i3+0],  hess[j3+2, i3+1],  hess[j3+2, i3+2] },
            };

            Vector coordij = (coords[atomj] - coords[atomi]).UnitVector();

            double anm_spr_ij = LinAlg.VtMV(coordij, hess_ij, coordij);
            double anm_spr_ji = LinAlg.VtMV(coordij, hess_ji, coordij);

            double threshold = 0.00001;
            HDebug.Assert(Math.Abs(anm_spr_ij - anm_spr_ji) < threshold);
            return (anm_spr_ij - anm_spr_ji)/2;
        }
    }
}
