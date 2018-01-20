using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public int Minimize_ConjugateGradient_v0(List<ForceField.IForceField> frcflds, double k, double threshold)
        {
            int t = 0;
            int iter = 0;
            // 0. Initial configuration of atoms
            Dictionary<string,object> cache = new Dictionary<string,object>();
            Vectors coords = GetCoords();
            Vectors h = GetVectorsZero();
            Vectors forces = null;
            double energy = double.PositiveInfinity;
            while(true)
            {
                // 1. Save the position of atoms
                Vectors coords0 = coords;
                // 2. Calculate the potential energy of system and the net forces on atoms
                double  energy0 = energy;
                Vectors forces0 = forces;
                energy = GetPotential(frcflds, coords, out forces, cache); iter++; System.Console.WriteLine("" + iter + "-iter: energy(" + energy + ")");
                // 3. Check if every force reaches to zero,
                //    , and END if yes
                double forces_NormInf = forces.NormsInf().NormInf();
                double forces_Norm1   = forces.Norms(1).Norm(1);
                double forces_Norm2   = forces.Norms(2).Norm(2);
                if(forces_NormInf < threshold)
                {
                    if(forces != null)
                    {
                        HDebug.Assert(t == 0);
                        SetCoords((Vector[])coords);
                    }
                    return iter;
                }
                // 4. Move atoms with conjugated gradient
                /////////////////////////////////////////////////////////////////////////////////
                //  Algorithm                                                                  //
                //    coord[i]_(t) = coord[i]_(t-1) + k * h[i]_(t)                             //
                //    h[i]_(t) = F[i]_(t) + r[i]_(t-1) * h[i]_(t-1)                            //
                //    F[i]_(t) = force with atom i at coord[i]_(t)                             //
                //    r[i]_(t-1) = F[i]_(t) . F[i]_(t) / F[i]_(t-1) . F[i]_(t-1)               //
                //    where coord[i] is the coordinate of atom i,                              //
                //          F.F is inner product, and                                          //
                //          h[i]_0 = 0 (same to the steepest descent)                          //
                /////////////////////////////////////////////////////////////////////////////////
                //    Initial                                                                  //
                //      h = 0                                                                  //
                //      forces0 = forces                                                       //
                //    Iteration                                                                //
                //      a. get forces                                                          //
                //      b. r = forces . forces / forces0 . forces0                             //
                //      c. h = forces + r * h                                                  //
                //      d. coords = coords + k * h                                             //
                /////////////////////////////////////////////////////////////////////////////////
                {
                    if(forces0 != null)
                    {
                        // not the first iteration
                        HDebug.Assert(forces0 != null);
                        double r = Vectors.VtV(forces, forces).Sum() / Vectors.VtV(forces0, forces0).Sum();
                        h      = forces + r * h; // h = forces + r * h
                        coords = coords + k * h; //coords = coords + k * h
                    }
                    else
                    {
                        // same to the steepest descent
                        h      = forces        ; // h = forces + r * h
                        coords = coords + k * h; //coords = coords + k * h
                    }
                }
                // 5. Predict energy or forces on atoms
                Vectors forces_prd;
                double energy_prd = GetPotential(frcflds, coords, out forces_prd, cache); iter++; System.Console.WriteLine("" + iter + "-iter: energy(" + energy_prd + ")");
                // 6. Check if the predicted forces or energy will exceed over the limit
                //    , and goto 1 if no
                double forces_prd_NormInf = forces_prd.NormsInf().NormInf();
                double forces_prd_Norm1   = forces_prd.Norms(1).Norm(1);
                double forces_prd_Norm2   = forces_prd.Norms(2).Norm(2);
                if(energy_prd < energy)// && forces_prd_NormInfinity < forces_NormInfinite)
                    continue;
                // 7. Back to saved configuration
                coords = coords0;
                // 8. Move atoms with simple gradient
                coords = coords + k * (new Vectors(forces)); //coords = coords + k * h
                System.Console.WriteLine("Steepest backup");
                // 9. goto 1
                //double energy_prd_ = GetPotential(frcflds, coords, out forces_prd); iter++; System.Console.WriteLine("" + iter + "-iter: energy(" + energy_prd_ + ")");
                //double forces_prd_NormInf_ = forces_prd.NormsInf().NormInf();
                //double forces_prd_Norm1_   = forces_prd.Norms(1).Norm(1);
                //double forces_prd_Norm2_   = forces_prd.Norms(2).Norm(2);
                //kkkk = true;
            }
        }
    }
}
