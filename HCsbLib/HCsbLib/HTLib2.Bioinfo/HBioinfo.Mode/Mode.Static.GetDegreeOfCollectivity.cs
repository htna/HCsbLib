using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class ModeStatic
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
        public static double GetDegreeOfCollectivity(this Mode mode, double[] masses, double[] buff_u2_ij=null)
        {
            int n = masses.Length;
            HDebug.Assert(mode.eigvec.Size == n*3);

            double[] u2_ij = buff_u2_ij;
            if(u2_ij == null || u2_ij.Length != n)
                u2_ij = new double[n];
            double   alpha = 0;
            for(int j=0; j<n; j++)
            {
                double dx = mode.eigvec[j*3+0];
                double dy = mode.eigvec[j*3+1];
                double dz = mode.eigvec[j*3+2];
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
        public static void GetDegreeOfCollectivity(string matV, string vecD, string vecM)
        {
            //  assert(matV = 3n x 3n);
            //  assert(vecD = 3n x 1 );
            //  assert(vecM =  n x 1 );
            /// doc.n   = length(vecM);
            /// doc.u2  = (matV(1:3:end,:) .^ 2);               // (n x 3n) u2 = x^2
            /// doc.u2  = doc.u2 + (matV(2:3:end,:) .^ 2);      // (n x 3n) u2 = x^2 + y^2
            /// doc.u2  = doc.u2 + (matV(3:3:end,:) .^ 2);      // (n x 3n) u2 = x^2 + y^2 + z^3
            /// doc.m   = vecM * ones(1,3*doc.n);               // (n x 3n)
            /// doc.u2  = doc.u2 ./ doc.m;                      // (n x 3n) u2 = (x^2 + y^2 + z^3) / m
            /// doc.div = sum(doc.u2);                          // (1 x 3n) alpha = sum[ (x^2 + y^2 + z^3) / m ]
            /// doc.div = ones(doc.n,1) * doc.div;              // (n x 3n)
            /// doc.u2  = doc.u2 ./ doc.div;                    // (n x 3n) u2 = 1/alpha * (x^2 + y^2 + z^3) / m
            /// doc.ki  = sum( -1 * doc.u2 .* log(doc.u2) );    // (1 x 3n) ki = sum[ -1 * u2 * log(u2) ]
            /// doc.ki  = exp(doc.ki) / doc.n;                  // (1 x 3n) ki = 1/n * exp[ sum[ -1 * u2 * log(u2) ] ]
        }
        public static void GetDegreeOfCollectivity
            ( string matV
            , string vecD
            , string vecM
            , string outvec
            , string remdoc = "remdoc"
            , bool   clear_remdoc = true
            , double? assert_threshold = null
            )
        {
            if(HDebug.IsDebuggerAttached)
            {
                //if(_RemoveDOF_IsOrthonormal(dof, assert_threshold) == false)
                //    throw new Exception("dof is not orthonormal");
            }
          //Matlab.Execute(("assert(matV = 3n x 3n);                     ").Replace("remdoc",remdoc));
          //Matlab.Execute(("assert(vecD = 3n x 1 );                     ").Replace("remdoc",remdoc));
          //Matlab.Execute(("assert(vecM =  n x 1 );                     ").Replace("remdoc",remdoc));
            Matlab.Execute(("remdoc.matV = "+matV+";                                    ").Replace("remdoc",remdoc));
            Matlab.Execute(("remdoc.vecD = "+vecD+";                                    ").Replace("remdoc",remdoc));
            Matlab.Execute(("remdoc.vecM = "+vecM+";                                    ").Replace("remdoc",remdoc));
            Matlab.Execute(("remdoc.n   = length(remdoc.vecM);                          ").Replace("remdoc",remdoc));
            Matlab.Execute(("remdoc.u2  = (remdoc.matV(1:3:end,:) .^ 2);                ").Replace("remdoc",remdoc));    // (n x 3n) u2 = x^2
            Matlab.Execute(("remdoc.u2  = remdoc.u2 + (remdoc.matV(2:3:end,:) .^ 2);    ").Replace("remdoc",remdoc));    // (n x 3n) u2 = x^2 + y^2
            Matlab.Execute(("remdoc.u2  = remdoc.u2 + (remdoc.matV(3:3:end,:) .^ 2);    ").Replace("remdoc",remdoc));    // (n x 3n) u2 = x^2 + y^2 + z^3
            Matlab.Execute(("remdoc.m   = remdoc.vecM * ones(1,3*remdoc.n);             ").Replace("remdoc",remdoc));    // (n x 3n)
            Matlab.Execute(("remdoc.u2  = remdoc.u2 ./ remdoc.m;                        ").Replace("remdoc",remdoc));    // (n x 3n) u2 = (x^2 + y^2 + z^3) / m
            Matlab.Execute(("remdoc.div =  sum(remdoc.u2);                              ").Replace("remdoc",remdoc));    // (1 x 3n) alpha = sum[ (x^2 + y^2 + z^3) / m ]
            Matlab.Execute(("remdoc.div = ones(remdoc.n,1) * remdoc.div;                ").Replace("remdoc",remdoc));    // (n x 3n)
            Matlab.Execute(("remdoc.u2  = remdoc.u2 ./ remdoc.div;                      ").Replace("remdoc",remdoc));    // (n x 3n) u2 = 1/alpha * (x^2 + y^2 + z^3) / m
            Matlab.Execute(("remdoc.ki  = sum( -1 * remdoc.u2 .* log(remdoc.u2) );      ").Replace("remdoc",remdoc));    // (1 x 3n) ki = sum[ -1 * u2 * log(u2) ]
            Matlab.Execute(("remdoc.ki  = exp(remdoc.ki) / remdoc.n;                    ").Replace("remdoc",remdoc));    // (1 x 3n) ki = 1/n * exp[ sum[ -1 * u2 * log(u2) ] ]
            Matlab.Execute((outvec+"=remdoc.ki;                                         ").Replace("remdoc",remdoc));
            if(clear_remdoc) Matlab.Execute(("clear remdoc;                             ").Replace("remdoc",remdoc));
        }
        public static IEnumerable<double> GetDegreeOfCollectivity(this IEnumerable<Mode> modes, double[] masses)
        {
            double[] buff_u2_ij = null;
            foreach(var mode in modes)
            {
                if(buff_u2_ij == null)
                {
                    int n = masses.Length;
                    HDebug.Assert(mode.eigvec.Size == n*3);
                    buff_u2_ij = new double[n];
                }
                double doc = GetDegreeOfCollectivity(mode, masses, buff_u2_ij);
                yield return doc;
            }
        }
    }
}
