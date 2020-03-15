using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public class PwImproper : IImproper, IHessBuilder4PwIntrAct
		{
			// IMPROPER
			// !
			// !V(improper) = Kpsi(psi - psi0)**2
			// !
			// !Kpsi: kcal/mole/rad**2
			// !psi0: degrees
			// !note that the second column of numbers (0) is ignored
			// !
			// !atom types           Kpsi                   psi0
			// !
			// HN2  X    X    NN2      1.0     0     0.0     !C, adm jr. 11/97
			// NN2G CN4  CN1  HN2      0.8     0     0.0     !Inosine, adm jr. 2/94
			// ...
			/////////////////////////////////////////////////////////////
            public virtual string[] FrcFldType { get { return new string[] { "Improper", "Mindy" }; } }
            public virtual double? GetDefaultMinimizeStep() { return 0.00001; }
            public virtual void EnvClear() { }
            public virtual bool EnvAdd(string key, object value) { return false; }
            public virtual void Compute(Universe.Improper improper, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null)
			{
                double Kchi  = improper.Kpsi;
                int    n     = improper.n   ;
                double delta = improper.psi0;
                Compute(coords, ref energy, ref forces, ref hessian, Kchi, n, delta, pwfrc, pwspr);
            }
            public static void Compute(Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian,
                                       double Kchi, int n, double delta, double[,] pwfrc=null, double[,] pwspr=null)
			{
                double Kpsi = Kchi;
                HDebug.Assert(n == 0);
                double psi0 = delta;

                double lenergy, force03, spring03;
                Compute(coords, out lenergy, out force03, out spring03, Kpsi, psi0);
                ///////////////////////////////////////////////////////////////////////////////
                // energy
                energy += lenergy;
                ///////////////////////////////////////////////////////////////////////////////
                // force
                if(forces != null)
                {
                    Vector frc0, frc3;
                    GetForceVector(coords[0], coords[3], force03, out frc0, out frc3);
                    forces[0] += frc0;
                    forces[3] += frc3;
                }
                ///////////////////////////////////////////////////////////////////////////////
                // hessian
                if(hessian != null)
                {
                    hessian[0, 3] += GetHessianBlock(coords[0], coords[3], spring03, force03);
                    hessian[3, 0] += GetHessianBlock(coords[3], coords[0], spring03, force03);
                }
			}
            public static void Compute(Vector[] coords, out double energy, out double force03, out double spring03,
                                       double Kpsi, double psi0)
            {
                Func<double, double> sqrt = Math.Sqrt;
                Func<double, double> acos = Math.Acos;
                Func<double, double, double> pow  = Math.Pow;
                /// IMPROPER
                /// !V(improper) = Kpsi(psi - psi0)**2
                /// 
                ///                                                        nABC = (B-A) x (C-B)
                ///          nABC                                          nBCD = (C-B) x (D-C)
                ///           |                                               X = line(B,C) ∩ plane(A, nABC x nBCD)
                ///       A . | . .  C     nBCD      A          C             Y = line(B,C) ∩ plane(D, nABC x nBCD)
                ///       |   |     / \   /          |         / \            ψ = ∠ plane(A,B,C) and plane(B,C,D)
                ///       |   |   /    \/            |       Y    \             = ∠ nABC and nBCD
                ///       |     /     / \            |     /   \   \            = ∠ A X D'
                ///       |   /     /    \           |   X       \  \
                ///       | /             \          | /   \       \ \       plane(A,X,D') ⊥ line(D,D')
                ///       B . . . . . . . D          B       \       D          line(X,D') ⊥ line(D,D')
                ///                                            \   /
                ///                                              D'          D' = D + (Y→X)
                ///                                                             = D + X - Y
                ///               A                           
                ///             / | \                         
                ///           /   |   \                       
                ///         b     |     \                     
                ///       /       |       \                   plane(A,X,D') ⊥ line(D,D')
                ///     /         |         \                 plane(A,X,D') != plane(A,D',D)
                ///    X ψ        a           e               a = (D'-A).dist = (D+X-Y - A).dist = sqrt(e^2-d^2)
                ///     \         |             \             b = (A -X).dist
                ///       \       |               \           c = (D'-X).dist = (D+X-Y - X).dist = (D-Y).dist
                ///         c     |                 \         d = (D'-D).dist = (D+X-Y - D).dist = (X-Y).dist
                ///           \   |                   \       e = (A -D).dist
                ///             \ | ⊥                   \    
                ///               D' -------------d----- D   
                ///
                ///     V        =   Kψ ( ψ - ψ0 ) ^ 2
                ///   d_V / d_ψ  = 2 Kψ ( ψ - ψ0 )
                ///  d2_V / d_ψ2 = 2 Kψ
                ///
                ///     ψ        = acos( (b^2 + c^2 - a^2)/(2*b*c) )
                ///              = acos( (b^2 + c^2 - (e^2-d^2))/(2*b*c) )
                ///              = acos( (b2  + c2 + d2 - e2 )/(2*b*c) )
                ///              = acos( cosψ )                        ⇐ cosψ = (b2  + c2 + d2 - e2 )/(2*b*c)
                ///   d_ψ / d_e  = e / (b c sqrt(1 - cosψ2))           ⇐ cosψ2 = cosψ * cosψ
                ///  d2_ψ / d_e2 = {e2/(b2 c2)} * {-cosψ / sqrt(1 - cosψ2)^3} + {1/(b c sqrt(1 - cosψ2))}
                ///  
                /// energy = V
                /// force  = d_V / d_e
                ///        = {d_V / d_ψ} * {d_ψ / d_e}
                /// spring = d_V2 / d2_e
                ///        = d_(d_V / d_e) / d_e
                ///        = d_({d_V / d_ψ } * {d_ψ / d_e}) / d_e
                ///        =   {d_{d_V / d_ψ } / d_e}        *      {d_ψ / d_e}
                ///          +    {d_V / d_ψ }               *   {d_{d_ψ / d_e} / d_e}
                ///        =     {d2_V / d_ψ2}*{d_ψ / d_e}   *      {d_ψ / d_e}
                ///          +    {d_V / d_ψ }               *     {d2_ψ / d_e2}
                ///        = {d2_V / d_ψ2}*{d_ψ / d_e}*{d_ψ / d_e}      +      {d_V / d_ψ}*{d2_ψ / d_e2}
                ///        
                double Kψ = Kpsi;
                double ψ0 = psi0;
                Vector A = coords[0];
                Vector B = coords[1];
                Vector C = coords[2];
                Vector D = coords[3];
                Vector nABC = LinAlg.CrossProd(B-A, C-B);   // nABC = (B-A) x (C-B)
                Vector nBCD = LinAlg.CrossProd(C-B, D-C);   // nBCD = (C-B) x (D-C)
                Vector nABCxnBCD = LinAlg.CrossProd(nABC, nBCD);
                Vector X = Geometry.Point4LinePlaneIntersect(nABCxnBCD, A, B-C, B); // X = line(B,C) ∩ plane(A, nABC x nBCD)
                Vector Y = Geometry.Point4LinePlaneIntersect(nABCxnBCD, D, B-C, B); // Y = line(B,C) ∩ plane(D, nABC x nBCD)
                double b = (A - X).Dist;  double b2=b*b;  // b = (A-X).dist
                double c = (D - Y).Dist;  double c2=c*c;  // c = (D-Y).dist
                double d = (X - Y).Dist;  double d2=d*d;  // d = (X-Y).dist
                double e = (A - D).Dist;  double e2=e*e;  // e = (A-D).dist
                double a = sqrt(e*e-d*d); double a2=a*a;  // a = sqrt(e^2-d^2)
                double cosψ = (b2  + c2 + d2 - e2 )/(2*b*c); double cosψ2 = cosψ*cosψ;
                double   ψ     = acos(cosψ);                                                            //     ψ        = acos( cosψ )
                double  dψ_de  = e / (b*c*sqrt(1 - cosψ2));                                             //   d_ψ / d_e  = e / (b c sqrt(1 - cosψ2))
                double d2ψ_de2 = (e2/(b2*c2)) * (-cosψ/pow(sqrt(1-cosψ2),3)) + (1/(b*c*sqrt(1-cosψ2))); //  d2_ψ / d_e2 = {e2/(b2 c2)} * {-cosψ / sqrt(1 - cosψ2)^3} + {1/(b c sqrt(1 - cosψ2))}
                double   V     =   Kψ*( ψ - ψ0 )*( ψ - ψ0 );    //     V        =   Kψ ( ψ - ψ0 ) ^ 2
                double  dV_dψ  = 2*Kψ*( ψ - ψ0 );               //   d_V / d_ψ  = 2 Kψ ( ψ - ψ0 )
                double d2V_dψ2 = 2*Kψ;                          //  d2_V / d_ψ2 = 2 Kψ

                energy   = V;                                   // energy = V
                force03  = dV_dψ * dψ_de;                       // force  = {d_V / d_ψ} * {d_ψ / d_e}
                spring03 = d2V_dψ2*dψ_de*dψ_de + dV_dψ*d2ψ_de2; // spring = {d2_V / d_ψ2}*{d_ψ / d_e}*{d_ψ / d_e}      +      {d_V / d_ψ}*{d2_ψ / d_e2}
            }
            public void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out ValueTuple<int, int>[] pwidxs, out PwIntrActInfo[] pwhessinfos)
            {
                pwidxs = null;
                pwhessinfos = null;
                //Universe.Improper improper = (Universe.Improper)info;
                //double Kpsi  = improper.Kpsi;
                ////int    n     = improper.param.n;
                //double psi0 = improper.psi0;
                //Vector a = coords[0];
                //Vector b = coords[1];
                //Vector c = coords[2];
                //Vector d = coords[3];
                //Pair<double, double> fij_kij = GetFijKij(Kpsi, psi0, a, b, c, d);
                //double fij = fij_kij.first;
                //double kij = fij_kij.second;
                //
                //Debug.Assert(coords.Length == 4);
                //pwidxs = new Pair<int, int>[1];
                //pwidxs[0] = new Pair<int, int>(0, 3);
                //pwhessinfos = new PwIntrActInfo[1];
                //pwhessinfos[0] = new PwIntrActInfo(kij, fij);
            }
        }
	}
}
