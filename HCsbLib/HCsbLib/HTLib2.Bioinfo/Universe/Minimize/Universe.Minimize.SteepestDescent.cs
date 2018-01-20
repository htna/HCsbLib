using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        //public int Minimize_SteepestDescent(List<ForceField.IForceField> frcflds)
        //{
        //    double k = 0.0005;
        //    double threshold = 0.001;
        //    System.IO.TextWriter logwriter = null;
        //    return Minimize_SteepestDescent(frcflds, k, threshold, logwriter);
        //}
        //public int Minimize_SteepestDescent(List<ForceField.IForceField> frcflds, double k, double threshold)
        //{
        //    System.IO.TextWriter logwriter = null;
        //    return Minimize_SteepestDescent(frcflds, k, threshold, logwriter);
        //}
        public void Minimize_Minimize_SteepestDescent_WriteLog(System.IO.TextWriter logwriter, int iter, double energy, Vectors forces, string message="")
        {
            if(logwriter == null)
                return;
            double forces_NormInf = forces.NormsInf().NormInf();
            double forces_Norm1   = forces.Norms(1).Norm(1);
            double forces_Norm2   = forces.Norms(2).Norm(2);
            logwriter.Write("" + iter + "-iter: energy(" + energy + ")");
            logwriter.Write(                 ", force-norm-1(" + forces_Norm1 + ")");
            logwriter.Write(                 ", force-norm-2(" + forces_Norm2 + ")");
            logwriter.Write(                 ", force-norm-inf(" + forces_NormInf + ")");
            if(message.Length != 0)
                logwriter.Write(" - " + message);
            logwriter.WriteLine();
        }
        public long? Minimize_SteepestDescent(List<ForceField.IForceField> frcflds
                                             , double thresholdForMaxNormInf // 0.001
                                             , long maxIter = long.MaxValue
                                             , double maxMovePerIter = 0.01
                                             , double? k = 1
                                             , bool[] atomsMovable = null // true  for movable atoms,
                                                                          // false for non-movable atoms,
                                                                          // null  for all atoms movable
                                             , IMinimizeLogger logger = null
                                             )
        {
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

            long iter = 0;
            // 0. Initial configuration of atoms
            Vectors coords = GetCoords();
            if(atomsMovable == null)
            {
                atomsMovable = new bool[size];
                for(int i=0; i<size; i++)
                    atomsMovable[i] = true;
            }

            Vector masses = GetMasses(3);
                   masses = masses / masses.ToArray().Min();
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
                }
                // 1. Save the position of atoms
                // 2. Calculate the potential energy of system and the net forces on atoms
                // 3. Check if every force reaches to zero,
                //    , and END if yes
                bool stopIteration = false;
                if(forces_NormInf < thresholdForMaxNormInf) stopIteration = true;
                if(iter>=maxIter) stopIteration = true;
                if(stopIteration)
                {
                    // double check
                    cache = new Dictionary<string, object>(); // reset cache
                    energy = GetPotential(frcflds, coords, out forces, cache);
                    forces_NormInf = NormInf(forces, atomsMovable);
                    forces_Norm1   = Norm(1, forces, atomsMovable);
                    forces_Norm2   = Norm(2, forces, atomsMovable);

                    if(forces_NormInf < thresholdForMaxNormInf)
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
                        if(lextra != null)
                        {
                            lextra.Add("energy", energy);
                            lextra.Add("forces", forces);
                            lextra.Add("forces norm-1", forces_Norm1);
                            lextra.Add("forces norm-2", forces_Norm2);
                            lextra.Add("forces norm-inf", forces_NormInf);
                            lextra.Add("iter", iter);
                        }
                        return iter;
                    }
                }
                // 4. Move atoms with conjugated gradient
                Vectors coords_prd;
                {
                    if(iter % 100 == 0)
                    {
                        cache = new Dictionary<string, object>(); // reset cache
                    }
                    #region code for purturbation (commented out)
                    //if((randomPurturb > 0) && (iter % randomPurturb == 0))
                    //{
                    //    Vectors dcoords = GetForcesRandom();
                    //    dcoords *= max_atom_movement;
                    //    coords = AddConditional(coords, dcoords, atomsMovable);
                    //}
                    #endregion
                    {
                        // same to the steepest descent for the first iteration
                        h = forces;
                        double kk = k.Value;
                        double hNormInf = NormInf(h, atomsMovable);
                        if(kk*hNormInf > maxMovePerIter)
                        {
                            // make the maximum movement as atomsMovable
                            //kk = maxMovePerIter/(hNormInf*kk);
                            kk = maxMovePerIter/hNormInf;
                        }
                        //for(int i=0; i<h.Length; i++)
                        //    h[i] = h[i] / masses[i];
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
                //if(energy_prd < energy)// && forces_prd_NormInfinity < forces_NormInfinite)
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
                // 9. goto 1
            }
        }
    }
}
