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
        public class Nonbondeds_v1
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
            Dictionary<Atom,CachedPotential>[] nonbondeds = null; // (atom, (potential, force))

            public Nonbondeds_v1(Atoms atoms
							, int size
							, double maxdist // = 12
							)
            {
                HDebug.Depreciated("use class Universe.Nonbondeds, instead of this");
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
                this.nonbondeds = new Dictionary<Atom, CachedPotential>[size];
            }
            public void UpdateNonbondeds(Vector[] coords, double updateDistThres)
            {
                if(updateDistThres == 0)
                    UpdateNonbondedsAll(coords);
                else
                    UpdateNonbondedsPartial(coords, updateDistThres);
            }

            public void UpdateNonbondedsAll(Vector[] coords)
            {
                HDebug.Assert(size == coords.Length);
                // coords
                foreach(Atom atom in atoms)
                {
                    int id = atom.ID;
                    this.coords[id] = coords[id].Clone();
                }
                // nonbondeds
                foreach(Atom atom in atoms)
                {
                    this.nonbondeds[atom.ID] = new Dictionary<Atom, CachedPotential>();
                    foreach(Atom nonbonded in SearchNonbondeds(atom))
                        this.nonbondeds[atom.ID].Add(nonbonded, null);
                }

                if(HDebug.IsDebuggerAttached)
                    #region Check   nonbondeds[atom.ID].Contains(nonbonded) == nonbondeds[nonbonded.ID].Contains(atom)
                {
                    foreach(Atom atom in atoms)
                    {
                        foreach(Atom nonbonded in this.nonbondeds[atom.ID].Keys)
                        {
                            HDebug.Assert(this.nonbondeds[atom.ID].ContainsKey(nonbonded));
                            HDebug.Assert(this.nonbondeds[nonbonded.ID].ContainsKey(atom));
                        }
                    }
                }
                #endregion
            }
            public void UpdateNonbondedsPartial(Vector[] coords, double updateDistThres)
            {
                HDebug.Assert(size == coords.Length);
                bool[] update = new bool[size];
                // coords
                double updateDistThres2 = updateDistThres*updateDistThres;
                int update_count = 0;
                foreach(Atom atom in atoms)
                {
                    int id = atom.ID;
                    if((this.coords[id] != null) && ((this.coords[id] - coords[id]).Dist2 < updateDistThres2))
                        continue; // skip update of the non-bonded pair list
                    this.coords[id] = coords[id].Clone();
                    update[id] = true;
                    update_count++;
                }
                // nonbondeds
                foreach(Atom atom in atoms)
                {
                    if(update[atom.ID] == false)
                        continue;
                    List<Atom> nonbondeds_new = SearchNonbondeds(atom);
                    List<Atom> nonbondeds_union = new List<Atom>(this.nonbondeds[atom.ID].Keys.Union(nonbondeds_new));
                    foreach(Atom nonbonded in nonbondeds_union)
                    {
                        bool isnew  = nonbondeds_new.Contains(nonbonded);
                        bool iscurr = this.nonbondeds[atom.ID].ContainsKey(nonbonded);
                        bool istoadd = (isnew == true ) && (iscurr == false);
                        bool istodel = (isnew == false) && (iscurr == true );
                        if(istoadd)
                        {
                            HDebug.Assert(istodel == false);
                            HDebug.Assert(this.nonbondeds[nonbonded.ID].ContainsKey(atom) == false);
                            HDebug.Assert(this.nonbondeds[atom.ID].ContainsKey(nonbonded) == false);
                            CachedPotential cache = new CachedPotential(atom, nonbonded);
                            this.nonbondeds[atom.ID].Add(nonbonded, cache);
                            this.nonbondeds[nonbonded.ID].Add(atom, cache);
                        }
                        if(istodel)
                        {
                            HDebug.Assert(istoadd == false);
                            HDebug.Assert(this.nonbondeds[nonbonded.ID].ContainsKey(atom) == true);
                            HDebug.Assert(this.nonbondeds[atom.ID].ContainsKey(nonbonded) == true);
                            this.nonbondeds[atom.ID].Remove(nonbonded);
                            this.nonbondeds[nonbonded.ID].Remove(atom);
                        }
                    }
                }
            }
            public List<Atom> SearchNonbondeds(Atom atom)
            {
                // nonbondeds
                HDebug.Assert(this.nonbondeds != null);
                List<Atom> nonbonded = new List<Atom>();
                bool[] bondeds = new bool[size];
                bondeds[atom.ID] = true;
                foreach(Atom inter in atom.Inter123) bondeds[inter.ID] = true;
                foreach(Atom inter in atom.Inter14) bondeds[inter.ID] = true;
                foreach(Atom inter in atoms)
                {
                    if(bondeds[inter.ID] == true)
                        continue;
                    //if(atom.ID > inter.ID)
                    //    continue; // avoid double count
                    double dist2 = (coords[atom.ID] - coords[inter.ID]).Dist2;
                    if(dist2 > maxdist2)
                        continue;
                    nonbonded.Add(inter);
                }
                return nonbonded;
            }
            //public void Update(Atom atom, Vector coord, double threshold)
            //{
            //    Debug.Assert(nodes[atom.ID] != null);
            //    Vector coord0 = nodes[atom.ID].coord;
            //    Vector coord1 = coord;
            //    double dist = (coord0 - coord1).Dist;
            //    if(dist > threshold)
            //    {
            //        nodes[atom.ID].coord = coord1;
            //        kdtree.delete(coord0);
            //        kdtree.insert(coord1, nodes[atom.ID]);
            //    }
            //}
            public List<Nonbonded>.Enumerator GetEnumerator()
            {
                List<Nonbonded> list = GetEnumerable();
                return list.GetEnumerator();
            }
            public List<Nonbonded> GetEnumerable()
            {
                List<Nonbonded> list = new List<Nonbonded>();
                for(int i=0; i<atoms.Length; i++)
                {
                    if(atoms[i] == null)
                        continue;
                    Atom atom = atoms[i];
                    foreach(KeyValuePair<Atom,CachedPotential> inter in nonbondeds[i])
                    {
                        if(atom.ID > inter.Key.ID)
                            continue; // avoid double count
                        HDebug.Assert(atom.ID < inter.Key.ID);
                        list.Add(new Nonbonded(atom, inter.Key));
                    }
                }
                return list;
            }
            public List<Atom> ListAtomsNonbondedsOf(Atom atom)
            {
                return nonbondeds[atom.ID].Keys.ToList();
            }
        }
	}
}
