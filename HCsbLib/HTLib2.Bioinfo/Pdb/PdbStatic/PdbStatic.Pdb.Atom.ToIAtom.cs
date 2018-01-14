using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom  = Pdb.Atom;
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static IAtom ToIAtom<ATOM>(this ATOM atom)
            where ATOM:IAtom
        {
            return atom;
        }

        public static IAtom[] ToIAtoms<ATOM>(this ATOM[] atoms)
            where ATOM : IAtom
        {
            IAtom[] iatoms = new IAtom[atoms.Length];
            for(int i = 0; i < atoms.Length; i++)
                iatoms[i] = atoms[i];
            return iatoms;
        }

        public static List<IAtom> ToIAtoms<ATOM>(this List<ATOM> atoms)
            where ATOM : IAtom
        {
            List<IAtom> iatoms = new List<IAtom>(atoms.Count);
            for(int i = 0; i < atoms.Count; i++)
                iatoms.Add(atoms[i]);
            return iatoms;
        }

        public static IList<IAtom> ToIAtoms<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            IAtom[] iatoms = new IAtom[atoms.Count];
            for(int i = 0; i < atoms.Count; i++)
                iatoms[i] = atoms[i];
            return iatoms;
        }
    }
}
