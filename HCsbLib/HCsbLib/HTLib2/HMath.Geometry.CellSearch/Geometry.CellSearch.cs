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
            private readonly Dictionary<(int x, int y, int z), List<(double x,double y,double z)>> cells;

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
                HDebug.Assert(point != null && point.Length == 3); // "point must be a double array of length 3.");

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
            // Get cells that can contain points within cutoff
            // ============================================================
            private (int dx, int dy, int dz)[] GetSearchCellIndices(double cutoff)
            {
                HDebug.Assert(cutoff > 0);

                int    range   = (int)Math.Ceiling(cutoff / CellSize);
                double cutoff2 = cutoff * cutoff;

                List<(int dx, int dy, int dz)> searchcells = new List<(int dx, int dy, int dz)>();

                for(int dx=-range; dx<=range; dx++)
                for(int dy=-range; dy<=range; dy++)
                for(int dz=-range; dz<=range; dz++)
                {
                    // minimum possible distance between cell (0,0,0)
                    // and cell (dx,dy,dz)

                    double vx = Math.Max(0, Math.Abs(dx)-1) * CellSize;
                    double vy = Math.Max(0, Math.Abs(dy)-1) * CellSize;
                    double vz = Math.Max(0, Math.Abs(dz)-1) * CellSize;

                    double dist2 = vx*vx + vy*vy + vz*vz;

                    if(dist2 <= cutoff2)
                        searchcells.Add((dx,dy,dz));
                }

                return searchcells.ToArray();
            }
            // ============================================================
            // Enumerate all points within cutoff
            // ============================================================
            public IEnumerable<(double x, double y, double z)> Search
                (double[] point, double cutoff)
            {
                HDebug.Assert(point != null && point.Length == 3);
                HDebug.Assert(cutoff > 0);

                var icell = GetCellIndex(point);

                var searchcells = GetSearchCellIndices(cutoff);

                double cutoff2 = cutoff * cutoff;

                foreach(var dcell in searchcells)
                {
                    var cell =
                    (
                        icell.x + dcell.dx,
                        icell.y + dcell.dy,
                        icell.z + dcell.dz
                    );

                    if(cells.TryGetValue(cell, out var cellpoints) == false)
                        continue;

                    foreach(var neighbor in cellpoints)
                    {
                        double dx = neighbor.x - point[0];
                        double dy = neighbor.y - point[1];
                        double dz = neighbor.z - point[2];

                        double dist2 = dx*dx + dy*dy + dz*dz;

                        if(dist2 <= cutoff2)
                            yield return neighbor;
                    }
                }
            }
        }
	}
}
