using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class References
    {
        // The Effective Pair Force-Constant
        // HinsenK, PetrescuA-J, Dellerue S, Bellissent-Funel M-C and Kneller G R 2000 Harmonicity in slow protein dynamics Chem. Phys. 261 25–37
        public class EffectiveForceConstant
        {
            public static Matrix GetEffFrcCst(IHessMatrix hess)
            {
                int bcs = hess.ColBlockSize;
                int brs = hess.RowBlockSize;
                Matrix efc = Matrix.Zeros(bcs,brs);
                foreach(var bc_br_bval in hess.EnumBlocks())
                {
                    int bc  = bc_br_bval.Item1;
                    int br  = bc_br_bval.Item2;
                    var blk = bc_br_bval.Item3;
                    HDebug.Assert(blk != null);
                    HDebug.Assert(blk.HAbsMax() == 0);
                    double efc_bcbr = blk[0,0] + blk[1,1] + blk[2,2];
                    efc[bc,br] = efc_bcbr;
                }
                return efc;
            }
            public static double GetEffFrcCst(IHessMatrix hess, int bc, int br)
            {
                var blk = hess.GetBlock(bc, br);
                if(blk == null)
                    return 0;
                double efc_bcbr = blk[0,0] + blk[1,1] + blk[2,2];
                return efc_bcbr;
            }
            // reference for matlab code
            public static Matrix GetEffFrcCst(string matlab_hess)
            {
                // HMSGEFC : (H)ess(M)atrix(S)tatic.(G)et(E)ffective(F)orce(C)onstants
                Matlab.Execute("HMSGEFC.hess = "+matlab_hess+";");
                Matlab.Execute("HMSGEFC.efc11 = HMSGEFC.hess(1:3:end, 1:3:end);");
                Matlab.Execute("HMSGEFC.efc22 = HMSGEFC.hess(2:3:end, 2:3:end);");
                Matlab.Execute("HMSGEFC.efc33 = HMSGEFC.hess(3:3:end, 3:3:end);");
                Matlab.Execute("HMSGEFC.efc   = HMSGEFC.efc11 + HMSGEFC.efc22 + HMSGEFC.efc22;");
                Matrix efc = Matlab.GetMatrix("HMSGEFC.efc");
                Matlab.Execute("clear HMSGEFC;");
                return efc;
            }
        }
    }
}
