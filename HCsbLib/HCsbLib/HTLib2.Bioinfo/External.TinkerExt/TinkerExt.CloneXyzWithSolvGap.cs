using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class TinkerExt
    {
        public static Tinker.Xyz CloneXyzWithSolvGap(this Tinker.Xyz prot_solv, double solvgap)
        {
            List<Tinker.Xyz.Atom> prot_atoms = new List<Tinker.Xyz.Atom>();
            List<(Tinker.Xyz.Atom OT, Tinker.Xyz.Atom HT1, Tinker.Xyz.Atom HT2)> solvs = new List<(Tinker.Xyz.Atom OT, Tinker.Xyz.Atom HT1, Tinker.Xyz.Atom HT2)>();
            {
                Dictionary<int, Tinker.Xyz.Atom> id_atom = prot_solv.atoms.ToIdDictionary();
                foreach(var atom in prot_solv.atoms)
                {
                    if(atom.AtomType == "HT ")
                        continue;
                    else if(atom.AtomType == "OT ")
                    {
                        HDebug.Assert(atom.BondedIds.Length == 2);
                        Tinker.Xyz.Atom OT  = atom;
                        Tinker.Xyz.Atom HT1 = id_atom[atom.BondedId1.Value];
                        Tinker.Xyz.Atom HT2 = id_atom[atom.BondedId2.Value];

                        id_atom.Remove(OT .Id);
                        id_atom.Remove(HT1.Id);
                        id_atom.Remove(HT2.Id);

                        HDebug.Assert(HT1.BondedIds.Contains(OT.Id) && HT1.AtomType == "HT ");
                        HDebug.Assert(HT2.BondedIds.Contains(OT.Id) && HT2.AtomType == "HT ");
                        solvs.Add((OT, HT1, HT2));

                        continue;
                    }
                    else
                    {
                        // add atom in prot_gat
                        prot_atoms.Add(atom);
                    }
                }
                HDebug.Assert(id_atom.Count == 0);
            }

            KDTreeDLL.KDTree<Tinker.Xyz.Atom> kdtree = new KDTreeDLL.KDTree<Tinker.Xyz.Atom>(3);
            foreach(var atom in prot_atoms)
            {
                // add atom into kdtree, in order to check the distance between protein and a solvent atom
                kdtree.insert(atom.Coord, atom);
            }

            List<int> lstIdToSelect = new List<int>();
            foreach(var atom in prot_atoms)
                lstIdToSelect.Add(atom.Id);

            foreach(var solv in solvs)
            {
                HDebug.Assert(solv.OT .AtomType == "OT ");
                HDebug.Assert(solv.HT1.AtomType == "HT ");
                HDebug.Assert(solv.HT2.AtomType == "HT ");
                HDebug.Assert(solv.HT1.Id != solv.HT2.Id);

                var near = kdtree.nearest(solv.OT.Coord);
                double dist = (solv.OT.Coord  - near.Coord).Dist;
                if(dist <= solvgap)
                {
                    lstIdToSelect.Add(solv.OT .Id);
                    lstIdToSelect.Add(solv.HT1.Id);
                    lstIdToSelect.Add(solv.HT2.Id);
                }
            }

            Tinker.Xyz prot_solvgap = prot_solv.CloneBySelectIds(lstIdToSelect);

            return prot_solvgap;
        }
    }
}
