using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Coarse
    {
        public static partial class CoarseHessForc
        {
            [Serializable]
            public class HessForcInfo
            {
                public object[]     atoms   = null;
                public Vector       mass    = null;
                public Vector[]     coords  = null;
                public HessMatrix   hess    = null;
                public Vector[]     forc    = null;

                public static HessForcInfo From(Hess.HessInfo hessinfo)
                {
                    return new HessForcInfo{
                        atoms  = hessinfo.atoms ,
                        mass   = hessinfo.mass  ,
                        coords = hessinfo.coords,
                        hess   = hessinfo.hess  ,
                    };
                }
                public static HessForcInfo From(Hess.HessInfo hessinfo, Vector[] forc)
                {
                    return new HessForcInfo{
                        atoms  = hessinfo.atoms ,
                        mass   = hessinfo.mass  ,
                        coords = hessinfo.coords,
                        hess   = hessinfo.hess  ,
                        forc   = forc           ,
                    };
                }
                public Hess.HessInfo GetHessInfo()
                {
                    return new Hess.HessInfo
                    {
                        mass    = mass,
                        atoms   = atoms,
                        coords  = coords,
                        hess    = hess,
                        numZeroEigval = null,
                    };
                }
            }
        }
    }
}
