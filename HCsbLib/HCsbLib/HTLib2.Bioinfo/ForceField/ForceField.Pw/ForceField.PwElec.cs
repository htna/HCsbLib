using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class PwElec : INonbonded, IHessBuilder4PwIntrAct
        {
            public virtual string[] FrcFldType { get { return new string[] { "Nonbonded", "Electrostatic" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public virtual void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                Compute(atom1, atom2, coords, ref energy, ref forces, ref hessian, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Nonbonded nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                Compute(atom1, atom2, coords, ref energy, ref forces, ref hessian, pwfrc, pwspr);
            }
            public virtual void Compute(Universe.Atom atom1, Universe.Atom atom2, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                Vector pos0 = coords[0];
                Vector pos1 = coords[1];
                double pchi = atom1.Charge;
                double pchj = atom2.Charge;
                double ee  = 80;

                double pchij = pchi * pchj;

                double lenergy, forceij, springij;
                Compute(coords, out lenergy, out forceij, out springij, pchij, ee);
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
                    hessian[0, 1] += ForceField.GetHessianBlock(coords[0], coords[1], springij, forceij);
                    hessian[1, 0] += ForceField.GetHessianBlock(coords[1], coords[0], springij, forceij);
                }
            }
            public static void Compute(Vector[] coords, out double energy, out double forceij, out double springij, double pchij, double ee)
            {
                /// !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                /// !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                /// !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                ///
                /// V(rij) =           (332 * pchij / ee) * rij^-1
                /// F(rij) = (   -1) * (332 * pchij / ee) * rij^-2
                /// K(rij) = (-2*-1) * (332 * pchij / ee) * rij^-3
                double rij  = (coords[1] - coords[0]).Dist;
                double rij2 = rij*rij;
                double rij3 = rij*rij2;
                energy   =           (332 * pchij / ee) / rij;
                forceij  =    (-1) * (332 * pchij / ee) / rij2;
                springij = (-2*-1) * (332 * pchij / ee) / rij3;
                HDebug.AssertIf(forceij>0, pchij<0); // positive force => attractive
                HDebug.AssertIf(forceij<0, pchij>0); // negative force => repulsive
            }
            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out ValueTuple<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                Universe.Nonbonded nonbonded = (Universe.Nonbonded)info;

                int idx1 = 0; // nonbonded.atoms[0].ID;
                int idx2 = 1; // nonbonded.atoms[1].ID;
                Universe.Atom atom1 = nonbonded.atoms[0];
                Universe.Atom atom2 = nonbonded.atoms[1];
                double pchij = atom1.Charge * atom2.Charge;
                if(double.IsNaN(pchij))
                {
                    HDebug.Assert(false);
                    pwidxs = null;
                    pwhessinfos = null;
                    return;
                }

                Vector diff   = (coords[idx2] - coords[idx1]);
                double dx   = diff[0];
                double dy   = diff[1];
                double dz   = diff[2];
                double pchi = atom1.Charge;
                double pchj = atom2.Charge;
                double ee  = 80;

                // !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
                // !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
                // !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
                //
                // V(rij) =           (332 * pchij / ee) * rij^-1
                // F(rij) = (   -1) * (332 * pchij / ee) * rij^-2
                // K(rij) = (-2*-1) * (332 * pchij / ee) * rij^-3
//                double pchij = pchi * pchj;
                double rij2 = dx*dx + dy*dy + dz*dz;
                double rij  = Math.Sqrt(rij2);
                double rij3 = rij2*rij;
                double fij = (   -1) * (332 * pchij / ee) / rij2;
                double kij = (-2*-1) * (332 * pchij / ee) / rij3;

                pwidxs = new ValueTuple<int, int>[1];
                pwidxs[0] = new ValueTuple<int, int>(0, 1);
                pwhessinfos = new PwIntrActInfo[1];
                pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
        }
    }
}
