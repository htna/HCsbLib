using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HessMatrixStatic
    {
        // The Effective Pair Force-Constant
        // HinsenK, PetrescuA-J, Dellerue S, Bellissent-Funel M-C and Kneller G R 2000 Harmonicity in slow protein dynamics Chem. Phys. 261 25–37
        public static Matrix GetEffForcCnst(this IHessMatrix hess)
        {
            return References.EffectiveForceConstant.GetEffFrcCst(hess);
        }
        public static double GetEffForcCnst(this IHessMatrix hess, int bc, int br)
        {
            return References.EffectiveForceConstant.GetEffFrcCst(hess, bc, br);
        }
        public static Matrix GetEffForcCnst(string matlab_hess)
        {
            return References.EffectiveForceConstant.GetEffFrcCst(matlab_hess);
        }
    }
}
