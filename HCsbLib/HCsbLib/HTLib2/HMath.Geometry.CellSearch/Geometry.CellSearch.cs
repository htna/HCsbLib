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
            public double cellsize;

            // Absolute cell index corresponding to cells[0,0,0].
            private (int x, int y, int z) basecell;
            private List<(double x, double y, double z)>[,,] cells;

            //private CellSearch
            //    ( double cellSize
            //    , (int x, int y, int z) basecell
            //    , int sizex
            //    , int sizey
            //    , int sizez
            //    )
            //{
            //    HDebug.Exception(cellSize <= 0, "cellSize must be positive.");
            //
            //    CellSize = cellSize;
            //    BaseCell = basecell;
            //    cells    = new List<(double x, double y, double z)>[sizex, sizey, sizez];
            //}

            public static CellSearch FromPoints(double[][] points, double cellSize)
            {
                HDebug.Exception(points == null, "points must not be null.");
                HDebug.Exception(cellSize <= 0, "cellSize must be positive.");

                if(points.Length == 0)
                {
                    return new CellSearch
                    {
                        cellsize = cellSize,
                        basecell = (0,0,0),
                        cells    = new List<(double x, double y, double z)>[0, 0, 0]
                    };
                }

                double minx = double.PositiveInfinity;
                double miny = double.PositiveInfinity;
                double minz = double.PositiveInfinity;
                double maxx = double.NegativeInfinity;
                double maxy = double.NegativeInfinity;
                double maxz = double.NegativeInfinity;

                foreach(double[] point in points)
                {
                    HDebug.Assert(point != null && point.Length == 3);

                    minx = Math.Min(minx, point[0]);
                    miny = Math.Min(miny, point[1]);
                    minz = Math.Min(minz, point[2]);

                    maxx = Math.Max(maxx, point[0]);
                    maxy = Math.Max(maxy, point[1]);
                    maxz = Math.Max(maxz, point[2]);
                }

                // Absolute cell containing the minimum coordinates.
                // This cell is mapped to cells[0,0,0].
                var basecell =
                (
                    x: (int)Math.Floor(minx / cellSize),
                    y: (int)Math.Floor(miny / cellSize),
                    z: (int)Math.Floor(minz / cellSize)
                );

                // Absolute cell containing the maximum coordinates.
                var maxcell =
                (
                    x: (int)Math.Floor(maxx / cellSize),
                    y: (int)Math.Floor(maxy / cellSize),
                    z: (int)Math.Floor(maxz / cellSize)
                );

                int sizex = maxcell.x - basecell.x + 1;
                int sizey = maxcell.y - basecell.y + 1;
                int sizez = maxcell.z - basecell.z + 1;

                CellSearch cellsearch =
                    new CellSearch
                    {
                        cellsize = cellSize,
                        basecell = basecell,
                        cells    = new List<(double x, double y, double z)>[sizex, sizey, sizez]
                    };

                foreach(double[] point in points)
                    cellsearch.Add(point);

                return cellsearch;
            }

            private void Add(double[] point)
            {
                HDebug.Assert(point != null && point.Length == 3);

                var cell = GetCellIndex(point);

                HDebug.Assert
                (
                    0 <= cell.x && cell.x < cells.GetLength(0) &&
                    0 <= cell.y && cell.y < cells.GetLength(1) &&
                    0 <= cell.z && cell.z < cells.GetLength(2)
                );

                List<(double x, double y, double z)> cellpoints = cells[cell.x, cell.y, cell.z];

                if(cellpoints == null)
                {
                    cellpoints = new List<(double x, double y, double z)>();
                    cells[cell.x, cell.y, cell.z] = cellpoints;
                }

                cellpoints.Add( (point[0], point[1], point[2]) );
            }

            // ============================================================
            // Convert Cartesian coordinate into local 3D-array index.
            // ============================================================
            private (int x, int y, int z) GetCellIndex(double[] point)
            {
                HDebug.Assert(point != null && point.Length == 3);
                int x = (int)Math.Floor(point[0] / cellsize) - basecell.x;
                int y = (int)Math.Floor(point[1] / cellsize) - basecell.y;
                int z = (int)Math.Floor(point[2] / cellsize) - basecell.z;
                return (x,y,z);
            }

            // ============================================================
            // Relative cells that can possibly contain a point
            // within cutoff.
            //
            // The search point can be anywhere in cell (0,0,0).
            // Therefore the minimum cell-to-cell distance is used.
            // ============================================================
            private IEnumerable<(int dx, int dy, int dz)> GetSearchCellIndices(double cutoff)
            {
                HDebug.Assert(cutoff > 0);

                // For a relative cell dx:
                // minimum separation = max(0, |dx|-1) * CellSize
                // Therefore the largest necessary |dx| is floor(cutoff / CellSize) + 1.
                int    range   = (int)Math.Floor(cutoff / cellsize) + 1;
                double cutoff2 = cutoff * cutoff;

                List<(int dx, int dy, int dz)> searchcells = new List<(int dx, int dy, int dz)>();

                for(int dx=-range; dx<=range; dx++)
                for(int dy=-range; dy<=range; dy++)
                for(int dz=-range; dz<=range; dz++)
                {
                    double vx = Math.Max(0, Math.Abs(dx)-1) * cellsize;
                    double vy = Math.Max(0, Math.Abs(dy)-1) * cellsize;
                    double vz = Math.Max(0, Math.Abs(dz)-1) * cellsize;

                    double dist2 = vx*vx + vy*vy + vz*vz;

                    if(dist2 <= cutoff2)
                        yield return (dx,dy,dz);
                }
            }


            // ============================================================
            // Enumerate stored points within cutoff.
            // ============================================================

            public IEnumerable<(double x, double y, double z)> Search
                ( double[] point
                , double cutoff
                , Dictionary<double, (int dx, int dy, int dz)[]> buff_cutoff_searchcells = null
                )
            {
                HDebug.Assert(point != null && point.Length == 3);
                HDebug.Assert(cutoff > 0);

                var icell = GetCellIndex(point);

                (int dx, int dy, int dz)[] searchcells;
                if(buff_cutoff_searchcells != null && buff_cutoff_searchcells.ContainsKey(cutoff))
                    searchcells = buff_cutoff_searchcells[cutoff];
                else
                    searchcells = GetSearchCellIndices(cutoff).ToArray();

                double cutoff2 = cutoff * cutoff;

                foreach(var dcell in searchcells)
                {
                    int cellx = icell.x + dcell.dx;
                    int celly = icell.y + dcell.dy;
                    int cellz = icell.z + dcell.dz;

                    // The search point can be outside the bounding box.
                    if(cellx < 0 || cells.GetLength(0) <= cellx) continue;
                    if(celly < 0 || cells.GetLength(1) <= celly) continue;
                    if(cellz < 0 || cells.GetLength(2) <= cellz) continue;

                    List<(double x, double y, double z)> cellpoints = cells[cellx, celly, cellz];

                    if(cellpoints == null)
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
