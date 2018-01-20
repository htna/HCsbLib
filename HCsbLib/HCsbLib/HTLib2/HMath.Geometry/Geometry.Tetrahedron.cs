using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        public static Tuple<Vector,Vector,Vector,Vector> Points4TetrahedronEquilateral()
        {
            /// https://en.wikipedia.org/wiki/Tetrahedron
            /// Expressed symmetrically as 4 points on the unit sphere,
            /// centroid at the origin, with lower face level, the vertices are:
            /// v1 = ( sqrt(8 / 9),            0, -1 / 3)
            /// v2 = (-sqrt(2 / 9),  sqrt(2 / 3), -1 / 3)
            /// v3 = (-sqrt(2 / 9), -sqrt(2 / 3), -1 / 3)
            /// v4 = (           0,            0,      1)

            // determine point locations of equilateral tetrahedron
            Vector v1 = new double[3] {                0,  Math.Sqrt(8.0/9), -1.0/3};
            Vector v2 = new double[3] { Math.Sqrt(2.0/3), -Math.Sqrt(2.0/9), -1.0/3};
            Vector v3 = new double[3] {-Math.Sqrt(2.0/3), -Math.Sqrt(2.0/9), -1.0/3};
            Vector v4 = new double[3] {                0,                 0,      1};
            // scale to make each edge's length as 1
            double d12 = (v1 - v2).Dist;
            v1 = v1 / d12;  v2 = v2 / d12;  v3 = v3 / d12;  v4 = v4 / d12;
            // make v3 as origin
            v1 -= v3;
            v2 -= v3;
            v4 -= v3;
            v3 -= v3;
            // check length
            HDebug.AssertTolerance(0.00000001, 1 - (v1 - v2).Dist);
            HDebug.AssertTolerance(0.00000001, 1 - (v1 - v3).Dist);
            HDebug.AssertTolerance(0.00000001, 1 - (v1 - v4).Dist);
            HDebug.AssertTolerance(0.00000001, 1 - (v2 - v3).Dist);
            HDebug.AssertTolerance(0.00000001, 1 - (v2 - v4).Dist);
            HDebug.AssertTolerance(0.00000001, 1 - (v3 - v4).Dist);

            // check height (v1.z == v2.z == v3.z)
            HDebug.AssertTolerance(0.00000001, v1[2] - v2[2]);
            HDebug.AssertTolerance(0.00000001, v1[2] - v2[2]);
            HDebug.AssertTolerance(0.00000001, v1[2] - v3[2]);
            // check others
            HDebug.AssertTolerance(0.00000001, v1[2] - 0); // v1,v2,v3 are on z-plane
            HDebug.AssertTolerance(0.00000001, v2[2] - 0); // v1,v2,v3 are on z-plane
            HDebug.AssertTolerance(0.00000001, v3[2] - 0); // v1,v2,v3 are on z-plane
            HDebug.AssertTolerance(0.00000001, v2[1] - 0); // v2,v3 are on x-axis
            HDebug.AssertTolerance(0.00000001, v3[1] - 0); // v2,v3 are on x-axis
            HDebug.AssertTolerance(0.00000001, v2[0] - 1); // v3 is on (1,0,0)
            ///       v1
            ///      /  \
            ///     /    \
            ///    /  v4  \
            ///   /        \
            /// v3----------v2
            /// 
            return new Tuple<Vector, Vector, Vector, Vector>(v1, v2, v3, v4);
        }
	}
}
