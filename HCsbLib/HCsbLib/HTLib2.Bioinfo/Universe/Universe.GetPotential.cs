using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public double GetPotential(List<ForceField.IForceField> frcflds, Vector[] coords, out Vectors forces, Dictionary<string, object> cache, double[,] pwfrc=null, double[,] pwspr=null)
        {
            forces  = GetVectorsZero();
            MatrixByArr hessian = null;
            Vector[] forces_ = forces;
            double energy = GetPotential(frcflds, coords, ref forces_, ref hessian, cache, pwfrc: pwfrc, pwspr: pwspr);
            return energy;
        }
        public double GetPotential(List<ForceField.IForceField> frcflds, Vector[] coords, out Vector[] forces, Dictionary<string, object> cache, double[,] pwfrc=null, double[,] pwspr=null)
        {
            forces  = GetVectorsZero();
            MatrixByArr hessian = null;
            double energy = GetPotential(frcflds, coords, ref forces, ref hessian, cache, pwfrc: pwfrc, pwspr: pwspr);
            return energy;
        }
        public double GetPotential(List<ForceField.IForceField> frcflds, ref Vector[] forces, ref MatrixByArr hessian, Dictionary<string, object> cache, Vector[,] forceij=null, double[,] pwfrc=null, double[,] pwspr=null)
        {
            Vector[] coords = new Vector[size];
            foreach(Atom atom in atoms)
            {
                HDebug.Assert(coords[atom.ID] == null);
                coords[atom.ID] = atom.Coord;
            }
            return GetPotential(frcflds, coords, ref forces, ref hessian, cache, forceij, pwfrc, pwspr);
        }
        public double GetPotential(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr hessian, Dictionary<string, object> cache, Vector[,] forceij=null, double[,] pwfrc=null, double[,] pwspr=null)
        {
            //forceij = new Vector[size, size];
            MatrixByArr[,] hess = null;
            if(hessian != null)
            {
                int size = coords.GetLength(0);
                hess = LinAlg.CreateMatrixArray(size, size, new double[3, 3]);
            }

            double energy  = 0;
            double energy_bonds      = GetPotentialBonds     (frcflds, coords, ref forces, ref hess, cache, forceij, pwfrc, pwspr); energy += energy_bonds     ;
            double energy_angles     = GetPotentialAngles    (frcflds, coords, ref forces, ref hess, cache, forceij, pwfrc, pwspr); energy += energy_angles    ;
            double energy_dihedrals  = GetPotentialDihedrals (frcflds, coords, ref forces, ref hess, cache, forceij, pwfrc, pwspr); energy += energy_dihedrals ;
            double energy_impropers  = GetPotentialImpropers (frcflds, coords, ref forces, ref hess, cache, forceij, pwfrc, pwspr); energy += energy_impropers ;
            double energy_nonbondeds = GetPotentialNonbondeds(frcflds, coords, ref forces, ref hess, cache, forceij, pwfrc, pwspr); energy += energy_nonbondeds;
            double energy_customs    = GetPotentialCustoms   (frcflds, coords, ref forces, ref hess, cache, forceij, pwfrc, pwspr); energy += energy_customs   ;

            if(hess != null)
                GetPotentialBuildHessian(coords, hess, ref hessian);

            if(cache != null)
            {
                if(cache.ContainsKey("energy_bonds     ") == false) cache.Add("energy_bonds     ", 0); cache["energy_bonds     "] = energy_bonds     ;
                if(cache.ContainsKey("energy_angles    ") == false) cache.Add("energy_angles    ", 0); cache["energy_angles    "] = energy_angles    ;
                if(cache.ContainsKey("energy_dihedrals ") == false) cache.Add("energy_dihedrals ", 0); cache["energy_dihedrals "] = energy_dihedrals ;
                if(cache.ContainsKey("energy_impropers ") == false) cache.Add("energy_impropers ", 0); cache["energy_impropers "] = energy_impropers ;
                if(cache.ContainsKey("energy_nonbondeds") == false) cache.Add("energy_nonbondeds", 0); cache["energy_nonbondeds"] = energy_nonbondeds;
                if(cache.ContainsKey("energy_customs   ") == false) cache.Add("energy_customs   ", 0); cache["energy_customs   "] = energy_customs   ;
            }
            return energy;
        }
        public void GetPotentialBuildHessian(Vector[] coords, MatrixByArr[,] hess, ref MatrixByArr hessian)
        {
            int size = coords.GetLength(0);
            for(int c=0; c<size*3; c++)
                for(int r=0; r<size*3; r++)
                    hessian[c, r] = hess[c/3, r/3][c%3, r%3];

            hessian = (hessian + hessian.Tr())/2;

            for(int i=0; i<size; i++)
            {
                hessian[i*3+0, i*3+0] = 0;    hessian[i*3+0, i*3+1] = 0;    hessian[i*3+0, i*3+2] = 0;
                hessian[i*3+1, i*3+0] = 0;    hessian[i*3+1, i*3+1] = 0;    hessian[i*3+1, i*3+2] = 0;
                hessian[i*3+2, i*3+0] = 0;    hessian[i*3+2, i*3+1] = 0;    hessian[i*3+2, i*3+2] = 0;

                int c = i;
                for(int r=0; r<size; r++)
                {
                    hessian[i*3+0, i*3+0] -= hessian[c*3+0, r*3+0]; hessian[i*3+0, i*3+1] -= hessian[c*3+0, r*3+1]; hessian[i*3+0, i*3+2] -= hessian[c*3+0, r*3+2];
                    hessian[i*3+1, i*3+0] -= hessian[c*3+1, r*3+0]; hessian[i*3+1, i*3+1] -= hessian[c*3+1, r*3+1]; hessian[i*3+1, i*3+2] -= hessian[c*3+1, r*3+2];
                    hessian[i*3+2, i*3+0] -= hessian[c*3+2, r*3+0]; hessian[i*3+2, i*3+1] -= hessian[c*3+2, r*3+1]; hessian[i*3+2, i*3+2] -= hessian[c*3+2, r*3+2];
                }
            }
        }

        public List<T> SelectInFrcflds<T>(List<ForceField.IForceField> frcflds, List<T> selecteds) where T : ForceField.IForceField
        {
            foreach(ForceField.IForceField frcfld in frcflds)
                if(typeof(T).IsInstanceOfType(frcfld))
                    selecteds.Add((T)frcfld);
            return selecteds;
        }
        public double GetPotentialBonds(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                        Dictionary<string, object> cache, PwForceDecomposer forceij, double[,] pwfrc=null, double[,] pwspr=null)
        {
            List<ForceField.IBond> frcfld_bonds = SelectInFrcflds(frcflds, new List<ForceField.IBond>());
            double energy = 0;

            if(frcfld_bonds.Count == 0)
                return energy;

            double stat_min = double.MaxValue;
            double stat_max = double.MinValue;
            double netstat_min = double.MaxValue;
            double netstat_max = double.MinValue;

            Vector[] lcoords = new Vector[2];
            Vector[] lforces = (forces == null) ? null : new Vector[2];
            for(int i=0; i<bonds.Count; i++)
            {
                int id0 = bonds[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = bonds[i].atoms[1].ID; lcoords[1] = coords[id1];
                foreach(ForceField.IBond frcfld in frcfld_bonds)
                {
                    if(forces != null)
                    {
                        lforces[0] = new double[3];
                        lforces[1] = new double[3];
                    }
                    MatrixByArr[,] lhess = (hessian == null) ? null : LinAlg.CreateMatrixArray(2, 2, new double[3, 3]);
                    frcfld.Compute(bonds[i], lcoords, ref energy, ref lforces, ref lhess, pwfrc, pwspr);
                    HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                    if(forces != null)
                    {
                        forces[id0] += lforces[0];
                        forces[id1] += lforces[1];
                        forceij.AddBond(id0, id1, lcoords, lforces);
                        HDebug.AssertTolerance(0.00000001, lforces[0].Dist2 - lforces[1].Dist2);
                        double netstat = lforces[0].Dist2 + lforces[1].Dist2;
                        netstat_min = Math.Min(netstat_min, netstat);
                        netstat_max = Math.Max(netstat_max, netstat);
                        stat_min = Math.Min(stat_min, lforces[0].Dist);
                        stat_max = Math.Max(stat_max, lforces[0].Dist);
                        stat_min = Math.Min(stat_min, lforces[1].Dist);
                        stat_max = Math.Max(stat_max, lforces[1].Dist);
                    }
                    if(hessian != null)
                    {
                        hessian[id0, id0] += lhess[0, 0]; hessian[id0, id1] += lhess[0, 1];
                        hessian[id1, id0] += lhess[1, 0]; hessian[id1, id1] += lhess[1, 1];
                    }
                }
            }
            return energy;
        }
        public double GetPotentialAngles(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                         Dictionary<string, object> cache, PwForceDecomposer forceij, double[,] pwfrc=null, double[,] pwspr=null)
        {
            List<ForceField.IAngle> frcfld_angles = SelectInFrcflds(frcflds, new List<ForceField.IAngle>());
            double energy = 0;

            if(frcfld_angles.Count == 0)
                return energy;

            double stat_min = double.MaxValue;
            double stat_max = double.MinValue;
            double netstat_min = double.MaxValue;
            double netstat_max = double.MinValue;

            Vector[] lcoords = new Vector[3];
            Vector[] lforces = (forces == null) ? null : new Vector[3];
            for(int i=0; i<angles.Count; i++)
            {
                int id0 = angles[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = angles[i].atoms[1].ID; lcoords[1] = coords[id1];
                int id2 = angles[i].atoms[2].ID; lcoords[2] = coords[id2];
                foreach(ForceField.IAngle frcfld in frcfld_angles)
                {
                    if(forces != null)
                    {
                        lforces[0] = new double[3];
                        lforces[1] = new double[3];
                        lforces[2] = new double[3];
                    }
                    MatrixByArr[,] lhess = (hessian == null) ? null : LinAlg.CreateMatrixArray(3, 3, new double[3, 3]);
                    frcfld.Compute(angles[i], lcoords, ref energy, ref lforces, ref lhess, pwfrc, pwspr);
                    HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                    if(forces != null)
                    {
                        forces[id0] += lforces[0];
                        forces[id1] += lforces[1];
                        forces[id2] += lforces[2];
                        forceij.AddAngle(id0, id1, id2, lcoords, lforces);
                        HDebug.AssertTolerance(0.00000001, lforces[0] + lforces[1] + lforces[2]);
                        double netstat = lforces[0].Dist2 + lforces[1].Dist2 + lforces[2].Dist2;
                        netstat_min = Math.Min(netstat_min, netstat);
                        netstat_max = Math.Max(netstat_max, netstat);
                        stat_min = Math.Min(stat_min, lforces[0].Dist);
                        stat_max = Math.Max(stat_max, lforces[0].Dist);
                        stat_min = Math.Min(stat_min, lforces[1].Dist);
                        stat_max = Math.Max(stat_max, lforces[1].Dist);
                        stat_min = Math.Min(stat_min, lforces[2].Dist);
                        stat_max = Math.Max(stat_max, lforces[2].Dist);
                    }
                    if(hessian != null)
                    {
                        hessian[id0, id0] += lhess[0, 0]; hessian[id0, id1] += lhess[0, 1]; hessian[id0, id2] += lhess[0, 2];
                        hessian[id1, id0] += lhess[1, 0]; hessian[id1, id1] += lhess[1, 1]; hessian[id1, id2] += lhess[1, 2];
                        hessian[id2, id0] += lhess[2, 0]; hessian[id2, id1] += lhess[2, 1]; hessian[id2, id2] += lhess[2, 2];
                    }
                }
            }
            return energy;
        }
        public double GetPotentialDihedrals(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                            Dictionary<string, object> cache, PwForceDecomposer forceij, double[,] pwfrc=null, double[,] pwspr=null)
        {
            List<ForceField.IDihedral> frcfld_dihedrals = SelectInFrcflds(frcflds, new List<ForceField.IDihedral>());
            double energy = 0;

            if(frcfld_dihedrals.Count == 0)
                return energy;

            double stat_min = double.MaxValue;
            double stat_max = double.MinValue;
            double netstat_min = double.MaxValue;
            double netstat_max = double.MinValue;

            Vector[] lcoords = new Vector[4];
            Vector[] lforces = (forces == null) ? null : new Vector[4];
            for(int i=0; i<dihedrals.Count; i++)
            {
                int id0 = dihedrals[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = dihedrals[i].atoms[1].ID; lcoords[1] = coords[id1];
                int id2 = dihedrals[i].atoms[2].ID; lcoords[2] = coords[id2];
                int id3 = dihedrals[i].atoms[3].ID; lcoords[3] = coords[id3];
                foreach(ForceField.IDihedral frcfld in frcfld_dihedrals)
                {
                    if(forces != null)
                    {
                        lforces[0] = new double[3];
                        lforces[1] = new double[3];
                        lforces[2] = new double[3];
                        lforces[3] = new double[3];
                    }
                    MatrixByArr[,] lhess = (hessian == null) ? null : LinAlg.CreateMatrixArray(4, 4, new double[3, 3]);
                    frcfld.Compute(dihedrals[i], lcoords, ref energy, ref lforces, ref lhess, pwfrc, pwspr);
                    HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                    if(forces != null)
                    {
                        forces[id0] += lforces[0];
                        forces[id1] += lforces[1];
                        forces[id2] += lforces[2];
                        forces[id3] += lforces[3];
                        forceij.AddDihedral(id0, id1, id2, id3, lcoords, lforces);
                        HDebug.AssertTolerance(0.00000001, lforces[0] + lforces[1] + lforces[2] + lforces[3]);
                        double netstat = lforces[0].Dist2 + lforces[1].Dist2 + lforces[2].Dist2 + lforces[3].Dist2;
                        netstat_min = Math.Min(netstat_min, netstat);
                        netstat_max = Math.Max(netstat_max, netstat);
                        stat_min = Math.Min(stat_min, lforces[0].Dist);
                        stat_max = Math.Max(stat_max, lforces[0].Dist);
                        stat_min = Math.Min(stat_min, lforces[1].Dist);
                        stat_max = Math.Max(stat_max, lforces[1].Dist);
                        stat_min = Math.Min(stat_min, lforces[2].Dist);
                        stat_max = Math.Max(stat_max, lforces[2].Dist);
                        stat_min = Math.Min(stat_min, lforces[3].Dist);
                        stat_max = Math.Max(stat_max, lforces[3].Dist);
                    }
                    if(hessian != null)
                    {
                        hessian[id0, id0] += lhess[0, 0]; hessian[id0, id1] += lhess[0, 1]; hessian[id0, id2] += lhess[0, 2]; hessian[id0, id3] += lhess[0, 3];
                        hessian[id1, id0] += lhess[1, 0]; hessian[id1, id1] += lhess[1, 1]; hessian[id1, id2] += lhess[1, 2]; hessian[id1, id3] += lhess[1, 3];
                        hessian[id2, id0] += lhess[2, 0]; hessian[id2, id1] += lhess[2, 1]; hessian[id2, id2] += lhess[2, 2]; hessian[id2, id3] += lhess[2, 3];
                        hessian[id3, id0] += lhess[3, 0]; hessian[id3, id1] += lhess[3, 1]; hessian[id3, id2] += lhess[3, 2]; hessian[id3, id3] += lhess[3, 3];
                    }
                }
            }
            return energy;
        }
        public double GetPotentialImpropers(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                            Dictionary<string, object> cache, PwForceDecomposer forceij, double[,] pwfrc=null, double[,] pwspr=null)
        {
            List<ForceField.IImproper> frcfld_impropers = SelectInFrcflds(frcflds, new List<ForceField.IImproper>());
            double energy = 0;

            if(frcfld_impropers.Count == 0)
                return energy;

            double stat_min = double.MaxValue;
            double stat_max = double.MinValue;
            double netstat_min = double.MaxValue;
            double netstat_max = double.MinValue;

            Vector[] lcoords = new Vector[4];
            Vector[] lforces = (forces == null) ? null : new Vector[4];
            for(int i=0; i<impropers.Count; i++)
            {
                int id0 = impropers[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = impropers[i].atoms[1].ID; lcoords[1] = coords[id1];
                int id2 = impropers[i].atoms[2].ID; lcoords[2] = coords[id2];
                int id3 = impropers[i].atoms[3].ID; lcoords[3] = coords[id3];
                foreach(ForceField.IImproper frcfld in frcfld_impropers)
                {
                    if(forces != null)
                    {
                        lforces[0] = new double[3];
                        lforces[1] = new double[3];
                        lforces[2] = new double[3];
                        lforces[3] = new double[3];
                    }
                    MatrixByArr[,] lhess = (hessian == null) ? null : LinAlg.CreateMatrixArray(4, 4, new double[3, 3]);
                    frcfld.Compute(impropers[i], lcoords, ref energy, ref lforces, ref lhess, pwfrc, pwspr);
                    HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                    if(forces != null)
                    {
                        forces[id0] += lforces[0];
                        forces[id1] += lforces[1];
                        forces[id2] += lforces[2];
                        forces[id3] += lforces[3];
                        forceij.AddImproper(id0, id1, id2, id3, lcoords, lforces);
                        HDebug.AssertTolerance(0.00000001, lforces[0] + lforces[1] + lforces[2] + lforces[3]);
                        double netstat = lforces[0].Dist2 + lforces[1].Dist2 + lforces[2].Dist2 + lforces[3].Dist2;
                        netstat_min = Math.Min(netstat_min, netstat);
                        netstat_max = Math.Max(netstat_max, netstat);
                        stat_min = Math.Min(stat_min, lforces[0].Dist);
                        stat_max = Math.Max(stat_max, lforces[0].Dist);
                        stat_min = Math.Min(stat_min, lforces[1].Dist);
                        stat_max = Math.Max(stat_max, lforces[1].Dist);
                        stat_min = Math.Min(stat_min, lforces[2].Dist);
                        stat_max = Math.Max(stat_max, lforces[2].Dist);
                        stat_min = Math.Min(stat_min, lforces[3].Dist);
                        stat_max = Math.Max(stat_max, lforces[3].Dist);
                    }
                    if(hessian != null)
                    {
                        hessian[id0, id0] += lhess[0, 0]; hessian[id0, id1] += lhess[0, 1]; hessian[id0, id2] += lhess[0, 2]; hessian[id0, id3] += lhess[0, 3];
                        hessian[id1, id0] += lhess[1, 0]; hessian[id1, id1] += lhess[1, 1]; hessian[id1, id2] += lhess[1, 2]; hessian[id1, id3] += lhess[1, 3];
                        hessian[id2, id0] += lhess[2, 0]; hessian[id2, id1] += lhess[2, 1]; hessian[id2, id2] += lhess[2, 2]; hessian[id2, id3] += lhess[2, 3];
                        hessian[id3, id0] += lhess[3, 0]; hessian[id3, id1] += lhess[3, 1]; hessian[id3, id2] += lhess[3, 2]; hessian[id3, id3] += lhess[3, 3];
                    }
                }
            }
            return energy;
        }
        public double GetPotentialNonbondeds(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                             Dictionary<string, object> cache, PwForceDecomposer forceij, double[,] pwfrc=null, double[,] pwspr=null)
        {
            List<ForceField.INonbonded> frcfld_nonbondeds = SelectInFrcflds(frcflds, new List<ForceField.INonbonded>());
            double energy = 0;

            if(frcfld_nonbondeds.Count == 0)
                return energy;

            Vector[] lcoords = new Vector[2];
            Vector[] lforces = (forces == null) ? null : new Vector[2];

            Nonbondeds_v1 nonbondeds = null;
            if(cache == null)
            {
                cache = new Dictionary<string, object>();
            }

            if(cache.ContainsKey("all nonbondeds"))
            {
                // compute all non-bondeds
                double nonbondeds_maxdist = double.PositiveInfinity;
                nonbondeds = new Nonbondeds_v1(atoms, size, nonbondeds_maxdist);
                nonbondeds.UpdateNonbondeds(coords, 0);
                cache.Add("nonbondeds", nonbondeds);
            }
            else if(cache.ContainsKey("nonbondeds"))
            {
                // use existing cached nonbondeds, but update it as well
                nonbondeds = (Nonbondeds_v1)cache["nonbondeds"];
                nonbondeds.UpdateNonbondeds(coords, 0.01);
            }
            else
            {
                // create default nonbondeds, and add it into cache
                nonbondeds = new Nonbondeds_v1(atoms, size, 12);
                nonbondeds.UpdateNonbondeds(coords, 0);
                cache.Add("nonbondeds", nonbondeds);
            }

            double stat_min = double.MaxValue;
            double stat_max = double.MinValue;
            double netstat_min = double.MaxValue;
            double netstat_max = double.MinValue;
            foreach(Nonbonded nonbond in nonbondeds)
            {
                ///{   // this removes interactions in-between rigid atoms.
                ///    Atom atom0 = nonbond.atoms[0];
                ///    Atom atom1 = nonbond.atoms[1];
                ///    HashSet<Atom> atomsInRigid0 = GetAtomsInRigid(atom0);
                ///    if(atomsInRigid0.Contains(atom1))
                ///        continue;
                ///}
                int id0 = nonbond.atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = nonbond.atoms[1].ID; lcoords[1] = coords[id1];
                foreach(ForceField.INonbonded frcfld in frcfld_nonbondeds)
                {
                    if(forces != null)
                    {
                        lforces[0] = new double[3];
                        lforces[1] = new double[3];
                    }
                    MatrixByArr[,] lhess = (hessian == null) ? null : LinAlg.CreateMatrixArray(2, 2, new double[3, 3]);
                    double[,] lpwfrc = new double[2, 2];
                    double[,] lpwspr = new double[2, 2];
                    frcfld.Compute(nonbond, lcoords, ref energy, ref lforces, ref lhess, lpwfrc, lpwspr);
                    HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                    if(forces != null)
                    {
                        forces[id0] += lforces[0];
                        forces[id1] += lforces[1];
                        if(forceij != null) forceij.AddNonbonded(id0, id1, lcoords, lforces);
                        HDebug.AssertTolerance(0.00000001, lforces[0].Dist2 - lforces[1].Dist2);
                        double netstat = lforces[0].Dist2 + lforces[1].Dist2;
                        netstat_min = Math.Min(netstat_min, netstat);
                        netstat_max = Math.Max(netstat_max, netstat);
                        stat_min = Math.Min(stat_min, lforces[0].Dist);
                        stat_max = Math.Max(stat_max, lforces[0].Dist);
                        stat_min = Math.Min(stat_min, lforces[1].Dist);
                        stat_max = Math.Max(stat_max, lforces[1].Dist);
                    }
                    if(hessian != null)
                    {
                        hessian[id0, id0] += lhess[0, 0]; hessian[id0, id1] += lhess[0, 1];
                        hessian[id1, id0] += lhess[1, 0]; hessian[id1, id1] += lhess[1, 1];
                    }
                    if(pwfrc != null)
                    {
                        pwfrc[id0, id0] += lpwfrc[0, 0]; pwfrc[id0, id1] += lpwfrc[0, 1];
                        pwfrc[id1, id0] += lpwfrc[1, 0]; pwfrc[id1, id1] += lpwfrc[1, 1];
                    }
                    if(pwspr != null)
                    {
                        pwspr[id0, id0] += lpwspr[0, 0]; pwspr[id0, id1] += lpwspr[0, 1];
                        pwspr[id1, id0] += lpwspr[1, 0]; pwspr[id1, id1] += lpwspr[1, 1];
                    }
                }
            }
            double stat14_min = double.MaxValue;
            double stat14_max = double.MinValue;
            double netstat14_min = double.MaxValue;
            double netstat14_max = double.MinValue;
            foreach(Nonbonded14 nonbond in nonbonded14s)
            {
                int id0 = nonbond.atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = nonbond.atoms[1].ID; lcoords[1] = coords[id1];
                foreach(ForceField.INonbonded frcfld in frcfld_nonbondeds)
                {
                    if(forces != null)
                    {
                        lforces[0] = new double[3];
                        lforces[1] = new double[3];
                    }
                    MatrixByArr[,] lhess = (hessian == null) ? null : LinAlg.CreateMatrixArray(2, 2, new double[3, 3]);
                    double[,] lpwfrc = new double[2, 2];
                    double[,] lpwspr = new double[2, 2];
                    frcfld.Compute(nonbond, lcoords, ref energy, ref lforces, ref lhess, pwfrc, pwspr);
                    HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                    if(forces != null)
                    {
                        forces[id0] += lforces[0];
                        forces[id1] += lforces[1];
                        if(forceij != null) forceij.AddNonbonded14(id0, id1, lcoords, lforces);
                        HDebug.AssertTolerance(0.00000001, lforces[0].Dist2 - lforces[1].Dist2);
                        double netstat = lforces[0].Dist2 + lforces[1].Dist2;
                        netstat14_min = Math.Min(netstat14_min, netstat);
                        netstat14_max = Math.Max(netstat14_max, netstat);
                        stat14_min = Math.Min(stat14_min, lforces[0].Dist);
                        stat14_max = Math.Max(stat14_max, lforces[0].Dist);
                        stat14_min = Math.Min(stat14_min, lforces[1].Dist);
                        stat14_max = Math.Max(stat14_max, lforces[1].Dist);
                    }
                    if(hessian != null)
                    {
                        hessian[id0, id0] += lhess[0, 0]; hessian[id0, id1] += lhess[0, 1];
                        hessian[id1, id0] += lhess[1, 0]; hessian[id1, id1] += lhess[1, 1];
                    }
                    if(pwfrc != null)
                    {
                        pwfrc[id0, id0] += lpwfrc[0, 0]; pwfrc[id0, id1] += lpwfrc[0, 1];
                        pwfrc[id1, id0] += lpwfrc[1, 0]; pwfrc[id1, id1] += lpwfrc[1, 1];
                    }
                    if(pwspr != null)
                    {
                        pwspr[id0, id0] += lpwspr[0, 0]; pwspr[id0, id1] += lpwspr[0, 1];
                        pwspr[id1, id0] += lpwspr[1, 0]; pwspr[id1, id1] += lpwspr[1, 1];
                    }
                }
            }
            return energy;
        }
        public double GetPotentialCustoms(List<ForceField.IForceField> frcflds, Vector[] coords, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                          Dictionary<string, object> cache, PwForceDecomposer forceij, double[,] pwfrc=null, double[,] pwspr=null)
        {
            List<ForceField.ICustom> frcfld_customs = SelectInFrcflds(frcflds, new List<ForceField.ICustom>());

            double energy = 0;
            foreach(ForceField.ICustom frcfld in frcfld_customs)
            {
                Vector[] lforces = (forces == null) ? null : GetVectorsZero();
                frcfld.Compute(atoms, coords, ref energy, ref lforces, ref hessian, pwfrc, pwspr);
                HDebug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                if(forces != null)
                {
                    for(int i=0; i<size; i++) forces[i] += lforces[i];
                    forceij.AddCustom(coords, lforces);
                }

            }
            return energy;
        }
    }
}
