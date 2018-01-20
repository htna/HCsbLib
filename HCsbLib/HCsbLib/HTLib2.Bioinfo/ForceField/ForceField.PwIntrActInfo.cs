using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public struct PwIntrActInfo
        {
            public double Kij; // Kij
            public double Fij; // Fij
            public PwIntrActInfo(double K=0, double F=0) { this.Kij = K; this.Fij = F; }
            public static PwIntrActInfo operator+(PwIntrActInfo l, PwIntrActInfo r)
            {
                return new PwIntrActInfo(l.Kij+r.Kij, l.Fij+r.Fij);
            }
        }
        public interface IHessBuilder4PwIntrAct { void BuildHess4PwIntrAct(Universe.AtomPack info, Vector[] coords, out Pair<int, int>[] pwidxs, out PwIntrActInfo[] pwintractinfos); }

        public static void GetForceVector(Vector coord1, Vector coord2, double Fij, out Vector frc1, out Vector frc2)
        {
            Vector v12 = (coord2 - coord1).UnitVector();
            //        a1 ---- a2
            //  (+)    +-->
            //  (-) <--+
            frc1 = (Fij *  v12);
            frc2 = (Fij * -v12);
        }
        public static double GetHessianBlock_c
        {
            get { HDebug.Assert(false); return 1; }
            set { HDebug.Assert(false); }
        }
        public static MatrixByArr GetHessianBlock(Vector coord1, Vector coord2, double Kij, double Fij)
        {
            double dx = coord2[0] - coord1[0];
            double dy = coord2[1] - coord1[1];
            double dz = coord2[2] - coord1[2];
            double dist2 = (dx*dx + dy*dy + dz*dz);
            double dist  = Math.Sqrt(dist2);
            MatrixByArr Htilde = new double[3, 3]{{ dx*-dx, dx*-dy, dx*-dz },
                                             { dy*-dx, dy*-dy, dy*-dz },
                                             { dz*-dx, dz*-dy, dz*-dz }};
            Htilde = Htilde / dist2;
            MatrixByArr I3 = LinAlg.Eye(3);
            // hess_ij = Kij * HTilde[i, j] - c * f_r * (I3 + HTilde[i, j]);
            double c = 1;// GetHessianBlock_c;
            MatrixByArr Hij  = new double[3, 3];
                   Hij += Kij * Htilde;
                   Hij -= (c * (Fij/dist) * (I3 + Htilde));
            return Hij;
        }
    }
}
