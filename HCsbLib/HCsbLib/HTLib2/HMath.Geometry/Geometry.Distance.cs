using System;
using System.Collections.Generic;
using System.Text;

// References
// - http://www.geometryalgorithms.com
// - http://www.gamedev.net/community/forums/topic.asp?topic_id=444154


namespace HTLib2
{
	public partial class Geometry
	{
//        //public static double DistancePointLine(PointF point, PointF line1, PointF line2)
//        //{
//        //    double dist
//        //        = (line2.x - line1.x) * (line1.y - point.y)
//        //        - (line1.x - point.x) * (line2.y - line1.y);
//        //    dist = Math.Abs(dist) / (line2 - line1).Length;
//        //    return dist;
//        //}
        public static bool   DistancePointLine_selftest = HDebug.IsDebuggerAttached;
        public static double DistancePointLine(Vector point, Vector line1, Vector line2)
        {
            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            if(DistancePointLine_selftest)
            {
                DistancePointLine_selftest = false;
                Vector tp  = new Vector(2, 2,   0);
                Vector tl1 = new Vector(0, 0, -20);
                Vector tl2 = new Vector(0, 0, -10);
                double tdist0 = tp.Dist;
                double tdist1 = DistancePointLine(tp, tl1, tl2);
                double tdist2 = (ClosestPointOnLine(tl1, tl2, tp, false) - tp).Dist;
                HDebug.AssertTolerance(0.00000001, tdist0-tdist1);
                HDebug.AssertTolerance(0.00000001, tdist0-tdist2);
            }
            Vector x21 = line2 - line1;
            Vector x10 = line1 - point;
            double dist
                = LinAlg.CrossProd(x21, x10).Dist
                / x21.Dist;
            return dist;
        }
        public static double DistancePointSegment(Vector point, Line line)
		{
            throw new NotImplementedException();

			Segment segment = new Segment(line.Base, line.Normal);
			double index = segment.IndexClosestFrom(point);
			Vector closest = segment[index];
			double dist = (closest-point).Dist;
			//if(Debug.IsDebuggerAttached)
			//{
			//    // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
			//    DoubleVector3 pt1 = line.Base;
			//    DoubleVector3 pt2 = line.Base + line.Normal;
			//    DoubleVector3 x21 = pt2 - pt1;
			//    DoubleVector3 x10 = pt1 - point;
			//    double dist2
			//        = DoubleVector3.CrossProduct(x21, x10).Length
			//        / x21.Length;
			//    Debug.AssertSimilar(dist2, dist, 0.00000001);
			//}
			return dist;
		}
//		public static double DistancePointLineSegment(PointF point, PointF line1, PointF line2)
//		{
//			double distPointLine = DistancePointLine(point, line1, line2);
//			double distPointLine1 = (line2 - point).Length;
//			double distPointLine2 = (line1 - point).Length;
//			return HMath.Mid(distPointLine, distPointLine1, distPointLine2);
//		}
		public static double DistancePointPlane(Vector pt, Plane plane)
		{
			// Return the signed distance.
			// If the point is in the normal direction, its sign will be plus
			// , otherwise (pt is in opposite of normal direction), sign will be negative.
			//
			// http://mathworld.wolfram.com/Point-PlaneDistance.html
			// point: (X,Y,Z)
			// plane: 0 = ax + by + cz + d
			//          = a(x-x0) + b(y-y0) + c(z-z0)
			//          = (a,b,c) . ((x,y,z) - (x0,y0,z0))
			// dist = (ax0 + by0 + cz0 + d) / Sqrt(a*a + b*b + c*c)
			//      = (normal . (pt - base)) / abs(normal)
			//      = (normal . (pt - base))
			//              because abs(normal) = 1
			double dist = (LinAlg.DotProd(plane.Normal, pt-plane.Base));
			//dist = Math.Abs(dist);
			return dist;
		}
//		public static double DistanceLineLine(Line line1, Line line2)
//		{
//			// http://mathworld.wolfram.com/Line-LineDistance.html
//			DoubleVector3 x1 = line1.Base;
//			DoubleVector3 x2 = line1.Normal;
//			DoubleVector3 x3 = line2.Base;
//			DoubleVector3 x4 = line2.Normal;
//			DoubleVector3 a = x2 - x1;
//			DoubleVector3 b = x4 - x3;
//			DoubleVector3 c = x3 - x1;
//			DoubleVector3 axb = DoubleVector3.CrossProduct(a, b);
//			double dist = Math.Abs(DoubleVector3.InnerProduct(c, axb)) / axb.Length;
//			return dist;
//		}
        public static double AngleBetween(Vector left, Vector right)
        {
            double cos_angle = LinAlg.DotProd(left, right) / (left.Dist * right.Dist);
                   cos_angle = HMath.Between(-1, cos_angle, 1);
            double angle = Math.Acos(cos_angle);
            while(angle >  Math.PI) angle -= Math.PI;
            while(angle < -Math.PI) angle += Math.PI;
            HDebug.Assert(angle >= 0);
            HDebug.Assert(angle <= Math.PI);
            return angle;
        }
        public static bool AngleBetween_selftest2 = HDebug.IsDebuggerAttached;
        public static double AngleBetween(Vector a, Vector b, Vector c)
        {
            if(AngleBetween_selftest2)
            {
                AngleBetween_selftest2 = false;
                Vector ta, tb, tc;
                double tang;
                ta = new double[] { 1, 0, 0 };
                tb = new double[] { 0, 0, 0 };
                tc = new double[] { 0, 1, 0 };
                tang = AngleBetween(ta, tb, tc);
                HDebug.AssertTolerance(0.0001, Math.PI/2 - tang);
                ta = new double[] { 0, 1, 0 };
                tb = new double[] { 0, 0, 0 };
                tc = new double[] { 0, 1, 1 };
                tang = AngleBetween(ta, tb, tc);
                HDebug.AssertTolerance(0.0001, Math.PI/4 - tang);
            }
            Vector left  = a-b;
            Vector right = c-b;
            double ang = AngleBetween(left, right);
            HDebug.Assert(ang >= 0);
            HDebug.Assert(ang <= Math.PI);
            return ang;
        }
    }
}
