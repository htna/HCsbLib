using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static List<Vector> PointsFromLine(IList<Vector> line, double pointwisedist)
        {
            List<Vector> points = new List<Vector>();
            for(int i=1; i<line.Count; i++)
            {
                Vector line0 = line[i-1];
                Vector line1 = line[i  ];
                Vector vec01 = line1 - line0;
                double vec01dist = vec01.Dist;
                Vector vec01unit = vec01 / vec01dist;
                for(double t=0; t<vec01dist; t+=pointwisedist)
                {
                    Vector point = line0 + t * vec01unit;
                    points.Add(point);
                }
            }
            return points;
        }
	}
}
