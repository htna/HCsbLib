using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class Paper
    {
        public static partial class Arnaud96
        {
            public static void Selftest()
            {
                Vector[] coords = new Vector[4]
                {
                    new double[3]{0,0,0},
                    new double[3]{0,1,0},
                    new double[3]{0,1,1},
                    new double[3]{0,0,1},
                };

                Vector dpi_dr = dPI_dR(coords);
                MatrixByArr d2pi_dr2 = d2PI_dR2(coords);
            }
            static bool HessSpring_selftest = HDebug.IsDebuggerAttached;
            public static MatrixByArr HessSpring(IList<Vector> coords, double K)
            {
                if(HessSpring_selftest)
                {
                    HessSpring_selftest = false;
                    HTLib3.Vector[] coords0 = new HTLib3.Vector[]
                    {
                        coords[0].ToArray(),
                        coords[1].ToArray(),
                        coords[2].ToArray(),
                        coords[3].ToArray(),
                    };
                    MatrixByArr HSpr0 = HTLib3.Bioinfo.Hess._HessTorSpr(coords0, K).ToArray();
                    MatrixByArr HSpr1 = HessSpring(coords, K);
                    HDebug.AssertTolerance(0.00000001, HSpr0-HSpr1);
                }

                Vector dpi_dr = dPI_dR(coords);
                MatrixByArr Hspr = K * LinAlg.VVt(dpi_dr, dpi_dr);
                return Hspr;
            }
            ////////////////////////////////////////////////////////////////////////////////////
            /// http://onlinelibrary.wiley.com/doi/10.1002/(SICI)1096-987X(19960715)17:9%3C1132::AID-JCC5%3E3.0.CO;2-T/abstract
            /// Blondel, A. and Karplus, M. (1996), New formulation for derivatives of torsion angles
            /// and improper torsion angles in molecular mechanics: Elimination of singularities.
            /// J. Comput. Chem., 17: 1132–1141.
            /// 
            /// ri                     rj        coordinates: ri, rj, rk, rl
            ///   \                    |         vectors: F := rj -> ri = ri-rj
            ///    \F    /A            |   /A             G := rj -> rk = rk-rj
            ///     \  /               | /                H := rk -> rl = rl-rk
            ///      rj------G--------rk pi      ortho  : A := A ⊥ ∆ijk = F×G
            ///                         \                 B := B ⊥ ∆jkl = H×G
            ///                           \B     angle  : cos(pi) = 〈A,B〉 / ‖A‖*‖B‖
            /// 
            ////////////////////////////////////////////////////////////////////////////////////
            public static Vector dPI_dR(IList<Vector> coords)
            {
                Vector ri = coords[0];
                Vector rj = coords[1];
                Vector rk = coords[2];
                Vector rl = coords[3];
                Vector F = ri - rj;
                Vector G = rj - rk;                double G1 = G.Dist;
                Vector H = rl - rk;
                Vector A = CrossProd(F, G); double A2 = A.Dist2;
                Vector B = CrossProd(H, G); double B2 = B.Dist2;

                double F_G = DotProd(F, G);
                double H_G = DotProd(H, G);
                Vector dpi_dri = -1 * (G1/A2) * A;
                Vector dpi_drj = (G1/A2)*A + (F_G/(A2*G1))*A - (H_G/(B2*G1))*B;
                Vector dpi_drk = (H_G/(B2*G1))*B - (F_G/(A2*G1))*A - (G1/B2)*B;
                Vector dpi_drl = (G1/B2)*B;
                Vector dpi_dr = new Vector(dpi_dri, dpi_drj, dpi_drk, dpi_drl);

                return dpi_dr;
            }
            public static Func<Vector,Vector,double> DotProd   = LinAlg.DotProd;
            public static Func<Vector,Vector,Vector> CrossProd = LinAlg.CrossProd;
            public static Func<Vector,Vector,MatrixByArr> TensrProd = LinAlg.VVt;
            public static MatrixByArr d2PI_dR2(IList<Vector> coords)
            {
                Vector ri = coords[0];
                Vector rj = coords[1];
                Vector rk = coords[2];
                Vector rl = coords[3];
                Vector F = ri - rj;
                Vector G = rj - rk; double G2 = G.Dist2; double G1 = Math.Sqrt(G2); double G3=G1*G2;
                Vector H = rl - rk;
                Vector A = LinAlg.CrossProd(F, G); double A2 = A.Dist2; double A4=A2*A2;
                Vector B = LinAlg.CrossProd(H, G); double B2 = B.Dist2; double B4=B2*B2;

                double FG = DotProd(H,G);
                Vector G_A = CrossProd(G,A);
                Vector F_A = CrossProd(F,A);
                Vector G_B = CrossProd(G,B);
                Vector H_B = CrossProd(H,B);

                Vector one3 = new double[3] { 1, 1, 1 };
                Vector dF_di =  one3;
                Vector dF_dj = -one3;
                Vector dG_dj =  one3;
                Vector dG_dk = -one3;
                Vector dH_dk = -one3;
                Vector dH_dl =  one3;

                MatrixByArr d2pi_dF2 = ( G1/A4) * (TensrProd(A, CrossProd(G, A)) + TensrProd(CrossProd(G, A), A));   /// (32)
                MatrixByArr d2pi_dH2 = (-G1/B4) * (TensrProd(B, CrossProd(G, B)) + TensrProd(CrossProd(G, B), B));   /// (33)
                MatrixByArr d2pi_dG2 = (1/(2*G3*A2)) * (TensrProd(G_A,A) + TensrProd(A,G_A))                         /// (44)
                                + (1/(  G1*A4)) * (TensrProd(A,F_A) + TensrProd(F_A,A))
                                - (1/(2*G3*B2)) * (TensrProd(G_B,B) + TensrProd(B,G_B))
                                - (1/(  G1*B4)) * (TensrProd(B,H_B) + TensrProd(H_B,B));

                HDebug.ToDo();
                //Matrix d2pi_dFdG =                                                                            /// (38)
                //Matrix d2pi_dGdH =                                                                            /// (39)
                MatrixByArr d2pi_dFdH = new double[3, 3];                                                            /// (40)

                return null;
            }
        }
    }
}
