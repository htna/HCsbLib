using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    using Vel     = Tinker.Vel;
    public static partial class TinkerStatic
    {
        public static Dictionary<int, Tinker.Vel.Atom> ToIdDictionary(this IEnumerable<Tinker.Vel.Atom> atoms)
        {
            Dictionary<int, Tinker.Vel.Atom> dict = new Dictionary<int, Tinker.Vel.Atom>();
            foreach(Tinker.Vel.Atom atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public static Tinker.Vel.Atom[] HSelectCorrectAtomType(this IList<Tinker.Vel.Atom> atoms)
        {
            List<Tinker.Vel.Atom> sels = new List<Tinker.Vel.Atom>();
            foreach(var atom in atoms)
                if(atom.AtomType.Trim().Length != 0)
                    sels.Add(atom);
            return sels.ToArray();
        }
        public static Vector[] HListVelXYZ(this IList<Tinker.Vel.Atom> atoms)
        {
            Vector[] coords = new Vector[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                coords[i] = atoms[i].VelXYZ;
            return coords;
        }
        public static IList<int> HListId(this IList<Tinker.Vel.Atom> atoms)
        {
            int[] ids = new int[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                ids[i] = atoms[i].Id;
            return ids;
        }
        public static IEnumerable<int> HEnumId(this IEnumerable<Tinker.Vel.Atom> atoms)
        {
            foreach(var atom in atoms)
                yield return atom.Id;
        }
        public static Dictionary<int, Tinker.Vel.Atom> HToDictionaryIdAtom
            ( this IEnumerable<Tinker.Vel.Atom> atoms
            )
        {
            Dictionary<int, Tinker.Vel.Atom> dict = new Dictionary<int, Tinker.Vel.Atom>();
            foreach(var atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public static Dictionary<int, int> HToDictionaryIdIndex
            ( this IList<Tinker.Vel.Atom> atoms
            )
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for(int idx=0; idx<atoms.Count(); idx++)
            {
                int id = atoms[idx].Id;
                dict.Add(id, idx);
            }
            return dict;
        }
        public static IEnumerable<Tinker.Vel.Atom> HSelectByAtomType(this IEnumerable<Tinker.Vel.Atom> atoms, string AtomType)
        {
            foreach(var atom in atoms)
            {
                if(atom.AtomType == AtomType)
                    yield return atom;
            }
        }
    }
}
