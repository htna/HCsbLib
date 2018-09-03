using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class PwAngle : IAngle, IHessBuilder4PwIntrAct
        {
            // ANGLES
            // !
            // !V(angle) = Ktheta(Theta - Theta0)**2
            // !
            // !V(Urey-Bradley) = Kub(S - S0)**2
            // !
            // !Ktheta: kcal/mole/rad**2
            // !Theta0: degrees
            // !Kub: kcal/mole/A**2 (Urey-Bradley)
            // !S0: A
            // !
            // !atom types     Ktheta    Theta0   Kub     S0
            // !
            // CN7  CN8  CN8      58.35    113.60   11.16   2.561 !alkane
            // CN8  CN7  CN8      58.35    113.60   11.16   2.561 !alkane
            // ...
            /////////////////////////////////////////////////////////////
            public virtual string[] FrcFldType { get { return new string[] { "Angle", "Mindy" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.0001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public virtual void Compute(Universe.Angle angle, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
            {
                double Ktheta = angle.Ktheta;
                double Theta0 = angle.Theta0;
                double Kub    = angle.Kub   ;
                double S0     = angle.S0    ;
                Compute(coords, ref energy, ref forces, ref hessian, Ktheta, Theta0, Kub, S0, pwfrc, pwspr);
            }

            public static void Compute(Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                       double Ktheta, double Theta0, double Kub, double S0, double[,] pwfrc=null, double[,] pwspr=null)
            {
                double lenergy, force02, spring02;
                Compute(coords, out lenergy, out force02, out spring02, Ktheta, Theta0, Kub, S0);
                ///////////////////////////////////////////////////////////////////////////////
                // energy
                energy += lenergy;
                ///////////////////////////////////////////////////////////////////////////////
                // force
                if(forces != null)
                {
                    Vector frc0, frc2;
                    GetForceVector(coords[0], coords[2], force02, out frc0, out frc2);
                    forces[0] += frc0;
                    forces[2] += frc2;
                }
                ///////////////////////////////////////////////////////////////////////////////
                // hessian
                if(hessian != null)
                {
                    hessian[0, 2] += GetHessianBlock(coords[0], coords[2], spring02, force02);
                    hessian[2, 0] += GetHessianBlock(coords[2], coords[0], spring02, force02);
                }
            }
            public static void Compute(Vector[] coords, out double energy, out double force02, out double spring02,
                                       double Ktheta, double Theta0, double Kub, double S0)
            {
                Func<double,double> sqrt = Math.Sqrt;
                Func<double,double> acos = Math.Acos;
                Func<double,double,double> pow  = Math.Pow;
                /// ANGLES
                /// !V(angle) = Ktheta(Theta - Theta0)**2
                /// !V(Urey-Bradley) = Kub(S - S0)**2          // ignore this term here...
                ///                             
                ///             p1              
                ///            /θ \             
                ///           /    \            
                ///          b      c           
                ///         /        \          
                ///        /          \         
                ///      p0 ----a----- p2
                ///     (+) --→   ←-- (+)
                /// ←-- (-)           (-) --→
                ///
                ///     V        =   Kθ ( θ - θ0 ) ^ 2
                ///   d_V / d_θ  = 2 Kθ ( θ - θ0 )
                ///  d2_V / d_θ2 = 2 Kθ
                ///
                ///     θ        = acos( (b^2 + c^2 - a^2)/(2*b*c) )
                ///              = acos( (b2  + c2  - a2 )/(2*b*c) )
                ///              = acos( cosθ )                        ⇐ cosθ = (b^2 + c^2 - a^2)/(2*b*c)
                ///   d_θ / d_a  = a / (b c sqrt(1 - cosθ2))           ⇐ cosθ2 = cosθ * cosθ
                ///  d2_θ / d_a2 = {a2/(b2 c2)} * {-cosθ / sqrt(1 - cosθ2)^3} + {1/(b c sqrt(1 - cosθ2))}
                ///       
                /// energy = V
                /// force  = d_V / d_a
                ///        = {d_V / d_θ} * {d_θ / d_a}
                /// spring = d_V2 / d2_a
                ///        = d_(d_V / d_a) / d_a
                ///        = d_({d_V / d_θ } * {d_θ / d_a}) / d_a
                ///        =   {d_{d_V / d_θ } / d_a}        *      {d_θ / d_a}
                ///          +    {d_V / d_θ }               *   {d_{d_θ / d_a} / d_a}
                ///        =     {d2_V / d_θ2}*{d_θ / d_a}   *      {d_θ / d_a}
                ///          +    {d_V / d_θ }               *     {d2_θ / d_a2}
                ///        = {d2_V / d_θ2}*{d_θ / d_a}*{d_θ / d_a}      +      {d_V / d_θ}*{d2_θ / d_a2}
                ///////////////////////////////////////////////////////////////////////////////
                Vector p0 = coords[0];
                Vector p1 = coords[1];
                Vector p2 = coords[2];
                double a = (p0 - p2).Dist; double a2 = a*a;
                double b = (p0 - p1).Dist; double b2 = b*b;
                double c = (p1 - p2).Dist; double c2 = c*c;
                double KA = Ktheta;
                double A0 = Theta0;
                double cosA = (b2 + c2 - a2)/(2*b*c); double cosA2 = cosA*cosA;
                double   A     = acos(cosA);
                double  dA_da  = a / (b*c*sqrt(1 - cosA2));
                double d2A_da2 = (a2/(b2*c2)) * (-cosA/pow(sqrt(1 - cosA2),3)) + (1/(b*c*sqrt(1-cosA2)));
                double   V     =   KA*( A - A0 )*( A - A0 );
                double  dV_dA  = 2*KA*( A - A0 );
                double d2V_dA2 = 2*KA;
                energy   = V;
                force02  = dV_dA * dA_da;
                spring02 = d2V_dA2*dA_da*dA_da  +  dV_dA*d2A_da2;
                HDebug.AssertIf(force02>0, A0<A ); // positive force => attractive
                HDebug.AssertIf(force02<0, A <A0); // negative force => repulsive
            }

            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out Pair<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                Universe.Angle angle = (Universe.Angle)info;
                double Ktheta = angle.Ktheta;
                double Theta0 = angle.Theta0;
                double Kub    = angle.Kub   ;
                double S0     = angle.S0    ;

                Vector pos1 = coords[0];  // const Vector *pos1 = coords + angle->atom1;
                Vector pos2 = coords[1];  // const Vector *pos2 = coords + angle->atom2;
                Vector pos3 = coords[2];  // const Vector *pos3 = coords + angle->atom3;
                double a = (pos2-pos1).Dist;
                double b = (pos3-pos2).Dist;
                double c = (pos3-pos1).Dist;
                ValueTuple<double,double> fij_kij = GetFijKij(a, b, c, Ktheta, Theta0);
                double fij = fij_kij.Item1;
                double kij = fij_kij.Item2;

                HDebug.Assert(coords.Length == 3);
                pwidxs = new Pair<int,int>[1];
                pwidxs[0] = new Pair<int,int>(0,2);
                pwhessinfos = new PwIntrActInfo[1];
                pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
            public static ValueTuple<double, double> GetFijKij(double a, double b, double c, double K0, double T0, double fij_sign=+1, double kij_sign0=+1, double kij_sign1=+1)
            {
                ///   +
                ///   |\
                ///   | \
                /// a |  \ c
                ///   |   \
                ///   |T   \
                ///   +-----+
                ///      b
                ///      
                /// V                   =   K0 (T - T0)^2
                /// F =           dV/dT = 2 K0 (T - T0)
                /// K = (d^2 V)/(d T^2) = 2 K0
                /// return {  dV/dc,  (d^2 V)/(d c^2)  }
                double a2  = a*a;
                double b2  = b*b;
                double c2  = c*c; double c4 = c2*c2;
                double ab = a*b;  double ab2 = ab*ab;
                
                double T = acos((a2+b2-c2)/(2*ab));                                    // θ               = acos⁡((a^2+b^2-c^2)/2ab)
                double V = K0*(T - T0)*(T - T0);                                       // V               =  K0 ( θ - θ0 )^2
                double dV_dT   = 2*K0*(T-T0);                                          // ∂V/∂θ           = 2K0 ( θ - θ0 )
                double d2V_dT2 = 2*K0;                                                 // (∂^2 V)/(∂θ^2 ) = 2K0
                double dT_dc   = 2*c               /sqrt((4*ab2)-pow(a2+b2-c2,2));     // ∂θ/∂c           = 2c/√((2ab)^2-(a^2+b^2-c^2 )^2 )
                double d2T_dc2 = 2*(c4-pow(a2-b2,2))/pow((4*ab2)-pow(a2+b2-c2,2),1.5); // (∂^2 θ)/(∂c^2 ) = 2(c^4-(a^2-b^2 )^2 )/((2ab)^2-(a^2+b^2-c^2 )^2 )^1.5
                double dV_dc   = 2*K0*(T-T0)*2*c   /sqrt((4*ab2)-pow(a2+b2-c2, 2));    // ∂V/∂c           = 2K0(θ-θ0 ) ⋅ 2c/√((2ab)^2-(a^2+b^2-c^2 )^2 )
                double d2V_dc2 = kij_sign0 * (2*K0*4*c2/((4*ab2)-pow(a2+b2-c2, 2)))    // (∂^2 V)/(∂c^2 ) = {2K_0⋅(4c^2)/((2ab)^2-(a^2+b^2-c^2 )^2 )}
                               + kij_sign1 * (2*K0*(T-T0)*2*(c4-pow(a2-b2, 2))         //                 + {2K_0 (θ-θ_0 )⋅2(c^4-(a^2-b^2 )^2 )
                                               /pow((4*ab2)-pow(a2+b2-c2,2),1.5)       //                    /((2ab)^2-(a^2+b^2-c^2 )^2 )^1.5 }
                                             );
                double fij = dV_dc;
                double kij = d2V_dc2;
                HDebug.AssertIf(fij>0,   T0<T); // positive force => attractive
                HDebug.AssertIf(fij<0, T<T0  ); // negative force => repulsive
                return new ValueTuple<double, double>(fij, kij);
            }
            public static double acos(double d)            { return Math.Acos(d);   }
            public static double sqrt(double d)            { return Math.Sqrt(d);   }
            public static double pow(double x, double y)   { return Math.Pow(x, y); }
        }
    }
}
