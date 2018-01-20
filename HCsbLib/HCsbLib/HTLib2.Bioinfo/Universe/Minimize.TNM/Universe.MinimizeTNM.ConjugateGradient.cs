using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        public partial class MinimizeTNMImpl
        {
            public int ConjugateGradient(List<ForceField.IForceField> frcflds
                                                    , double threshold
                                                    , double? k                = null
                                                    , double max_atom_movement = 0.01
                                                    , int? max_iteration       = null
                                                    , IMinimizeLogger logger   = null // logger = new MinimizeLogger_PrintEnergyForceMag(logpath);

                                                    , HPack<double> optOutEnergy        = null  // optional output for final energy
                                                    , List<Vector> optOutForces        = null  // optional output for final force vectors
                                                    , HPack<double> optOutForcesNorm1   = null  // optional output for norm of final force vectors
                                                    , HPack<double> optOutForcesNorm2   = null  // optional output for norm of final force vectors
                                                    , HPack<double> optOutForcesNormInf = null  // optional output for norm of final force vectors
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

                Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = univ.BuildFlexibilityGraph();
                List<Universe.RotableInfo> univ_rotinfos = univ.GetRotableInfo(univ_flexgraph);
                // double k = 0.0001;
                int iter = 0;
                // 0. Initial configuration of atoms
                Vector[] coords0 = univ.GetCoords();
                Vectors coords = univ.GetCoords();
                bool[] atomsMovable = null;
                if(atomsMovable == null)
                {
                    atomsMovable = new bool[size];
                    for(int i=0; i<size; i++)
                        atomsMovable[i] = true;
                }

                Vectors h = univ.GetVectorsZero();
                Vectors forces = univ.GetVectorsZero();
                Dictionary<string,object> cache;
                double energy;
                cache = new Dictionary<string, object>();
                GetPotentialAndTorForce(frcflds, coords
                                       , out energy, out forces._vecs
                                       , cache);

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
                        {   // logger.logTrajectory(univ, iter, coords);
                            Vector[] coordsx = coords._vecs.HClone<Vector>();
                            Trans3 trans = ICP3.OptimalTransform(coordsx, coords0);
                            trans.DoTransform(coordsx);
                            logger.logTrajectory(univ, iter, coordsx);
                        }
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
                        GetPotentialAndTorForce(frcflds, coords
                                               , out energy, out forces._vecs
                                               , cache);
                        forces_NormInf = NormInf(forces, atomsMovable);
                        forces_Norm1   = Norm(1, forces, atomsMovable);
                        forces_Norm2   = Norm(2, forces, atomsMovable);

                        if(forces_NormInf < threshold)
                        {
                            if(iter != 1)
                            {
                                univ.SetCoords((Vector[])coords);
                            }
                            {
                                if(optOutEnergy        != null) optOutEnergy.value = energy;
                                if(optOutForces        != null) { optOutForces.Clear(); optOutForces.AddRange(forces.ToArray()); }
                                if(optOutForcesNorm1   != null) optOutForcesNorm1.value = forces_Norm1;
                                if(optOutForcesNorm2   != null) optOutForcesNorm2.value = forces_Norm2;
                                if(optOutForcesNormInf != null) optOutForcesNormInf.value = forces_NormInf;
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
                        if(iter >= 1)
                        {
                            HDebug.Assert(forces0 != null);
                            double r = Vectors.VtV(forces, forces).Sum() / Vectors.VtV(forces0, forces0).Sum();
                            h          = forces + r * h;
                            double kk      = k.Value;
                            double hNormInf = NormInf(h, atomsMovable);
                            if(kk*hNormInf > max_atom_movement)
                                // make the maximum movement as atomsMovable
                                kk = max_atom_movement/(hNormInf);

                            Vector dangles = Paper.TNM.GetRotAngles(univ, coords, kk * h, 1);
                            //dangles *= -1;
                            coords_prd = Paper.TNM.RotateTorsionals(coords, dangles, univ_rotinfos);
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

                            Vector dangles = Paper.TNM.GetRotAngles(univ, coords, kk * h, 1);
                            //dangles *= -1;
                            coords_prd = Paper.TNM.RotateTorsionals(coords, dangles, univ_rotinfos);
                        }
                    }
                    // 5. Predict energy or forces on atoms
                    iter++;
                    double energy_prd;
                    Vectors forces_prd = univ.GetVectorsZero();
                    GetPotentialAndTorForce(frcflds, coords_prd
                                           , out energy_prd, out forces_prd._vecs
                                           , cache);

    //                double energy_prd = univ.GetPotential(frcflds, coords_prd, ref forces_prd._vecs, ref hessian_prd, cache);
    //                Vector[] dcoord_prd = univ.GetVectorsZero();
    //                double[] dangles_prd = TNM.GetRotAngles(univ, coords_prd, forces_prd, 1, dcoordsRotated: dcoord_prd);
    //                         dangles_prd = TNM.GetRotAngles(univ, coords_prd, hessian_prd, forces_prd, forceProjectedByTorsional: dcoord_prd);
                    //Vectors coords_prd2 = coords_prd.Clone();
                    //TNM.RotateTorsionals(coords_prd2, dangles_prd, univ_rotinfos);

                    double forces_prd_NormInf = NormInf(forces_prd, atomsMovable); // NormInf(forces_prd, atomsMovable);
                    double forces_prd_Norm1   = Norm(1, forces_prd, atomsMovable); // Norm(1, forces_prd, atomsMovable);
                    double forces_prd_Norm2   = Norm(2, forces_prd, atomsMovable); // Norm(2, forces_prd, atomsMovable);
                    // 6. Check if the predicted forces or energy will exceed over the limit
                    //    , and goto 1 if no
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
                        Vector dangles = Paper.TNM.GetRotAngles(univ, coords, kk * h, 1);
                        //dangles *= -1;
                        coords_prd = Paper.TNM.RotateTorsionals(coords, dangles, univ_rotinfos);
                    }
                    forces_prd = univ.GetVectorsZero();
                    //energy_prd = univ.GetPotential(frcflds, coords_prd, ref forces_prd._vecs, ref hessian_prd, cache);
                    GetPotentialAndTorForce(frcflds, coords_prd
                           , out energy_prd, out forces_prd._vecs
                           , cache);
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
}
