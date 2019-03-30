using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        static Universe BuildUnion(Universe univ1, Universe univ2)
        {
            Universe univ = new Universe();

            Atoms atoms = new Atoms(univ);
            //int maxresi = int.MinValue;
            //foreach(var atom in univ1.atoms)
            //{
            //    if(atom._ResidueId != null)
            //        maxresi = Math.Max(maxresi, atom._ResidueId.Value);
            //    atoms.Add(atom);
            //}
            //foreach(var atom in univ2.atoms)
            //{
            //    if(atom._ResidueId != null)
            //        atom._ResidueId = (atom._ResidueId + maxresi + 100);
            //    atoms.Add(atom);
            //}
            foreach(var atom in univ1.atoms) atoms.Add(atom);
            foreach(var atom in univ2.atoms) atoms.Add(atom);

            Bonds bonds = new Bonds();
            foreach(var bond in univ1.bonds)
            {
                bonds.Add(bond);
                Atom atom0 = bond.atoms[0];
                Atom atom1 = bond.atoms[1];
                HDebug.Assert(atom0.Bonds.Contains(bond)); HDebug.Assert(atom0.Inter123.Contains(atom1)); HDebug.Assert(atom0.Inter12.Contains(atom1));
                HDebug.Assert(atom1.Bonds.Contains(bond)); HDebug.Assert(atom1.Inter123.Contains(atom0)); HDebug.Assert(atom1.Inter12.Contains(atom0));
            }
            foreach(var bond in univ2.bonds)
            {
                bonds.Add(bond);
                Atom atom0 = bond.atoms[0];
                Atom atom1 = bond.atoms[1];
                HDebug.Assert(atom0.Bonds.Contains(bond)); HDebug.Assert(atom0.Inter123.Contains(atom1)); HDebug.Assert(atom0.Inter12.Contains(atom1));
                HDebug.Assert(atom1.Bonds.Contains(bond)); HDebug.Assert(atom1.Inter123.Contains(atom0)); HDebug.Assert(atom1.Inter12.Contains(atom0));
            }

            Angles angles = new Angles();
            foreach(Angle angle in univ1.angles)
            {
                angles.Add(angle);
                Atom atom0 = angle.atoms[0];
                Atom atom1 = angle.atoms[1];
                Atom atom2 = angle.atoms[2];
                HDebug.Assert(atom0.Angles.Contains(angle)); HDebug.Assert(atom0.Inter123.Contains(atom1)); HDebug.Assert(atom0.Inter123.Contains(atom2));
                HDebug.Assert(atom1.Angles.Contains(angle)); HDebug.Assert(atom1.Inter123.Contains(atom2)); HDebug.Assert(atom1.Inter123.Contains(atom0));
                HDebug.Assert(atom2.Angles.Contains(angle)); HDebug.Assert(atom2.Inter123.Contains(atom0)); HDebug.Assert(atom2.Inter123.Contains(atom1));
            }
            foreach(Angle angle in univ2.angles)
            {
                angles.Add(angle);
                Atom atom0 = angle.atoms[0];
                Atom atom1 = angle.atoms[1];
                Atom atom2 = angle.atoms[2];
                HDebug.Assert(atom0.Angles.Contains(angle)); HDebug.Assert(atom0.Inter123.Contains(atom1)); HDebug.Assert(atom0.Inter123.Contains(atom2));
                HDebug.Assert(atom1.Angles.Contains(angle)); HDebug.Assert(atom1.Inter123.Contains(atom2)); HDebug.Assert(atom1.Inter123.Contains(atom0));
                HDebug.Assert(atom2.Angles.Contains(angle)); HDebug.Assert(atom2.Inter123.Contains(atom0)); HDebug.Assert(atom2.Inter123.Contains(atom1));
            }

            Dihedrals dihedrals = new Dihedrals();
            foreach(Dihedral dihedral in univ1.dihedrals)
            {
                dihedrals.Add(dihedral);
                HDebug.Assert(dihedral.atoms[0].Dihedrals.Contains(dihedral));
                HDebug.Assert(dihedral.atoms[1].Dihedrals.Contains(dihedral));
                HDebug.Assert(dihedral.atoms[2].Dihedrals.Contains(dihedral));
                HDebug.Assert(dihedral.atoms[3].Dihedrals.Contains(dihedral));
            }
            foreach(Dihedral dihedral in univ2.dihedrals)
            {
                dihedrals.Add(dihedral);
                HDebug.Assert(dihedral.atoms[0].Dihedrals.Contains(dihedral));
                HDebug.Assert(dihedral.atoms[1].Dihedrals.Contains(dihedral));
                HDebug.Assert(dihedral.atoms[2].Dihedrals.Contains(dihedral));
                HDebug.Assert(dihedral.atoms[3].Dihedrals.Contains(dihedral));
            }

            Impropers impropers = new Impropers();
            foreach(var improper in univ1.impropers)
            {
                impropers.Add(improper);
                HDebug.Assert(improper.atoms[0].Impropers.Contains(improper));
                HDebug.Assert(improper.atoms[1].Impropers.Contains(improper));
                HDebug.Assert(improper.atoms[2].Impropers.Contains(improper));
                HDebug.Assert(improper.atoms[3].Impropers.Contains(improper));
            }
            foreach(var improper in univ2.impropers)
            {
                impropers.Add(improper);
                HDebug.Assert(improper.atoms[0].Impropers.Contains(improper));
                HDebug.Assert(improper.atoms[1].Impropers.Contains(improper));
                HDebug.Assert(improper.atoms[2].Impropers.Contains(improper));
                HDebug.Assert(improper.atoms[3].Impropers.Contains(improper));
            }

            Nonbonded14s nonbonded14s = new Nonbonded14s();
            nonbonded14s.Build(atoms);

            Pdb pdb;
            {
                List<Pdb.Element> elements = new List<Pdb.Element>();
                elements.AddRange(univ1.pdb.atoms);
                elements.AddRange(univ2.pdb.atoms);
                pdb = new Pdb(elements);
            }

            //Universe univ = new Universe();
            univ.pdb          = pdb;
            univ.refs.Add("pdb", pdb);
            univ.atoms        = atoms;
            univ.bonds        = bonds;
            univ.angles       = angles;
            univ.dihedrals    = dihedrals;
            univ.impropers    = impropers;
            //univ.nonbondeds   = nonbondeds  ;  // do not make this list in advance, because it depends on the atom positions
            univ.nonbonded14s = nonbonded14s;
            return univ;
        }
        public static Universe BuildUnionNamd(Universe univ1, Universe univ2)
        {
            Universe univ = BuildUnion(univ1, univ2);

            List<Namd.Psf> psfs = new List<Namd.Psf>();
            if(univ1.refs.ContainsKey("psf" )) psfs.Add     (univ1.refs["psf" ].Get<Namd.Psf  >());
            if(univ1.refs.ContainsKey("psfs")) psfs.AddRange(univ1.refs["psfs"].Get<Namd.Psf[]>());
            if(univ2.refs.ContainsKey("psf" )) psfs.Add     (univ2.refs["psf" ].Get<Namd.Psf  >());
            if(univ2.refs.ContainsKey("psfs")) psfs.AddRange(univ2.refs["psfs"].Get<Namd.Psf[]>());

            List<Namd.Prm> prms = new List<Namd.Prm>();
            if(univ1.refs.ContainsKey("prm" )) prms.Add     (univ1.refs["prm" ].Get<Namd.Prm  >());
            if(univ1.refs.ContainsKey("prms")) prms.AddRange(univ1.refs["prms"].Get<Namd.Prm[]>());
            if(univ2.refs.ContainsKey("prm" )) prms.Add     (univ2.refs["prm" ].Get<Namd.Prm  >());
            if(univ2.refs.ContainsKey("prms")) prms.AddRange(univ2.refs["prms"].Get<Namd.Prm[]>());

            univ.refs.Add("psfs", psfs.ToArray());
            univ.refs.Add("prms", prms.ToArray());

            return univ;
        }
        public static Universe BuildUnionTinker(Universe univ1, Universe univ2)
        {
            Universe univ = BuildUnion(univ1, univ2);

            Tinker.Xyz xyz;
            {
                List<Tinker.TkFile.Element> elements = new List<Tinker.TkFile.Element>();
                elements.AddRange(univ1.refs["xyz"].Get<Tinker.Xyz>().elements);
                elements.AddRange(univ2.refs["xyz"].Get<Tinker.Xyz>().elements);
                xyz = new Tinker.Xyz
                {
                    elements = elements.ToArray()
                };
            }

            Tinker.Prm prm = univ1.refs["prm"].Get<Tinker.Prm>();

            univ.refs.Add("xyz", xyz);
            univ.refs.Add("prm", prm);

            return univ;
        }
    }
}
