using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class UniverseStatic
    {
        public static IList<Universe.Atom[]> ListAtoms(this Universe.Molecule[] moles)
        {
            List<Universe.Atom[]> lstatoms = new List<Universe.Atom[]>();
            foreach(var mole in moles)
                lstatoms.Add(mole.atoms.ToArray());
            return lstatoms;
        }
    }
	public partial class Universe
	{
        public partial class Molecule
        {
            public HashSet<Atom> atoms;
            public HashSet<Bond> bonds;
            public int[] GetAtomIDs()
            {
                int[] ids = atoms.ToArray().ListIDs().ToArray();
                return ids;
            }

            public override string ToString()
            {
                string msg = string.Format("{0} atoms, {1} bonds", atoms.Count, bonds.Count);
                return msg;
            }
        }
        public Molecule GetMolecule(Atom atom)
        {
            Molecule mole = new Molecule();
            mole.atoms = new HashSet<Atom>();
            mole.bonds = new HashSet<Bond>();
            GetMoleculeRec(mole, atom);
            return mole;
        }
        void GetMoleculeRec(Molecule mole, Atom atom)
        {
            if(mole.atoms.Contains(atom))
                return;
            mole.atoms.Add(atom);
            foreach(var bond in atom.Bonds)
            {
                if(mole.bonds.Contains(bond))
                    continue;
                HDebug.Verify(mole.bonds.Add(bond));
                HDebug.Assert(bond.atoms.Length == 2);
                Atom batom = (bond.atoms[0] == atom) ? bond.atoms[1] : bond.atoms[0];
                HDebug.Assert(object.ReferenceEquals(atom, batom) == false);
                GetMoleculeRec(mole, batom);
            }
        }
        public Molecule GetMolecule()
        {
            Molecule[] moles = GetMolecules();
            if(moles.Length != 1)
                throw new Exception("more than one mole when calling GetMolecule()");
            return moles[0];
        }
        public Molecule[] GetMolecules()
        {
            List<Molecule> moles = new List<Molecule>();
            HashSet<Atom> latoms = new HashSet<Atom>(atoms.ToArray());

            while(latoms.Count > 0)
            {
                Molecule mole = GetMolecule(latoms.First());
                foreach(var matom in mole.atoms)
                    HDebug.Verify(latoms.Remove(matom));
                moles.Add(mole);
            }

            if(HDebug.IsDebuggerAttached)
            {
                HashSet<Atom> datoms = new HashSet<Atom>();
                HashSet<Bond> dbonds = new HashSet<Bond>();
                foreach(var mole in moles)
                {
                    foreach(var atom in mole.atoms) datoms.Add(atom);
                    foreach(var bond in mole.bonds) dbonds.Add(bond);
                }
                HDebug.Assert(datoms.Count == atoms.Count);
                HDebug.Assert(dbonds.Count == bonds.Count);
            }

            return moles.ToArray();
        }
    }
}
