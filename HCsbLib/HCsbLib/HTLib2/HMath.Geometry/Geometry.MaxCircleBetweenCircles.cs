using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Tuple<double,Vector,bool> MaxCircleBetweenCircles(Vector pt1, double rad1,
                                                                        Vector pt2, double rad2,
                                                                        Vector pt3, double rad3)
        {
            ///        b:pt3
            ///        /   \
            ///      /      \
            ///    /         \
            /// o:pt1 ------ a:pt2
            ///
            /// o : (0  ,   0), radii ro
            /// a : (pax,   0), radii ra
            /// b : (pbx, pby), radii rb

            Vector uvec12 = (pt2 - pt1).UnitVector();
            Vector uvec23 = (pt3 - pt2).UnitVector();
            Vector uvec31 = (pt1 - pt3).UnitVector();
            double cos123 = LinAlg.VtV(-uvec12, uvec23);
            double cos231 = LinAlg.VtV(-uvec23, uvec31);
            double cos312 = LinAlg.VtV(-uvec31, uvec12);

            double cos_aob;
            double ro, ra, rb;
            Vector po, pa, pb;
                 if(cos123 > 0) { pa=pt1; ra=rad1; po=pt2; ro=rad2; pb=pt3; rb=rad3; cos_aob = cos123; }
            else if(cos231 > 0) { pa=pt2; ra=rad2; po=pt3; ro=rad3; pb=pt1; rb=rad1; cos_aob = cos231; }
            else if(cos312 > 0) { pa=pt2; ra=rad2; po=pt1; ro=rad1; pb=pt3; rb=rad3; cos_aob = cos312; }
            else throw new Exception("cos123>0 and cos231>0");

            Vector uvec_oa = (pa-po).UnitVector();
            Vector uvec_ob = (pb-po).UnitVector();
            Vector normal  = LinAlg.CrossProd(uvec_oa, uvec_ob);
            double sin_aob = normal.Dist;

            double pax = (pa-po).Dist;
            double pbx = (pb-po).Dist * cos_aob;
            double pby = (pb-po).Dist * sin_aob;
            Trans3 trans;
            {
                Vector npo = new double[] { 0,   0,   0 };
                Vector npa = new double[] { 0, pax,   0 };
                Vector npb = new double[] { 0, pbx, pby };
                trans = Trans3.GetTransformNoScale(npo, npa, npb, po, pa, pb);
                if(HDebug.IsDebuggerAttached)
                {
                    Vector pox = trans.DoTransform(npo);
                    HDebug.AssertTolerance(0.0000001, po-trans.DoTransform(npo));
                    HDebug.AssertTolerance(0.0000001, pa-trans.DoTransform(npa));
                    HDebug.AssertTolerance(0.0000001, pb-trans.DoTransform(npb));
                }
            }

            Tuple<double,Vector,bool> rad_cent_in = MaxCircleBetweenCircles(ro, ra, rb, pax, pbx, pby);
            if(rad_cent_in == null)
                return null;
            double radius = rad_cent_in.Item1;
            bool cenerInTriangle = rad_cent_in.Item3;

            Vector center;
            {
                HDebug.Assert(rad_cent_in.Item2.Size == 2);
                center = new double[] { 0, rad_cent_in.Item2[0], rad_cent_in.Item2[1] };
                center = trans.DoTransform(center);
            }
            if(HDebug.IsDebuggerAttached)
            {
                double radx_1 = (center-pt1).Dist - rad1;
                double radx_2 = (center-pt2).Dist - rad2;
                double radx_3 = (center-pt3).Dist - rad3;
                HDebug.AssertTolerance(0.000001, radx_1-radx_2, radx_2-radx_3, radx_3-radx_1);
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(normal, pt1-pt2));
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(normal, pt2-pt3));
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(normal, pt3-pt1));
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(normal, pt1-center));
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(normal, pt2-center));
                HDebug.AssertTolerance(0.00000001, LinAlg.VtV(normal, pt3-center));
            }

            return new Tuple<double, Vector, bool>(radius, center, cenerInTriangle);
        }
        public static Tuple<double, Vector, bool> MaxCircleBetweenCircles(double ro, double ra, double rb,
                                                                          double pax, double pbx, double pby)
        {
            ///         b
            ///       /   \
            ///     /      \
            ///   /         \
            /// o ---------- a
            ///
            /// o : (0  ,   0), radii ro
            /// a : (pax,   0), radii ra
            /// b : (pbx, pby), radii rb
            HDebug.Assert(ro > 0, ra > 0, rb > 0);
            HDebug.Assert(pax > 0, pbx > 0, pby > 0);

            double ro2 = ro*ro;
            double ra2 = ra*ra;
            double rb2 = rb*rb;
            Vector po = new double[] {   0,   0 };
            Vector pa = new double[] { pax,   0 };
            Vector pb = new double[] { pbx, pby };

            double maxAngInTriangle = Math.Acos(pbx/pb.Dist);

            double alpha = (ro2 - ra2 + pa.Dist2)*(rb - ro)
                         - (ro2 - rb2 + pb.Dist2)*(ra - ro);
            Vector beta = (pa.Dist2 - (ro-ra)*(ro-ra)) * pb
                        - (pb.Dist2 - (ro-rb)*(ro-rb)) * pa;

            double[] initangs;
            {
                double aa = beta.Dist2;
                double bb = 2*alpha*beta[0];
                double cc = alpha - beta[1]*beta[1];
                double[] coss = HRoots.GetRootsClosedFormDegree2(aa, bb, cc);
                
                initangs = new double[0];
                if(coss != null)
                {
                    initangs = initangs.HAddRange
                        (
                            Math.Acos(HMath.Between(-1, coss[0], 1)),
                            Math.Acos(HMath.Between(-1, coss[1], 1))
                        ).ToArray();
                }

                initangs = initangs.HAddRange
                    (
                        0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, // Math.PI*2 = 6.2831853071795862
                        0.5, 1.5, 2.5, 3.5, 4.5, 5.5
                    ).ToArray();
            }

            Func<double,double> func = delegate(double lang)
            {
                lang = lang % Math.PI;
                double lcos = Math.Cos(lang);
                double lsin = Math.Sin(lang);
                //double lsin;
                //if(0<lcos && lcos<1) lsin = Math.Sqrt(1- lcos*lcos);
                //else                 lsin = 1-lcos;
                double val = alpha + beta[0]*lcos + beta[1]*lsin;
                return val;
            };

            foreach(double initang in initangs)
            {
                double? ang = HRoots.GetRootSecant(func, initang, initang+0.01, 100); /// scant method works better than bisection !!
                if(ang == null)
                    continue;
                ang = ang % Math.PI;

                bool cenerInTriangle = ((0 <= ang) && (ang <= maxAngInTriangle));

                double cos = Math.Cos(ang.Value);
                double sin = Math.Sin(ang.Value);
                double radius_oa = (ro2 - ra2 + pa.Dist2 - 2*ro*(pax*cos))/(2*(ra - ro + (pax*cos)));
                double radius_ob = (ro2 - rb2 + pb.Dist2 - 2*ro*(pbx*cos+pby*sin))/(2*(rb - ro + (pbx*cos+pby*sin)));
                //HDebug.AssertTolerance(0.00000001, radius_oa - radius_ob);

                double radius = radius_oa;
                Vector center = (ro+radius) * new Vector(cos, sin);
                double radius2o = (po - center).Dist - ro; double err2o = Math.Abs(radius - radius2o);
                double radius2a = (pa - center).Dist - ra; double err2a = Math.Abs(radius - radius2a);
                double radius2b = (pb - center).Dist - rb; double err2b = Math.Abs(radius - radius2b);
                
                double toler_err = Math.Abs(radius)*0.000001;
                if((err2o < toler_err) && (err2a < toler_err) && (err2b < toler_err))
                    return new Tuple<double, Vector, bool>(radius, center, cenerInTriangle);
            }

            /// There is a case that three circles are positioned almost in a line, whose circles are all overlap
            /// and the middle circle has the largest radius. (like, oOo)
            /// In that case, the common largest circle cannot be found.
            return null;

            //double ang;
            //{
            //    double val0 = func(initangs[0]);
            //    double val1 = func(initangs[1]);
            //    ang = (Math.Abs(val0) < Math.Abs(val1)) ? initangs[0] : initangs[1];
            //    ang = HRoots.GetRootSecant(func, ang, ang*1.01, 100); /// scant method works better than bisection !!
            //    //cos = HRoots.GetRootBisection(func, 0, tmax).Value;
            //    ang = ang % Math.PI;
            //}
            //
            //Vector center;
            //{
            //    double cos = Math.Cos(ang);
            //    double sin = Math.Sin(ang);
            //    double radius_oa = (ro2 - ra2 + pa.Dist2 - 2*ro*(pax*cos        ))/(2*(ra - ro + (pax*cos        )));
            //    double radius_ob = (ro2 - rb2 + pb.Dist2 - 2*ro*(pbx*cos+pby*sin))/(2*(rb - ro + (pbx*cos+pby*sin)));
            //    HDebug.AssertTolerance(0.00000001, radius_oa - radius_ob);
            //
            //    center = (ro+radius_oa) * new Vector(cos, sin);
            //}
            //
            //{
            //    double radius2o = (po - center).Dist - ro;
            //    radius = radius2o;
            //
            //    if(HDebug.IsDebuggerAttached)
            //    {
            //        double radius2a = (pa - center).Dist - ra;
            //        double radius2b = (pb - center).Dist - rb;
            //        HDebug.AssertTolerance(0.00000001, radius-radius2o);
            //        HDebug.AssertTolerance(0.00000001, radius-radius2a);
            //        HDebug.AssertTolerance(0.00000001, radius-radius2b);
            //    }
            //}
            //
            //return new Tuple<double, Vector>(radius, center);

            //{
            //    double a = 2;
            //    double b = 3;
            //    double c = 5;
            //    double ro  = 0.5; double ro2 = ro*ro;
            //    double ra  = 0.3; double ra2 = ra*ra;
            //    double rbc = 1.0; double rbc2 = rbc*rbc;
            //    Vector po = new double[] { 0, 0 };
            //    Vector pa = new double[] { a, 0 };
            //    Vector pbc = new double[] { b, c };
            //
            //    double alpha = (ro2 - ra2  + pa.Dist2)*(rbc - ro)
            //                 - (ro2 - rbc2 + pbc.Dist2)*(ra  - ro);
            //    Vector beta = (pa.Dist2 - (ro-ra)*(ro-ra)) * pbc
            //                - (pbc.Dist2 - (ro-rbc)*(ro-rbc)) * pa;
            //    double aa = beta.Dist2;
            //    double bb = 2*alpha*beta[0];
            //    double cc = alpha - beta[1]*beta[1];
            //    double[] ts = HRoots.GetRootsClosedFormDegree2(aa, bb, cc);
            //    double tmax = b/pbc.Dist;
            //    {
            //        double chk0 = aa*ts[0]*ts[0] + bb*ts[0] + cc;
            //        double chk1 = aa*ts[1]*ts[1] + bb*ts[1] + cc;
            //        Vector q0 = new double[] { ts[0], Math.Sqrt(1 - ts[0]*ts[0]) };
            //        Vector q1 = new double[] { ts[1], Math.Sqrt(1 - ts[1]*ts[1]) };
            //        double chkq0 = alpha + LinAlg.VtV(beta, q0);
            //        double chkq1 = alpha + LinAlg.VtV(beta, q1);
            //    }
            //
            //    Func<double,double> func = delegate(double cos)
            //    {
            //        double sin = Math.Sqrt(1- cos*cos);
            //        double val = alpha + beta[0]*cos + beta[1]*sin;
            //        return val;
            //    };
            //    //Func<double,double> dfunc = delegate(double cos)
            //    //{
            //    //    double sin = Math.Sqrt(1- cos*cos);
            //    //    double dval = -1*beta[0]*sin + beta[1]*cos;
            //    //    return dval;
            //    //};
            //    double tb = HRoots.GetRootBisection(func, 0, tmax).Value;
            //    double tc = HRoots.GetRootSecant(func, 0, tmax, 100);
            //    //double td = HRoots.GetRootNewton(func, dfunc, ts[0]).Value;
            //    tb = tc;
            //    {
            //        double cos = tb;
            //        double sin = Math.Sqrt(1 - tb*tb);
            //        Vector q = new double[] { cos, sin };
            //        double chk = alpha + LinAlg.VtV(beta, q);
            //    }
            //
            //    {
            //        double r_a0 = (ro2 - ra2 + pa.Dist2 - 2*ro*a*ts[0])/(2*(ra - ro + a *ts[0]));
            //        double r_a1 = (ro2 - ra2 + pa.Dist2 - 2*ro*a*ts[1])/(2*(ra - ro + a *ts[1]));
            //        double r_a2 = (ro2 - ra2 + pa.Dist2 - 2*ro*a*tb)/(2*(ra - ro + a *tb));
            //
            //        Vector cent0 = (ro+r_a0) * new Vector(ts[0], Math.Sqrt(1-ts[0]*ts[0]));
            //        double radi0o  = (po  - cent0).Dist - ro;
            //        double radi0a  = (pa  - cent0).Dist - ra;
            //        double radi0bc = (pbc - cent0).Dist - rbc;
            //
            //        Vector cent1 = (ro+r_a1) * new Vector(ts[1], Math.Sqrt(1-ts[1]*ts[1]));
            //        double radi1o  = (po  - cent1).Dist - ro;
            //        double radi1a  = (pa  - cent1).Dist - ra;
            //        double radi1bc = (pbc - cent1).Dist - rbc;
            //
            //        Vector cent2 = (ro+r_a2) * new Vector(tb, Math.Sqrt(1-tb*tb));
            //        double radi2o  = (po  - cent2).Dist - ro;
            //        double radi2a  = (pa  - cent2).Dist - ra;
            //        double radi2bc = (pbc - cent2).Dist - rbc;
            //    }
            //}
        }
	}
}
