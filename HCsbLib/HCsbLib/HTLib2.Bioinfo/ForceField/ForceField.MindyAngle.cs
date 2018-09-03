using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class MindyAngle : IAngle, IHessBuilder4PwIntrAct
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
                #region original source in mindy
                // double ComputeBonded::compute_angles(const Vector *coords, Vector *f) const {
                //   double energy = 0.0;
                //   AngleElem *angle = angles;
                //   for (int i=0; i<nangles; i++) {
                //     Vector f1, f2, f3;
                //     const Vector *pos1 = coords + angle->atom1;
                //     const Vector *pos2 = coords + angle->atom2;
                //     const Vector *pos3 = coords + angle->atom3;
                //     Vector r12 = *pos1 - *pos2;
                //     Vector r32 = *pos3 - *pos2;
                //     double d12 = r12.length();
                //     double d32 = r32.length();
                //     double cos_theta = (r12*r32)/(d12*d32);
                //     if (cos_theta > 1.0) cos_theta = 1.0;
                //     else if (cos_theta < -1.0) cos_theta = -1.0;
                //     double sin_theta = sqrt(1.0 - cos_theta * cos_theta);
                //     double theta = acos(cos_theta);
                //     double diff = theta-angle->theta0;
                //     energy += angle->k * diff * diff; 
                //
                //     // forces
                //     double d12inv = 1.0/d12;
                //     double d32inv = 1.0/d32;
                //     diff *= (-2.0*angle->k) / sin_theta;
                //     double c1 = diff * d12inv;
                //     double c2 = diff * d32inv;
                //     Vector f12 = c1*(r12*(d12inv*cos_theta) - r32*d32inv);
                //     f1 = f12;
                //     Vector f32 = c2*(r32*(d32inv*cos_theta) - r12*d12inv);
                //     f3 = f32;
                //     f2 = -f12 - f32;
                //
                //     if (angle->k_ub > 0.0) {
                //       Vector r13 = r12 - r32;
                //       double d13 = r13.length();
                //       diff = d13 - angle->r_ub;
                //       energy += angle->k_ub * diff * diff;
                //
                //       // ub forces
                //       diff *= -2.0*angle->k_ub / d13;
                //       r13 *= diff;
                //       f1 += r13;
                //       f3 -= r13;
                //     } 
                //     f[angle->atom1] += f1;
                //     f[angle->atom2] += f2;
                //     f[angle->atom3] += f3;
                //
                //     angle++;
                //   }
                //   return energy;
                // }
                #endregion

                ///////////////////////////////////////////////////////////////////////////////
                // energy
                Vector f1, f2, f3;
                Vector pos1 = coords[0];                                    // const Vector *pos1 = coords + angle->atom1;
                Vector pos2 = coords[1];                                    // const Vector *pos2 = coords + angle->atom2;
                Vector pos3 = coords[2];                                    // const Vector *pos3 = coords + angle->atom3;
                Vector r12 = pos1 - pos2;                                   // Vector r12 = *pos1 - *pos2;
                Vector r32 = pos3 - pos2;                                   // Vector r32 = *pos3 - *pos2;
                Vector r13 = r12 - r32;                                     // Vector r13 = r12 - r32;
                double d12 = r12.Dist;                                      // double d12 = r12.length();
                double d32 = r32.Dist;                                      // double d32 = r32.length();
                double d13 = r13.Dist;                                      // double d13 = r13.length();
                double cos_theta = LinAlg.DotProd(r12, r32)/(d12*d32);      // double cos_theta = (r12*r32)/(d12*d32);
                if(cos_theta > 1.0) cos_theta = 1.0;                        // if (cos_theta > 1.0) cos_theta = 1.0;
                else if(cos_theta < -1.0) cos_theta = -1.0;                 // else if (cos_theta < -1.0) cos_theta = -1.0;
                double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);  // double sin_theta = sqrt(1.0 - cos_theta * cos_theta);
                double theta = Math.Acos(cos_theta);                        // double theta = acos(cos_theta);
                double diff = theta - Theta0;                               // double diff = theta-angle->theta0;
                double diff_ub = d13 - S0;                                  // double diff_ub = d13 - angle->r_ub;
                HDebug.Assert(double.IsNaN(Ktheta * diff * diff) == false, double.IsInfinity(Ktheta * diff * diff) == false);
                energy += Ktheta * diff * diff;                             // energy += angle->k * diff * diff; 
                if(Kub > 0.0)
                {                                                           //     if (angle->k_ub > 0.0) {
                    energy += Kub * diff_ub * diff_ub;                      //       energy += angle->k_ub * diff_ub * diff_ub;
                }                                                           //     } 
                ///////////////////////////////////////////////////////////////////////////////
                // force
                //
                //
                // assume: * angle-change(p1,p2,p3) is close to zero
                //        -> angle-force(p1,p2,p3) is close to arc  p1-p3
                //        -> angle-force(p1,p2,p3) is close to line p1-q
                //        -> force magnitude p2-p1 = force p1-q / sin(p1,p2,p3)
                //           force magnitude p2-p3 = force p1-q / tan(p1,p2,p3)
                //        -> force p2->p1 = f21 = unitvec(p1-p2) * force p1-q / sin(p1,p2,p3)
                //           force p2->p3 = f23 = unitvec(p3-p2) * force p1-q / tan(p1,p2,p3)
                //        -> force p1     = f21
                //           force p3     = f23
                //           force p2     = -f21 + -f23
                //
                //           p1         
                //         / |
                //       /   |
                //     /     |
                //  p2 ------+--p3
                //           q
                //
                if(forces != null)
                {
                    // forces                                               //     // forces
                    double d12inv = 1.0/d12;                                //     double d12inv = 1.0/d12;
                    double d32inv = 1.0/d32;                                //     double d32inv = 1.0/d32;
                    diff *= (-2.0*Ktheta) / sin_theta;                      //     diff *= (-2.0*angle->k) / sin_theta;
                    double c1 = diff * d12inv;                              //     double c1 = diff * d12inv;
                    double c2 = diff * d32inv;                              //     double c2 = diff * d32inv;
                    Vector f12 = c1*(r12*(d12inv*cos_theta) - r32*d32inv);  //     Vector f12 = c1*(r12*(d12inv*cos_theta) - r32*d32inv);
                    f1 = f12;                                               //     f1 = f12;
                    Vector f32 = c2*(r32*(d32inv*cos_theta) - r12*d12inv);  //     Vector f32 = c2*(r32*(d32inv*cos_theta) - r12*d12inv);
                    f3 = f32;                                               //     f3 = f32;
                    f2 = -f12 - f32;                                        //     f2 = -f12 - f32;
                    //
                    if(Kub > 0.0)
                    {                                                       //     if (angle->k_ub > 0.0) {
                        // ub forces                                        //       // ub forces
                        double diff_ub_ = diff_ub * -2.0*Kub / d13;         //       diff_ub *= -2.0*angle->k_ub / d13;
                        r13 *= diff_ub_;                                    //       r13 *= diff_ub;
                        f1 += r13;                                          //       f1 += r13;
                        f3 -= r13;                                          //       f3 -= r13;
                    }                                                       //     } 
                    forces[0] += f1;                                        //     f[angle->atom1] += f1;
                    forces[1] += f2;                                        //     f[angle->atom2] += f2;
                    forces[2] += f3;                                        //     f[angle->atom3] += f3;
                }
                ///////////////////////////////////////////////////////////////////////////////
                // hessian
                if(hessian != null)
                {
                    //// !V(angle) = Ktheta(Theta - Theta0)**2
                    //Debug.Assert(false);
                    //// !V(Urey-Bradley) = Kub(S - S0)**2
                    //hessian[0, 2].Kij += 2 * Kub;
                    //hessian[0, 2].Fij += diff_ub * (2*Kub);
                    //hessian[2, 0].Kij += 2 * Kub;
                    //hessian[2, 0].Fij += diff_ub  * (2*Kub);

                    //Debug.Assert(false);
                    double dcoord = 0.0001;
                    HDebug.Assert(hessian.GetLength(0) == 3, hessian.GetLength(1) == 3);
                    NumericSolver.Derivative2(ComputeFunc, coords, dcoord, ref hessian, Ktheta, Theta0, Kub, S0);
                }
            }
            public static double ComputeFunc(Vector[] coords, double[] info)
            {
                HDebug.Assert(info.Length == 4);
                double energy = 0;
                Vector[] forces = null;
                MatrixByArr[,] hessian = null;
                Compute(coords, ref energy, ref forces, ref hessian, info[0], info[1], info[2], info[3]);
                return energy;
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
                return new ValueTuple<double, double>(fij, kij);
            }
            public static double acos(double d)            { return Math.Acos(d);   }
            public static double sqrt(double d)            { return Math.Sqrt(d);   }
            public static double pow(double x, double y)   { return Math.Pow(x, y); }
        }
    }
}

