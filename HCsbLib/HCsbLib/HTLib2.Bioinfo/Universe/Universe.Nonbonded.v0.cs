/*
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
        public class Nonbonded_v0 : AtomPack
        {
            public Nonbonded_v0(Atom atom1, Atom atom2)
                : base(atom1, atom2) { }
        }

        public class Nonbondeds_v0
        {
            class SearchNode
            {
                public Atom   atom;
                public Vector coord;
            }

            SearchNode[]              nodes  = null;
            KDTree.KDTree<SearchNode> kdtree = null;
            public void Build(Atoms atoms)
            {
                int maxid = 0;
                foreach(Atom atom in atoms)
                    maxid = Math.Max(maxid, atom.ID);

                kdtree = new KDTree.KDTree<SearchNode>(3);
                nodes  = new SearchNode[maxid+1];
                foreach(Atom atom in atoms)
                {
                    Debug.Assert(nodes[atom.ID] == null);
                    nodes[atom.ID] = new SearchNode();
                    nodes[atom.ID].atom  = atom;
                    nodes[atom.ID].coord = atom.Coord.Clone();
                    kdtree.insert(atom.Coord, nodes[atom.ID]);
                }
            }
            public void Update(Atom atom, double threshold)
            {
                Debug.Assert(nodes[atom.ID] != null);
                Vector coord0 = nodes[atom.ID].coord;
                Vector coord1 = atom.Coord;
                double dist = (coord0 - coord1).Dist;
                if(dist > threshold)
                {
                    nodes[atom.ID].coord = coord1;
                    kdtree.delete(coord0);
                    kdtree.insert(coord1, nodes[atom.ID]);
                }
            }
            public List<Atom> SearchNonbondeds(Atom atom, int numNearest, double distMax)
            {
                List<SearchNode> sercheds = new List<SearchNode>(kdtree.nearest(atom.Coord, numNearest));
                List<Atom> nonbondeds = new List<Atom>();
                foreach(SearchNode serched in sercheds)
                    nonbondeds.Add(serched.atom);
                // remove self
                nonbondeds.Remove(atom);
                // remove 1-2, 1-3 interactions
                foreach(Atom bonded in atom.Inter123)
                    nonbondeds.Remove(bonded);
                // remove 1-4 interactions. (1-4 interactions can be considered manually without time-overhead.)
                foreach(Atom bonded in atom.Inter14)
                    nonbondeds.Remove(bonded);
                // remove "> distMax"
                double dist2Max  = distMax * distMax;
                int nonbondeds_Count0 = nonbondeds.Count;
                for(int idx=0; idx<nonbondeds.Count; )
                {
                    double dist2 = (atom.Coord - nonbondeds[idx].Coord).Dist2;
                    if(dist2 > dist2Max)
                        nonbondeds.RemoveAt(idx);
                    else
                        idx++;
                }
                Debug.AssertIf((sercheds.Count != nodes.Length), (nonbondeds_Count0 - nonbondeds.Count > 0)); // at least one atom is out of cutoff
                return nonbondeds;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////
            // Enumerate 1-5,6,7... interactions
            public Enumerable Enumerate()
            {
                return new Enumerable(this);
            }
            public class Enumerable : IEnumerable<Nonbonded_v0>
            {
                Nonbondeds_v0 parent;
                public Enumerable(Nonbondeds_v0 parent) { this.parent = parent; }
                IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Nonbonded_v0>)this).GetEnumerator(); }
                IEnumerator<Nonbonded_v0> IEnumerable<Nonbonded_v0>.GetEnumerator()
                {
                    HashSet<Nonbonded_v0> nonbondeds = new HashSet<Nonbonded_v0>();
                    int numNearest = Math.Min(1000, parent.nodes.Length);
                    double distMax = 8.5;
                    distMax = 12;
                    foreach(SearchNode node in parent.nodes)
                    {
                        List<Atom> nonbonds = parent.SearchNonbondeds(node.atom, numNearest, distMax);
                        foreach(Atom nonbond in nonbonds)
                        {
                            Nonbonded_v0 nonbonded = new Nonbonded_v0(node.atom, nonbond);
                            nonbondeds.Add(nonbonded);
                        }
                    }

                    return new Enumerator(nonbondeds);
                }
                public struct Enumerator : IEnumerator<Nonbonded_v0>
                {
                    HashSet<Nonbonded_v0>            nonbondeds;
                    HashSet<Nonbonded_v0>.Enumerator enumerator;
                    public Enumerator(HashSet<Nonbonded_v0> nonbondeds) { this.nonbondeds = nonbondeds; this.enumerator = nonbondeds.GetEnumerator(); }
                    object                     IEnumerator.Current      { get { return enumerator.Current; } }
                    Nonbonded_v0 IEnumerator<Nonbonded_v0>.Current      { get { return enumerator.Current; } }
                    public void Dispose()                               { nonbondeds = null; enumerator.Dispose(); }
                    public bool MoveNext()                              { return enumerator.MoveNext(); }
                    void IEnumerator.Reset()                            { ((IEnumerator)enumerator).Reset(); }
                }
            }
        }
	}
}
*/