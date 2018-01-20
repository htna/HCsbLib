using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Top = Gromacs.Top;

    public partial class Universe
    {
        public static Universe Build(Pdb pdb, Gromacs.Top top, ITextLogger logger)
        {
            return BuilderGromacs.Build(pdb, top, logger);
        }
        public static Universe Build(Pdb pdb, Gromacs.Top top)
        {
            return BuilderGromacs.Build(pdb, top, new TextLogger());
        }
        public class BuilderGromacs : Builder
        {
            public static Universe Build(string pdbpath, string toppath, ITextLogger logger)
            {
                Pdb pdb = Pdb.FromFile(pdbpath);
                Top top = Top.FromFile(toppath);
                return Build(pdb, top, logger);
            }
            public static Universe Build(Pdb pdb, Gromacs.Top top, ITextLogger logger)
            {
                // atoms
                List<Top.Atom>     top_atoms    = top.elements.SelectSourceExtTop().ListAtom().SelectMatchToPdb(pdb.atoms);
                List<Top.Bond>     top_bonds    = top.elements.SelectSourceExtTop().ListType<Top.Bond>();
                List<Top.Pair>     top_pairs    = top.elements.SelectSourceExtTop().ListType<Top.Pair>(); ///Debug.ToDo("handle pairtype <= nbnd 1-4");
                List<Top.Angle>    top_angles   = top.elements.SelectSourceExtTop().ListType<Top.Angle>();
                List<Top.Dihedral> top_dihedral = top.elements.SelectSourceExtTop().ListType<Top.Dihedral>();
                List<Top.Atomtypes    > top_atomtypes     = top.elements.ListAtomtypes();
                List<Top.Bondtypes    > top_bondtypes     = top.elements.ListType<Top.Bondtypes    >();
                List<Top.Angletypes   > top_angletypes    = top.elements.ListType<Top.Angletypes   >();
                List<Top.Dihedraltypes> top_dihedraltypes = top.elements.ListType<Top.Dihedraltypes>();
                List<Top.Pairtypes    > top_pairtypes     = top.elements.ListType<Top.Pairtypes    >(); ///Debug.ToDo("handle pairtype <= nbnd 1-4");

                {
                    ///Debug.ToDo("handle here");
                    List<Top.LineElement> elems = new List<Top.LineElement>(top.elements);
                    foreach(var elem in top_atoms        ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_bonds        ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_pairs        ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_angles       ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_dihedral     ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_atomtypes    ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_bondtypes    ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_angletypes   ) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_dihedraltypes) HDebug.Verify(elems.Remove(elem));
                    foreach(var elem in top_pairtypes    ) HDebug.Verify(elems.Remove(elem));
                    List<Top.LineElement> srcitp = new List<Top.LineElement>();
                    List<Top.LineElement> srctop = new List<Top.LineElement>();
                    for(int i=0; i<elems.Count; i++)
                    {
                        if((elems[i].source as string).EndsWith(".itp")) { srcitp.Add(elems[i]); elems[i] = null; continue;  }
                        if((elems[i].source as string).EndsWith(".top")) { srctop.Add(elems[i]); elems[i] = null; continue; }
                    }
                    elems = elems.HRemoveAllNull().ToList();
                    HDebug.Assert(elems.Count == 0);
                }

                Atoms atoms = new Atoms();
                HDebug.Assert(pdb.atoms.Length == top_atoms.Count);
                for(int i=0; i<top_atoms.Count; i++)
                {
                    //Debug.Assert(psf.atoms[i].AtomId == pdb.atoms[i].serial);
                    //string type0 = psf.atoms[i].AtomType;
                    //Atom atom = new Atom(psf.atoms[i], prm.FindNonbonded(type0, logger), pdb.atoms[i]);
                    //atom.Coord = pdb.atoms[i].coord;
                    //atoms.Add(atom);
                    Gromacs.Top.Atom atom = top_atoms[i];
                    HDebug.Assert(i+1 == atom.cgnr);
                    HDebug.Assert(atom.cgnr == pdb.atoms[i].serial);
                    //Gromacs.Top.Atomtypes atomtype = top_atomtypes[atom.type];
                    List<Gromacs.Top.Atomtypes> atomtypes = FindTypes(top_atomtypes, atom.type);
                    HDebug.Assert(atomtypes.Count == 1);
                    Gromacs.Top.Atomtypes atomtype = atomtypes.Last();
                    //Debug.Assert(atom.charge == atomtype.charge);
                    //Debug.Assert(atom.mass   == atomtype.mass  );
                    Atom uatom = new Atom( atom.cgnr, atom.atom, atom.type, pdb.atoms[i].element.Trim()
                                         , atom.resnr, atom.residu
                                         , atom.charge, atom.mass
                                         , atomtype.epsilon, atomtype.sigma
                                         , double.NaN, double.NaN
                                         , atom, atomtype
                                         );
                    uatom.Coord = pdb.atoms[i].coord;
                    atoms.Add(uatom);
                }

                // bonds
                Bonds bonds = new Bonds();
                for(int i=0; i<top_bonds.Count; i++)
                {
                    int idx0 = top_bonds[i].ai - 1; Atom atom0 = atoms[idx0]; string type0 = atom0.AtomType;
                    int idx1 = top_bonds[i].aj - 1; Atom atom1 = atoms[idx1]; string type1 = atom1.AtomType;
                    List<Gromacs.Top.Bondtypes> bondtypes = FindTypes(top_bondtypes, type0, type1);
                    //Debug.Assert(bondtypes.Count == 1);
                    Gromacs.Top.Bondtypes bondtype = bondtypes.Last();
                    //Gromacs.Top.Bondtypes bondtype = top_bondtypes[Gromacs.Top.Bondtypes.GetStringKey(type0,type1)];
                    Bond bond = new Bond(atom0, atom1, bondtype.kb, bondtype.b0, bondtype);
                    bonds.Add(bond);
                    atom0.Bonds.Add(bond); atom0.Inter123.Add(atom1); atom0.Inter12.Add(atom1);
                    atom1.Bonds.Add(bond); atom1.Inter123.Add(atom0); atom1.Inter12.Add(atom0);
                }

                // angles
                Angles angles = new Angles();
                for(int i=0; i<top_angles.Count; i++)
                {
                    int idx0 = top_angles[i].ai - 1; Atom atom0 = atoms[idx0]; string type0 = atom0.AtomType;
                    int idx1 = top_angles[i].aj - 1; Atom atom1 = atoms[idx1]; string type1 = atom1.AtomType;
                    int idx2 = top_angles[i].ak - 1; Atom atom2 = atoms[idx2]; string type2 = atom2.AtomType;
                    List<Gromacs.Top.Angletypes> angletypes = FindTypes(top_angletypes, type0, type1, type2);
                    //Debug.Assert(angletypes.Count == 1);
                    Gromacs.Top.Angletypes angletype = angletypes.Last();
                    //Gromacs.Top.Angletypes angletype = top_angletypes[Gromacs.Top.Bondtypes.GetStringKey(type0, type1, type2)];
                    Angle angle = new Angle(atom0, atom1, atom2
                                           , angletype.cth, angletype.th0, angletype.cub, angletype.ub0
                                           , angletype
                                           );
                    angles.Add(angle);
                    atom0.Angles.Add(angle); atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2);
                    atom1.Angles.Add(angle); atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom0);
                    atom2.Angles.Add(angle); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                }

                // dihedrals
                Dihedrals dihedrals = new Dihedrals();
                Impropers impropers = new Impropers();
                for(int i=0; i<top_dihedral.Count; i++)
                {
                    int idx0 = top_dihedral[i].ai - 1; Atom atom0 = atoms[idx0]; string type0 = atom0.AtomType;
                    int idx1 = top_dihedral[i].aj - 1; Atom atom1 = atoms[idx1]; string type1 = atom1.AtomType;
                    int idx2 = top_dihedral[i].ak - 1; Atom atom2 = atoms[idx2]; string type2 = atom2.AtomType;
                    int idx3 = top_dihedral[i].al - 1; Atom atom3 = atoms[idx3]; string type3 = atom3.AtomType;
                    int funct = top_dihedral[i].funct;
                    List<Gromacs.Top.Dihedraltypes> dihedraltypes = FindTypes(top_dihedraltypes, type0, type1, type2, type3);
                    {
                        List<Gromacs.Top.Dihedraltypes> ldihedraltypes = new List<Top.Dihedraltypes>(dihedraltypes);
                        // select funct matching to its query
                        for(int j=0; j<ldihedraltypes.Count; )
                        {
                            if(ldihedraltypes[j].func == funct)
                                j++;
                            else
                                ldihedraltypes.RemoveAt(j);
                        }
                        // if there are no matching query but improper, select the improper
                        if((ldihedraltypes.Count == 0) && (dihedraltypes[0].func == 2))
                            ldihedraltypes.Add(dihedraltypes[0]);
                        dihedraltypes = new List<Top.Dihedraltypes>(ldihedraltypes);
                    }
                    //{
                    //    // if dihedral(func==9) and improper(func==2) are mixed
                    //    // select only improper
                    //    bool has_func_2 = false;
                    //    for(int j=0; j<dihedraltypes.Count; j++)
                    //        if(dihedraltypes[j].func == 2)
                    //            has_func_2 = true;
                    //    if(has_func_2)
                    //    {
                    //        for(int j=0; j<dihedraltypes.Count; )
                    //        {
                    //            if(dihedraltypes[j].func == 2)
                    //                j++;
                    //            else
                    //                dihedraltypes.RemoveAt(j);
                    //        }
                    //    }
                    //}
                    if(dihedraltypes[0].func == 9)
                    {
                        // dihedral
                        for(int j=1; j<dihedraltypes.Count; j++)
                        {
                            HDebug.Assert(dihedraltypes[0].func == dihedraltypes[j].func);
                            HDebug.Assert(dihedraltypes[0].i.Trim() == dihedraltypes[j].i.Trim());
                            HDebug.Assert(dihedraltypes[0].j.Trim() == dihedraltypes[j].j.Trim());
                            HDebug.Assert(dihedraltypes[0].k.Trim() == dihedraltypes[j].k.Trim());
                            HDebug.Assert(dihedraltypes[0].l.Trim() == dihedraltypes[j].l.Trim());
                        }
                        Gromacs.Top.Dihedraltypes dihedraltype = dihedraltypes.Last();
                        Dihedral dihedral = new Dihedral(atom0, atom1, atom2, atom3
                                                        , dihedraltype.cp, dihedraltype.mult, dihedraltype.phi0
                                                        , dihedraltype
                                                        );
                        dihedrals.Add(dihedral);
                        atom0.Dihedrals.Add(dihedral); //atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2); atom0.Inter123.Add(atom3);
                        atom1.Dihedrals.Add(dihedral); //atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom3); atom1.Inter123.Add(atom0);
                        atom2.Dihedrals.Add(dihedral); //atom2.Inter123.Add(atom3); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                        atom3.Dihedrals.Add(dihedral); //atom3.Inter123.Add(atom0); atom3.Inter123.Add(atom1); atom3.Inter123.Add(atom2);
                        continue;
                    }
                    if(dihedraltypes[0].func == 2)
                    {
                        // improper
                        for(int j=1; j<dihedraltypes.Count; j++)
                        {
                            HDebug.Assert(dihedraltypes[0].func == dihedraltypes[j].func);
                            HDebug.Assert(dihedraltypes[0].i.Trim() == dihedraltypes[j].i.Trim());
                            HDebug.Assert(dihedraltypes[0].j.Trim() == dihedraltypes[j].j.Trim());
                            HDebug.Assert(dihedraltypes[0].k.Trim() == dihedraltypes[j].k.Trim());
                            HDebug.Assert(dihedraltypes[0].l.Trim() == dihedraltypes[j].l.Trim());
                        }
                        Gromacs.Top.Dihedraltypes impropertype = dihedraltypes.Last();
                        Improper improper = new Improper(atom0, atom1, atom2, atom3
                                                        , impropertype.cp, impropertype.phi0
                                                        , impropertype
                                                        );
                        impropers.Add(improper);
                        atom0.Impropers.Add(improper); //atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2); atom0.Inter123.Add(atom3);
                        atom1.Impropers.Add(improper); //atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom3); atom1.Inter123.Add(atom0);
                        atom2.Impropers.Add(improper); //atom2.Inter123.Add(atom3); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                        atom3.Impropers.Add(improper); //atom3.Inter123.Add(atom0); atom3.Inter123.Add(atom1); atom3.Inter123.Add(atom2);
                        continue;
                    }
                    HDebug.Assert(false);


                    //Debug.Assert(dihedraltypes.Count == 1);
                    //
                    //for(int j=0; j<dihedraltypes.Count; j++)
                    //    if(dihedraltypes[j].func != 2)
                    //        dihedraltype = dihedraltypes[j];
                    //Debug.Assert(dihedraltype != null);
                    //List<Gromacs.Top.Dihedraltypes> dihedraltypes = top_dihedraltypes[Gromacs.Top.Dihedraltypes.GetStringKey(type0, type1, type2, type3)];
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
                univ.refs.Add("top", top);
                univ.atoms        = atoms;
                univ.bonds        = bonds;
                univ.angles       = angles;
                univ.dihedrals    = dihedrals;
                univ.impropers    = impropers;
                //univ.nonbondeds   = nonbondeds  ;  // do not make this list in advance, because it depends on the atom positions
                univ.nonbonded14s = nonbonded14s;

                HDebug.Assert(univ.Verify());
                return univ;
            }
        }
    }
}
