using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public int Minimize_ConjugateGradient_AtomwiseUpdate(
                            List<ForceField.IForceField> frcflds
                            , double  threshold         = 0.001 // * threshold for forces.NormInf
                            , double? k                 = null  // * step step
                                                                // 0.0001
                            , double  max_atom_movement = 0.1   // * maximum atom movement
                            , int?    max_iteration     = null  // null for the infinite iteration until converged
                            , bool[]  atomsMovable      = null  // * selection of movable atoms
                                                                //   move all atoms if (atomsMovable == null) or (atomsMovable[all] == true)
                                                                //   move only atom whose (atomsMovable[id] == true)
                            , IMinimizeLogger logger    = null  // * write log
                                                                // = new MinimizeLogger_PrintEnergyForceMag()
                            , InfoPack extra            = null  // get extra information
                            , bool?    doSteepDeescent  = null  // null or true for default

                            , HPack<double> optOutEnergy        = null  // optional output for final energy
                            , List<Vector> optOutForces        = null  // optional output for final force vectors
                            , HPack<double> optOutForcesNorm1   = null  // optional output for norm of final force vectors
                            , HPack<double> optOutForcesNorm2   = null  // optional output for norm of final force vectors
                            , HPack<double> optOutForcesNormInf = null  // optional output for norm of final force vectors
                            )
        {
            if(doSteepDeescent == null)
                doSteepDeescent = true;
            if(k == null)
            {
                k = double.MaxValue;
                foreach(ForceField.IForceField frcfld in frcflds)
                {
                    double? kk = frcfld.GetDefaultMinimizeStep();
                    if(kk.HasValue)
                        k = Math.Min(k.Value, kk.Value);
                }
            }
            int iter = 0;

            // 0. Initial configuration of atoms
            Vector[] coords = GetCoords();
            if(atomsMovable == null)
            {
                atomsMovable = new bool[size];
                for(int i=0; i<size; i++)
                    atomsMovable[i] = true;
            }

            Vectors h = GetVectorsZero();
            Vectors forces = Vector.NewVectors(size, new double[3]);
            Vectors moves  = Vector.NewVectors(size, new double[3]);
            Nonbondeds_v1 nonbondeds = null;
//            Dictionary<string,object> cache = new Dictionary<string, object>();
            double energy = GetPotentialUpdated(frcflds, null, null, null, coords, forces, ref nonbondeds);
            double forces_NormInf = NormInf(forces, atomsMovable);
            double forces_Norm1   = Norm(1, forces, atomsMovable);
            double forces_Norm2   = Norm(2, forces, atomsMovable);
            Vector[] forces0 = forces;
            double   energy0 = energy;
            Vectors  moves0  = moves;
            double leastMove = 0.000001;
            while(true)
            {
                if(forces.IsComputable == false)
                {
                    System.Console.Error.WriteLine("non-computable components while doing steepest-descent");
                    HEnvironment.Exit(0);
                }
                if(logger != null)
                {
                    logger.log(iter, coords, energy, forces, atomsMovable);
                    logger.logTrajectory(this, iter, coords);
                }
                // 1. Save the position of atoms
                // 2. Calculate the potential energy of system and the net forces on atoms
                // 3. Check if every force reaches to zero,
                //    , and END if yes
                bool stopIteration = false;
                if(forces_NormInf < threshold) stopIteration = true;
                if((max_iteration != null) && (iter>=max_iteration.Value)) stopIteration = true;
                if(stopIteration)
                {
                    // double check
                    //cache = new Dictionary<string, object>(); // reset cache
                    //energy = GetPotential(frcflds, coords, out forces, cache);
                    nonbondeds = null;
                    energy = GetPotentialUpdated(frcflds, null, null, null, coords, forces, ref nonbondeds);
                    forces_NormInf = NormInf(forces, atomsMovable);
                    forces_Norm1   = Norm(1, forces, atomsMovable);
                    forces_Norm2   = Norm(2, forces, atomsMovable);

                    if(forces_NormInf < threshold)
                    {
                        if(iter != 1)
                        {
                            SetCoords(coords);
                        }

                        {
                            if(optOutEnergy        != null) optOutEnergy.value = energy;
                            if(optOutForces        != null) {optOutForces.Clear(); optOutForces.AddRange(forces.ToArray()); }
                            if(optOutForcesNorm1   != null) optOutForcesNorm1  .value = forces_Norm1  ;
                            if(optOutForcesNorm2   != null) optOutForcesNorm2  .value = forces_Norm2  ;
                            if(optOutForcesNormInf != null) optOutForcesNormInf.value = forces_NormInf;
                        }
                        return iter;
                    }
                }
                // 4. Move atoms with conjugated gradient
                Vectors coords_prd;
                double kk;
                {
                    if((iter > 0) && (iter % 100 == 0))
                    {
                        //cache = new Dictionary<string, object>(); // reset cache
                        nonbondeds = null;
                    }
                    if(iter > 1)
                    {
                        HDebug.Assert(forces0 != null);
                        double r        = Vectors.VtV(forces, forces).Sum() / Vectors.VtV(forces0, forces0).Sum();
                        h               = forces + r * h;
                        kk              = k.Value;
                        double hNormInf = NormInf(h, atomsMovable);
                        if(kk*hNormInf > max_atom_movement)
                            // make the maximum movement as atomsMovable
                            kk = max_atom_movement/(hNormInf);
                        moves = moves0.Clone();
                        coords_prd = AddConditional(coords, atomsMovable, moves, kk * h, leastMove);
                    }
                    else
                    {
                        // same to the steepest descent for the first iteration
                        h               = forces;
                        kk              = k.Value;
                        double hNormInf = NormInf(h, atomsMovable);
                        if(kk*hNormInf > max_atom_movement)
                            // make the maximum movement as atomsMovable
                            kk = max_atom_movement/(hNormInf);
                        //double kk = (k*h.NormsInf().NormInf() < max_atom_movement) ? k : (max_atom_movement/h.NormsInf().NormInf());
                        //double   kk = (k.Value*NormInf(h,atomsMovable) < max_atom_movement)? k.Value : (max_atom_movement/NormInf(h, atomsMovable));
                        moves = moves0.Clone();
                        coords_prd = AddConditional(coords, atomsMovable, moves, kk * h, leastMove);
                    }
                }
                // 5. Predict energy or forces on atoms
                Vectors forces_prd = forces.Clone();
                double energy_prd = GetPotentialUpdated(frcflds, energy, coords, forces, coords_prd, forces_prd, ref nonbondeds); iter++;
                //double energy_prd = GetPotential(frcflds, coords_prd, out forces_prd, cache); iter++;

                double forces_prd_NormInf = NormInf(forces_prd, atomsMovable);
                double forces_prd_Norm1   = Norm(1, forces_prd, atomsMovable);
                double forces_prd_Norm2   = Norm(2, forces_prd, atomsMovable);
                // 6. Check if the predicted forces or energy will exceed over the limit
                //    , and goto 1 if no
                doSteepDeescent = true;
                //if((doSteepDeescent == false) || ((energy_prd <= energy) && (forces_prd_NormInf < forces_NormInf+1.0))
                if((energy_prd < energy+0.1) && (forces_prd_NormInf < forces_NormInf+0.0001))
                {
                    energy0 = energy;
                    forces0 = forces;
                    moves0  = moves;
                    coords = coords_prd;
                    forces = forces_prd;
                    energy = energy_prd;
                    forces_NormInf = forces_prd_NormInf;
                    forces_Norm1   = forces_prd_Norm1;
                    forces_Norm2   = forces_prd_Norm2;
                    continue;
                }
                if(logger != null)
                    logger.log(iter, coords_prd, energy_prd, forces_prd, atomsMovable, "will do steepest");
                // 7. Back to saved configuration
                // 8. Move atoms with simple gradient
                {
                    // same to the steepest descent
                    h               = forces;
                    kk              = k.Value;
                    double hNormInf = NormInf(h, atomsMovable);
                    if(kk*hNormInf > max_atom_movement)
                        // make the maximum movement as atomsMovable
                        kk = max_atom_movement/(hNormInf);
                    moves = moves0.Clone();
                    coords_prd = AddConditional(coords, atomsMovable, moves, kk * h, leastMove);
                }
                //energy_prd = GetPotential(frcflds, coords_prd, out forces_prd, cache);
                energy_prd = GetPotentialUpdated(frcflds, energy, coords, forces, coords_prd, forces_prd, ref nonbondeds);
                forces_prd_NormInf = NormInf(forces_prd, atomsMovable);
                forces_prd_Norm1   = Norm(1, forces_prd, atomsMovable);
                forces_prd_Norm2   = Norm(2, forces_prd, atomsMovable);

                energy0 = energy;
                forces0 = forces;
                moves0  = moves;
                coords = coords_prd;
                forces = forces_prd;
                energy = energy_prd;
                forces_NormInf = forces_prd_NormInf;
                forces_Norm1   = forces_prd_Norm1;
                forces_Norm2   = forces_prd_Norm2;
                // 9. goto 1
            }
        }
    }
}
