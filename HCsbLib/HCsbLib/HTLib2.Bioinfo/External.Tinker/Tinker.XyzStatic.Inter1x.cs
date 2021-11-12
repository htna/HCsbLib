using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    using Xyz     = Tinker.Xyz;
    public static partial class TinkerStatic
    {
        public static bool IsInter12(this Tinker.Xyz.Atom atom0, Tinker.Xyz.Atom atom1)
        {
            bool inter12 = atom0.BondedIds.Contains(atom1.Id);
            HDebug.Assert( atom1.BondedIds.Contains(atom0.Id) == inter12);
            return inter12;
        }
        public static bool IsInter123(this Tinker.Xyz.Atom atom0, Tinker.Xyz.Atom atom1)
        {
            var atom0bonds = atom0.BondedIds;
            var atom1bonds = atom1.BondedIds;
            //return (atom0bonds.Intersect(atom1bonds).Count() > 0);
            foreach(int id0 in atom0bonds)
                foreach(int id1 in atom1bonds)
                    if(id0 == id1)
                        return true;
            return false;
        }
        public static bool IsInter1234
            ( this Tinker.Xyz.Atom atom0
            , Tinker.Xyz.Atom atom1
            , Dictionary<int, Tinker.Xyz.Atom> id2atom  // = atoms.HToType<object, Tinker.Xyz.Atom>().ToIdDictionary();
            )
        {
            int id1 = atom0.Id;
            var id2s = atom0.BondedIds;
            var id3s = atom1.BondedIds;
            int id4 = atom1.Id;
            foreach(int id2 in id2s)
            {
                foreach(int id3 in id2atom[id2].BondedIds)
                {
                    if(id3s.Contains(id3))
                        return true;
                }
            }
            return false;
        }
    }
}
