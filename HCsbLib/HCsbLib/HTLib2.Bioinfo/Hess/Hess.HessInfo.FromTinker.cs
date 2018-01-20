using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public partial class HessInfo : HessInfoBase
        {
            public static HessInfo FromTinker(Tinker.Xyz xyz, Tinker.Prm prm, HessMatrix testhess)
            {
                var hessinfo = new HessInfo
                {
                    coords = xyz.atoms.HListCoords(),
                    hess   = testhess,
                    mass   = prm.atoms.SelectByXyzAtom(xyz.atoms).ListMass(),
                    atoms  = xyz.atoms,
                };
                return hessinfo;
            }
        }
    }
}
