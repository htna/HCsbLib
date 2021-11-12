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
        public static void GetInterIdList
            ( this Tinker.Xyz.Atom[] atoms
            , out HashSet<int>[] atom_id12
            , out HashSet<int>[] atom_id13
            , out HashSet<int>[] atom_id14
            , out HashSet<int>[] atom_id15
            )
        {
            HDebug.ToDo();
            Dictionary<int, Xyz.Atom> id_atom = new Dictionary<int, Xyz.Atom>(atoms.Length);
            Dictionary<int, int[]   > id_id12 = new Dictionary<int, int[]   >(atoms.Length);
            foreach(var atom in atoms)
            {
                id_atom.Add(atom.Id, atom          );
                id_id12.Add(atom.Id, atom.BondedIds);
            }

            atom_id12 = new HashSet<int>[atoms.Length];
            atom_id13 = new HashSet<int>[atoms.Length];
            atom_id14 = new HashSet<int>[atoms.Length];
            atom_id15 = new HashSet<int>[atoms.Length];
            for(int i=0; i<atoms.Length; i++)
            {
                var atom = atoms[i];
                int id1 = atom.Id;

                HashSet<int> id012    = id_id12[id1].HToHashSet(); id012.Add(id1);
                HashSet<int> id0123   = new HashSet<int>(); foreach(int id in id012  ) foreach(int i3 in id_id12[id])id0123  .Add(i3);
                HashSet<int> id01234  = new HashSet<int>(); foreach(int id in id0123 ) foreach(int i4 in id_id12[id])id01234 .Add(i4);
                HashSet<int> id012345 = new HashSet<int>(); foreach(int id in id01234) foreach(int i5 in id_id12[id])id012345.Add(i5);

                HashSet<int> id12 = id_id12[id1].HToHashSet();
                HashSet<int> id13 = id0123  .Except(id012  ).HToHashSet();
                HashSet<int> id14 = id01234 .Except(id0123 ).HToHashSet();
                HashSet<int> id15 = id012345.Except(id01234).HToHashSet();

                atom_id12[i] = id12;
                atom_id13[i] = id13;
                atom_id14[i] = id14;
                atom_id15[i] = id15;
            }
        }

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
