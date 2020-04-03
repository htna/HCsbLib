using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class TinkerExt
    {
        public static Tinker.Xyz CloneXyzWithSolvGap(this Tinker.Xyz xyz, double solvgap)
        {
            return null;
            /*
            int               prot_cnt       = prot_iso.atoms.Length;
            Tinker.Xyz.Atom[] prot_iso_atoms = prot_iso .atoms;
            Tinker.Xyz.Atom[] prot_slv_atoms = prot_solv.atoms;

            KDTreeDLL.KDTree<Tinker.Xyz.Atom> kdtree = new KDTreeDLL.KDTree<Tinker.Xyz.Atom>(3);
            List<Tinker.Xyz.Atom> prot_atoms = new List<Tinker.Xyz.Atom>();
            List<ValueTuple<Tinker.Xyz.Atom, Tinker.Xyz.Atom, Tinker.Xyz.Atom>> solvs = new List<(Tinker.Xyz.Atom, Tinker.Xyz.Atom, Tinker.Xyz.Atom)>();
            for(int i=0; i< prot_slv_atoms.Length; i++)
            {
                var atom = prot_slv_atoms[i];
                if(i < prot_cnt)
                {
                    // check if a protein atom in prot_iso are same to that in prot_solv
                    HDebug.Assert( atom.AtomType == prot_iso_atoms[i].AtomType);
                    HDebug.Assert((atom.Coord     - prot_iso_atoms[i].Coord).Dist < 0.0001);
                    // add atom in prot_gat
                    prot_atoms.Add(atom);
                    // add atom into kdtree, in order to check the distance between protein and a solvent atom
                    kdtree.insert     (atom.Coord, atom);
                }
                else
                {
                    if(atom.AtomType == "OT ")
                    {
                        var atom1 = prot_slv_atoms[i + 1];
                        var atom2 = prot_slv_atoms[i + 2];
                        HDebug.Assert(atom.BondedIds.Contains(atom1.Id) && atom1.AtomType == "HT ");
                        HDebug.Assert(atom.BondedIds.Contains(atom2.Id) && atom2.AtomType == "HT ");
                        solvs.Add(new ValueTuple<Tinker.Xyz.Atom, Tinker.Xyz.Atom, Tinker.Xyz.Atom>(atom, atom1, atom2));
                    }
                    else if(atom.AtomType == "HT ")
                    {
                        HDebug.Assert(solvs.Last().ToTuple().HContains(atom));
                    }
                    else
                    {
                        HDebug.Assert(false);
                    }
                }
            }

            List<int> prot_gap_idsidsSelect = new List<int>();
            foreach(var atm in prot_atoms)
                prot_gap_idsidsSelect.Add(atm.Id);

            foreach(var solv in solvs)
            {
                var OT  = solv.Item1; HDebug.Assert(OT .AtomType == "OT ");
                var HT1 = solv.Item2; HDebug.Assert(HT1.AtomType == "HT ");
                var HT2 = solv.Item3; HDebug.Assert(HT2.AtomType == "HT ");
                HDebug.Assert(HT1.Id != HT2.Id);

                var near = kdtree.nearest(OT.Coord);
                double dist = (OT.Coord  - near.Coord).Dist;
                if(dist <= solvgap)
                {
                    prot_gap_idsidsSelect.Add(OT .Id);
                    prot_gap_idsidsSelect.Add(HT1.Id);
                    prot_gap_idsidsSelect.Add(HT2.Id);
                }
            }
            if(prot_gap_idsidsSelect.Count == prot_slv_atoms.Length)
                return null; //break;

            Tinker.Xyz prot_solvgap = prot_solv.CloneBySelectIds(prot_gap_idsidsSelect);

            prot_solvgap.ToFile(filename_prot_gap_xyz, false);
            prot_solvgap.ToFile(filename_prot_gap_xyz.Replace(".xyz", "_pymol.xyz"), false, Tinker.Xyz.Atom.Format.defformat_digit06);
            */
        }
    }
}
