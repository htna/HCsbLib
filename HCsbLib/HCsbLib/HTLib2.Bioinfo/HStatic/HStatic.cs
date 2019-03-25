using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
	public static partial class HStatic
	{
        public static Dictionary<object, Tuple<Pdb.Atom, Tinker.Xyz.Atom>> GetAtomMatchingpair
            ( this (IList<Pdb.Atom>, IList<Tinker.Xyz.Atom>) pdbatoms_xyzatoms
            , double tolerance = 0.1
            )
        {
            var pdb_atoms = pdbatoms_xyzatoms.Item1;
            var xyz_atoms = pdbatoms_xyzatoms.Item2;

            KDTreeDLL.KDTree<Tuple<Pdb.Atom,Vector>> kdtree_coord_atom = new KDTreeDLL.KDTree<Tuple<Pdb.Atom,Vector>>(3);
            foreach(var pdb_atom in pdb_atoms)
            {
                Vector coord = pdb_atom.coord;
                kdtree_coord_atom.insert(coord, new Tuple<Pdb.Atom,Vector>(pdb_atom,coord));
            }

            double tolerance2 = tolerance*tolerance;
            var atom_matchingpair = new Dictionary<object, Tuple<Pdb.Atom, Tinker.Xyz.Atom>>();
            foreach(var xyz_atom in xyz_atoms)
            {
                Vector xyz_coord = xyz_atom.Coord;
                var nearests = kdtree_coord_atom.nearest(xyz_coord, 2);
                double dist20 = (xyz_coord - nearests[0].Item2).Dist2;
                double dist21 = (xyz_coord - nearests[1].Item2).Dist2;
                HDebug.Assert(dist20 < dist21);
                if(dist20 < tolerance2)
                {
                    var pdb_atom = nearests[0].Item1;
                    Tuple<Pdb.Atom, Tinker.Xyz.Atom> matchingpair = new Tuple<Pdb.Atom, Tinker.Xyz.Atom>(pdb_atom, xyz_atom);
                    atom_matchingpair.Add(pdb_atom, matchingpair);
                    atom_matchingpair.Add(xyz_atom, matchingpair);
                }
                if(dist21 < tolerance2)
                    HDebug.Exception();
            }

            return atom_matchingpair;
        }
	}
}
