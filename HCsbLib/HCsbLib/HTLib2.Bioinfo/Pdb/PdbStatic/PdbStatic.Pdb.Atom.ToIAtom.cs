using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class PdbStatic
    {
        public static Pdb.IAtom ToIAtom<ATOM>(this ATOM atom)
            where ATOM : Pdb.IAtom
        {
            return atom;
        }

        public static Pdb.IAtom[] ToIAtoms<ATOM>(this ATOM[] atoms)
            where ATOM : Pdb.IAtom
        {
            Pdb.IAtom[] iatoms = new Pdb.IAtom[atoms.Length];
            for(int i = 0; i < atoms.Length; i++)
                iatoms[i] = atoms[i];
            return iatoms;
        }

        public static List<Pdb.IAtom> ToIAtoms<ATOM>(this List<ATOM> atoms)
            where ATOM : Pdb.IAtom
        {
            List<Pdb.IAtom> iatoms = new List<Pdb.IAtom>(atoms.Count);
            for(int i = 0; i < atoms.Count; i++)
                iatoms.Add(atoms[i]);
            return iatoms;
        }

        public static IList<Pdb.IAtom> ToIAtoms<ATOM>(this IList<ATOM> atoms)
            where ATOM : Pdb.IAtom
        {
            Pdb.IAtom[] iatoms = new Pdb.IAtom[atoms.Count];
            for(int i = 0; i < atoms.Count; i++)
                iatoms[i] = atoms[i];
            return iatoms;
        }
    }
}
