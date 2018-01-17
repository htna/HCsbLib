using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Mode : ICloneable
    {
    }
    public static partial class ListMode
    {
        public static bool CheckModeMassReduced(this IList<Mode> modes, double[] masses, ILinAlg la, double tolerance=0.000000001)
        {
            Vector[] mweigvecs = modes.GetMassReduced(masses.HInv()).ListEigvec().ToArray();
            MatrixByArr chkmat;
            {
                var V = la.ToILMat(mweigvecs.ToMatrix());
                var VV = V.Tr * V;
                chkmat = VV.ToArray();
                V.Dispose();
                VV.Dispose();
            }
            HDebug.Assert(chkmat.ColSize == chkmat.RowSize);
            for(int i=0; i<chkmat.ColSize; i++)
            {
                if(Math.Abs(chkmat[i,i] - 1) >= tolerance)
                {
                    /// not normal
                    return false;
                }
                chkmat[i,i] = 0;
            }
            for(int c=0; c<chkmat.ColSize; c++)
            {
                if(modes[c].eigval == 0)
                    continue;
                for(int r=0; r<chkmat.RowSize; r++)
                {
                    if(modes[r].eigval == 0)
                        continue;
                    double v = chkmat[c, r];
                    if(Math.Abs(v) >= tolerance)
                    {
                        /// not orthogonal
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
