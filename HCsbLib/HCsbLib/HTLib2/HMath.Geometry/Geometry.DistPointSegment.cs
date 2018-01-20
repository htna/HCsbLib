using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        static double Dist2PointSegment(Vector point, Vector line0, Vector line1)
		{
            //double dista = (point - line0).Dist2;                // square distance: dist2
            //double distb = (point - line1).Dist2;                // square distance: dist2
            //double dist0 = Math.Min(dista, distb);
            //double dist1 = Math.Max(dista, distb);
            //double dist01 = Dist2PointLine(point, line0, line1); // square distance: dist2
            //return HMath.Between(dist0, dist01, dist1);
            Vector closest = ClosestPointOnLine(line0, line1, point, true);
            double dist2 = (point - closest).Dist2;
            return dist2;
		}
        static double DistPointSegment(Vector point, Vector line0, Vector line1)
        {
            double dist2 = Dist2PointSegment(point, line0, line1);
            double dist = Math.Sqrt(dist2);
            return dist;
        }
        public static double DistPointSegment(Vector point, IList<Vector> line)
        {
            double dist2 = double.PositiveInfinity;
            for(int i=1; i<line.Count; i++)
            {
                Vector line0 = line[i-1];
                Vector line1 = line[i  ];
                double ldist2 = Dist2PointSegment(point, line0, line1);
                dist2 = Math.Min(dist2, ldist2);
                HDebug.Assert(dist2 >= 0);
            }
            double dist = Math.Sqrt(dist2);
            HDebug.Assert(double.IsNaN(dist) == false, double.IsInfinity(dist) == false);
            return dist;
        }
        public static List<double> DistsPointsSegment(IList<Vector> points, IList<Vector> line)
        {
            List<double> dists = new List<double>();
            for(int i=0; i<points.Count; i++)
            {
                double dist = DistPointSegment(points[i], line);
                dists.Add(dist);
            }
            return dists;
        }
    }
}
