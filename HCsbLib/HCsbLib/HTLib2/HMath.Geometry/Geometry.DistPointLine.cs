using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        static double Dist2PointLine(Vector point, Vector line0, Vector line1)
		{
			/// Squared distance from a point
			/// http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            ///
            /// (line0) x1-----+--------x2 (line1)
            ///                |
            ///                | d
            ///                x0 (point)
            Vector x1 = line0;
            Vector x2 = line1;
            Vector x0 = point;
            double d21 = (x2 - x1).Dist; HDebug.Assert(d21 != 0);
            double d10 = (x1 - x0).Dist;
            double d1021 = LinAlg.DotProd(x1-x0, x2-x1);
			double dist2 = ((d10*d10 * d21*d21) - d1021*d1021)/(d21*d21);
            if((0 > dist2) && (dist2 > -0.0000000001))
            {
                // consider this as the precision problem by numerical error
                dist2 = 0;
            }
			return dist2;
		}
        static double DistPointLine(Vector point, Vector line0, Vector line1)
        {
            double dist2 = Dist2PointLine(point, line0, line1);
            double dist = Math.Sqrt(dist2);
            return dist;
        }
        static double DistPointLine(Vector point, IList<Vector> line)
        {
            double dist2 = double.PositiveInfinity;
            for(int i=1; i<line.Count; i++)
            {
                Vector line0 = line[i-1];
                Vector line1 = line[i  ];
                dist2 = Math.Min(dist2, Dist2PointLine(point, line0, line1));
                HDebug.Assert(dist2 >= 0);
            }
            double dist = Math.Sqrt(dist2);
            HDebug.Assert(double.IsNaN(dist) == false, double.IsInfinity(dist) == false);
            return dist;
        }
        static List<double> DistsPointsLine(IList<Vector> points, IList<Vector> line)
        {
            List<double> dists = new List<double>();
            for(int i=0; i<points.Count; i++)
            {
                double dist = DistPointLine(points[i], line);
                dists.Add(dist);
            }
            return dists;
        }
    }
}
