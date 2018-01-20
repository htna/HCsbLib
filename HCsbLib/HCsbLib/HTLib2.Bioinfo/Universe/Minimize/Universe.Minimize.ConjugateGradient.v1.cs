using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public class ConjugateGradientOption
        {
          //int    iterInitial       = 0;                                        // * index of initial iteration
          //double k                 = 0.0001;                                   // * step step
          //double max_atom_movement = 0.1;                                      // * maximum atom movement
          //double threshold         = 0.001;                                    // * threshold for forces.NormInf
          //int    randomPurturb     = 0;                                        // * add random purturbation when (iter % randomPurturb == 0)
          //                                                                     //   no random purturbation if (randomPurturb == 0)
          //bool[] atomsMovable      = null;                                     // * selection of movable atoms
                                                                                 //   move all atoms if (atomsMovable == null) or (atomsMovable[all] == true)
                                                                                 //   move only atom whose (atomsMovable[id] == true)
            IMinimizeLogger logger   = new MinimizeLogger_PrintEnergyForceMag(); // * write log
        }
        public int Minimize_ConjugateGradient_v1(int iterInitial,
                                                 List<ForceField.IForceField> frcflds,
                                                 double? k,
                                                 double max_atom_movement,
                                                 int? max_iteration,
                                                 double threshold,
                                                 int randomPurturb,
                                                 bool[] atomsMovable,
                                                 IMinimizeLogger logger,
                                                 InfoPack extra,
                                                 bool? doSteepDeescent           // null or true for default
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

            // double k = 0.0001;
            int iter = iterInitial;
            // 0. Initial configuration of atoms
            Vectors coords = GetCoords();
            if(atomsMovable == null)
            {
                atomsMovable = new bool[size];
                for(int i=0; i<size; i++)
                    atomsMovable[i] = true;
            }

            Vectors h = GetVectorsZero();
            Vectors forces = null;
            Dictionary<string,object> cache = new Dictionary<string, object>();
            double energy = GetPotential(frcflds, coords, out forces, cache);
            double forces_NormInf = NormInf(forces, atomsMovable);
            double forces_Norm1   = Norm(1, forces, atomsMovable);
            double forces_Norm2   = Norm(2, forces, atomsMovable);
            Vectors forces0 = forces;
            double  energy0 = energy;
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
                    //if(iter %10 == 0)
                    //{
                    //    System.IO.Directory.CreateDirectory("output");
                    //    string pdbname = string.Format("mini.conju.{0:D5}.pdb", iter);
                    //    pdb.ToFile("output\\"+pdbname, coords.ToArray());
                    //    System.IO.File.AppendAllLines("output\\mini.conju.[animation].pml", new string[] { "load "+pdbname+", 1A6G" });
                    //}
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
                    cache = new Dictionary<string, object>(); // reset cache
                    energy = GetPotential(frcflds, coords, out forces, cache);
                    forces_NormInf = NormInf(forces, atomsMovable);
                    forces_Norm1   = Norm(1, forces, atomsMovable);
                    forces_Norm2   = Norm(2, forces, atomsMovable);

                    // This is already checked by "if(forces_NormInf < threshold) stopIteration = true;"
                    //if(forces_NormInf < threshold)
                    /////////////////////////////////////////////////////
                    {
                        if(iter != 1)
                        {
                            SetCoords((Vector[])coords);
                        }
                        //{
                        //    string pdbname = string.Format("mini.conju.{0:D5}.pdb", iter);
                        //    pdb.ToFile("output\\"+pdbname, coords.ToArray());
                        //    System.IO.File.AppendAllLines("output\\mini.conju.[animation].pml", new string[] { "load "+pdbname+", 1A6G" });
                        //}
                        if(extra != null)
                        {
                            extra.SetValue("energy", energy);
                            extra.SetValue("forces", forces);
                            extra.SetValue("forces norm-1", forces_Norm1);
                            extra.SetValue("forces norm-2", forces_Norm2);
                            extra.SetValue("forces norm-inf", forces_NormInf);
                            extra.SetValue("iter", iter);
                        }
                        return iter;
                    }
                }
                // 4. Move atoms with conjugated gradient
                Vectors coords_prd;
                {
                    if((iter > 0) && (iter % 100 == 0))
                    {
                        cache = new Dictionary<string, object>(); // reset cache
                    }
                    if((randomPurturb > 0) && (iter % randomPurturb == 0))
                    {
                        Vectors dcoords = GetVectorsRandom();
                        dcoords *= max_atom_movement;
                        coords = AddConditional(coords, dcoords, atomsMovable);
                    }
                    if(iter > 1)
                    {
                        HDebug.Assert(forces0 != null);
                        double r = Vectors.VtV(forces, forces).Sum() / Vectors.VtV(forces0, forces0).Sum();
                        h          = forces + r * h;
                        double kk      = k.Value;
                        double hNormInf = NormInf(h, atomsMovable);
                        if(kk*hNormInf > max_atom_movement)
                            // make the maximum movement as atomsMovable
                            kk = max_atom_movement/(hNormInf);
                        //double kk = (k*h.NormsInf().NormInf() < max_atom_movement) ? k : (max_atom_movement/h.NormsInf().NormInf());
                        //double   kk = (k.Value*NormInf(h,atomsMovable) < max_atom_movement)? k.Value : (max_atom_movement/NormInf(h,atomsMovable));
                        coords_prd = AddConditional(coords, kk * h, atomsMovable);
                    }
                    else
                    {
                        // same to the steepest descent for the first iteration
                        h          = forces;
                        double kk      = k.Value;
                        double hNormInf = NormInf(h, atomsMovable);
                        if(kk*hNormInf > max_atom_movement)
                            // make the maximum movement as atomsMovable
                            kk = max_atom_movement/(hNormInf);
                        //double kk = (k*h.NormsInf().NormInf() < max_atom_movement) ? k : (max_atom_movement/h.NormsInf().NormInf());
                        //double   kk = (k.Value*NormInf(h,atomsMovable) < max_atom_movement)? k.Value : (max_atom_movement/NormInf(h, atomsMovable));
                        coords_prd = AddConditional(coords, kk * h, atomsMovable);
                    }
                }
                // 5. Predict energy or forces on atoms
                Vectors forces_prd;
                double energy_prd = GetPotential(frcflds, coords_prd, out forces_prd, cache); iter++;

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
                    h          = forces;
                    double kk      = k.Value;
                    double hNormInf = NormInf(h, atomsMovable);
                    if(kk*hNormInf > max_atom_movement)
                        // make the maximum movement as atomsMovable
                        kk = max_atom_movement/(hNormInf);
                    //double kk = (k*h.NormsInf().NormInf() < max_atom_movement) ? k : (max_atom_movement/h.NormsInf().NormInf());
                    //double   kk = (k.Value*NormInf(h,atomsMovable) < max_atom_movement)? k.Value : (max_atom_movement/NormInf(h, atomsMovable));
                    coords_prd = AddConditional(coords, kk * h, atomsMovable);

                    //if(randomPurturb)
                    //{
                    //    Vectors dcoords = GetForcesRandom();
                    //    dcoords *= (0.1*max_atom_movement);
                    //    coords += dcoords;
                    //}
                }
                energy_prd = GetPotential(frcflds, coords_prd, out forces_prd, cache);
                forces_prd_NormInf = NormInf(forces_prd, atomsMovable);
                forces_prd_Norm1   = Norm(1, forces_prd, atomsMovable);
                forces_prd_Norm2   = Norm(2, forces_prd, atomsMovable);

                energy0 = energy;
                forces0 = forces;
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
