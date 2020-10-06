using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Mode
    {
        public double GetDegreeOfCollectivity(double[] masses, double[] buff_u2_ij=null)
        {
            /// Rafael Bruschweilera
            /// Collective protein dynamics and nuclear spin relaxation
            /// J. Chem. Phys. 102, 3396--3403 (1995); https://doi.org/10.1063/1.469213
            /// 
            /// Degree of Collectivity (k_i) of i-th mode
            ///     k_i = 1/N exp( -\sum_{n=1}^N  u_{i,j}^2  log u_{i,j}^2 )
            /// where 
            ///     u_{i,n}^2 = \alpha * ( Q_{i,3n-2}^2 + Q_{i,3n-1}^2 + Q_{i,3n-0}^2 ) / m_n
            ///     \alpha = \sum_{i=1)^N u_{i,n}^2 = 1
            ///     m_i is atomic mass
            /// where
            ///     N                                   : number of atoms
            ///     m_i                                 : mass of i-th atom
            ///     Q_i                                 : i-th mode in mass-reduced space, M^{-1/2} K M^{-1/2} Q_i = w_i^2 Q_i
            ///     Q_{i,3n-2}, Q_{i,3n-1}, Q_{i,3n-0}  : x,y,z mode component of n-th atom of i-th mode
            ///     u_{i,j}                             : normalized squared Cartesian motion of j-th atom in i-th mode
            /// 
            /// If k_i = 1, mode i is maximally collective and all amplitudes u_{i,n}^2 are identical, which is the case
            /// for the three modes describing global translation.
            /// 
            /// In the limit of extreme local motion, where a mode affects a single atom only, k_i is nimimal and equal to 1/N.
            int n = masses.Length;
            HDebug.Assert(eigvec.Size == n*3);

            double[] u2_ij = buff_u2_ij;
            if(u2_ij == null || u2_ij.Length != n)
                u2_ij = new double[n];
            double   alpha = 0;
            for(int j=0; j<n; j++)
            {
                double dx = eigvec[j*3+0];
                double dy = eigvec[j*3+1];
                double dz = eigvec[j*3+2];
                double u2_ij_ = (dx*dx + dy*dy + dz*dz)/masses[j];
                alpha   += u2_ij_;
                u2_ij[j] = u2_ij_;
            }
            for(int j=0; j<n; j++)
                u2_ij[j] = u2_ij[j] / alpha;
            
            double k_i = 0;
            for(int j=0; j<n; j++)
                k_i += u2_ij[j] * Math.Log(u2_ij[j]);
            k_i = (1.0 / n) * Math.Exp(-1 * k_i);

            HDebug.Assert(k_i <= 1.0  ); // maximal collective
            HDebug.Assert(k_i >= 1.0/n); // extreme local motion, where a mode affects a single atom only

            return k_i;
        }
    }
}
