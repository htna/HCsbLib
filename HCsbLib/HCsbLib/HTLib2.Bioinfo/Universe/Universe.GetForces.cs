using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public static bool GetPotentialUpdated_SelfTestDo = HDebug.IsDebuggerAttached;
        public static void GetPotentialUpdated_SelfTest(List<ForceField.IForceField> frcflds
                                                        , double? energy0, Vector[] coords0, Vector[] forces0
                                                        , Vector[] coords, Vector[] forces)
        {
            //GetPotentialUpdated_SelfTestDo = false;
            if(energy0 == null)
            {
                HDebug.Assert(coords0 == null, forces0 == null);
                return;
            }
        }

        static double GetPotentialUpdated_ProbToCheckWithGetPotential = 0;//1;//0.1;
        public double GetPotentialUpdated(List<ForceField.IForceField> frcflds
                                , double? energy0, Vector[] coords0, Vector[] forces0
                                , Vector[] coords,  Vector[] forces
                                , ref Nonbondeds_v1 nonbondeds
                                )
        {
            if(GetPotentialUpdated_SelfTestDo == true)
                #region selftest
            {
                GetPotentialUpdated_SelfTest(frcflds, energy0, coords0, forces0, coords,  forces);
            }
                #endregion

            bool[] updated = new bool[size];
            if((energy0 == null) || (nonbondeds == null))
            {
                HDebug.AssertIf(energy0 == null, coords0 == null);
                HDebug.AssertIf(energy0 == null, forces0 == null);
                energy0 = 0;
                coords0 = null;
                forces0 = null;
                for(int i=0; i<size; i++) forces[i] = new double[3];
                for(int i=0; i<size; i++) updated[i] = true;
            }
            else
            {
                HDebug.Assert(size == coords0.Length);
                HDebug.Assert(size == forces0.Length);
                HDebug.Assert(size == coords.Length);
                HDebug.Assert(size == forces.Length);
                for(int i=0; i<size; i++) forces[i] = forces0[i].Clone();

                int countskip = 0;
                //double[] dist2s = new double[size];
                //for(int i=0; i<size; i++)
                //    dist2s[i] = (coords0[i] - coords[i]).Dist2;
                //int[] idxsorted = dist2s.IdxSorted();
                //for(int i=0; i<size; i++)
                //    updated[i] = (dist2s[i] > (0.000001*0.000001));
                //for(int i=size/2; i<size; i++)
                //    updated[idxsorted[i]] = true;
                for(int i=0; i<size; i++) updated[i] = (coords0[i] != coords[i]);
                for(int i=0; i<size; i++) countskip += (updated[i] == false) ? 1 : 0;
                if((size-countskip)*10 > size)
                {
                    energy0 = 0;
                    coords0 = null;
                    forces0 = null;
                    for(int i=0; i<size; i++) forces[i] = new double[3];
                    for(int i=0; i<size; i++) updated[i] = true;
                }
                System.Console.Write(" countskip({0:000}) ", countskip);
            }
            Vector[] dforces = GetVectorsZero();

            double denergy = 0;
            denergy += GetPotentialUpdated_ComputeCustomsBond     (frcflds, updated, 0, coords0, coords, dforces);
            denergy += GetPotentialUpdated_ComputeCustomsAngle    (frcflds, updated, 0, coords0, coords, dforces);
            denergy += GetPotentialUpdated_ComputeCustomsDihedral (frcflds, updated, 0, coords0, coords, dforces);
            denergy += GetPotentialUpdated_ComputeCustomsImproper (frcflds, updated, 0, coords0, coords, dforces);
            denergy += GetPotentialUpdated_ComputeCustomsNonbonded(frcflds, updated, 0, coords0, coords, dforces, ref nonbondeds);
            //energy += GetPotentialUpdated_ComputeCustomsCustoms  (frcflds, updated, 0, coords0, forces0, coords, forces);
            
            double energy = energy0.Value + denergy;
            for(int i=0; i<size; i++)
                forces[i] += dforces[i];

            #region commented from threading
            //Nonbondeds lnonbondeds = nonbondeds;
            //
            //double energy = energy0.Value;
            //
            //object lockobj = new object();
            //System.Threading.Tasks.ParallelOptions parallelOptions = new ParallelOptions();
            ////parallelOptions.MaxDegreeOfParallelism = 1;
            //Parallel.ForEach(compunits_bonded, parallelOptions, delegate(GetForcesCompUnit compunit)
            //{
            //    if(compunit == null)
            //    {
            //        // in the first iteration, collect nonbonded components
            //        GetForces_CollectCompUnitNonbonded(frcflds, updated, coords0, coords, ref lnonbondeds, compunits_nonbonded);
            //    }
            //    else
            //    {
            //        Triple<double, int[], Vector[]> denergy_idx_dforces = compunit.Compute(coords0, coords);
            //        double   denergy = denergy_idx_dforces.first;
            //        int[]    idx     = denergy_idx_dforces.second;
            //        Vector[] dforces = denergy_idx_dforces.third;
            //        Debug.Assert(idx.Length == dforces.Length);
            //        lock(lockobj)
            //        {
            //            energy += denergy;
            //            for(int i=0; i<idx.Length; i++)
            //                forces[idx[i]] += dforces[i];
            //        }
            //    }
            //});
            //nonbondeds = lnonbondeds;
            //Parallel.ForEach(compunits_nonbonded, parallelOptions, delegate(GetForcesCompUnit compunit)
            //{
            //    Triple<double, int[], Vector[]> denergy_idx_dforces = compunit.Compute(coords0, coords);
            //    double   denergy = denergy_idx_dforces.first;
            //    int[]    idx     = denergy_idx_dforces.second;
            //    Vector[] dforces = denergy_idx_dforces.third;
            //    Debug.Assert(idx.Length == dforces.Length);
            //    lock(lockobj)
            //    {
            //        energy += denergy;
            //        for(int i=0; i<idx.Length; i++)
            //            forces[idx[i]] += dforces[i];
            //    }
            //});
            #endregion

            if(HDebug.IsDebuggerAttachedWithProb(GetPotentialUpdated_ProbToCheckWithGetPotential))
                #region check force with GetPotential(...)
            {
                //Vector[] _forces0  = GetVectorsZero();
                //Matrix   _hessian0 = null;
                //double   _energy0  = GetPotential(frcflds, coords0, ref _forces0, ref _hessian0, new Dictionary<string,object>());
                //Debug.AssertTolerance(0.00000001, energy0 - _energy0);
                //Debug.AssertTolerance(0.00000001, Vector.Sub(forces0, _forces0));

                Vector[] _forces  = GetVectorsZero();
                MatrixByArr   _hessian = null;
                double   _energy  = GetPotential(frcflds, coords, ref _forces, ref _hessian, new Dictionary<string, object>());
                HDebug.AssertTolerance(0.00000001, energy  - _energy);
                HDebug.AssertToleranceVector(0.00000001, Vector.Sub(forces , _forces));
            }
                #endregion

            //if(cache != null)
            //{
            //    if(cache.ContainsKey("energy_bonds     ") == false) cache.Add("energy_bonds     ", 0); cache["energy_bonds     "] = energy_bonds     ;
            //    if(cache.ContainsKey("energy_angles    ") == false) cache.Add("energy_angles    ", 0); cache["energy_angles    "] = energy_angles    ;
            //    if(cache.ContainsKey("energy_dihedrals ") == false) cache.Add("energy_dihedrals ", 0); cache["energy_dihedrals "] = energy_dihedrals ;
            //    if(cache.ContainsKey("energy_impropers ") == false) cache.Add("energy_impropers ", 0); cache["energy_impropers "] = energy_impropers ;
            //    if(cache.ContainsKey("energy_nonbondeds") == false) cache.Add("energy_nonbondeds", 0); cache["energy_nonbondeds"] = energy_nonbondeds;
            //    if(cache.ContainsKey("energy_customs   ") == false) cache.Add("energy_customs   ", 0); cache["energy_customs   "] = energy_customs   ;
            //}
            return energy;
        }
        public delegate void GetPotentialUpdated_ComputeFunc<FFUNIT>(FFUNIT ffUnit, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null);
        public static double GetPotentialUpdated_Compute<FFUNIT>( FFUNIT ffUnit, GetPotentialUpdated_ComputeFunc<FFUNIT> ffCompute
                                                                , Vector[] coords0, Vector[] coords
                                                                , Vector[] dforces
                                                                , int[] buffIdx, Vector[] buffCoords,   Vector[] buffForces
                                                                , bool compOld, bool compNew
                                                                )
            where FFUNIT : Universe.AtomPack
        {
            int length = ffUnit.atoms.Length;
            int   [] idx     = buffIdx;    for(int i=0; i<length; i++) idx[i] = ffUnit.atoms[i].ID;
            Vector[] lcoords = buffCoords;
            Vector[] lforces = buffForces;
            MatrixByArr[,] lhessian = null;
            double denergy = 0;
            if(compNew) //(compOpt == CompOpt.compBothOldNew || compOpt == CompOpt.compOnlyNew)
            {
                double lenergy=0;
                for(int i=0; i<length; i++) lcoords[i] = coords[idx[i]];
                for(int i=0; i<length; i++) lforces[i].SetZero();
                ffCompute(ffUnit, lcoords, ref lenergy, ref lforces, ref lhessian);
                denergy += lenergy;
                HDebug.AssertTolerance(0.00000001, lforces.Take(length).Mean());
                for(int i=0; i<length; i++) dforces[idx[i]] += lforces[i];
            }
            if(compOld && coords0 != null) //(coords0 != null && (compOpt == CompOpt.compBothOldNew || compOpt == CompOpt.compOnlyOld))
            {
                double lenergy=0;
                for(int i=0; i<length; i++) lcoords[i] = coords0[idx[i]];
                for(int i=0; i<length; i++) lforces[i].SetZero();
                ffCompute(ffUnit, lcoords, ref lenergy, ref lforces, ref lhessian);
                denergy -= lenergy;
                HDebug.AssertTolerance(0.00000001, lforces.Take(length).Mean());
                for(int i=0; i<length; i++) dforces[idx[i]] -= lforces[i];
            }
            return denergy;
        }

        public double GetPotentialUpdated_ComputeCustomsBond( List<ForceField.IForceField> frcflds, bool[] updated
                                                            , double energy0, Vector[] coords0, Vector[] coords
                                                            , Vector[] dforces
                                                            )
        {
            List<ForceField.IBond> frcfld_bonds = SelectInFrcflds(frcflds, new List<ForceField.IBond>());
            Vector[] buffCoords = Vector.NewVectors(2, new double[3]);
            Vector[] buffForces = Vector.NewVectors(2, new double[3]);
            int[]    buffIdx    = new int   [2];
            double   energy = energy0;
            for(int i=0; i<bonds.Count; i++)
            {
                bool updated0 = updated[bonds[i].atoms[0].ID];
                bool updated1 = updated[bonds[i].atoms[1].ID];
                if(updated0 || updated1)
                    foreach(ForceField.IBond frcfld in frcfld_bonds)
                    {
                        energy += GetPotentialUpdated_Compute(bonds[i], frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, true, true);
                    }
            }
            return energy;
        }
        public double GetPotentialUpdated_ComputeCustomsAngle( List<ForceField.IForceField> frcflds, bool[] updated
                                                             , double energy0, Vector[] coords0, Vector[] coords
                                                             , Vector[] dforces
                                                             )
        {
            List<ForceField.IAngle> frcfld_angles = SelectInFrcflds(frcflds, new List<ForceField.IAngle>());
            Vector[] buffCoords = Vector.NewVectors(3, new double[3]);
            Vector[] buffForces = Vector.NewVectors(3, new double[3]);
            int[]    buffIdx    = new int   [3];
            double   energy = energy0;
            for(int i=0; i<angles.Count; i++)
            {
                bool updated0 = updated[angles[i].atoms[0].ID];
                bool updated1 = updated[angles[i].atoms[1].ID];
                bool updated2 = updated[angles[i].atoms[2].ID];
                if(updated0 || updated1 || updated2)
                    foreach(ForceField.IAngle frcfld in frcfld_angles)
                    {
                        energy += GetPotentialUpdated_Compute(angles[i], frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, true, true);
                    }
            }
            return energy;
        }
        public double GetPotentialUpdated_ComputeCustomsDihedral( List<ForceField.IForceField> frcflds, bool[] updated
                                                                , double energy0, Vector[] coords0, Vector[] coords
                                                                , Vector[] dforces
                                                                )
        {
            List<ForceField.IDihedral> frcfld_dihedrals = SelectInFrcflds(frcflds, new List<ForceField.IDihedral>());
            Vector[] buffCoords = Vector.NewVectors(4, new double[3]);
            Vector[] buffForces = Vector.NewVectors(4, new double[3]);
            int[]    buffIdx    = new int   [4];
            double   energy = energy0;
            for(int i=0; i<dihedrals.Count; i++)
            {
                bool updated0 = updated[dihedrals[i].atoms[0].ID];
                bool updated1 = updated[dihedrals[i].atoms[1].ID];
                bool updated2 = updated[dihedrals[i].atoms[2].ID];
                bool updated3 = updated[dihedrals[i].atoms[3].ID];
                if(updated0 || updated1 || updated2 || updated3)
                    foreach(ForceField.IDihedral frcfld in frcfld_dihedrals)
                    {
                        energy += GetPotentialUpdated_Compute(dihedrals[i], frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, true, true);
                    }
            }
            return energy;
        }
        public double GetPotentialUpdated_ComputeCustomsImproper( List<ForceField.IForceField> frcflds, bool[] updated
                                                                , double energy0, Vector[] coords0, Vector[] coords
                                                                , Vector[] dforces
                                                                )
        {
            List<ForceField.IImproper> frcfld_impropers = SelectInFrcflds(frcflds, new List<ForceField.IImproper>());
            Vector[] buffCoords = Vector.NewVectors(4, new double[3]);
            Vector[] buffForces = Vector.NewVectors(4, new double[3]);
            int[]    buffIdx    = new int   [4];
            double   energy = energy0;
            for(int i=0; i<impropers.Count; i++)
            {
                bool updated0 = updated[impropers[i].atoms[0].ID];
                bool updated1 = updated[impropers[i].atoms[1].ID];
                bool updated2 = updated[impropers[i].atoms[2].ID];
                bool updated3 = updated[impropers[i].atoms[3].ID];
                if(updated0 || updated1 || updated2 || updated3)
                    foreach(ForceField.IImproper frcfld in frcfld_impropers)
                    {
                        energy += GetPotentialUpdated_Compute(impropers[i], frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, true, true);
                    }
            }
            return energy;
        }
        public double GetPotentialUpdated_ComputeCustomsNonbonded( List<ForceField.IForceField> frcflds, bool[] updated
                                                                  , double energy0, Vector[] coords0, Vector[] coords
                                                                  , Vector[] dforces
                                                                  , ref Nonbondeds_v1 nonbondeds
                                                                  )
            //, Vector[] coords0, Vector[] coords, ref Nonbondeds nonbondeds, List<GetForcesCompUnit> compunits)
        {
            List<ForceField.INonbonded> frcfld_nonbondeds = SelectInFrcflds(frcflds, new List<ForceField.INonbonded>());
            Vector[] buffCoords = Vector.NewVectors(2, new double[3]);
            Vector[] buffForces = Vector.NewVectors(2, new double[3]);
            int[]    buffIdx    = new int[2];
            double   energy = energy0;
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Nonbonded14
            {
                foreach(Nonbonded14 nonbond in nonbonded14s)
                {
                    bool updated0 = updated[nonbond.atoms[0].ID];
                    bool updated1 = updated[nonbond.atoms[1].ID];
                    if(updated0 || updated1)
                        foreach(ForceField.INonbonded frcfld in frcfld_nonbondeds)
                        {
                            energy += GetPotentialUpdated_Compute(nonbond, frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, true, true);
                        }
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Nonbonded
            {
                ///////////////////////////////////////////
                // compute old nonbondeds
                if(nonbondeds == null)
                {
                    ///////////////////////////////////////////
                    // build nonbondeds structure
                    HDebug.Assert(nonbondeds == null);
                    double nonbondeds_maxdist = 12;
                    nonbondeds = new Nonbondeds_v1(atoms, size, nonbondeds_maxdist);
                    nonbondeds.UpdateNonbondeds(coords, 0);
                }
                else
                {
                    ///////////////////////////////////////////
                    // remove updated old nonbondeds structure
                    if(coords0 != null)
                    {
                        foreach(Nonbonded nonbond in nonbondeds)
                        {
                            bool updated0 = updated[nonbond.atoms[0].ID];
                            bool updated1 = updated[nonbond.atoms[1].ID];
                            if(updated0 || updated1)
                                foreach(ForceField.INonbonded frcfld in frcfld_nonbondeds)
                                {
                                    energy += GetPotentialUpdated_Compute(nonbond, frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, true, false);
                                }
                        }
                    }
                    ///////////////////////////////////////////
                    // updated nonbondeds structure
                    nonbondeds.UpdateNonbondeds(coords, 0.01);
                }
                ///////////////////////////////////////////
                // compute new nonbondeds
                foreach(Nonbonded nonbond in nonbondeds)
                {
                    bool updated0 = updated[nonbond.atoms[0].ID];
                    bool updated1 = updated[nonbond.atoms[1].ID];
                    if(updated0 || updated1)
                        foreach(ForceField.INonbonded frcfld in frcfld_nonbondeds)
                        {
                            energy += GetPotentialUpdated_Compute(nonbond, frcfld.Compute, coords0, coords, dforces, buffIdx, buffCoords, buffForces, false, true);
                        }
                }
            }
            return energy;
        }
        public double GetPotentialUpdated_ComputeCustoms( List<ForceField.IForceField> frcflds, bool[] updated
                                                        , double energy0, Vector[] coords0, Vector[] forces0
                                                        , Vector[] coords, Vector[] forces
                                                        )
        {
            List<ForceField.ICustom> frcfld_customs = SelectInFrcflds(frcflds, new List<ForceField.ICustom>());

            double energy = energy0;
            /*
            Matrix[,] hessian = null;

            foreach(ForceField.ICustom frcfld in frcfld_customs)
            {
                Vector[] lforces = (forces == null) ? null : GetVectorsZero();
                frcfld.Compute(atoms, coords, ref energy, ref lforces, ref hessian);
                Debug.Assert(double.IsNaN(energy) == false, double.IsInfinity(energy) == false);
                if(forces != null)
                {
                    for(int i=0; i<size; i++) forces[i] += lforces[i];
                }
            }
            return denergy;


            List<ForceField.ICustom> frcfld_customs = SelectInFrcflds(frcflds, new List<ForceField.ICustom>());
            Vector[] buffCoords = new Vector[4];
            Vector[] buffForces = new Vector[4];
            int[]    buffIdx    = new int   [4];
            double   energy = energy0;
            for(int i=0; i<impropers.Count; i++)
            {
                bool updated0 = updated[impropers[i].atoms[0].ID];
                bool updated1 = updated[impropers[i].atoms[1].ID];
                bool updated2 = updated[impropers[i].atoms[2].ID];
                bool updated3 = updated[impropers[i].atoms[3].ID];
                if(updated0 || updated1 || updated2 || updated3)
                    foreach(ForceField.IImproper frcfld in frcfld_impropers)
                    {
                        energy += GetPotentialUpdated_Compute(impropers[i], frcfld.Compute, coords0, forces0, coords, forces, buffIdx, buffCoords, buffForces, true, true);
                    }
            }
            */
            return energy;
        }
    }
}
