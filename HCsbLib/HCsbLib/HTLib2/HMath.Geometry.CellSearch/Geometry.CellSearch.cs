using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class GeometryStatic
    {
    }
	public partial class Geometry
	{
        public partial class CellSearch
        {
            public readonly double CellSize;
            private readonly Dictionary<(int x, int y, int z), List<(double,double,double)>> cells;

            private CellSearch(double cellSize)
            {
                HDebug.Exception((cellSize <= 0), "cellSize must be positive.");

                CellSize = cellSize;
                cells = new Dictionary<(int,int,int), List<(double,double,double)>>();
            }

            public static CellSearch FromPoints(double[][] points, double cellSize)
            {
                CellSearch search = new CellSearch(cellSize);
                foreach (double[] point in points)
                    search.Add(point);
                return search;
            }

            private void Add(double[] point)
            {
                HDebug.Assert((point == null || point.Length != 3)); // "point must be a double array of length 3.");

                var cell = GetCellIndex(point);
                if (cells.TryGetValue(cell, out List<(double,double,double)> list) == false)
                {
                    list = new List<(double,double,double)>();
                    cells.Add(cell, list);
                }
                list.Add((point[0], point[1], point[2]));
            }

            private (int x, int y, int z) GetCellIndex(double[] point)
            {
                // Get voxel index
                int x = (int)Math.Floor(point[0] / CellSize);
                int y = (int)Math.Floor(point[1] / CellSize);
                int z = (int)Math.Floor(point[2] / CellSize);

                return (x, y, z);
            }

            // ============================================================
            // Enumerate all points within Cutoff
            // ============================================================
            public IEnumerable<(double x, double y, double z)> Search(double[] point, double cutoff)
            {
                if (point == null || point.Length != 3)
                    throw new ArgumentException(
                        "point must be a double array of length 3.");

                double cutoff2 = cutoff * cutoff;

                var icenter = GetCellIndex(point);
                int range   = (int)Math.Ceiling(cutoff / CellSize);

                for (int dx=-range; dx<=range; dx++)
                {
                    for (int dy=-range; dy<=range; dy++)
                    {
                        for (int dz=-range; dz<=range; dz++)
                        {
                            var cell = ( icenter.x + dx, icenter.y + dy, icenter.z + dz );

                            if (cells.TryGetValue(cell, out List<(double,double,double)> points) == false)
                                continue;

                            foreach ((double x, double y, double z) neighbor in points)
                            {
                                double vx = neighbor.x - point[0];
                                double vy = neighbor.y - point[1];
                                double vz = neighbor.z - point[2];

                                double dist2 =
                                    vx * vx +
                                    vy * vy +
                                    vz * vz;

                                if (dist2 <= cutoff2)
                                    yield return neighbor;
                            }
                        }
                    }
                }
            }
        }
	}
}
