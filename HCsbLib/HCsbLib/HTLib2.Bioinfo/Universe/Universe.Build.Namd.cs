using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        public static Universe Build(Namd.Psf psf, Namd.Prm prm, Pdb pdb, bool? ignore_neg_occupancy, ITextLogger logger)
        {
            return BuilderNamd.Build(psf, prm, pdb, ignore_neg_occupancy, logger);
        }
        public static Universe Build(Namd.Psf psf, Namd.Prm prm, Pdb pdb, bool? ignore_neg_occupancy)
        {
            return BuilderNamd.Build(psf, prm, pdb, ignore_neg_occupancy, new TextLogger());
        }
        public class BuilderNamd : Builder
        {
            public static Universe Build(IList<Tuple<Pdb, Namd.Psf>> lstPdbPsf, Namd.Prm prm, bool? ignore_neg_occupancy, IList<List<string>> lstLogger)
            {
                ITextLogger[] llstLogger = new ITextLogger[lstLogger.Count];
                for(int i=0; i<lstLogger.Count; i++)
                    llstLogger[i] = new TextLogger(lstLogger[i]);
                var univ = Build(lstPdbPsf, prm, ignore_neg_occupancy, llstLogger);
                return univ;
            }
            public static Universe Build(IList<Tuple<Pdb, Namd.Psf>> lstPdbPsf, Namd.Prm prm, bool? ignore_neg_occupancy, IList<ITextLogger> lstLogger)
            {
                Universe univ = null;
                for(int i=0; i<lstPdbPsf.Count; i++)
                {
                    var pdb = lstPdbPsf[i].Item1;
                    var psf = lstPdbPsf[i].Item2;
                    var log = lstLogger[i];
                    Universe luniv = Universe.BuilderNamd.Build(psf, prm, pdb, ignore_neg_occupancy, log);
                    if(univ == null)
                        univ = luniv;
                    else
                        univ = Universe.BuildUnionNamd(univ, luniv);
                }
                return univ;
            }

            public static Universe Build(Namd.Psf psf, Namd.Prm prm, Pdb pdb, bool? ignore_neg_occupancy, ITextLogger logger)
            {
                // atoms
                Atoms atoms = new Atoms();
                Dictionary<int, Atom> id_atom = new Dictionary<int, Atom>();
                Pdb.Atom[] pdb_atoms = pdb.atoms;
                HDebug.Assert(psf.atoms.Length == pdb_atoms.Length);
                for(int i=0; i<psf.atoms.Length; i++)
                {
                    HDebug.Assert(pdb_atoms[i].try_serial == null || psf.atoms[i].AtomId == pdb_atoms[i].serial); // when num atoms in pdb is too large (>99999), it is marked as *****
                    string type0 = psf.atoms[i].AtomType;
                    Atom atom = new Atom(psf.atoms[i], prm.FindNonbonded(type0,logger), pdb_atoms[i]);
                    atom.Coord = pdb_atoms[i].coord;
                    {
                        if(ignore_neg_occupancy == null)
                        {
                            if(pdb_atoms[i].occupancy < 0)
                                throw new HException("unhandled negative occupancy during building universe");
                        }
                        else
                        {
                            if(ignore_neg_occupancy.Value && pdb_atoms[i].occupancy < 0)
                                continue;
                        }
                    }
                    atoms.Add(atom);
                    id_atom.Add(psf.atoms[i].AtomId, atom);
                }

                // bonds
                Bonds bonds = new Bonds();
                for(int i=0; i<psf.bonds.GetLength(0); i++)
                {
                    int id0 = psf.bonds[i, 0]; if(id_atom.ContainsKey(id0) == false) continue; Atom atom0 = id_atom[id0]; string type0 = atom0.AtomType;
                    int id1 = psf.bonds[i, 1]; if(id_atom.ContainsKey(id1) == false) continue; Atom atom1 = id_atom[id1]; string type1 = atom1.AtomType;
                    Bond bond = new Bond(atom0, atom1, prm.FindBond(type0, type1, logger));
                    bonds.Add(bond);
                    atom0.Bonds.Add(bond); atom0.Inter123.Add(atom1); atom0.Inter12.Add(atom1);
                    atom1.Bonds.Add(bond); atom1.Inter123.Add(atom0); atom1.Inter12.Add(atom0);
                }

                // angles
                Angles angles = new Angles();
                for(int i=0; i<psf.angles.GetLength(0); i++)
                {
                    int id0 = psf.angles[i, 0]; if(id_atom.ContainsKey(id0) == false) continue; Atom atom0 = id_atom[id0]; string type0 = atom0.AtomType;
                    int id1 = psf.angles[i, 1]; if(id_atom.ContainsKey(id1) == false) continue; Atom atom1 = id_atom[id1]; string type1 = atom1.AtomType;
                    int id2 = psf.angles[i, 2]; if(id_atom.ContainsKey(id2) == false) continue; Atom atom2 = id_atom[id2]; string type2 = atom2.AtomType;
                    var prm_angle = prm.FindAngle(type0, type1, type2, logger);
                    if(prm_angle == null)
                    {
                        HDebug.Assert(false);
                        logger.Log(string.Format
                            ( "try to add non-existing angle (({0}, prm {1})-({2}, prm {3})-({4}, prm {5})) in prm"
                            , atom0, type0, atom1, type1, atom2, type2
                            ));
                        continue;
                    }
                    Angle angle = new Angle(atom0, atom1, atom2, prm_angle);
                    angles.Add(angle);
                    atom0.Angles.Add(angle); atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2);
                    atom1.Angles.Add(angle); atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom0);
                    atom2.Angles.Add(angle); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                }

                // dihedrals
                Dihedrals dihedrals = new Dihedrals();
                for(int i=0; i<psf.dihedrals.GetLength(0); i++)
                {
                    int id0 = psf.dihedrals[i, 0]; if(id_atom.ContainsKey(id0) == false) continue; Atom atom0 = id_atom[id0]; string type0 = atom0.AtomType;
                    int id1 = psf.dihedrals[i, 1]; if(id_atom.ContainsKey(id1) == false) continue; Atom atom1 = id_atom[id1]; string type1 = atom1.AtomType;
                    int id2 = psf.dihedrals[i, 2]; if(id_atom.ContainsKey(id2) == false) continue; Atom atom2 = id_atom[id2]; string type2 = atom2.AtomType;
                    int id3 = psf.dihedrals[i, 3]; if(id_atom.ContainsKey(id3) == false) continue; Atom atom3 = id_atom[id3]; string type3 = atom3.AtomType;
                    var prm_dihedrals = prm.FindDihedral(type0, type1, type2, type3, logger);
                    if(prm_dihedrals.Length == 0)
                    {
                        HDebug.Assert(false);
                        logger.Log(string.Format
                            ("try to add non-existing dihedral (({0}, prm {1})-({2}, prm {3})-({4}, prm {5})-({6}, prm {7})) in prm"
                            , atom0, type0, atom1, type1, atom2, type2, atom3, type3
                            ));
                        continue;
                    }
                    foreach(var prm_dihedral in prm_dihedrals)
                    {
                        Dihedral dihedral;
                        if(atom0.ID < atom3.ID) dihedral = new Dihedral(atom0, atom1, atom2, atom3, prm_dihedral);
                        else                    dihedral = new Dihedral(atom3, atom2, atom1, atom0, prm_dihedral);
                        dihedrals.Add(dihedral);
                        atom0.Dihedrals.Add(dihedral); //atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2); atom0.Inter123.Add(atom3);
                        atom1.Dihedrals.Add(dihedral); //atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom3); atom1.Inter123.Add(atom0);
                        atom2.Dihedrals.Add(dihedral); //atom2.Inter123.Add(atom3); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                        atom3.Dihedrals.Add(dihedral); //atom3.Inter123.Add(atom0); atom3.Inter123.Add(atom1); atom3.Inter123.Add(atom2);
                    }
                }

                // impropers
                Impropers impropers = new Impropers();
                for(int i=0; i<psf.impropers.GetLength(0); i++)
                {
                    int id0 = psf.impropers[i, 0]; if(id_atom.ContainsKey(id0) == false) continue; Atom atom0 = id_atom[id0]; string type0 = atom0.AtomType;
                    int id1 = psf.impropers[i, 1]; if(id_atom.ContainsKey(id1) == false) continue; Atom atom1 = id_atom[id1]; string type1 = atom1.AtomType;
                    int id2 = psf.impropers[i, 2]; if(id_atom.ContainsKey(id2) == false) continue; Atom atom2 = id_atom[id2]; string type2 = atom2.AtomType;
                    int id3 = psf.impropers[i, 3]; if(id_atom.ContainsKey(id3) == false) continue; Atom atom3 = id_atom[id3]; string type3 = atom3.AtomType;
                    Improper improper = new Improper(atom0, atom1, atom2, atom3, prm.FindImproper(type0, type1, type2, type3, logger));
                    impropers.Add(improper);
                    atom0.Impropers.Add(improper); //atom0.Inter123.Add(atom1); atom0.Inter123.Add(atom2); atom0.Inter123.Add(atom3);
                    atom1.Impropers.Add(improper); //atom1.Inter123.Add(atom2); atom1.Inter123.Add(atom3); atom1.Inter123.Add(atom0);
                    atom2.Impropers.Add(improper); //atom2.Inter123.Add(atom3); atom2.Inter123.Add(atom0); atom2.Inter123.Add(atom1);
                    atom3.Impropers.Add(improper); //atom3.Inter123.Add(atom0); atom3.Inter123.Add(atom1); atom3.Inter123.Add(atom2);
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
                univ.refs.Add("psf", psf);
                univ.refs.Add("prm", prm);
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
