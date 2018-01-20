using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static double DerivativeOfTriangleRadius(Vector p1, Vector p2, Vector p3, Vector dp1, Vector dp2, Vector dp3)
        {
            /// r = |p1-p2|*|p2-p3|*|p3-p1| / (2 area(p1,p2,p3))
            ///   = |p1-p2|*|p2-p3|*|p3-p1| / (2 |(p1-p2)x(p2-p3)|)
            /// r2 = ( |p1-p2|*|p2-p3|*|p3-p1| / (2 area(p1,p2,p3)) )^2
            ///    = |p1-p2|^2 * |p2-p3|^2 * |p3-p1|^2 / (4 |(p1-p2)x(p2-p3)|^2)
            /// (r+dr*t)  = R  = |(p1+dp1*t)-(p2+dp2*t)|*|(p2+dp2*t)-(p3+dp3*t)|*|(p3+dp3*t)-(p1+dp1*t)| / (2 area(p1+dp1*t,p2+dp2*t,p3+dp3*t))
            /// (r+dr*t)2 = R2 = { |(p1+dp1*t)-(p2+dp2*t)|*|(p2+dp2*t)-(p3+dp3*t)|*|(p3+dp3*t)-(p1+dp1*t)| / (2 area(p1+dp1*t,p2+dp2*t,p3+dp3*t)) }^2
            ///   
            /// d(r+dr*t)/dt = dR_dt
            ///              = d(r+dr*t)/d((r+dr*t)^2) * d((r+dr*t)^2)/dt
            ///              = dR_dR2 * dR2_dt
            /// d(r+dr*t)/d((r+dr*t)^2) = dR_dR2
            ///                         = 1/(2*r)
            /// d((r+dr*t)^2)/dt = dR2_dt
            ///                = + (   Dotp31dp31 * P12dist2 * P23dist2
            ///                      + Dotp23dp31 * P12dist2 * P31dist2
            ///                      + Dotp12dp12 * P23dist2 * P31dist2
            ///                    )/(2 * Area2)
            ///                  - ( DotCroP12P23CroP12Dp23CroDp12P23 * P12dist2 * P23dist2 * P31dist2)/(2 * Area4)
            ///
            /// P12dist2 := ((p1x-p2x)^2+(p1y-p2y)^2+(p1z-p2z)^2) = (p1-p2).dist2
            /// P23dist2 := ((p2x-p3x)^2+(p2y-p3y)^2+(p2z-p3z)^2) = (p2-p3).dist2
            /// P31dist2 := ((p1x-p3x)^2+(p1y-p3y)^2+(p1z-p3z)^2) = (p1-p3).dist2
            /// Dotp12dp12 := ((dp1x-dp2x)*(p1x-p2x) + (dp1y-dp2y)*(p1y-p2y) + (dp1z-dp2z)*(p1z-p2z))
            ///             = [dp1x-dp2x, dp1y-dp2y, dp1z-dp2z] . [p1x-p2x, p1y-p2y, p1z-p2z]
            ///             = (dp1-dp2).(p1-p2)
            /// Dotp23dp31 := ((dp2x-dp3x)*(p2x-p3x) + (dp2y-dp3y)*(p2y-p3y) + (dp2z-dp3z)*(p2z-p3z))
            ///             = (dp2-dp3).(p2-p3)
            /// Dotp31dp31 := ((dp1x-dp3x)*(p1x-p3x) + (dp1y-dp3y)*(p1y-p3y) + (dp1z-dp3z)*(p1z-p3z))
            ///             = (dp1-dp3).(p1-p3)
            /// DotCroP12P23CroP12Dp23CroDp12P23 := Dot[Cross[p1-p2,p2-p3],(Cross[p1-p2,dp2-dp3]+Cross[dp1-dp2,p2-p3])]
            ///                                   = [(p1-p2)x(p2-p3)] . [(p1-p2)x(dp2-dp3) + (dp1-dp2)x(p2-p3)]
                
            if(HDebug.Selftest())
                #region selftest
            {
                Vector lp1 = new double[] { 1, 0, 0 };
                Vector lp2 = new double[] { 0, 1, 0 };
                Vector lp3 = new double[] { 0, 0, 1 };
                Vector ldp1 = new double[] { 0.1, 0.1, 0.1 };
                Vector ldp2 = new double[] { 1, 2, 3 };
                Vector ldp3 = new double[] { -1, 0, -0.1 };

                double r0 = Geometry.RadiusOfTriangle(lp1, lp2, lp3);
                double dr0 = DerivativeOfTriangleRadius(lp1, lp2, lp3, ldp1, ldp2, ldp3);
                List<Tuple<double, double, double, double>> drx = new List<Tuple<double, double, double, double>>();
                foreach(double dt in new double[] { 0.1, 0.01, 0.001, 0.0001, 0.00001, 0.000001, 0.0000001 })
                {
                    double t00 = -dt; double r00 = Geometry.RadiusOfTriangle(lp1+t00*ldp1, lp2+t00*ldp2, lp3+t00*ldp3);
                    double t01 = +dt; double r01 = Geometry.RadiusOfTriangle(lp1+t01*ldp1, lp2+t01*ldp2, lp3+t01*ldp3);
                    double dr1 = (r01    -r00)/(t01-t00);
                    double dr2 = (r01*r01-r00*r00)/(t01-t00);

                    drx.Add(new Tuple<double, double, double, double>(dt, dr1, dr2, (0.5/r0)*dr2));
                }
                double diff = dr0-drx.Last().Item2;
                HDebug.AssertTolerance(0.00000001, diff);
            }
                #endregion

            double P12dist2 = (p1-p2).Dist2;
            double P23dist2 = (p2-p3).Dist2;
            double P31dist2 = (p1-p3).Dist2;
            double Dotp12dp12 = LinAlg.VtV(dp1-dp2, p1-p2);
            double Dotp23dp31 = LinAlg.VtV(dp2-dp3, p2-p3);
            double Dotp31dp31 = LinAlg.VtV(dp1-dp3, p1-p3);
            double DotCroP12P23CroP12Dp23CroDp12P23 = LinAlg.VtV( LinAlg.CrossProd(p1-p2,  p2- p3)
                                                                , LinAlg.CrossProd(p1-p2, dp2-dp3)+LinAlg.CrossProd(dp1-dp2, p2-p3)
                                                                );
            double Area2 = LinAlg.CrossProd(p1-p2, p2-p3).Dist2;
            double Area4 = Area2*Area2;
            double Rad2 = P12dist2 * P23dist2 * P31dist2 / (4*Area2);
            double Rad  = Math.Sqrt(Rad2);
            HDebug.AssertTolerance(0.00000001, Rad-Geometry.RadiusOfTriangle(p1, p2, p3));

            double t = 1;
            double dR_dR2 = 0.5/Rad;
            double dR2_dt = ((t*Dotp31dp31)*P12dist2*P23dist2 + (t*Dotp23dp31)*P12dist2*P31dist2 + (t*Dotp12dp12)*P23dist2*P31dist2)/(2 * Area2)
                            - ((t*DotCroP12P23CroP12Dp23CroDp12P23) * P12dist2 * P23dist2 * P31dist2)/(2 * Area4);
            double dR_dt = dR_dR2 * dR2_dt;
            return dR_dt;
        }
    }
}
