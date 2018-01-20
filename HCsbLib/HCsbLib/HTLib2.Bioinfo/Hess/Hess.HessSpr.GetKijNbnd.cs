using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public partial class HessSpr
        {
            public class HKijFij { public double Kij, Fij; };
            public static HKijFij GetKijFijNbnd(bool vdW, bool elec
                                               ,Vector coordi, double ri0, double qi, double ei
                                               ,Vector coordj, double rj0, double qj, double ej
                                               ,double D // dielectric constant: 80 for general (solvent), 1 for Tinker (NMA)
                                               )
            {
                /// http://www.ks.uiuc.edu/Training/Tutorials/science/forcefield-tutorial/forcefield-html/node5.html
                /// http://www.charmmtutorial.org/index.php/The_Energy_Function
                /// 
                /// NONBONDED
                /// 
                /// charmm : V_nbnd   = epsilon_ij (              (r0ij / rij)^12  -       2  * (r0ij / rij)^6 )   +           (qi*qj)/(epsilon*rij)
                ///                   = epsilon_ij (             r0ij^12 * rij^-12 -       2  * r0ij^6 * rij^-6)   +           (qi*qj/epsilon) * rij^-1
                ///          frc_nbnd = epsilon_ij (       -12 * r0ij^12 * rij^-13 -   (-6*2) * r0ij^6 * rij^-7)   +      (-1)*(qi*qj/epsilon) * rij^-2
                ///          spr_nbnd = epsilon_ij ( (-12*-13) * r0ij^12 * rij^-14 -(-6*-7*2) * r0ij^6 * rij^-8)   +   (-1*-2)*(qi*qj/epsilon) * rij^-3
                ///                   = epsilon_ij (      156  * r0ij^12 * rij^-14 +       84 * r0ij^6 * rij^-8)   +         2*(qi*qj/epsilon) * rij^-3
                /// 
                /// V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                /// 
                /// epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                /// Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                /// 
                /// atom  ignored    epsilon      Rmin/2   ignored   eps,1-4       Rmin/2,1-4
                /// 
                /// V(electrostatic) : V(i,j) = (qi*qj)/(epsilon*rij)
                ///                           = 332*qi*qj/r_ij/D
                ///                    Where D is dielectric constant, for proteins. D is normally 80. (see equation (13) in the following pdf file). 
                ///                    http://pharmacy.ucsd.edu/labs/gilson/ce_www1a.pdf
                ///                    epsilon = D/332
                HDebug.AssertIf(elec, (double.IsNaN(D) == false) && (double.IsInfinity(D) == false));

                // electric: conversion from electron**2/Angstrom to Kcal/mol
                double electric;
                electric = 332.063709;  // tinker pmpb.c line 570
                electric = 332.05382;   // tinker pmpb.c line 716
                electric = 332.063709;  // tinker pmpb.c line 851
                // units.f line 53      : c     coulomb     conversion from electron**2/Ang to kcal/mole
                // units.f line 82      :       parameter (coulomb=332.063714d0)
                // initprm.f line 272   :       electric = coulomb
                electric = 332.063714;
                // chgpot.f line 16     : c     dielec     dielectric constant for electrostatic interactions
                // initprm.f line 273   :       dielec = 1.0d0
                double dielec = 1.0;

                double rij  = (coordi - coordj).Dist;
                double r0ij = (ri0 + rj0);
                double qij  = qi * qj;
                double eij  = Math.Sqrt(ei * ej);
                double eps  = D/332;

                double Kvdw  = eij * (-12*-13) * Math.Pow(r0ij/rij, 12) / (rij*rij)
                             - eij * (-6*-7*2) * Math.Pow(r0ij/rij, 6) / (rij*rij);
                double Kelec = 2 * (qij/eps) / (rij*rij*rij);

                double Fvdw  = eij * (    -12) * Math.Pow(r0ij/rij, 12) / rij
                             - eij * (   -6*2) * Math.Pow(r0ij/rij,  6) / rij;
                double Felec =       (     -1) * (qij/eps) / (rij*rij);
                double Evdw  = eij * (1      ) * Math.Pow(r0ij/rij, 12)
                             - eij * (      2) * Math.Pow(r0ij/rij,  6);
                double Eelec =       (1      ) * (qij/eps) / (rij);

                double ee = D;
                double Kvdw_stem  = eij; //Math.Sqrt(atom0.epsilon * atom1.epsilon)
                double Kelec_stem = (332.0/60.0 * qij / ee / r0ij);

                //Debug.Assert(Kvdw + Kelec >= 0);
                double Kij = 0;
                double Fij = 0;
                if(vdW ) { Kij += Kvdw ; Fij += Fvdw ; }
                if(elec) { Kij += Kelec; Fij += Felec; }

                //double pweng = Evdw + Eelec;
                //double pwfrc = Fvdw + Felec;
                //double pwspr = Kij;

                return new HKijFij { Kij=Kij, Fij=Fij };
            }
        }
    }
}
