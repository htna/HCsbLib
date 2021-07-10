using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class TinkerExt
    {
        public static Tinker.Xyz CloneXyzWithProtOnly(this Tinker.Xyz xyz)
        {
            List<Tinker.Xyz.Atom> protein = new List<Tinker.Xyz.Atom>();
            List<Tinker.Xyz.Atom> waters  = new List<Tinker.Xyz.Atom>();
            foreach(var atom in xyz.atoms)
            {
                if(atom.AtomType == "OT " || atom.AtomType == "HT ")
                    waters.Add(atom);
                else
                    protein.Add(atom);
            }

            var xyz_nowater = xyz.CloneByRemoveIds(waters.HListId());
            var xyz_prot    = xyz_nowater.CloneByReindex(1);

            return xyz_prot;
        }
    }
}
