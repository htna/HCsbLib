using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // int i;                                                                                                 //
        // MinParameters minparams;                                                                               //
        // int nsteps = atoi(argv[1]);                                                                            //
        // minparams.pdbname = argv[2];                                                                           //
        // minparams.psfname = argv[3];                                                                           //
        // minparams.prmname = argv[4];                                                                           //
        // minparams.switchdist = SWITCHDIST;                                                                     //
        // minparams.cutoff     = CUTOFF;                                                                         //
        // minparams.pairlistdist = PAIRLISTDIST;                                                                 //
        //                                                                                                        //
        // PDB pdb(minparams.pdbname);                                                                            //
        //                                                                                                        //
        // Parameters params(minparams.prmname);                                                                  //
        //                                                                                                        //
        // Molecule mol(&params, minparams.psfname);                                                              //
        //                                                                                                        //
        // const int natoms = pdb.num_atoms();                                                                    //
        // Vector *pos = new Vector[natoms];                                                                      //
        // Vector *vel = new Vector[natoms];                                                                      //
        // Vector *f = new Vector[natoms];                                                                        //
        // double *imass = new double[natoms];                                                                    //
        // memset((void *)vel, 0, natoms*sizeof(Vector));                                                         //
        // for (i=0; i<natoms; i++)                                                                               //
        //     imass[i] = 1.0/mol.atommass(i);                                                                    //
        // pdb.get_all_positions(pos);                                                                            //
        //                                                                                                        //
        // ComputeBonded bonded(&mol, &params);                                                                   //
        //                                                                                                        //
        // ComputeNonbonded nonbonded(&mol, &params, &minparams);                                                 //
        //                                                                                                        //
        // double Ebond, Eangle, Edihedral, Eimproper, Evdw, Eelec;                                               //
        //                                                                                                        //
        // Ebond = Eangle = Edihedral = Eimproper = Evdw = Eelec = 0;                                             //
        // //                                                                                                     //
        // // Begin velocity verlet integration                                                                   //
        // //                                                                                                     //
        // const double dt = 1.0/TIMEFACTOR;                                                                      //
        // double t = 0.0;                                                                                        //
        // double Ekin, Etot;                                                                                     //
        // Ekin = 0.0;                                                                                            //
        // // Compute forces at time 0                                                                            //
        // memset((void *)f, 0, natoms*sizeof(Vector));                                                           //
        // bonded.compute(pos, f, Ebond, Eangle, Edihedral, Eimproper);                                           //
        // nonbonded.compute(&mol, pos, f, Evdw, Eelec);                                                          //
        // Etot = Ebond + Eangle + Edihedral + Eimproper + Evdw + Eelec + Ekin;                                   //
        // std::cout << "t        bond    angle   dihedral   improper   vdw         elec       kinetic  total"    //
        //           << std::endl;                                                                                //
        // std::cout << t << "     " << Ebond << "    " << Eangle << "  " << Edihedral << "    "                  //
        // << Eimproper << "    " << Evdw << "    " << Eelec << "    " << Ekin                                    //
        // << "    " <<Etot<<std::endl;                                                                           //
        // double start = time_of_day();                                                                          //
        // for (int i=0; i<nsteps; i++)                                                                           //
        // {                                                                                                      //
        //     int j;                                                                                             //
        //     for (j=0; j<natoms; j++)                                                                           //
        //     {                                                                                                  //
        //         pos[j] += dt*vel[j] + 0.5*dt*dt*f[j]*imass[j];                                                 //
        //         vel[j] += 0.5*dt*f[j]*imass[j];                                                                //
        //     }                                                                                                  //
        //                                                                                                        //
        //     // Compute forces at time t+dt                                                                     //
        //     memset((void *)f, 0, natoms*sizeof(Vector));                                                       //
        //     bonded.compute(pos, f, Ebond, Eangle, Edihedral, Eimproper);                                       //
        //     nonbonded.compute(&mol, pos, f, Evdw, Eelec);                                                      //
        //                                                                                                        //
        //     Ekin = 0;                                                                                          //
        //     for (j=0; j<natoms; j++)                                                                           //
        //     {                                                                                                  //
        //         vel[j] += 0.5*dt*f[j]*imass[j];                                                                //
        //         Ekin += vel[j]*vel[j]*mol.atommass(j);                                                         //
        //     }                                                                                                  //
        //                                                                                                        //
        //     Ekin *= 0.5;                                                                                       //
        //     Etot = Ebond + Eangle + Edihedral + Eimproper + Evdw + Eelec + Ekin;                               //
        //     t += dt*TIMEFACTOR;                                                                                //
        //                                                                                                        //
        //     if (!(i%100))                                                                                      //
        //         std::cout << t << "     " << Ebond << "    " << Eangle << "  " << Edihedral << "    "          //
        //                   << Eimproper << "    " << Evdw << "    " << Eelec << "    " << Ekin << "    "        //
        //                   << Etot << std::endl;                                                                //
        // }                                                                                                      //
        // double stop = time_of_day();                                                                           //
        // std::cout << t << "     " << Ebond << "    " << Eangle << "  " << Edihedral << "    "                  //
        //           << Eimproper << "    " << Evdw << "    " << Eelec << "    " << Ekin << "    "                //
        //           << Etot << std::endl;                                                                        //
        // std::cout << "time per step = " << (stop-start)/nsteps << std::endl;                                   //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        const double TIMEFACTOR = 48.88821;
        public void Minimize(List<ForceField.IForceField> frcflds, double dt, int steps)
        {
            //double dt = 1.0/TIMEFACTOR
            Vector[] coords = GetCoords();
            Vector[] velocities = GetVectorsZero();
            Vector[] forces = GetVectorsZero();
            Dictionary<string, object> cache = new Dictionary<string,object>();
            double[] masses = new double[size];
            foreach(Atom atom in atoms)
                masses[atom.ID] = atom.Mass;

            double energy_kinetic = 0;
            double energy_potential = GetPotential(frcflds, coords, out forces, cache);

            for(int iter=0; iter<steps; iter++)
            {
                foreach(Atom atom in atoms)
                {
                    int id = atom.ID;
                    coords[id] = coords[id] + (dt * velocities[id]) + (0.5 * dt * dt * forces[id] * masses[id]);
                    //energy_kinetic += Vector.VtV(velocities[id],velocities[id]) * 
                    velocities[id] += 0.5 * dt * forces[id] * masses[id];
                }

                // Compute forces at time t+dt
                energy_potential = GetPotential(frcflds, coords, out forces, cache);

                energy_kinetic = 0;
                foreach(Atom atom in atoms)
                {
                    int id = atom.ID;
                    velocities[id] += 0.5 * dt * forces[id] * masses[id];
                    energy_kinetic += LinAlg.VtV(velocities[id], velocities[id]) * masses[id];
                }
                energy_kinetic *= 0.5;

                double energy_total = energy_potential + energy_kinetic;
                // t += dt * TIMEFACTOR
            }
        }
	}
}
