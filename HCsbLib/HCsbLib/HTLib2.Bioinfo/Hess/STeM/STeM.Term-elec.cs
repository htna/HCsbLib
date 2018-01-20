using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        /// <summary>
        /// Nonbonded Term (Van der Waals)
        /// V = 332 * pchij / (r_ij * ee)
        ///   ~ (332 * pchij / ee) * [ 1/r0_ij   -   (r_ij - r0_ij)/r0_ij^2   +   (r_ij - r0_ij)^2/r0_ij^3 ]
        ///   = (332 * pchij / ee * (2 /  r0_ij  ))
        ///   + (332 * pchij / ee * (1 / -r0_ij^2)) *  r_ij
        ///   + (332 * pchij / ee * (1 /  r0_ij^3)) * (r_ij - r0_ij)^2
        ///   
        /// where   (332 * pchij / ee * (1 /  r0_ij^3)) * (r_ij - r0_ij)^2
        ///         = 60 * (332/60 * pchij / ee / r0_ij) / r0_ij^2 * (r_ij - r0_ij)^2
        ///         = 60 * Epsilon                       / r0_ij^2 * (r_ij - r0_ij)^2
        ///         => same to vdW term in (36) with fixing bug (120 -> 60)
        ///     
        /// dVV/dxdy = FourthTerm with "Epsilon = (332/60 * pchij / ee / r0_ij)"
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="Epsilon"></param>
        /// <returns></returns>
        private static void TermElec(IList<Vector> caArray, double pchi, double pchj, double ee, HessMatrix hessian, int i, int j)
        {
            double r0_ij = (caArray[i] - caArray[j]).Dist;
            double pchij = pchi * pchj;
            double Epsilon = (332.0/60.0 * pchij / ee / r0_ij);
            FourthTerm(caArray, Epsilon, hessian, i, j);
        }
    }
}
}
