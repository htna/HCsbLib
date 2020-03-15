using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class PwVdw : INonbonded, IHessBuilder4PwIntrAct
        {
            public virtual string[] FrcFldType { get { return new string[] { "Nonbonded", "LennardJones" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            // ! Wildcards used to minimize memory requirements
            // NONBONDED  NBXMOD 5  ATOM CDIEL FSHIFT VATOM VDISTANCE VFSWITCH -
            //      CUTNB 14.0  CTOFNB 12.0  CTONNB 10.0  EPS 1.0  E14FAC 1.0  WMIN 1.5
            // !
            // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
            // !
            // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
            // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
            // !
            // !atom  ignored    epsilon      Rmin/2   ignored   eps,1-4       Rmin/2,1-4
            // !
            // HT       0.0       -0.0460    0.2245 ! TIP3P
            // HN1      0.0       -0.0460    0.2245 
            // CN7      0.0       -0.02      2.275  0.0   -0.01 1.90 !equivalent to protein CT1
            // CN7B     0.0       -0.02      2.275  0.0   -0.01 1.90 !equivalent to protein CT1
            // ...
            /////////////////////////////////////////////////////////////
            public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double radi = atom1.Rmin2_14; radi = (double.IsNaN(radi)==false) ? radi : atom1.Rmin2;
                double radj = atom2.Rmin2_14; radj = (double.IsNaN(radj)==false) ? radj : atom2.Rmin2;
                double epsi = atom1.eps_14; epsi = (double.IsNaN(epsi)==false) ? epsi : atom1.epsilon;
                double epsj = atom2.eps_14; epsj = (double.IsNaN(epsj)==false) ? epsj : atom2.epsilon;
                Compute(nonbonded, coords, ref energy, ref forces, ref hessian, radi, radj, epsi, epsj, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double radi = atom1.Rmin2;
                double radj = atom2.Rmin2;
                double epsi = atom1.epsilon;
                double epsj = atom2.epsilon;
                Compute(nonbonded, coords, ref energy, ref forces, ref hessian, radi, radj, epsi, epsj, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.AtomPack nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian
                                       ,double radi ,double radj ,double epsi ,double epsj, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];

                Vector pos0 = coords[0];
                Vector pos1 = coords[1];

                double rmin = (radi + radj);
                double epsij  = Math.Sqrt(epsi * epsj);

                double lenergy, forceij, springij;
                Compute(coords, out lenergy, out forceij, out springij, epsij, rmin);
                double abs_forceij = Math.Abs(forceij);
                if(pwfrc != null) pwfrc[0, 1] = pwfrc[1, 0] = forceij;
                if(pwspr != null) pwspr[0, 1] = pwspr[1, 0] = springij;


                ///////////////////////////////////////////////////////////////////////////////
				// energy
                energy += lenergy;
                ///////////////////////////////////////////////////////////////////////////////
				// force
                if(forces != null)
                {
                    Vector frc0, frc1;
                    GetForceVector(pos0, pos1, forceij, out frc0, out frc1);
                    forces[0] += frc0;
                    forces[1] += frc1;
                }
                ///////////////////////////////////////////////////////////////////////////////
                // hessian
                if(hessian != null)
                {
                    hessian[0, 1] += GetHessianBlock(coords[0], coords[1], springij, forceij);
                    hessian[1, 0] += GetHessianBlock(coords[1], coords[0], springij, forceij);
                }
            }
            public static void Compute(Vector[] coords, out double energy, out double forceij, out double springij, double epsij, double rmin)
            {
                /// !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                /// !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                /// !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                ///
                /// V(r) =           epsij * r0^12 * rij^-12         -        2 * epsij * r0^6 * rij^-6
                ///      =           epsij * (r0  /  rij)^12         -        2 * epsij * (r0  / rij)^6
                /// F(r) =     -12 * epsij * r0^12 * rij^-13         -     -6*2 * epsij * r0^6 * rij^-7
                ///      =     -12 * epsij * (r0  /  rij)^12 / rij   -     -6*2 * epsij * (r0  / rij)^6 / rij
                /// K(r) = -13*-12 * epsij * r0^12 * rij^-14         -  -7*-6*2 * epsij * r0^6 * rij^-8
                ///      = -13*-12 * epsij * (r0  /  rij)^12 / rij^2 -  -7*-6*2 * epsij * (r0  / rij)^6 / rij^2
                double rij  = (coords[1] - coords[0]).Dist;
                double rij2 = rij*rij;
                double rmin_rij    = rmin / rij;
                double rmin_rij_2  = rmin_rij   * rmin_rij;
                double rmin_rij_6  = rmin_rij_2 * rmin_rij_2 * rmin_rij_2;
                double rmin_rij_12 = rmin_rij_6 * rmin_rij_6;
                energy   =             epsij * rmin_rij_12         -         2  * epsij * rmin_rij_6;
                forceij  =     (-12) * epsij * rmin_rij_12 / rij   -     (-6*2) * epsij * rmin_rij_6 / rij;
                springij = (-13*-12) * epsij * rmin_rij_12 / rij2  -  (-7*-6*2) * epsij * rmin_rij_6 / rij2;
                HDebug.AssertIf(forceij>0, rmin<rij); // positive force => attractive
                HDebug.AssertIf(forceij<0, rij<rmin); // negative force => repulsive
            }
            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out ValueTuple<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                int idx1 = 0; // nonbonded.atoms[0].ID;
                int idx2 = 1; // nonbonded.atoms[1].ID;
                Universe.Atom atom1 = info.atoms[0];
                Universe.Atom atom2 = info.atoms[1];

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];
                double radi = double.NaN;
                double radj = double.NaN;
                double epsi = double.NaN;
                double epsj = double.NaN;

                if(typeof(Universe.Nonbonded14).IsInstanceOfType(info))
                {
                    radi = atom1.Rmin2_14; radi = (double.IsNaN(radi)==false) ? radi : atom1.Rmin2;
                    radj = atom2.Rmin2_14; radj = (double.IsNaN(radj)==false) ? radj : atom2.Rmin2;
                    epsi = atom1.eps_14; epsi = (double.IsNaN(epsi)==false) ? epsi : atom1.epsilon;
                    epsj = atom2.eps_14; epsj = (double.IsNaN(epsj)==false) ? epsj : atom2.epsilon;
                }
                if(typeof(Universe.Nonbonded).IsInstanceOfType(info))
                {
                    radi = atom1.Rmin2;
                    radj = atom2.Rmin2;
                    epsi = atom1.epsilon;
                    epsj = atom2.epsilon;
                }
                HDebug.Assert(double.IsNaN(radi) == false, double.IsNaN(radj) == false, double.IsNaN(epsi) == false, double.IsNaN(epsj) == false);

                // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                //
                // V(r) =           epsij * r0^12 * rij^-12  -        2 * epsij * r0^6 * rij^-6
                // F(r) =     -12 * epsij * r0^12 * rij^-13  -     -6*2 * epsij * r0^6 * rij^-7
                // K(r) = -13*-12 * epsij * r0^12 * rij^-14  -  -7*-6*2 * epsij * r0^6 * rij^-8
                double r   = (radi + radj);
                double r6  = Math.Pow(r, 6);
                double r12 = Math.Pow(r, 12);
                double rij2  = (dx*dx + dy*dy + dz*dz);
                double rij   = Math.Sqrt(rij2);
                double rij7  = Math.Pow(rij2, 3)*rij;
                double rij8  = Math.Pow(rij2, 4);
                double rij13 = Math.Pow(rij2, 6)*rij;
                double rij14 = Math.Pow(rij2, 7);
                double epsij = epsi*epsj;
                double fij = (    -12) * epsij * r12 / rij13  -  (   -6*2) * epsij * r6 / rij7;
                double kij = (-13*-12) * epsij * r12 / rij14  -  (-7*-6*2) * epsij * r6 / rij8;

                pwidxs = new ValueTuple<int, int>[1];
                pwidxs[0] = new ValueTuple<int, int>(0, 1);
                pwhessinfos = new PwIntrActInfo[1];
                pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
        }
    }
}
