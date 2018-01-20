using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
	{
        public static IEnumerable<Universe.Nonbonded> EnumNonbondeds
            ( Universe.Atoms atoms
            , IList<Vector> coords
			, int size
			, double maxdist // = 12
            )
        {
            Universe.Nonbondeds _nonbondeds;
            IEnumerable<Universe.Nonbonded> nonbondeds;

            _nonbondeds = new Universe.Nonbondeds(atoms, size, maxdist);
            _nonbondeds.UpdateCoords(coords, true);
            nonbondeds = _nonbondeds.EnumNonbondeds(true);
            return nonbondeds;
        }
        public delegate IEnumerable<Universe.Nonbonded> FuncListTip3pHBond(Universe.Atoms atoms, IList<Vector> coords, bool[] waters, KDTree.KDTree<Universe.Atom> kdtree_water, string[] options);
        public static IEnumerable<Universe.Nonbonded> EnumNonbondeds
            ( Universe.Atoms atoms
            , IList<Vector> coords
			, int size
			, double maxdist // = 12
            , FuncListTip3pHBond func
            , string[] options
            )
        {
            bool[] waters = new bool[coords.Count];
            KDTree.KDTree<Universe.Atom> kdtree_water = new KDTree.KDTree<Universe.Atom>(3);
            foreach(var atom in atoms)
            {
                if(coords[atom.ID] == null)
                    continue;

                bool water = atom.IsWater();
                waters[atom.ID] = water;
                if(water)
                    kdtree_water.insert(coords[atom.ID], atom);
            }

            Universe.Nonbondeds _nonbondeds;
            _nonbondeds = new Universe.Nonbondeds(atoms, size, maxdist);
            _nonbondeds.UpdateCoords(coords, true);
            foreach(var nonbonded in _nonbondeds.EnumNonbondeds(true))
            {
                HDebug.Assert(nonbonded.atoms.Length == 2);
                var atom0 = nonbonded.atoms[0];
                var atom1 = nonbonded.atoms[1];
                if(waters[atom0.ID] && waters[atom1.ID])
                    continue;

                yield return nonbonded;
            }

            foreach(Universe.Nonbonded nonbonded in func(atoms, coords, waters, kdtree_water, options))
                yield return nonbonded;
        }
        public static IEnumerable<Universe.Nonbonded> ListTip3pTetraHBond
            ( Universe.Atoms atoms
            , IList<Vector> coords
            , bool[] waters
            , KDTree.KDTree<Universe.Atom> kdtree_water
            , string[] options
            )
        {
            double cutoff = 16;     foreach(var option in options) if(option.StartsWith("TIP3P:cutoff:"      )) cutoff       = double.Parse(option.Replace("TIP3P:cutoff:"      , ""));
            int numHydNbnd = 3;     foreach(var option in options) if(option.StartsWith("TIP3P:numHydNbnd:"  )) numHydNbnd   = int   .Parse(option.Replace("TIP3P:numHydNbnd:"  , ""));
            bool bOxyOxyInter=true; foreach(var option in options) if(option.StartsWith("TIP3P:bOxyOxyInter:")) bOxyOxyInter = bool  .Parse(option.Replace("TIP3P:bOxyOxyInter:", ""));
            bool bHydHydInter=false;foreach(var option in options) if(option.StartsWith("TIP3P:bHydHydInter:")) bHydHydInter = bool  .Parse(option.Replace("TIP3P:bHydHydInter:", ""));

            // oxy-oxy interactions
            if(bOxyOxyInter)
            foreach(var oxy1 in atoms)
            {
                if(waters[oxy1.ID] == false) continue;
                if(oxy1.AtomElem != "O") continue;
                foreach(var oxy2 in kdtree_water.nearest(coords[oxy1.ID], 100))
                {
                    if(oxy2 == oxy1) continue;
                    if(oxy2.AtomElem != "O") continue;
                    if(oxy2.Inter123.Contains(oxy1)) continue;
                    if(oxy2.ID < oxy1.ID) continue;
                    double dist = (coords[oxy1.ID] - coords[oxy2.ID]).Dist;
                    if(dist > cutoff) continue;
                    yield return new Universe.Nonbonded(oxy1, oxy2);
                }
            }

            // hyd-hyd interactions
            if(bHydHydInter)
            foreach(var hyd1 in atoms)
            {
                if(waters[hyd1.ID] == false) continue;
                if(hyd1.AtomElem != "H") continue;
                foreach(var hyd2 in kdtree_water.nearest(coords[hyd1.ID], 100))
                {
                    if(hyd2 == hyd1) continue;
                    if(hyd2.AtomElem != "H") continue;
                    if(hyd2.Inter123.Contains(hyd1)) continue;
                    if(hyd2.ID < hyd1.ID) continue;
                    double dist = (coords[hyd1.ID] - coords[hyd2.ID]).Dist;
                    if(dist > cutoff) continue;
                    yield return new Universe.Nonbonded(hyd1, hyd2);
                }
            }

            // oxy-hyd interactions
            Dictionary<Universe.Atom, HashSet<Universe.Atom>> water_oxy2hyd = new Dictionary<Universe.Atom, HashSet<Universe.Atom>>();
            for(int i=0; i<waters.Length; i++)
            {
                if(waters[i] == false) continue;
                if(atoms[i].AtomElem != "O") continue;
                water_oxy2hyd.Add(atoms[i], new HashSet<Universe.Atom>());
            }
            foreach(var hyd in atoms)
            {
                if(waters[hyd.ID] == false) continue;
                if(hyd.AtomElem != "H") continue;
                int count = 0;
                foreach(var oxy in kdtree_water.nearest(coords[hyd.ID], 100))
                {
                    if(oxy == hyd) continue;
                    if(oxy.AtomElem != "O") continue;
                    if(oxy.Inter123.Contains(hyd)) continue;
                    double dist = (coords[hyd.ID] - coords[oxy.ID]).Dist;
                    if(dist > cutoff) continue;
                    water_oxy2hyd[oxy].Add(hyd);
                    count++;
                    if(count < numHydNbnd) continue;
                    break;
                }
            }

            foreach(var oxy2hyd in water_oxy2hyd)
            {
                var oxy = oxy2hyd.Key;
                if(oxy2hyd.Value.Count < Math.Max(2,numHydNbnd))
                {
                    foreach(var hyd in kdtree_water.nearest(coords[oxy.ID], 100))
                    {
                        if(oxy == hyd) continue;
                        if(hyd.AtomElem != "H") continue;
                        if(oxy.Inter123.Contains(hyd)) continue;
                        double dist = (coords[hyd.ID] - coords[oxy.ID]).Dist;
                        if(dist > cutoff) continue;
                        oxy2hyd.Value.Add(hyd);
                        if(oxy2hyd.Value.Count >= Math.Max(2, numHydNbnd)) break;
                    }
                }
                foreach(var hyd in oxy2hyd.Value)
                    yield return new Universe.Nonbonded(oxy, hyd);
            }
        }
        public static IEnumerable<Universe.Nonbonded> ListTip3pNears
            ( Universe.Atoms atoms
            , IList<Vector> coords
            , bool[] waters
            , KDTree.KDTree<Universe.Atom> kdtree_water
            , string[] options
            )
        {
            double cutoff = 16;     foreach(var option in options) if(option.StartsWith("TIP3P:cutoff:"    )) cutoff     = double.Parse(option.Replace("TIP3P:cutoff:"    , ""));

            foreach(var atom in atoms)
            {
                if(waters[atom.ID] == false) continue;
                foreach(var near in kdtree_water.nearest(coords[atom.ID], 100))
                {
                    if(waters[near.ID] == false) continue;

                    if(atom == near) continue;
                    if(atom.Inter123.Contains(near)) continue;
                    double dist = (coords[atom.ID] - coords[near.ID]).Dist;
                    if(dist > cutoff) continue;
                    yield return new Universe.Nonbonded(atom, near);
                }
            }
        }
    }
}
