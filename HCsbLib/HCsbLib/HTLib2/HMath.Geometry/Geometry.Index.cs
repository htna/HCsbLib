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
//		public static double ClosestIndexFromLine(DoubleVector3 point, DoubleVector3 LinePt0, DoubleVector3 LinePt1)
//		{
//			//     Q (query)
//			//    /|\
//			//   / | \
//			//  /  |  \
//			// /---+---\
//			// A   T   B
//			// 
//			// AQ . AB = |AQ| * |AB| * cos(QAB)
//			// cos(QAB) = |AT| / |AQ|
//			// |AT| / |AB| = AQ . AB / |AB|^2
//			DoubleVector3 P = point;
//			DoubleVector3 A = LinePt0;
//			DoubleVector3 B = LinePt1;
//
//			DoubleVector3 AP = P - A;
//			DoubleVector3 AB = B - A;
//			double ab2   = AB.Length2;
//			if(ab2 == 0)
//				return 0;
//			double ap_ab = DoubleVector3.InnerProduct(AP, AB);
//			double t = ap_ab / ab2;
//			return t;
//		}
//		public static double IndexSegmentClosestToLine(Segment segment, Line line)
//		{
//			// 1. find the plane containing line and point which is the closest on the segment
//			DoubleVector3 orthogonal_line_segment = DoubleVector3.CrossProduct(segment.Direct, line.Normal);
//			DoubleVector3 normal_plane = DoubleVector3.CrossProduct(orthogonal_line_segment, line.Normal).UnitVector;
//			Plane plane = new Plane(line.Base, normal_plane);
//			// 2. fine index of segment intersecting the plane
//			double index = IndexSegmentIntersectingPlane(segment, plane);
//			//Debug.Assert(DoubleVector3.InnerProduct(segment[index]-))
//			if(Debug.IsDebuggerAttached)
//			{
//				double dist_pt2line = DistancePointLine(segment[index],line);
//				double dist_line2line = DistanceLineLine(line, segment.ToLine());
//				Debug.AssertSimilar(dist_pt2line, dist_line2line, 0.000000001);
//			}
//			return index;
//		}
//		public static double[] IndexSegmentToLineByDistance(Segment segment, Line line, double distance)
//		{
//			double dist_lineline = DistanceLineLine(line, segment.ToLine());
//			if(dist_lineline > distance)
//				return null;
//			double index = IndexSegmentClosestToLine(segment, line);
//			if(dist_lineline == distance)
//				return new double[1] { index };
//			double dist_more = Math.Sqrt(distance*distance + dist_lineline*dist_lineline);
//			double angle_segment_line = DoubleVector3.AngleBetween(segment.Direct, line.Normal).Radian;
//			double index_dist = (dist_more / Math.Atan(angle_segment_line)) / segment.Direct.Length;
//			Debug.Assert(DistancePointLine(segment[index+index_dist], line) > distance);
//			Func<double,double> funcdist = delegate(double t) { return DistancePointLine(segment[t], line); };
//			Pair<bool,double> index_dist2 = RootsBisection.Root(funcdist, index, index+index_dist);
//			Debug.Assert(index_dist2.first == true);
//			double[] indexes = new double[2] { index-index_dist2.second, index+index_dist2.second };
//			if(Debug.IsDebuggerAttached)
//			{
//				DoubleVector3 pt1 = segment[indexes[0]];
//				DoubleVector3 pt2 = segment[indexes[1]];
//				Debug.AssertSimilar(distance, DistancePointLine(pt1, line), 0.00000001);
//				Debug.AssertSimilar(distance, DistancePointLine(pt2, line), 0.00000001);
//			}
//			return indexes;
//		}
//		public static double IndexSegmentIntersectingPlane(Segment segment, Plane plane)
//		{
//			// http://softsurfer.com/Archive/algorithm_0104/algorithm_0104B.htm
//			// s = -n.w / n.u
//			double dist = -1 * DoubleVector3.InnerProduct(plane.Normal, segment.Base-plane.Base)
//							 / DoubleVector3.InnerProduct(plane.Normal, segment.Direct);
//			Debug.Assert(plane.DistanceFrom(segment[dist]) < 0.00000001);
//			return dist;
//			//// index of segment which intersecting plane
//			//// if index is in [0,1], the segment intersects plane
//			//// otherwise, it is outside of plane
//			//if(DoubleVector3.InnerProduct(segment.Direct, plane.Normal) == 0)
//			//    return double.NaN;
//			//double dist = DistancePointPlane(segment.Base, plane);
//			//DoubleVector3 point2plane = -1*Math.Sign(dist)*plane.Normal;
//			//double sin = DoubleVector3.CrossProduct(point2plane, segment.Direct.UnitVector).Length;
//			//double cos = DoubleVector3.InnerProduct(point2plane, segment.Direct.UnitVector);
//			//double sign = (cos >=0) ? 1 : -1;
//			//double unitlength = sign * sin * segment.Direct.Length;
//			//return dist/unitlength;
//		}
//		public static double IndexSegmentIntersectingTriangle(Segment segment, DoubleVector3 tri0, DoubleVector3 tri1, DoubleVector3 tri2)
//		{
//			// return [0,1] if the segement intersects triangle
//			// return (-inf,0) or (1,inf) if the line of segment intersects the triangle
//			// return double.NegativeInfinity or double.PositiveInfinity if line does not intersects the triangle
//			double index = IndexSegmentIntersectingPlane(segment, ToPlane(tri0, tri1, tri2));
//			bool ptInTriangle = CheckPointInTriangle(segment[index], tri0, tri1, tri2);
//			if(ptInTriangle == true)
//				return index;
//			if(index >= 0)
//				return double.PositiveInfinity;
//			if(index < 0)
//				return double.NegativeInfinity;
//			return double.NaN;
//		}

		public static double IndexSegmentClosestPoint(Segment segment, Vector point)
		{
			// http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
			// t = - (x1 - x0) . (x2 - x1 ) / |x2-x1|^2
			Vector x1 = segment.PtFrom;
			Vector x2 = segment.PtTo;
			Vector x0 = point;
			double index = -1 * LinAlg.DotProd(x1-x0, x2-x1) / (x2-x1).Dist2;
			if(HDebug.IsDebuggerAttached)
			{
				// check orthogonal
				Vector ortho = (point - segment[index]);
                HDebug.Assert(LinAlg.DotProd(ortho, segment.Direct) < 0.00000001);
				// check closestness
				double dist  = (point - segment[index]      ).Dist;
				double dist1 = (point - segment[index-0.001]).Dist;
				double dist2 = (point - segment[index+0.001]).Dist;
				HDebug.Assert(dist <= dist1);
				HDebug.Assert(dist <= dist2);
			}
			return index;
		}
        public static double IndexSegmentsClosestPoint(IList<Vector> segments, Vector point)
        {
            double index = 0;
            double dist2 = (point - segments[0]).Dist2;
            for(int i=0; i<segments.Count-1; i++)
            {
                Segment segment = new Segment(segments[i], segments[i+1]);
                double  lindex = HMath.Between(0, IndexSegmentClosestPoint(segment, point), 1);
                double  ldist2 = (point - segment[lindex]).Dist2;
                if(ldist2 < dist2)
                {
                    index = i+lindex;
                    dist2 = ldist2;
                }
            }
            return index;
        }
    }
}
