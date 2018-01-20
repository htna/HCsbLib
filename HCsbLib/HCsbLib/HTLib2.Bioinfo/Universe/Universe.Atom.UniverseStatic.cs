using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom        = Universe.Atom       ;
    using Bond        = Universe.Bond       ;
	using Angle       = Universe.Angle      ; 
	using Dihedral    = Universe.Dihedral   ; 
	using Improper    = Universe.Improper   ; 
    using Nonbonded   = Universe.Nonbonded  ; 
    using Nonbonded14 = Universe.Nonbonded14;

    public static partial class UniverseStatic
    {
        public static Atom[] SortByIDs(this IList<Atom> atoms)
        {
            int [] IDs    = atoms.ListIDs();
            int [] idxsrt = IDs.HIdxSorted();
            Atom[] atmsot = atoms.HSelectByIndex(idxsrt);
            return atmsot;
        }

        public static int[] ListIDs(this IList<Atom> atoms)
        {
            List<int> ids = new List<int>();
            foreach(Atom atom in atoms)
                ids.Add(atom.ID);
            return ids.ToArray();
        }
        public static int[][] ListIDs(this IList<Atom[]> atomss)
        {
            List<int[]> idss = new List<int[]>();
            foreach(Atom[] atoms in atomss)
                idss.Add(atoms.ListIDs());
            return idss.ToArray();
        }
        public static Tuple<int[], int[]> ListIDs(this Tuple<Atom[], Atom[]> atoms)
        {
            return new Tuple<int[], int[]>
            (
                atoms.Item1.ListIDs(),
                atoms.Item2.ListIDs()
            );
        }
        public static Tuple<int[], int[]>[] ListIDs(this IList<Tuple<Atom[], Atom[]>> atomss)
        {
            List<Tuple<int[], int[]>> idss = new List<Tuple<int[], int[]>>();
            foreach(var atoms in atomss) idss.Add(atoms.ListIDs());
            return idss.ToArray();
        }

        public static Bond       [] ListWithinBonds       (this IList<Atom> atoms) { return atoms.ListWithinELEMs(delegate(Atom atom) { return atom.Bonds       ; }); }
        public static Angle      [] ListWithinAngles      (this IList<Atom> atoms) { return atoms.ListWithinELEMs(delegate(Atom atom) { return atom.Angles      ; }); }
        public static Dihedral   [] ListWithinDihedrals   (this IList<Atom> atoms) { return atoms.ListWithinELEMs(delegate(Atom atom) { return atom.Dihedrals   ; }); }
        public static Improper   [] ListWithinImpropers   (this IList<Atom> atoms) { return atoms.ListWithinELEMs(delegate(Atom atom) { return atom.Impropers   ; }); }
        public static Nonbonded14[] ListWithinNonbonded14s(this IList<Atom> atoms) { return atoms.ListWithinELEMs(delegate(Atom atom) { return atom.Nonbonded14s; }); }
        public static ELEM[] ListWithinELEMs<ELEM>(this IList<Atom> atoms, Func<Atom, IList<ELEM>> GetELEMs)
            where ELEM : Universe.AtomPack
        {
            HashSet<Atom> setatoms = new HashSet<Atom>(atoms);
            List<ELEM> withinelems = new List<ELEM>();
            foreach(Atom atom in atoms)
            {
                foreach(ELEM elem in GetELEMs(atom))
                {
                    //int leng = elem.atoms.Length;
                    //if(leng<=1 && setatoms.Contains(elem.atoms[0]) == false) continue;
                    //if(leng<=2 && setatoms.Contains(elem.atoms[1]) == false) continue;
                    //if(leng<=3 && setatoms.Contains(elem.atoms[2]) == false) continue;
                    //if(leng<=4 && setatoms.Contains(elem.atoms[3]) == false) continue;
                    //withinelems.Add(elem);

                    if(withinelems.Contains(elem))
                        continue;
                    bool add_to_withinelems = true;
                    foreach(var elem_atom in elem.atoms)
                        if(setatoms.Contains(elem_atom) == false)
                        {
                            add_to_withinelems = false;
                            break;
                        }
                    if(add_to_withinelems)
                        withinelems.Add(elem);
                }
            }
            return withinelems.ToArray();
        }
        public static Nonbonded[] ListWithinNonbondeds(this IList<Atom> atoms, Universe.Nonbondeds nonbondeds)
        {
            HashSet<Atom> setatoms = new HashSet<Atom>(atoms);
            List<Nonbonded> withinnonbondeds = new List<Nonbonded>();
            foreach(Nonbonded nbond in nonbondeds.EnumNonbondeds(atoms, false))
            {
                if(setatoms.Contains(nbond.atoms[0]) == false) continue;
                if(setatoms.Contains(nbond.atoms[1]) == false) continue;
                withinnonbondeds.Add(nbond);
            }
            return withinnonbondeds.ToArray();
        }

        public static Vector GetMasses(this IList<Atom> atoms, int dim=1)
        {
            int size = atoms.Count;
            double[] masses = new double[size*dim];
            for(int i=0; i<size; i++)
                for(int j=0; j<dim; j++)
                    masses[i*dim+j] = atoms[i].Mass;
            return masses;
        }
        public static Matrix GetMassMatrix(this IList<Atom> atoms, int dim=1)
        {
            int size = atoms.Count;
            double[,] masses = new double[size*dim, size*dim];
            for(int i=0; i<size; i++)
                for(int j=0; j<dim; j++)
                    masses[i*dim+j, i*dim+j] = atoms[i].Mass;
            return masses;
        }

        public static int[] ListIdxHydro(this Universe.Atoms atoms)
        {
            return atoms.ToArray().ListIdxHydro();
        }
        public static int[] ListIdxHydro(this IList<Universe.Atom> atoms)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<atoms.Count; i++)
                if(atoms[i].IsHydrogen())
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] ListIdxHeavy(this Universe.Atoms atoms)
        {
            return atoms.ToArray().ListIdxHeavy();
        }
        public static int[] ListIdxHeavy(this IList<Universe.Atom> atoms)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<atoms.Count; i++)
                if(atoms[i].IsHydrogen() == false)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static Pdb.Atom[] ListPdbAtoms(this IList<Universe.Atom> atoms)
        {
            return atoms.ListType<Pdb.Atom>();
        }
        public static string[] ListPdbAtomName(this IList<Universe.Atom> atoms, bool trim)
        {
            Pdb.Atom[] pdbatoms = atoms.ListPdbAtoms();
            string[] names = new string[pdbatoms.Length];
            for(int i=0; i<pdbatoms.Length; i++)
                if(pdbatoms[i] != null)
                {
                    string name = pdbatoms[i].name;
                    if(trim) name = name.Trim();
                    names[i] = name;
                }
            return names;
        }
        public static T[] ListType<T>(this IList<Universe.Atom> atoms)
            where T : class
        {
            int size = atoms.Count;
            T[] list = new T[size];
            for(int i=0; i<size; i++)
            {
                var atom = atoms[i];
                T[] item = atom.sources_ListType<T>();
                if(item.Length == 0)
                    continue;
                HDebug.Assert(item.Length == 1);
                list[i] = item[0];
            }
            return list;
        }
        public static Graph<Atom, Bond> BuildBondedGraph(this IList<Atom> atoms)
        {
            HashSet<Bond> bonds = new HashSet<Bond>();
            foreach(var node in atoms)
            {
                foreach(Bond bond in node.Bonds)
                    bonds.Add(bond);
            }
            Graph<Atom,Bond> bondedgraph = atoms.BuildBondedGraph(bonds.ToArray());
            return bondedgraph;
        }
        public static Graph<Atom, Bond> BuildBondedGraph(this IList<Atom> atoms, IList<Bond> bonds)
        {
            Graph<Atom,Bond> bondedgraph = new Graph<Atom, Bond>();
            foreach(var atom in atoms)
                bondedgraph.AddNode(atom);
            foreach(var bond in bonds)
            {
                Atom node0 = bond.atoms[0];
                Atom node1 = bond.atoms[1];
                if(bondedgraph.GetNode(node0) == null) continue; // skip if node0 is not in graph
                if(bondedgraph.GetNode(node1) == null) continue; // skip if node1 is not in graph
                bondedgraph.AddEdge(node0, node1, bond);
            }
            return bondedgraph;
        }
        public static int[] ListBondedDistFrom(this IList<Atom> atoms, IList<Atom> froms)
        {
            // build graph
            Graph<Atom, Bond> bondedgraph = atoms.BuildBondedGraph();

            // build dijkstra
            Graph<Atom, Bond>.Dijkstra.Elem[] elems;
            {
                Dictionary<Graph<Atom, Bond>.Node, double> source2initdist = new Dictionary<Graph<Atom,Bond>.Node,double>();
                foreach(var from in froms)
                {
                    Graph<Atom, Bond>.Node fromnode = bondedgraph.GetNode(from);
                    source2initdist.Add(fromnode, 0);
                }
                Dictionary<Graph<Atom, Bond>.Edge, double> edge_dist = new Dictionary<Graph<Atom, Bond>.Edge, double>();
                foreach(var edge in bondedgraph.Edges)
                {
                    edge_dist.Add(edge, 1);
                }
                elems = Graph<Atom, Bond>.Dijkstra.BuildMinSum(bondedgraph, source2initdist, edge_dist, IsTarget:null);
            }

            // build distances
            int[] dists;
            {
                int maxdist = -1;
                dists = new int[atoms.Count];
                for(int i=0; i<atoms.Count; i++)
                {
                    Atom atom = atoms[i];
                    var  node = bondedgraph.GetNode(atom);
                    var  path = Graph.Dijkstra.GetPathNode(elems, node);
                    dists[i] = path.Length - 1; // this include "CA" atom, and the distance of "CA" itself is 0.
                    HDebug.Assert(dists[i] >= 0);
                    HDebug.AssertIf(atom.AtomName.Trim() == "CA", dists[i] == 0);
                    maxdist = Math.Max(maxdist, dists[i]);
                }
            }

            return dists;
        }
        public static List<Atom[]> GroupByResidue(this IList<Atom> atoms)
        {
            Dictionary<string, Atom[]> resi2atoms = new Dictionary<string, Atom[]>();
            foreach(var atom in atoms)
            {
                string key;
                if(atom._ResidueId != null)
                {
                    key = atom._ResidueId;
                }
                else
                {
                    Pdb.Atom pdbatom = atom.correspond_PdbAtom();
                    if(pdbatom != null)
                        key = string.Format("[{0},{1},{2}]", pdbatom.chainID, pdbatom.resSeq, pdbatom.iCode);
                    else
                    {
                        // if corresponding PDB atom cannot be found, then
                        // * if it is not hydrogen, then use AtomId
                        // * if it is     hydrogen, then use its connected atom's AtomId
                        Atom key_atom = null;
                        if(atom.AtomElem == "H")
                            key_atom = atom.Inter12.First();
                        else
                            key_atom = atom;

                        key = string.Format("ID-{0}", key_atom.AtomId);
                    }
                }
                if(resi2atoms.ContainsKey(key) == false)
                    resi2atoms.Add(key, new Atom[0]);
                resi2atoms[key] = resi2atoms[key].HAdd(atom);
            }
            return resi2atoms.Values.ToList();
        }
        //public static Dictionary<int,Atom[]> GroupByResidueId(this IList<Atom> atoms)
        //{
        //    HDebug.ToDo("GroupByResidueId() can have a problem when univ has several chains or is composed of several pdb files.");
        //
        //    Dictionary<int, Atom[]> resi_atoms = new Dictionary<int, Atom[]>();
        //    for(int ia=0; ia<atoms.Count; ia++)
        //    {
        //        HDebug.Assert(ia == atoms[ia].ID);
        //        // find resseq of atom
        //        int? ir = null;
        //        for(int i=ia; i>=0; i--)
        //        {
        //            Pdb.Atom pdbatom = atoms[i].sources_PdbAtom();
        //            if(pdbatom != null)
        //            {
        //                ir = pdbatom.resSeq;
        //                break;
        //            }
        //        }
        //        //
        //        if(resi_atoms.ContainsKey(ir.Value) == false)
        //            resi_atoms.Add(ir.Value, new Atom[0]);
        //        resi_atoms[ir.Value] = resi_atoms[ir.Value].HAdd(atoms[ia]);
        //    }
        //    return resi_atoms;
        //}
        //public static Dictionary<int,Atom[]> GroupByResidueId(this IList<Atom> atoms)
        //{
        //    Dictionary<int, Atom[]> resi_atoms = new Dictionary<int, Atom[]>();
        //    for(int ia=0; ia<atoms.Count; ia++)
        //    {
        //        HDebug.Assert(ia == atoms[ia].ID);
        //        // find resseq of atom
        //        int ir;
        //        Atom atomia = atoms[ia];
        //        if(atomia._ResidueId != null)
        //        {
        //            ir = atomia._ResidueId.Value;
        //        }
        //        else
        //        {
        //            if(atomia.Inter12.First()._ResidueId != null)
        //            {
        //                ir = atomia.Inter12.First()._ResidueId.Value;
        //            }
        //            else
        //            {
        //                ir = 0;
        //                for(int i=ia; i>=0; i--)
        //                {
        //                    Pdb.Atom pdbatom = atoms[i].sources_PdbAtom();
        //                    if(pdbatom != null)
        //                    {
        //                        ir = pdbatom.resSeq;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        //
        //        if(resi_atoms.ContainsKey(ir) == false)
        //            resi_atoms.Add(ir, new Atom[0]);
        //        resi_atoms[ir] = resi_atoms[ir].HAdd(atoms[ia]);
        //    }
        //    return resi_atoms;
        //}
        public class CSplitByNames
        {
            public Atom[] match = null;
            public Atom[] other = null;
        }
        public static Tuple<Atom[],Atom[]> HToTuple(this CSplitByNames splits)
        {
            return new Tuple<Atom[], Atom[]>(splits.match, splits.other);
        }
        public static Tuple<Atom[], Atom[]>[] HToTuple(this IList<CSplitByNames> splits)
        {
            List<Tuple<Atom[],Atom[]>> list = new List<Tuple<Atom[], Atom[]>>();
            foreach(var split in splits) list.Add(split.HToTuple());
            return list.ToArray();
        }
        public static CSplitByNames HSplitByNames(this IList<Atom> atoms, params string[] names)
        {
            List<Atom> match = new List<Atom>();
            List<Atom> other = new List<Atom>();
            foreach(Atom atom in atoms)
            {
                if(names.Contains(atom.AtomName.Trim())) match.Add(atom);
                else                                     other.Add(atom);
            }
            return new CSplitByNames
            {
                match = match.ToArray(),
                other = other.ToArray()
            };
        }
        public static CSplitByNames HSplitByMatches(this IList<Atom> atoms, params Atom[] matches)
        {
            return HSplitByMatches(atoms, matches.HToHashSet());
        }
        public static CSplitByNames HSplitByMatches(this IList<Atom> atoms, HashSet<Atom> matches)
        {
            List<Atom> match = new List<Atom>();
            List<Atom> other = new List<Atom>();
            foreach(Atom atom in atoms)
            {
                if(matches.Contains(atom)) match.Add(atom);
                else                       other.Add(atom);
            }
            return new CSplitByNames
            {
                match = match.ToArray(),
                other = other.ToArray()
            };
        }
        public static CSplitByNames[] HSplitByNames(this IList<Atom[]> atomss, params string[] names)
        {
            List<CSplitByNames> splits = new List<CSplitByNames>();
            foreach(var atoms in atomss) splits.Add(atoms.HSplitByNames(names));
            return splits.ToArray();
        }
        public static CSplitByNames[] HSplitByMatches(this IList<Atom[]> atomss, params Atom[] matches)
        {
            return HSplitByMatches(atomss, matches.HToHashSet());
        }
        public static CSplitByNames[] HSplitByMatches(this IList<Atom[]> atomss, HashSet<Atom> matches)
        {
            List<CSplitByNames> splits = new List<CSplitByNames>();
            foreach(var atoms in atomss) splits.Add(atoms.HSplitByMatches(matches));
            return splits.ToArray();
        }
    }
}
