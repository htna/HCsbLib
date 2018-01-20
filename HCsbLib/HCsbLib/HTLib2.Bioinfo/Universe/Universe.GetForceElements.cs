using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public void GetForceElements(Vector[] coords, ForceField.IBond ffbond, out Tuple<int[],Vector[]>[] frcbond)
        {
            double energy = 0;
            MatrixByArr[,] lhess = null;

            frcbond = new Tuple<int[],Vector[]>[bonds.Count];
            {
                //ForceField.MindyBond ffbond = new ForceField.MindyBond();
                for(int i=0; i<bonds.Count; i++)
                {
                    int[] ids = new int[2];
                    Vector[] lcoords = new Vector[2];
                    ids[0] = bonds[i].atoms[0].ID; lcoords[0] = coords[ids[0]];
                    ids[1] = bonds[i].atoms[1].ID; lcoords[1] = coords[ids[1]];
                    Vector[] lforces = new Vector[2] { new double[3], new double[3] };
                    ffbond.Compute(bonds[i], lcoords, ref energy, ref lforces, ref lhess);
                    frcbond[i] = new Tuple<int[],Vector[]>( ids, lforces );
                }
            }
        }

        public void GetForceElements(Vector[] coords, ForceField.IAngle ffangle, out Tuple<int[], Vector[]>[] frcangle)
        {
            double energy = 0;
            MatrixByArr[,] lhess = null;

            frcangle = new Tuple<int[], Vector[]>[angles.Count];
            {
                //ForceField.MindyAngle ffangle = new ForceField.MindyAngle();
                for(int i=0; i<angles.Count; i++)
                {
                    int[] ids = new int[3];
                    Vector[] lcoords = new Vector[3];
                    ids[0] = angles[i].atoms[0].ID; lcoords[0] = coords[ids[0]];
                    ids[1] = angles[i].atoms[1].ID; lcoords[1] = coords[ids[1]];
                    ids[2] = angles[i].atoms[2].ID; lcoords[2] = coords[ids[2]];
                    Vector[] lforces = new Vector[3] { new double[3], new double[3], new double[3] };
                    ffangle.Compute(angles[i], lcoords, ref energy, ref lforces, ref lhess);
                    frcangle[i] = new Tuple<int[],Vector[]>( ids, lforces );
                }
            }
        }

        public void GetForceElements(Vector[] coords, ForceField.IDihedral ffdihedral, out Tuple<int[], Vector[]>[] frcdihedral)
        {
            double energy = 0;
            MatrixByArr[,] lhess = null;

            //lfrcfld = new List<ForceField.IForceField>(); lfrcfld.Add(new ForceField.MindyDihedral());
            frcdihedral = new Tuple<int[], Vector[]>[dihedrals.Count];
            {
                //ForceField.MindyDihedral ffdihedral = new ForceField.MindyDihedral();
                for(int i=0; i<dihedrals.Count; i++)
                {
                    int[] ids = new int[4];
                    Vector[] lcoords = new Vector[4];
                    ids[0] = dihedrals[i].atoms[0].ID; lcoords[0] = coords[ids[0]];
                    ids[1] = dihedrals[i].atoms[1].ID; lcoords[1] = coords[ids[1]];
                    ids[2] = dihedrals[i].atoms[2].ID; lcoords[2] = coords[ids[2]];
                    ids[3] = dihedrals[i].atoms[3].ID; lcoords[3] = coords[ids[3]];
                    Vector[] lforces = new Vector[4] { new double[3], new double[3], new double[3], new double[3] };
                    ffdihedral.Compute(dihedrals[i], lcoords, ref energy, ref lforces, ref lhess);
                    frcdihedral[i] = new Tuple<int[], Vector[]>(ids, lforces);
                }
            }
        }

        public void GetForceElements(Vector[] coords, ForceField.IImproper ffimproper, out Tuple<int[], Vector[]>[] frcimproper)
        {
            double energy = 0;
            MatrixByArr[,] lhess = null;

            //lfrcfld = new List<ForceField.IForceField>(); lfrcfld.Add(new ForceField.MindyImproper());
            frcimproper = new Tuple<int[], Vector[]>[impropers.Count];
            {
                //ForceField.MindyImproper ffimproper = new ForceField.MindyImproper();
                for(int i=0; i<impropers.Count; i++)
                {
                    int[] ids = new int[4];
                    Vector[] lcoords = new Vector[4];
                    ids[0] = impropers[i].atoms[0].ID; lcoords[0] = coords[ids[0]];
                    ids[1] = impropers[i].atoms[1].ID; lcoords[1] = coords[ids[1]];
                    ids[2] = impropers[i].atoms[2].ID; lcoords[2] = coords[ids[2]];
                    ids[3] = impropers[i].atoms[3].ID; lcoords[3] = coords[ids[3]];
                    Vector[] lforces = new Vector[4] { new double[3], new double[3], new double[3], new double[3] };
                    ffimproper.Compute(impropers[i], lcoords, ref energy, ref lforces, ref lhess);
                    frcimproper[i] = new Tuple<int[], Vector[]>(ids, lforces);
                }
            }
        }

        public void GetForceElements( Vector[] coords
                                    , ForceField.INonbonded ffnonbond
                                    , out Tuple<int[], Vector[]>[] frcnonbond
                                    , out Tuple<int[], Vector[]>[] frcnonbond14
                                    )
        {
            double nonbonds_maxdist = 12;
            GetForceElements(coords, ffnonbond, nonbonds_maxdist, out frcnonbond, out frcnonbond14);
        }
        public void GetForceElements( Vector[] coords
                                    , ForceField.INonbonded ffnonbond
                                    , double nonbonds_maxdist
                                    , out Tuple<int[], Vector[]>[] frcnonbond
                                    , out Tuple<int[], Vector[]>[] frcnonbond14
                                    )
        {
            double energy = 0;
            MatrixByArr[,] lhess = null;

            //lfrcfld = new List<ForceField.IForceField>(); lfrcfld.Add(new ForceField.MindyNonbondedLennardJones(false));
            //lfrcfld = new List<ForceField.IForceField>(); lfrcfld.Add(new ForceField.MindyNonbondedElectrostatic());
            List<Tuple<int[], Vector[]>> _frcnonbond   = new List<Tuple<int[], Vector[]>>();
            List<Tuple<int[], Vector[]>> _frcnonbond14 = new List<Tuple<int[], Vector[]>>();
            {
                //List<ForceField.IForceField> frcfld_nonbondeds = new List<ForceField.IForceField>();
                //frcfld_nonbondeds.Add(new ForceField.MindyNonbondedLennardJones(false));
                //frcfld_nonbondeds.Add(new ForceField.MindyNonbondedElectrostatic());
                Universe.Nonbondeds_v1 nonbondeds = new Universe.Nonbondeds_v1(atoms, size, nonbonds_maxdist);
                nonbondeds.UpdateNonbondeds(coords, 0);

                foreach(Universe.Nonbonded nonbond in nonbondeds)
                {
                    int[] ids = new int[2];
                    Vector[] lcoords = new Vector[2];
                    ids[0] = nonbond.atoms[0].ID; lcoords[0] = coords[ids[0]];
                    ids[1] = nonbond.atoms[1].ID; lcoords[1] = coords[ids[1]];
                    Vector[] lforces = new Vector[2] { new double[3], new double[3] };
                    ffnonbond.Compute(nonbond, lcoords, ref energy, ref lforces, ref lhess);
                    //if((lforces[0].Dist2 == 0) || (lforces[1].Dist2 == 0))
                    //    continue;
                    _frcnonbond.Add(new Tuple<int[], Vector[]>(ids, lforces));
                }

                foreach(Universe.Nonbonded14 nonbond in nonbonded14s)
                {
                    int[] ids = new int[2];
                    Vector[] lcoords = new Vector[2];
                    ids[0] = nonbond.atoms[0].ID; lcoords[0] = coords[ids[0]];
                    ids[1] = nonbond.atoms[1].ID; lcoords[1] = coords[ids[1]];
                    Vector[] lforces = new Vector[2] { new double[3], new double[3] };
                    ffnonbond.Compute(nonbond, lcoords, ref energy, ref lforces, ref lhess);
                    //if((lforces[0].Dist2 == 0) || (lforces[1].Dist2 == 0))
                    //    continue;
                    _frcnonbond14.Add(new Tuple<int[], Vector[]>(ids, lforces));
                }
            }
            frcnonbond   = _frcnonbond  .ToArray();
            frcnonbond14 = _frcnonbond14.ToArray();
        }
    }
}
