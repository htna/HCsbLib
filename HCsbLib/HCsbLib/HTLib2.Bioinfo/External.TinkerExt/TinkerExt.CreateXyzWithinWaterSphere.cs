using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class TinkerExt
    {
        public static Tinker.Xyz CreateXyzWithinWaterSphere
            ( this Tinker.Xyz xyz0
            , Tinker.Prm prm
            , double protein_water_gap = 15
            )
        {
            /// Select indexes to keep/remove to use in coarse-graining
            /// Other atoms will not be included in coarse-graining procedure

            // kdtree of xyz-atoms
            KDTreeDLL.KDTree<Tinker.Xyz.Atom> kdtree_xyzatoms = new KDTreeDLL.KDTree<Tinker.Xyz.Atom>(3);
            foreach(var atom in xyz0.atoms)
                kdtree_xyzatoms.insert(atom.Coord, atom);

            // 1. build universe
            // 2. separate proteins and waters
            var univ0 = Universe.BuilderTinker.Build(xyz0, prm);
            List<Universe.Atom> protein = new List<Universe.Atom>();
            List<Universe.Atom> waters  = new List<Universe.Atom>();
            foreach(var atom in univ0.atoms)
            {
                Vector coord = atom.Coord;
                var xyzatom = kdtree_xyzatoms.nearest(coord);
                HDebug.Assert((coord - xyzatom.Coord).Dist == 0);
                if(xyzatom.AtomType == "OT " || xyzatom.AtomType == "HT ")
                    waters.Add(atom);
                else
                    protein.Add(atom);
            }

            // determine protein center
            Vector protein_center = new double[3];
            foreach(var atom in protein)
                protein_center += atom.Coord;
            protein_center /= protein.Count;
            // determine furtherest distance in protein from protein center
            double protein_radius = 0;
            foreach(var atom in protein)
                protein_radius = Math.Max(protein_radius, (protein_center - atom.Coord).Dist);

            // 1. add proteins into sphere
            // 2. determine radius of sphere : protein-radius + 15A (for waters)
            // 3. for all waters
            // 4.     if oxygen is within the sphere-radius
            // 5.         add oxygen
            // 6.         add hydrogens (1-2 interaction atoms)
            List<Universe.Atom> sphere = new List<Universe.Atom>();
            sphere.AddRange(protein);
            double sphere_radius = protein_radius + protein_water_gap;
            foreach(var atom in waters)
            {
                Vector coord = atom.Coord;
                var xyzatom = kdtree_xyzatoms.nearest(coord);
                if(xyzatom.AtomType == "OT ")
                {
                    double dist_from_center = (coord - protein_center).Dist;
                    if(dist_from_center < sphere_radius)
                    {
                        sphere.Add(atom);
                        HDebug.Assert(atom.Inter12.Count == 2);
                        sphere.AddRange(atom.Inter12);
                    }
                }
            }
            HDebug.Assert(sphere.Count == sphere.HToHashSet().Count);

            // determine xyz-IDs that is not in sphere
            var offsphere_xyzIDs = xyz0.atoms.HListId().HToHashSet();
            foreach(var atom in sphere)
            {
                var atom_xyzsource = atom.sources_ListType<Tinker.Xyz.Atom>();
                HDebug.Assert(atom_xyzsource.Length == 1);
                offsphere_xyzIDs.Remove(atom_xyzsource[0].Id);
            }

            // get xyz by removing off-sphere atoms
            
            var xyzsphere = xyz0.CloneByRemoveIds(offsphere_xyzIDs.ToList())
                                .CloneByReindex();
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(xyzsphere.atoms.Length == sphere.Count());
                int num_xyzatoms   = xyz0.atoms.Length;
                int num_sphere     = sphere.Count();
                int num_offsphere  = offsphere_xyzIDs.Count();
                int num_sph_offsph = num_sphere + num_offsphere;
                HDebug.Assert(num_xyzatoms == num_sph_offsph);
            }

            return xyzsphere;
        }
    }
}
