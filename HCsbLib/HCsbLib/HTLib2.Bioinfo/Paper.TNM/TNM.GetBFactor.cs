using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;
    using RotableInfo = Universe.RotableInfo;
public static partial class Paper
{
    public partial class TNM
    {
        public static Vector GetBFactor(Universe univ, HessMatrix hessian, MatrixByArr J)
        {
            Vector masses = univ.GetMasses();
            Mode[] modes = GetModeByTorsional(hessian, masses, J);

            Vector bfactor = new double[univ.size];
            {
                int m = modes.Length;
                for(int i=0; i<univ.size; i++)
                {
                    bfactor[i] = 0;
                    for(int idx=0; idx<m; idx++)
                    {
                        double dx = modes[idx].eigvec[i*3+0]; // tormodes[idx][i*3+0];
                        double dy = modes[idx].eigvec[i*3+1]; // tormodes[idx][i*3+1];
                        double dz = modes[idx].eigvec[i*3+2]; // tormodes[idx][i*3+2];
                        bfactor[i] += (dx*dx + dy*dy + dz*dz) / modes[idx].eigval; // (dx*dx + dy*dy + dz*dz) / toreigvals[idx];
                    }
                }
            }
            return bfactor;
        }
    }
}
}
