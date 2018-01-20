using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Prm = Tinker.Prm;
    using Xyz = Tinker.Xyz;

    public partial class Universe
    {
        public static Universe Build(Tinker.Xyz xyz, Tinker.Prm prm)
        {
            return BuilderTinker.Build(xyz, prm);
        }
        public static Universe Build(Tinker.Xyz xyz, Tinker.Prm prm, Pdb pdb)
        {
            return BuilderTinker.Build(xyz, prm, pdb, 0.002);
        }
        public class BuilderTinker : Builder
        {
            public static Universe Build(string xyzpath, bool loadxyzLatest, string prmpath)
            {
                Tinker.Xyz xyz = Tinker.Xyz.FromFile(xyzpath, loadxyzLatest);
                Tinker.Prm prm = Tinker.Prm.FromFile(prmpath);
                return Build(xyz, prm);
            }
            public static Universe Build(string xyzpath, bool loadxyzLatest, string prmpath, string pdbpath)
            {
                Tinker.Xyz xyz  = Tinker.Xyz.FromFile(xyzpath, false);
                Tinker.Xyz xyz1 = Tinker.Xyz.FromFile(xyzpath, loadxyzLatest);
                Tinker.Prm prm  = Tinker.Prm.FromFile(prmpath);
                Pdb        pdb  = Pdb.FromFile(pdbpath);
                Universe univ = Build(xyz, prm, pdb, 0.002);
                if(HDebug.IsDebuggerAttached)
                {
                    HDebug.Assert(xyz.atoms.Length == univ.size);
                    Vector[] xyzcoords = xyz.atoms.HListCoords();
                    Vector[] unvcoords = univ.GetCoords();
                    for(int i=0; i<univ.size; i++)
                    {
                        Vector dcoord = xyzcoords[i]-unvcoords[i];
                        HDebug.AssertTolerance(0.00000001, dcoord);
                    }
                }
                univ.SetCoords(xyz1.atoms.HListCoords());
                return univ;
            }
            public static Universe Build(Tinker.Xyz xyz, Tinker.Prm prm)
            {
                Pdb pdb = null;
                return Build(xyz, prm, pdb, 0.002);
            }
            public static Universe Build(Tinker.Xyz xyz, Tinker.Prm prm, Pdb pdb0, Tinker.Xyz xyz0, double tolCoordPdbXyz/*=0.002*/)
            {
                Universe univ = Build(xyz0, prm, pdb0, tolCoordPdbXyz);
                Vector[] coords = xyz.atoms.HListCoords();
                univ.SetCoords(coords);
                return univ;
            }
            public static Universe Build(Tinker.Xyz xyz, Tinker.Prm prm, Pdb pdb
                                        , double? tolCoordPdbXyz // 0.002 for default distance
                                                                 //    (0.001 * sqrt(3) = 0.0017321, which is the largest tolerance between pdb and xyz)
                                                                 // null  for not ordering by distance; 
                                        )
            {
                Dictionary<int,                    Prm.Atom    > prm_id2atom      = prm.atoms    .ToIdDictionary();
                Dictionary<int,                    Prm.Vdw     > prm_cls2vdw      = prm.vdws     .ToClassDictionary();
                Dictionary<int,                    Prm.Vdw14   > prm_cls2vdw14    = prm.vdw14s   .ToClassDictionary();
                Dictionary<Tuple<int,int>,         Prm.Bond    > prm_cls2bond     = prm.bonds    .ToClassDictionary();
                Dictionary<Tuple<int,int,int>,     Prm.Angle   > prm_cls2angle    = prm.angles   .ToClassDictionary();
                Dictionary<Tuple<int,int,int>,     Prm.Ureybrad> prm_cls2ureybrad = prm.ureybrads.ToClassDictionary();
                Dictionary<Tuple<int,int,int,int>, Prm.Improper> prm_cls2improper = prm.impropers.ToClassDictionary();
                Dictionary<Tuple<int,int,int,int>, Prm.Torsion > prm_cls2torsion  = prm.torsions .ToClassDictionary();
                Dictionary<int, Prm.Charge> prm_id2charge = prm.charges.ToIdDictionary();
                Prm.Biotype[]               prm_biotypes  = prm.biotypes;

                Xyz.Atom[] xyz_atoms = xyz.atoms;
                Pdb.Atom[] pdb_atoms = (pdb != null) ? pdb.atoms : null;
                Dictionary<int, Xyz.Atom> xyz_id2atom = xyz_atoms.ToIdDictionary();

                KDTree.KDTree<Tuple<int,Pdb.Atom>> coord2pdbatom = null;
                {
                    if(pdb_atoms != null)
                    {
                        coord2pdbatom = new KDTree.KDTree<Tuple<int, Pdb.Atom>>(3);
                        for(int ia=0; ia<pdb_atoms.Length; ia++)
                        {
                            Pdb.Atom pdb_atom = pdb_atoms[ia];
                            Vector   coord = pdb_atom.coord;
                            coord2pdbatom.insert(coord.ToArray(), new Tuple<int, Pdb.Atom>(ia, pdb_atom));
                        }
                    }
                }

                Atoms atoms = new Atoms();
                /// Debug.Assert(pdb.atoms.Length == top_atoms.Count);
                for(int i=0; i<xyz_atoms.Length; i++)
                {
                    Xyz.Atom xyz_atom = xyz_atoms[i];
                    Pdb.Atom pdb_atom = null; // = (pdb_atoms != null) ? (pdb_atoms[i]) : null;
                    if(coord2pdbatom != null)
                    {
                        Vector xyz_coord = xyz_atom.Coord;
                        Tuple<int, Pdb.Atom> ia_atom = coord2pdbatom.nearest(xyz_coord);
                        Vector pdb_coord = ia_atom.Item2.coord;
                        if(tolCoordPdbXyz == null)
                        {
                            pdb_atom = pdb_atoms[i];
                        }
                        else
                        {
                            if((xyz_coord-pdb_coord).Dist < tolCoordPdbXyz)
                                pdb_atom = ia_atom.Item2;
                            else
                            {
                                //HDebug.Assert(false);
                                pdb_atom = null;
                            }
                        }
                    }
                    if(pdb_atom != null)
                    {
                        string pdb_atom_type = pdb_atom.element.Trim();
                        string xyz_atom_type = xyz_atom.AtomType.Trim();
                        if(HDebug.IsDebuggerAttached && pdb_atom_type.Length > 0)   // sometimes element is blank: " "
                            HDebug.AssertIf(pdb_atom_type[0] == xyz_atom_type[0]);
                        if(tolCoordPdbXyz != null)
                            HDebug.AssertTolerance(tolCoordPdbXyz.Value, xyz_atom.Coord - pdb_atom.coord);
                    }
                    //if(pdb_atom != null) Debug.Assert(xyz_atom.Id == pdb_atom.serial);

                    HDebug.Assert(i+1 == xyz_atom.Id);
                    Prm.Atom   prm_atom   = prm_id2atom  [xyz_atom.AtomId];
                    Prm.Charge prm_charge = prm_id2charge[xyz_atom.AtomId];
                    Prm.Vdw    prm_vdw    = null;
                    Prm.Vdw14  prm_vdw14  = null;
                    if(prm_cls2vdw  .ContainsKey(prm_atom.Class)) prm_vdw   = prm_cls2vdw  [prm_atom.Class];
                    if(prm_cls2vdw14.ContainsKey(prm_atom.Class)) prm_vdw14 = prm_cls2vdw14[prm_atom.Class];

                    if(pdb_atom != null) if(pdb_atom.element.Trim() != "") if(pdb_atom.element.Trim() != prm_atom.AtomElem) throw new Exception();

                    Atom uatom = new Atom( AtomId      : xyz_atom.Id
                                         , AtomName    : ((pdb_atom != null) ? (pdb_atom.name.Trim()   ) : ("?"+prm_atom.Type)) /// fix later
                                         , AtomType    : prm_atom.Type
                                         , AtomElem    : prm_atom.AtomElem
                                         , ResidueId   : ((pdb_atom != null) ? (pdb_atom.resSeq        ) : (-1               )) /// fix later
                                         , ResidueName : ((pdb_atom != null) ? (pdb_atom.resName.Trim()) : ("?res"           )) /// fix later
                                         , Charge      : prm_charge.pch
                                         , Mass        : prm_atom.Mass
                                         , epsilon     : ((prm_vdw   != null) ? prm_vdw.Epsilon    :          0)
                                         , Rmin2       : ((prm_vdw   != null) ? prm_vdw.Rmin2      :          0)
                                         , eps_14      : ((prm_vdw14 != null) ? prm_vdw14.Eps_14   : double.NaN)
                                         , Rmin2_14    : ((prm_vdw14 != null) ? prm_vdw14.Rmin2_14 : double.NaN)
                                         , sources     : new object[] { xyz_atom, pdb_atom, prm_atom, prm_charge }
                                         );

                    uatom.Coord = xyz_atom.Coord;
                    atoms.Add(uatom);
                }

                
                // bonds
                Bonds bonds;
                {
                    Dictionary<Tuple<Atom, Atom>, Bond> lbonds = new Dictionary<Tuple<Atom, Atom>, Bond>();
                    for(int i=0; i<xyz_atoms.Length; i++)
                    {
                        int id0  = xyz_atoms[i].Id; HDebug.Assert(id0 == i+1);
                        int atm0 = xyz_atoms[i].AtomId;
                        int cls0 = prm_id2atom[atm0].Class;
                        Atom atom0 = atoms[id0-1]; HDebug.Assert(atom0.AtomId == id0);

                        Tuple<int, int> cls;
                        foreach(int id1 in xyz_atoms[i].BondedIds)
                        {
                            int atm1 = xyz_id2atom[id1].AtomId;
                            int cls1 = prm_id2atom[atm1].Class;
                            Atom atom1 = atoms[id1-1]; HDebug.Assert(atom1.AtomId == id1);
                            HashSet<Prm.Bond> bondtypes = new HashSet<Prm.Bond>();
                            Atom[] iatom = null;
                            cls = new Tuple<int, int>(cls0, cls1); if(prm_cls2bond.ContainsKey(cls)) { bondtypes.Add(prm_cls2bond[cls]); iatom=new Atom[]{atom0,atom1}; }
                            cls = new Tuple<int, int>(cls1, cls0); if(prm_cls2bond.ContainsKey(cls)) { bondtypes.Add(prm_cls2bond[cls]); iatom=new Atom[]{atom1,atom0}; }
                            HDebug.Assert(bondtypes.Count == 1);
                            if(bondtypes.Count >= 1)
                            {
                                Prm.Bond bondtype = bondtypes.Last();
                                HDebug.Assert(bondtype != null);

                                // sort atom id, in order to avoid duplication of bonds such that (0,1) and (1,0)
                                if(iatom.First().ID > iatom.Last().ID)
                                    iatom = iatom.Reverse().ToArray();

                                var key = new Tuple<Atom, Atom>(iatom[0], iatom[1]);
                                Bond bond = new Bond(iatom[0], iatom[1], bondtype.Kb, bondtype.b0, bondtype);
                                if(lbonds.ContainsKey(key) == false)
                                {
                                    lbonds.Add(key, bond);
                                }
                                else
                                {
                                    HDebug.Assert(bond.Kb == lbonds[key].Kb);
                                    HDebug.Assert(bond.b0 == lbonds[key].b0);
                                }
                            }
                        }
                    }
                    bonds = new Bonds();
                    foreach(Bond bond in lbonds.Values)
                    {
                        HDebug.Assert(bond.atoms.Length == 2);
                        HDebug.Assert(bond.atoms[0].ID < bond.atoms[1].ID);
                        bonds.Add(bond);
                        Atom atom0 = bond.atoms[0];
                        Atom atom1 = bond.atoms[1];
                        atom0.Bonds.Add(bond); atom0.Inter123.Add(atom1); atom0.Inter12.Add(atom1);
                        atom1.Bonds.Add(bond); atom1.Inter123.Add(atom0); atom1.Inter12.Add(atom0);
                    }
                }

                HashSet<Atom                 >[] inter12   = new HashSet<Atom                 >[xyz_atoms.Length];
                HashSet<Tuple<Atom,Atom>     >[] inter123  = new HashSet<Tuple<Atom,Atom>     >[xyz_atoms.Length];
                HashSet<Tuple<Atom,Atom,Atom>>[] inter1234 = new HashSet<Tuple<Atom,Atom,Atom>>[xyz_atoms.Length];
                {
                    HDebug.Assert(xyz_atoms.Length == atoms.Count);
                    foreach(Atom atom in atoms)
                    {
                        inter12  [atom.ID] = new HashSet<Atom                   >(atom.Inter12);
                        inter123 [atom.ID] = new HashSet<Tuple<Atom, Atom>      >();
                        inter1234[atom.ID] = new HashSet<Tuple<Atom, Atom, Atom>>();
                    }

                    // build inter123 and inter1234
                    for(int i=0; i<xyz_atoms.Length; i++)
                    {
                        Atom atom0 = atoms[i];
                        HDebug.Assert(atom0.ID == i);
                        foreach(Atom atom1 in inter12[atom0.ID])
                        {
                            HDebug.Assert(atom0 != atom1);
                            foreach(Atom atom2 in inter12[atom1.ID])
                            {
                                HDebug.Assert(atom1 != atom2);
                                if(atom0 == atom2) continue;
                                inter123[atom0.ID].Add(new Tuple<Atom, Atom>(atom1, atom2));
                                foreach(Atom atom3 in inter12[atom2.ID])
                                {
                                    HDebug.Assert(atom2 != atom3);
                                    if(atom0 == atom2) continue;
                                    if(atom0 == atom3) continue;
                                    if(atom1 == atom3) continue;
                                    inter1234[atom0.ID].Add(new Tuple<Atom, Atom, Atom>(atom1, atom2, atom3));
                                }
                            }
                        }
                    }
                }

                // angles
                Angles angles;
                {
                    Dictionary<Tuple<Atom, Atom, Atom>, Angle> langles = new Dictionary<Tuple<Atom, Atom, Atom>, Angle>();
                    foreach(Atom atom0 in atoms)
                    {
                        int id0  = xyz_atoms[atom0.ID].Id; HDebug.Assert(id0 == atom0.ID+1);
                        int atm0 = xyz_atoms[atom0.ID].AtomId;
                        int cls0 = prm_id2atom[atm0].Class;

                        foreach(var atom123 in inter123[atom0.ID])
                        {
                            Atom atom1 = atom123.Item1; int atm1 = xyz_atoms[atom1.ID].AtomId; int cls1 = prm_id2atom[atm1].Class;
                            Atom atom2 = atom123.Item2; int atm2 = xyz_atoms[atom2.ID].AtomId; int cls2 = prm_id2atom[atm2].Class;

                            Tuple<int, int, int> cls;
                            Atom[] iatom = null;
                            HashSet<Prm.Angle   > angs = new HashSet<Prm.Angle   >();
                            HashSet<Prm.Ureybrad> urbs = new HashSet<Prm.Ureybrad>();
                            cls = new Tuple<int, int, int>(cls0, cls1, cls2); if(prm_cls2angle.ContainsKey(cls)) { angs.Add(prm_cls2angle[cls]); iatom=new Atom[] {atom0,atom1,atom2}; } if(prm_cls2ureybrad.ContainsKey(cls)) { urbs.Add(prm_cls2ureybrad[cls]); }
                            cls = new Tuple<int, int, int>(cls2, cls1, cls0); if(prm_cls2angle.ContainsKey(cls)) { angs.Add(prm_cls2angle[cls]); iatom=new Atom[] {atom2,atom1,atom0}; } if(prm_cls2ureybrad.ContainsKey(cls)) { urbs.Add(prm_cls2ureybrad[cls]); }
                            HDebug.Assert(angs.Count == 1);
                            HDebug.Assert(urbs.Count <= angs.Count);
                            if(angs.Count >= 1)
                            {
                                Prm.Angle ang = angs.Last();
                                HDebug.Assert(ang != null);
                                Prm.Ureybrad urb = null;
                                if(urbs.Count >= 1)
                                    urb = urbs.Last();

                                // sort atom id, in order to avoid duplication of bonds such that (0,1) and (1,0)
                                if(iatom.First().ID > iatom.Last().ID)
                                    iatom = iatom.Reverse().ToArray();

                                var key = new Tuple<Atom, Atom, Atom>(iatom[0], iatom[1], iatom[2]);
                                Angle angle = new Angle(iatom[0], iatom[1], iatom[2]
                                                        , Ktheta: ang.Ktheta
                                                        , Theta0: ang.Theta0
                                                        , Kub: ((urb != null) ? urb.Kub : 0)
                                                        , S0 : ((urb != null) ? urb.S0  : 0)
                                                        , sources: new object[] { ang, urb }
                                                        );
                                if(langles.ContainsKey(key) == false)
                                {
                                    langles.Add(key, angle);
                                }
                                else
                                {
                                    HDebug.Assert( langles[key].Ktheta == angle.Ktheta
                                                , langles[key].Theta0 == angle.Theta0
                                                , langles[key].Kub    == angle.Kub
                                                , langles[key].S0     == angle.S0
                                                );
                                }
                            }
                        }
                    }
                    angles = new Angles();
                    foreach(Angle angle in langles.Values)
                    {
                        HDebug.Assert(angle.atoms.Length == 3);
                        HDebug.Assert(angle.atoms[0].ID < angle.atoms[2].ID);
                        angles.Add(angle);
                        Atom atom0 = angle.atoms[0];
                        Atom atom1 = angle.atoms[1];
                        Atom atom2 = angle.atoms[2];
                        atom0.Angles.Add(angle); atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2);
                        atom1.Angles.Add(angle); atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom0);
                        atom2.Angles.Add(angle); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                    }
                }

                // dihedrals
                Dihedrals dihedrals;
                {
                    Dictionary<Tuple<Atom, Atom, Atom, Atom>, List<Dihedral>> ldihedrals = new Dictionary<Tuple<Atom, Atom, Atom, Atom>, List<Dihedral>>();
                    foreach(Atom atom0 in atoms)
                    {
                        int id0  = xyz_atoms[atom0.ID].Id; HDebug.Assert(id0 == atom0.ID+1);
                        int atm0 = xyz_atoms[atom0.ID].AtomId;
                        int cls0 = prm_id2atom[atm0].Class;

                        Tuple<int, int, int, int> cls;
                        foreach(var atom1234 in inter1234[atom0.ID])
                        {
                            Atom atom1 = atom1234.Item1; int atm1 = xyz_atoms[atom1.ID].AtomId; int cls1 = prm_id2atom[atm1].Class;
                            Atom atom2 = atom1234.Item2; int atm2 = xyz_atoms[atom2.ID].AtomId; int cls2 = prm_id2atom[atm2].Class;
                            Atom atom3 = atom1234.Item3; int atm3 = xyz_atoms[atom3.ID].AtomId; int cls3 = prm_id2atom[atm3].Class;

                            HashSet<Prm.Torsion> tors = new HashSet<Prm.Torsion>();
                            Atom[] iatom = null;
                            cls = new Tuple<int, int, int, int>(cls0, cls1, cls2, cls3); if(prm_cls2torsion.ContainsKey(cls)) { tors.Add(prm_cls2torsion[cls]); iatom = new Atom[] {atom0,atom1,atom2,atom3}; }
                            cls = new Tuple<int, int, int, int>(cls3, cls2, cls1, cls0); if(prm_cls2torsion.ContainsKey(cls)) { tors.Add(prm_cls2torsion[cls]); iatom = new Atom[] {atom3,atom2,atom1,atom0}; }
                            HDebug.Assert(tors.Count == 1);
                            if(tors.Count >= 1)
                            {
                                // sort atom id, in order to avoid duplication of bonds such that (0,1) and (1,0)
                                if(iatom.First().ID > iatom.Last().ID)
                                    iatom = iatom.Reverse().ToArray();

                                var key = new Tuple<Atom, Atom, Atom, Atom>(iatom[0], iatom[1], iatom[2], iatom[3]);
                                if(ldihedrals.ContainsKey(key) == false)
                                {
                                    ldihedrals.Add(key, new List<Dihedral>());

                                    Prm.Torsion tor = tors.Last();
                                    foreach(var tordat in tor.GetListData())
                                    {
                                        Dihedral dihedral = new Dihedral(iatom[0], iatom[1], iatom[2], iatom[3]
                                                                        , Kchi : tordat.Kchi
                                                                        , n    : tordat.n
                                                                        , delta: tordat.delta
                                                                        , sources: new object[]{ tor }
                                                                        );
                                        ldihedrals[key].Add(dihedral);
                                        HDebug.Assert(dihedral.n != 0);
                                    }
                                }
                                else
                                {
                                    // do not check its contents because ...
                                }
                            }
                        }
                    }
                    dihedrals = new Dihedrals();
                    foreach(var ldihedral in ldihedrals.Values)
                    {
                        foreach(Dihedral dihedral in ldihedral)
                        {
                            HDebug.Assert(dihedral.atoms.Length == 4);
                            HDebug.Assert(dihedral.atoms[0].ID < dihedral.atoms[3].ID);
                            dihedrals.Add(dihedral);
                            dihedral.atoms[0].Dihedrals.Add(dihedral);
                            dihedral.atoms[1].Dihedrals.Add(dihedral);
                            dihedral.atoms[2].Dihedrals.Add(dihedral);
                            dihedral.atoms[3].Dihedrals.Add(dihedral);
                        }
                    }
                }

                // impropers
                Impropers impropers = new Impropers();
                {
                    Dictionary<Tuple<Atom, Atom, Atom, Atom>, Improper> limpropers = new Dictionary<Tuple<Atom, Atom, Atom, Atom>, Improper>();
                    foreach(Atom atom0 in atoms)
                    {
                        ///       ####################################
                        ///       ##                                ##
                        ///       ##  Improper Dihedral Parameters  ##
                        ///       ##                                ##
                        ///       ####################################
                        /// 
                        ///    ##################################################################
                        ///    ##                                                              ##
                        ///    ##  Following CHARMM style, the improper for a trigonal atom    ##
                        ///    ##  D bonded to atoms A, B and C could be input as improper     ##
                        ///    ##  dihedral angle D-A-B-C. The actual angle computed by the    ##
                        ///    ##  program is then literally the dihedral D-A-B-C, which will  ##
                        ///    ##  always have as its ideal value zero degrees. In general     ##
                        ///    ##  D-A-B-C is different from D-B-A-C; the order of the three   ##
                        ///    ##  peripheral atoms matters. In the original CHARMM parameter  ##
                        ///    ##  files, the trigonal atom is often listed last; ie, as       ##
                        ///    ##  C-B-A-D instead of D-A-B-C.                                 ##
                        ///    ##                                                              ##
                        ///    ##  Some of the improper angles are "double counted" in the     ##
                        ///    ##  CHARMM protein parameter set. Since TINKER uses only one    ##
                        ///    ##  improper parameter per site, we have doubled these force    ##
                        ///    ##  constants in the TINKER version of the CHARMM parameters.   ##
                        ///    ##  Symmetric parameters, which are the origin of the "double   ##
                        ///    ##  counted" CHARMM values, are handled in the TINKER package   ##
                        ///    ##  by assigning all symmetric states and using the TINKER      ##
                        ///    ##  force constant divided by the symmetry number.              ##
                        ///    ##                                                              ##
                        ///    ##  ...                                                         ##
                        ///    ##################################################################

                        int id0  = xyz_atoms[atom0.ID].Id; HDebug.Assert(id0 == atom0.ID+1);
                        int atm0 = xyz_atoms[atom0.ID].AtomId;
                        int cls0 = prm_id2atom[atm0].Class;

                        bool bAtom0InImpropers = false;
                        foreach(var cls in prm_cls2improper.Keys)
                            if(cls.Item1 == cls0)
                                bAtom0InImpropers = true;
                        if(bAtom0InImpropers == false)
                            continue;

                        Atom[] bondeds0 = inter12[atom0.ID].ToArray();
                        if(bondeds0.Length < 3)
                            continue;
                        for(int i=0; i<bondeds0.Length-2; i++)
                        {
                            Atom atom1 = bondeds0[i];
                            int atm1 = xyz_atoms[atom1.ID].AtomId;
                            int cls1 = prm_id2atom[atm1].Class;
                            for(int j=i+1; j<bondeds0.Length-1; j++)
                            {
                                Atom atom2 = bondeds0[j];
                                int atm2 = xyz_atoms[atom2.ID].AtomId;
                                int cls2 = prm_id2atom[atm2].Class;
                                for(int k=j+1; k<bondeds0.Length; k++)
                                {
                                    Atom atom3 = bondeds0[k];
                                    int atm3 = xyz_atoms[atom3.ID].AtomId;
                                    int cls3 = prm_id2atom[atm3].Class;

                                    Tuple<int, int, int, int> cls;
                                    HashSet<Prm.Improper> imps = new HashSet<Prm.Improper>();
                                    Atom[] iatom = null;
                                    cls = new Tuple<int, int, int, int>(cls0, cls1, cls2, cls3); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom0, atom1, atom2, atom3 }; }
                                    cls = new Tuple<int, int, int, int>(cls3, cls2, cls1, cls0); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom3, atom2, atom1, atom0 }; }
                                    cls = new Tuple<int, int, int, int>(cls0, cls1, cls3, cls2); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom0, atom1, atom3, atom2 }; }
                                    cls = new Tuple<int, int, int, int>(cls2, cls3, cls1, cls0); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom2, atom3, atom1, atom0 }; }
                                    cls = new Tuple<int, int, int, int>(cls0, cls2, cls1, cls3); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom0, atom2, atom1, atom3 }; }
                                    cls = new Tuple<int, int, int, int>(cls3, cls1, cls2, cls0); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom3, atom1, atom2, atom0 }; }
                                    cls = new Tuple<int, int, int, int>(cls0, cls2, cls3, cls1); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom0, atom2, atom3, atom1 }; }
                                    cls = new Tuple<int, int, int, int>(cls1, cls3, cls2, cls0); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom1, atom3, atom2, atom0 }; }
                                    cls = new Tuple<int, int, int, int>(cls0, cls3, cls1, cls2); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom0, atom3, atom1, atom2 }; }
                                    cls = new Tuple<int, int, int, int>(cls2, cls1, cls3, cls0); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom2, atom1, atom3, atom0 }; }
                                    cls = new Tuple<int, int, int, int>(cls0, cls3, cls2, cls1); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom0, atom3, atom2, atom1 }; }
                                    cls = new Tuple<int, int, int, int>(cls1, cls2, cls3, cls0); if(prm_cls2improper.ContainsKey(cls)) { imps.Add(prm_cls2improper[cls]); iatom=new Atom[] { atom1, atom2, atom3, atom0 }; }
                                    HDebug.Assert(imps.Count <= 1); // for example, H-C-HHH has C-HHH connectivity but it is not improper...
                                                                   // so imps.count <= 1
                                    if(imps.Count >= 1)
                                    {
                                        Prm.Improper imp = imps.Last(); // because iatoms contains the last case only.

                                        ///////////////////////////////////////////////////////////////////////////////////////
                                        // This bug was raised by Jae-Kyun Song at 2016-04-01                                //
                                        // In 1AAC, (1501,C)-(1503,NC2)-(1502,NC2)-(1500,NC2) is reordered                   //
                                        //       as (1500,NC2)-(1502,NC2)-(1503,NC2)-(1501,C)                                //
                                        // This bug was originally copied from dihedral code that is added to prevent adding //
                                        // duplicated interactions such as 1-2-3-4 and 4-3-2-1.                              //
                                        ///////////////////////////////////////////////////////////////////////////////////////
                                        /// old code 
                                        //  // sort atom id, in order to avoid duplication of bonds such that (0,1) and (1,0)
                                        //  if(iatom.First().ID > iatom.Last().ID)
                                        //      iatom = iatom.Reverse().ToArray();
                                        //  var key = new Tuple<Atom, Atom, Atom, Atom>(iatom[0], iatom[1], iatom[2], iatom[3]);
                                        ///////////////////////////////////////////////////////////////////////////////////////
                                        /// new code
                                        Tuple<Atom, Atom, Atom, Atom> key;
                                        {
                                            Atom   key0   = iatom[0];
                                            Atom[] key123 = (new Atom[] { iatom[1], iatom[2], iatom[3]}).SortByIDs();
                                            key = new Tuple<Atom, Atom, Atom, Atom>(key0, key123[0], key123[1], key123[2]);
                                        }
                                        ///////////////////////////////////////////////////////////////////////////////////////

                                        Improper improper = new Improper(iatom[0], iatom[1], iatom[2], iatom[3]
                                                                        , Kpsi: imp.Kpsi
                                                                        , psi0: imp.psi0
                                                                        , sources: new object[] { imp }
                                                                        );

                                        if(limpropers.ContainsKey(key) == false)
                                        {
                                            limpropers.Add(key, improper);
                                        }
                                        else
                                        {
                                            HDebug.Assert(limpropers[key].Kpsi == improper.Kpsi
                                                        ,limpropers[key].psi0 == improper.psi0
                                                        );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach(var improper in limpropers.Values)
                    {
                        HDebug.Assert(improper.atoms.Length == 4);
                        //HDebug.Assert(improper.atoms[0].ID < improper.atoms[3].ID);
                        impropers.Add(improper);
                        improper.atoms[0].Impropers.Add(improper);
                        improper.atoms[1].Impropers.Add(improper);
                        improper.atoms[2].Impropers.Add(improper);
                        improper.atoms[3].Impropers.Add(improper);
                    }                
                }

                // 1-4 interactions
                for(int i=0; i<atoms.Count; i++)
                {
                    HashSet<Atom> Inter14 = new HashSet<Atom>();
                    BuildInter1toN(atoms[i], 4, Inter14); // find all atoms for 1-4 interaction
                    Inter14.Remove(atoms[i]);             // remove self
                    foreach(Atom atom in atoms[i].Inter123)
                        Inter14.Remove(atom);             // remove all 1-2, 1-3 interactions
                    atoms[i].Inter14 = Inter14;
                }
                Nonbonded14s nonbonded14s = new Nonbonded14s();
                nonbonded14s.Build(atoms);

                //// nonbondeds
                //// do not make this list in advance, because it depends on the atom positions
                //Nonbondeds nonbondeds = new Nonbondeds();
                //nonbondeds.Build(atoms);


                Universe univ = new Universe();
                univ.pdb          = pdb;
                univ.refs.Add("xyz", xyz);
                univ.refs.Add("prm", prm);
                univ.refs.Add("pdb", pdb);
                univ.atoms        = atoms;
                univ.bonds        = bonds;
                univ.angles       = angles;
                univ.dihedrals    = dihedrals;
                univ.impropers    = impropers;
                //univ.nonbondeds   = nonbondeds  ;  // do not make this list in advance, because it depends on the atom positions
                univ.nonbonded14s = nonbonded14s;

                HDebug.Assert(univ.Verify());
                if(HDebug.False)
                {
                    List<Tuple<double, string, Bond>> lbnds = new List<Tuple<double, string, Bond>>();
                    foreach(Bond bnd in bonds)
                        lbnds.Add(new Tuple<double, string, Bond>(bnd.Kb
                                                                 , bnd.atoms[0].AtomType + "-" + bnd.atoms[1].AtomType
                                                                 , bnd));
                    lbnds = lbnds.HSelectByIndex(lbnds.HListItem1().HIdxSorted().Reverse().ToArray()).ToList();
                    double avgKb = lbnds.HListItem1().Average();

                    List<Tuple<double, string, Angle>> langs = new List<Tuple<double, string, Angle>>();
                    List<Tuple<double, string, Angle>> langubs = new List<Tuple<double, string, Angle>>();
                    foreach(Angle ang in angles)
                    {
                        langs.Add(new Tuple<double, string, Angle>(ang.Ktheta
                                                                 , ang.atoms[0].AtomType + "-" + ang.atoms[1].AtomType + "-" + ang.atoms[2].AtomType
                                                                 , ang));
                        if(ang.Kub != 0)
                        langubs.Add(new Tuple<double, string, Angle>(ang.Kub
                                                                    , ang.atoms[0].AtomType + "-" + ang.atoms[1].AtomType + "-" + ang.atoms[2].AtomType
                                                                    , ang));
                    }
                    langs   = langs  .HSelectByIndex(langs  .HListItem1().HIdxSorted().Reverse().ToArray()).ToList();
                    langubs = langubs.HSelectByIndex(langubs.HListItem1().HIdxSorted().Reverse().ToArray()).ToList();
                    double avgKtheta = langs.HListItem1().Average();
                    double avgKub    = langubs.HListItem1().Average();

                    List<Tuple<double, string, Improper>> limps = new List<Tuple<double, string, Improper>>();
                    foreach(Improper imp in impropers)
                        limps.Add(new Tuple<double, string, Improper>(imp.Kpsi
                                                                     , imp.atoms[0].AtomType + "-" + imp.atoms[1].AtomType + "-" + imp.atoms[2].AtomType + "-" + imp.atoms[3].AtomType
                                                                     , imp));
                    limps = limps.HSelectByIndex(limps.HListItem1().HIdxSorted().Reverse().ToArray()).ToList();
                    double avgKpsi = limps.HListItem1().Average();

                    List<Tuple<double, string, Dihedral>> ldihs = new List<Tuple<double, string, Dihedral>>();
                    foreach(Dihedral dih in dihedrals)
                        ldihs.Add(new Tuple<double, string, Dihedral>(dih.Kchi
                                                                     , dih.atoms[0].AtomType + "-" + dih.atoms[1].AtomType + "-" + dih.atoms[2].AtomType + "-" + dih.atoms[3].AtomType
                                                                     , dih));
                    ldihs = ldihs.HSelectByIndex(ldihs.HListItem1().HIdxSorted().Reverse().ToArray()).ToList();
                    double avgKchi = ldihs.HListItem1().Average();
                }
                return univ;
            }
        }
    }
}
