using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static bool GetPwEnrgFrcSprNbnd( bool vdW, bool elec
                                            , Universe.Nonbonded nonbonded
                                            , IList<Vector> coords
                                            , double D // = 80 // dielectric constant
                                            , out double Eij
                                            , out double Fij
                                            , out double Kij
                                            )
        {
            Eij = Fij = Kij = double.NaN;

            Universe.Atom atom0 = nonbonded.atoms[0];
            Universe.Atom atom1 = nonbonded.atoms[1];
            int id0 = atom0.ID; if(coords[id0] == null) return false;
            int id1 = atom1.ID; if(coords[id1] == null) return false;

            Vector coord0 = coords[id0];
            Vector coord1 = coords[id1];

            double ri0 = atom0.Rmin2;
            double rj0 = atom1.Rmin2;
            double qi = atom0.Charge;
            double qj = atom1.Charge;
            double ei = atom0.epsilon;
            double ej = atom1.epsilon;

            GetPwEnrgFrcSprNbnd(vdW, elec
                               , coord0, ri0, qi, ei
                               , coord1, rj0, qj, ej
                               , D
                               , out Eij, out Fij, out Kij
                               );
            return true;
        }
        public static bool GetPwEnrgFrcSprNbnd(bool vdW, bool elec
                                            , Universe.Nonbonded14 nonbonded14
                                            , IList<Vector> coords
                                            , double D // = 80 // dielectric constant
                                            , out double Eij
                                            , out double Fij
                                            , out double Kij
                                            )
        {
            Eij = Fij = Kij = double.NaN;

            Universe.Atom atom0 = nonbonded14.atoms[0];
            Universe.Atom atom1 = nonbonded14.atoms[1];
            int id0 = atom0.ID; if(coords[id0] == null) return false;
            int id1 = atom1.ID; if(coords[id1] == null) return false;

            Vector coord0 = coords[id0];
            Vector coord1 = coords[id1];

            double ri0 = atom0.Rmin2_14; if(double.IsNaN(ri0)) ri0 = atom0.Rmin2;
            double rj0 = atom1.Rmin2_14; if(double.IsNaN(rj0)) rj0 = atom1.Rmin2;
            double qi = atom0.Charge;
            double qj = atom1.Charge;
            double ei = atom0.eps_14; if(double.IsNaN(ei)) ei = atom0.epsilon;
            double ej = atom1.eps_14; if(double.IsNaN(ej)) ej = atom1.epsilon;

            GetPwEnrgFrcSprNbnd(vdW, elec
                               , coord0, ri0, qi, ei
                               , coord1, rj0, qj, ej
                               , D
                               , out Eij, out Fij, out Kij
                               );
            return true;
        }
        public static void GetPwEnrgFrcSprNbnd(bool vdW, bool elec
                                            , Vector coordi, double ri0, double qi, double ei
                                            , Vector coordj, double rj0, double qj, double ej
                                            , double D // = 80 // dielectric constant
                                            , out double Eij
                                            , out double Fij
                                            , out double Kij
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

            double rij2 = (coordi - coordj).Dist2;
            double rij  = Math.Sqrt(rij2);
            double r0ij = (ri0 + rj0);
            double qij  = qi * qj;
            double eij  = Math.Sqrt(ei * ej);
            double eps  = D/332;

            Eij = 0;
            Fij = 0;
            Kij = 0;
            if(vdW)
            {
                double Evdw  = eij *             Math.Pow(r0ij/rij, 12)        - eij * (      2) * Math.Pow(r0ij/rij, 6)       ; Eij += Evdw;
                double Fvdw  = eij * (    -12) * Math.Pow(r0ij/rij, 12) / rij  - eij * (   -6*2) * Math.Pow(r0ij/rij, 6) / rij ; Fij += Fvdw;
                double Kvdw  = eij * (-12*-13) * Math.Pow(r0ij/rij, 12) / rij2 - eij * (-6*-7*2) * Math.Pow(r0ij/rij, 6) / rij2; Kij += Kvdw;
            }
            if(elec)
            {
                double Eelec =           (qij/eps) / (rij     ); Eij += Eelec;
                double Felec = (   -1) * (qij/eps) / (rij*rij ); Fij += Felec;
                double Kelec = (-2*-1) * (qij/eps) / (rij*rij2); Kij += Kelec;
                HDebug.AssertIf(qij < 0, Felec > 0); /// attractive, positive pw-force
                HDebug.AssertIf(qij > 0, Felec < 0); /// repulsive , negative pw-force
            }

            if(HDebug.IsDebuggerAttached)
            {
                var KijFij = Hess.HessSpr.GetKijFijNbnd(vdW, elec
                                               ,coordi, ri0, qi, ei
                                               ,coordj, rj0, qj, ej
                                               ,D
                                               );
                HDebug.AssertTolerance(0.00000001, Kij - KijFij.Kij);
                HDebug.AssertTolerance(0.00000001, Fij - KijFij.Fij);
            }
        }
    }
}
