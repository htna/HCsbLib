using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class TinkerExt
    {
        public static IEnumerable<Tinker.Xyz.Atom> EnumProtAtoms(this Tinker.Xyz xyz)
        {
            foreach(var atom in xyz.atoms)
            {
                if(atom.AtomType == "OT " || atom.AtomType == "HT ")
                    continue;
                else
                    yield return atom;
            }
        }
        public static IEnumerable<Tinker.Xyz.Atom> EnumSolvAtoms(this Tinker.Xyz xyz)
        {
            foreach(var atom in xyz.atoms)
            {
                if(atom.AtomType == "OT " || atom.AtomType == "HT ")
                    yield return atom;
                else
                    continue;
            }
        }
    }
}
