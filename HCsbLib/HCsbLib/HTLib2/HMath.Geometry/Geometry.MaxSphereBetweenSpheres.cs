using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        static bool MaxSphereBetweenSpheres_SelfTest = HDebug.IsDebuggerAttached;
        public static Tuple<double,Vector> MaxSphereBetweenSpheres(Vector pt1, double rad1,
                                                                   Vector pt2, double rad2,
                                                                   Vector pt3, double rad3,
                                                                   Vector pt4, double rad4)
        {
            if(MaxSphereBetweenSpheres_SelfTest)
            {
                MaxSphereBetweenSpheres_SelfTest = false;
                Vector tpo = new double[] { 0+1, 0+2, 0+3 }; double tro = 0.10;
                Vector tpa = new double[] { 1+1, 2+2, 3+3 }; double tra = 0.20;
                Vector tpb = new double[] { 5+1, 3+2, 2+3 }; double trb = 0.30;
                Vector tpc = new double[] { 3+1, 3+2, 3+3 }; double trc = 0.15;

                var test_rad_cent = MaxSphereBetweenSpheres(tpo, tro,
                                                            tpa, tra,
                                                            tpb, trb,
                                                            tpc, trc
                                                            );
                double radius = test_rad_cent.Item1;
                Vector center = test_rad_cent.Item2;
                HDebug.AssertTolerance(0.00000001, radius-((center - tpo).Dist - tro));
                HDebug.AssertTolerance(0.00000001, radius-((center - tpa).Dist - tra));
                HDebug.AssertTolerance(0.00000001, radius-((center - tpb).Dist - trb));
                HDebug.AssertTolerance(0.00000001, radius-((center - tpc).Dist - trc));
            }

            Vector po = pt1-pt1; double po2=po.Dist2; double ro = rad1; double ro2=ro*ro;
            Vector pa = pt2-pt1; double pa2=pa.Dist2; double ra = rad2; double ra2=ra*ra;
            Vector pb = pt3-pt1; double pb2=pb.Dist2; double rb = rad3; double rb2=rb*rb;
            Vector pc = pt4-pt1; double pc2=pc.Dist2; double rc = rad4; double rc2=rc*rc;

            double alpah_ab = (ro2 - ra2 + pa2)*(rb - ro) - (ro2 - rb2 + pb2)*(ra - ro);
            double alpah_bc = (ro2 - rb2 + pb2)*(rc - ro) - (ro2 - rc2 + pc2)*(rb - ro);
            Vector beta_ab  = (pa2 - (ro-ra)*(ro-ra))*pb  - (pb2 - (ro-rb)*(ro-rb))*pa;
            Vector beta_bc  = (pb2 - (ro-rb)*(ro-rb))*pc  - (pc2 - (ro-rc)*(ro-rc))*pb;

            double n_alpah_ab = alpah_ab / beta_ab.Dist; Vector n_beta_ab = beta_ab.UnitVector();
            double n_alpah_bc = alpah_bc / beta_bc.Dist; Vector n_beta_bc = beta_bc.UnitVector();

            var line_pt_vec = Geometry.LineOnTwoPlanes(n_beta_ab, n_alpah_ab,
                                                       n_beta_bc, n_alpah_bc);
            Vector gamma = line_pt_vec.Item1;
            Vector delta = line_pt_vec.Item2;

            double a = delta.Dist2;
            double b = 2 * LinAlg.VtV(gamma, delta);
            double c = gamma.Dist2 - 1;

            double[] roots = HRoots.GetRootsClosedFormDegree2(a, b, c);

            foreach(double root in roots)
            {
                Vector q = gamma + root * delta;
                double pa_q = LinAlg.VtV(pa, q);
                double radius = (ro2 - ra2 + pa2 - 2*ro*pa_q) / (2*(ra - ro + pa_q));
                if(radius < 0)
                    continue;
                Vector center = (ro + radius) * q;

                HDebug.AssertTolerance(0.00000001, radius-((center - po).Dist - ro));
                HDebug.AssertTolerance(0.00000001, radius-((center - pa).Dist - ra));
                HDebug.AssertTolerance(0.00000001, radius-((center - pb).Dist - rb));
                HDebug.AssertTolerance(0.00000001, radius-((center - pc).Dist - rc));

                HDebug.AssertTolerance(0.00000001, radius-((center+pt1 - pt1).Dist - rad1));
                HDebug.AssertTolerance(0.00000001, radius-((center+pt1 - pt2).Dist - rad2));
                HDebug.AssertTolerance(0.00000001, radius-((center+pt1 - pt3).Dist - rad3));
                HDebug.AssertTolerance(0.00000001, radius-((center+pt1 - pt4).Dist - rad4));

                return new Tuple<double, Vector>(radius, center+pt1);
            }
            HDebug.Assert(roots == null);
            return null;
        }
	}
}
