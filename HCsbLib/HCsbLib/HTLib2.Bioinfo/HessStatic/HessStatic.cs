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
        public static double GetHessTraceSpringBwAtoms(this Matrix hess, int atomi, int atomj)
        {
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.RowSize % 3 == 0);
            HDebug.Assert(atomi < hess.ColSize / 3);
            HDebug.Assert(atomj < hess.ColSize / 3);

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

            double spr_ij = -1 * (bibj_trace + bjbi_trace)/2;
            return spr_ij;
        }
        static bool selftest_GetHessTraceSpringBwAtoms_hess_atomi_atomj = HDebug.IsDebuggerAttached;
        public static double GetHessTraceSpringBwAtoms(this HessMatrix hess, int atomi, int atomj)
        {
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.RowSize % 3 == 0);
            HDebug.Assert(atomi < hess.ColSize / 3);
            HDebug.Assert(atomj < hess.ColSize / 3);
            HDebug.Assert(hess.ColBlockSize == hess.RowBlockSize);
            HDebug.Assert(atomi < hess.ColBlockSize);
            HDebug.Assert(atomj < hess.ColBlockSize);

            double bibj_trace = 0;
            {
                MatrixByArr hess_ij = hess.GetBlock(atomi, atomj);
                if(hess_ij != null)
                {
                    bibj_trace += hess_ij[0, 0];
                    bibj_trace += hess_ij[1, 1];
                    bibj_trace += hess_ij[2, 2];
                }
            }
            double bjbi_trace = 0;
            {
                MatrixByArr hess_ji = hess.GetBlock(atomj, atomi);
                if(hess_ji != null)
                {
                    bjbi_trace += hess_ji[0, 0];
                    bjbi_trace += hess_ji[1, 1];
                    bjbi_trace += hess_ji[2, 2];
                }
            }

            double threshold = 0.00001;
            HDebug.Assert(Math.Abs(bibj_trace - bjbi_trace) < threshold);

            double spr_ij = -1 * (bibj_trace + bjbi_trace)/2;

            if(selftest_GetHessTraceSpringBwAtoms_hess_atomi_atomj)
            {
                selftest_GetHessTraceSpringBwAtoms_hess_atomi_atomj = false;
                double _spr_ij =  GetHessTraceSpringBwAtoms(hess.ToMatrix(), atomi, atomj);
                HDebug.Assert(Math.Abs(spr_ij - _spr_ij) < threshold);
            }

            return spr_ij;
        }

        /// https://en.wikipedia.org/wiki/Anisotropic_Network_Model
        /// 
        /// H_ij = - spring_constant / S_ij^2 * [xj - xi]
        ///                                     [yj - yi] [xj - xi,  yj - yi,  zj - zi]
        ///                                     [zj - zi]
        ///      = - spring_constant * (vj - vi).UnitVector * (vj - vi).UnitVector.Tr
        public static double GetHessAnmSpringBwAtoms(this Matrix hess, IList<Vector> coords, int atomi, int atomj)
        {

            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.RowSize % 3 == 0);
            HDebug.Assert(atomi         < hess.ColSize / 3);
            HDebug.Assert(atomj         < hess.ColSize / 3);
            HDebug.Assert(coords.Count == hess.ColSize / 3);

            int i3 = atomi * 3;
            int j3 = atomj * 3;

            Matrix hess_ij = new double[3,3]
            {
                { hess[i3+0, j3+0],  hess[i3+0, j3+1],  hess[i3+0, j3+2] },
                { hess[i3+1, j3+0],  hess[i3+1, j3+1],  hess[i3+1, j3+2] },
                { hess[i3+2, j3+0],  hess[i3+2, j3+1],  hess[i3+2, j3+2] },
            };
            Matrix hess_ji = new double[3,3]
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

            double spr_ij = -1 * (anm_spr_ij + anm_spr_ji)/2;
            return spr_ij;
        }
        static bool selftest_GetHessAnmSpringBwAtoms_hess_coords_atomi_atomj = HDebug.IsDebuggerAttached;
        public static double GetHessAnmSpringBwAtoms(this HessMatrix hess, IList<Vector> coords, int atomi, int atomj)
        {
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.RowSize % 3 == 0);
            HDebug.Assert(atomi         < hess.ColSize / 3);
            HDebug.Assert(atomj         < hess.ColSize / 3);
            HDebug.Assert(coords.Count == hess.ColSize / 3);
            HDebug.Assert(hess.ColBlockSize == hess.RowBlockSize);
            HDebug.Assert(atomi         < hess.ColBlockSize);
            HDebug.Assert(atomj         < hess.ColBlockSize);
            HDebug.Assert(coords.Count == hess.ColBlockSize);

            Vector coordij = (coords[atomj] - coords[atomi]).UnitVector();

            double anm_spr_ij = 0;
            {
                MatrixByArr hess_ij = hess.GetBlock(atomi, atomj);
                if(hess_ij != null)
                    anm_spr_ij = LinAlg.VtMV(coordij, hess_ij, coordij);
            }

            double anm_spr_ji = 0;
            {
                MatrixByArr hess_ji = hess.GetBlock(atomj, atomi);
                if(hess_ji != null)
                    anm_spr_ji = LinAlg.VtMV(coordij, hess_ji, coordij);
            }

            double threshold = 0.00001;
            HDebug.Assert(Math.Abs(anm_spr_ij - anm_spr_ji) < threshold);

            double spr_ij = -1 * (anm_spr_ij + anm_spr_ji)/2;

            if(selftest_GetHessAnmSpringBwAtoms_hess_coords_atomi_atomj)
            {
                selftest_GetHessAnmSpringBwAtoms_hess_coords_atomi_atomj = false;
                double _spr_ij =  GetHessAnmSpringBwAtoms(hess.ToMatrix(), coords, atomi, atomj);
                HDebug.Assert(Math.Abs(spr_ij - _spr_ij) < threshold);
            }

            return spr_ij;
        }
    }
}
