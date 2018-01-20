using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static Vector GetFrcByFij(IList<Vector> coords, double[,] F, Vector frc=null)
        {
            int size = coords.Count;
            if(frc == null)
                frc = new double[size*3];
        /// Parallel.For(0, size, delegate(int i)
            for(int i=0; i<size; i++)
            {
                for(int j=i; j<size; j++)
                {
                    if(i == j)
                        continue;
                    if(F[i, j] == 0)
                        continue;

                    HDebug.AssertTolerance(0.00000001, F[i,j] - F[j,i]);
                    Vector frc_i = GetFrciByFij(coords[i], coords[j], F[i, j]);

                    int i3 = i*3;             int j3 = j*3;
                    frc[i3+0] += frc_i[0];    frc[j3+0] += -1*frc_i[0];
                    frc[i3+1] += frc_i[1];    frc[j3+1] += -1*frc_i[1];
                    frc[i3+2] += frc_i[2];    frc[j3+2] += -1*frc_i[2];
                }
            }
        /// );
            return frc;
        }
        public static Vector GetFrciByFij(Vector coordi, Vector coordj, double Fij)
        {
            /// unit vector i->j
            Vector uvec_ij = coordj - coordi;
            uvec_ij = uvec_ij / uvec_ij.Dist;

            /// attractive, positive pw-force
            /// repulsive , negative pw-force
            /// 
            /// fij(+) attractive
            /// fij(-) repulsive
            Vector frc_i = Fij * uvec_ij.Clone();
            return frc_i;
        }
    }
}
