/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public int Minimize_ConjugateGradient_v2(List<ForceField.IForceField> frcflds, double atom_max_move, double threshold, System.IO.TextWriter logwriter)
        {
            // atom_max_move = 0.03;
            int iter = 0;
            // 0. Initial configuration of atoms
            Vectors coords = GetCoords();
            Vectors h = GetForcesZero();
            Vectors forces = null;
            Dictionary<string,object> cache = new Dictionary<string, object>();
            double energy = GetPotential(frcflds, coords, out forces, cache); iter++; Minimize_ConjugateGradient_WriteLog(logwriter, iter, energy, forces, null);
            double forces_NormInf = forces.NormsInf().NormInf();
            double forces_Norm1   = forces.Norms(1).Norm(1);
            double forces_Norm2   = forces.Norms(2).Norm(2);
            Vectors forces0 = forces;
            double  energy0 = energy;
            List<double> log_steepest = new List<double>();
            while(true)
            {
                if((iter+1) %10 == 0)
                {
                    System.IO.Directory.CreateDirectory("output");
                    string pdbname = string.Format("mini.conju.{0:D5}.pdb", iter);
                    pdb.ToFile("output\\"+pdbname, coords.ToArray());
                    System.IO.File.AppendAllLines("output\\mini.conju.[animation].pml", new string[] { "load "+pdbname+", 1A6G" });
                }

                // 1. Save the position of atoms
                // 2. Calculate the potential energy of system and the net forces on atoms
                // 3. Check if every force reaches to zero,
                //    , and END if yes
                if(forces_NormInf < threshold)
                {
                    if(iter != 1)
                    {
                        SetCoords(coords);
                    }
                    return iter;
                }
                // 4. Move atoms with conjugated gradient
                Vectors coords_prd;
                double timestep;
                {
                    if(iter > 1)
                    {
                        Debug.Assert(forces0 != null);
                        double r = Vectors.VtV(forces, forces).Sum() / Vectors.VtV(forces0, forces0).Sum();
                        h          = forces + r * h;
                    }
                    else
                    {
                        // same to the steepest descent for the first iteration
                        h          = forces;
                    }
                    double h_NormInf = h.NormsInf().NormInf();
                    timestep = atom_max_move / h_NormInf;
                    coords_prd = coords + timestep * h;
                }
                // 5. Predict energy or forces on atoms
                Vectors forces_prd;
                double energy_prd = GetPotential(frcflds, coords_prd, out forces_prd, cache); iter++; Minimize_ConjugateGradient_WriteLog(logwriter, iter, energy_prd, forces_prd, null);
                double forces_prd_NormInf = forces_prd.NormsInf().NormInf();
                double forces_prd_Norm1   = forces_prd.Norms(1).Norm(1);
                double forces_prd_Norm2   = forces_prd.Norms(2).Norm(2);
                // 6. Check if the predicted forces or energy will exceed over the limit
                //    , and goto 1 if no
                if(log_steepest.Count >= 20)
                    log_steepest.RemoveAt(0);
                if(energy_prd < energy)// && forces_prd_NormInfinity < forces_NormInfinite)
                {
                    energy0 = energy;
                    forces0 = forces;
                    coords = coords_prd;
                    forces = forces_prd;
                    energy = energy_prd;
                    forces_NormInf = forces_prd_NormInf;
                    forces_Norm1   = forces_prd_Norm1;
                    forces_Norm2   = forces_prd_Norm2;
                    log_steepest.Add(0);

                    if(log_steepest.Count == 20 && log_steepest.Average() <0.06)
                    {
                        log_steepest.Clear();
                        atom_max_move *= 1.1;
                        System.Console.WriteLine("Increase atom maximum movement as "+atom_max_move);
                    }
                    continue;
                }
                // 7. Back to saved configuration
                // 8. Move atoms with simple gradient
                {
                    // same to the steepest descent
                    h          = forces;
                    double h_NormInf = h.NormsInf().NormInf();
                    timestep = atom_max_move / h_NormInf;
                    coords_prd = coords + timestep * h;
                    log_steepest.Add(1);

                    if(log_steepest.Count >= 10 && log_steepest.Average() > 0.4)
                    {
                        atom_max_move *= 0.8;
                        log_steepest.Clear();
                        System.Console.WriteLine("Reduce atom maximum movement as "+atom_max_move);
                    }
                }
                energy_prd = GetPotential(frcflds, coords_prd, out forces_prd, cache); iter++; Minimize_ConjugateGradient_WriteLog(logwriter, iter, energy_prd, forces_prd, null, "steepest");
                forces_prd_NormInf = forces_prd.NormsInf().NormInf();
                forces_prd_Norm1   = forces_prd.Norms(1).Norm(1);
                forces_prd_Norm2   = forces_prd.Norms(2).Norm(2);

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
*/