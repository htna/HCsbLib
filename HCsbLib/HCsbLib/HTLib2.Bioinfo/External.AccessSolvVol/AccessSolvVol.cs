using HTLib2;
using HTLib3;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using static HTLib2.Bioinfo.Gromacs.Top;
using HDebug = HTLib2.HDebug;

namespace PpiDemo;

public static partial class HTLib2Bioinfo
{
    public sealed class SAV
    {
        public const double DefaultSolvAccRadius     = 4.0;
        public const double DefaultSolvAccRadiusEnrgMax = 6.0;
        public const double DefaultProbeRadius       = 1.4;
        public const double DefaultVoxelSize         = 0.1;

        private readonly VoxelGrid voxelGrid;

        public double MaxSolvAccRadius { get; }
        public double ProbeRadius { get; }
        public double VoxelSize { get { return voxelGrid.Spacing; } }

        private SAV(VoxelGrid voxelGrid, double maxSolvAccRadius, double probeRadius)
        {
            this.voxelGrid = voxelGrid;
            ProbeRadius = probeRadius;
            MaxSolvAccRadius = maxSolvAccRadius;
        }

        public static SAV Build
            ( IList<(double[] center, double rad)> center_rad
            , double maxSolvAccRadius, double probeRadius, double voxelSize
            )
        {
            HDebug.Assert(center_rad != null && center_rad.Count > 0); // Supply the complete, nonempty protein atom list.
            HDebug.Assert(double.IsFinite(probeRadius) && probeRadius >= 0); // The probe radius must be finite and nonnegative.
            HDebug.Assert(double.IsFinite(maxSolvAccRadius) && maxSolvAccRadius > 0); // The maximum query radius must be finite and positive.
            HDebug.Assert(double.IsFinite(voxelSize) && voxelSize > 0); // The voxel side length must be finite and positive.

            double[] minimum = { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };
            double[] maximum = { double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity };
            double[] expandedRadii = new double[center_rad.Count];
            for (int i = 0; i < center_rad.Count; i++)
            {
                (double[] center, double rad) = center_rad[i];
                HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
                HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
                expandedRadii[i] = rad + probeRadius;
                HDebug.Assert(double.IsFinite(rad) && rad > 0 && double.IsFinite(expandedRadii[i])); // rad is a positive finite radius in angstroms, with a finite probe-expanded radius.
                double extent = Math.Max(expandedRadii[i], maxSolvAccRadius);
                for (int axis = 0; axis < 3; axis++)
                {
                    minimum[axis] = Math.Min(minimum[axis], center[axis] - extent);
                    maximum[axis] = Math.Max(maximum[axis], center[axis] + extent);
                }
            }

            VoxelGrid grid = new VoxelGrid(minimum, maximum, maxSolvAccRadius, voxelSize);
            for (int i = 0; i < center_rad.Count; i++)
            {
                grid.MarkExcluded(center_rad[i].center, expandedRadii[i]);
            }
            return new SAV(grid, maxSolvAccRadius, probeRadius);
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalculateAtomSAV
            ( IList<(double[] atom, double rad)> proteinAtoms
            , double[] center
            , double solvAccRadius, double probeRadius, double voxelSize
            )
        {
            return IntegrateOverAtomSAV(proteinAtoms, center, solvAccRadius, probeRadius, voxelSize, null);
        }
        private static double IntegrateOverAtomSAV
            ( IList<(double[] atom, double rad)> proteinAtoms
            , double[] center
            , double solvAccRadius, double probeRadius, double voxelSize
            , Func<double[], double[], double> GetValueAt
            )
        {
            HDebug.Assert(proteinAtoms != null); // Supply all occluding atoms; an empty list is allowed.
            HDebug.Assert(double.IsFinite(probeRadius) && probeRadius >= 0); // The probe radius must be finite and nonnegative.
            HDebug.Assert(double.IsFinite(voxelSize) && voxelSize > 0); // The voxel side length must be finite and positive.
            HDebug.Assert(center != null && center.Length == 3); // The query center must contain three coordinates.
            HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Query coordinates must be finite.
            HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer query radius must be finite and positive.

            double cx = center[0], cy = center[1], cz = center[2];
            var nearbyAtoms = new List<(double x, double y, double z, double radiusSquared)>();
            foreach ((double[] atom, double rad) in proteinAtoms)
            {
                HDebug.Assert(atom != null && atom.Length == 3); // Each protein atom must contain three coordinates.
                HDebug.Assert(double.IsFinite(atom[0]) && double.IsFinite(atom[1]) && double.IsFinite(atom[2])); // Protein coordinates must be finite.
                double expandedRadius = rad + probeRadius;
                HDebug.Assert(double.IsFinite(rad) && rad > 0 && double.IsFinite(expandedRadius)); // Atomic radii must be positive and finite, with a finite probe-expanded radius.

                // Shift atoms so the query sphere is at the origin; keep caller arrays unchanged.
                double x = atom[0] - cx, y = atom[1] - cy, z = atom[2] - cz;
                double extent = solvAccRadius + expandedRadius;
                // Skip atoms whose expanded bounding boxes cannot touch the query box.
                if (Math.Abs(x) > extent || Math.Abs(y) > extent || Math.Abs(z) > extent) continue;
                nearbyAtoms.Add((x, y, z, expandedRadius * expandedRadius));
            }

            // All axes include the local origin and symmetric voxel centers on either side.
            double halfCount = Math.Ceiling(solvAccRadius / voxelSize);
            HDebug.Assert(halfCount >= 1 && halfCount < int.MaxValue); // Leave room to increment past the inclusive upper bound without overflow.
            int n = (int)halfCount;

            double radiusSquared = solvAccRadius * solvAccRadius;
            double sum = 0;
            double[] point = GetValueAt == null ? null : new double[3];
            for (int iz = -n; iz <= n; iz++)
            {
                double pz = iz * voxelSize;
                for (int iy = -n; iy <= n; iy++)
                {
                    double py = iy * voxelSize;
                    double yzSquared = py * py + pz * pz;
                    if (yzSquared > radiusSquared) continue;
                    for (int ix = -n; ix <= n; ix++)
                    {
                        double px = ix * voxelSize;
                        if (px * px + yzSquared > radiusSquared) continue;

                        bool excluded = false;
                        foreach (var atom in nearbyAtoms)
                        {
                            double ax = px - atom.x, ay = py - atom.y, az = pz - atom.z;
                            if (ax * ax + (ay * ay + az * az) <= atom.radiusSquared)
                            {
                                excluded = true;
                                break;
                            }
                        }
                        if (excluded) continue;

                        double value = 1;
                        if (GetValueAt != null)
                        {
                            // Restore absolute coordinates and refresh the reused buffer for every call.
                            point[0] = px + cx;
                            point[1] = py + cy;
                            point[2] = pz + cz;
                            value = GetValueAt(center, point);
                            HDebug.Assert(double.IsFinite(value)); // Each sampled function value must be finite.
                        }
                        sum += value;
                    }
                }
            }
            double voxelVolume = voxelSize * voxelSize * voxelSize;
            return sum * voxelVolume;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateAtomSAV(double[] center, double solvAccRadius)
        {
            HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
            HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
            HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer radius must be finite and positive.
            HDebug.Assert(solvAccRadius <= MaxSolvAccRadius); // Build the grid for the largest radius that will be requested.
            HDebug.Assert(voxelGrid.Contains(center, solvAccRadius)); // The query sphere must fit inside the grid.

            double voxelVolume = VoxelSize * VoxelSize * VoxelSize;
            double sum = voxelGrid.SumAccessibleAtom(center, solvAccRadius);
            return sum * voxelVolume;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValType IntegrateOverAtomSAV<ValType>(double[] center, double solvAccRadius, Func<double[], double[], ValType> GetValueAt, Func<ValType> GetZero)
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
            HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
            HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer radius must be finite and positive.
            HDebug.Assert(solvAccRadius <= MaxSolvAccRadius); // Build the grid for the largest radius that will be requested.
            HDebug.Assert(voxelGrid.Contains(center, solvAccRadius)); // The query sphere must fit inside the grid.

            double voxelVolume = VoxelSize * VoxelSize * VoxelSize;
            ValType sum = voxelGrid.SumAccessibleAtom(center, solvAccRadius, GetValueAt, GetZero);
            return sum * voxelVolume;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] CalculateAtomSAVs(IList<(double[] center, double rad)> center_rad, double solvAccRadius, double probeRadius, double voxelSize)
        {
            SAV sav = SAV.Build(center_rad, solvAccRadius, probeRadius, voxelSize);
            return sav.CalculateAtomSAVs(center_rad.Select(sphere => sphere.center).ToArray(), solvAccRadius);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValType[] IntegrateOverAtomSAVs<ValType>
            ( IList<(double[] center, double rad)> center_rad
            , Func<double[], double[], ValType> GetValueAt
            , Func<ValType> GetZero
            , double solvAccRadius, double probeRadius, double voxelSize
            )
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            SAV sav = SAV.Build(center_rad, solvAccRadius, probeRadius, voxelSize);
            return sav.IntegrateOverAtomSAVs(center_rad.Select(sphere => sphere.center).ToArray(), solvAccRadius, GetValueAt, GetZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double[] CalculateAtomSAVs(IList<double[]> centers, double solvAccRadius)
        {
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(centers != null); // Supply the query center list; an empty list is allowed.
                HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer radius must be finite and positive.
                HDebug.Assert(solvAccRadius <= MaxSolvAccRadius); // Build the grid for the largest radius that will be requested.
                HDebug.Assert(centers.Count >= 0);
                foreach(double[] center in centers)
                {
                    HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
                    HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
                    HDebug.Assert(voxelGrid.Contains(center, solvAccRadius)); // Query spheres must fit the grid; use centers from the original protein.
                }
            }

            double voxelVolume = VoxelSize * VoxelSize * VoxelSize;
            double[] values = new double[centers.Count];
            for (int i = 0; i < centers.Count; i++)
            {
                values[i] = voxelGrid.SumAccessibleAtom(centers[i], solvAccRadius) * voxelVolume;
            }
            return values;
        }
        private ValType[] IntegrateOverAtomSAVs<ValType>
            ( IList<double[]> centers
            , double solvAccRadius
            , Func<double[], double[], ValType> GetValueAt
            , Func<ValType> GetZero
            )
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(centers != null); // Supply the query center list; an empty list is allowed.
                HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer radius must be finite and positive.
                HDebug.Assert(solvAccRadius <= MaxSolvAccRadius); // Build the grid for the largest radius that will be requested.
                HDebug.Assert(centers.Count >= 0);
                foreach(double[] center in centers)
                {
                    HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
                    HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
                    HDebug.Assert(voxelGrid.Contains(center, solvAccRadius)); // Query spheres must fit the grid; use centers from the original protein.
                }
            }

            double voxelVolume = VoxelSize * VoxelSize * VoxelSize;
            ValType[] values = new ValType[centers.Count];
            for(int i = 0; i < centers.Count; i++)
            {
                values[i] = voxelGrid.SumAccessibleAtom(centers[i], solvAccRadius, GetValueAt, GetZero) * voxelVolume;
            }
            return values;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateUnionSAV(IList<double[]> centers, double solvAccRadius)
        {
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(centers != null); // Supply the query center list; an empty list is allowed.
                HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer radius must be finite and positive.
                HDebug.Assert(solvAccRadius <= MaxSolvAccRadius); // Build the grid for the largest radius that will be requested.
                HDebug.Assert(centers.Count > 0);
                foreach(double[] center in centers)
                {
                    HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
                    HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
                    HDebug.Assert(voxelGrid.Contains(center, solvAccRadius)); // Query spheres must fit the grid; use centers from the original protein.
                }
            }

            BitArray visited = new BitArray(voxelGrid.VoxelCount);
            double voxelVolume = VoxelSize * VoxelSize * VoxelSize;
            double sum = 0;
            for(int i = 0; i < centers.Count; i++)
            {
                sum += voxelGrid.SumAccessibleUnion(centers[i], solvAccRadius, visited);
            }
            return sum * voxelVolume;
        }
        public ValType IntegrateOverUnionSAV<ValType>(IList<double[]> centers, double solvAccRadius, Func<double[], ValType> GetValueAt, Func<ValType> GetZero)
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            if(HDebug.IsDebuggerAttached)
            {
                HDebug.Assert(GetValueAt != null);
                HDebug.Assert(GetZero    != null);
                HDebug.Assert(centers    != null); // Supply the query center list; an empty list is allowed.
                HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The outer radius must be finite and positive.
                HDebug.Assert(solvAccRadius <= MaxSolvAccRadius); // Build the grid for the largest radius that will be requested.
                HDebug.Assert(centers.Count > 0);
                foreach(double[] center in centers)
                {
                    HDebug.Assert(center != null && center.Length == 3); // Each center must contain three coordinates.
                    HDebug.Assert(double.IsFinite(center[0]) && double.IsFinite(center[1]) && double.IsFinite(center[2])); // Center coordinates must be finite.
                    HDebug.Assert(voxelGrid.Contains(center, solvAccRadius)); // Query spheres must fit the grid; use centers from the original protein.
                }
            }

            BitArray visited = new BitArray(voxelGrid.VoxelCount);
            double voxelVolume = VoxelSize * VoxelSize * VoxelSize;
            ValType sum = GetZero();
            for (int i = 0; i < centers.Count; i++)
            {
                sum += voxelGrid.SumAccessibleUnion(centers[i], solvAccRadius, GetValueAt, GetZero, visited);
            }
            return sum * voxelVolume;
        }

        private sealed class VoxelGrid
        {
            private readonly double voxelSize;
            private readonly double[] x;
            private readonly double[] y;
            private readonly double[] z;
            private readonly byte[] excluded;
            private readonly (int ix, int iy, int iz)[] queryOffsets;

            public double Spacing { get { return voxelSize; } }
            public int VoxelCount { get { return excluded.Length; } }

            public VoxelGrid(double[] minimum, double[] maximum, double maxSolvAccRadius, double voxelSize)
            {
                this.voxelSize = voxelSize;
                queryOffsets = CreateVoxelOffsets(maxSolvAccRadius, voxelSize);
                long[] firstIndices = new long[3];
                int[] sizes = new int[3];
                for (int axis = 0; axis < 3; axis++)
                {
                    // Cover exclusion/query spheres with extra boundary cells for rounding.
                    long first = checked((long)Math.Floor(minimum[axis] / voxelSize) - 1);
                    long last = checked((long)Math.Ceiling(maximum[axis] / voxelSize) + 1);
                    firstIndices[axis] = first;
                    sizes[axis] = checked((int)(last - first + 1));
                    HDebug.Assert(sizes[axis] > 0); // Each grid axis must have a positive, representable length.
                }

                long bytes = checked((long)sizes[0] * sizes[1] * sizes[2]);
                HDebug.Assert(bytes <= 1024L * 1024 * 1024 && bytes <= Array.MaxLength); // Keep the one-byte-per-voxel grid within 1 GiB and the array index limit.
                x = MakeAxis(firstIndices[0], sizes[0]);
                y = MakeAxis(firstIndices[1], sizes[1]);
                z = MakeAxis(firstIndices[2], sizes[2]);
                excluded = new byte[checked((int)bytes)];
            }

            // Candidate offsets relative to the nearest voxel center. The extra half-voxel
            // diagonal covers query centers between grid points; callers check exact distances.
            public static (int ix, int iy, int iz)[] CreateVoxelOffsets(double maxSolvAccRadius, double voxelSize)
            {
                HDebug.Assert(double.IsFinite(maxSolvAccRadius) && maxSolvAccRadius > 0); // The query radius must be finite and positive.
                HDebug.Assert(double.IsFinite(voxelSize) && voxelSize > 0); // The voxel spacing must be finite and positive.
                double extent = maxSolvAccRadius / voxelSize + Math.Sqrt(3) * 0.5;
                HDebug.Assert(double.IsFinite(extent) && extent < int.MaxValue - 1); // Offsets must fit integer indices.
                int maximumOffset = (int)Math.Ceiling(extent);
                double extentSquared = extent * extent;
                List<(int ix, int iy, int iz)> offsets = new List<(int ix, int iy, int iz)>();
                for (int iz = -maximumOffset; iz <= maximumOffset; iz++)
                {
                    for (int iy = -maximumOffset; iy <= maximumOffset; iy++)
                    {
                        for (int ix = -maximumOffset; ix <= maximumOffset; ix++)
                        {
                            double distanceSquared = (double)ix * ix + ((double)iy * iy + (double)iz * iz);
                            if (distanceSquared <= extentSquared) { offsets.Add((ix, iy, iz)); }
                        }
                    }
                }
                return offsets.ToArray();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(double[] center, double radius)
            {
                return center[0] - radius >= x[0] && center[0] + radius <= x[^1]
                    && center[1] - radius >= y[0] && center[1] + radius <= y[^1]
                    && center[2] - radius >= z[0] && center[2] + radius <= z[^1];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private double[] MakeAxis(long firstIndex, int size)
            {
                double[] axis = new double[size];
                for (int i = 0; i < size; i++)
                {
                    // Shared world-origin lattice; works for negative coordinates too.
                    axis[i] = (firstIndex + i + 0.5) * voxelSize;
                }
                return axis;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private (int Low, int High) Bounds(double center, double radius, double[] axis)
            {
                // Include one extra candidate at each end to protect index rounding.
                int low = (int)Math.Clamp(Math.Floor((center - radius - axis[0]) / voxelSize) - 1, 0, axis.Length - 1);
                int high = (int)Math.Clamp(Math.Ceiling((center + radius - axis[0]) / voxelSize) + 1, 0, axis.Length - 1);
                return (low, high);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private (int Low, int High) XSpan(double center, double yzSquared, double radiusSquared)
            {
                double halfWidth = Math.Sqrt(radiusSquared - yzSquared);
                (int low, int high) = Bounds(center, halfWidth, x);
                // Trim using actual voxel-center coordinates. All remaining centers
                // are in this sphere chord; no continuous areas are integrated.
                while (low <= high && Square(x[low] - center) + yzSquared > radiusSquared) { low++; }
                while (high >= low && Square(x[high] - center) + yzSquared > radiusSquared) { high--; }
                return (low, high);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void MarkExcluded(double[] center, double radius)
            {
                double cx = center[0], cy = center[1], cz = center[2];
                double radiusSquared = radius * radius;
                (int y0, int y1) = Bounds(cy, radius, y);
                (int z0, int z1) = Bounds(cz, radius, z);
                for (int iz = z0; iz <= z1; iz++)
                {
                    double dzSquared = Square(z[iz] - cz);
                    for (int iy = y0; iy <= y1; iy++)
                    {
                        double yzSquared = Square(y[iy] - cy) + dzSquared;
                        if (yzSquared > radiusSquared) continue;
                        (int low, int high) = XSpan(cx, yzSquared, radiusSquared);
                        if (low > high) continue;
                        int start = (iz * y.Length + iy) * x.Length + low;
                        // A boundary center is excluded. Overlapping spheres set the
                        // same byte again, so their common volume is not subtracted twice.
                        Array.Fill(excluded, (byte)1, start, high - low + 1);
                    }
                }
            }

            // Whole-set queries use a point-only field and a shared visitation mask.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public double SumAccessibleUnion(double[] center, double solvAccRadius, BitArray visited = null)
            {
                return SumAccessibleAtom(center, solvAccRadius, visited);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValType SumAccessibleUnion<ValType>(double[] center, double solvAccRadius, Func<double[], ValType> GetValueAt, Func<ValType> GetZero, BitArray visited = null)
                where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
            {
                return SumAccessibleAtom(center, solvAccRadius, AtPoint, GetZero, visited);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                ValType AtPoint(double[] queryCenter, double[] point)
                {
                    return GetValueAt(point);
                }
            }

            // Volume-only queries count accessible voxels without evaluating a function.
            public double SumAccessibleAtom(double[] center, double solvAccRadius, BitArray visited = null)
            {
                double cx = center[0], cy = center[1], cz = center[2];
                double radiusSquared = solvAccRadius * solvAccRadius;
                int centerX = (int)Math.Round((cx - x[0]) / voxelSize);
                int centerY = (int)Math.Round((cy - y[0]) / voxelSize);
                int centerZ = (int)Math.Round((cz - z[0]) / voxelSize);
                double sum = 0;
                // One immutable offset array serves every atom and every radius up to
                // MaxSolvAccRadius. Accessibility still depends on the actual center.
                foreach ((int offsetX, int offsetY, int offsetZ) in queryOffsets)
                {
                    int ix = centerX + offsetX; if(ix < 0 || ix >= x.Length) continue;
                    int iy = centerY + offsetY; if(iy < 0 || iy >= y.Length) continue;
                    int iz = centerZ + offsetZ; if(iz < 0 || iz >= z.Length) continue;

                    double distanceSquared = Square(x[ix] - cx) + (Square(y[iy] - cy) + Square(z[iz] - cz));
                    if (distanceSquared > radiusSquared) continue;
                    int index = (iz * y.Length + iy) * x.Length + ix;
                    if (excluded[index] != 0) continue;
                    if (visited != null)
                    {
                        // Molecule queries count each accessible voxel only once.
                        if (visited[index]) continue;
                        visited[index] = true;
                    }
                    sum ++;
                }
                return sum;
            }

            // Atom-wise queries can depend on their query center as well as the sampled point.
            public ValType SumAccessibleAtom<ValType>(double[] center, double solvAccRadius, Func<double[], double[], ValType> GetValueAt, Func<ValType> GetZero, BitArray visited = null)
                where ValType : IAdditionOperators<ValType, ValType, ValType>
            {
                HDebug.Assert(GetValueAt != null);
                HDebug.Assert(GetZero    != null);
                double cx = center[0], cy = center[1], cz = center[2];
                double radiusSquared = solvAccRadius * solvAccRadius;
                int centerX = (int)Math.Round((cx - x[0]) / voxelSize);
                int centerY = (int)Math.Round((cy - y[0]) / voxelSize);
                int centerZ = (int)Math.Round((cz - z[0]) / voxelSize);
                ValType sum = GetZero();
                double[] point = new double[3];
                // One immutable offset array serves every atom and every radius up to
                // MaxSolvAccRadius. Accessibility still depends on the actual center.
                foreach ((int offsetX, int offsetY, int offsetZ) in queryOffsets)
                {
                    int ix = centerX + offsetX; if(ix < 0 || ix >= x.Length) continue;
                    int iy = centerY + offsetY; if(iy < 0 || iy >= y.Length) continue;
                    int iz = centerZ + offsetZ; if(iz < 0 || iz >= z.Length) continue;

                    double distanceSquared = Square(x[ix] - cx) + (Square(y[iy] - cy) + Square(z[iz] - cz));
                    if (distanceSquared > radiusSquared) continue;
                    int index = (iz * y.Length + iy) * x.Length + ix;
                    if (excluded[index] != 0) continue;
                    if (visited != null)
                    {
                        // Molecule queries count each accessible voxel only once.
                        if (visited[index]) continue;
                        visited[index] = true;
                    }
                    // The callback must treat center and point as read-only coordinates.
                    point[0] = x[ix];
                    point[1] = y[iy];
                    point[2] = z[iz];
                    ValType value = GetValueAt(center, point);
                    //HDebug.Assert(double.IsFinite(value)); // Each sampled function value must be finite.
                    sum = sum + value;
                }
                return sum;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static double Square(double value) { return value * value; }
        }
    }
}

public static partial class Facade
{
    /// <summary>Forwards SAV calculations to HTLib2Bioinfo.SAV while preserving the application API.</summary>
    public sealed class SAV
    {
        public const double DefaultSolvAccRadius = HTLib2Bioinfo.SAV.DefaultSolvAccRadius;
        public const double DefaultSolvAccRadiusEnrgMax = HTLib2Bioinfo.SAV.DefaultSolvAccRadiusEnrgMax;
        public const double DefaultProbeRadius = HTLib2Bioinfo.SAV.DefaultProbeRadius;
        public const double DefaultVoxelSize = HTLib2Bioinfo.SAV.DefaultVoxelSize;

        private readonly HTLib2Bioinfo.SAV sav;

        public double MaxSolvAccRadius { get { return sav.MaxSolvAccRadius; } }
        public double ProbeRadius { get { return sav.ProbeRadius; } }
        public double VoxelSize { get { return sav.VoxelSize; } }

        private SAV(HTLib2Bioinfo.SAV sav)
        {
            this.sav = sav;
        }

        public static SAV Build
            ( Chem.Complex complex
            , double maxSolvAccRadius, double probeRadius, double voxelSize
            )
        {
            HDebug.Assert(complex != null); // Supply the complete complex before cropping.
            HDebug.Assert(complex.atoms.Length > 0);

            List<(double[] center, double rad)> proteinAtoms = complex.atoms
                .Select(atom => (new double[] { atom.Feature.X, atom.Feature.Y, atom.Feature.Z }, atom.Feature.VdwRadius)).ToList();
            return new SAV(HTLib2Bioinfo.SAV.Build(proteinAtoms, maxSolvAccRadius, probeRadius, voxelSize));
        }

        public static SAV Build
            ( IList<(double[] center, double rad)> center_rad
            , double maxSolvAccRadius, double probeRadius, double voxelSize
            )
        {
            return new SAV(HTLib2Bioinfo.SAV.Build(center_rad, maxSolvAccRadius, probeRadius, voxelSize));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalculateAtom
            ( IList<(double[] atom, double rad)> proteinAtoms
            , double[] center
            , double solvAccRadius, double probeRadius, double voxelSize
            )
        {
            return HTLib2Bioinfo.SAV.CalculateAtomSAV(proteinAtoms, center, solvAccRadius, probeRadius, voxelSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateAtom(double[] center, double solvAccRadius)
        {
            return sav.CalculateAtomSAV(center, solvAccRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValType IntegrateAtom<ValType>(double[] center, double solvAccRadius, Func<double[], double[], ValType> GetValueAt, Func<ValType> GetZero)
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            return sav.IntegrateOverAtomSAV(center, solvAccRadius, GetValueAt, GetZero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] CalculateAtoms(Chem.Complex complex, double solvAccRadius, double probeRadius, double voxelSize)
        {
            HDebug.Assert(complex != null); // Supply the complete complex before cropping.
            HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The SAV query radius is positive, in angstroms.
            HDebug.Assert(double.IsFinite(voxelSize) && voxelSize > 0); // The voxel side length is positive, in angstroms.
            HDebug.Assert(double.IsFinite(probeRadius) && probeRadius >= 0); // The solvent probe radius is nonnegative.
            HDebug.Assert(complex.atoms.Length > 0);

            List<(double[] center, double rad)> spheres = complex.atoms
                .Select(atom => (new double[] { atom.Feature.X, atom.Feature.Y, atom.Feature.Z }, atom.Feature.VdwRadius)).ToList();
            return HTLib2Bioinfo.SAV.CalculateAtomSAVs(spheres, solvAccRadius, probeRadius, voxelSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValType[] IntegrateAtoms<ValType>
            ( Chem.Complex complex
            , Func<double[], double[], ValType> GetValueAt
            , Func<ValType> GetZero
            , double solvAccRadius, double probeRadius, double voxelSize
            )
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            HDebug.Assert(complex != null); // Supply the complete complex before cropping.
            HDebug.Assert(double.IsFinite(solvAccRadius) && solvAccRadius > 0); // The SAV query radius is positive, in angstroms.
            HDebug.Assert(double.IsFinite(voxelSize) && voxelSize > 0); // The voxel side length is positive, in angstroms.
            HDebug.Assert(double.IsFinite(probeRadius) && probeRadius >= 0); // The solvent probe radius is nonnegative.
            HDebug.Assert(complex.atoms.Length > 0);

            List<(double[] center, double rad)> spheres = complex.atoms
                .Select(atom => (new double[] { atom.Feature.X, atom.Feature.Y, atom.Feature.Z }, atom.Feature.VdwRadius)).ToList();
            return HTLib2Bioinfo.SAV.IntegrateOverAtomSAVs(spheres, GetValueAt, GetZero, solvAccRadius, probeRadius, voxelSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double[] CalculateAtoms(IList<double[]> centers, double solvAccRadius)
        {
            return sav.CalculateAtomSAVs(centers, solvAccRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateMole(IList<double[]> centers, double solvAccRadius)
        {
            return sav.CalculateUnionSAV(centers, solvAccRadius);
        }

        public ValType IntegrateMole<ValType>(IList<double[]> centers, double solvAccRadius, Func<double[], ValType> GetValueAt, Func<ValType> GetZero)
            where ValType : IAdditionOperators<ValType, ValType, ValType>, IMultiplyOperators<ValType, double, ValType>
        {
            return sav.IntegrateOverUnionSAV(centers, solvAccRadius, GetValueAt, GetZero);
        }
    }
}
