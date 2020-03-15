using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class PwBond : IBond, IHessBuilder4PwIntrAct
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
                double lenergy, force01, spring01;
                Compute(coords, out lenergy, out force01, out spring01, Kb, b0);
                ///////////////////////////////////////////////////////////////////////////////
                // energy
                energy += lenergy;
                ///////////////////////////////////////////////////////////////////////////////
                // force
                if(forces != null)
                {
                    Vector frc0, frc1;
                    GetForceVector(coords[0], coords[1], force01, out frc0, out frc1);
                    forces[0] += frc0;
                    forces[1] += frc1;
                }
                ///////////////////////////////////////////////////////////////////////////////
                // hessian
                if(hessian != null)
                {
                    hessian[0, 1] += GetHessianBlock(coords[0], coords[1], spring01, force01);
                    hessian[1, 0] += GetHessianBlock(coords[1], coords[0], spring01, force01);
                }
            }
            public static void Compute(Vector[] coords, out double energy, out double force01, out double spring01, double Kb, double b0)
            {
                /// BONDS
                /// !V(bond) = Kb(b - b0)**2
                ///
                ///      p0 ---------- p1
                ///     (+) --→   ←-- (+)
                /// ←-- (-)           (-) --→
                ///
                ///     V        = Kb ( b - b0 ) ^ 2
                ///   d_V / d_b  = 2 Kb ( b - b0 )
                ///  d2_V / d_b2 = 2 Kb
                ///
                ///     b        = b1 + t
                ///   d_b / d_t  = 1
                ///  d2_b / d_t2 = 0
                ///       
                /// energy = V
                ///        = Kb ( b - b0 ) ^ 2
                ///        
                /// force  = d_V / d_t
                ///        = {d_V / d_b      } * {d_b / d_t}
                ///        = {2 Kb ( b - b0 )} * { 1       }
                ///        =  2 Kb ( b - b0 )
                ///        
                /// spring = d_V2 / d2_t
                ///        = d_(d_V / d_t) / d_t
                ///        = d_({d_V / d_b } * {d_b / d_t}) / d_t
                ///        =   {d_{d_V / d_b } / d_t}        *      {d_b / d_t}
                ///          +    {d_V / d_b }               *   {d_{d_b / d_t} / d_t}
                ///        =     {d2_V / d_b2}*{d_b / d_t}   *      {d_b / d_t}
                ///          +    {d_V / d_b }               *     {d2_b / d_t2}
                ///        = {d2_V / d_b2}*{d_b / d_t}*{d_b / d_t}      +      {d_V / d_b    }*{d2_b / d_t2}
                ///        = {2 Kb       }*{ 1       }*{ 1       }      +      {2 Kb (b - b0)}*{ 0         }
                ///        =  2 Kb
                ///////////////////////////////////////////////////////////////////////////////
                Vector pos1 = coords[0];
                Vector pos2 = coords[1];
                double b = (pos1 - pos2).Dist;
                HDebug.Assert(b != 0, double.IsNaN(b) == false, double.IsInfinity(b) == false);
                energy   =     Kb * (b-b0) * (b-b0);
                force01  = 2 * Kb * (b-b0);
                spring01 = 2 * Kb;
                HDebug.AssertIf(force01>0, b0<b ); // positive force => attractive
                HDebug.AssertIf(force01<0, b <b0); // negative force => repulsive
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
