using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
	{
        public static HessInfo GetHessNMA(Tinker.Xyz xyz, Tinker.Prm prm, string tempbase)
        {
            int? digits = 16;
            string[] keys = null;
            return GetHessNMA(xyz, prm, tempbase, digits, keys);
        }
        public static HessInfo GetHessNMA(Tinker.Xyz xyz, Tinker.Prm prm, string tempbase, int? digits, params string[] keys)
        {
            var ctesthess = Tinker.Run.Testhess
                ( xyz, prm, tempbase
                , digits: digits
                , keys: keys
                );

            var hessinfo = HessInfo.FromTinker(ctesthess);
            hessinfo.numZeroEigval = 6;
            return hessinfo;

            //double[] masses = prm.atoms.SelectByXyzAtom(xyz.atoms).ListMass();
            //
            //return new HessInfo
            //{
            //    hess          = hess,
            //    mass          = masses,
            //    atoms         = xyz.atoms.HClone(),
            //    coords        = xyz.atoms.HListCoords(),
            //    numZeroEigval = 6,
            //};
        }
        public static HessInfo GetHessNMA(Universe univ, IList<Vector> coords, string tempbase, int? digits, params string[] keys)
        {
            var xyz = univ.refs["xyz"].Get<Tinker.Xyz>().CloneByCoords(coords);
            var prm = univ.refs["prm"].Get<Tinker.Prm>();
            var ctesthess = Tinker.Run.Testhess
                (xyz, prm, tempbase
                , digits: digits
                , keys: keys
                );

            var hessinfo = HessInfo.FromTinker(ctesthess);
            hessinfo.numZeroEigval = 6;
            return hessinfo;

            //return new HessInfo
            //{
            //    hess          = hess,
            //    mass          = univ.GetMasses(),
            //    atoms         = univ.atoms.ToArray(),
            //    coords        = coords.HCloneVectors().ToArray(),
            //    numZeroEigval = 6,
            //};
        }
    }
}
