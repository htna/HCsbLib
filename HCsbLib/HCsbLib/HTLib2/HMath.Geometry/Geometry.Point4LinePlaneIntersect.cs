using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Vector Point4LinePlaneIntersect(Vector plane1, Vector plane2, Vector plane3, Vector line1, Vector line2)
        {
            Vector planeNormal = Geometry.PlaneNormal(plane1, plane2, plane3);
            Vector lineDire    = line1 - line2;
            return Point4LinePlaneIntersect(planeNormal, plane1, lineDire, line1);

            //double x1 = plane1[0], y1 = plane1[1], z1 = plane1[2];
            //double x2 = plane2[0], y2 = plane2[1], z2 = plane2[2];
            //double x3 = plane3[0], y3 = plane3[1], z3 = plane3[2];
            //double x4 =  line1[0], y4 =  line1[1], z4 =  line1[2];
            //double x5 =  line2[0], y5 =  line2[1], z5 =  line1[2];
            //double t = -1 * Matrix.Det(new double[4,4]{{ 1,  1,  1,  1}
            //                                          ,{x1, x2, x3, x4}
            //                                          ,{y1, y2, y3, y4}
            //                                          ,{z1, z2, z3, z4}}).Value
            //              / Matrix.Det(new double[4,4]{{ 1,  1,  1,     0}
            //                                          ,{x1, x2, x3, x5-x4}
            //                                          ,{y1, y2, y3, y5-y4}
            //                                          ,{z1, z2, z3, z5-z4}}).Value;
            //double x = x4 + (x5-x4)*t;
            //double y = y4 + (y5-y4)*t;
            //double z = z4 + (z5-z4)*t;
            //Vector point = new double[] { x, y, z };
            //return point;
        }
        static bool Point4LinePlaneIntersect_SelfTest = HDebug.IsDebuggerAttached;
        public static Vector Point4LinePlaneIntersect(Vector planeNormal, Vector planePoint, Vector lineDire, Vector linePoint)
        {
            if(Point4LinePlaneIntersect_SelfTest)
            {
                Point4LinePlaneIntersect_SelfTest = false;
                Vector _plnm = new double[] { 1, 0, 0 };
                Vector _plpt = new double[] { 2, 0, 0 };
                Vector _lndr = new double[] { 1, 1, 1 };
                Vector _lnpt = new double[] { 0, 0, 0 };
                Vector _itpt0 = Point4LinePlaneIntersect(_plnm, _plpt, _lndr, _lnpt);
                Vector _itpt1 = new double[] { 2, 2, 2 };
                HDebug.AssertTolerance(0.000001, _itpt0-_itpt1);
            }

            /// http://en.wikipedia.org/wiki/Line-plane_intersection
            /// Algebraic form
            /// plane : (pt - p0).n = 0
            /// line  :  pt = d l + l0
            ///
            /// point-line intersection: d = ((p0 - l0) . n) / (l . n)
            ///                          pt = d * l + l0
            Vector n = planeNormal;
            Vector p0 = planePoint;
            Vector l = lineDire;
            Vector l0 = linePoint;
            double d = LinAlg.DotProd(p0-l0, n) / LinAlg.DotProd(l, n);
            Vector pt = d * l + l0;
            HDebug.AssertTolerance(0.00000001, LinAlg.DotProd(pt-p0, n));
            HDebug.AssertTolerance(0.00000001, pt - (d*l + l0));
            return pt;
        }
        public static Vector Point4IntersectSegmentPlane(Vector plane1, Vector plane2, Vector plane3, Vector segment1, Vector segment2, double tolerance=0.00001)
        {
            /// return intersection point between plane (by p1, p2, p3) and line semgnet (seg1, seg2).
            /// If the line segment does not pass through the plane, return null

            Vector point = Point4LinePlaneIntersect(plane1, plane2, plane3, segment1, segment2);

            Vector vec12  = (segment2 - segment1);
            Vector uvec12 = vec12.UnitVector();
            double leng12 = vec12.Dist;
            Vector vec1p  = (point - segment1);
            double t = LinAlg.VtV(uvec12, vec1p);

            tolerance = Math.Abs(tolerance);
            if(t <       -tolerance) point = null; // smaller than 0
            if(t > leng12+tolerance) point = null; // larger  than leng12
            return point;
        }
        static bool CheckPointOnLine_SelfTest = HDebug.IsDebuggerAttached;
        public static bool CheckPointOnLine(Vector point, Vector line1, Vector line2, double toleranceAlign=0.001)
        {
            if(CheckPointOnLine_SelfTest)
            {
                CheckPointOnLine_SelfTest = false;
                HDebug.Assert(CheckPointOnLine(new double[] { 2, 2, 2 }, new double[] { 0, 0, 0 }, new double[] { 1, 1, 1 }) == true );
                HDebug.Assert(CheckPointOnLine(new double[] { 1, 2, 2 }, new double[] { 0, 0, 0 }, new double[] { 1, 1, 1 }) == false);
            }
            Vector uvec1p = (point - line1).UnitVector();
            Vector uvec12 = (line2 - line1).UnitVector();
            double dot = Math.Abs(LinAlg.VtV(uvec1p, uvec12));
            if(dot < 1.0-toleranceAlign)
                return false;
            return true;
        }
        static bool CheckPointOnSegment_SelfTest = HDebug.IsDebuggerAttached;
        public static bool CheckPointOnSegment(Vector point, Vector segment1, Vector segment2, double toleranceAlign=0.001, double toleranceWithin=0.000001)
        {
            if(CheckPointOnSegment_SelfTest)
            {
                CheckPointOnSegment_SelfTest = false;
                HDebug.Assert(CheckPointOnSegment(new double[] { 2, 2, 2 }, new double[] { 1, 1, 1 }, new double[] { 3, 3, 3 }) == true );
                HDebug.Assert(CheckPointOnSegment(new double[] { 0, 0, 0 }, new double[] { 1, 1, 1 }, new double[] { 3, 3, 3 }) == false);
                HDebug.Assert(CheckPointOnSegment(new double[] { 4, 4, 4 }, new double[] { 1, 1, 1 }, new double[] { 3, 3, 3 }) == false);
                HDebug.Assert(CheckPointOnSegment(new double[] { 1, 2, 2 }, new double[] { 1, 1, 1 }, new double[] { 3, 3, 3 }) == false);
            }
            Vector vec1p = (point    - segment1); Vector uvec1p = vec1p.UnitVector();
            Vector vec12 = (segment2 - segment1); Vector uvec12 = vec12.UnitVector();

            double dot = Math.Abs(LinAlg.VtV(uvec1p, uvec12));
            if(dot < 1.0-toleranceAlign)
                return false; // not aligned

            double leng12 = vec12.Dist;
            double t = LinAlg.VtV(vec1p, uvec12);
            if(t <       -toleranceWithin) return false; // smaller than 0
            if(t > leng12+toleranceWithin) return false; // larger  than leng12
            return true;
        }
        public static Vector Point4IntersectLineCircle(Vector circle1, Vector circle2, Vector circle3, Vector line1, Vector line2, double tolerance=0.00001)
        {
            /// return intersection point between circle (by c1, c2, c3) and line (by line1, line2).
            /// If the line does not pass through the plane, return null
            Vector point = Point4LinePlaneIntersect(circle1, circle2, circle3, line1, line2);

            double a, b, c, A, B, C, radius;
            Vector center;
            Geometry.Triangle.GetTriGeom(circle1, circle2, circle3,
                                         out a, out b, out c,
                                         out A, out B, out C,
                                         out radius,
                                         out center
                                        );

            double dist = (center - point).Dist;
            HDebug.Assert(tolerance >= 0);
            if(dist > radius-tolerance)
                point = null;

            return point;
        }
        public static Vector Point4IntersectSegmentCircle(Vector circle1, Vector circle2, Vector circle3, Vector segment1, Vector segment2, double tolerance=0.00001)
        {
            /// return intersection point between circle (by c1, c2, c3) and segment (by seg1, seg2).
            /// If the line segment does not pass through the plane, return null
            Vector point = Point4IntersectLineCircle(circle1, circle2, circle3, segment1, segment2);
            if(point == null)
                return null;

            if(CheckPointOnSegment(point, segment1, segment2) == false)
                return null;

            return point;
        }
    }
}
