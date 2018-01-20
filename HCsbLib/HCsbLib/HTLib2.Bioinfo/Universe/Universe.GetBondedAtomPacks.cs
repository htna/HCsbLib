using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public AtomPack[] GetBondedAtomPacks(bool countBond=true, bool countAngle=true, bool countImproper=true, bool countDihedral=true)
        {
            // re-implement without using "HashSet<AtomPcak>" if necessary
            throw new NotImplementedException();
            //HashSet<AtomPack> pairBond = null;
            //HashSet<AtomPack> pairAngle = null;
            //HashSet<AtomPack> pairImproper = null;
            //HashSet<AtomPack> pairDihedral = null;
            //
            //if(countBond    ) pairBond     = new HashSet<AtomPack>();
            //if(countAngle   ) pairAngle    = new HashSet<AtomPack>();
            //if(countImproper) pairImproper = new HashSet<AtomPack>();
            //if(countDihedral) pairDihedral = new HashSet<AtomPack>();
            //
            //GetBondedAtomPacks(pairBond, pairAngle, pairImproper, pairDihedral);
            //
            //HashSet< Universe.AtomPack> setAtomPackBonded = new HashSet< Universe.AtomPack>();
            //if(pairBond     != null) foreach(Universe.AtomPack pair in pairBond    ) if(setAtomPackBonded.Contains(pair) == false) { pair.info = "bond"    ; HDebug.Verify(setAtomPackBonded.Add(pair)); }
            //if(pairAngle    != null) foreach(Universe.AtomPack pair in pairAngle   ) if(setAtomPackBonded.Contains(pair) == false) { pair.info = "angle"   ; HDebug.Verify(setAtomPackBonded.Add(pair)); }
            //if(pairImproper != null) foreach(Universe.AtomPack pair in pairImproper) if(setAtomPackBonded.Contains(pair) == false) { pair.info = "improper"; HDebug.Verify(setAtomPackBonded.Add(pair)); }
            //if(pairDihedral != null) foreach(Universe.AtomPack pair in pairDihedral) if(setAtomPackBonded.Contains(pair) == false) { pair.info = "dihedral"; HDebug.Verify(setAtomPackBonded.Add(pair)); }
            //
            //List<Universe.AtomPack> listAtomPackBonded = new List<Universe.AtomPack>(setAtomPackBonded);
            //listAtomPackBonded.Sort();
            //return listAtomPackBonded.ToArray();
        }
        //public void GetBondedAtomPacks(HashSet<AtomPack> pairBond, HashSet<AtomPack> pairAngle, HashSet<AtomPack> pairImproper, HashSet<AtomPack> pairDihedral)
        //{
        //    if(pairBond != null)
        //    {
        //        pairBond.Clear();
        //        foreach(Universe.Bond bond in bonds)
        //        {
        //            pairBond.Add(new Universe.AtomPack(bond.atoms[0], bond.atoms[1]));
        //        }
        //    }
        //    if(pairAngle != null)
        //    {
        //        pairAngle.Clear();
        //        foreach(Universe.Angle angle in angles)
        //        {
        //            pairAngle.Add(new Universe.AtomPack(angle.atoms[0], angle.atoms[1]));
        //            pairAngle.Add(new Universe.AtomPack(angle.atoms[0], angle.atoms[2]));
        //            pairAngle.Add(new Universe.AtomPack(angle.atoms[1], angle.atoms[2]));
        //        }
        //    }
        //    if(pairImproper != null)
        //    {
        //        pairImproper.Clear();
        //        foreach(Universe.Improper improper in impropers)
        //        {
        //            pairImproper.Add(new Universe.AtomPack(improper.atoms[0], improper.atoms[1]));
        //            pairImproper.Add(new Universe.AtomPack(improper.atoms[0], improper.atoms[2]));
        //            pairImproper.Add(new Universe.AtomPack(improper.atoms[0], improper.atoms[3]));
        //            pairImproper.Add(new Universe.AtomPack(improper.atoms[1], improper.atoms[2]));
        //            pairImproper.Add(new Universe.AtomPack(improper.atoms[1], improper.atoms[3]));
        //            pairImproper.Add(new Universe.AtomPack(improper.atoms[2], improper.atoms[3]));
        //        }
        //    }
        //    if(pairDihedral != null)
        //    {
        //        pairDihedral.Clear();
        //        foreach(Universe.Dihedral dihedral in dihedrals)
        //        {
        //            pairDihedral.Add(new Universe.AtomPack(dihedral.atoms[0], dihedral.atoms[1]));
        //            pairDihedral.Add(new Universe.AtomPack(dihedral.atoms[0], dihedral.atoms[2]));
        //            pairDihedral.Add(new Universe.AtomPack(dihedral.atoms[0], dihedral.atoms[3]));
        //            pairDihedral.Add(new Universe.AtomPack(dihedral.atoms[1], dihedral.atoms[2]));
        //            pairDihedral.Add(new Universe.AtomPack(dihedral.atoms[1], dihedral.atoms[3]));
        //            pairDihedral.Add(new Universe.AtomPack(dihedral.atoms[2], dihedral.atoms[3]));
        //        }
        //    }
        //}
    }
}
