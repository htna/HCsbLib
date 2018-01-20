using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
		public static double[,] DistPtPt(MatrixByArr points1, MatrixByArr points2)
		{
			HDebug.Assert(points1.RowSize == points2.RowSize);
			double[,] dists = new double[points1.ColSize, points2.ColSize];
			for(int c1=0; c1<points1.ColSize; c1++)
				for(int c2=0; c2<points2.ColSize; c2++)
				{
					double dist2 = 0;
					for(int r=0; r<points1.RowSize; r++)
					{
						double diff = points1[c1, r] - points2[c2, r];
						dist2 += diff*diff;
					}
					dists[c1, c2] = Math.Sqrt(dist2);
				}
			return dists;
		}
		public static double[,] DistPtPt(IList<Vector> points1, IList<Vector> points2)
		{
			HDebug.Assert(points1.Count == points2.Count);
			double[,] dists = new double[points1.Count, points2.Count];
			for(int c1=0; c1<points1.Count; c1++)
				for(int c2=0; c2<points2.Count; c2++)
				{
					Vector pt1 = points1[c1];
					Vector pt2 = points2[c2];
					HDebug.Assert(pt1.Size == pt2.Size);
					double dist2 = LinAlg.VtV(pt1, pt2);
					dists[c1, c2] = Math.Sqrt(dist2);
				}
			return dists;
		}
	}
}
