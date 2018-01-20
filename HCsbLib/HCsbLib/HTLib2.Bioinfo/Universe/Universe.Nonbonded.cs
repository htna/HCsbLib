using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using IEnumerator = System.Collections.IEnumerator;
    using IEnumerable = System.Collections.IEnumerable;

	public partial class Universe
	{
        public class Nonbonded : AtomPack
        {
            public Nonbonded(Atom atom1, Atom atom2)
                : base(atom1, atom2) { }
        }
        public class Nonbondeds : IEnumerable<Nonbonded>
        {
            public class CachedPotential : AtomPack
            {
                public CachedPotential(Atom atom1, Atom atom2) : base(atom1, atom2) { }
                public double energy;
                public Vector force;
            }
            double          maxdist2   = 12*12;
            double          maxdist    = 12;
            int             size       = -1;
            Vector[]        coords     = null;
            Atom[]          atoms      = null;
            KDTree.KDTree<Atom> kdtree  = null;

            public Nonbondeds(Atoms atoms
							, int size
							, double maxdist // = 12
							)
            {
                // length
                this.size     = size;
                // maxdist
                this.maxdist  = maxdist;
                this.maxdist2 = maxdist * maxdist;
                // atoms & coords
                HDebug.Assert(size == atoms.Count);
                this.atoms  = new Atom[size];
                foreach(Atom atom in atoms)
                {
                    int id = atom.ID;
                    HDebug.Assert(this.atoms[id] == null);
                    this.atoms[id] = atom;
                }
                // assign memory for this.coords and this.nonbondeds
                this.coords = new Vector[size];
                this.kdtree = null;
            }
            public void UpdateCoords(IList<Vector> coords, bool skip_nulls)
            {
                kdtree = new KDTree.KDTree<Atom>(3);
                foreach(Atom atom in atoms)
                {
                    int id = atom.ID;
                    if(skip_nulls && coords[id] == null)
                        continue;
                    this.coords[id] = coords[id].Clone();
                    try
                    {
                        kdtree.insert(this.coords[id], atom);
                    }
                    catch(Exception)
                    {
                        HDebug.Assert(false);
                        var check = kdtree.search(this.coords[id]);
                        Vector veps = new double[3];
                        veps.SetValue(0.00000001);
                        kdtree.insert(this.coords[id] + veps, atom);
                    }
                }
            }

            IEnumerator<Nonbonded> IEnumerable<Nonbonded>.GetEnumerator()
            {
                return EnumNonbondeds(false).GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return EnumNonbondeds(false).GetEnumerator();
            }
            public IEnumerable<Nonbonded> EnumNonbondeds(IList<Atom>   atoms, bool skip_nulls) { return EnumNonbondedsImpl(atoms, skip_nulls); }
            public IEnumerable<Nonbonded> EnumNonbondeds(bool skip_nulls, params Atom[] atoms) { return EnumNonbondedsImpl(atoms, skip_nulls); }
            public IEnumerable<Nonbonded> EnumNonbondedsImpl(IList<Atom> atoms, bool skip_nulls)
            {
                if(atoms.Count == 0)
                    atoms = this.atoms;
                foreach(Atom atom in atoms)
                {
                    HDebug.Assert(object.ReferenceEquals(atom, this.atoms[atom.ID]));
                    int id = atom.ID;
                    Vector atomcoord = coords[id];
                    if(skip_nulls && atomcoord == null)
                        continue;
                    double[] lowk = atomcoord - new Vector(maxdist,maxdist,maxdist);
                    double[] uppk = atomcoord + new Vector(maxdist,maxdist,maxdist);
                    Atom[] nears = kdtree.range(lowk, uppk);
                    //Atom[] nbonds = nears.Except(atom.Inter01234).ToArray().HSort(Atom.CompareById).ToArray();
                    var nbonds = nears.Except(atom.Inter01234);
                    foreach(var nbond in nbonds)
                    {
                        if(atom.ID >= nbond.ID) continue;
                        if((atomcoord - coords[nbond.ID]).Dist2 > maxdist2) continue;
                        yield return new Nonbonded(atom, nbond);
                    }
                }
            }
        }
        public IEnumerable<Nonbonded> EnumNonbondedsInfCutoff()
        {
#pragma warning disable CS0162
            for(int i=0; i<size; i++)
            {
                Atom atom = atoms[i];
                for(int j=i+1; j<size; j++)
                {
                    Atom nbond = atoms[j];
                    if(atom.ID < nbond.ID) yield return new Nonbonded(atom, nbond);
                    if(atom.ID > nbond.ID) yield return new Nonbonded(nbond, atom);
                    throw new Exception("univ has atoms with the same ID");
                }
            }
#pragma warning restore CS0162
        }
        public IEnumerable<Nonbonded> EnumNonbondeds(Vector[] coords, double cutoff, bool skip_nulls)
        {
            Universe.Nonbondeds nonbondeds = new Universe.Nonbondeds(atoms, size
                                                                    , maxdist: cutoff
                                                                    );
            nonbondeds.UpdateCoords(coords, skip_nulls);
            return nonbondeds.EnumNonbondeds(skip_nulls);
        }
	}
}
