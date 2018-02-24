using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;

	public partial class Universe
	{
        public class Atoms
        {
            Universe univ;
            List<Atom> atoms = new List<Atom>();
            public int                   Count           { get { return atoms.Count;  } }
            public Atom                  this[int index] { get { return atoms[index]; } }
            public List<Atom>.Enumerator GetEnumerator() { return atoms.GetEnumerator(); }
            public Atom[]                ToArray()       { return atoms.ToArray(); }

            //HashSet<Atom> _dbgatoms = (HDebug.IsDebuggerAttached ? new HashSet<Atom>() : null);
            public Atoms(Universe univ)
            {
                this.univ = univ;
            }

            public void Add(Atom atom)
            {
                int id=Count;
                //HDebug.Assert(atoms.Contains(atom) == false);
                //if(HDebug.IsDebuggerAttached)
                //{
                //    HDebug.Assert(_dbgatoms.Contains(atom) == false);
                //    _dbgatoms.Add(atom);
                //}
                atoms.Add(atom);
                atom.AssignID(id);
            }
            public void Remove(Atom atom)
            {
                HDebug.Assert(Verify());
                HDebug.Assert(atom == atoms[atom.ID]);
                atom.Isolate(univ);
                atoms.RemoveAt(atom.ID);
                for(int id=0; id<atoms.Count; id++)
                {
                    atoms[id].AssignID(id);
                }
            }
            public bool Verify()
            {
                for(int id=0; id<atoms.Count; id++)
                {
                    if(atoms[id].ID != id)
                        return false;
                }
                return true;
            }
            public Atom[] SelectByResiduePdbId(params int[] ResidueIds)
            {
                HashSet<int> resids = new HashSet<int>(ResidueIds);
                List<Atom> selecteds = new List<Atom>();
                foreach(var atom in atoms)
                    if(resids.Contains(atom.ResiduePdbId))
                        selecteds.Add(atom);
                return selecteds.ToArray();
            }
            public double[] ListRmin2()
            {
                double[] lstRmin2 = new double[Count];
                for(int i=0; i<Count; i++)
                    lstRmin2[i] = atoms[i].Rmin2;
                return lstRmin2;
            }
            public int[] ListResiduePdbId()
            {
                int[] resids = new int[Count];
                for(int i=0; i<Count; i++)
                    resids[i] = atoms[i].ResiduePdbId;
                return resids;
            }
            public int[] SetResiduePdbId()
            {
                HashSet<int> resids = new HashSet<int>();
                foreach(Atom atom in atoms)
                    resids.Add(atom.ResiduePdbId);
                return resids.ToArray().HSort().ToArray();
            }
            public string[] ListAtomName(bool bTrim)
            {
                string[] lstAtomName = new string[Count];
                for(int i=0; i<Count; i++)
                    lstAtomName[i] = atoms[i].AtomName;

                if(bTrim)
                {
                    for(int i=0; i<Count; i++)
                        lstAtomName[i] = lstAtomName[i].Trim();
                }

                return lstAtomName;
            }
            public Tuple<Pdb.Atom,int>[] SelectPdbAtoms()
            {
                HDebug.Assert(false); // use ListPdbAtoms()
                List<Tuple<Pdb.Atom,int>> lstPdbatomIdx = new List<Tuple<Pdb.Atom, int>>();
                Pdb.Atom[] pdbatoms = ListPdbAtoms();
                for(int idx=0; idx<atoms.Count; idx++)
                {
                    if(pdbatoms[idx] == null)
                        continue;
                    lstPdbatomIdx.Add(new Tuple<Pdb.Atom, int>(pdbatoms[idx], idx));
                }
                return lstPdbatomIdx.ToArray();
            }
            public Pdb.Atom[] ListPdbAtoms()
            {
                return atoms.ListPdbAtoms();
            }
            public T[] ListType<T>()
                where T : class
            {
                return atoms.ListType<T>();
            }
        }
    }
}
