using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Bioinfo
    {
        public static partial class Hess
        {
            public static Matrix _HessTorSpr(Vector[] coords, double K)
            {
                ////////////////////////////////////////////////////////////////////////////////////
                /// ref: Arnaud Blondel and Martin Karplus, "New Formulation for Derivatives of
                ///      Torsion Angles and Improper Torsion Angles in Molecular Mechanics,"
                ///      Journal of Computational Chemistry, Vol. 17, No. 9, 1132-1141 (1996)
                ////////////////////////////////////////////////////////////////////////////////////

                Vector ri = coords[0];
                Vector rj = coords[1];
                Vector rk = coords[2];
                Vector rl = coords[3];
                Vector F = ri - rj;
                Vector G = rj - rk;                double G1 = G.Dist;
                Vector H = rl - rk;
                Vector A = Vector.CrossProd(F, G); double A2 = A.Dist2;
                Vector B = Vector.CrossProd(H, G); double B2 = B.Dist2;

                double F_G = Vector.DotProd(F, G);
                double H_G = Vector.DotProd(H, G);
                Vector dpi_dri = -1 * (G1/A2) * A;
                Vector dpi_drj = (G1/A2)*A + (F_G/(A2*G1))*A - (H_G/(B2*G1))*B;
                Vector dpi_drk = (H_G/(B2*G1))*B - (F_G/(A2*G1))*A - (G1/B2)*B;
                Vector dpi_drl = (G1/B2)*B;
                Vector dpi_dr = new Vector(dpi_dri, dpi_drj, dpi_drk, dpi_drl);

                Matrix Hspr = K * dpi_dr.AlterDotProd();

                return Hspr;
            }
        }
    }
}
