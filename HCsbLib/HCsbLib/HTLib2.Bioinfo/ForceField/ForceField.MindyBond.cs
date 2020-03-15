using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class MindyBond : IBond, IHessBuilder4PwIntrAct
        {
            // BONDS
            // !
            // !V(bond) = Kb(b - b0)**2
            // !
            // !Kb: kcal/mole/A**2
            // !b0: A
            // !
            // !atom type Kb          b0
            // !
            // CN8  NN6    200.000     1.480   ! methylammonium
            // NN6  HN1    403.000     1.040   ! methylammonium
            // ...
            /////////////////////////////////////////////////////////////
            public virtual string[] FrcFldType { get { return new string[] { "Bond", "Mindy" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            static double optHessianForceFactor = 1;
            public virtual void EnvClear() { optHessianForceFactor = 1; }
            public virtual bool EnvAdd(string key, object value)
            {
                if(key == "optHessianForceFactor")
                {
                    optHessianForceFactor = (double)value;
                    return true;
                }
                return false;
            }
            public virtual void Compute(Universe.Bond bond, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                double Kb = bond.Kb;
                double b0 = bond.b0;
                Compute(coords, ref energy, ref forces, ref hessian, Kb, b0, pwfrc, pwspr);
            }
            public static void Compute(Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                       double Kb, double b0, double[,] pwfrc=null, double[,] pwspr=null)
            {
                #region original source in mindy
                // double ComputeBonded::compute_bonds(const Vector *coords, Vector *f) const {
                //   double energy = 0.0;
                //   BondElem *bond = bonds;
                //   for (int i=0; i<nbonds; i++) {
                //     Vector r12 = coords[bond->atom1] - coords[bond->atom2];
                //     double r = r12.length();
                //     double diff = r - bond->x0;
                //     Vector f12 = r12 * diff * (-2*bond->k) / r;
                //     energy += bond->k * diff * diff; 
                //     f[bond->atom1] += f12;
                //     f[bond->atom2] -= f12; 
                //     bond++;
                //   }
                //   return energy;
                // }
                #endregion

                ///////////////////////////////////////////////////////////////////////////////
                // energy
                Vector pos1 = coords[0];    
                Vector pos2 = coords[1];
                Vector r12 = pos1 - pos2;       //     Vector r12 = coords[bond->atom1] - coords[bond->atom2];
                double r = r12.Dist;            //     double r = r12.length();
                HDebug.Assert(r != 0, double.IsNaN(r) == false, double.IsInfinity(r) == false);
                double diff = r - b0;           //     double diff = r - bond->x0;
                HDebug.Assert(double.IsNaN(Kb * diff * diff) == false, double.IsInfinity(Kb * diff * diff) == false);
                energy += Kb * diff * diff;     //     energy += bond->k * diff * diff; 
                ///////////////////////////////////////////////////////////////////////////////
                // force
                if(forces != null)
                {
                    Vector f12 = r12 * diff * (-2*Kb) / r;    //     Vector f12 = r12 * diff * (-2*bond->k) / r;
                    forces[0] += f12;                     //     f[bond->atom1] += f12;
                    forces[1] -= f12;                     //     f[bond->atom2] -= f12; 
                }
                ///////////////////////////////////////////////////////////////////////////////
                // hessian
                if(hessian != null)
                {
                    //// !V(bond) = Kb(b - b0)**2
                    //hessian[0,1].Kij = 2 * Kb;
                    //hessian[0,1].Fij = diff * (-2*Kb);
                    //hessian[1,0].Kij = 2 * Kb;
                    //hessian[1,0].Fij = diff * (-2*Kb);
                    //Debug.Assert(false);

                    //Debug.Assert(false);
                    double dcoord = 0.0001;
                    HDebug.Assert(hessian.GetLength(0) == 2, hessian.GetLength(1) == 2);
                    NumericSolver.Derivative2(ComputeFunc, coords, dcoord, ref hessian, Kb, b0);
                }
            }
            public static double ComputeFunc(Vector[] coords, double[] info)
            {
                HDebug.Assert(info.Length == 2);
                double energy = 0;
                Vector[] forces = null;
                MatrixByArr[,] hessian = null;
                Compute(coords, ref energy, ref forces, ref hessian, info[0], info[1]);
                return energy;
            }
            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out ValueTuple<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                Universe.Bond bond = (Universe.Bond)info;
                double Kb = bond.Kb;
                double b0 = bond.b0;

                HDebug.Assert(coords.Length == 2);
                Vector pos1 = coords[0];
                Vector pos2 = coords[1];
                Vector r12 = pos1 - pos2;
                double r = r12.Dist;

                double kij = 2 * Kb;
                double fij = (2*Kb) * (r - b0);

                pwidxs = new ValueTuple<int, int>[1];
                pwidxs[0] = new ValueTuple<int, int>(0, 1);
                pwhessinfos = new PwIntrActInfo[1];
                pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
        }
    }
}
