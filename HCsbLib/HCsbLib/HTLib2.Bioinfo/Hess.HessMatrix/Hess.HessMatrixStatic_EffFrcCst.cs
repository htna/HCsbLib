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
            return HRef.EffectivePairForceConstant.GetEffFrcCst(hess);
        }
        public static double GetEffForcCnst(this IHessMatrix hess, int bc, int br)
        {
            return HRef.EffectivePairForceConstant.GetEffFrcCst(hess, bc, br);
        }
        public static Matrix GetEffForcCnst(string matlab_hess)
        {
            string efcmat = "HessMatrixStaticGetEffForcCnst";
            HRef.EffectivePairForceConstant.GetEffFrcCst(matlab_hess, efcmat);
            Matrix efc = Matlab.GetMatrix(efcmat);
            return efc;
        }
    }
}
